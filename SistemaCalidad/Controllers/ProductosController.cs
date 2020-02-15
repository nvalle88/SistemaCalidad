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
    public class ProductosController : Controller
    {
        private readonly CALIDADContext db;

        public ProductosController(CALIDADContext context)
        {
            db = context;

        }


        private async Task<ProductoEspecificacionViewModel> ObtenerProductoEspecificacion(int id, ProductoEspecificacionViewModel productoEspecificacionView = null)
        {
            if (productoEspecificacionView == null)
            {
                var producto = new ProductoEspecificacionViewModel
                {
                    ProductoId = 0,
                    Producto = await db.Producto.Where(x => x.ProductoId == id).Select(x => new Producto
                    {
                        DescripcionProducto = x.DescripcionProducto,
                        ObservacionProducto = x.ObservacionProducto,
                        CodigoProducto=x.CodigoProducto,
                        Nominal = x.Nominal,
                        Grado = x.Grado,
                        ClaseProducto = new ClaseProducto { ClaseDescripcion = x.ClaseProducto.ClaseDescripcion },

                    }).FirstOrDefaultAsync(),
                    ProductoEspecificacion = await ListaProdctoEspecificacion(id)
                };
                return producto;
            }

            var productoEspecificacionResult = new ProductoEspecificacionViewModel
            {
                ProductoId = productoEspecificacionView.ProductoId,
                EspecificacionId = productoEspecificacionView.EspecificacionId,
                RangoMaximo = productoEspecificacionView.RangoMaximo,
                RangoMinimo = productoEspecificacionView.RangoMinimo,
                ValorEsperado = productoEspecificacionView.ValorEsperado,
                ProductoEspecificacion = await ListaProdctoEspecificacion(id),
                Producto = await db.Producto.Where(x => x.ProductoId == id).Select(x => new Producto
                {
                    DescripcionProducto = x.DescripcionProducto,
                    ObservacionProducto = x.ObservacionProducto,
                    Nominal = x.Nominal,
                    Grado = x.Grado,
                    CodigoProducto=x.CodigoProducto,
                    ClaseProducto = new ClaseProducto { ClaseDescripcion = x.ClaseProducto.ClaseDescripcion },

                }).FirstOrDefaultAsync(),

            };
            return productoEspecificacionResult;

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Especificaciones(ProductoEspecificacionViewModel productoEspecificacion)
        {

            try
            {
                await cargarListaEspecificacionesViewData();
               

                var existeRegistro = false;
               
                if (!await db.ProductoEspecificacion.AnyAsync(c => c.EspecificacionId == productoEspecificacion.EspecificacionId && c.ProductoId == productoEspecificacion.ProductoId))
                {
                    var especificacion =await db.Especificacion.Where(x => x.EspecificacionId == productoEspecificacion.EspecificacionId).FirstOrDefaultAsync();

                    var p = new ProductoEspecificacion
                    {
                        ProductoId = productoEspecificacion.ProductoId,
                        EspecificacionId = productoEspecificacion.EspecificacionId,
                        
                    };

                    switch (especificacion.TipoEspecificacion)
                    {
                        case "Rango":
                            if (productoEspecificacion.RangoMinimo == null && productoEspecificacion.RangoMaximo == null)
                            {
                                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.DebeIntroducirAlMenosUnRango}";
                                return View(await ObtenerProductoEspecificacion(productoEspecificacion.ProductoId, productoEspecificacion));
                            }
                            if (productoEspecificacion.RangoMinimo > productoEspecificacion.RangoMaximo)
                            {
                                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.RangoMinimoMayorRangoMaximo}";
                                return View(await ObtenerProductoEspecificacion(productoEspecificacion.ProductoId, productoEspecificacion));
                            }
                            p.RangoMaximo = productoEspecificacion.RangoMaximo;
                            p.RangoMinimo = productoEspecificacion.RangoMinimo;
                            break;
                        case "Texto":
                            p.ValorEsperado = productoEspecificacion.ValorEsperado;
                            break;
                        case "Número":
                            p.ValorEsperadoNum = productoEspecificacion.ValorEsperadoNum;
                            break;
                        case "Bit":
                            p.ValorEsperado = "CUMPLE";
                            break;
                    }
                    await db.AddAsync(p);
                }
                else
                    existeRegistro = true;

                if (!existeRegistro)
                {
                    await db.SaveChangesAsync();
                    TempData["Mensaje"] = $"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}";
                    return View(await ObtenerProductoEspecificacion(productoEspecificacion.ProductoId, null));
                }
                else
                    TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ExisteRegistro}";

                return View(await ObtenerProductoEspecificacion(productoEspecificacion.ProductoId, productoEspecificacion));
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
            }
        }


        public async Task<JsonResult>  TipoMaterial(int id)
        {

            var especificacion =await db.Especificacion.Where(x => x.EspecificacionId == id)
                           .Select(x=>new 
                           Especificacion
                           {
                               EspecificacionId =x.EspecificacionId,TipoEspecificacion=x.TipoEspecificacion
                           }).FirstOrDefaultAsync();

            return Json(especificacion);
        }

        private async Task<List<ProductoEspecificacion>> ListaProdctoEspecificacion(int id)
        {
            var listaEspecificaciones = await db.ProductoEspecificacion.Where(x => x.ProductoId == id)
                                               .Select(x => new ProductoEspecificacion
                                               {
                                                   ProductoEspecificacionId = x.ProductoEspecificacionId,
                                                   Especificacion = new Especificacion { Descripcion = x.Especificacion.Descripcion, },
                                                   RangoMaximo = x.RangoMaximo,
                                                   RangoMinimo = x.RangoMinimo,
                                                   ValorEsperado = x.ValorEsperado,
                                                   ValorEsperadoNum=x.ValorEsperadoNum,
                                               }).ToListAsync();
            return listaEspecificaciones;
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

        public async Task<IActionResult> Especificaciones(int id)
        {
            await cargarListaEspecificacionesViewData();
            return View(await ObtenerProductoEspecificacion(id, new ProductoEspecificacionViewModel { ProductoId = id }));
        }
        public async Task<IActionResult> Index()
        {

            try
            {
               
                return View(new ProductoViewModel {ListaProductos= new List<Producto>(),CodigoPtroducto=string.Empty });
            }
            catch (Exception)
            {
                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ErrorListado}";
                return View();
            }

        }


        [HttpPost]
        public async Task<IActionResult> Index(ProductoViewModel productoViewModel)
        {
            try
            {
                var lista = new List<Producto>();
                if (string.IsNullOrEmpty(productoViewModel.CodigoPtroducto))
                {
                   
                    lista = await db.Producto.Select(x => new Producto
                    {
                        ProductoId = x.ProductoId,
                        CodigoProducto = x.CodigoProducto,
                        DescripcionProducto = x.DescripcionProducto,
                        ObservacionProducto = x.ObservacionProducto,
                        Grado = x.Grado,
                        Nominal = x.Nominal,
                    }).OrderBy(x=>x.ProductoId).ToListAsync();
                    productoViewModel.ListaProductos = lista;
                    return View(productoViewModel);
                }
               
                lista = await db.Producto.Where(x=>x.CodigoProducto.Contains(productoViewModel.CodigoPtroducto)).Select(x => new Producto
                {
                    ProductoId = x.ProductoId,
                    CodigoProducto = x.CodigoProducto,
                    DescripcionProducto = x.DescripcionProducto,
                    ObservacionProducto = x.ObservacionProducto,
                    Grado = x.Grado,
                    Nominal = x.Nominal,
                }).OrderBy(x => x.ProductoId).ToListAsync();
                productoViewModel.ListaProductos = lista;
                return View(productoViewModel);
            }
            catch (Exception)
            {
                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ErrorListado}";
                return View();
            }
        }


        private async Task cargarCombos()
        {
            ViewData["IdClaseProducto"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList
                                                (await db.ClaseProducto.Select(x => new ClaseProducto
                                                { ClaseDescripcion = x.ClaseDescripcion,
                                                  ClaseProductoId = x.ClaseProductoId }).ToListAsync(), 
                                                "ClaseProductoId", "ClaseDescripcion"
                                                );
        }

        public async Task<IActionResult> Manage(int id)
        {
            try
            {
                ViewBag.accion = id == 0 ? "Crear" : "Editar";
                await cargarCombos();
                if (id != 0)
                {
                    var Producto = await db.Producto.FirstOrDefaultAsync(c => c.ProductoId == Convert.ToInt32(id));
                    if (Producto == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");
                    return View(Producto);
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
        public async Task<IActionResult> Manage(Producto Producto)
        {
            try
            {
                ViewBag.accion = Producto.ProductoId == 0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    var existeRegistro = false;
                    if (Producto.ProductoId == 0)
                    {
                        if (!await db.Producto.AnyAsync(c => c.CodigoProducto.ToUpper().Trim() == Producto.CodigoProducto.ToUpper().Trim()))
                        {
                            await db.AddAsync(Producto);
                        }

                        else
                            existeRegistro = true;
                    }
                    else
                    {
                        if (!await db.Producto.Where(c => c.CodigoProducto.ToUpper().Trim() == Producto.CodigoProducto.ToUpper().Trim()).AnyAsync(c => c.ProductoId != Producto.ProductoId))
                        {
                            var CurrentProducto = await db.Producto.Where(x => x.ProductoId == Producto.ProductoId).FirstOrDefaultAsync();
                            CurrentProducto.CodigoProducto = Producto.CodigoProducto;
                            CurrentProducto.DescripcionProducto = Producto.DescripcionProducto;
                            CurrentProducto.ObservacionProducto = Producto.ObservacionProducto;
                            CurrentProducto.Grado = Producto.Grado;
                            CurrentProducto.DimensionMaxima = Producto.DimensionMaxima;
                            CurrentProducto.DimensionMinima = Producto.DimensionMinima;
                            CurrentProducto.Nominal = Producto.Nominal;
                            CurrentProducto.DefUsuario1 = Producto.DefUsuario1;
                            CurrentProducto.DefUsuario2 = Producto.DefUsuario2;
                            CurrentProducto.ClaseProductoId = Producto.ClaseProductoId;
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
                    return View(Producto);
                }

                return View(Producto);

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
                var CurrentProducto = await db.ProductoEspecificacion.Where(x => x.ProductoEspecificacionId == id).FirstOrDefaultAsync();
                if (CurrentProducto != null)
                {
                    var result = db.ProductoEspecificacion.Remove(CurrentProducto);
                    await db.SaveChangesAsync();
                    return this.Redireccionar("Productos", "Especificaciones", new { id = CurrentProducto.ProductoId }, $"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}");
                }
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.BorradoNoSatisfactorio}");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                var CurrentProducto = await db.Producto.Where(x => x.ProductoId == id).FirstOrDefaultAsync();
                if (CurrentProducto != null)
                {
                    var result = db.Producto.Remove(CurrentProducto);
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