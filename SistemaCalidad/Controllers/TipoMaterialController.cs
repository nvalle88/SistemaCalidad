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
using SistemaCalidad.Models.BussinessViewModels;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Controllers
{
    [Authorize(Policy = "Administracion")]
    public class TipoMaterialController : Controller
    {
        private readonly CALIDADContext db;

        public TipoMaterialController(CALIDADContext context)
        {
            db = context;
        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var lista = await db.TipoMaterial.ToListAsync();
                return View(lista);
            }
            catch (Exception)
            {
                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ErrorListado}";
                return View();
            }

        }

        private async Task<List<Models.Especificacion>> ListaEspecificaciones()
        {
            var listaEspecificaciones = await db.Especificacion
                                               .Select(x => new Especificacion
                                               {
                                                   EspecificacionId = x.EspecificacionId,
                                                   Descripcion = x.Descripcion,
                                               }).ToListAsync();
            return listaEspecificaciones;
        }

        private async Task cargarListaEspecificacionesViewData()
        {
            ViewData["IdEspecificacion"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await ListaEspecificaciones(), "EspecificacionId", "Descripcion");
        }

        private async Task<List<TipoMaterialEspecificacion>> ListaTipoMaterialEspecificacion(int id)
        {
            var listaEspecificaciones = await db.TipoMaterialEspecificacion.Where(x => x.TipoMaterialId == id)
                                               .Select(x => new TipoMaterialEspecificacion
                                               {
                                                   EspecificacionId = x.EspecificacionId,
                                                   Especificacion = new Especificacion { Descripcion = x.Especificacion.Descripcion, },
                                                   TipoMaterialEspecificacionId=x.TipoMaterialEspecificacionId,
                                                   TipoMaterialId=x.TipoMaterialId,
                                               }).ToListAsync();
            return listaEspecificaciones;
        }

        private async Task<TipoMaterialEspecificacionViewModel> ObtenerTipoMaterialEspecificacion(int id, TipoMaterialEspecificacionViewModel tipoMaterialEspecificacionView = null)
        {
            if (tipoMaterialEspecificacionView == null)
            {
                var tipoMaterial = new TipoMaterialEspecificacionViewModel
                {
                    TipoMaterialId = 0,
                    TipoMaterial = await db.TipoMaterial.Where(x => x.TipoMaterialId == id).Select(x => new TipoMaterial
                    {
                       DescripcionTipoMaterial=x.DescripcionTipoMaterial,
                       TipoMaterialId=x.TipoMaterialId,

                    }).FirstOrDefaultAsync(),
                    TipoMaterialEspecificacion = await ListaTipoMaterialEspecificacion(id)
                };
                return tipoMaterial;
            }

            var tipoMaterialEspecificacionResult = new TipoMaterialEspecificacionViewModel
            {

                TipoMaterialId=tipoMaterialEspecificacionView.TipoMaterialId,
                EspecificacionId = tipoMaterialEspecificacionView.EspecificacionId,

                TipoMaterialEspecificacion = await ListaTipoMaterialEspecificacion(id),
                TipoMaterial = await db.TipoMaterial.Where(x => x.TipoMaterialId == id).Select(x => new TipoMaterial
                {
                    DescripcionTipoMaterial = x.DescripcionTipoMaterial,

                }).FirstOrDefaultAsync(),

            };
            return tipoMaterialEspecificacionResult;

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Especificaciones(TipoMaterialEspecificacionViewModel tipoMaterialEspecificacion)
        {

            try
            {
                await cargarListaEspecificacionesViewData();

                var existeRegistro = false;

                if (!await db.TipoMaterialEspecificacion.AnyAsync(c => c.EspecificacionId == tipoMaterialEspecificacion.EspecificacionId && c.TipoMaterialId == tipoMaterialEspecificacion.TipoMaterialId))
                {
                    var p = new TipoMaterialEspecificacion
                    {
                       TipoMaterialId  = tipoMaterialEspecificacion.TipoMaterialId,
                        EspecificacionId = tipoMaterialEspecificacion.EspecificacionId,
                    };
                    await db.AddAsync(p);
                }
                else
                    existeRegistro = true;

                if (!existeRegistro)
                {
                    await db.SaveChangesAsync();
                    TempData["Mensaje"] = $"{Mensaje.Informacion}|{Mensaje.Satisfactorio}";
                    return View(await ObtenerTipoMaterialEspecificacion(tipoMaterialEspecificacion.TipoMaterialId, null));
                }
                else
                    TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ExisteRegistro}";

                return View(await ObtenerTipoMaterialEspecificacion(tipoMaterialEspecificacion.TipoMaterialId, tipoMaterialEspecificacion));
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
            }


        }
        public async Task<IActionResult> DeleteEspecificacion(int id)
        {

            try
            {
                var CurrentTipoMaterial = await db.TipoMaterialEspecificacion.Where(x => x.TipoMaterialEspecificacionId == id).FirstOrDefaultAsync();
                if (CurrentTipoMaterial != null)
                {
                    var result = db.TipoMaterialEspecificacion.Remove(CurrentTipoMaterial);
                    await db.SaveChangesAsync();
                    return this.Redireccionar("TipoMaterial", "Especificaciones", new { id = CurrentTipoMaterial.TipoMaterialId }, $"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}");
                }
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.BorradoNoSatisfactorio}");
            }
        }
        public async Task<IActionResult> Especificaciones(int id)
        {
            await cargarListaEspecificacionesViewData();
            return View(await ObtenerTipoMaterialEspecificacion(id, new TipoMaterialEspecificacionViewModel { TipoMaterialId = id }));
        }

        public async Task<IActionResult> Manage(int id)
        {
            try
            {
                ViewBag.accion = id == 0 ? "Crear" : "Editar";
                if (id != 0)
                {
                    var TipoMaterial = await db.TipoMaterial.FirstOrDefaultAsync(c => c.TipoMaterialId == Convert.ToInt32(id));
                    if (TipoMaterial == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");

                    return View(TipoMaterial);
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
        public async Task<IActionResult> Manage(TipoMaterial TipoMaterial)
        {
            try
            {
                ViewBag.accion = TipoMaterial.TipoMaterialId == 0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    var existeRegistro = false;
                    if (TipoMaterial.TipoMaterialId == 0)
                    {
                        if (!await db.TipoMaterial.AnyAsync(c => c.DescripcionTipoMaterial.ToUpper().Trim() == TipoMaterial.DescripcionTipoMaterial.ToUpper().Trim()))
                        {
                            await db.AddAsync(TipoMaterial);
                        }

                        else
                            existeRegistro = true;
                    }
                    else
                    {
                        if (!await db.TipoMaterial.Where(c => c.DescripcionTipoMaterial.ToUpper().Trim() == TipoMaterial.DescripcionTipoMaterial.ToUpper().Trim()).AnyAsync(c => c.TipoMaterialId != TipoMaterial.TipoMaterialId))
                        {
                            var CurrentTipoMaterial = await db.TipoMaterial.Where(x => x.TipoMaterialId == TipoMaterial.TipoMaterialId).FirstOrDefaultAsync();
                            CurrentTipoMaterial.DescripcionTipoMaterial = TipoMaterial.DescripcionTipoMaterial;
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
                    return View(TipoMaterial);
                }

                return View(TipoMaterial);

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
                var CurrentTipoMaterial = await db.TipoMaterial.Where(x => x.TipoMaterialId == id).FirstOrDefaultAsync();
                if (CurrentTipoMaterial != null)
                {
                    var result = db.TipoMaterial.Remove(CurrentTipoMaterial);
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