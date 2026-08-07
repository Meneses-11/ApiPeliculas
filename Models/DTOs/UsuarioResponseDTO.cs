namespace ApiPeliculas.Models.DTOs;

public class UsuarioResponseDTO
{
    public Usuario Usuario { get; set; }
    public string Rol { get; set; }
    public string Token { get; set; }
}
