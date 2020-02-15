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
    public class MaquinasController : Controller
    {
        private readonly CALIDADContext db;
        
        public MaquinasController(CALIDADContext context)
        {
             db = context;
        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var lista = await db.Maquina.ToListAsync();
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
                ViewBag.accion = id==0 ? "Crear" : "Editar";
                if (id != 0)
                {
                    var maquina = await db.Maquina.FirstOrDefaultAsync(c => c.MaquinaId == Convert.ToInt32(id));
                    if (maquina == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");

                    return View(maquina);
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
        public async Task<IActionResult> Manage(Maquina maquina)
        {
            try
            {
                ViewBag.accion = maquina.MaquinaId==0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    var existeRegistro = false;
                    if (maquina.MaquinaId == 0)
                    {
                        if (!await db.Maquina.AnyAsync(c => c.NombreMaquina.ToUpper().Trim() == maquina.NombreMaquina.ToUpper().Trim()))
                        {
                            await db.AddAsync(maquina);
                        }

                        else
                            existeRegistro = true;
                    }
                    else
                    {
                        if (!await db.Maquina.Where(c => c.NombreMaquina.ToUpper().Trim() == maquina.NombreMaquina.ToUpper().Trim()).AnyAsync(c => c.MaquinaId != maquina.MaquinaId))
                        {
                            var CurrentMaquina = await db.Maquina.Where(x=>x.MaquinaId==maquina.MaquinaId).FirstOrDefaultAsync();
                            CurrentMaquina.NombreMaquina = maquina.NombreMaquina;
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
                    return View(maquina);
                }

                return View(maquina);

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
                var CurrentMaquina = await db.Maquina.Where(x=>x.MaquinaId==id).FirstOrDefaultAsync() ;
                if (CurrentMaquina != null)
                {
                    var result = db.Maquina.Remove(CurrentMaquina);
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
