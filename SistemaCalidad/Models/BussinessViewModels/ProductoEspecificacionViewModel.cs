using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models.BussinessViewModels
{
    public class ProductoEspecificacionViewModel
    {

        public int ProductoEspecificacionId { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Range(0, double.MaxValue, ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Range(1, double.MaxValue, ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Especificación")]
        public int EspecificacionId { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Valor Esperado")]
        public string ValorEsperado { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Valor")]
        public decimal? ValorEsperadoNum { get; set; }

       
        [Display(Name = "Rango Mínimo")]
        [DisplayFormat(DataFormatString = "{0:#.#########}")]
        public decimal? RangoMinimo { get; set; }

        
        [Display(Name = "Rango Máximo")]
        [DisplayFormat(DataFormatString = "{0:#.#########}")]
        public decimal? RangoMaximo { get; set; }

        public Especificacion Especificacion { get; set; }
        public Producto Producto { get; set; }

        public List<ProductoEspecificacion> ProductoEspecificacion { get; set; }

       
    }
}
