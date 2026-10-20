using System;
using HeroesFluent.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HeroesFluent.Data.Migrations
{
    [DbContext(typeof(HeroesDbContext))]
    [Migration("20261011204937_ActualizarDatosInicialesEquipos")]
    partial class ActualizarDatosInicialesEquipos
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.27")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HeroesFluent.Models.Equipo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FechaFundacion")
                        .HasColumnType("date");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.Property<decimal>("Presupuesto")
                        .HasPrecision(12, 2)
                        .HasColumnType("decimal(12,2)");

                    b.HasKey("Id");

                    b.HasIndex("Nombre")
                        .IsUnique();

                    b.ToTable("Equipos", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FechaFundacion = new DateTime(2020, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Nombre = "Guardianes",
                            Presupuesto = 250000.00m
                        },
                        new
                        {
                            Id = 2,
                            FechaFundacion = new DateTime(2022, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Nombre = "Vigilantes del Sur",
                            Presupuesto = 120500.50m
                        });
                });

            modelBuilder.Entity("HeroesFluent.Models.Heroe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<int?>("EquipoId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaRegistro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("NivelPoder")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Alias")
                        .IsUnique();

                    b.HasIndex("EquipoId");

                    b.ToTable("Heroes", null, t =>
                        {
                            t.HasCheckConstraint("CK_Heroes_NivelPoder", "[NivelPoder] BETWEEN 1 AND 100");
                        });
                });

            modelBuilder.Entity("HeroesFluent.Models.HeroeMision", b =>
                {
                    b.Property<int>("HeroeId")
                        .HasColumnType("int");

                    b.Property<int>("MisionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaAsignacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Rol")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("HeroeId", "MisionId");

                    b.HasIndex("MisionId");

                    b.ToTable("HeroeMisiones", (string)null);
                });

            modelBuilder.Entity("HeroesFluent.Models.Mision", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Dificultad")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime?>("FechaLimite")
                        .HasColumnType("date");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.ToTable("Misiones", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Descripcion = "Evacuar a los ciudadanos en peligro.",
                            Dificultad = "Baja",
                            FechaLimite = new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Titulo = "Rescate en la ciudad"
                        },
                        new
                        {
                            Id = 2,
                            Descripcion = "Proteger la ciudad de una invasión.",
                            Dificultad = "Alta",
                            FechaLimite = new DateTime(2026, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Titulo = "Detener la invasión"
                        },
                        new
                        {
                            Id = 3,
                            Dificultad = "Extrema",
                            Titulo = "Defender el planeta"
                        });
                });

            modelBuilder.Entity("HeroesFluent.Models.Heroe", b =>
                {
                    b.HasOne("HeroesFluent.Models.Equipo", "Equipo")
                        .WithMany("Heroes")
                        .HasForeignKey("EquipoId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Equipo");
                });

            modelBuilder.Entity("HeroesFluent.Models.HeroeMision", b =>
                {
                    b.HasOne("HeroesFluent.Models.Heroe", "Heroe")
                        .WithMany("HeroeMisiones")
                        .HasForeignKey("HeroeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HeroesFluent.Models.Mision", "Mision")
                        .WithMany("HeroeMisiones")
                        .HasForeignKey("MisionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Heroe");

                    b.Navigation("Mision");
                });

            modelBuilder.Entity("HeroesFluent.Models.Equipo", b =>
                {
                    b.Navigation("Heroes");
                });

            modelBuilder.Entity("HeroesFluent.Models.Heroe", b =>
                {
                    b.Navigation("HeroeMisiones");
                });

            modelBuilder.Entity("HeroesFluent.Models.Mision", b =>
                {
                    b.Navigation("HeroeMisiones");
                });
#pragma warning restore 612, 618
        }
    }
}
