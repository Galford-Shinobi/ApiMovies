using ApiMovies.Common.Entities;

namespace ApiMovies.Common.Dtos
{
    public class UsuarioLoginRespuestaDto
    {
        public UsuarioDatosDto Usuario { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
