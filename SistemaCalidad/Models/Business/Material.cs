using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCalidad.Models
{
    public partial class Material
    {
        public Material()
        {
            AnalisisMaterial = new HashSet<AnalisisMaterial>();
            MaterialEspecificacion = new HashSet<MaterialEspecificacion>();
        }

        public int MaterialId { get; set; }
        public int TipoMaterialId { get; set; }
        public decimal StockDisponible { get; set; }
        public string UnidadMedida { get; set; }
        public int ProveedorId { get; set; }
        public string CodigoIngreso { get; set; }
        public string Identificador { get; set; }
       
        public bool Aprobado { get; set; }

        [NotMapped]
        public int CantidadAnalisis { get; set; }

        [NotMapped]
        public int CantidadEspecificaciones { get; set; }

        [NotMapped]
        public MaterialEspecificacion Diametro { get; set; }

        public int? PaisId { get; set; }
        public int? TipoNormaId { get; set; }

        public TipoNorma TipoNorma { get; set; }
        public Pais Pais { get; set; }

        

        public Proveedor Proveedor { get; set; }
        public TipoMaterial TipoMaterial { get; set; }
        public ICollection<AnalisisMaterial> AnalisisMaterial { get; set; }
        public ICollection<MaterialEspecificacion> MaterialEspecificacion { get; set; }
    }
}
