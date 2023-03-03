using Microsoft.AspNetCore.Identity;

namespace ApiMovies.Common.Entities
{
    public class AppUsuario : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
