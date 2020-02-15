using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaCalidad.Models.BussinessViewModels
{
    public class CertificadoViewModel
    {
        public int Tipo { get; set; }
        public List<Certificado> ListaCertificados { get; set; }
    }
}
