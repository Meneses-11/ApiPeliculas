using ApiPeliculas.Models;

namespace ApiPeliculas.Repositories.IRepositories;

public interface IPeliculaRepository
{
    ICollection<Pelicula> GetPeliculas();
    ICollection<Pelicula> GetPeliculasXCategoria(int idCategoria);
    IEnumerable<Pelicula> SearchPelicula(string nombrePelicula);
    Pelicula GetPelicula(int id);
    bool ExistPelicula(int id);
    bool ExistPelicula(string nombre);
    bool CreatePelicula(Pelicula pelicula);
    bool DeletePelicula(Pelicula pelicula);
    bool EditPelicula(Pelicula pelicula);
    bool Save();
}
