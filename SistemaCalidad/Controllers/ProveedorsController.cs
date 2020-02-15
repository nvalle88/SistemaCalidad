using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaCalidad.Data;
using SistemaCalidad.Extensions;
using SistemaCalidad.Models;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Controllers
{
    [Authorize(Policy = "Administracion")]
    public class ProveedorsController : Controller
    {
        private readonly CALIDADContext db;
       
        public ProveedorsController(CALIDADContext context)
        {
            db = context;
           
        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var lista = await db.Proveedor.ToListAsync();
                return View(lista);
            }
            catch (Exception)
            {
                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ErrorListado}";
                return View();
            }

        }

        public async Task<IActionResult> Manage(int id)
        {
            try
            {
                ViewBag.accion = id == 0 ? "Crear" : "Editar";
                if (id != 0)
                {
                    var proveedor = await db.Proveedor.FirstOrDefaultAsync(c => c.ProveedorId ==Convert.ToInt32(id));
                    if (proveedor == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");
                    
                    return View(proveedor);
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
        public async Task<IActionResult> Manage(Proveedor proveedor)
        {
            try
            {
                ViewBag.accion = proveedor.ProveedorId == 0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    var existeRegistro = false;
                    if (proveedor.ProveedorId == 0)
                    {
                        if (!await db.Proveedor.AnyAsync(c => c.CodigoProveedor.ToUpper().Trim() == proveedor.CodigoProveedor.ToUpper().Trim()))
                        {
                            await db.AddAsync(proveedor);
                        }

                        else
                            existeRegistro = true;
                    }
                    else
                    {
                        if (!await db.Proveedor.Where(c => c.CodigoProveedor.ToUpper().Trim() == proveedor.CodigoProveedor.ToUpper().Trim()).AnyAsync(c => c.ProveedorId != proveedor.ProveedorId))
                        {
                            var CurrentProveedor = await db.Proveedor.Where(x=>x.ProveedorId== proveedor.ProveedorId).FirstOrDefaultAsync();
                            CurrentProveedor.NombreProveedor = proveedor.NombreProveedor;
                            CurrentProveedor.CodigoProveedor = proveedor.CodigoProveedor;
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
                    return View(proveedor);
                }
               
                return View(proveedor);

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
                var CurrentProveedor = await db.Proveedor.Where(x=>x.ProveedorId== id).FirstOrDefaultAsync();
                if (CurrentProveedor != null)
                {
                    var result =db.Proveedor.Remove(CurrentProveedor);
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
