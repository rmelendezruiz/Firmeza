using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firmeza.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CountersYPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sedes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ciudad = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    direccion = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    telefono_peritaje = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    correo = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    horario = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    latitud = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitud = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    capacidad = table.Column<int>(type: "integer", nullable: false),
                    area_m2 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sedes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "activos_sede",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sede_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cantidad = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activos_sede", x => x.id);
                    table.ForeignKey(
                        name: "FK_activos_sede_sedes_sede_id",
                        column: x => x.sede_id,
                        principalTable: "sedes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fotos_sede",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sede_id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fotos_sede", x => x.id);
                    table.ForeignKey(
                        name: "FK_fotos_sede_sedes_sede_id",
                        column: x => x.sede_id,
                        principalTable: "sedes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sedes_ciudad",
                table: "sedes",
                column: "ciudad");

            migrationBuilder.CreateIndex(
                name: "IX_sedes_codigo",
                table: "sedes",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_activos_sede_sede_id_nombre",
                table: "activos_sede",
                columns: new[] { "sede_id", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fotos_sede_sede_id_orden",
                table: "fotos_sede",
                columns: new[] { "sede_id", "orden" },
                unique: true);

            migrationBuilder.Sql(
                """
                INSERT INTO sedes (id, codigo, nombre, ciudad, direccion, telefono, telefono_peritaje, correo, horario, latitud, longitud, capacidad, area_m2)
                VALUES
                    ('5ede0000-0000-4000-8000-000000000001', 'BAQ', 'Barranquilla', 'Barranquilla', 'Calle 76 # 49C-32', '+57 605 000 1101', '+57 300 000 1101', 'baq@firmeza.local', 'Lunes a viernes 8:00–18:00 · Sábados 9:00–14:00', 10.999600, -74.807700, 40, 1800),
                    ('5ede0000-0000-4000-8000-000000000002', 'CTG', 'Cartagena', 'Cartagena', 'Calle 30 # 47-15', '+57 605 000 2202', '+57 300 000 2202', 'ctg@firmeza.local', 'Lunes a viernes 8:00–18:00 · Sábados 9:00–14:00', 10.405000, -75.509800, 28, 1250),
                    ('5ede0000-0000-4000-8000-000000000003', 'SMR', 'Santa Marta', 'Santa Marta', 'Avenida Calle 22 # 22-40', '+57 605 000 3303', '+57 300 000 3303', 'smr@firmeza.local', 'Lunes a viernes 8:00–18:00 · Sábados 9:00–13:00', 11.235800, -74.188000, 22, 950);

                INSERT INTO fotos_sede (id, sede_id, url, descripcion, orden)
                VALUES
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000001', '/set/counters/baq-1.jpg', 'Fachada sobre la Calle 76', 1),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000001', '/set/counters/baq-2.jpg', 'Sala de exhibición', 2),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000001', '/set/counters/baq-3.jpg', 'Bahía de peritaje', 3),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000002', '/set/counters/ctg-1.jpg', 'Fachada sobre la Calle 30', 1),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000002', '/set/counters/ctg-2.jpg', 'Sala de exhibición', 2),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000002', '/set/counters/ctg-3.jpg', 'Patio de alistamiento', 3),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000003', '/set/counters/smr-1.jpg', 'Fachada con la Sierra Nevada al fondo', 1),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000003', '/set/counters/smr-2.jpg', 'Sala de exhibición', 2),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000003', '/set/counters/smr-3.jpg', 'Zona de entrega', 3);

                INSERT INTO activos_sede (id, sede_id, nombre, tipo, cantidad, estado)
                VALUES
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000001', 'Sala de exhibición cubierta', 'Infraestructura', 1, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000001', 'Patio de exhibición', 'Infraestructura', 1, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000001', 'Bahías de alistamiento', 'Infraestructura', 4, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000001', 'Elevador de dos columnas', 'Equipo', 2, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000001', 'Escáner de diagnóstico OBD-II', 'Equipo', 3, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000001', 'Medidor de espesor de pintura', 'Equipo', 2, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000002', 'Sala de exhibición cubierta', 'Infraestructura', 1, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000002', 'Bahías de alistamiento', 'Infraestructura', 2, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000002', 'Lavadero', 'Infraestructura', 1, 'Mantenimiento'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000002', 'Elevador de dos columnas', 'Equipo', 1, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000002', 'Escáner de diagnóstico OBD-II', 'Equipo', 2, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000003', 'Sala de exhibición cubierta', 'Infraestructura', 1, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000003', 'Bahías de alistamiento', 'Infraestructura', 2, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000003', 'Elevador de dos columnas', 'Equipo', 1, 'Mantenimiento'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000003', 'Escáner de diagnóstico OBD-II', 'Equipo', 1, 'Operativo'),
                    (gen_random_uuid(), '5ede0000-0000-4000-8000-000000000003', 'Medidor de espesor de pintura', 'Equipo', 1, 'Operativo');
                """);

            migrationBuilder.RenameTable(
                name: "fichas_comercial",
                newName: "fichas_equipo");

            migrationBuilder.Sql(
                """
                ALTER TABLE fichas_equipo RENAME CONSTRAINT "PK_fichas_comercial" TO "PK_fichas_equipo";
                """);

            migrationBuilder.RenameIndex(
                name: "IX_fichas_comercial_correo",
                table: "fichas_equipo",
                newName: "IX_fichas_equipo_correo");

            migrationBuilder.AddColumn<Guid>(
                name: "sede_id",
                table: "fichas_equipo",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_fichas_equipo_sede_id",
                table: "fichas_equipo",
                column: "sede_id");

            migrationBuilder.AddForeignKey(
                name: "FK_fichas_equipo_sedes_sede_id",
                table: "fichas_equipo",
                column: "sede_id",
                principalTable: "sedes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddColumn<Guid>(
                name: "sede_id",
                table: "carros",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE carros
                SET sede_id = sedes.id
                FROM sedes
                WHERE lower(sedes.ciudad) = lower(carros.ciudad);

                UPDATE carros
                SET sede_id = '5ede0000-0000-4000-8000-000000000001'
                WHERE sede_id IS NULL;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "sede_id",
                table: "carros",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropIndex(
                name: "IX_carros_ciudad",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "ciudad",
                table: "carros");

            migrationBuilder.CreateIndex(
                name: "IX_carros_sede_id",
                table: "carros",
                column: "sede_id");

            migrationBuilder.AddForeignKey(
                name: "FK_carros_sedes_sede_id",
                table: "carros",
                column: "sede_id",
                principalTable: "sedes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddColumn<Guid>(
                name: "sede_id",
                table: "ventas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "metodo_pago",
                table: "ventas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE ventas
                SET sede_id = carros.sede_id
                FROM carros
                WHERE carros.id = ventas.carro_id;

                UPDATE ventas
                SET sede_id = '5ede0000-0000-4000-8000-000000000001'
                WHERE sede_id IS NULL;

                UPDATE ventas
                SET metodo_pago = CASE WHEN canal = 'Pagina' THEN 'Transferencia' ELSE 'Efectivo' END;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "sede_id",
                table: "ventas",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "metodo_pago",
                table: "ventas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "");

            migrationBuilder.DropIndex(
                name: "IX_ventas_carro_id",
                table: "ventas");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_carro_id",
                table: "ventas",
                column: "carro_id",
                unique: true,
                filter: "carro_id IS NOT NULL AND estado <> 'Anulada'");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_fecha",
                table: "ventas",
                column: "fecha");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_sede_id_fecha",
                table: "ventas",
                columns: new[] { "sede_id", "fecha" });

            migrationBuilder.AddForeignKey(
                name: "FK_ventas_sedes_sede_id",
                table: "ventas",
                column: "sede_id",
                principalTable: "sedes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ventas_sedes_sede_id",
                table: "ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_carros_sedes_sede_id",
                table: "carros");

            migrationBuilder.DropForeignKey(
                name: "FK_fichas_equipo_sedes_sede_id",
                table: "fichas_equipo");

            migrationBuilder.DropIndex(
                name: "IX_ventas_carro_id",
                table: "ventas");

            migrationBuilder.DropIndex(
                name: "IX_ventas_fecha",
                table: "ventas");

            migrationBuilder.DropIndex(
                name: "IX_ventas_sede_id_fecha",
                table: "ventas");

            migrationBuilder.DropColumn(
                name: "metodo_pago",
                table: "ventas");

            migrationBuilder.DropColumn(
                name: "sede_id",
                table: "ventas");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_carro_id",
                table: "ventas",
                column: "carro_id");

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
                SET ciudad = sedes.ciudad
                FROM sedes
                WHERE sedes.id = carros.sede_id;
                """);

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

            migrationBuilder.DropIndex(
                name: "IX_carros_sede_id",
                table: "carros");

            migrationBuilder.DropColumn(
                name: "sede_id",
                table: "carros");

            migrationBuilder.CreateIndex(
                name: "IX_carros_ciudad",
                table: "carros",
                column: "ciudad");

            migrationBuilder.DropIndex(
                name: "IX_fichas_equipo_sede_id",
                table: "fichas_equipo");

            migrationBuilder.DropColumn(
                name: "sede_id",
                table: "fichas_equipo");

            migrationBuilder.RenameIndex(
                name: "IX_fichas_equipo_correo",
                table: "fichas_equipo",
                newName: "IX_fichas_comercial_correo");

            migrationBuilder.Sql(
                """
                ALTER TABLE fichas_equipo RENAME CONSTRAINT "PK_fichas_equipo" TO "PK_fichas_comercial";
                """);

            migrationBuilder.RenameTable(
                name: "fichas_equipo",
                newName: "fichas_comercial");

            migrationBuilder.DropTable(
                name: "activos_sede");

            migrationBuilder.DropTable(
                name: "fotos_sede");

            migrationBuilder.DropTable(
                name: "sedes");
        }
    }
}
