using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models
{
    public partial class ClaseProducto
    {
        public ClaseProducto()
        {
            Producto = new HashSet<Producto>();
        }

        public int ClaseProductoId { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Descripción")]
        public string ClaseDescripcion { get; set; }

        public ICollection<Producto> Producto { get; set; }
    }
}
