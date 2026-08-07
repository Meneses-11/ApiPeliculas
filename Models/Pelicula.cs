using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Models;

public class Pelicula
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Duracion { get; set; }
    public string RutaImagen { get; set; }
    public TipoClasificacion Clasificacion { get; set; }
    public enum TipoClasificacion
    {
        Siete, Trece, Diecisei, Dieciocho
    }
    public DateTime FechaCreacion { get; set; }

    //Relacion con tabla categoria
    public int categoriaId { get; set; }
    [ForeignKey("categoriaId")]
    public Categoria Categoria { get; set; }
}
