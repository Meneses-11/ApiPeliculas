using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.DTOs;

public class CategoriaCrearDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El número máximo de catacteres es 100")]
    public string Nombre { get; set; }
}
