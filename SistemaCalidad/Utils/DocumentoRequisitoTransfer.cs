using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaCalidad.Models.Utiles
{
    public class DocumentoTransfer
    {
        public string Nombre { get; set; }
        public byte[] Fichero { get; set; }
        public int? IdRequisito { get; set; }
    }
}
