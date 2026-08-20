using ApiPeliculas.Models;

namespace ApiPeliculas.Repositories.IRepositories;

public interface ICategoriaRepository
{
    Task<ICollection<Categoria>> GetCategorias();
    Task<Categoria?> GetCategoria(int id);
    Task<bool> ExisteCategoria(int id);
    Task<bool> ExisteCategoria(string nombre);
    Task<bool> ExisteCategoriaPorNombreExceptoId(string nombre, int id);
    Task<bool> CrearCategoria(Categoria categoria);
    Task<bool> EditCategoria(Categoria categoria);
    Task<bool> DeleteCategoria(Categoria categoria);
    Task<bool> Guardar();
}
