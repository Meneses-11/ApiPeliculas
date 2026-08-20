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

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
[ApiVersion("1.0")]
public class PeliculasController : ControllerBase
{
    private readonly IPeliculaRepository _repositoryPelicula;
    private readonly ICategoriaRepository _repositoryCategoria;
    private readonly IMapper _mapper;
    private static readonly string[] ImagenesPermitidas = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public PeliculasController(IPeliculaRepository peliculaRepository, ICategoriaRepository categoriaRepository, IMapper mapper)
    {
        _repositoryPelicula = peliculaRepository;
        _repositoryCategoria = categoriaRepository;
        _mapper = mapper;
    }

    [HttpGet("GetPeliculasXCategoria/{idCategoria:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> GetPeliculasXCategorias(int idCategoria)
    {
        try
        {
            if (idCategoria <= 0)
                return BadRequest(RespuestaAPIHelper.Error("Id invalido", HttpStatusCode.BadRequest));

            var listaPeliculas = await _repositoryPelicula.GetPeliculasXCategoria(idCategoria);

            if (listaPeliculas == null || !listaPeliculas.Any())
                return NotFound(RespuestaAPIHelper.Error("No se encontraron peliculas en esa categoria", HttpStatusCode.NotFound));

            var listaPeliculasDTO = _mapper.Map<List<PeliculaDTO>>(listaPeliculas);

            return Ok(RespuestaAPIHelper.Success(listaPeliculasDTO));
        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error("Ocurrió un Error interno en el servidor", HttpStatusCode.InternalServerError));
        }
    }

    [HttpGet("PeliculasSearch")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> BuscarPelicula(string nombre)
    {
        try
        {
            if (string.IsNullOrEmpty(nombre))
                return BadRequest(RespuestaAPIHelper.Error("Nombre invalido", HttpStatusCode.BadRequest));

            var peliculas = await _repositoryPelicula.SearchPelicula(nombre);
            if (!peliculas.Any())
                return NotFound(RespuestaAPIHelper.Error("No se encontraron Peliculas con los criterios de búsqueda.", HttpStatusCode.NotFound));

            var peliculasDTO = _mapper.Map<IEnumerable<PeliculaDTO>>(peliculas);

            return Ok(RespuestaAPIHelper.Success(peliculasDTO));

        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error("Ocurrió un Error interno en el servidor", HttpStatusCode.InternalServerError));
        }
    }

    [HttpGet("{pageNumber:int}/{pageSize:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> GetPeliculas([FromRoute] int pageNumber = 1, [FromRoute] int pageSize = 10)
    {
        try
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 10;

            var totalPeliculas = await _repositoryPelicula.GetTotalPeliculas();
            var peliculas = await _repositoryPelicula.GetPeliculas(pageNumber, pageSize);

            if (peliculas == null || !peliculas.Any())
            {
                return NotFound(RespuestaAPIHelper.Error("No se encontraron Peliculas", HttpStatusCode.NotFound));
            }

            var peliculasDTO = _mapper.Map<List<PeliculaDTO>>(peliculas);

            var response = new
            {
                PageNumber = pageNumber,
                pageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalPeliculas / (double)pageSize),
                TotalItems = totalPeliculas,
                Items = peliculasDTO
            };

            return Ok(RespuestaAPIHelper.Success(response));
        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error("Ocurrió un Error interno en el servidor", HttpStatusCode.InternalServerError));
        }
    }

