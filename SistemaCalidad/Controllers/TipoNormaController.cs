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
    public class TipoNormaController : Controller
    {
        private readonly CALIDADContext db;

        public TipoNormaController(CALIDADContext context)
        {
            db = context;

        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var lista = await db.TipoNorma.ToListAsync();
                return View(lista);
            }
            catch (Exception)
            {
                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ErrorListado}";
                return View();
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Especificaciones(TipoNormaEspecificacionViewModel tipoNormaEspecificacion)
        {

            try
            {
                await cargarListaEspecificacionesViewData();

                if (tipoNormaEspecificacion.ValorMinimo==null && tipoNormaEspecificacion.ValorMaximo==null)
                {
                    TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.DebeIntroducirAlMenosUnRango}";
                    return View(await ObtenerNormaEspecificacion(tipoNormaEspecificacion.TipoNormaId, tipoNormaEspecificacion));
                }

                if (tipoNormaEspecificacion.ValorMinimo > tipoNormaEspecificacion.ValorMaximo)
                {
                    TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ValorMinimoMayorRangoMaximo}";
                    return View(await ObtenerNormaEspecificacion(tipoNormaEspecificacion.TipoNormaId, tipoNormaEspecificacion));
                }

                var existeRegistro = false;

                if (!await db.Norma.AnyAsync(c => c.EspecificacionId == tipoNormaEspecificacion.EspecificacionId && c.TipoNormaId == tipoNormaEspecificacion.TipoNormaId))
                {
                    var p = new Norma
                    {
                        TipoNormaId = tipoNormaEspecificacion.TipoNormaId,
                        EspecificacionId = tipoNormaEspecificacion.EspecificacionId,
                        ValorMaximo=tipoNormaEspecificacion.ValorMaximo,
                        ValorMinimo=tipoNormaEspecificacion.ValorMinimo,
                    };
                    await db.AddAsync(p);
                }
                else
                    existeRegistro = true;

                if (!existeRegistro)
                {
                    await db.SaveChangesAsync();
                    TempData["Mensaje"] = $"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}";
                    return View(await ObtenerNormaEspecificacion(tipoNormaEspecificacion.TipoNormaId, null));
                }
                else
                    TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ExisteRegistro}";

                return View(await ObtenerNormaEspecificacion(tipoNormaEspecificacion.TipoNormaId, tipoNormaEspecificacion));
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
                var CurrentNorma = await db.Norma.Where(x => x.NormaId == id).FirstOrDefaultAsync();
                if (CurrentNorma != null)
                {
                    var result = db.Norma.Remove(CurrentNorma);
                    await db.SaveChangesAsync();
                    return this.Redireccionar("TipoNorma", "Especificaciones", new { id = CurrentNorma.TipoNormaId }, $"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}");
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
            return View(await ObtenerNormaEspecificacion(id, new TipoNormaEspecificacionViewModel { TipoNormaId = id }));
        }

         private async Task<TipoNormaEspecificacionViewModel> ObtenerNormaEspecificacion(int id, TipoNormaEspecificacionViewModel tipoNormaEspecificacionView = null)
        {
            if (tipoNormaEspecificacionView == null)
            {
                var tipoNorma = new TipoNormaEspecificacionViewModel
                {
                    NormaId = 0,
                    TipoNorma = await db.TipoNorma.Where(x => x.TipoNormaId == id).Select(x => new TipoNorma
                    {
                        DescripcionNorma=x.DescripcionNorma,
                        Sae=x.Sae,

                    }).FirstOrDefaultAsync(),
                    Normas = await  ListaNormaEspecificacion(id)
                };
                return tipoNorma;
            }

            var NormaEspecificacionResult = new TipoNormaEspecificacionViewModel
            {
                TipoNormaId=tipoNormaEspecificacionView.TipoNormaId,
                EspecificacionId = tipoNormaEspecificacionView.EspecificacionId,

                Normas = await ListaNormaEspecificacion(id),
                TipoNorma = await db.TipoNorma.Where(x => x.TipoNormaId == id).Select(x => new TipoNorma
                {
                    DescripcionNorma=x.DescripcionNorma,
                    Sae=x.Sae,
                }).FirstOrDefaultAsync(),

            };
            return NormaEspecificacionResult;

        }

        private async Task<List<Norma>> ListaNormaEspecificacion(int id)
        {
            var listaEspecificaciones = await db.Norma.Where(x => x.TipoNormaId == id)
                                               .Select(x => new Norma
                                               {
                                                   EspecificacionId = x.EspecificacionId,
                                                   Especificacion = new Especificacion { Descripcion = x.Especificacion.Descripcion, },
                                                   NormaId = x.NormaId,
                                                   ValorMaximo = x.ValorMaximo,
                                                   ValorMinimo = x.ValorMinimo,
                                                   TipoNormaId = x.TipoNormaId,
                                               }).ToListAsync();
            return listaEspecificaciones;
        }

        private async Task cargarListaEspecificacionesViewData()
        {
            ViewData["IdEspecificacion"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await ListaEspecificaciones(), "EspecificacionId", "Descripcion");
        }

        private async Task<List<Models.Especificacion>> ListaEspecificaciones()
        {
            var listaEspecificaciones = await db.Especificacion.Where(x=>x.TipoEspecificacion==ConstanteEspecificaciones.Rango)
                                               .Select(x => new Especificacion
                                               {
                                                   EspecificacionId = x.EspecificacionId,
                                                   Descripcion = x.Descripcion,
                                               }).ToListAsync();
            return listaEspecificaciones;
        }

        public async Task<IActionResult> Manage(int id)
        {
            try
            {
                ViewBag.accion = id == 0 ? "Crear" : "Editar";
                if (id != 0)
                {
                    var TipoNorma = await db.TipoNorma.FirstOrDefaultAsync(c => c.TipoNormaId == Convert.ToInt32(id));
                    if (TipoNorma == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");

                    return View(TipoNorma);
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
        public async Task<IActionResult> Manage(TipoNorma TipoNorma)
        {
            try
            {
                ViewBag.accion = TipoNorma.TipoNormaId == 0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                   
                    if (TipoNorma.TipoNormaId == 0)
                    {
                        
                            await db.AddAsync(TipoNorma);
                       
                    }
                    else
                    {
                      
                            var CurrentTipoNorma = await db.TipoNorma.Where(x => x.TipoNormaId == TipoNorma.TipoNormaId).FirstOrDefaultAsync();
                            CurrentTipoNorma.DescripcionNorma = TipoNorma.DescripcionNorma;
                            CurrentTipoNorma.Sae = TipoNorma.Sae;
                      
                    }
                   
                        await db.SaveChangesAsync();
                        return this.Redireccionar($"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}");
                   
                }

                return View(TipoNorma);

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
                var CurrentTipoNorma = await db.TipoNorma.Where(x => x.TipoNormaId == id).FirstOrDefaultAsync();
                if (CurrentTipoNorma != null)
                {
                    var result = db.TipoNorma.Remove(CurrentTipoNorma);
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