using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models
{
    public partial class Pais
    {
        public Pais()
        {
            Material = new HashSet<Material>();
        }

        public int PaisId { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "País")]
        public string DescripcionPais { get; set; }

        public ICollection<Material> Material { get; set; }
    }
}
