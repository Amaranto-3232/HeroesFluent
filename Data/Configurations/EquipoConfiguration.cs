using HeroesFluent.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeroesFluent.Data.Configurations;

public class EquipoConfiguration : IEntityTypeConfiguration<Equipo>
{
    public void Configure(EntityTypeBuilder<Equipo> builder)
    {
        builder.ToTable("Equipos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(80);
        builder.Property(e => e.FechaFundacion).HasColumnType("date");
        builder.Property(e => e.Presupuesto).HasPrecision(12, 2);

        builder.HasIndex(e => e.Nombre).IsUnique();

        builder.HasData(
            new Equipo { Id = 1, Nombre = "Guardianes", FechaFundacion = new DateTime(2020, 3, 15), Presupuesto = 250000.00m },
            new Equipo { Id = 2, Nombre = "Vigilantes del Sur", FechaFundacion = new DateTime(2022, 8, 1), Presupuesto = 120500.50m });
    }
}
