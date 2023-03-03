using ApiMovies.Common.Applications.Interfaces;
using ApiMovies.Common.DataBase;
using ApiMovies.Common.Dtos;
using ApiMovies.Common.Entities;
using ApiMovies.Common.Response;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace ApiMovies.Common.Applications.Implementacion
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private string claveSecreta;
        public UsuarioRepositorio(ApplicationDbContext applicationDbContext, IConfiguration config, UserManager<AppUsuario> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            claveSecreta = _configuration["ApiSettings:Secreta"];
        }

        public AppUsuario GetAppUsuario(string UsuarioId)
        {
            return _applicationDbContext.AppUsuario.FirstOrDefault(u => u.Id == UsuarioId);
        }

        public async Task<GenericResponse<AppUsuario>> GetUsuarioAsync(string AppUsuarioId)
        {
            try
            {
                var Result = await _applicationDbContext.AppUsuario.FirstOrDefaultAsync(u => u.Id.Equals(AppUsuarioId));

                if (Result == null) { return new GenericResponse<AppUsuario> { IsSuccess = false, ErrorMessage = "No Data" }; }

                return new GenericResponse<AppUsuario> { IsSuccess = true, DirectObject = Result, };
            }
            catch (Exception exception)
            {
                return new GenericResponse<AppUsuario> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public ICollection<AppUsuario> GetUsuarios()
        {
            return _applicationDbContext.AppUsuario.OrderBy(u => u.UserName).ToList();
        }

        public async Task<GenericResponse<AppUsuario>> GetUsuariosAsync()
        {
            try
            {
                var ListResult = await _applicationDbContext.AppUsuario.OrderBy(u => u.UserName).ToListAsync();
                if (ListResult == null)
                {
                    return new GenericResponse<AppUsuario> { IsSuccess = false, ErrorMessage = "No Data" };
                }
                return new GenericResponse<AppUsuario> { IsSuccess = true, MyCollection = ListResult };
            }
            catch (Exception exception)
            {
                return new GenericResponse<AppUsuario> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public bool IsUniqueUser(string Usuario)
        {
            var usuariobd = _applicationDbContext.AppUsuario.FirstOrDefault(u => u.UserName == Usuario);
            if (usuariobd == null)
            {
                return true;
            }

            return false;
        }

        public async Task<GenericResponse<AppUsuario>> IsUniqueUserAsync(string Usuario)
        {
            try
            {
                var usuariobd = await _applicationDbContext.AppUsuario.FirstOrDefaultAsync(u => u.UserName == Usuario);
                if (usuariobd == null)
                {
                    return new GenericResponse<AppUsuario> { IsSuccess = true, };
                }

                return new GenericResponse<AppUsuario> { IsSuccess = false, DirectObject = usuariobd}; ;
            }
            catch (Exception ex)
            {
               return new GenericResponse<AppUsuario>{ 
                    IsSuccess = false, ErrorMessage=ex.Message
                };
            }
        }

        public async Task<GenericResponse<UsuarioLoginRespuestaDto>> LoginAsync(UsuarioLoginDto UsuarioLoginDto)
        {
            try
            {
                //var passwordEncriptado = obtenermd5(usuarioLoginDto.Password);

                //var usuario = await _applicationDbContext.Usuario.FirstOrDefaultAsync(
                //    u => u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
                //    && u.Password == passwordEncriptado
                //    );
                var usuario = await _applicationDbContext.AppUsuario.FirstOrDefaultAsync
               (u => u.UserName.ToLower() == UsuarioLoginDto.NombreUsuario.ToLower());

                bool isValid = await _userManager.CheckPasswordAsync(usuario, UsuarioLoginDto.Password);


                //Validamos si el usuario no existe con la combinación de usuario y contraseña correcta
                if (usuario == null || isValid == false)
                {
                    //return null;
                    //return new UsuarioLoginRespuestaDto()
                    //{
                    //    Token = "",
                    //    Usuario = null
                    //};
                    var user = new UsuarioLoginRespuestaDto()
                    {
                        Token = "",
                        Usuario = null
                    };

                    return new GenericResponse<UsuarioLoginRespuestaDto> { IsSuccess = false, DirectObject = user };
                }
                

                //Aquí existe el usuario entonces podemos procesar el login      
                var manejadorToken = new JwtSecurityTokenHandler();
                var SecretKey = _configuration["ApiSettings:Secreta"];
                var Issuer = _configuration["ApiSettings:Issuer"];
                var Audience = _configuration["ApiSettings:Audience"];

                //Aquí existe el usuario entonces podemos procesar el login
                var roles = await _userManager.GetRolesAsync(usuario);

                var JwtSecurityToken = GenerateJWTToken(usuario, roles.FirstOrDefault());

                var key = Encoding.ASCII.GetBytes(claveSecreta);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = manejadorToken.CreateToken(tokenDescriptor);

                //UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
                //{
                //    //Token = manejadorToken.WriteToken(token),
                //    Token = JwtSecurityToken,
                //    Usuario = usuario
                //};

                UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
                {
                    //Token = manejadorToken.WriteToken(token),
                    Token = JwtSecurityToken,
                    Role = roles.FirstOrDefault(),
                    Usuario = _mapper.Map<UsuarioDatosDto>(usuario),
                };

                return new GenericResponse<UsuarioLoginRespuestaDto> { IsSuccess = true, DirectObject = usuarioLoginRespuestaDto };
            }
            catch (Exception exception)
            {
                return new GenericResponse<UsuarioLoginRespuestaDto> { IsSuccess = false, ErrorMessage = exception.InnerException.Message };
            }
        }

        public async Task<GenericResponse<UsuarioDatosDto>> RegistroAsync(UsuarioRegistroDto UsuarioRegistroDto)
        {
            try
            {
                //var passwordEncriptado = obtenermd5(usuarioRegistroDto.Password);
                UsuarioDatosDto datosDto = new UsuarioDatosDto();
                AppUsuario usuario = new AppUsuario()
                {
                    UserName = UsuarioRegistroDto.NombreUsuario,
                    Email = UsuarioRegistroDto.NombreUsuario,
                    NormalizedEmail = UsuarioRegistroDto.NombreUsuario.ToUpper(),
                    FirstName = UsuarioRegistroDto.Nombre,
                    LastName = UsuarioRegistroDto.Apellidos,
                };

                var result = await _userManager.CreateAsync(usuario, UsuarioRegistroDto.Password);

                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("registrado"));
                    }
                    await _userManager.AddToRoleAsync(usuario, "admin");
                    var usuarioRetornado = await _applicationDbContext.AppUsuario
                        .FirstOrDefaultAsync(u => u.UserName == UsuarioRegistroDto.NombreUsuario);
                    //Opción 1
                    //return new UsuarioDatosDto()
                    //{
                    //    ID = usuarioRetornado.Id,
                    //    Username = usuarioRetornado.UserName,
                    //    Nombre = usuarioRetornado.Nombre
                    //};
                    datosDto = _mapper.Map<UsuarioDatosDto>(usuarioRetornado);

                }

                return new GenericResponse<UsuarioDatosDto> { IsSuccess = await GuardarAsync(), DirectObject = datosDto };
            }
            catch (Exception exception)
            {
                return new GenericResponse<UsuarioDatosDto> { IsSuccess = false, ErrorMessage = exception.InnerException.Message };
            }
        }
        private async Task<bool> GuardarAsync()
        {
            return await _applicationDbContext.SaveChangesAsync() >= 0 ? true : false;
        }
        //Método para encriptar contraseña con MD5 se usa tanto en el Acceso como en el Registro
        public static string obtenermd5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;
        }

        public string GenerateJWTToken(AppUsuario userInfo, string Role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApiSettings:Secreta"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim("fullName", $"{Role.ToString()}{" "}{userInfo.FirstName.ToString()}{" "}{userInfo.LastName}"),
                new Claim("FolioNumber",userInfo.UserName),
                new Claim("Role",Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["ApiSettings:Issuer"],
                audience: _configuration["ApiSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
