using ApiMovies.Common.Dtos;
using ApiMovies.Common.Entities;
using AutoMapper;

namespace ApiMovies.PeliculasMappers
{
    public class PeliculasMapper : Profile
    {
        public PeliculasMapper()
        {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Categoria, CrearCategoriaDto>().ReverseMap();
            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<AppUsuario, UsuarioDatosDto>().ReverseMap();
            //CreateMap<Pelicula, CrearPeliculaDto>().ReverseMap();
            //CreateMap<Pelicula, PeliculaUpdateDto>().ReverseMap();
        }
    }
}
