using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repositories.IRepositories;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
[ApiVersion("1.0")]
//[ApiVersion("2.0")]
public class PeliculasController : ControllerBase
{
    private readonly IPeliculaRepository _repositoryPelicula;
    private readonly IMapper _mapper;

    public PeliculasController(IPeliculaRepository peliculaRepository, IMapper mapper)
    {
        _repositoryPelicula = peliculaRepository;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetPeliculas()
    {
        var listaPeliculas = _repositoryPelicula.GetPeliculas();

        var listaPeliculasDTO = new List<PeliculaDTO>();

        foreach (var peli in listaPeliculas)
        {
            listaPeliculasDTO.Add(_mapper.Map<PeliculaDTO>(peli));
        }

        return Ok(listaPeliculasDTO);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetPelicula")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPelicula([FromRoute] int id)
    {
        var pelicula = _repositoryPelicula.GetPelicula(id);

        if (pelicula == null) return NotFound();

        var peliculaDTO = _mapper.Map<PeliculaDTO>(pelicula);

        return Ok(peliculaDTO);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(PeliculaDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult PostPelicula([FromForm] CrearPeliculaDTO crearPeliculaDTO)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);

        if(crearPeliculaDTO == null) return BadRequest(ModelState);

        if (_repositoryPelicula.ExistPelicula(crearPeliculaDTO.Nombre))
        {
            ModelState.AddModelError("", "La pelucula ya existe");
            return StatusCode(404, ModelState);
        }

        var pelicula = _mapper.Map<Pelicula>(crearPeliculaDTO);

        /*if (!_repositoryPelicula.CreatePelicula(pelicula))
        {
            ModelState.AddModelError("", $"No se pudo crear la pelicula con id {pelicula.Id}");
            return StatusCode(404, ModelState);
        }*/

        if(crearPeliculaDTO.Imagen != null)
        {
            string nombreArchivo = pelicula.Id + System.Guid.NewGuid().ToString() + Path.GetExtension(crearPeliculaDTO.Imagen.FileName);
            string rutaArchivo = @"wwwroot\ImgMovies\" + nombreArchivo;

            var ubicacionDirectory = Path.Combine(Directory.GetCurrentDirectory(), rutaArchivo);

            FileInfo file = new FileInfo(ubicacionDirectory);

            if (file.Exists)
            {
                file.Delete();
            }

            using (var fileStream = new FileStream(ubicacionDirectory, FileMode.Create))
            {
                crearPeliculaDTO.Imagen.CopyTo(fileStream);
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}"; //Contruye la url en base al sitio web actual
            pelicula.RutaImagen = baseUrl + "/ImgMovies/" + nombreArchivo;
            pelicula.RutaLocalImagen = rutaArchivo;
        }
        else
        {
            pelicula.RutaImagen = "https://placehold.co/600x400";
        }

        if (!_repositoryPelicula.CreatePelicula(pelicula))
        {
            ModelState.AddModelError("", $"No se pudo crear la pelicula con id {pelicula.Id}");
            return StatusCode(404, ModelState);
        }

        return CreatedAtRoute("GetPelicula", new { id = pelicula.Id }, pelicula);
    }
    
    [HttpPatch("{id:int}", Name = "PatchPelicula")] //Patch nos permite actualizar solo un campo
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult PatchPelicula(int id, [FromForm] ActualizarPeliculaDTO actualizarPeliculaDTO)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (actualizarPeliculaDTO == null || id != actualizarPeliculaDTO.Id) return BadRequest(ModelState);

        var categoriaExistente = _repositoryPelicula.GetPelicula(id);
        if (categoriaExistente == null) return NotFound($"No se encontro la pelicula con id: {id}");

        var pelicula = _mapper.Map<Pelicula>(actualizarPeliculaDTO);

        if (actualizarPeliculaDTO.Imagen != null)
        {
            string nombreArchivo = pelicula.Id + System.Guid.NewGuid().ToString() + Path.GetExtension(actualizarPeliculaDTO.Imagen.FileName);
            string rutaArchivo = @"wwwroot\ImgMovies\" + nombreArchivo;

            var ubicacionDirectory = Path.Combine(Directory.GetCurrentDirectory(), rutaArchivo);

            FileInfo file = new FileInfo(ubicacionDirectory);

            if (file.Exists)
            {
                file.Delete();
            }

            using (var fileStream = new FileStream(ubicacionDirectory, FileMode.Create))
            {
                actualizarPeliculaDTO.Imagen.CopyTo(fileStream);
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}"; //Contruye la url en base al sitio web actual
            pelicula.RutaImagen = baseUrl + "/ImgMovies/" + nombreArchivo;
            pelicula.RutaLocalImagen = rutaArchivo;
        }
        else
        {
            pelicula.RutaImagen = "https://placehold.co/600x400";
        }

        if (!_repositoryPelicula.EditPelicula(pelicula))
        {
            ModelState.AddModelError("", $"Algo salio mal actualizando el registro {pelicula.Nombre}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "DeletePelicula")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeletePelicula(int id)
    {
        if (!_repositoryPelicula.ExistPelicula(id)) return NotFound();

        var pelicula = _repositoryPelicula.GetPelicula(id);

        if (!_repositoryPelicula.DeletePelicula(pelicula))
        {
            ModelState.AddModelError("", $"Algo salio mal borrando el registro {id}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("GetPeliculasXCategoria/{idCategoria:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetPeliculasXCategorias(int idCategoria)
    {
        var listaPeliculas = _repositoryPelicula.GetPeliculasXCategoria(idCategoria);
        
        if(listaPeliculas == null) return NotFound();

        var itemPelicula = new List<PeliculaDTO>();

        foreach(var peli in listaPeliculas)
        {
            itemPelicula.Add(_mapper.Map<PeliculaDTO>(peli));
        }

        return Ok(itemPelicula);
    }

    [AllowAnonymous]
    [HttpGet("PeliculasSearch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult BuscarPelicula(string nombre)
    {
        try
        {
            var resultado = _repositoryPelicula.SearchPelicula(nombre);
            if (resultado.Any())
            {
                return Ok(resultado);
            }

            return NotFound();
        }
        catch (Exception ex) {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos: "+ex.Message);
        }
    }
}
