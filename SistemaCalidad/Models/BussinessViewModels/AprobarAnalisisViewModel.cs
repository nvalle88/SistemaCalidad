using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models.BussinessViewModels
{
    public class AprobarAnalisisViewModel
    {
        public Analisis Analisis { get; set; }

        public int AnalisisId { get; set; }

        public List<String> AnalisisAprobados { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Observaciones")]
        [StringLength(maximumLength: 1000, ErrorMessage = Validaciones.LongitudString)]
        public string ObservacionesAprobado { get; set; }
    }
}