    [HttpGet("{id:int}", Name = "GetPelicula")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> GetPelicula([FromRoute] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(RespuestaAPIHelper.Error("Id invalido", HttpStatusCode.BadRequest));

            var pelicula = await _repositoryPelicula.GetPelicula(id);

            if (pelicula == null)
                return NotFound(RespuestaAPIHelper.Error("No se Encontro ninguna pelicula con ese id", HttpStatusCode.NotFound));

            var peliculaDTO = _mapper.Map<PeliculaDTO>(pelicula);

            return Ok(RespuestaAPIHelper.Success(peliculaDTO));
        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error("Ocurrió un Error interno en el servidor", HttpStatusCode.InternalServerError));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> PostPelicula([FromForm] PeliculaCrearDTO crearPeliculaDTO)
    {
        try
        {

            if (!ModelState.IsValid)
            {
                List<string> errores = ModelState.Values.SelectMany(errs => errs.Errors).Select(errs => errs.ErrorMessage).ToList() ?? [];
                return BadRequest(RespuestaAPIHelper.Error(errores, HttpStatusCode.BadRequest));
            }

            if (await _repositoryPelicula.ExistPelicula(crearPeliculaDTO.Nombre))
                return Conflict(RespuestaAPIHelper.Error("Ya Existe una pelicula con ese nombre", HttpStatusCode.Conflict));

            if(crearPeliculaDTO.Imagen != null && crearPeliculaDTO.Imagen.Length > 5 * 1024 * 1024)
                return BadRequest(RespuestaAPIHelper.Error("La imagen no puede superar 5MB", HttpStatusCode.BadRequest));

            if(crearPeliculaDTO.Imagen != null && !ImagenesPermitidas.Contains(Path.GetExtension(crearPeliculaDTO.Imagen.FileName).ToLower()))
                return BadRequest(RespuestaAPIHelper.Error("Formato de imagen no permitido", HttpStatusCode.BadRequest));

            if(!await _repositoryCategoria.ExisteCategoria(crearPeliculaDTO.CategoriaId))
                return BadRequest(RespuestaAPIHelper.Error("No existe ninguna categoria con ese Id", HttpStatusCode.BadRequest));

            var pelicula = _mapper.Map<Pelicula>(crearPeliculaDTO);

            if (crearPeliculaDTO.Imagen != null)
            {
                string nombreArchivo = System.Guid.NewGuid().ToString() + Path.GetExtension(crearPeliculaDTO.Imagen.FileName);
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

            if (! await _repositoryPelicula.CreatePelicula(pelicula))
                return StatusCode(500, RespuestaAPIHelper.Error("Error al intentar Crear pelicula"));

            PeliculaDTO peliculaDTO = _mapper.Map<PeliculaDTO>(pelicula);

            return CreatedAtRoute(
                nameof(GetPelicula), 
                new { id = pelicula.Id }, 
                RespuestaAPIHelper.Success(peliculaDTO, HttpStatusCode.Created)
                );

        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error("Ocurrió un Error interno en el servidor", HttpStatusCode.InternalServerError));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}", Name = "PatchPelicula")] //Patch nos permite actualizar solo un campo
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> PatchPelicula(int id, [FromForm] PeliculaActualizarDTO actualizarPeliculaDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                List<string> errores = ModelState.Values.SelectMany(errs => errs.Errors).Select(errs => errs.ErrorMessage).ToList();
                return BadRequest(RespuestaAPIHelper.Error(errores, HttpStatusCode.BadRequest));
            }

            if (id != actualizarPeliculaDTO.Id) 
                return BadRequest(RespuestaAPIHelper.Error("Los id's deben coincidir", HttpStatusCode.BadRequest));

            var peliculaExistente = await _repositoryPelicula.GetPelicula(id);
            if (peliculaExistente == null) 
                return NotFound(RespuestaAPIHelper.Error("No se encontro ninguna pelicula con ese ID", HttpStatusCode.NotFound));

            if (actualizarPeliculaDTO.Imagen != null && actualizarPeliculaDTO.Imagen.Length > 5 * 1024 * 1024)
                return BadRequest(RespuestaAPIHelper.Error("La imagen no puede superar 5MB", HttpStatusCode.BadRequest));

            if (actualizarPeliculaDTO.Imagen != null && !ImagenesPermitidas.Contains(Path.GetExtension(actualizarPeliculaDTO.Imagen.FileName).ToLower()))
                return BadRequest(RespuestaAPIHelper.Error("Formato de imagen no permitido", HttpStatusCode.BadRequest));

            if (! await _repositoryCategoria.ExisteCategoria(actualizarPeliculaDTO.CategoriaId))
                return BadRequest(RespuestaAPIHelper.Error("No existe ninguna categoria con ese Id", HttpStatusCode.BadRequest));

            var pelicula = _mapper.Map<Pelicula>(actualizarPeliculaDTO);

            if (actualizarPeliculaDTO.Imagen != null)
            {
                if (!string.IsNullOrEmpty(peliculaExistente.RutaLocalImagen))
                {
                    string rutaImgAnterior = Path.Combine(Directory.GetCurrentDirectory(), peliculaExistente.RutaLocalImagen);

                    FileInfo fileImgAnt = new FileInfo(rutaImgAnterior);

                    if (fileImgAnt.Exists)
                        fileImgAnt.Delete();
                }

                string nombreArchivo = System.Guid.NewGuid().ToString() + Path.GetExtension(actualizarPeliculaDTO.Imagen.FileName);
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
                pelicula.RutaImagen = peliculaExistente.RutaImagen;
                pelicula.RutaLocalImagen = peliculaExistente.RutaLocalImagen;
            }

            if (! await _repositoryPelicula.EditPelicula(pelicula))
                return StatusCode(500, RespuestaAPIHelper.Error("Error al intentar actualizar la pelicula"));

            PeliculaDTO peliculaDTO = _mapper.Map<PeliculaDTO>(pelicula);
            return Ok(RespuestaAPIHelper.Success(peliculaDTO));
        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error("Ocurrió un Error interno en el servidor", HttpStatusCode.InternalServerError));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}", Name = "DeletePelicula")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(RespuestaAPI))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(RespuestaAPI))]
    public async Task<IActionResult> DeletePelicula(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(RespuestaAPIHelper.Error("Id invalido", HttpStatusCode.BadRequest));

            if (! await _repositoryPelicula.ExistPelicula(id)) 
                return NotFound(RespuestaAPIHelper.Error("No se encontro ninguna pelicula con ese id", HttpStatusCode.NotFound));

            var pelicula = await _repositoryPelicula.GetPelicula(id);

            if (! await _repositoryPelicula.DeletePelicula(pelicula))
            {
                return StatusCode(500, RespuestaAPIHelper.Error("Error al intentar Eliminar la pelicula"));
            }

            if (!string.IsNullOrEmpty(pelicula.RutaLocalImagen))
            {
                string ubicacionImg = Path.Combine(Directory.GetCurrentDirectory(), pelicula.RutaLocalImagen);
                FileInfo fileImg = new FileInfo(ubicacionImg);

                if (fileImg.Exists)
                    fileImg.Delete();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, RespuestaAPIHelper.Error("Ocurrió un Error interno en el servidor", HttpStatusCode.InternalServerError));
        }
    }
}
