using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models
{
    public partial class ProductoEspecificacion
    {
        public int ProductoEspecificacionId { get; set; }
        public int ProductoId { get; set; }
        public int EspecificacionId { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Valor Esperado")]
        public string ValorEsperado { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Rango Mínimo")]
        [DisplayFormat(DataFormatString = "{0:#.#####}")]
        public decimal? RangoMinimo { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Rango Máximo")]
        [DisplayFormat(DataFormatString = "{0:#.#####}")]
        public decimal? RangoMaximo { get; set; }

        public decimal? ValorEsperadoNum { get; set; }

        public Especificacion Especificacion { get; set; }
        public Producto Producto { get; set; }
    }
}
