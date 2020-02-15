using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SistemaCalidad.Models;

namespace SistemaCalidad.Data
{
    public partial class CALIDADContext : DbContext
    {
        public virtual DbSet<Analisis> Analisis { get; set; }

        public virtual DbSet<Certificado> Certificado { get; set; }

        public virtual DbSet<AnalisisCertificado> AnalisisCertificado { get; set; }
        public virtual DbSet<Norma> Norma { get; set; }
        public virtual DbSet<TipoNorma> TipoNorma { get; set; }
        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<AnalisisMaterial> AnalisisMaterial { get; set; }
        public virtual DbSet<ClaseProducto> ClaseProducto { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<DetalleAnalisis> DetalleAnalisis { get; set; }
        public virtual DbSet<Especificacion> Especificacion { get; set; }
        public virtual DbSet<Maquina> Maquina { get; set; }
        public virtual DbSet<Material> Material { get; set; }
        public virtual DbSet<MaterialEspecificacion> MaterialEspecificacion { get; set; }
        public virtual DbSet<Producto> Producto { get; set; }
        public virtual DbSet<ProductoEspecificacion> ProductoEspecificacion { get; set; }
        public virtual DbSet<Proveedor> Proveedor { get; set; }
        public virtual DbSet<TipoMaterial> TipoMaterial { get; set; }
        public virtual DbSet<TipoMaterialEspecificacion> TipoMaterialEspecificacion { get; set; }

        public virtual DbSet<ProductoFinal> ProductoFinal { get; set; }
        public CALIDADContext(DbContextOptions<CALIDADContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {




            modelBuilder.Entity<ProductoFinal>(entity =>
            {
                entity.Property(e => e.Codigo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Producto)
                    .WithMany(p => p.ProductoFinal)
                    .HasForeignKey(d => d.ProductoId)
                    .HasConstraintName("FK_ProductoFinal_Producto");
            });

            modelBuilder.Entity<Analisis>(entity =>
            {
                entity.Property(e => e.FechaAnalisis).HasColumnType("datetime");
                entity.Property(e => e.Temperatura).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.NumeroOrden)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ObservacionesAprobado)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Observaciones)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NombreUsuario)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Resultado)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Cliente)
                    .WithMany(p => p.Analisis)
                    .HasForeignKey(d => d.ClienteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Analisis_Cliente");

                entity.HasOne(d => d.Maquina)
                    .WithMany(p => p.Analisis)
                    .HasForeignKey(d => d.MaquinaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Analisis_Maquina");

                entity.HasOne(d => d.Producto)
                    .WithMany(p => p.Analisis)
                    .HasForeignKey(d => d.ProductoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Analisis_Producto");
            });


            modelBuilder.Entity<AnalisisCertificado>(entity =>
            {
                entity.HasOne(d => d.Analisis)
                    .WithMany(p => p.AnalisisCertificado)
                    .HasForeignKey(d => d.AnalisisId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnalisisCertificado_Analisis");

                entity.HasOne(d => d.Certificado)
                    .WithMany(p => p.AnalisisCertificado)
                    .HasForeignKey(d => d.CertificadoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnalisisCertificado_Certificado");
            });

            modelBuilder.Entity<Certificado>(entity =>
            {
                entity.Property(e => e.CodigoCertificado)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.FechaGeneracion).HasColumnType("date");

                entity.Property(e => e.Valor).HasColumnType("decimal(18, 9)");
                entity.Property(e => e.Peso).HasColumnType("decimal(18, 9)");

                entity.Property(e => e.NumeroGuia)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.OrdenFinal)
                   .IsRequired()
                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.OrdenCliente)
                   .IsRequired()
                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.Referencia)
                   .IsRequired()
                   .HasMaxLength(500)
                   .IsUnicode(false);

                entity.Property(e => e.NumeroFactura)
                  .IsRequired()
                  .HasMaxLength(500)
                  .IsUnicode(false);

                entity.Property(e => e.NombreCliente)
                 .IsRequired()
                 .HasMaxLength(250)
                 .IsUnicode(false);

                entity.Property(e => e.PartidaArancelaria)
                  .IsRequired()
                  .HasMaxLength(500)
                  .IsUnicode(false);

                entity.Property(e => e.OrdenVenta)
                   .IsRequired()
                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.PedidoVenta)
                  .IsRequired()
                  .HasMaxLength(50)
                  .IsUnicode(false);

