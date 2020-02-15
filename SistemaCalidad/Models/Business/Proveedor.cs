using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models
{
    public partial class Proveedor
    {
        public Proveedor()
        {
            Material = new HashSet<Material>();
        }

        [Key]
        public int ProveedorId { get; set; }


        [Required(ErrorMessage =Validaciones.Requerido)]
        [StringLength(50,ErrorMessage =Validaciones.LongitudString)]
        [Display(Name = "Código")]
        public string CodigoProveedor { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Nombres y apellidos")]
        public string NombreProveedor { get; set; }

        public ICollection<Material> Material { get; set; }
    }
}
