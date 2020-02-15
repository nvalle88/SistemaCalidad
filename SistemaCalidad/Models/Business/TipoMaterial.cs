using SistemaCalidad.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaCalidad.Models
{
    public partial class TipoMaterial
    {
        public TipoMaterial()
        {
            Material = new HashSet<Material>();
            TipoMaterialEspecificacion = new HashSet<TipoMaterialEspecificacion>();
        }

        public int TipoMaterialId { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Descripción Tipo Material")]
        public string DescripcionTipoMaterial { get; set; }

        public ICollection<Material> Material { get; set; }
        public ICollection<TipoMaterialEspecificacion> TipoMaterialEspecificacion { get; set; }
    }
}
