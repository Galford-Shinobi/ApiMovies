using ApiMovies.Common.Applications.Interfaces;
using ApiMovies.Common.DataBase;
using ApiMovies.Common.Entities;
using ApiMovies.Common.Response;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiMovies.Common.Applications.Implementacion
{
    public class PeliculaRepositorio : IPeliculaRepositorio
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public PeliculaRepositorio(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<GenericResponse<Pelicula>> ActualizarPeliculaAsync(Pelicula pelicula)
        {
            try
            {
                pelicula.FechaCreacion = DateTime.Now;
                _applicationDbContext.Pelicula.Update(pelicula);
                
                return new GenericResponse<Pelicula> { IsSuccess = await GuardarAsync(), };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return new GenericResponse<Pelicula>
                    {
                        IsSuccess = false,
                        Message = "Ya existe en el sistema verifique los datos!",
                    };
                }
                else
                {
                    return new GenericResponse<Pelicula>
                    {
                        IsSuccess = false,
                        Message = dbUpdateException.InnerException.Message,
                    };
                }
            }
            catch (Exception exception)
            {
                return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Pelicula>> BorrarPeliculaAsync(Pelicula pelicula)
        {
            try
            {
                _applicationDbContext.Pelicula.Remove(pelicula);
                return new GenericResponse<Pelicula> { IsSuccess = await GuardarAsync(), };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public ICollection<Pelicula> BuscarPelicula(string nombre)
        {
            IQueryable<Pelicula> query = _applicationDbContext.Pelicula;

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre));
            }
            return query.ToList();
        }

        public async Task<GenericResponse<Pelicula>> BuscarPeliculaAsync(string nombre)
        {
            try
            {
                IQueryable<Pelicula> query = _applicationDbContext.Pelicula;

                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre));
                }

                return new GenericResponse<Pelicula> { IsSuccess = true,MyCollection = await query.ToListAsync() };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Pelicula>> CrearPeliculaAsync(Pelicula pelicula)
        {
            try
            {
                pelicula.FechaCreacion = DateTime.Now;
                _applicationDbContext.Pelicula.Add(pelicula);
                return new GenericResponse<Pelicula> { IsSuccess = await GuardarAsync(), };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return new GenericResponse<Pelicula>
                    {
                        IsSuccess = false,
                        Message = "Ya existe en el sistema verifique los datos!",
                    };
                }
                else
                {
                    return new GenericResponse<Pelicula>
                    {
                        IsSuccess = false,
                        Message = dbUpdateException.InnerException.Message,
                    };
                }
            }
            catch (Exception exception)
            {
                return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Pelicula>> ExistePeliculaAsync(string nombre)
        {
            try
            {
                bool valor = await _applicationDbContext.Pelicula.AnyAsync(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
                return new GenericResponse<Pelicula> { IsSuccess = valor, };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Pelicula>> ExistePeliculaAsync(int id)
        {
            try
            {
                bool valor = await _applicationDbContext.Pelicula.AnyAsync(c => c.Id==id);
                return new GenericResponse<Pelicula> { IsSuccess = valor, };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Pelicula>> GetAllPeliculasAsync()
        {
            try
            {
               var movie = await _applicationDbContext.Pelicula.OrderBy(c => c.Nombre).ToListAsync();

                if (movie is null)
                {
                    return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = "Not Found" };
                }
            
                return new GenericResponse<Pelicula> { IsSuccess = true, ListObjet = movie };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Pelicula>> GetPeliculaAsync(int peliculaId)
        {
            try
            {
                var movie = await _applicationDbContext.Pelicula.FirstOrDefaultAsync(c => c.Id == peliculaId);
                if (movie is null)
                {
                    return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = "Not Found" };
                }
                return new GenericResponse<Pelicula> { IsSuccess = true, DirectObject = movie,};
            }
            catch (Exception exception)
            {
                return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public ICollection<Pelicula> GetPeliculas()
        {
          return  _applicationDbContext.Pelicula.OrderBy(c => c.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculasEnCategoria(int catId)
        {
            return   _applicationDbContext.Pelicula.Include(ca => ca.Categoria).Where(ca => ca.categoriaId == catId).ToList();
        }

        public async Task<GenericResponse<Pelicula>> GetPeliculasEnCategoriaAsync(int catId)
        {
            try
            {
              var ListMovie = await  _applicationDbContext.Pelicula.Include(ca => ca.Categoria).Where(ca => ca.categoriaId == catId).ToListAsync();
                if (ListMovie is null)
                {
                    return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = "Not Found" };
                }
                return new GenericResponse<Pelicula> { IsSuccess = true, ListObjet = ListMovie };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Pelicula> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<bool> GuardarAsync()
        {
            return await _applicationDbContext.SaveChangesAsync() >= 0 ? true : false;
        }
    }
}
