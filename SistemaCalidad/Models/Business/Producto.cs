using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models
{
    public partial class Producto
    {
        public Producto()
        {
            Analisis = new HashSet<Analisis>();
            ProductoEspecificacion = new HashSet<ProductoEspecificacion>();
        }

        public int ProductoId { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Código")]
        public string CodigoProducto { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(250, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Descripción")]
        public string DescripcionProducto { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(500, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Observaciones")]
        public string ObservacionProducto { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Grado")]
        public string Grado { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Range(1,double.MaxValue,ErrorMessage =Validaciones.Requerido)]
        [Display(Name = "Clase")]
        public int? ClaseProductoId { get; set; }
        
        [Display(Name = "Dimensión mínima")]
        [DisplayFormat(DataFormatString = "{0:#.#####}")]
        public decimal? DimensionMinima { get; set; }

      
        [Display(Name = "Dimensión máxima")]
        [DisplayFormat(DataFormatString = "{0:#.#####}")]
        public decimal? DimensionMaxima { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Diametro")]
        [DisplayFormat(DataFormatString = "{0:#.#####}")]
        public decimal? Nominal { get; set; }
        public string DefUsuario1 { get; set; }
        public string DefUsuario2 { get; set; }

        [NotMapped]
        public int ClaseProductoIdFiltar { get; set; }

        public ClaseProducto ClaseProducto { get; set; }
        public ICollection<Analisis> Analisis { get; set; }
        public ICollection<ProductoEspecificacion> ProductoEspecificacion { get; set; }

        public ICollection<ProductoFinal> ProductoFinal { get; set; }
    }
}
