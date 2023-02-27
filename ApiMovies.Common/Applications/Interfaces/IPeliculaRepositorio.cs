using ApiMovies.Common.Entities;
using ApiMovies.Common.Response;

namespace ApiMovies.Common.Applications.Interfaces
{
    public interface IPeliculaRepositorio

    {
        ICollection<Pelicula> GetPeliculas();
        Task<GenericResponse<Pelicula>> GetAllPeliculasAsync();
        Task<GenericResponse<Pelicula>> GetPeliculaAsync(int peliculaId);
        Task<GenericResponse<Pelicula>> ExistePeliculaAsync(string nombre);
        Task<GenericResponse<Pelicula>> ExistePeliculaAsync(int id);
        Task<GenericResponse<Pelicula>> CrearPeliculaAsync(Pelicula pelicula);
        Task<GenericResponse<Pelicula>> ActualizarPeliculaAsync(Pelicula pelicula);
        Task<GenericResponse<Pelicula>> BorrarPeliculaAsync(Pelicula pelicula);

        //Métodos para buscar pelicualas en categoría y buscar película por nombre
        ICollection<Pelicula> GetPeliculasEnCategoria(int catId);
        ICollection<Pelicula> BuscarPelicula(string nombre);

        Task<GenericResponse<Pelicula>> GetPeliculasEnCategoriaAsync(int catId);
        Task<GenericResponse<Pelicula>> BuscarPeliculaAsync(string nombre);

        Task<bool> GuardarAsync();
    }
}
