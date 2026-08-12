using ApiPeliculas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Data;

public class AplicationDBContext : IdentityDbContext<UsuarioIdentity>
{
    public AplicationDBContext(DbContextOptions<AplicationDBContext> option) : base(option)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public DbSet<Categoria> Categoria { get; set; }
    public DbSet<Pelicula> Pelicula { get; set; }
    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<UsuarioIdentity> UsuarioIdentity { get; set; }
}
