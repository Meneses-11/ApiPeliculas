using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using AutoMapper;

namespace ApiPeliculas.PeliculasMappers;

public class PeliculasMapper : Profile
{
    public PeliculasMapper()
    {
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<Categoria, CategoriaCrearDTO>().ReverseMap();
        CreateMap<Categoria, CategoriaActualizarDTO>().ReverseMap();
        CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
        CreateMap<Pelicula, PeliculaCrearDTO>().ReverseMap();
        CreateMap<Pelicula, PeliculaActualizarDTO>().ReverseMap();
        CreateMap<UsuarioIdentity, UsuarioDatosDTO>().ReverseMap();
        CreateMap<UsuarioIdentity, UsuarioDTO>().ReverseMap();
    }
}
