using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers;

[Route("api/[controller]")]
[ApiController]
//[EnableCors("NombrePoliticaCORS")] Por si se quiere aplicar CORS directamente a un controlador 
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
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    //[EnableCors("NombrePoliticaCORS")] Por si se quiere aplicar CORS directamente a un endpoint 
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

    [HttpGet("{id:int}", Name = "GetCategoria")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCategoria([FromRoute] int id)
    {
        var categoria = _repositoryCategoria.GetCategoria(id);

        if (categoria == null) return NotFound();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

        return Ok(categoriaDTO);
    }

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
