using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;

namespace ApiPeliculas.Repositories.IRepositories;

public interface IUsuarioRepository
{
    ICollection<UsuarioIdentity> GetUsuarios();
    Task<UsuarioIdentity> GetUsuario(string id);
    bool IsUniqueUser(string username);
    Task<UsuarioResponseDTO> Login(UsuarioLoginDTO usuarioLoginDTO);
    Task<UsuarioDatosDTO> Registro(CrearUsuarioDTO crearUsuarioDTO);
    Task<bool> DeleteUsuario(UsuarioIdentity usuario);
    Task<bool> ExisteUsuario(string id);
}