                entity.Property(e => e.FileUrl)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.NumeroOrden)
                    .IsRequired()
                    .HasMaxLength(500);
                
                entity.HasOne(d => d.ProductoFinal)
                    .WithMany(p => p.Certificado)
                    .HasForeignKey(d => d.ProductoFinalId)
                    .HasConstraintName("FK_Certificado_ProductoFinal");
            });

            modelBuilder.Entity<AnalisisMaterial>(entity =>
            {
                entity.HasKey(e => e.AnalisisMateriaId);

                entity.HasOne(d => d.Analisis)
                    .WithMany(p => p.AnalisisMaterial)
                    .HasForeignKey(d => d.AnalisisId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnalisisMaterial_Analisis");

                entity.HasOne(d => d.Materia)
                    .WithMany(p => p.AnalisisMaterial)
                    .HasForeignKey(d => d.MateriaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnalisisMaterial_Material");
            });

           
            modelBuilder.Entity<ClaseProducto>(entity =>
            {
                entity.Property(e => e.ClaseDescripcion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Norma>(entity =>
            {
                entity.Property(e => e.Activo).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Especificacion)
                    .WithMany(p => p.Norma)
                    .HasForeignKey(d => d.EspecificacionId)
                    .HasConstraintName("FK_Norma_Especificacion");

                entity.HasOne(d => d.TipoNorma)
                    .WithMany(p => p.Norma)
                    .HasForeignKey(d => d.TipoNormaId)
                    .HasConstraintName("FK_Norma_TipoNorma");
            });

            modelBuilder.Entity<Pais>(entity =>
            {
                entity.Property(e => e.DescripcionPais)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.Property(e => e.Activo).HasDefaultValueSql("((1))");

                entity.Property(e => e.CodigoCliente)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.NombreCliente)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);
                
            });

            modelBuilder.Entity<DetalleAnalisis>(entity =>
            {
                entity.Property(e => e.RangoReferenciaActual)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.AprobadoSupervisor).HasDefaultValueSql("((-1))");
                entity.Property(e => e.Resultado)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.HasOne(d => d.Analisis)
                    .WithMany(p => p.DetalleAnalisis)
                    .HasForeignKey(d => d.AnalisisId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DetalleAnalisis_Analisis");

                entity.HasOne(d => d.Especificacion)
                    .WithMany(p => p.DetalleAnalisis)
                    .HasForeignKey(d => d.EspecificacionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DetalleAnalisis_Especificacion");
            });


            modelBuilder.Entity<TipoNorma>(entity =>
            {
                entity.HasIndex(e => e.TipoNormaId)
                    .HasName("IX_TipoNorma")
                    .IsUnique();

                entity.Property(e => e.Activo).HasDefaultValueSql("((1))");

                entity.Property(e => e.DescripcionNorma)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Sae)
                    .HasColumnName("SAE")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Especificacion>(entity =>
            {
                entity.Property(e => e.Activo).HasDefaultValueSql("((1))");

                entity.Property(e => e.ClaseEspecificacion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.DescripcionIngles)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TipoEspecificacion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Maquina>(entity =>
            {
                entity.Property(e => e.NombreMaquina)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Material>(entity =>
            {
                

                entity.Property(e => e.CodigoIngreso)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Identificador)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StockDisponible).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.UnidadMedida)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Proveedor)
                    .WithMany(p => p.Material)
                    .HasForeignKey(d => d.ProveedorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Material_Proveedor");

                entity.HasOne(d => d.TipoNorma)
                    .WithMany(p => p.Material)
                    .HasForeignKey(d => d.TipoNormaId)
                    .HasConstraintName("FK_Material_TipoNorma");

                entity.HasOne(d => d.TipoMaterial)
                    .WithMany(p => p.Material)
                    .HasForeignKey(d => d.TipoMaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Material_TipoMaterial");
            });

            modelBuilder.Entity<MaterialEspecificacion>(entity =>
            {
                entity.Property(e => e.ValorEspecificacion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Especificacion)
                    .WithMany(p => p.MaterialEspecificacion)
                    .HasForeignKey(d => d.EspecificacionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MaterialEspecificacion_Especificacion");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.MaterialEspecificacion)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MaterialEspecificacion_Material");
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.Property(e => e.CodigoProducto)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DefUsuario1)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DefUsuario2)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DescripcionProducto)
                    .HasMaxLength(250)
                    .IsUnicode(false);

              
                entity.Property(e => e.DimensionMaxima).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.DimensionMinima).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.Grado)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nominal).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.ObservacionProducto)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.ClaseProducto)
                    .WithMany(p => p.Producto)
                    .HasForeignKey(d => d.ClaseProductoId)
                    .HasConstraintName("FK_Producto_ClaseProducto");
            });

            modelBuilder.Entity<ProductoEspecificacion>(entity =>
            {
                entity.Property(e => e.RangoMaximo).HasColumnType("decimal(18, 9)");

                entity.Property(e => e.RangoMinimo).HasColumnType("decimal(18, 9)");

                entity.Property(e => e.ValorEsperadoNum).HasColumnType("decimal(18, 9)");

                entity.Property(e => e.ValorEsperado)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Especificacion)
                    .WithMany(p => p.ProductoEspecificacion)
                    .HasForeignKey(d => d.EspecificacionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductoEspecificacion_Especificacion");

                entity.HasOne(d => d.Producto)
                    .WithMany(p => p.ProductoEspecificacion)
                    .HasForeignKey(d => d.ProductoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductoEspecificacion_Producto");
            });

            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.Property(e => e.CodigoProveedor)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NombreProveedor)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoMaterial>(entity =>
            {
                entity.Property(e => e.DescripcionTipoMaterial)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoMaterialEspecificacion>(entity =>
            {
                entity.HasOne(d => d.Especificacion)
                    .WithMany(p => p.TipoMaterialEspecificacion)
                    .HasForeignKey(d => d.EspecificacionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TipoMaterialEspecificacion_Especificacion");

                entity.HasOne(d => d.TipoMaterial)
                    .WithMany(p => p.TipoMaterialEspecificacion)
                    .HasForeignKey(d => d.TipoMaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TipoMaterialEspecificacion_TipoMaterial");
            });
        }
    }
}
