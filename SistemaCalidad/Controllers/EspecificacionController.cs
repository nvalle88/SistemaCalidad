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
    public class EspecificacionController : Controller
    {
        private readonly CALIDADContext db;

        public EspecificacionController(CALIDADContext context)
        {
            db = context;

        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var lista = await db.Especificacion.OrderBy(x=>x.ClaseEspecificacion).ThenBy(x=>x.Descripcion).ToListAsync();
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
                   
                    var Especificacion = await db.Especificacion.FirstOrDefaultAsync(c => c.EspecificacionId == Convert.ToInt32(id));


                    if (Especificacion == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");


                    if (await db.ProductoEspecificacion.AnyAsync(x => x.EspecificacionId == id) || await db.Norma.AnyAsync(x => x.EspecificacionId == id))
                    {
                        Especificacion.NoEditable = true;
                    }
                    return View(Especificacion);
                }
                return base.View(new Models.Especificacion { Analisis=false,NoEditable=false});
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.ErrorCargarDatos}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(Models.Especificacion Especificacion)
        {
            try
            {
                ViewBag.accion = Especificacion.EspecificacionId == 0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    var existeRegistro = false;
                    if (Especificacion.EspecificacionId == 0)
                    {
                        if (!await db.Especificacion.AnyAsync(c => c.Descripcion.ToUpper().Trim() == Especificacion.Descripcion.ToUpper().Trim()))
                        {
                            await db.AddAsync(Especificacion);
                        }

                        else
                            existeRegistro = true;
                    }
                    else
                    {
                        if (!await db.Especificacion.Where(c => c.Descripcion.ToUpper().Trim() == Especificacion.Descripcion.ToUpper().Trim()).AnyAsync(c => c.EspecificacionId != Especificacion.EspecificacionId))
                        {
                            var CurrentEspecificacion = await db.Especificacion.Where(x => x.EspecificacionId == Especificacion.EspecificacionId).FirstOrDefaultAsync();
                            CurrentEspecificacion.Descripcion = Especificacion.Descripcion;
                            CurrentEspecificacion.TipoEspecificacion = Especificacion.TipoEspecificacion;
                            CurrentEspecificacion.Analisis = Especificacion.Analisis;
                            CurrentEspecificacion.ClaseEspecificacion = Especificacion.ClaseEspecificacion;
                            CurrentEspecificacion.DescripcionIngles = Especificacion.DescripcionIngles;
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
                    return base.View(Especificacion);
                }

                return base.View(Especificacion);

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
                var CurrentEspecificacion = await db.Especificacion.Where(x => x.EspecificacionId == id).FirstOrDefaultAsync();
                if (CurrentEspecificacion != null)
                {
                    var result = db.Especificacion.Remove(CurrentEspecificacion);
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