using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositories.IRepositories;

namespace ApiPeliculas.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AplicationDBContext _dbContext;

    public CategoriaRepository(AplicationDBContext dBContext)
    {
        _dbContext = dBContext;
    }

    public bool CrearCategoria(Categoria categoria)
    {
        categoria.FechaCreacion = DateTime.Now;
        _dbContext.Categoria.Add(categoria);
        return Guardar();
    }

    public bool DeleteCategoria(Categoria categoria)
    {
        _dbContext.Categoria.Remove(categoria);
        return Guardar();
    }

    public bool EditCategoria(Categoria categoria)
    {
        categoria.FechaCreacion = DateTime.Now;

        var categoriaExistente = _dbContext.Categoria.Find(categoria.Id);
        if(categoriaExistente != null)
        {
            _dbContext.Entry(categoriaExistente).CurrentValues.SetValues(categoria);
        }
        else
        {
            _dbContext.Categoria.Update(categoria);
        }

        return Guardar();
    }

    public bool ExisteCategoria(int id)
    {
        return _dbContext.Categoria.Any(cat => cat.Id == id);
    }

    public bool ExisteCategoria(string nombre)
    {
        return _dbContext.Categoria.Any(cat => cat.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
    }

    public Categoria GetCategoria(int id)
    {
        return _dbContext.Categoria.FirstOrDefault(c => c.Id == id);
    }

    public ICollection<Categoria> GetCategorias()
    {
        return _dbContext.Categoria.OrderBy(c => c.Nombre).ToList();
    }

    public bool Guardar()
    {
        return _dbContext.SaveChanges() > 0;
    }
}
