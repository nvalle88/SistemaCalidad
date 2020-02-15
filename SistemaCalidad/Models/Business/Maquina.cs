using SistemaCalidad.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaCalidad.Models
{
    public partial class Maquina
    {
        public Maquina()
        {
            Analisis = new HashSet<Analisis>();
        }

        public int MaquinaId { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50,ErrorMessage =Validaciones.LongitudString)]
        [Display(Name ="Descripción de la Máquina")]
        public string NombreMaquina { get; set; }

        public ICollection<Analisis> Analisis { get; set; }
    }
}
