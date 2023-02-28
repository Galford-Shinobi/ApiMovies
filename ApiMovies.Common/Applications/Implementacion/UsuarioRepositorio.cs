using ApiMovies.Common.Applications.Interfaces;
using ApiMovies.Common.DataBase;
using ApiMovies.Common.Dtos;
using ApiMovies.Common.Entities;
using ApiMovies.Common.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XAct.Users;
using XSystem.Security.Cryptography;

namespace ApiMovies.Common.Applications.Implementacion
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        private string claveSecreta;
        public UsuarioRepositorio(ApplicationDbContext applicationDbContext, IConfiguration config)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = config;
            claveSecreta = _configuration["ApiSettings:Secreta"];
        }

        public Usuario GetUsuario(int usuarioId)
        {
            return _applicationDbContext.Usuario.FirstOrDefault(u => u.Id == usuarioId);
        }

        public async Task<GenericResponse<Usuario>> GetUsuarioAsync(int usuarioId)
        {
            try
            {
                var Result = await _applicationDbContext.Usuario.FirstOrDefaultAsync(u => u.Password.Equals(usuarioId));

                if (Result == null) { return new GenericResponse<Usuario> { IsSuccess = false, ErrorMessage="No Data"}; }

                return new GenericResponse<Usuario> {IsSuccess = true, DirectObject = Result, };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Usuario>{ IsSuccess = false, ErrorMessage = exception.Message};
            }
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _applicationDbContext.Usuario.OrderBy(u => u.NombreUsuario).ToList();
        }

        public async Task<GenericResponse<Usuario>> GetUsuariosAsync()
        {
            try
            {
                var ListResult = await _applicationDbContext.Usuario.OrderBy(u => u.NombreUsuario).ToListAsync();
                if (ListResult == null)
                {
                    return new GenericResponse<Usuario> { IsSuccess = false, ErrorMessage = "No Data" };
                }
                return new GenericResponse<Usuario> { IsSuccess = true,MyCollection = ListResult };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Usuario> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public bool IsUniqueUser(string usuario)
        {
             var usuariobd = _applicationDbContext.Usuario.FirstOrDefault(u => u.NombreUsuario == usuario);
            if (usuariobd == null)
            {
                return true;
            }

            return false;
        }

        public async Task<GenericResponse<UsuarioLoginRespuestaDto>> LoginAsync(UsuarioLoginDto usuarioLoginDto)
        {
            try
            {
                var passwordEncriptado = obtenermd5(usuarioLoginDto.Password);

                var usuario = await _applicationDbContext.Usuario.FirstOrDefaultAsync(
                    u => u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
                    && u.Password == passwordEncriptado
                    );

                //Validamos si el usuario no existe con la combinación de usuario y contraseña correcta
                if (usuario == null)
                {
                    var user= new UsuarioLoginRespuestaDto()
                    {
                        Token = "",
                        Usuario = null
                    };

                    return new GenericResponse<UsuarioLoginRespuestaDto> { IsSuccess = false, DirectObject = user};
                }

                //Aquí existe el usuario entonces podemos procesar el login      
                var manejadorToken = new JwtSecurityTokenHandler();
                var SecretKey = _configuration["ApiSettings:Secreta"];
                var Issuer = _configuration["ApiSettings:Issuer"];
                var Audience = _configuration["ApiSettings:Audience"];
                
                var JwtSecurityToken = GenerateJWTToken(usuario);
                
                var key = Encoding.ASCII.GetBytes(claveSecreta);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                        new Claim(ClaimTypes.Role, usuario.Role)
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = manejadorToken.CreateToken(tokenDescriptor);

                UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
                {
                    Token = manejadorToken.WriteToken(token),
                    Usuario = usuario
                };

                return new GenericResponse<UsuarioLoginRespuestaDto> { IsSuccess = true, DirectObject = usuarioLoginRespuestaDto };
            }
            catch (Exception exception)
            {
                return new GenericResponse<UsuarioLoginRespuestaDto> { IsSuccess = false, ErrorMessage = exception.InnerException.Message };
            }
        }

        public async Task<GenericResponse<Usuario>> RegistroAsync(UsuarioRegistroDto usuarioRegistroDto)
        {
            try
            {
                var passwordEncriptado = obtenermd5(usuarioRegistroDto.Password);

                Usuario usuario = new Usuario(){
                    NombreUsuario = usuarioRegistroDto.NombreUsuario,
                    Password = passwordEncriptado,
                    Nombre = usuarioRegistroDto.Nombre,
                    Role = usuarioRegistroDto.Role
                };

                _applicationDbContext.Usuario.Add(usuario);
               
                usuario.Password = passwordEncriptado;
                
                return new GenericResponse<Usuario> { IsSuccess= await GuardarAsync(), DirectObject = usuario };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Usuario> { IsSuccess = false,ErrorMessage =exception.InnerException.Message };
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

        public string GenerateJWTToken(Usuario userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApiSettings:Secreta"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.NombreUsuario),
                new Claim("fullName", $"{userInfo.Role.ToString()}{" "}{userInfo.Nombre.ToString()}{" "}{userInfo.NombreUsuario}"),
                new Claim("FolioNumber",userInfo.NombreUsuario),
                new Claim("Role",userInfo.Role),
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
