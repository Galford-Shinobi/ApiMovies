using ApiMovies.Common.Applications.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiMovies.Common.Applications.Implementacion
{
    public static class ServiceExtensions
    {
        public static void AddApplication(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
            Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
        }
    }
}
