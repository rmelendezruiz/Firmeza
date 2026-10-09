using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firmeza.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CanalesYCarros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "canal",
                table: "ventas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "PuntoFisico");

            migrationBuilder.AddColumn<Guid>(
                name: "carro_id",
                table: "ventas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "carros",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    marca = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    linea = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    anio = table.Column<int>(type: "integer", nullable: false),
                    precio = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    publicado = table.Column<bool>(type: "boolean", nullable: false),
                    vendido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carros", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ventas_carro_id",
                table: "ventas",
                column: "carro_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ventas_carros_carro_id",
                table: "ventas",
                column: "carro_id",
                principalTable: "carros",
                principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);

            migrationBuilder.AlterColumn<string>(
                name: "canal",
                table: "ventas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "PuntoFisico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ventas_carros_carro_id",
                table: "ventas");

            migrationBuilder.DropTable(
                name: "carros");

            migrationBuilder.DropIndex(
                name: "IX_ventas_carro_id",
                table: "ventas");

            migrationBuilder.DropColumn(
                name: "canal",
                table: "ventas");

            migrationBuilder.DropColumn(
                name: "carro_id",
                table: "ventas");
        }
    }
}
