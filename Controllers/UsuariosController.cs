using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repositories.IRepositories;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiPeliculas.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    [ApiVersionNeutral]
    //[ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        protected RespuestaAPI _respuestaAPI;
        private readonly IMapper _maper;

        public UsuariosController(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _maper = mapper;
            _respuestaAPI = new RespuestaAPI();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet()]
        [ResponseCache(CacheProfileName = "Global30Cache")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _usuarioRepository.GetUsuarios();

            var listaUsuariosDTO = new List<UsuarioDTO>();

            foreach (var user in listaUsuarios)
            {
                listaUsuariosDTO.Add(_maper.Map<UsuarioDTO>(user));
            }

            return Ok(listaUsuariosDTO);
        }

        [HttpGet("{id:int}", Name = "GetUsuario")]
        [ResponseCache(CacheProfileName = "Global30Cache")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUsuario(int id)
        {
            var usuario = _usuarioRepository.GetUsuario(id);

            if (usuario == null)
                return NotFound();

            return Ok(_maper.Map<UsuarioDTO>(usuario));
        }

        [AllowAnonymous]
        [HttpPost("Registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistroUsuario([FromBody] CrearUsuarioDTO crearUsuarioDTO)
        {
            bool usuarioUnico = _usuarioRepository.IsUniqueUser(crearUsuarioDTO.NombreUsuario);

            if (!usuarioUnico)
            {
                _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestaAPI.IsSuccess = false;
                _respuestaAPI.ErrorMessage.Add("El nombre de usuario ya existe");
                return BadRequest(_respuestaAPI);
            }

            var usuario = await _usuarioRepository.Registro(crearUsuarioDTO);

            if(usuario == null)
            {
                _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestaAPI.IsSuccess = false;
                _respuestaAPI.ErrorMessage.Add("Error en el registro");
                return BadRequest(_respuestaAPI);
            }

            _respuestaAPI.StatusCode = HttpStatusCode.Created;
            _respuestaAPI.IsSuccess = true;
            return Ok(_respuestaAPI);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUsuario([FromBody] UsuarioLoginDTO usuarioLoginDTO)
        {
            var respuestaLogin = await _usuarioRepository.Login(usuarioLoginDTO);

            if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
            {
                _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestaAPI.IsSuccess = false;
                _respuestaAPI.ErrorMessage.Add("El nombre de usuario o password son incorrectos");
                return BadRequest(_respuestaAPI);
            }

            _respuestaAPI.StatusCode = HttpStatusCode.OK;
            _respuestaAPI.IsSuccess = true;
            _respuestaAPI.Result = respuestaLogin;
            return Ok(_respuestaAPI);
        }
    }
}
