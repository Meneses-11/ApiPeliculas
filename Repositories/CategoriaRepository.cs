using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AplicationDBContext _dbContext;

    public CategoriaRepository(AplicationDBContext dBContext)
    {
        _dbContext = dBContext;
    }

    public async Task<bool> CrearCategoria(Categoria categoria)
    {
        try
        {
            categoria.FechaCreacion = DateTime.Now;
            await _dbContext.Categoria.AddAsync(categoria);
            return await Guardar();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al Intentar Crear Categoria: {ex.Message}");
        }
    }

    public async Task<bool> DeleteCategoria(Categoria categoria)
    {
        try
        {
            _dbContext.Categoria.Remove(categoria);
            return await Guardar();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al Intentar Eliminar Categoria: {ex.Message}");
        }
    }

    public async Task<bool> EditCategoria(Categoria categoria)
    {
        try
        {

            var categoriaExistente = await _dbContext.Categoria.FindAsync(categoria.Id);
            if (categoriaExistente != null)
            {
                categoria.FechaCreacion = categoriaExistente.FechaCreacion;
                _dbContext.Entry(categoriaExistente).CurrentValues.SetValues(categoria);
            }
            else
            {
                _dbContext.Categoria.Update(categoria);
            }

            return await Guardar();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al Intentar Editar Categoria: {ex.Message}");
        }
    }

    public async Task<bool> ExisteCategoria(int id)
    {
        try
        {
            return await _dbContext.Categoria.AnyAsync(cat => cat.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al Verificar Categoria: {ex.Message}");
        }
    }

    public async Task<bool> ExisteCategoria(string nombre)
    {
        try
        {
            return await _dbContext.Categoria.AnyAsync(cat => cat.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al Verificar Categoria: {ex.Message}");
        }
    }

    public async Task<bool> ExisteCategoriaPorNombreExceptoId(string nombre, int id)
    {
        try
        {
            return await _dbContext.Categoria.AnyAsync(ctgr => ctgr.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && ctgr.Id != id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al Verificar Categoria: {ex.Message}");
        }
    }

    public async Task<Categoria> GetCategoria(int id)
    {
        try
        {
            return await _dbContext.Categoria.FirstOrDefaultAsync(c => c.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al Intentar Obtener Categoria: {ex.Message}");
        }
    }

    public async Task<ICollection<Categoria>> GetCategorias()
    {
        try
        {
            return await _dbContext.Categoria.OrderBy(c => c.Nombre).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al Intentar Obtener Categorias: {ex.Message}");
        }
    }

    public async Task<bool> Guardar()
    {
        try
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al Intentar Guardar cambios en bd: {ex.Message}");
        }
    }
}
