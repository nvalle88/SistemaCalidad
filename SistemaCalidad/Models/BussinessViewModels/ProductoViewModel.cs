using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaCalidad.Models.BussinessViewModels
{
    public class ProductoViewModel
    {
        public string CodigoPtroducto { get; set; }
        public List<Producto> ListaProductos { get; set; }
    }
}
