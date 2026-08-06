using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using AutoMapper;

namespace ApiPeliculas.PeliculasMappers;

public class PeliculasMapper : Profile
{
    public PeliculasMapper()
    {
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<Categoria, CrearCategoriaDTO>().ReverseMap();
    }
}
