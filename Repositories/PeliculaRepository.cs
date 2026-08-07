using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repositories;

public class PeliculaRepository : IPeliculaRepository
{
    private readonly AplicationDBContext _dbContext;
    public PeliculaRepository(AplicationDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public bool CreatePelicula(Pelicula pelicula)
    {
        pelicula.FechaCreacion = DateTime.Now;
        _dbContext.Pelicula.Add(pelicula);
        return Save();
    }

    public bool DeletePelicula(Pelicula pelicula)
    {
        _dbContext.Pelicula.Remove(pelicula);
        return Save();
    }

    public bool EditPelicula(Pelicula pelicula)
    {
        pelicula.FechaCreacion = DateTime.Now;
        var peliculaExistente = _dbContext.Pelicula.Find(pelicula.Id);
        if(peliculaExistente != null)
        {
            _dbContext.Entry(peliculaExistente).CurrentValues.SetValues(pelicula);
        }
        else
        {

            _dbContext.Pelicula.Update(pelicula);
        }
        return Save();
    }

    public bool ExistPelicula(int id)
    {
        return _dbContext.Pelicula.Any(peli => peli.Id == id);
    }

    public bool ExistPelicula(string nombre)
    {
        return _dbContext.Pelicula.Any(peli => peli.Nombre == nombre);
    }

    public Pelicula GetPelicula(int id)
    {
        return _dbContext.Pelicula.FirstOrDefault(peli => peli.Id == id);
    }

    public ICollection<Pelicula> GetPeliculas()
    {
        return _dbContext.Pelicula.OrderBy(pel => pel.Nombre).ToList();
    }

    public ICollection<Pelicula> GetPeliculasXCategoria(int idCategoria)
    {
        return _dbContext.Pelicula.Include(ca => ca.Categoria).Where(peli => peli.categoriaId == idCategoria).ToList();
    }

    public bool Save()
    {
        return _dbContext.SaveChanges() > 0;
    }

    public IEnumerable<Pelicula> SearchPelicula(string nombrePelicula)
    {
        if(!string.IsNullOrEmpty(nombrePelicula))
            return _dbContext.Pelicula.Where(peli => peli.Nombre.Contains(nombrePelicula) || peli.Descripcion.Contains(nombrePelicula)).ToList();

        return new List<Pelicula>();
    }
}
