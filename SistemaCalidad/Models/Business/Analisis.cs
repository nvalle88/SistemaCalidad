using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models
{
    public partial class Analisis
    {
        public Analisis()
        {
            AnalisisMaterial = new HashSet<AnalisisMaterial>();
            DetalleAnalisis = new HashSet<DetalleAnalisis>();
        }

        public int AnalisisId { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaAnalisis { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "# Rollo")]
        [Range(1,int.MaxValue,ErrorMessage =("El {0} debe estar entre {1} y {2}"))]
        public int Rollo { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Número de orden")]
        [StringLength(maximumLength: 50, ErrorMessage = Validaciones.LongitudString)]
        public string NumeroOrden { get; set; }

        [Display(Name = "Producto")]
        [Range(1,int.MaxValue,ErrorMessage = "Valor")]
        public int ProductoId { get; set; }

        [Display(Name = "Cliente")]
        [Range(1, double.MaxValue, ErrorMessage = Validaciones.Requerido)]
        public int ClienteId { get; set; }

        [Display(Name = "Máquina")]
        [Range(1, double.MaxValue, ErrorMessage = Validaciones.Requerido)]
        public int MaquinaId { get; set; }


      
        public string ObservacionesAprobado { get; set; }
        

        public string Resultado { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Temperatura")]
        [Range(minimum: -40, maximum: 50, ErrorMessage = "La {0} debe estar entre {1} y {2} grados")]
        public decimal Temperatura { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Turno")]
        [Range(1, 5, ErrorMessage = Validaciones.Requerido)]
        public int Turno { get; set; }
        public string Observaciones { get; set; }

        public string NombreUsuario { get; set; }

        public Cliente Cliente { get; set; }
        public Maquina Maquina { get; set; }
        public Producto Producto { get; set; }
        [NotMapped]
        public MaterialEspecificacion MaterialEspecificacion { get; set; }
        public ICollection<AnalisisCertificado> AnalisisCertificado { get; set; }
        public ICollection<AnalisisMaterial> AnalisisMaterial { get; set; }
        public ICollection<DetalleAnalisis> DetalleAnalisis { get; set; }
    }
}
