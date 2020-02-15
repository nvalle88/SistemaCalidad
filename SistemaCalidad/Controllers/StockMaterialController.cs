using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaCalidad.Data;
using SistemaCalidad.Models;
using SistemaCalidad.Models.BussinessViewModels;

namespace SistemaCalidad.Controllers
{
    [Authorize(Policy = "Planificacion")]
    public class StockMaterialController : Controller
    {
        private readonly CALIDADContext db;


        public StockMaterialController(CALIDADContext context)
        {
            db = context;
        }

        public async Task<List<Material>> ListarMateriales()
        {
            var ListaMateriales = await db.Material
                                           .Where(x =>x.Aprobado == true)
                                                       .Select(x => new Material
                                                       {
                                                           MaterialId = x.MaterialId,
                                                           Identificador = x.Identificador,
                                                           CodigoIngreso = x.CodigoIngreso,
                                                       }).OrderBy(x=>x.CodigoIngreso).ToListAsync();
            return ListaMateriales;
            
        }

        private async Task CargarCombox()
        {
            var lista =await ListarMateriales();
            ViewData["IdMaterial"] = new SelectList( lista, "MaterialId", "CodigoIngreso");
        }


        public async Task<StockMaterialViewModel> GetMaterial(int id)
        {
            var material = await db.Material
                                    .Where(x =>
                                            x.MaterialId == id)
                                            .Select(x => new StockMaterialViewModel
                                            {
                                                MaterialId = x.MaterialId,
                                                StockDisponible = x.StockDisponible,
                                                CodigoIngreso = x.CodigoIngreso,
                                                Identificador = x.Identificador,
                                                UnidadMedida = x.UnidadMedida,
                                                SAE=string.Format("{0}",x.TipoNorma.DescripcionNorma),
                                                Proveedor = new Proveedor
                                                {
                                                    ProveedorId = x.Proveedor.ProveedorId,
                                                    NombreProveedor = x.Proveedor.NombreProveedor
                                                },
                                                Pais = new Pais { PaisId = x.Pais.PaisId, DescripcionPais = x.Pais.DescripcionPais },

                                            }).FirstOrDefaultAsync();

            return material;
        }


        [HttpPost]
        public async Task<JsonResult> ActualizarStock(int id,decimal valor, int signo)
        {
            var material=await db.Material.Where(x => x.MaterialId == id).FirstOrDefaultAsync();

            if (signo < 0) { 

            if (material.StockDisponible<Convert.ToDecimal(valor))
            {
                return Json(-1);
            }
            }

            material.StockDisponible = material.StockDisponible + Convert.ToDecimal(valor)*Convert.ToDecimal(signo);
            await db.SaveChangesAsync();
            var materialSalida = await GetMaterial(material.MaterialId);
            return Json(materialSalida);
        }

        public async Task<IActionResult> Index()
        {
            await CargarCombox();
            return View();
        }
    }
}