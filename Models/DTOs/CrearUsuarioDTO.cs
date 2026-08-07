using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.DTOs;

public class CrearUsuarioDTO
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    public string NombreUsuario { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "La contraseña es obligatoria")]
    public string Password { get; set; }
}
