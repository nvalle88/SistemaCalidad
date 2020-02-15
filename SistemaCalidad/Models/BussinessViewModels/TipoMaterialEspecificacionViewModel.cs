using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models.BussinessViewModels
{
    public class TipoMaterialEspecificacionViewModel
    {
        public int TipoMaterialId { get; set; }
        
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Range(1, double.MaxValue, ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Especificación")]
        public int EspecificacionId { get; set; }

        public Especificacion Especificacion { get; set; }
        public TipoMaterial TipoMaterial { get; set; }

        public List<TipoMaterialEspecificacion> TipoMaterialEspecificacion { get; set; }
    }
}
