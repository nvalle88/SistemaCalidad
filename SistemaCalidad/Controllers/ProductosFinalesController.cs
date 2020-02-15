using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCalidad.Data;
using SistemaCalidad.Extensions;
using SistemaCalidad.Models;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Controllers
{
    [Authorize(Policy = "Administracion")]
    public class ProductosFinalesController : Controller
    {
        private readonly CALIDADContext db;

        public ProductosFinalesController(CALIDADContext context)
        {
            db = context;

        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var lista = await db.ProductoFinal.Include(x=>x.Producto).OrderBy(x=>x.Codigo).ToListAsync();
                return View(lista);
            }
            catch (Exception)
            {
                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ErrorListado}";
                return View();
            }

        }


        private async Task cargarCombos()
        {
            ViewData["IdProducto"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList
                                           (await db.Producto.Select(x => new Producto
                                           {
                                             ProductoId=x.ProductoId,
                                             DescripcionProducto =string.Format("{0} | {1}",x.CodigoProducto,x.DescripcionProducto),
                                           }).ToListAsync(),
                                             "ProductoId", "DescripcionProducto"
                                             );
           
        }

        public async Task<IActionResult> Manage(int id)
        {
            try
            {
                await cargarCombos();
                ViewBag.accion = id == 0 ? "Crear" : "Editar";
                if (id != 0)
                {
                    var ProductoFinal = await db.ProductoFinal.FirstOrDefaultAsync(c => c.ProductoFinalId == Convert.ToInt32(id));
                    if (ProductoFinal == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");

                    return View(ProductoFinal);
                }
                return View();
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.ErrorCargarDatos}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(ProductoFinal ProductoFinal)
        {
            try
            {
                ViewBag.accion = ProductoFinal.ProductoFinalId == 0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    await cargarCombos();
                    var existeRegistro = false;
                    if (ProductoFinal.ProductoFinalId == 0)
                    {
                        if (!await db.ProductoFinal.AnyAsync(c => c.Codigo.ToUpper().Trim() == ProductoFinal.Codigo.ToUpper().Trim() && c.ProductoId==ProductoFinal.ProductoId ))
                        {
                            await db.AddAsync(ProductoFinal);
                        }

                        else
                            existeRegistro = true;
                    }
                    else
                    {
                        if (!await db.ProductoFinal.Where(c => c.Codigo.ToUpper().Trim() == ProductoFinal.Codigo.ToUpper().Trim() && c.ProductoId ==ProductoFinal.ProductoId).AnyAsync(c => c.ProductoFinalId != ProductoFinal.ProductoFinalId))
                        {
                            var CurrentProductoFinal = await db.ProductoFinal.Where(x => x.ProductoFinalId == ProductoFinal.ProductoFinalId).FirstOrDefaultAsync();
                            CurrentProductoFinal.Codigo = ProductoFinal.Codigo;
                            CurrentProductoFinal.Descripcion = ProductoFinal.Descripcion;
                        }
                        else
                            existeRegistro = true;
                    }
                    if (!existeRegistro)
                    {
                        await db.SaveChangesAsync();
                        return this.Redireccionar($"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}");
                    }
                    else
                        TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ExisteRegistro}";
                    return View(ProductoFinal);
                }

                return View(ProductoFinal);

            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
            }
        }



        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                var CurrentProductoFinal = await db.ProductoFinal.Where(x => x.ProductoFinalId == id).FirstOrDefaultAsync();
                if (CurrentProductoFinal != null)
                {
                    var result = db.ProductoFinal.Remove(CurrentProductoFinal);
                    await db.SaveChangesAsync();
                    return this.Redireccionar($"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}");
                }
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.BorradoNoSatisfactorio}");
            }
        }
    }
}