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
        CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
        CreateMap<Pelicula, CrearPeliculaDTO>().ReverseMap();
        CreateMap<UsuarioIdentity, UsuarioDatosDTO>().ReverseMap();
        CreateMap<UsuarioIdentity, UsuarioDTO>().ReverseMap();
    }
}
