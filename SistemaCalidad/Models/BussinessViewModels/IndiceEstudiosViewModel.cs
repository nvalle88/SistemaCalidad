using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaCalidad.Models.BussinessViewModels
{
    public class IndiceEstudiosViewModel
    {
        public string Dia { get; set; }
        public int Realizados { get; set; }
        public int Cumple { get; set; }
        public int NoCumple { get; set; }

        public decimal CumplePorcentaje { get; set; }
        public decimal NoCumplePorcentaje { get; set; }

    }
}
