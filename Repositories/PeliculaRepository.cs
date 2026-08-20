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

    public async Task<int> GetTotalPeliculas()
    {
        try
        {
            return await _dbContext.Pelicula.CountAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener total peliculas: {ex.Message}");
        }
    }

    public async Task<ICollection<Pelicula>> GetPeliculas(int pageNumber, int pageSize)
    {
        try
        {
            return await _dbContext.Pelicula.OrderBy(pel => pel.Nombre).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener peliculas: {ex.Message}");
        }
    }

    public async Task<ICollection<Pelicula>> GetPeliculasXCategoria(int idCategoria)
    {
        try
        {
            return await _dbContext.Pelicula.Include(ca => ca.Categoria).Where(peli => peli.CategoriaId == idCategoria).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener peliculas por categoria: {ex.Message}");
        }
    }

    public async Task<IEnumerable<Pelicula>> SearchPelicula(string nombrePelicula)
    {
        try
        {
            if (!string.IsNullOrEmpty(nombrePelicula))
                return await _dbContext.Pelicula.Where(peli => peli.Nombre.Contains(nombrePelicula) || peli.Descripcion.Contains(nombrePelicula)).ToListAsync();

            return new List<Pelicula>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener peliculas: {ex.Message}");
        }
    }

    public async Task<Pelicula?> GetPelicula(int id)
    {
        try
        {
            return await _dbContext.Pelicula.FirstOrDefaultAsync(peli => peli.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener peliculas buscador: {ex.Message}");
        }
    }

    public async Task<bool> ExistPelicula(int id)
    {
        try
        {
            return await _dbContext.Pelicula.AnyAsync(peli => peli.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error en Existe pelicula: {ex.Message}");
        }
    }

    public async Task<bool> ExistPelicula(string nombre)
    {
        try
        {
            return await _dbContext.Pelicula.AnyAsync(peli => peli.Nombre == nombre);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error en existe pelicula: {ex.Message}");
        }
    }

    public async Task<bool> CreatePelicula(Pelicula pelicula)
    {
        try
        {
            pelicula.FechaCreacion = DateTime.UtcNow;
            await _dbContext.Pelicula.AddAsync(pelicula);
            return await Save();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            throw new Exception($"Error al crear peliculas: {ex.Message}");
        }
    }

    public async Task<bool> DeletePelicula(Pelicula pelicula)
    {
        try
        {
            _dbContext.Pelicula.Remove(pelicula);
            return await Save();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al eliminar peliculas: {ex.Message}");
        }
    }

    public async Task<bool> EditPelicula(Pelicula pelicula)
    {
        try
        {
            var peliculaExistente = await _dbContext.Pelicula.FindAsync(pelicula.Id);
            if (peliculaExistente != null)
            {
                _dbContext.Entry(peliculaExistente).CurrentValues.SetValues(pelicula);
            }
            else
            {

                _dbContext.Pelicula.Update(pelicula);
            }
            return await Save();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al editar peliculas: {ex.Message}");
        }
    }

    public async Task<bool> Save()
    {
        try
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {ex.ToString()}");
            throw new Exception($"Error al guardar cambios: {ex.Message}");
        }
    }
}
