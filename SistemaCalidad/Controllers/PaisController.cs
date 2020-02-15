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
    public class PaisController : Controller
    {
        private readonly CALIDADContext db;

        public PaisController(CALIDADContext context)
        {
            db = context;

        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var lista = await db.Pais.ToListAsync();
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
                    var Pais = await db.Pais.FirstOrDefaultAsync(c => c.PaisId == Convert.ToInt32(id));
                    if (Pais == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");

                    return View(Pais);
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
        public async Task<IActionResult> Manage(Pais Pais)
        {
            try
            {
                ViewBag.accion = Pais.PaisId == 0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    var existeRegistro = false;
                    if (Pais.PaisId == 0)
                    {
                        if (!await db.Pais.AnyAsync(c => c.DescripcionPais.ToUpper().Trim() == Pais.DescripcionPais.ToUpper().Trim()))
                        {
                            await db.AddAsync(Pais);
                        }

                        else
                            existeRegistro = true;
                    }
                    else
                    {
                        if (!await db.Pais.Where(c => c.DescripcionPais.ToUpper().Trim() == Pais.DescripcionPais.ToUpper().Trim()).AnyAsync(c => c.PaisId != Pais.PaisId))
                        {
                            var CurrentPais = await db.Pais.Where(x => x.PaisId == Pais.PaisId).FirstOrDefaultAsync();
                            CurrentPais.DescripcionPais = Pais.DescripcionPais;
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
                    return View(Pais);
                }

                return View(Pais);

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
                var CurrentPais = await db.Pais.Where(x => x.PaisId == id).FirstOrDefaultAsync();
                if (CurrentPais != null)
                {
                    var result = db.Pais.Remove(CurrentPais);
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