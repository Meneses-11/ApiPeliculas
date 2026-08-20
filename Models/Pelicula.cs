using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Models;

public class Pelicula
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Duracion { get; set; }
    public string? RutaImagen { get; set; }
    public string? RutaLocalImagen { get; set; }
    public TipoClasificacion Clasificacion { get; set; }
    public enum TipoClasificacion
    {
        Siete, Trece, Dieciseis, Dieciocho
    }
    public DateTime? FechaCreacion { get; set; }

    //Relacion con tabla categoria
    public int CategoriaId { get; set; }
    [ForeignKey("CategoriaId")]
    public Categoria Categoria { get; set; }
}
