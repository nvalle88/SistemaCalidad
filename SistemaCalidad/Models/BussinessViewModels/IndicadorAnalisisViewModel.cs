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
    public class IndicadorAnalisisViewModel
    {
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Fecha")]
        [NotMapped]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Fecha { get; set; }
        public int Realizados { get; set; }
        public int Cumplen { get; set; }
        public int NoCumplen { get; set; }



        public List<Analisis> ListaAnalisis { get; set; }

    }
}