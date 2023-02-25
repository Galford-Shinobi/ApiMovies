using ApiMovies.Common.Entities;
using ApiMovies.Common.Response;

namespace ApiMovies.Common.Applications.Interfaces
{
    public interface ICategoriaRepositorio
    {
        Task<GenericResponse<Categoria>> GetCategoriasAsync();
        Task<GenericResponse<Categoria>> GetCategoriaAsync(int categorId);
        Task<GenericResponse<Categoria>> ExisteCategoriaAsync(string nombre);
        Task<GenericResponse<Categoria>> ExisteCategoriaAsync(int id);
        Task<GenericResponse<Categoria>> CrearCategoriaAsync(Categoria categoria);
        Task<GenericResponse<Categoria>> ActualizarCategoriaAsync(Categoria categoria);
        Task<GenericResponse<Categoria>> BorrarCategoriaAsync(Categoria categoria);
        Task<bool> GuardarAsync();
    }
}
