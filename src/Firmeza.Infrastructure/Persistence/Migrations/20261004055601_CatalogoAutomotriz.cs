using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firmeza.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CatalogoAutomotriz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "categoria_id",
                table: "productos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "marca_id",
                table: "productos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sku",
                table: "productos",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "marcas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marcas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehiculos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    marca = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    linea = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    anio = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehiculos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aplicaciones",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    producto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehiculo_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aplicaciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_aplicaciones_productos_producto_id",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_aplicaciones_vehiculos_vehiculo_id",
                        column: x => x.vehiculo_id,
                        principalTable: "vehiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productos_categoria_id",
                table: "productos",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_productos_marca_id",
                table: "productos",
                column: "marca_id");

            migrationBuilder.CreateIndex(
                name: "IX_productos_sku",
                table: "productos",
                column: "sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_aplicaciones_producto_id_vehiculo_id",
                table: "aplicaciones",
                columns: new[] { "producto_id", "vehiculo_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_aplicaciones_vehiculo_id",
                table: "aplicaciones",
                column: "vehiculo_id");

            migrationBuilder.CreateIndex(
                name: "IX_categorias_nombre",
                table: "categorias",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_marcas_nombre",
                table: "marcas",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_marca_linea_anio",
                table: "vehiculos",
                columns: new[] { "marca", "linea", "anio" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_productos_categorias_categoria_id",
                table: "productos",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_productos_marcas_marca_id",
                table: "productos",
                column: "marca_id",
                principalTable: "marcas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AlterColumn<Guid>(
                name: "categoria_id",
                table: "productos",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "marca_id",
                table: "productos",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "sku",
                table: "productos",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldDefaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productos_categorias_categoria_id",
                table: "productos");

            migrationBuilder.DropForeignKey(
                name: "FK_productos_marcas_marca_id",
                table: "productos");

            migrationBuilder.DropTable(
                name: "aplicaciones");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "marcas");

            migrationBuilder.DropTable(
                name: "vehiculos");

            migrationBuilder.DropIndex(
                name: "IX_productos_categoria_id",
                table: "productos");

            migrationBuilder.DropIndex(
                name: "IX_productos_marca_id",
                table: "productos");

            migrationBuilder.DropIndex(
                name: "IX_productos_sku",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "categoria_id",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "marca_id",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "sku",
                table: "productos");
        }
    }
}
