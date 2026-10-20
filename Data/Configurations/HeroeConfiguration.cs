using HeroesFluent.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeroesFluent.Data.Configurations;

public class HeroeConfiguration : IEntityTypeConfiguration<Heroe>
{
    public void Configure(EntityTypeBuilder<Heroe> builder)
    {
        builder.ToTable("Heroes", table =>
            table.HasCheckConstraint("CK_Heroes_NivelPoder", "[NivelPoder] BETWEEN 1 AND 100"));

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(h => h.Alias).IsRequired().HasMaxLength(60);
        builder.Property(h => h.NivelPoder).IsRequired();
        builder.Property(h => h.FechaRegistro).HasDefaultValueSql("GETDATE()");

        builder.HasIndex(h => h.Alias).IsUnique();

        builder.HasOne(h => h.Equipo)
            .WithMany(e => e.Heroes)
            .HasForeignKey(h => h.EquipoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasData(
            new Heroe { Id = 1, Nombre = "Ana Torres", Alias = "Centella", NivelPoder = 85, FechaRegistro = new DateTime(2026, 1, 10, 9, 30, 0), EquipoId = 1 },
            new Heroe { Id = 2, Nombre = "Bruno Díaz", Alias = "Sombra", NivelPoder = 72, FechaRegistro = new DateTime(2026, 2, 3, 14, 0, 0), EquipoId = 1 },
            new Heroe { Id = 3, Nombre = "Carla Méndez", Alias = "Tormenta", NivelPoder = 90, FechaRegistro = new DateTime(2026, 3, 21, 11, 15, 0), EquipoId = 2 },
            new Heroe { Id = 4, Nombre = "Diego Ruiz", Alias = "Eco", NivelPoder = 40, FechaRegistro = new DateTime(2026, 5, 8, 16, 45, 0), EquipoId = null });
    }
}
