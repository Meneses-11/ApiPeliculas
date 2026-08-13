using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Models.DTOs;

public class CrearPeliculaDTO
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Duracion { get; set; }
    public string? RutaImagen { get; set; }
    public IFormFile Imagen { get; set; }
    public CrearTipoClasificacion Clasificacion { get; set; }
    public enum CrearTipoClasificacion
    {
        Siete, Trece, Diecisei, Dieciocho
    }
    public int categoriaId { get; set; }
}
