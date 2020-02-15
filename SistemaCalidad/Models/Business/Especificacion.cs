using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models
{
    public partial class Especificacion
    {
        public Especificacion()
        {
            DetalleAnalisis = new HashSet<DetalleAnalisis>();
            MaterialEspecificacion = new HashSet<MaterialEspecificacion>();
            ProductoEspecificacion = new HashSet<ProductoEspecificacion>();
            TipoMaterialEspecificacion = new HashSet<TipoMaterialEspecificacion>();
        }

        public int EspecificacionId { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(250, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }


        
        [StringLength(250, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Descripción Inglés")]
        public string DescripcionIngles { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Tipo")]
        public string TipoEspecificacion { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(50, ErrorMessage = Validaciones.LongitudString)]
        [Display(Name = "Clase")]
        public string ClaseEspecificacion { get; set; }
        public bool Analisis { get; set; }

        public bool? Activo { get; set; }

        [NotMapped]
        public string ValorEspecificacion { get; set; }

        [NotMapped]
        public bool NoEditable { get; set; }

        public ICollection<DetalleAnalisis> DetalleAnalisis { get; set; }
        public ICollection<Norma> Norma { get; set; }
        public ICollection<MaterialEspecificacion> MaterialEspecificacion { get; set; }
        public ICollection<ProductoEspecificacion> ProductoEspecificacion { get; set; }
        public ICollection<TipoMaterialEspecificacion> TipoMaterialEspecificacion { get; set; }
    }
}
