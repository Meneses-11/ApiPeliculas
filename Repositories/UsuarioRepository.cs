using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repositories.IRepositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiPeliculas.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private AplicationDBContext _dbContext;
    private string claveSecreta;
    public UsuarioRepository(AplicationDBContext dBContext, IConfiguration config)
    {
        _dbContext = dBContext;
        claveSecreta = config.GetValue<string>("ApiSettings:Secret");
    }

    public Usuario GetUsuario(int id)
    {
        return _dbContext.Usuario.Find(id);
    }

    public ICollection<Usuario> GetUsuarios()
    {
        return _dbContext.Usuario.OrderBy(x => x.NombreUsuario).ToList();
    }

    public bool IsUniqueUser(string username)
    {
        return _dbContext.Usuario.FirstOrDefault(usr => usr.NombreUsuario == username) == null ? true : false;
    }

    public async Task<UsuarioResponseDTO> Login(UsuarioLoginDTO usuarioLoginDTO)
    {
        var passwordEncypt = ObtenerMD5(usuarioLoginDTO.Password);
        var usuario = _dbContext.Usuario.FirstOrDefault(usr => usr.NombreUsuario.ToLower() == usuarioLoginDTO.NombreUsuario.ToLower() && usr.Password == passwordEncypt);
        
        if(usuario == null)
        {
            return new UsuarioResponseDTO()
            {
                Token = "",
                Usuario = null
            };
        }

        var manejadorToken = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(claveSecreta);

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = manejadorToken.CreateToken(tokenDescription);

        UsuarioResponseDTO usuarioResponseDTO = new UsuarioResponseDTO()
        {
            Token = manejadorToken.WriteToken(token),
            Usuario = usuario
        };

        return usuarioResponseDTO;
    }

    public async Task<Usuario> Registro(CrearUsuarioDTO crearUsuarioDTO)
    {
        var passwordEncriptado = ObtenerMD5(crearUsuarioDTO.Password);

        Usuario usuario = new Usuario()
        {
            NombreUsuario = crearUsuarioDTO.NombreUsuario,
            Password = passwordEncriptado,
            Nombre = crearUsuarioDTO.Nombre,
            Rol = crearUsuarioDTO.Rol
        };

        _dbContext.Usuario.Add(usuario);
        await _dbContext.SaveChangesAsync();

        usuario.Password = passwordEncriptado;

        return usuario;
    }

    //Metodo para encriptar mediate MD5
    public static string ObtenerMD5(string password)
    {
        MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(password);
        data = x.ComputeHash(data);
        string resp = ""; 
        for(int i=0; i<data.Length; i++)
        {
            resp += data[i].ToString("x2").ToLower();
        }
        return resp;
    }
}
