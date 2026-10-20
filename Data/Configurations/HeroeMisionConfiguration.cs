using HeroesFluent.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeroesFluent.Data.Configurations;

public class HeroeMisionConfiguration : IEntityTypeConfiguration<HeroeMision>
{
    public void Configure(EntityTypeBuilder<HeroeMision> builder)
    {
        builder.ToTable("HeroeMisiones");

        builder.HasKey(hm => new { hm.HeroeId, hm.MisionId });

        builder.Property(hm => hm.Rol).IsRequired().HasMaxLength(50);
        builder.Property(hm => hm.FechaAsignacion).HasDefaultValueSql("GETDATE()");

        builder.HasOne(hm => hm.Heroe)
            .WithMany(h => h.HeroeMisiones)
            .HasForeignKey(hm => hm.HeroeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(hm => hm.Mision)
            .WithMany(m => m.HeroeMisiones)
            .HasForeignKey(hm => hm.MisionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new HeroeMision { HeroeId = 1, MisionId = 1, Rol = "Líder", FechaAsignacion = new DateTime(2026, 9, 1) },
            new HeroeMision { HeroeId = 2, MisionId = 1, Rol = "Apoyo", FechaAsignacion = new DateTime(2026, 9, 1) },
            new HeroeMision { HeroeId = 1, MisionId = 2, Rol = "Apoyo", FechaAsignacion = new DateTime(2026, 9, 20) },
            new HeroeMision { HeroeId = 3, MisionId = 2, Rol = "Líder", FechaAsignacion = new DateTime(2026, 9, 20) },
            new HeroeMision { HeroeId = 3, MisionId = 3, Rol = "Líder", FechaAsignacion = new DateTime(2026, 10, 5) });
    }
}
