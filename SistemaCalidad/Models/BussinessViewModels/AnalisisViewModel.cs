using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using SistemaCalidad.Utils;
using Microsoft.EntityFrameworkCore;

namespace SistemaCalidad.Models.BussinessViewModels
{
    public class AnalisisViewModel
    {
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Fecha inicio")]
        [NotMapped]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? FechaInicio { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Fecha final")]
        [NotMapped]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? FechaFin { get; set; }
        public List<Analisis> ListaAnalisis { get; set; }

    }
}
