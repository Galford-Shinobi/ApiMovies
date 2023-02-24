using ApiMovies.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiMovies.Common.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}
        //Agregar los modelos aquí
        public DbSet<Categoria> Categoria { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
