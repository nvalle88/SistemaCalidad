using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models.BussinessViewModels
{
    public class TipoNormaEspecificacionViewModel
    {
        public int TipoNormaId { get; set; }
        
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Range(1, double.MaxValue, ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Especificación")]
        public int EspecificacionId { get; set; }
        public int NormaId { get; set; }
       
        [Display(Name = "Valor Mínimo")]
        public decimal? ValorMinimo { get; set; }
      
        [Display(Name = "Valor Máximo")]
        public decimal? ValorMaximo { get; set; }
        public bool? Activo { get; set; }
        public Especificacion Especificacion { get; set; }
        public TipoNorma TipoNorma { get; set; }
        public List<Norma> Normas { get; set; }
    }
}
