using HeroesFluent.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeroesFluent.Data.Configurations;

public class MisionConfiguration : IEntityTypeConfiguration<Mision>
{
    public void Configure(EntityTypeBuilder<Mision> builder)
    {
        builder.ToTable("Misiones");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Titulo).IsRequired().HasMaxLength(150);
        builder.Property(m => m.Descripcion).HasMaxLength(500).IsRequired(false);
        builder.Property(m => m.Dificultad).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(m => m.FechaLimite).HasColumnType("date").IsRequired(false);

        builder.HasData(
            new Mision { Id = 1, Titulo = "Rescate en la ciudad", Descripcion = "Evacuar a los ciudadanos en peligro.", Dificultad = Dificultad.Baja, FechaLimite = new DateTime(2026, 11, 1) },
            new Mision { Id = 2, Titulo = "Detener la invasión", Descripcion = "Proteger la ciudad de una invasión.", Dificultad = Dificultad.Alta, FechaLimite = new DateTime(2026, 11, 15) },
            new Mision { Id = 3, Titulo = "Defender el planeta", Descripcion = null, Dificultad = Dificultad.Extrema, FechaLimite = null });
    }
}
