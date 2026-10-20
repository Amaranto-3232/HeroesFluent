using HeroesFluent.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroesFluent.Data;

public class HeroesDbContext : DbContext
{
    public HeroesDbContext(DbContextOptions<HeroesDbContext> options) : base(options)
    {
    }

    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Heroe> Heroes => Set<Heroe>();
    public DbSet<Mision> Misiones => Set<Mision>();
    public DbSet<HeroeMision> HeroeMisiones => Set<HeroeMision>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(HeroesDbContext).Assembly);
}
