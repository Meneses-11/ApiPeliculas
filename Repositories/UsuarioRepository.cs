using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<UsuarioIdentity> _userMAnager;
    private readonly RoleManager<IdentityRole> _rolManager;
    private readonly IMapper _mapper;

    public UsuarioRepository(AplicationDBContext dBContext, IConfiguration config, UserManager<UsuarioIdentity> userManager, RoleManager<IdentityRole> rolManager, IMapper mapper)
    {
        _dbContext = dBContext;
        claveSecreta = config.GetValue<string>("ApiSettings:Secret");
        _userMAnager = userManager;
        _rolManager = rolManager;
        _mapper = mapper;
    }

    public UsuarioIdentity GetUsuario(string id)
    {
        return _dbContext.UsuarioIdentity.Find(id);
    }

    public ICollection<UsuarioIdentity> GetUsuarios()
    {
        return _dbContext.UsuarioIdentity.OrderBy(x => x.UserName).ToList();
    }

    public bool IsUniqueUser(string username)
    {
        return _dbContext.UsuarioIdentity.FirstOrDefault(usr => usr.UserName == username) == null ? true : false;
    }

    public async Task<UsuarioResponseDTO> Login(UsuarioLoginDTO usuarioLoginDTO)
    {
        //var passwordEncypt = ObtenerMD5(usuarioLoginDTO.Password);
        var usuario = _dbContext.UsuarioIdentity.FirstOrDefault(usr => usr.UserName.ToLower() == usuarioLoginDTO.NombreUsuario.ToLower());
        bool isValid = await _userMAnager.CheckPasswordAsync(usuario, usuarioLoginDTO.Password);

        if(usuario == null || !isValid)
        {
            return new UsuarioResponseDTO()
            {
                Token = "",
                Usuario = null
            };
        }

        var roles = await _userMAnager.GetRolesAsync(usuario);
        var manejadorToken = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(claveSecreta);

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = manejadorToken.CreateToken(tokenDescription);

        UsuarioResponseDTO usuarioResponseDTO = new UsuarioResponseDTO()
        {
            Token = manejadorToken.WriteToken(token),
            Usuario = _mapper.Map<UsuarioDatosDTO>(usuario)
        };

        return usuarioResponseDTO;
    }

    public async Task<UsuarioDatosDTO> Registro(CrearUsuarioDTO crearUsuarioDTO)
    {
        //var passwordEncriptado = ObtenerMD5(crearUsuarioDTO.Password);

        UsuarioIdentity usuario = new UsuarioIdentity()
        {
            UserName = crearUsuarioDTO.NombreUsuario,
            Email = crearUsuarioDTO.NombreUsuario,
            NormalizedUserName = crearUsuarioDTO.NombreUsuario.ToUpper(),
            Nombre = crearUsuarioDTO.Nombre
        };

        var result = await _userMAnager.CreateAsync(usuario, crearUsuarioDTO.Password);

        if (result.Succeeded)
        {
            if (!_rolManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                await _rolManager.CreateAsync(new IdentityRole("Admin"));
                await _rolManager.CreateAsync(new IdentityRole("Registrado"));
            }

            await _userMAnager.AddToRoleAsync(usuario, "Admin");
            var usuarioRetornado = _dbContext.UsuarioIdentity.FirstOrDefault(usr => usr.UserName == usuario.UserName);

            return _mapper.Map<UsuarioDatosDTO>(usuarioRetornado);
        }

        /*_dbContext.Usuario.Add(usuario);
        await _dbContext.SaveChangesAsync();

        usuario.Password = passwordEncriptado;

        return usuario;*/
        return new UsuarioDatosDTO();
    }

    //Metodo para encriptar mediate MD5
    /*public static string ObtenerMD5(string password)
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
    }*/
}
