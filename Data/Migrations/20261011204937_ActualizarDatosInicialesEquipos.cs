using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroesFluent.Data.Migrations
{
    public partial class ActualizarDatosInicialesEquipos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Equipos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaFundacion", "Nombre", "Presupuesto" },
                values: new object[] { new DateTime(2020, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Guardianes", 250000.00m });

            migrationBuilder.UpdateData(
                table: "Equipos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFundacion", "Nombre", "Presupuesto" },
                values: new object[] { new DateTime(2022, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vigilantes del Sur", 120500.50m });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Equipos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaFundacion", "Nombre", "Presupuesto" },
                values: new object[] { new DateTime(1963, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vengadores", 1500000m });

            migrationBuilder.UpdateData(
                table: "Equipos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFundacion", "Nombre", "Presupuesto" },
                values: new object[] { new DateTime(1960, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Liga de la Justicia", 2000000m });
        }
    }
}
