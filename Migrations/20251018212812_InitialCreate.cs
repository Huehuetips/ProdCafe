using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiEjemplo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    contacto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "etapas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreTostadoMoliendaEmpaque = table.Column<string>(name: "nombre(Tostado|Molienda|Empaque)", type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etapas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    fechaIngreso = table.Column<DateOnly>(type: "date", nullable: false),
                    fechaLote = table.Column<DateOnly>(type: "date", nullable: false),
                    fechaVencimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lotes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "presentaciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_presentaciones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "proveedores",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proveedores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rutas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    zona = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tiempoEstimadoH = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rutas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tiposGrano",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombrearábicarobustablends = table.Column<string>(name: "nombre(arábica|robusta|blends)", type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tiposGrano", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pedidos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_Cliente = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    prioritaria = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedidos", x => x.id);
                    table.ForeignKey(
                        name: "FK_pedidos_clientes_id_Cliente",
                        column: x => x.id_Cliente,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lote_etapa",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_Lote = table.Column<int>(type: "int", nullable: false),
                    id_Etapa = table.Column<int>(type: "int", nullable: false),
                    fechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fechaFin = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lote_etapa", x => x.id);
                    table.ForeignKey(
                        name: "FK_lote_etapa_etapas_id_Etapa",
                        column: x => x.id_Etapa,
                        principalTable: "etapas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lote_etapa_lotes_id_Lote",
                        column: x => x.id_Lote,
                        principalTable: "lotes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_Presentacion = table.Column<int>(type: "int", nullable: false),
                    nivelTostado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tipoMolido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    precio = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos", x => x.id);
                    table.ForeignKey(
                        name: "FK_productos_presentaciones_id_Presentacion",
                        column: x => x.id_Presentacion,
                        principalTable: "presentaciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ordenesCompra",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_proveedor = table.Column<int>(type: "int", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaEmision = table.Column<DateOnly>(type: "date", nullable: false),
                    fechaRecepcion = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordenesCompra", x => x.id);
                    table.ForeignKey(
                        name: "FK_ordenesCompra_proveedores_id_proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "proveedores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pedido_ruta",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_pedido = table.Column<int>(type: "int", nullable: false),
                    id_Ruta = table.Column<int>(type: "int", nullable: false),
                    fechaSalida = table.Column<DateOnly>(type: "date", nullable: false),
                    fechaEntrega = table.Column<DateOnly>(type: "date", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedido_ruta", x => x.id);
                    table.ForeignKey(
                        name: "FK_pedido_ruta_pedidos_id_pedido",
                        column: x => x.id_pedido,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedido_ruta_rutas_id_Ruta",
                        column: x => x.id_Ruta,
                        principalTable: "rutas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lotesTerminados",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_Lote = table.Column<int>(type: "int", nullable: false),
                    id_Producto = table.Column<int>(type: "int", nullable: false),
                    fechaEnvasado = table.Column<DateOnly>(type: "date", nullable: false),
                    fechaVencimiento = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lotesTerminados", x => x.id);
                    table.ForeignKey(
                        name: "FK_lotesTerminados_lotes_id_Lote",
                        column: x => x.id_Lote,
                        principalTable: "lotes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lotesTerminados_productos_id_Producto",
                        column: x => x.id_Producto,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ordenCompra_tipoGrano",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_OrdenCompra = table.Column<int>(type: "int", nullable: false),
                    id_TipoGrano = table.Column<int>(type: "int", nullable: false),
                    cantidadKg = table.Column<int>(type: "int", nullable: false),
                    precioUnitarioKg = table.Column<double>(type: "float", nullable: false),
                    precioTotal = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordenCompra_tipoGrano", x => x.id);
                    table.ForeignKey(
                        name: "FK_ordenCompra_tipoGrano_ordenesCompra_id_OrdenCompra",
                        column: x => x.id_OrdenCompra,
                        principalTable: "ordenesCompra",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordenCompra_tipoGrano_tiposGrano_id_TipoGrano",
                        column: x => x.id_TipoGrano,
                        principalTable: "tiposGrano",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "catacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_LoteTerminado = table.Column<int>(type: "int", nullable: false),
                    puntaje = table.Column<double>(type: "float", nullable: false),
                    humedad = table.Column<double>(type: "float", nullable: false),
                    notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    aprobado = table.Column<bool>(type: "bit", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catacion", x => x.id);
                    table.ForeignKey(
                        name: "FK_catacion_lotesTerminados_id_LoteTerminado",
                        column: x => x.id_LoteTerminado,
                        principalTable: "lotesTerminados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pedido_loteTerminado",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_LoteTerminado = table.Column<int>(type: "int", nullable: false),
                    id_Producto = table.Column<int>(type: "int", nullable: false),
                    id_Pedido = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedido_loteTerminado", x => x.id);
                    table.ForeignKey(
                        name: "FK_pedido_loteTerminado_lotesTerminados_id_LoteTerminado",
                        column: x => x.id_LoteTerminado,
                        principalTable: "lotesTerminados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedido_loteTerminado_pedidos_id_Pedido",
                        column: x => x.id_Pedido,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedido_loteTerminado_productos_id_Producto",
                        column: x => x.id_Producto,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ordenCompra_tipoGrano_lote",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_OrdenCompra_TipoGrano = table.Column<int>(type: "int", nullable: false),
                    id_Lote = table.Column<int>(type: "int", nullable: false),
                    cantidadKg = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordenCompra_tipoGrano_lote", x => x.id);
                    table.ForeignKey(
                        name: "FK_ordenCompra_tipoGrano_lote_lotes_id_Lote",
                        column: x => x.id_Lote,
                        principalTable: "lotes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordenCompra_tipoGrano_lote_ordenCompra_tipoGrano_id_OrdenCompra_TipoGrano",
                        column: x => x.id_OrdenCompra_TipoGrano,
                        principalTable: "ordenCompra_tipoGrano",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_catacion_id_LoteTerminado",
                table: "catacion",
                column: "id_LoteTerminado");

            migrationBuilder.CreateIndex(
                name: "IX_lote_etapa_id_Etapa",
                table: "lote_etapa",
                column: "id_Etapa");

            migrationBuilder.CreateIndex(
                name: "IX_lote_etapa_id_Lote",
                table: "lote_etapa",
                column: "id_Lote");

            migrationBuilder.CreateIndex(
                name: "IX_lotesTerminados_id_Lote",
                table: "lotesTerminados",
                column: "id_Lote");

            migrationBuilder.CreateIndex(
                name: "IX_lotesTerminados_id_Producto",
                table: "lotesTerminados",
                column: "id_Producto");

            migrationBuilder.CreateIndex(
                name: "IX_ordenCompra_tipoGrano_id_OrdenCompra",
                table: "ordenCompra_tipoGrano",
                column: "id_OrdenCompra");

            migrationBuilder.CreateIndex(
                name: "IX_ordenCompra_tipoGrano_id_TipoGrano",
                table: "ordenCompra_tipoGrano",
                column: "id_TipoGrano");

            migrationBuilder.CreateIndex(
                name: "IX_ordenCompra_tipoGrano_lote_id_Lote",
                table: "ordenCompra_tipoGrano_lote",
                column: "id_Lote");

            migrationBuilder.CreateIndex(
                name: "IX_ordenCompra_tipoGrano_lote_id_OrdenCompra_TipoGrano",
                table: "ordenCompra_tipoGrano_lote",
                column: "id_OrdenCompra_TipoGrano");

            migrationBuilder.CreateIndex(
                name: "IX_ordenesCompra_id_proveedor",
                table: "ordenesCompra",
                column: "id_proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_loteTerminado_id_LoteTerminado",
                table: "pedido_loteTerminado",
                column: "id_LoteTerminado");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_loteTerminado_id_Pedido",
                table: "pedido_loteTerminado",
                column: "id_Pedido");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_loteTerminado_id_Producto",
                table: "pedido_loteTerminado",
                column: "id_Producto");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_ruta_id_pedido",
                table: "pedido_ruta",
                column: "id_pedido");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_ruta_id_Ruta",
                table: "pedido_ruta",
                column: "id_Ruta");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_id_Cliente",
                table: "pedidos",
                column: "id_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_productos_id_Presentacion",
                table: "productos",
                column: "id_Presentacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "catacion");

            migrationBuilder.DropTable(
                name: "lote_etapa");

            migrationBuilder.DropTable(
                name: "ordenCompra_tipoGrano_lote");

            migrationBuilder.DropTable(
                name: "pedido_loteTerminado");

            migrationBuilder.DropTable(
                name: "pedido_ruta");

            migrationBuilder.DropTable(
                name: "etapas");

            migrationBuilder.DropTable(
                name: "ordenCompra_tipoGrano");

            migrationBuilder.DropTable(
                name: "lotesTerminados");

            migrationBuilder.DropTable(
                name: "pedidos");

            migrationBuilder.DropTable(
                name: "rutas");

            migrationBuilder.DropTable(
                name: "ordenesCompra");

            migrationBuilder.DropTable(
                name: "tiposGrano");

            migrationBuilder.DropTable(
                name: "lotes");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "proveedores");

            migrationBuilder.DropTable(
                name: "presentaciones");
        }
    }
}
