using System;
using System.Collections.Generic;

namespace SistemaCalidad.Models
{
    public partial class MaterialEspecificacion
    {
        public int MaterialEspecificacionId { get; set; }
        public int MaterialId { get; set; }
        public int EspecificacionId { get; set; }
        public string ValorEspecificacion { get; set; }

        public Especificacion Especificacion { get; set; }
        public Material Material { get; set; }
    }
}
