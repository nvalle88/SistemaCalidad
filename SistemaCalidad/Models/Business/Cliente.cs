using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models
{
    public partial class Cliente
    {
        public Cliente()
        {
            Analisis = new HashSet<Analisis>();
           
        }

        [Key]
        public int ClienteId { get; set; }
        
        [Display(Name = "Código")]
        public string CodigoCliente { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Nombres y apellidos")]
        public string NombreCliente { get; set; }

        public bool? Activo { get; set; }

        public ICollection<Analisis> Analisis { get; set; }



    }
}
