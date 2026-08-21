using System.Net;
using ApiPeliculas.Helpers;
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var listaUsuarios = await _usuarioRepository.GetUsuarios();

                var listaUsuariosDTO = new List<UsuarioDTO>();

                listaUsuariosDTO = _maper.Map<List<UsuarioDTO>>(listaUsuarios);

                return Ok(RespuestaAPIHelper.Success(listaUsuariosDTO));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaAPIHelper.Error("Ocurrion un Error interno en el servidor"));
            }
        }

        [HttpGet("{id}", Name = "GetUsuario")]
        [ResponseCache(CacheProfileName = "Global30Cache")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
        public async Task<IActionResult> GetUsuario(string id)
        {
            try 
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest(RespuestaAPIHelper.Error("Id invalido", HttpStatusCode.BadRequest));

                var usuario = await _usuarioRepository.GetUsuario(id);

                if (usuario == null)
                    return NotFound(RespuestaAPIHelper.Error("El usuario No existe", HttpStatusCode.NotFound));
                
                return Ok(RespuestaAPIHelper.Success(_maper.Map<UsuarioDTO>(usuario)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaAPIHelper.Error("Ocurrió un Error interno en el servidor"));
            }
        }

        [AllowAnonymous]
        [HttpPost("Registro")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RespuestaAPI))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
        public async Task<IActionResult> RegistroUsuario([FromBody] UsuarioCrearDTO crearUsuarioDTO)
        {
            try 
            {
                if (!ModelState.IsValid)
                {
                    List<string> errors = ModelState.Values.SelectMany(errs => errs.Errors).Select(errs => errs.ErrorMessage).ToList();
                    return BadRequest(RespuestaAPIHelper.Error(errors, HttpStatusCode.BadRequest));
                }

                bool usuarioUnico = await _usuarioRepository.IsUniqueUser(crearUsuarioDTO.NombreUsuario);

                if (!usuarioUnico)
                    return BadRequest(RespuestaAPIHelper.Error("El nombre de usuari ya existe", HttpStatusCode.BadRequest));

                var usuario = await _usuarioRepository.Registro(crearUsuarioDTO);

                if(usuario == null)
                    return StatusCode(500, RespuestaAPIHelper.Error("Error interno en el registro"));
                
                return CreatedAtAction(
                    nameof(GetUsuario), 
                    new {id = usuario.Id}, 
                    RespuestaAPIHelper.Success(usuario, HttpStatusCode.Created));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaAPIHelper.Error("Ocurrió un Error interno en el servidor"));
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
        public async Task<IActionResult> LoginUsuario([FromBody] UsuarioLoginDTO usuarioLoginDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    List<string> errors = ModelState.Values.SelectMany(errs => errs.Errors).Select(errs => errs.ErrorMessage).ToList();
                    return BadRequest(RespuestaAPIHelper.Error(errors, HttpStatusCode.BadRequest));
                }

                var respuestaLogin = await _usuarioRepository.Login(usuarioLoginDTO);

                if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
                    return BadRequest(RespuestaAPIHelper.Error("El nombre de usuario o password son incorrectos", HttpStatusCode.BadRequest));
                
                return Ok(RespuestaAPIHelper.Success(respuestaLogin));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaAPIHelper.Error("Ocurrió un Error interno en el servidor"));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
        public async Task<IActionResult> DeleteUsuario([FromRoute] string id)
        {
            try
            {
                if(string.IsNullOrEmpty(id))
                    return BadRequest(RespuestaAPIHelper.Error("Id invalido", HttpStatusCode.BadRequest));

                if(!(await _usuarioRepository.ExisteUsuario(id)))
                    return NotFound(RespuestaAPIHelper.Error("No existe ningun usuario con ese Id", HttpStatusCode.NotFound));
                
                UsuarioIdentity? usuarioIdentity = await _usuarioRepository.GetUsuario(id);

                if(usuarioIdentity == null)
                    return NotFound(RespuestaAPIHelper.Error("No existe ningun usuario con ese Id", HttpStatusCode.NotFound));

                if (!(await _usuarioRepository.DeleteUsuario(usuarioIdentity)))
                    return StatusCode(500, RespuestaAPIHelper.Error("Ocurrio un Error interno al intentar eliminar el usuario"));
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaAPIHelper.Error("Ocurrió un Error interno en el servidor"));
            }
        }
    }
}
