using ApiPeliculas.Helpers;
using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repositories.IRepositories;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiPeliculas.Controllers.V1;

//[ResponseCache(Duration = 20)] //Cache a nivel controlador, Tiempo en segundos
//[EnableCors("NombrePoliticaCORS")] Por si se quiere aplicar CORS directamente a un controlador 
//[Obsolete("Esta version del controlador esta obsoleta")]

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
[ApiVersion("1.0")]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaRepository _repositoryCategoria;
    private readonly IMapper _mapper;

    public CategoriaController(ICategoriaRepository categoriaRepository, IMapper mapper)
    {
        _repositoryCategoria = categoriaRepository;
        _mapper = mapper;
    }

    [HttpGet()]
    [ResponseCache(CacheProfileName = "Global30Cache")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> GetCategorias()
    {
        try
        {
            var listaCategorias = await _repositoryCategoria.GetCategorias();

            if (listaCategorias is null || listaCategorias.Count == 0)
                return NotFound(RespuestaAPIHelper.Error("No se encontro ninguna catgoria", HttpStatusCode.NotFound));

            var listaCategoriasDTO = new List<CategoriaDTO>();

            listaCategoriasDTO = _mapper.Map<List<CategoriaDTO>>(listaCategorias);

            return Ok(RespuestaAPIHelper.Success(listaCategoriasDTO));
        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error(ex.Message));
        }
    }

    [HttpGet("{id:int}", Name = "GetCategoria")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)] //Donde guardar cliente o servidor, Las respoestas no deben ser almacenadas en cache
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> GetCategoria([FromRoute] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(RespuestaAPIHelper.Error("Id invalido", HttpStatusCode.BadRequest));

            var categoria = await _repositoryCategoria.GetCategoria(id);

            if (categoria == null) 
                return NotFound(RespuestaAPIHelper.Error("No se encontro ninguna categoria con ese Id", HttpStatusCode.NotFound));

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(RespuestaAPIHelper.Success(categoriaDTO));
        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error(ex.Message));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> PostCategoria([FromBody] CrearCategoriaDTO crearCategoriaDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(errs => errs.Errors).Select(errs => errs.ErrorMessage).ToList();
                return BadRequest(RespuestaAPIHelper.Error(errors, HttpStatusCode.BadRequest));
            }

            if (await _repositoryCategoria.ExisteCategoria(crearCategoriaDTO.Nombre))
                return Conflict(RespuestaAPIHelper.Error("Ya existe una categoria con ese nombre", HttpStatusCode.Conflict));
            
            var categoria = _mapper.Map<Categoria>(crearCategoriaDTO);

            if (!await _repositoryCategoria.CrearCategoria(categoria))
                return StatusCode(500, RespuestaAPIHelper.Error("Algo salio mal al crear categoria"));

            CategoriaDTO categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return CreatedAtRoute(
                nameof(GetCategoria), 
                new { id = categoria.Id }, 
                RespuestaAPIHelper.Success(categoriaDTO, HttpStatusCode.Created)
                );
        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error(ex.Message));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}", Name = "PatchCategoria")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> PatchCategoria(int id, [FromBody] CategoriaEditarDTO categoriaEditarDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(errs => errs.Errors).Select(errs => errs.ErrorMessage).ToList();
                return BadRequest(RespuestaAPIHelper.Error(errors, HttpStatusCode.BadRequest));
            }

            if (id != categoriaEditarDTO.Id) 
                return BadRequest(RespuestaAPIHelper.Error("Los id's deben coincidir", HttpStatusCode.BadRequest));

            var categoriaExistente = await _repositoryCategoria.GetCategoria(id);
            if (categoriaExistente == null) 
                return NotFound(RespuestaAPIHelper.Error($"No se encontro una categoria con ese id", HttpStatusCode.NotFound));

            if (await _repositoryCategoria.ExisteCategoriaPorNombreExceptoId(categoriaEditarDTO.Nombre, categoriaExistente.Id))
                return Conflict(RespuestaAPIHelper.Error("Ya existe una categoria con ese nombre", HttpStatusCode.Conflict));

            var categoria = _mapper.Map<Categoria>(categoriaEditarDTO);

            if (!await _repositoryCategoria.EditCategoria(categoria))
                return StatusCode(500, RespuestaAPIHelper.Error("Algo salio mal al editar la categoria"));

            CategoriaDTO categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(RespuestaAPIHelper.Success(categoriaDTO));
        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error(ex.Message));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}", Name = "PutCategoria")] //Put nos permite actualizar todo el modelo
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> PutCategoria(int id, [FromBody] CategoriaEditarDTO categoriaEditarDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(errs => errs.Errors).Select(errs => errs.ErrorMessage).ToList();
                return BadRequest(RespuestaAPIHelper.Error(errors, HttpStatusCode.BadRequest));
            }

            if (id != categoriaEditarDTO.Id)
                return BadRequest(RespuestaAPIHelper.Error("Los id's deben coincidir", HttpStatusCode.BadRequest));

            var categoriaExistente = await _repositoryCategoria.GetCategoria(id);
            if (categoriaExistente == null)
                return NotFound(RespuestaAPIHelper.Error($"No se encontro una categoria con ese id", HttpStatusCode.NotFound));

            if (await _repositoryCategoria.ExisteCategoriaPorNombreExceptoId(categoriaEditarDTO.Nombre, categoriaExistente.Id))
                return Conflict(RespuestaAPIHelper.Error("Ya existe una categoria con ese nombre", HttpStatusCode.Conflict));

            var categoria = _mapper.Map<Categoria>(categoriaEditarDTO);

            if (!await _repositoryCategoria.EditCategoria(categoria))
                return StatusCode(500, RespuestaAPIHelper.Error("Algo salio mal al editar la categoria"));

            CategoriaDTO categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(RespuestaAPIHelper.Success(categoriaDTO));
        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error(ex.Message));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}", Name = "DeleteCategoria")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        try
        {
            if(id <= 0)
                return BadRequest(RespuestaAPIHelper.Error("Id invalido", HttpStatusCode.BadRequest));

            if (!await _repositoryCategoria.ExisteCategoria(id))
                return NotFound(RespuestaAPIHelper.Error("No existe una categoria con ese Id", HttpStatusCode.NotFound));

            var categoria = await _repositoryCategoria.GetCategoria(id);

            if (!await _repositoryCategoria.DeleteCategoria(categoria))
                return StatusCode(500, RespuestaAPIHelper.Error("Algo salio mal al eliminar la Categoria"));

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error(ex.Message));
        }
    }
}
