using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SistemaCalidad.Migrations
{
    public partial class ejemplo_12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClaseProducto",
                columns: table => new
                {
                    ClaseProductoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaseDescripcion = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaseProducto", x => x.ClaseProductoId);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    ClienteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Activo = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    CodigoCliente = table.Column<string>(unicode: false, maxLength: 150, nullable: false),
                    NombreCliente = table.Column<string>(unicode: false, maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "Especificacion",
                columns: table => new
                {
                    EspecificacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Activo = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    Analisis = table.Column<bool>(nullable: false),
                    ClaseEspecificacion = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(unicode: false, maxLength: 250, nullable: false),
                    DescripcionIngles = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    TipoEspecificacion = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especificacion", x => x.EspecificacionId);
                });

            migrationBuilder.CreateTable(
                name: "Maquina",
                columns: table => new
                {
                    MaquinaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NombreMaquina = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maquina", x => x.MaquinaId);
                });

            migrationBuilder.CreateTable(
                name: "Pais",
                columns: table => new
                {
                    PaisId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DescripcionPais = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pais", x => x.PaisId);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    ProveedorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodigoProveedor = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    NombreProveedor = table.Column<string>(unicode: false, maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.ProveedorId);
                });

            migrationBuilder.CreateTable(
                name: "TipoMaterial",
                columns: table => new
                {
                    TipoMaterialId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DescripcionTipoMaterial = table.Column<string>(unicode: false, maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoMaterial", x => x.TipoMaterialId);
                });

            migrationBuilder.CreateTable(
                name: "TipoNorma",
                columns: table => new
                {
                    TipoNormaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Activo = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    DescripcionNorma = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    SAE = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoNorma", x => x.TipoNormaId);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    ProductoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaseProductoId = table.Column<int>(nullable: false),
                    CodigoProducto = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    DefUsuario1 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    DefUsuario2 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    DescripcionProducto = table.Column<string>(unicode: false, maxLength: 250, nullable: false),
                    DimensionMaxima = table.Column<decimal>(type: "decimal(18, 5)", nullable: true),
                    DimensionMinima = table.Column<decimal>(type: "decimal(18, 5)", nullable: true),
                    Grado = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Nominal = table.Column<decimal>(type: "decimal(18, 5)", nullable: false),
                    ObservacionProducto = table.Column<string>(unicode: false, maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.ProductoId);
                    table.ForeignKey(
                        name: "FK_Producto_ClaseProducto",
                        column: x => x.ClaseProductoId,
                        principalTable: "ClaseProducto",
                        principalColumn: "ClaseProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipoMaterialEspecificacion",
                columns: table => new
                {
                    TipoMaterialEspecificacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EspecificacionId = table.Column<int>(nullable: false),
                    TipoMaterialId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoMaterialEspecificacion", x => x.TipoMaterialEspecificacionId);
                    table.ForeignKey(
                        name: "FK_TipoMaterialEspecificacion_Especificacion",
                        column: x => x.EspecificacionId,
                        principalTable: "Especificacion",
                        principalColumn: "EspecificacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TipoMaterialEspecificacion_TipoMaterial",
                        column: x => x.TipoMaterialId,
                        principalTable: "TipoMaterial",
                        principalColumn: "TipoMaterialId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    MaterialId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Aprobado = table.Column<bool>(nullable: false),
                    CodigoIngreso = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Identificador = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    PaisId = table.Column<int>(nullable: true),
                    ProveedorId = table.Column<int>(nullable: false),
                    StockDisponible = table.Column<decimal>(type: "decimal(18, 5)", nullable: false),
                    TipoMaterialId = table.Column<int>(nullable: false),
                    TipoNormaId = table.Column<int>(nullable: true),
                    UnidadMedida = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.MaterialId);
                    table.ForeignKey(
                        name: "FK_Material_Pais_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Pais",
                        principalColumn: "PaisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Material_Proveedor",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Material_TipoMaterial",
                        column: x => x.TipoMaterialId,
                        principalTable: "TipoMaterial",
                        principalColumn: "TipoMaterialId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Material_TipoNorma",
                        column: x => x.TipoNormaId,
                        principalTable: "TipoNorma",
                        principalColumn: "TipoNormaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Norma",
                columns: table => new
                {
                    NormaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Activo = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    EspecificacionId = table.Column<int>(nullable: true),
                    TipoNormaId = table.Column<int>(nullable: true),
                    ValorMaximo = table.Column<decimal>(nullable: true),
                    ValorMinimo = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Norma", x => x.NormaId);
                    table.ForeignKey(
                        name: "FK_Norma_Especificacion",
                        column: x => x.EspecificacionId,
                        principalTable: "Especificacion",
                        principalColumn: "EspecificacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Norma_TipoNorma",
                        column: x => x.TipoNormaId,
                        principalTable: "TipoNorma",
                        principalColumn: "TipoNormaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Analisis",
                columns: table => new
                {
                    AnalisisId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(nullable: false),
                    FechaAnalisis = table.Column<DateTime>(type: "datetime", nullable: false),
                    MaquinaId = table.Column<int>(nullable: false),
                    NombreUsuario = table.Column<string>(unicode: false, maxLength: 256, nullable: false),
                    NumeroOrden = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Observaciones = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    ObservacionesAprobado = table.Column<string>(unicode: false, maxLength: 1000, nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    Resultado = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Rollo = table.Column<int>(nullable: false),
                    Temperatura = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Turno = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analisis", x => x.AnalisisId);
                    table.ForeignKey(
                        name: "FK_Analisis_Cliente",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analisis_Maquina",
                        column: x => x.MaquinaId,
                        principalTable: "Maquina",
                        principalColumn: "MaquinaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analisis_Producto",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductoEspecificacion",
                columns: table => new
                {
                    ProductoEspecificacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EspecificacionId = table.Column<int>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    RangoMaximo = table.Column<decimal>(type: "decimal(18, 9)", nullable: false),
                    RangoMinimo = table.Column<decimal>(type: "decimal(18, 9)", nullable: false),
                    ValorEsperado = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    ValorEsperadoNum = table.Column<decimal>(type: "decimal(18, 9)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoEspecificacion", x => x.ProductoEspecificacionId);
                    table.ForeignKey(
                        name: "FK_ProductoEspecificacion_Especificacion",
                        column: x => x.EspecificacionId,
                        principalTable: "Especificacion",
                        principalColumn: "EspecificacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductoEspecificacion_Producto",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductoFinal",
                columns: table => new
                {
                    ProductoFinalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Codigo = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    ProductoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoFinal", x => x.ProductoFinalId);
                    table.ForeignKey(
                        name: "FK_ProductoFinal_Producto",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialEspecificacion",
                columns: table => new
                {
                    MaterialEspecificacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EspecificacionId = table.Column<int>(nullable: false),
                    MaterialId = table.Column<int>(nullable: false),
                    ValorEspecificacion = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialEspecificacion", x => x.MaterialEspecificacionId);
                    table.ForeignKey(
                        name: "FK_MaterialEspecificacion_Especificacion",
                        column: x => x.EspecificacionId,
                        principalTable: "Especificacion",
                        principalColumn: "EspecificacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialEspecificacion_Material",
                        column: x => x.MaterialId,
                        principalTable: "Material",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnalisisMaterial",
                columns: table => new
                {
                    AnalisisMateriaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnalisisId = table.Column<int>(nullable: false),
                    MateriaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalisisMaterial", x => x.AnalisisMateriaId);
                    table.ForeignKey(
                        name: "FK_AnalisisMaterial_Analisis",
                        column: x => x.AnalisisId,
                        principalTable: "Analisis",
                        principalColumn: "AnalisisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnalisisMaterial_Material",
                        column: x => x.MateriaId,
                        principalTable: "Material",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetalleAnalisis",
                columns: table => new
                {
                    DetalleAnalisisId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnalisisId = table.Column<int>(nullable: false),
                    Aprobado = table.Column<bool>(nullable: false),
                    AprobadoSupervisor = table.Column<int>(nullable: false, defaultValueSql: "((-1))"),
                    EspecificacionId = table.Column<int>(nullable: false),
                    RangoReferenciaActual = table.Column<string>(unicode: false, maxLength: 150, nullable: false),
                    Resultado = table.Column<string>(unicode: false, maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleAnalisis", x => x.DetalleAnalisisId);
                    table.ForeignKey(
                        name: "FK_DetalleAnalisis_Analisis",
                        column: x => x.AnalisisId,
                        principalTable: "Analisis",
                        principalColumn: "AnalisisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleAnalisis_Especificacion",
                        column: x => x.EspecificacionId,
                        principalTable: "Especificacion",
                        principalColumn: "EspecificacionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificado",
                columns: table => new
                {
                    CertificadoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ArchivoCargado = table.Column<bool>(nullable: false),
                    CodigoCertificado = table.Column<string>(unicode: false, maxLength: 150, nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "date", nullable: false),
                    FileUrl = table.Column<string>(unicode: false, maxLength: 1000, nullable: false),
                    Liberado = table.Column<bool>(nullable: true),
                    NombreCliente = table.Column<string>(unicode: false, maxLength: 250, nullable: false),
                    NumeroFactura = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    NumeroGuia = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    NumeroOrden = table.Column<string>(maxLength: 500, nullable: false),
                    OrdenCliente = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    OrdenFinal = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    OrdenVenta = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    PartidaArancelaria = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    PedidoVenta = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Peso = table.Column<decimal>(type: "decimal(18, 9)", nullable: true),
                    ProductoFinalId = table.Column<int>(nullable: true),
                    Referencia = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    Tipo = table.Column<int>(nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18, 9)", nullable: true),
                    VerMateriaPrima = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificado", x => x.CertificadoId);
                    table.ForeignKey(
                        name: "FK_Certificado_ProductoFinal",
                        column: x => x.ProductoFinalId,
                        principalTable: "ProductoFinal",
                        principalColumn: "ProductoFinalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnalisisCertificado",
                columns: table => new
                {
                    AnalisisCertificadoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnalisisId = table.Column<int>(nullable: false),
                    CertificadoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalisisCertificado", x => x.AnalisisCertificadoId);
                    table.ForeignKey(
                        name: "FK_AnalisisCertificado_Analisis",
                        column: x => x.AnalisisId,
                        principalTable: "Analisis",
                        principalColumn: "AnalisisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnalisisCertificado_Certificado",
                        column: x => x.CertificadoId,
                        principalTable: "Certificado",
                        principalColumn: "CertificadoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analisis_ClienteId",
                table: "Analisis",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Analisis_MaquinaId",
                table: "Analisis",
                column: "MaquinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Analisis_ProductoId",
                table: "Analisis",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalisisCertificado_AnalisisId",
                table: "AnalisisCertificado",
                column: "AnalisisId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalisisCertificado_CertificadoId",
                table: "AnalisisCertificado",
                column: "CertificadoId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalisisMaterial_AnalisisId",
                table: "AnalisisMaterial",
                column: "AnalisisId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalisisMaterial_MateriaId",
                table: "AnalisisMaterial",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificado_ProductoFinalId",
                table: "Certificado",
                column: "ProductoFinalId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleAnalisis_AnalisisId",
                table: "DetalleAnalisis",
                column: "AnalisisId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleAnalisis_EspecificacionId",
                table: "DetalleAnalisis",
                column: "EspecificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_PaisId",
                table: "Material",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_ProveedorId",
                table: "Material",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_TipoMaterialId",
                table: "Material",
                column: "TipoMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_TipoNormaId",
                table: "Material",
                column: "TipoNormaId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialEspecificacion_EspecificacionId",
                table: "MaterialEspecificacion",
                column: "EspecificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialEspecificacion_MaterialId",
                table: "MaterialEspecificacion",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Norma_EspecificacionId",
                table: "Norma",
                column: "EspecificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Norma_TipoNormaId",
                table: "Norma",
                column: "TipoNormaId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_ClaseProductoId",
                table: "Producto",
                column: "ClaseProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoEspecificacion_EspecificacionId",
                table: "ProductoEspecificacion",
                column: "EspecificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoEspecificacion_ProductoId",
                table: "ProductoEspecificacion",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoFinal_ProductoId",
                table: "ProductoFinal",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoMaterialEspecificacion_EspecificacionId",
                table: "TipoMaterialEspecificacion",
                column: "EspecificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoMaterialEspecificacion_TipoMaterialId",
                table: "TipoMaterialEspecificacion",
                column: "TipoMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoNorma",
                table: "TipoNorma",
                column: "TipoNormaId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalisisCertificado");

            migrationBuilder.DropTable(
                name: "AnalisisMaterial");

            migrationBuilder.DropTable(
                name: "DetalleAnalisis");

            migrationBuilder.DropTable(
                name: "MaterialEspecificacion");

            migrationBuilder.DropTable(
                name: "Norma");

            migrationBuilder.DropTable(
                name: "ProductoEspecificacion");

            migrationBuilder.DropTable(
                name: "TipoMaterialEspecificacion");

            migrationBuilder.DropTable(
                name: "Certificado");

            migrationBuilder.DropTable(
                name: "Analisis");

            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropTable(
                name: "Especificacion");

            migrationBuilder.DropTable(
                name: "ProductoFinal");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Maquina");

            migrationBuilder.DropTable(
                name: "Pais");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "TipoMaterial");

            migrationBuilder.DropTable(
                name: "TipoNorma");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "ClaseProducto");
        }
    }
}
