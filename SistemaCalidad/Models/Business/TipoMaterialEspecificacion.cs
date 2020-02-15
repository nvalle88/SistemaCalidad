using System;
using System.Collections.Generic;

namespace SistemaCalidad.Models
{
    public partial class TipoMaterialEspecificacion
    {
        public int TipoMaterialEspecificacionId { get; set; }
        public int EspecificacionId { get; set; }
        public int TipoMaterialId { get; set; }

        public Especificacion Especificacion { get; set; }
        public TipoMaterial TipoMaterial { get; set; }
    }
}
