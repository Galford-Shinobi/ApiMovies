using ApiMovies.Common.Dtos;
using ApiMovies.Common.Entities;
using ApiMovies.Common.Response;

namespace ApiMovies.Common.Applications.Interfaces
{
    public interface IUsuarioRepositorio
    {
        ICollection<AppUsuario> GetUsuarios();
        AppUsuario GetAppUsuario(string UsuarioId);
        bool IsUniqueUser(string Usuario);
        Task<GenericResponse<AppUsuario>> IsUniqueUserAsync(string Usuario);
        Task<GenericResponse<AppUsuario>> GetUsuariosAsync();
        Task<GenericResponse<AppUsuario>> GetUsuarioAsync(string AppUsuarioId);
        Task<GenericResponse<UsuarioLoginRespuestaDto>> LoginAsync(UsuarioLoginDto UsuarioLoginDto);
        Task<GenericResponse<UsuarioDatosDto>> RegistroAsync(UsuarioRegistroDto UsuarioRegistroDto);
    }
}
