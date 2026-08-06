using ApiPeliculas.Models;

namespace ApiPeliculas.Repositories.IRepositories;

public interface ICategoriaRepository
{
    ICollection<Categoria> GetCategorias();
    Categoria GetCategoria(int id);
    bool ExisteCategoria(int id);
    bool ExisteCategoria(string nombre);
    bool CrearCategoria(Categoria categoria);
    bool EditCategoria(Categoria categoria);
    bool DeleteCategoria(Categoria categoria);
    bool Guardar();
}
