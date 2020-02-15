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
    public class MaterialesController : Controller
    {

        private readonly CALIDADContext db;

        public MaterialesController(CALIDADContext context)
        {
            db = context;

        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ListaEspecificaciones =await db.MaterialEspecificacion.Where(x => x.MaterialId == id).ToListAsync();
                db.MaterialEspecificacion.RemoveRange(ListaEspecificaciones);
                await db.SaveChangesAsync();
                var CurrentMaterial = await db.Material.Where(x => x.MaterialId == id).FirstOrDefaultAsync();
                if (CurrentMaterial != null)
                {
                    var result = db.Material.Remove(CurrentMaterial);
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

        
        public async Task<IActionResult> AprobarDesaprobarMaterial(int id)
        {


            var materialActual =await db.Material.Where(x => x.MaterialId == id).FirstOrDefaultAsync();

            if (materialActual==null)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");
            }

            if (materialActual.Aprobado==true)
            {
                materialActual.Aprobado = false;
            }
            else
            {
                materialActual.Aprobado = true;
            }

            await db.SaveChangesAsync();
            return this.Redireccionar($"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}");
        }


        [HttpPost]
        public async Task<JsonResult> ListadoEspecificacionesPorMaterialEditar(int idMaterial,int idTipoMaterial)
        {


            var lista = await db.MaterialEspecificacion
                                .Where(x => x.MaterialId == idMaterial)
                                .Select(x => new MaterialEspecificacion
                                {
                                    EspecificacionId=x.EspecificacionId,
                                    ValorEspecificacion=x.ValorEspecificacion,
                                }).ToListAsync();

            var listaSalida = await db.TipoMaterialEspecificacion
                     .Where(x => x.TipoMaterialId == idTipoMaterial)
                     .Select(y => new Especificacion { Descripcion = y.Especificacion.Descripcion, EspecificacionId = y.Especificacion.EspecificacionId, TipoEspecificacion = y.Especificacion.TipoEspecificacion,ValorEspecificacion=lista.Where(r=>r.EspecificacionId==y.Especificacion.EspecificacionId).FirstOrDefault().ValorEspecificacion }).ToListAsync();

            return Json(listaSalida);

        }


        [HttpPost]
        public async Task<JsonResult> ListadoEspecificacionesPorMaterial(int id)
        {

            var a = await db.TipoMaterialEspecificacion
                      .Where(x=>x.TipoMaterialId==id)
                      .Select(y =>new Especificacion { Descripcion = y.Especificacion.Descripcion, EspecificacionId = y.Especificacion.EspecificacionId,TipoEspecificacion=y.Especificacion.TipoEspecificacion } ).ToListAsync();
            return Json(a);

        }

        private async Task cargarCombos()
        {
            ViewData["IdProveedor"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList
                                           (await db.Proveedor.Select(x => new Proveedor
                                           { NombreProveedor = x.NombreProveedor,
                                             ProveedorId = x.ProveedorId }).ToListAsync(),
                                             "ProveedorId", "NombreProveedor"
                                             );

            ViewData["IdTipoMaterial"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList
                                                (await db.TipoMaterial.Select(x => new TipoMaterial
                                                {
                                                    DescripcionTipoMaterial = x.DescripcionTipoMaterial,
                                                    TipoMaterialId = x.TipoMaterialId
                                                }).OrderBy(x=>x.DescripcionTipoMaterial).ToListAsync(),
                                                  "TipoMaterialId", "DescripcionTipoMaterial"
                                                );

            ViewData["IdPais"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList
                                               (await db.Pais.Select(x => new Pais
                                               {
                                                   DescripcionPais = x.DescripcionPais,
                                                   PaisId = x.PaisId
                                               }).ToListAsync(),
                                                 "PaisId", "DescripcionPais"
                                               );

            ViewData["IdSAE"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList
                                              (await db.TipoNorma.Select(x => new TipoNorma
                                              {
                                                 TipoNormaId=x.TipoNormaId,
                                                 Sae=x.Sae,
                                                 DescripcionNorma=x.DescripcionNorma,
                                              }).Distinct().ToListAsync(),
                                                "TipoNormaId", "DescripcionNorma"
                                              );
        }

        private async Task<List<Models.Especificacion>> ListaEspecificaciones()
        {
            var listaEspecificaciones = await db.Especificacion
                                               .Select(x => new Especificacion
                                               {
                                                   EspecificacionId = x.EspecificacionId,
                                                   Descripcion = x.Descripcion,
                                                   TipoEspecificacion=x.TipoEspecificacion,
                                               }).ToListAsync();
            return listaEspecificaciones;
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Especificaciones(MaterialEspecificacionViewModel materialEspecificacion)
        {

            try
            {
                await cargarListaEspecificacionesViewData();

                var especificacionId = materialEspecificacion.EspecificacionId;
                var Material =await db.Material.Where(x=>x.MaterialId==materialEspecificacion.MaterialId).FirstOrDefaultAsync();

               // var materialEspecificacionA = db.Material.Where(x => x.MaterialId == materialEspecificacion.MaterialId).FirstOrDefaultAsync();

                var Norma =await db.Norma.Where(c=>c.EspecificacionId==especificacionId
                                            && c.TipoNorma.TipoNormaId == Material.TipoNormaId)
                                            .FirstOrDefaultAsync();

                var cumpleNorma = true;

                if (Norma.ValorMinimo>=Convert.ToDecimal(materialEspecificacion.ValorEspecificacion) || Norma.ValorMinimo<=Convert.ToDecimal(materialEspecificacion.ValorEspecificacion))
                {

                    cumpleNorma = false;
                }


                var existeRegistro = false;

                if (!await db.MaterialEspecificacion.AnyAsync(c => c.EspecificacionId == materialEspecificacion.EspecificacionId && c.MaterialId == materialEspecificacion.MaterialId))
                {
                    var p = new MaterialEspecificacion
                    {
                        MaterialId = materialEspecificacion.MaterialId,
                        EspecificacionId = materialEspecificacion.EspecificacionId,
                        ValorEspecificacion = materialEspecificacion.ValorEspecificacion,
                    };
                    await db.AddAsync(p);
                }
                else
                    existeRegistro = true;

                if (!existeRegistro)
                {
                    await db.SaveChangesAsync();

                    if (cumpleNorma)
                    {
                        TempData["Mensaje"] = $"{Mensaje.Informacion}|{Mensaje.Satisfactorio}";
                        return View(await ObtenerMaterialeEspecificacion(materialEspecificacion.MaterialId, null));
                    }

                    TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.NoCumpleNorma}";
                    return View(await ObtenerMaterialeEspecificacion(materialEspecificacion.MaterialId, null));
                }
                else
                    TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ExisteRegistro}";

                return View(await ObtenerMaterialeEspecificacion(materialEspecificacion.MaterialId, materialEspecificacion));
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
                var CurrentMaterial = await db.MaterialEspecificacion.Where(x => x.MaterialEspecificacionId == id).FirstOrDefaultAsync();
                if (CurrentMaterial != null)
                {
                    var result = db.MaterialEspecificacion.Remove(CurrentMaterial);
                    await db.SaveChangesAsync();
                    return this.Redireccionar("Materiales", "Especificaciones", new { id = CurrentMaterial.MaterialId }, $"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}");
                }
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.BorradoNoSatisfactorio}");
            }
        }

        private async Task cargarListaEspecificacionesViewData()
        {
            ViewData["IdEspecificacion"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await ListaEspecificaciones(), "EspecificacionId", "Descripcion");
        }

        public async Task<IActionResult> Especificaciones(int id)
        {
            await cargarListaEspecificacionesViewData();
            return View(await ObtenerMaterialeEspecificacion(id, new MaterialEspecificacionViewModel { MaterialId = id }));
        }




        private async Task<List<MaterialEspecificacion>> ListaMaterialEspecificacion(int id)
        {
            var listaEspecificaciones = await db.MaterialEspecificacion.Where(x => x.MaterialId == id)
                                               .Select(x => new MaterialEspecificacion
                                               {
                                                   MaterialEspecificacionId = x.MaterialEspecificacionId,
                                                   Especificacion = new Especificacion { Descripcion = x.Especificacion.Descripcion, },
                                                  ValorEspecificacion=x.ValorEspecificacion,
                                               }).ToListAsync();
            return listaEspecificaciones;
        }


       
        private async Task<MaterialEspecificacionViewModel> ObtenerMaterialeEspecificacion(int id, MaterialEspecificacionViewModel materialEspecificacionView = null)
        {
            if (materialEspecificacionView == null)
            {
                var producto = new MaterialEspecificacionViewModel
                {
                    MaterialId = 0,
                    Material = await db.Material.Where(x => x.MaterialId == id).Select(x => new Material
                    {
                        
                        TipoMaterial=new TipoMaterial {TipoMaterialId=x.TipoMaterial.TipoMaterialId,DescripcionTipoMaterial=x.TipoMaterial.DescripcionTipoMaterial },
                        Pais=new Pais {DescripcionPais=x.Pais.DescripcionPais },
                        Proveedor=new Proveedor {CodigoProveedor=x.Proveedor.CodigoProveedor,NombreProveedor=x.Proveedor.NombreProveedor },
                        StockDisponible=x.StockDisponible,
                        UnidadMedida=x.UnidadMedida,
                        CodigoIngreso=x.CodigoIngreso,
                        Identificador=x.Identificador,
                        TipoNorma= new TipoNorma {Sae=x.TipoNorma.Sae,TipoNormaId=x.TipoNorma.TipoNormaId,DescripcionNorma=x.TipoNorma.DescripcionNorma }
                    }).FirstOrDefaultAsync(),
                    MaterialEspecificacion = await ListaMaterialEspecificacion(id)
                };
                return producto;
            }

            var productoEspecificacionResult = new MaterialEspecificacionViewModel
            {
                MaterialId = materialEspecificacionView.MaterialId,
                EspecificacionId = materialEspecificacionView.EspecificacionId,
                ValorEspecificacion = materialEspecificacionView.ValorEspecificacion,
                MaterialEspecificacion = await ListaMaterialEspecificacion(id),
                Material = await db.Material.Where(x => x.MaterialId == id).Select(x => new Material
                {
                    TipoMaterial = new TipoMaterial { TipoMaterialId = x.TipoMaterial.TipoMaterialId, DescripcionTipoMaterial = x.TipoMaterial.DescripcionTipoMaterial },
                    Pais = new Pais { DescripcionPais = x.Pais.DescripcionPais },
                    Proveedor = new Proveedor { CodigoProveedor = x.Proveedor.CodigoProveedor, NombreProveedor = x.Proveedor.NombreProveedor },
                    StockDisponible = x.StockDisponible,
                    UnidadMedida = x.UnidadMedida,
                    CodigoIngreso = x.CodigoIngreso,
                    Identificador = x.Identificador,
                    TipoNorma = new TipoNorma { Sae = x.TipoNorma.Sae, TipoNormaId = x.TipoNorma.TipoNormaId, DescripcionNorma = x.TipoNorma.DescripcionNorma }
                }).FirstOrDefaultAsync(),

            };
            return productoEspecificacionResult;

        }




        



        public async Task<JsonResult> Create(List<MaterialEspecificacion> listaDetalle, Material material,bool aprobado)
        {
            try
            {
                //var existe = await db.Material.AnyAsync(x => x.CodigoIngreso == material.CodigoIngreso);
                //if (existe)
                //{
                //    return Json(-1);
                //}

                material.Aprobado = aprobado;
                await db.Material.AddAsync(material);
                await db.SaveChangesAsync();

                var listaNew = listaDetalle.Select(x => new MaterialEspecificacion { EspecificacionId = x.EspecificacionId, MaterialId = material.MaterialId, ValorEspecificacion = x.ValorEspecificacion }).ToList();

                await db.MaterialEspecificacion.AddRangeAsync(listaNew);
                await db.SaveChangesAsync();
                return Json(1);
            }
            catch (Exception)
            {
                return Json(0);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Editar(List<MaterialEspecificacion> listaDetalle, Material material, bool aprobado)
        {
            try
            {
                var existe = await db.Material.AnyAsync(x => x.CodigoIngreso == material.CodigoIngreso && x.MaterialId != material.MaterialId);
                if (existe)
                {
                    return Json(-1);
                }

                var materialUpdate = await db.Material.Where(x => x.MaterialId == material.MaterialId).FirstOrDefaultAsync();

                materialUpdate.PaisId = material.PaisId;
                materialUpdate.ProveedorId = material.ProveedorId;
                materialUpdate.TipoNormaId = material.TipoNormaId;
                materialUpdate.StockDisponible = material.StockDisponible;
                materialUpdate.UnidadMedida = material.UnidadMedida;
                materialUpdate.Identificador = material.Identificador;
                materialUpdate.CodigoIngreso = material.CodigoIngreso;
                material.Aprobado = aprobado;
                await db.SaveChangesAsync();

                var listaDelete =await db.MaterialEspecificacion.Where(x => x.MaterialId == material.MaterialId).ToListAsync();
                db.RemoveRange(listaDelete);
                await db.SaveChangesAsync();

                var listaNew = listaDetalle.Select(x => new MaterialEspecificacion { EspecificacionId = x.EspecificacionId, MaterialId = material.MaterialId, ValorEspecificacion = x.ValorEspecificacion }).ToList();

                await db.MaterialEspecificacion.AddRangeAsync(listaNew);
                await db.SaveChangesAsync();
                return Json(1);
            }
            catch (Exception)
            {
                return Json(0);
            }
        }



        public async Task<IActionResult> Editar(int id)
        {
            ViewBag.accion ="Editar";
            await cargarCombos();
                var MaterialEditar = await db.Material.Where(x => x.MaterialId == id)
                                       .Select(x => new Material
                                       {
                                           CodigoIngreso = x.CodigoIngreso,
                                           PaisId = x.PaisId,
                                           TipoNorma = new TipoNorma { Sae = x.TipoNorma.Sae, TipoNormaId = x.TipoNorma.TipoNormaId, DescripcionNorma = x.TipoNorma.DescripcionNorma },
                                           Identificador = x.Identificador,
                                           ProveedorId = x.ProveedorId,
                                           StockDisponible = x.StockDisponible,
                                           TipoNormaId=x.TipoNormaId,
                                           TipoMaterial = new TipoMaterial
                                           {
                                               TipoMaterialId = x.TipoMaterialId,
                                               TipoMaterialEspecificacion = x.TipoMaterial.TipoMaterialEspecificacion.Select(y => new TipoMaterialEspecificacion { Especificacion = new Especificacion { Descripcion = y.Especificacion.Descripcion, EspecificacionId = y.Especificacion.EspecificacionId } }).ToList(),
                                           },
                                           UnidadMedida = x.UnidadMedida,
                                           MaterialId = x.MaterialId,
                                           TipoMaterialId = x.TipoMaterialId,

                                       }).FirstOrDefaultAsync();
                if (MaterialEditar == null)
                    return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");
                return View(MaterialEditar);
     

        }

        public async Task<IActionResult> Manage(int id)
        {
            try
            {
                ViewBag.accion = id == 0 ? "Crear" : "Editar";
                await cargarCombos();
                

                var Material = new Material { MaterialId = 0,TipoNormaId=0};

                return View(Material);
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.ErrorCargarDatos}");
            }
        }

        public async Task<JsonResult> ValidarNormaKey(int sae,int idespecificacion,string valor)
        {
            try
            {
               var valorreal= Convert.ToDecimal(valor);
               var rango =await db.Norma.Where(x => x.EspecificacionId == idespecificacion && x.TipoNorma.TipoNormaId == sae).FirstOrDefaultAsync();

                if (rango.ValorMinimo > valorreal || rango.ValorMaximo < valorreal)
                {
                    return Json(0);
                }
                else
                {
                    return Json(1);
                };

            }
            catch (Exception)
            {
                return Json(-1);
                throw;
            }
        }

        public async Task<JsonResult> ValidarNorma(int sae ,List<MaterialEspecificacion> listaDetalle)
        {
            try
            {
                var lis = new List<int>();
                var listaFiltrada= listaDetalle.Where(x => !string.IsNullOrEmpty(x.ValorEspecificacion)).ToList();
                var ListaEspecificaciones = await db.Norma.Where(x=> x.TipoNorma.TipoNormaId == sae).ToListAsync();
                foreach (var item in listaDetalle)
                {
                    try
                    {
                        var valorreal = Convert.ToDecimal(item.ValorEspecificacion);
                        var rango = ListaEspecificaciones.Where(x => x.EspecificacionId == item.EspecificacionId).FirstOrDefault();
                        if (rango!=null) {

                            if (rango.ValorMinimo == null)
                            {
                                if (!(rango.ValorMaximo >= Convert.ToDecimal(item.ValorEspecificacion)))
                                {
                                    lis.Add(rango.EspecificacionId.Value);
                                }
                                continue;
                            }

                            if (rango.ValorMaximo == null)
                            {
                                if (!(rango.ValorMinimo <= Convert.ToDecimal(item.ValorEspecificacion)))
                                {
                                    lis.Add(rango.EspecificacionId.Value);
                                }
                                continue;
                            }

                            if (!(rango.ValorMaximo >= Convert.ToDecimal(item.ValorEspecificacion)) || !(rango.ValorMinimo <= Convert.ToDecimal(item.ValorEspecificacion)))
                            {
                                lis.Add(rango.EspecificacionId.Value);
                                continue;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                       
                    }
                }

                return Json(lis);

            }
            catch (Exception)
            {
                return Json(-1);
                throw;
            }

        }

        private  IQueryable<Material> Materiales()
        {
            var lista =  db.Material.Select(x => new Material
            {
                MaterialId = x.MaterialId,
                Identificador = x.Identificador,
                CodigoIngreso = x.CodigoIngreso,
                StockDisponible = x.StockDisponible,
               
                UnidadMedida = x.UnidadMedida,
                CantidadEspecificaciones = x.MaterialEspecificacion.Count(),
                CantidadAnalisis = x.AnalisisMaterial.Count(),
                Proveedor = new Proveedor { NombreProveedor = x.Proveedor.NombreProveedor },
                Pais = new Pais { DescripcionPais = x.Pais.DescripcionPais }

            });
            return lista;
        }

        public async Task<JsonResult> ListarINAPorTipoMaterial(int idTipoMAterial)
        {
            var listaINA = await db.Material.Where(x => x.TipoMaterialId == idTipoMAterial).Select(x => new Material { Identificador = x.Identificador }).OrderBy(x => x.Identificador).Distinct().ToListAsync();
            return Json(listaINA);
        }

        [HttpPost]
        public async Task<IActionResult> Index(MaterialViewModel materialViewModel)
        {
            try
            {
                var lista = new List<Material>();

                if (string.IsNullOrEmpty(materialViewModel.CodigoIngeso) && string.IsNullOrEmpty(materialViewModel.Identificador))
                {

                    lista = await db.Material.Select(x => new Material
                    {
                        MaterialId = x.MaterialId,
                        Identificador = x.Identificador,
                        CodigoIngreso = x.CodigoIngreso,
                        StockDisponible = x.StockDisponible,
                        UnidadMedida = x.UnidadMedida,
                        Proveedor = new Proveedor { NombreProveedor = x.Proveedor.NombreProveedor },
                        TipoNorma = new TipoNorma { Sae = x.TipoNorma.Sae, TipoNormaId = x.TipoNorma.TipoNormaId, DescripcionNorma = x.TipoNorma.DescripcionNorma },
                        Pais = new Pais { DescripcionPais = x.Pais.DescripcionPais },
                        MaterialEspecificacion = x.MaterialEspecificacion,
                        Aprobado = x.Aprobado,
                    }).ToListAsync();
                    materialViewModel.ListaMaretial = lista;


                    return View(materialViewModel);
                }

                if (string.IsNullOrEmpty(materialViewModel.CodigoIngeso) && !string.IsNullOrEmpty(materialViewModel.Identificador))
                {

                    lista = await db.Material.Where(x => x.Identificador.Contains(materialViewModel.Identificador)).Select(x => new Material
                    {
                        MaterialId = x.MaterialId,
                        Identificador = x.Identificador,
                        CodigoIngreso = x.CodigoIngreso,
                        StockDisponible = x.StockDisponible,
                        UnidadMedida = x.UnidadMedida,
                        Proveedor = new Proveedor { NombreProveedor = x.Proveedor.NombreProveedor },
                        TipoNorma = new TipoNorma { Sae = x.TipoNorma.Sae, TipoNormaId = x.TipoNorma.TipoNormaId, DescripcionNorma = x.TipoNorma.DescripcionNorma },
                        Pais = new Pais { DescripcionPais = x.Pais.DescripcionPais },
                        Aprobado = x.Aprobado,
                        MaterialEspecificacion = x.MaterialEspecificacion,
                    }).ToListAsync();
                    materialViewModel.ListaMaretial = lista;


                    return View(materialViewModel);
                }

                if (!string.IsNullOrEmpty(materialViewModel.CodigoIngeso) && !string.IsNullOrEmpty(materialViewModel.Identificador))
                {

                    lista = await db.Material.Where(x => x.CodigoIngreso.Contains(materialViewModel.CodigoIngeso) && x.Identificador.Contains(materialViewModel.Identificador)).Select(x => new Material
                    {
                        MaterialId = x.MaterialId,
                        Identificador = x.Identificador,
                        CodigoIngreso = x.CodigoIngreso,
                        StockDisponible = x.StockDisponible,
                        UnidadMedida = x.UnidadMedida,
                        Proveedor = new Proveedor { NombreProveedor = x.Proveedor.NombreProveedor },
                        TipoNorma = new TipoNorma { Sae = x.TipoNorma.Sae, TipoNormaId = x.TipoNorma.TipoNormaId, DescripcionNorma = x.TipoNorma.DescripcionNorma },
                        Pais = new Pais { DescripcionPais = x.Pais.DescripcionPais },
                        Aprobado = x.Aprobado,
                        MaterialEspecificacion = x.MaterialEspecificacion,
                    }).ToListAsync();
                    materialViewModel.ListaMaretial = lista;


                    return View(materialViewModel);
                }

                if (!string.IsNullOrEmpty(materialViewModel.CodigoIngeso) && string.IsNullOrEmpty(materialViewModel.Identificador))
                {

                    lista = await db.Material.Where(x => x.CodigoIngreso.Contains(materialViewModel.CodigoIngeso)).Select(x => new Material
                    {
                        MaterialId = x.MaterialId,
                        Identificador = x.Identificador,
                        CodigoIngreso = x.CodigoIngreso,
                        StockDisponible = x.StockDisponible,
                        TipoMaterial=new TipoMaterial {TipoMaterialId=x.TipoMaterialId },
                        UnidadMedida = x.UnidadMedida,
                        Proveedor = new Proveedor { NombreProveedor = x.Proveedor.NombreProveedor },
                        TipoNorma = new TipoNorma { Sae = x.TipoNorma.Sae, TipoNormaId = x.TipoNorma.TipoNormaId, DescripcionNorma = x.TipoNorma.DescripcionNorma },
                        Pais = new Pais { DescripcionPais = x.Pais == null ? " " : x.Pais.DescripcionPais },
                        MaterialEspecificacion = x.MaterialEspecificacion,
                        Aprobado = x.Aprobado,
                    }).ToListAsync();
                    
                    materialViewModel.ListaMaretial = lista;
                    return View(materialViewModel);
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ErrorListado}";
                return View();
            }
        }


        public async Task<IActionResult> Index()
        {
            
            return View(new MaterialViewModel {ListaMaretial= new List<Material>(),CodigoIngeso=string.Empty,Identificador= string.Empty });
        }
    }
}