using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HeroesFluent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarHeroesDeEjemplo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Heroes",
                columns: new[] { "Id", "Alias", "EquipoId", "FechaRegistro", "NivelPoder", "Nombre" },
                values: new object[,]
                {
                    { 1, "Centella", 1, new DateTime(2026, 1, 10, 9, 30, 0, 0, DateTimeKind.Unspecified), 85, "Ana Torres" },
                    { 2, "Sombra", 1, new DateTime(2026, 2, 3, 14, 0, 0, 0, DateTimeKind.Unspecified), 72, "Bruno Díaz" },
                    { 3, "Tormenta", 2, new DateTime(2026, 3, 21, 11, 15, 0, 0, DateTimeKind.Unspecified), 90, "Carla Méndez" },
                    { 4, "Eco", null, new DateTime(2026, 5, 8, 16, 45, 0, 0, DateTimeKind.Unspecified), 40, "Diego Ruiz" }
                });

            migrationBuilder.InsertData(
                table: "HeroeMisiones",
                columns: new[] { "HeroeId", "MisionId", "FechaAsignacion", "Rol" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Líder" },
                    { 1, 2, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Apoyo" },
                    { 2, 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Apoyo" },
                    { 3, 2, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Líder" },
                    { 3, 3, new DateTime(2026, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Líder" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HeroeMisiones",
                keyColumns: new[] { "HeroeId", "MisionId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "HeroeMisiones",
                keyColumns: new[] { "HeroeId", "MisionId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "HeroeMisiones",
                keyColumns: new[] { "HeroeId", "MisionId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "HeroeMisiones",
                keyColumns: new[] { "HeroeId", "MisionId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "HeroeMisiones",
                keyColumns: new[] { "HeroeId", "MisionId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Heroes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Heroes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Heroes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Heroes",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
