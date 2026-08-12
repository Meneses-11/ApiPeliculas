using Microsoft.AspNetCore.Identity;

namespace ApiPeliculas.Models;

public class UsuarioIdentity : IdentityUser
{
    public string Nombre { get; set; }
}
