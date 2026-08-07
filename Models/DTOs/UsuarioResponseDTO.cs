namespace ApiPeliculas.Models.DTOs;

public class UsuarioResponseDTO
{
    public UsuarioDatosDTO Usuario { get; set; }
    public string Rol { get; set; }
    public string Token { get; set; }
}
