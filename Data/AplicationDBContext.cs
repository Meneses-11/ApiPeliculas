using ApiPeliculas.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Data;

public class AplicationDBContext : DbContext
{
    public AplicationDBContext(DbContextOptions<AplicationDBContext> option) : base(option)
    {
        
    }

    public DbSet<Categoria> Categoria { get; set; }
}
