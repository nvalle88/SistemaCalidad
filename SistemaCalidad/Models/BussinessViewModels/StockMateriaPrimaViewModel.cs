using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models.BussinessViewModels
{
    public class StockMaterialViewModel
    {
        public int MaterialId { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Valor")]
        public int StockResta { get; set; }
        public decimal StockDisponible { get; set; }
        public string UnidadMedida { get; set; }
        public string CodigoIngreso { get; set; }
        public string Identificador { get; set; }
        public string SAE  { get; set; }
        public Proveedor Proveedor { get; set; }
        public Pais Pais { get; set; }

    }
}
