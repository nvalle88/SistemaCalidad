using System;
using System.Collections.Generic;

namespace SistemaCalidad.Models
{
    public partial class Norma
    {
        public int NormaId { get; set; }
        public int? TipoNormaId { get; set; }
        public int? EspecificacionId { get; set; }
        public decimal? ValorMinimo { get; set; }
        public decimal? ValorMaximo { get; set; }
        public bool? Activo { get; set; }

        public Especificacion Especificacion { get; set; }
        public TipoNorma TipoNorma { get; set; }
    }
}
