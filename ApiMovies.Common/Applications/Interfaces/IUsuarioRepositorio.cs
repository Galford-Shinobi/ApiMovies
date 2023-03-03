using ApiMovies.Common.Dtos;
using ApiMovies.Common.Entities;
using ApiMovies.Common.Response;

namespace ApiMovies.Common.Applications.Interfaces
{
    public interface IUsuarioRepositorio
    {
        ICollection<Usuario> GetUsuarios();
        Usuario GetUsuario(int usuarioId);
        bool IsUniqueUser(string usuario);
        Task<GenericResponse<Usuario>> IsUniqueUserAsync(string usuario);
        Task<GenericResponse<Usuario>> GetUsuariosAsync();
        Task<GenericResponse<Usuario>> GetUsuarioAsync(int usuarioId);
        Task<GenericResponse<UsuarioLoginRespuestaDto>> LoginAsync(UsuarioLoginDto usuarioLoginDto);
        Task<GenericResponse<Usuario>> RegistroAsync(UsuarioRegistroDto usuarioRegistroDto);
    }
}
