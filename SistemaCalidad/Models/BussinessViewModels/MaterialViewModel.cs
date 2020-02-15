using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaCalidad.Models.BussinessViewModels
{
    public class MaterialViewModel
    {
        public List<Material> ListaMaretial { get; set; }
        public string CodigoIngeso { get; set; }
        public string Identificador { get; set; }
    }
}
