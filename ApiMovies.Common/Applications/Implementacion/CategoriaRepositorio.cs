using ApiMovies.Common.Applications.Interfaces;
using ApiMovies.Common.DataBase;
using ApiMovies.Common.Entities;
using ApiMovies.Common.Response;
using Microsoft.EntityFrameworkCore;

namespace ApiMovies.Common.Applications.Implementacion
{
    internal class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CategoriaRepositorio(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<GenericResponse<Categoria>> ActualizarCategoriaAsync(Categoria categoria)
        {
            try
            {
                categoria.FechaCreacion = DateTime.Now;
                _applicationDbContext.Categoria.Update(categoria);
               
                return new GenericResponse<Categoria> { IsSuccess = await GuardarAsync(),DirectObject = categoria };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return new GenericResponse<Categoria>
                    {
                        IsSuccess = false,
                        Message = "Ya existe en el sistema verifique los datos!",
                    };
                }
                else
                {
                    return new GenericResponse<Categoria>
                    {
                        IsSuccess = false,
                        Message = dbUpdateException.InnerException.Message,
                    };
                }
            }
            catch (Exception exception)
            {
                return new GenericResponse<Categoria> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Categoria>> BorrarCategoriaAsync(Categoria categoria)
        {
            try
            {
                _applicationDbContext.Categoria.Remove(categoria);

                return new GenericResponse<Categoria> { IsSuccess = await GuardarAsync(), };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return new GenericResponse<Categoria>
                    {
                        IsSuccess = false,
                        Message = "Ya existe en el sistema verifique los datos!",
                    };
                }
                else
                {
                    return new GenericResponse<Categoria>
                    {
                        IsSuccess = false,
                        Message = dbUpdateException.InnerException.Message,
                    };
                }
            }
            catch (Exception exception)
            {
                return new GenericResponse<Categoria> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Categoria>> CrearCategoriaAsync(Categoria categoria)
        {
            try
            {
                categoria.FechaCreacion = DateTime.Now;
                _applicationDbContext.Categoria.Add(categoria);
                return new GenericResponse<Categoria> { IsSuccess = await GuardarAsync(), };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return new GenericResponse<Categoria>
                    {
                        IsSuccess = false,
                        Message = "Ya existe en el sistema verifique los datos!",
                    };
                }
                else
                {
                    return new GenericResponse<Categoria>
                    {
                        IsSuccess = false,
                        Message = dbUpdateException.InnerException.Message,
                    };
                }
            }
            catch (Exception exception)
            {
                return new GenericResponse<Categoria> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Categoria>> ExisteCategoriaAsync(string nombre)
        {
            try
            {
                bool valor = await _applicationDbContext.Categoria.AnyAsync(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
                return new GenericResponse<Categoria> { IsSuccess = valor, };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Categoria> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Categoria>> ExisteCategoriaAsync(int id)
        {
            try
            {
                bool valor = await _applicationDbContext.Categoria.AnyAsync(c => c.Id == id);
                return new GenericResponse<Categoria> { IsSuccess = valor, };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Categoria> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Categoria>> GetCategoriaAsync(int categorId)
        {
            try
            {
                bool valor = await _applicationDbContext.Categoria.AnyAsync(c => c.Id == categorId);

                if (!valor)
                    return new GenericResponse<Categoria> { IsSuccess = false, ErrorMessage = "no hay informacion en el sistema" };


                var result = await _applicationDbContext.Categoria.FirstOrDefaultAsync(c => c.Id == categorId);
                return new GenericResponse<Categoria> { IsSuccess = true,DirectObject = result };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Categoria> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<GenericResponse<Categoria>> GetCategoriasAsync()
        {
            try
            {
                var ListResult = await _applicationDbContext.Categoria.OrderBy(c => c.Nombre).ToListAsync();

                return new GenericResponse<Categoria> { IsSuccess = true, MyCollection = ListResult };
            }
            catch (Exception exception)
            {
                return new GenericResponse<Categoria> { IsSuccess = false, ErrorMessage = exception.Message };
            }
        }

        public async Task<bool> GuardarAsync()
        {
            return await _applicationDbContext.SaveChangesAsync() >= 0 ? true : false;
        }
    }
}
