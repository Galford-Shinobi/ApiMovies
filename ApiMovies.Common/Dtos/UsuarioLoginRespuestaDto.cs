using ApiMovies.Common.Entities;

namespace ApiMovies.Common.Dtos
{
    public class UsuarioLoginRespuestaDto
    {
        public Usuario Usuario { get; set; }
        public string Token { get; set; }
    }
}
