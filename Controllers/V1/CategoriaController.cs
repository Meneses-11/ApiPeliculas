using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repositories.IRepositories;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
//[EnableCors("NombrePoliticaCORS")] Por si se quiere aplicar CORS directamente a un controlador 
//[Authorize(Roles = "Administrador")]
//[ResponseCache(Duration = 20)] //Cache a nivel controlador, Tiempo en segundos
[ApiVersion("1.0")]//, Deprecated = true)]
//[Obsolete("Esta version del controlador esta obsoleta")]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaRepository _repositoryCategoria;
    private readonly IMapper _mapper;

    public CategoriaController(ICategoriaRepository categoriaRepository, IMapper mapper)
    {
        _repositoryCategoria = categoriaRepository;
        _mapper = mapper;
    }

    [HttpGet("GetString")]
    //[MapToApiVersion("2.0")]
    public IEnumerable<string> Get()
    {
        return new string[] { "valor1", "valor2", "valor3" };
    }

    //[AllowAnonymous]  //Poner publico aunque la clase tenga authorize
    [HttpGet()]
    //[ResponseCache(Duration = 20)]
    [ResponseCache(CacheProfileName = "Global30Cache")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    //[EnableCors("NombrePoliticaCORS")] Por si se quiere aplicar CORS directamente a un endpoint 
    //[MapToApiVersion("1.0")]
    public IActionResult GetCategorias()
    {
        var listaCategorias = _repositoryCategoria.GetCategorias();

        var listaCategoriasDTO = new List<CategoriaDTO>();

        foreach (var cat in listaCategorias)
        {
            listaCategoriasDTO.Add(_mapper.Map<CategoriaDTO>(cat));
        }

        return Ok(listaCategoriasDTO);
    }

    //[AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetCategoria")]
    //[ResponseCache(Duration = 40)]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)] //Donde guardar cliente o servidor, Las respoestas no deben ser almacenadas en cache
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[Obsolete("Este endpoint del controlador esta obsoleta")]
    public IActionResult GetCategoria([FromRoute] int id)
    {
        var categoria = _repositoryCategoria.GetCategoria(id);

        if (categoria == null) return NotFound();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return Ok(categoriaDTO);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult PostCategoria([FromBody] CrearCategoriaDTO crearCategoriaDTO)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);

        if(crearCategoriaDTO == null) return BadRequest(ModelState);

        if (_repositoryCategoria.ExisteCategoria(crearCategoriaDTO.Nombre))
        {
            ModelState.AddModelError("","La categoria ya existe");
            return StatusCode(404, ModelState);
        }

        var categoria = _mapper.Map<Categoria>(crearCategoriaDTO);

        if (!_repositoryCategoria.CrearCategoria(categoria))
        {
            ModelState.AddModelError("", $"Algo salio mal guardando el registro {categoria.Nombre}");
            return StatusCode(404, ModelState);
        }

        return CreatedAtRoute("GetCategoria", new { id = categoria.Id }, categoria);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id:int}", Name = "PatchCategoria")] //Patch nos permite actualizar solo un campo
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult PatchCategoria(int id, [FromBody] CategoriaDTO categoriaDTO)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (categoriaDTO == null || id != categoriaDTO.Id) return BadRequest(ModelState);

        var categoriaExistente = _repositoryCategoria.GetCategoria(id);
        if (categoriaExistente == null) return NotFound($"No se encontro la categoria con id: {id}");

        var categoria = _mapper.Map<Categoria>(categoriaDTO);

        if (!_repositoryCategoria.EditCategoria(categoria))
        {
            ModelState.AddModelError("", $"Algo salio mal actualizando el registro {categoria.Nombre}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id:int}", Name = "PutCategoria")] //Put nos permite actualizar todo el modelo
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult PutCategoria(int id, [FromBody] CategoriaDTO categoriaDTO)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (categoriaDTO == null || id != categoriaDTO.Id) return BadRequest(ModelState);

        var categoriaExistente = _repositoryCategoria.GetCategoria(id);
        if(categoriaExistente == null) return NotFound($"No se encontro la categoria con id: {id}");

        var categoria = _mapper.Map<Categoria>(categoriaDTO);

        if (!_repositoryCategoria.EditCategoria(categoria))
        {
            ModelState.AddModelError("", $"Algo salio mal actualizando el registro {categoria.Nombre}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}", Name = "DeleteCategoria")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteCategoria(int id)
    {
        if (!_repositoryCategoria.ExisteCategoria(id)) return NotFound();

        var categoria = _repositoryCategoria.GetCategoria(id);

        if (!_repositoryCategoria.DeleteCategoria(categoria))
        {
            ModelState.AddModelError("", $"Algo salio mal borrando el registro {id}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
}
