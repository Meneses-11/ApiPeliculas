using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repositories.IRepositories;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("2.0")]
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
        return new string[] { "Adrian", "Manuel" };
    }

}
