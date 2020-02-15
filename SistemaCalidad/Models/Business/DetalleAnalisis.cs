using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCalidad.Models
{
    public partial class DetalleAnalisis
    {
        public int DetalleAnalisisId { get; set; }
        public int AnalisisId { get; set; }
        public int EspecificacionId { get; set; }
        public string Resultado { get; set; }
        public string RangoReferenciaActual { get; set; }
        public bool Aprobado { get; set; }
        public int AprobadoSupervisor { get; set; }
       
        public Analisis Analisis { get; set; }
        public Especificacion Especificacion { get; set; }
    }
}
