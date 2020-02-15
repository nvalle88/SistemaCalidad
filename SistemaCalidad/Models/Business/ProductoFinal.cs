using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models
{
    public partial class ProductoFinal
    {
        public ProductoFinal()
        {
            Certificado = new HashSet<Certificado>();
        }

        public int ProductoFinalId { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(500, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Producto")]
        public int? ProductoId { get; set; }

        public Producto Producto { get; set; }

        public ICollection<Certificado> Certificado { get; set; }
    }
}
