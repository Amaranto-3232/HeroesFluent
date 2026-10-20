using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace HeroesFluent.Data.Migrations
{
    public partial class CrearModeloHeroes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FechaFundacion = table.Column<DateTime>(type: "date", nullable: false),
                    Presupuesto = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Misiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Dificultad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaLimite = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Misiones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Heroes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NivelPoder = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EquipoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heroes", x => x.Id);
                    table.CheckConstraint("CK_Heroes_NivelPoder", "[NivelPoder] BETWEEN 1 AND 100");
                    table.ForeignKey(
                        name: "FK_Heroes_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HeroeMisiones",
                columns: table => new
                {
                    HeroeId = table.Column<int>(type: "int", nullable: false),
                    MisionId = table.Column<int>(type: "int", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroeMisiones", x => new { x.HeroeId, x.MisionId });
                    table.ForeignKey(
                        name: "FK_HeroeMisiones_Heroes_HeroeId",
                        column: x => x.HeroeId,
                        principalTable: "Heroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeroeMisiones_Misiones_MisionId",
                        column: x => x.MisionId,
                        principalTable: "Misiones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Equipos",
                columns: new[] { "Id", "FechaFundacion", "Nombre", "Presupuesto" },
                values: new object[,]
                {
                    { 1, new DateTime(1963, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vengadores", 1500000m },
                    { 2, new DateTime(1960, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Liga de la Justicia", 2000000m }
                });

            migrationBuilder.InsertData(
                table: "Misiones",
                columns: new[] { "Id", "Descripcion", "Dificultad", "FechaLimite", "Titulo" },
                values: new object[,]
                {
                    { 1, "Evacuar a los ciudadanos en peligro.", "Baja", new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rescate en la ciudad" },
                    { 2, "Proteger la ciudad de una invasión.", "Alta", new DateTime(2026, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Detener la invasión" },
                    { 3, null, "Extrema", null, "Defender el planeta" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_Nombre",
                table: "Equipos",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroeMisiones_MisionId",
                table: "HeroeMisiones",
                column: "MisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_Alias",
                table: "Heroes",
                column: "Alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_EquipoId",
                table: "Heroes",
                column: "EquipoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeroeMisiones");

            migrationBuilder.DropTable(
                name: "Heroes");

            migrationBuilder.DropTable(
                name: "Misiones");

            migrationBuilder.DropTable(
                name: "Equipos");
        }
    }
}
