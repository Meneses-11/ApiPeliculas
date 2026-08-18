using ApiPeliculas.Models;

namespace ApiPeliculas.Repositories.IRepositories;

public interface IPeliculaRepository
{
    Task<int> GetTotalPeliculas();
    Task<ICollection<Pelicula>> GetPeliculas(int pageNumber, int pageSize);
    Task<ICollection<Pelicula>> GetPeliculasXCategoria(int idCategoria);
    Task<IEnumerable<Pelicula>> SearchPelicula(string nombrePelicula);
    Task<Pelicula> GetPelicula(int id);
    Task<bool> ExistPelicula(int id);
    Task<bool> ExistPelicula(string nombre);
    Task<bool> CreatePelicula(Pelicula pelicula);
    Task<bool> DeletePelicula(Pelicula pelicula);
    Task<bool> EditPelicula(Pelicula pelicula);
    Task<bool> Save();
}
