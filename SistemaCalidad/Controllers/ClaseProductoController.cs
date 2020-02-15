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
    public class ClaseProductoController : Controller
    {
        private readonly CALIDADContext db;

        public ClaseProductoController(CALIDADContext context)
        {
            db = context;

        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var lista = await db.ClaseProducto.ToListAsync();
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
                    var ClaseProducto = await db.ClaseProducto.FirstOrDefaultAsync(c => c.ClaseProductoId == Convert.ToInt32(id));
                    if (ClaseProducto == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");

                    return View(ClaseProducto);
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
        public async Task<IActionResult> Manage(ClaseProducto ClaseProducto)
        {
            try
            {
                ViewBag.accion = ClaseProducto.ClaseProductoId == 0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    var existeRegistro = false;
                    if (ClaseProducto.ClaseProductoId == 0)
                    {
                        if (!await db.ClaseProducto.AnyAsync(c => c.ClaseDescripcion.ToUpper().Trim() == ClaseProducto.ClaseDescripcion.ToUpper().Trim()))
                        {
                            await db.AddAsync(ClaseProducto);
                        }

                        else
                            existeRegistro = true;
                    }
                    else
                    {
                        if (!await db.ClaseProducto.Where(c => c.ClaseDescripcion.ToUpper().Trim() == ClaseProducto.ClaseDescripcion.ToUpper().Trim()).AnyAsync(c => c.ClaseProductoId != ClaseProducto.ClaseProductoId))
                        {
                            var CurrentClaseProducto = await db.ClaseProducto.Where(x => x.ClaseProductoId == ClaseProducto.ClaseProductoId).FirstOrDefaultAsync();
                            CurrentClaseProducto.ClaseDescripcion = ClaseProducto.ClaseDescripcion;
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
                    return View(ClaseProducto);
                }

                return View(ClaseProducto);

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
                var CurrentClaseProducto = await db.ClaseProducto.Where(x => x.ClaseProductoId == id).FirstOrDefaultAsync();
                if (CurrentClaseProducto != null)
                {
                    var result = db.ClaseProducto.Remove(CurrentClaseProducto);
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