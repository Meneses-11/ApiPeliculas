using System.Net;
using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repositories.IRepositories;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    [ApiVersionNeutral]
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var listaUsuarios = await _usuarioRepository.GetUsuarios();

                var listaUsuariosDTO = new List<UsuarioDTO>();

                listaUsuariosDTO = _maper.Map<List<UsuarioDTO>>(listaUsuarios);

                _respuestaAPI.IsSuccess = true;
                _respuestaAPI.StatusCode = HttpStatusCode.OK;
                _respuestaAPI.Result = listaUsuariosDTO;
                return Ok(_respuestaAPI);
            }
            catch (Exception ex)
            {
                _respuestaAPI.IsSuccess = false;
                _respuestaAPI.ErrorMessage.Add("Ocurrió un Error interno en el servidor");
                _respuestaAPI.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, _respuestaAPI);
            }
        }

        [HttpGet("{id}", Name = "GetUsuario")]
        [ResponseCache(CacheProfileName = "Global30Cache")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsuario(string id)
        {
            try 
            { 
                var usuario = await _usuarioRepository.GetUsuario(id);

                if (usuario == null)
                {
                    _respuestaAPI.IsSuccess = false;
                    _respuestaAPI.StatusCode = HttpStatusCode.NotFound;
                    _respuestaAPI.ErrorMessage.Add("El usuario no Existe");
                    return NotFound(_respuestaAPI);
                }

                _respuestaAPI.IsSuccess = true;
                _respuestaAPI.StatusCode = HttpStatusCode.OK;
                _respuestaAPI.Result = _maper.Map<UsuarioDTO>(usuario);
                return Ok(_respuestaAPI);
            }
            catch (Exception ex)
            {
                _respuestaAPI.IsSuccess = false;
                _respuestaAPI.ErrorMessage.Add("Ocurrió un Error interno en el servidor");
                _respuestaAPI.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, _respuestaAPI);
            }
        }

        [AllowAnonymous]
        [HttpPost("Registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistroUsuario([FromBody] UsuarioCrearDTO crearUsuarioDTO)
        {
            try 
            { 
                bool usuarioUnico = await _usuarioRepository.IsUniqueUser(crearUsuarioDTO.NombreUsuario);

                if (!usuarioUnico)
                {
                    _respuestaAPI.IsSuccess = false;
                    _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                    _respuestaAPI.ErrorMessage.Add("El nombre de usuario ya existe");
                    return BadRequest(_respuestaAPI);
                }

                var usuario = await _usuarioRepository.Registro(crearUsuarioDTO);

                if(usuario == null)
                {
                    _respuestaAPI.IsSuccess = false;
                    _respuestaAPI.StatusCode = HttpStatusCode.InternalServerError;
                    _respuestaAPI.ErrorMessage.Add("Error en el registro");
                    return StatusCode(500, _respuestaAPI);
                }

                _respuestaAPI.StatusCode = HttpStatusCode.Created;
                _respuestaAPI.IsSuccess = true;
                _respuestaAPI.Result = usuario;

                return CreatedAtAction(nameof(GetUsuario), new {id = usuario.Id}, _respuestaAPI);
            }
            catch (Exception ex)
            {
                _respuestaAPI.IsSuccess = false;
                _respuestaAPI.ErrorMessage.Add("Ocurrió un Error interno en el servidor");
                _respuestaAPI.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, _respuestaAPI);
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUsuario([FromBody] UsuarioLoginDTO usuarioLoginDTO)
        {
            try
            {
                var respuestaLogin = await _usuarioRepository.Login(usuarioLoginDTO);

                if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
                {
                    _respuestaAPI.IsSuccess = false;
                    _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                    _respuestaAPI.ErrorMessage.Add("El nombre de usuario o password son incorrectos");
                    return BadRequest(_respuestaAPI);
                }

                _respuestaAPI.IsSuccess = true;
                _respuestaAPI.StatusCode = HttpStatusCode.OK;
                _respuestaAPI.Result = respuestaLogin;
                return Ok(_respuestaAPI);
            }
            catch (Exception ex)
            {
                _respuestaAPI.IsSuccess = false;
                _respuestaAPI.ErrorMessage.Add("Ocurrió un Error interno en el servidor");
                _respuestaAPI.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, _respuestaAPI);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUsuario([FromRoute] string id)
        {
            try
            {
                if(string.IsNullOrEmpty(id))
                {
                    _respuestaAPI.IsSuccess = false;
                    _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                    _respuestaAPI.ErrorMessage.Add("Id invalido");
                    return BadRequest(_respuestaAPI);
                }

                if(!(await _usuarioRepository.ExisteUsuario(id)))
                {
                    _respuestaAPI.IsSuccess = false;
                    _respuestaAPI.StatusCode = HttpStatusCode.NotFound;
                    _respuestaAPI.ErrorMessage.Add("No existe ningun usuario con ese id");
                    return NotFound(_respuestaAPI);
                }

                UsuarioIdentity usuarioIdentity = await _usuarioRepository.GetUsuario(id);

                if(!(await _usuarioRepository.DeleteUsuario(usuarioIdentity)))
                {
                    _respuestaAPI.IsSuccess = false;
                    _respuestaAPI.StatusCode = HttpStatusCode.InternalServerError;
                    _respuestaAPI.ErrorMessage.Add($"Ocurrio un error al intentar eliminar al usuario con id: {id}");
                    return StatusCode(500, _respuestaAPI);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _respuestaAPI.IsSuccess = false;
                _respuestaAPI.ErrorMessage.Add("Ocurrió un Error interno en el servidor");
                _respuestaAPI.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, _respuestaAPI);
            }
        }
    }
}
