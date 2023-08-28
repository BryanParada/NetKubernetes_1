using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetKubernetes.Models;

namespace NetKubernetes.Data;

public class AppDbContext: IdentityDbContext<Usuario>{

    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder) 
    {

    // este metodo servira para disparar el evento para generar todas las tablas dentro de la bdd
    // tomando el esquema de IdentityDbContext y el esquema adicional de negocio que agregamos (Inmueble)
        base.OnModelCreating(builder);
 
    }

    public DbSet<Inmueble>? Inmuebles {get; set;}


}