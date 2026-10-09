using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firmeza.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GestionComercial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "vendedor_correo",
                table: "ventas",
                type: "character varying(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ciudad",
                table: "carros",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE carros
                SET ciudad = 'Barranquilla'
                WHERE ciudad = '';
                """);

            migrationBuilder.AlterColumn<string>(
                name: "vendedor_correo",
                table: "ventas",
                type: "character varying(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(160)",
                oldMaxLength: 160,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ciudad",
                table: "carros",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldDefaultValue: "");

            migrationBuilder.CreateTable(
                name: "fichas_comercial",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    correo = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    sueldo = table.Column<decimal>(type: "numeric(14,2)", precision: 14, scale: 2, nullable: false),
                    porcentaje_comision = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fichas_comercial", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mensajes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    de_correo = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    para_correo = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    texto = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mensajes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "metas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    correo = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    periodo = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    monto = table.Column<decimal>(type: "numeric(14,2)", precision: 14, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metas", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_carros_ciudad",
                table: "carros",
                column: "ciudad");

            migrationBuilder.CreateIndex(
                name: "IX_fichas_comercial_correo",
                table: "fichas_comercial",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mensajes_de_correo",
                table: "mensajes",
                column: "de_correo");

            migrationBuilder.CreateIndex(
                name: "IX_mensajes_para_correo",
                table: "mensajes",
                column: "para_correo");

            migrationBuilder.CreateIndex(
                name: "IX_metas_correo_periodo",
                table: "metas",
                columns: new[] { "correo", "periodo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fichas_comercial");

            migrationBuilder.DropTable(
                name: "mensajes");

            migrationBuilder.DropTable(
                name: "metas");

            migrationBuilder.DropIndex(
                name: "IX_carros_ciudad",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "vendedor_correo",
                table: "ventas");

            migrationBuilder.DropColumn(
                name: "ciudad",
                table: "carros");
        }
    }
}
