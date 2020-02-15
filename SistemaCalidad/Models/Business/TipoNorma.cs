using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models
{
    public partial class TipoNorma
    {
        public TipoNorma()
        {
            Norma = new HashSet<Norma>();
        }

        public int TipoNormaId { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Descripción")]
        public string DescripcionNorma { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "SAE")]
        public string Sae { get; set; }
        public bool? Activo { get; set; }

        public ICollection<Norma> Norma { get; set; }
        public ICollection<Material> Material { get; set; }
    }
}
