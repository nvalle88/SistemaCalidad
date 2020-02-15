using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models.BussinessViewModels
{
    public class MaterialEspecificacionViewModel
    {
        public int MaterialEspecificacionId { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Range(0, double.MaxValue, ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Material")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Range(1, double.MaxValue, ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Especificación")]
        public int EspecificacionId { get; set; }
       
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Valor de la especificación")]
        public string ValorEspecificacion { get; set; }

        public Especificacion Especificacion { get; set; }
        
        public Material Material { get; set; }

        public TipoNorma TipoNorma { get; set; }

        public List<MaterialEspecificacion> MaterialEspecificacion { get; set; }
    }
}
