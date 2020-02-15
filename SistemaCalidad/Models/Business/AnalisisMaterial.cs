using System;
using System.Collections.Generic;

namespace SistemaCalidad.Models
{
    public partial class AnalisisMaterial
    {
        public int AnalisisMateriaId { get; set; }
        public int AnalisisId { get; set; }
        public int MateriaId { get; set; }

        public Analisis Analisis { get; set; }
        public Material Materia { get; set; }
    }
}
