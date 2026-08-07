using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;

namespace ApiPeliculas.Repositories.IRepositories;

public interface IUsuarioRepository
{
    ICollection<Usuario> GetUsuarios();
    Usuario GetUsuario(int id);
    bool IsUniqueUser(string username);
    Task<UsuarioResponseDTO> Login(UsuarioLoginDTO usuarioLoginDTO);
    Task<Usuario> Registro(CrearUsuarioDTO crearUsuarioDTO);
}
