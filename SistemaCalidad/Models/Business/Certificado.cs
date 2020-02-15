using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Models
{
    public partial class Certificado
    {
        public Certificado()
        {
            AnalisisCertificado = new HashSet<AnalisisCertificado>();
        }

        public int CertificadoId { get; set; }

        [Required(ErrorMessage = Validaciones.Requerido)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaGeneracion { get; set; }
        public string CodigoCertificado { get; set; }
        public int? ProductoFinalId { get; set; }

        
        [Display(Name = "Número de Guía")]
        public string NumeroGuia { get; set; }

        [Display(Name = "Número de Guía")]
        public string OrdenCliente { get; set; }

        [Display(Name = "Número de Factura")]
        public string NumeroFactura { get; set; }

        [Display(Name = "Partida Arancelaria")]
        public string PartidaArancelaria { get; set; }
        [Required(ErrorMessage = Validaciones.Requerido)]
        [StringLength(maximumLength:50,ErrorMessage =Validaciones.LongitudString)]
        [Display(Name = "Número de Orden Final")]
        public string OrdenFinal { get; set; }
        [Display(Name = "Referncia")]
        public string Referencia { get; set; }
        

        [Required(ErrorMessage = Validaciones.Requerido)]
        [Display(Name = "Número de Orden")]
        public string NumeroOrden { get; set; }

        public string FileUrl { get; set; }

        public bool ArchivoCargado { get; set; }

        public int Tipo { get; set; }
       

        [Display(Name = "Orden de Venta")]
        public string OrdenVenta { get; set; }

        
        [Display(Name = "Pedido de Venta")]
        public string PedidoVenta { get; set; }

        
        [Display(Name = "Valor")]
        public decimal? Valor { get; set; }

        public string NombreCliente { get; set; }

        [Display(Name = "Peso (Kg)")]
        public decimal? Peso { get; set; }

        public bool VerMateriaPrima { get; set; }

        public bool? Liberado { get; set; }

        [NotMapped]
        public bool CumpleFechaOrdenes { get; set; }
        

        [NotMapped]
        public string[] ListaOrdenes { get; set; }


        /// <summary>
        /// Estado para la vista, para saber en que estado se encuentra la generación del certificado
        /// 0:Acaba de abrir la pantalla de nnuevo
        /// 1:Insertado Temporalmente y se muestra el input para subir el fichero...
        /// </summary>
        [NotMapped]
        public int Estado { get; set; }

        public ProductoFinal ProductoFinal { get; set; }

       
        public ICollection<AnalisisCertificado> AnalisisCertificado { get; set; }
    }
}
