using System;
using System.Collections.Generic;

namespace SistemaCalidad.Models
{
    public partial class AnalisisCertificado
    {
        public int AnalisisCertificadoId { get; set; }
        public int CertificadoId { get; set; }
        public int AnalisisId { get; set; }

        public Analisis Analisis { get; set; }
        public Certificado Certificado { get; set; }
    }
}
