using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firmeza.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PreciosYEstados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "precio_carro",
                table: "ventas",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "cargo_preliminar",
                table: "ventas",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "estado",
                table: "ventas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "plazo_peritaje",
                table: "ventas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "condicion",
                table: "carros",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "disponibilidad",
                table: "carros",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "foto_url",
                table: "carros",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "kilometraje",
                table: "carros",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "precio_counter",
                table: "carros",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "precio_pagina",
                table: "carros",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo",
                table: "carros",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE carros
                SET tipo = 'Automovil',
                    condicion = 'Usado',
                    kilometraje = CASE WHEN vendido THEN 48000 ELSE 62000 END,
                    disponibilidad = CASE WHEN publicado THEN 'Ambos' ELSE 'SoloCounter' END,
                    precio_pagina = CASE WHEN publicado THEN precio ELSE NULL END,
                    precio_counter = precio,
                    foto_url = CASE WHEN publicado AND NOT vendido THEN '/set/unidad.jpg' ELSE '' END;

                UPDATE ventas
                SET precio_carro = CASE WHEN carro_id IS NOT NULL THEN total - iva ELSE 0 END,
                    estado = 'Cerrada';
                """);

            migrationBuilder.DropColumn(name: "iva", table: "ventas");
            migrationBuilder.DropColumn(name: "total", table: "ventas");
            migrationBuilder.DropColumn(name: "precio", table: "carros");
            migrationBuilder.DropColumn(name: "publicado", table: "carros");

            migrationBuilder.AlterColumn<decimal>(
                name: "precio_carro",
                table: "ventas",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                table: "ventas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "condicion",
                table: "carros",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "disponibilidad",
                table: "carros",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "foto_url",
                table: "carros",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "tipo",
                table: "carros",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cargo_preliminar",
                table: "ventas");

            migrationBuilder.DropColumn(
                name: "estado",
                table: "ventas");

            migrationBuilder.DropColumn(
                name: "plazo_peritaje",
                table: "ventas");

            migrationBuilder.DropColumn(
                name: "condicion",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "disponibilidad",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "foto_url",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "kilometraje",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "precio_counter",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "precio_pagina",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "tipo",
                table: "carros");

            migrationBuilder.RenameColumn(
                name: "precio_carro",
                table: "ventas",
                newName: "total");

            migrationBuilder.AddColumn<decimal>(
                name: "iva",
                table: "ventas",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "precio",
                table: "carros",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "publicado",
                table: "carros",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
