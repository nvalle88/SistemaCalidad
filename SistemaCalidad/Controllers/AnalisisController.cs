using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaCalidad.Data;
using SistemaCalidad.Extensions;
using SistemaCalidad.Models;
using SistemaCalidad.Models.BussinessViewModels;
using SistemaCalidad.Models.ManageViewModels;
using SistemaCalidad.Services;
using SistemaCalidad.Utils;


namespace SistemaCalidad.Controllers
{
    [Authorize(Policy = "Laboratorio")]
    public class AnalisisController : Controller
    {
        private readonly CALIDADContext db;

        private readonly IEmailSender emailSender;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;


        public AnalisisController(CALIDADContext context, IEmailSender _emailSender, RoleManager<IdentityRole> _roleManager, UserManager<ApplicationUser> _userManager)
        {
            this.userManager = _userManager;
            this.roleManager = _roleManager;
            this.emailSender = _emailSender;
            db = context;
        }
        [Authorize(Policy = "Administracion")]
        [HttpPost]
        public async Task<IActionResult> Aprobar(AprobarAnalisisViewModel analisis)
        {


            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var analisisActualizar = await db.Analisis.Where(x => x.AnalisisId == analisis.AnalisisId).FirstOrDefaultAsync();
                    if (analisisActualizar == null)
                    {
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");

                    }

                    analisisActualizar.ObservacionesAprobado = analisis.ObservacionesAprobado;
                    db.Analisis.Update(analisisActualizar);


                    var totalDetalleAnalisis = await db.DetalleAnalisis.Where(x => x.AnalisisId == analisis.AnalisisId && x.Aprobado == false).ToListAsync();


                    analisisActualizar.Resultado = totalDetalleAnalisis.Count == analisis.AnalisisAprobados?.Count ? "CUMPLE" : "NO CUMPLE";



                    var listaUpdate = new List<DetalleAnalisis>();
                    listaUpdate.AddRange(totalDetalleAnalisis);
                    foreach (var item in listaUpdate.Where(x => x.AprobadoSupervisor != -1))
                    {
                        item.AprobadoSupervisor = 0;

                    }

                    db.UpdateRange(listaUpdate);
                    await db.SaveChangesAsync();

                    if (analisis.AnalisisAprobados != null)
                    {






                        foreach (var item in analisis.AnalisisAprobados)
                        {

                            var detalleActualizar = await db.DetalleAnalisis.Where(x => x.DetalleAnalisisId == Convert.ToInt32(item)).FirstOrDefaultAsync();

                            detalleActualizar.AprobadoSupervisor = 1;
                            db.DetalleAnalisis.Update(detalleActualizar);
                            await db.SaveChangesAsync();
                        }
                    }



                    transaction.Commit();
                    return this.Redireccionar($"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}");

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
                }
            }
        }
        [Authorize(Policy = "Administracion")]
        public async Task<IActionResult> Aprobar(int id)
        {
            try
            {
                var Analisis = await db.Analisis
                                   .Select(x => new Analisis
                                   {

                                       AnalisisId = x.AnalisisId,
                                       FechaAnalisis = x.FechaAnalisis.Date,
                                       Maquina = new Maquina { MaquinaId = x.MaquinaId, NombreMaquina = x.Maquina.NombreMaquina },
                                       Cliente = new Cliente { ClienteId = x.ClienteId, CodigoCliente = x.Cliente.CodigoCliente, NombreCliente = x.Cliente.NombreCliente },
                                       NumeroOrden = x.NumeroOrden,
                                       Observaciones = x.Observaciones,
                                       Resultado = x.Resultado,
                                       Rollo = x.Rollo,
                                       Temperatura = x.Temperatura,
                                       Turno = x.Turno,
                                       NombreUsuario = x.NombreUsuario,
                                       ObservacionesAprobado = x.ObservacionesAprobado,
                                       Producto = new Producto
                                       {
                                           ClaseProducto = new ClaseProducto
                                           {
                                               ClaseProductoId = x.Producto.ClaseProducto.ClaseProductoId,
                                               ClaseDescripcion = x.Producto.ClaseProducto.ClaseDescripcion
                                           },

                                           CodigoProducto = x.Producto.CodigoProducto,
                                           DefUsuario1 = x.Producto.DefUsuario1,
                                           DefUsuario2 = x.Producto.DefUsuario2,
                                           DescripcionProducto = x.Producto.DescripcionProducto,

                                           DimensionMaxima = x.Producto.DimensionMaxima.Value,
                                           DimensionMinima = x.Producto.DimensionMinima.Value,
                                           Grado = x.Producto.Grado,
                                           Nominal = x.Producto.Nominal,
                                           ProductoId = x.ProductoId,
                                           ObservacionProducto = x.Producto.ObservacionProducto,
                                       },
                                       DetalleAnalisis = x.DetalleAnalisis.Select(y => new DetalleAnalisis
                                       {

                                           AnalisisId = x.AnalisisId,
                                           Aprobado = y.Aprobado,
                                           Resultado = y.Resultado,
                                           AprobadoSupervisor = y.AprobadoSupervisor,
                                           RangoReferenciaActual = y.RangoReferenciaActual,
                                           Especificacion = new Especificacion
                                           {
                                               Analisis = y.Especificacion.Analisis,
                                               Descripcion = y.Especificacion.Descripcion,
                                               TipoEspecificacion = y.Especificacion.TipoEspecificacion,
                                               ClaseEspecificacion = y.Especificacion.ClaseEspecificacion,
                                               EspecificacionId = y.Especificacion.EspecificacionId,
                                           },
                                           DetalleAnalisisId = y.DetalleAnalisisId,
                                       }
                                                                                    ).ToList(),
                                       ClienteId = x.ClienteId,
                                       MaquinaId = x.MaquinaId,
                                       ProductoId = x.ProductoId,
                                       AnalisisMaterial = x.AnalisisMaterial.Select(j => new AnalisisMaterial
                                       {
                                           AnalisisId = x.AnalisisId,
                                           MateriaId = j.MateriaId,
                                           AnalisisMateriaId = j.AnalisisMateriaId,
                                           Materia = new Material
                                           {
                                               CodigoIngreso = j.Materia.CodigoIngreso,
                                               Identificador = j.Materia.Identificador,
                                               MaterialId = j.Materia.MaterialId,
                                               UnidadMedida = j.Materia.UnidadMedida,
                                               StockDisponible = j.Materia.StockDisponible,
                                               Proveedor = new Proveedor
                                               {
                                                   ProveedorId = j.Materia.ProveedorId,
                                                   CodigoProveedor = j.Materia.Proveedor.CodigoProveedor,
                                                   NombreProveedor = j.Materia.Proveedor.NombreProveedor,
                                               },
                                               TipoMaterial = new TipoMaterial
                                               {
                                                   DescripcionTipoMaterial = j.Materia.TipoMaterial.DescripcionTipoMaterial,
                                                   TipoMaterialId = j.Materia.TipoMaterial.TipoMaterialId,
                                               },
                                               TipoNorma = new TipoNorma { Sae = j.Materia.TipoNorma.Sae },
                                           },

                                       }).ToList(),
                                       MaterialEspecificacion = db.MaterialEspecificacion
                                                                    .Where(h => h.EspecificacionId == Constantes.IdEspecificacionDiamtroMaterial && h.MaterialId == x.AnalisisMaterial.Where(g => g.Materia.TipoMaterialId == Constantes.IdAlambron).FirstOrDefault().Materia.MaterialId).FirstOrDefault(),


                                   }
                                   ).Where(x => x.AnalisisId == id).FirstOrDefaultAsync();

                var salida = new AprobarAnalisisViewModel { ObservacionesAprobado = Analisis.ObservacionesAprobado, Analisis = Analisis, AnalisisAprobados = new List<string>(), AnalisisId = Analisis.AnalisisId };

                return View(salida);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Authorize(Policy = "Administracion")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ListaEspecificaciones = await db.DetalleAnalisis.Where(x => x.AnalisisId == id).ToListAsync();
                db.DetalleAnalisis.RemoveRange(ListaEspecificaciones);
                await db.SaveChangesAsync();
                var ListaAnalisisMaterial = await db.AnalisisMaterial.Where(x => x.AnalisisId == id).ToListAsync();
                db.AnalisisMaterial.RemoveRange(ListaAnalisisMaterial);
                await db.SaveChangesAsync();
                var CurrentAnalisis = await db.Analisis.Where(x => x.AnalisisId == id).FirstOrDefaultAsync();
                if (CurrentAnalisis != null)
                {
                    var result = db.Analisis.Remove(CurrentAnalisis);
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

        // GET: Analisis
        public async Task<IActionResult> Index()
        {
            try
            {
                var ListaAnalisis = new List<Analisis>();
                var fechaInicio = DateTime.Now.AddMonths(-Constantes.MesesListadoAnalisis).Date;
                var fechaFin = DateTime.Now.Date;
                ListaAnalisis = await db.Analisis.Where(x => x.FechaAnalisis >= fechaInicio && x.FechaAnalisis <= fechaFin).Select(x => new Analisis
                {
                    AnalisisId=x.AnalisisId,
                    Producto = new Producto
                    {
                        CodigoProducto = x.Producto.CodigoProducto,
                        DescripcionProducto = x.Producto.DescripcionProducto,
                    },
                    NombreUsuario = x.NombreUsuario,
                    FechaAnalisis = x.FechaAnalisis,
                    NumeroOrden = x.NumeroOrden,
                    Turno=x.Turno,
                    AnalisisMaterial = x.AnalisisMaterial
                                        .Select(y=> new AnalisisMaterial
                                                        {
                                                           Materia =new Material
                                                            {
                                                                MaterialEspecificacion=y.Materia.MaterialEspecificacion,
                                                                TipoMaterialId =y.Materia.TipoMaterialId,
                                                                MaterialId=y.MateriaId,
                                                                Identificador =y.Materia.Identificador,
                                                                CodigoIngreso=y.Materia.CodigoIngreso,
                                                                TipoNorma= new TipoNorma { Sae =y.Materia.TipoNorma.Sae },
                                                                TipoMaterial= new TipoMaterial { DescripcionTipoMaterial=y.Materia.TipoMaterial.DescripcionTipoMaterial}
                                                            }
                                                          
                                                        }
                                        ).ToList(),
                    Resultado=x.Resultado,

                    //MaterialEspecificacion = db.MaterialEspecificacion
                    //.Where(h => h.EspecificacionId == Constantes.IdEspecificacionDiamtroMaterial && h.MaterialId == x.AnalisisMaterial.Where(g => g.Materia.TipoMaterialId == Constantes.IdAlambron).FirstOrDefault().Materia.MaterialId).FirstOrDefault(),


                }
                ).OrderByDescending(x=>x.AnalisisId).ToListAsync();
                var analisisView = new AnalisisViewModel { FechaFin = DateTime.Now.Date, FechaInicio = DateTime.Now.AddMonths(-Constantes.MesesListadoAnalisis).Date, ListaAnalisis = ListaAnalisis };
                return View(analisisView);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        public async Task<IActionResult> Index(AnalisisViewModel viewModel)
        {
            var ListaAnalisis = new List<Analisis>();
            ListaAnalisis = await db.Analisis.Where(x => x.FechaAnalisis >= viewModel.FechaInicio && x.FechaAnalisis <= viewModel.FechaFin).Select(x => new Analisis
            {
                AnalisisId = x.AnalisisId,
                Producto = new Producto
                {
                    CodigoProducto = x.Producto.CodigoProducto,
                    DescripcionProducto = x.Producto.DescripcionProducto,
                },
                NombreUsuario = x.NombreUsuario,
                FechaAnalisis = x.FechaAnalisis,
                NumeroOrden = x.NumeroOrden,
                Turno = x.Turno,
                AnalisisMaterial = x.AnalisisMaterial
                                       .Select(y => new AnalisisMaterial
                                       {
                                           Materia = new Material
                                           {
                                               MaterialEspecificacion = y.Materia.MaterialEspecificacion,
                                               TipoMaterialId = y.Materia.TipoMaterialId,
                                               MaterialId = y.MateriaId,
                                               Identificador = y.Materia.Identificador,
                                               CodigoIngreso = y.Materia.CodigoIngreso,
                                               TipoNorma = new TipoNorma { Sae = y.Materia.TipoNorma.Sae },
                                               TipoMaterial = new TipoMaterial { DescripcionTipoMaterial = y.Materia.TipoMaterial.DescripcionTipoMaterial }
                                           }

                                       }
                                       ).ToList(),
                Resultado = x.Resultado,

            }
            ).OrderByDescending(x => x.AnalisisId).ToListAsync();
            var analisisView = new AnalisisViewModel { FechaFin = DateTime.Now.Date, FechaInicio = DateTime.Now.AddMonths(-Constantes.MesesListadoAnalisis).Date, ListaAnalisis = ListaAnalisis };
            return View(analisisView);
        }


        [HttpPost]
        public async Task<JsonResult> CantidadAnalisisRealizados(string numeroOrden)
        {
            var total = await db.Analisis.Where(x => x.NumeroOrden == numeroOrden).CountAsync();
            var cumple = await db.Analisis.Where(x => x.NumeroOrden == numeroOrden && x.Resultado == "CUMPLE").CountAsync();
            var nocumple = await db.Analisis.Where(x => x.NumeroOrden == numeroOrden && x.Resultado == "NO CUMPLE").CountAsync();
            var analisis = new AnalisisRealizados { Cumplidos = cumple, InCumplidos = nocumple, Total = total };
            return Json(analisis);
        }



        [HttpPost]
        public async Task<JsonResult> EspecificacionesGeneralesMaterial(int idmaterial)
        {

            var listaespecificaciones = await db.MaterialEspecificacion.Where(x => x.MaterialId == idmaterial && x.Especificacion.Analisis == false)
                                                                              .Select(y => new EspecificacionesGeneralesMaterial
                                                                              {
                                                                                  CodigoIngreso = y.Material.CodigoIngreso,
                                                                                  Identificador = y.Material.Identificador,
                                                                                  NombreProveedor = y.Material.Proveedor.NombreProveedor,
                                                                                  TipoMaterial = y.Material.TipoMaterial.DescripcionTipoMaterial,
                                                                                  StockDisponible = y.Material.StockDisponible,
                                                                                  UnidadMedida = y.Material.UnidadMedida,
                                                                                  DescripcionEspecificacion = y.Especificacion.Descripcion,
                                                                                  ValorEspecificacion = y.ValorEspecificacion,

                                                                              }).ToListAsync();
            return Json(listaespecificaciones);
        }
        [HttpPost]
        public async Task<JsonResult> ListaEspecificaciones(int ProductoId, List<Models.ManageViewModels.MaterialViewModel> listamateriales)
        {

            var listaEspecificaciones = await db.ProductoEspecificacion.Where(x => x.ProductoId == ProductoId)
                .Select(x => new EspecificacionViewModel
                {
                    Clase = x.Especificacion.ClaseEspecificacion,
                    Descripcion = x.Especificacion.Descripcion,
                    EspecificacionId = x.EspecificacionId,
                    Tipo = x.Especificacion.TipoEspecificacion,
                    ValorEsperadoProducto = x.ValorEsperado,
                    RangoMaximoProducto = x.RangoMaximo,
                    RangoMinimoProducto = x.RangoMinimo,
                    ValorMaterial = "0",
                    Analisis = x.Especificacion.Analisis,
                    ValorEsperadoNumProducto = x.ValorEsperadoNum,
                    DescripcionTipoMaterial = string.Empty,



                }).ToListAsync();

            var listaNoBuscar = new List<MaterialEspecificacion>();

            foreach (var item in listamateriales)
            {
                foreach (var producto in listaEspecificaciones)
                {
                    if (!listaNoBuscar.Any(x => x.EspecificacionId == producto.EspecificacionId))
                    {

                        producto.Editable = true;
                        var especificacion = producto.EspecificacionId;
                        var m = await db.MaterialEspecificacion.Where(x => x.EspecificacionId == producto.EspecificacionId
                                                                                     && x.MaterialId == item.idmaterial).Select(x => new MaterialEspecificacion
                                                                                     {
                                                                                         EspecificacionId=x.EspecificacionId,
                                                                                         MaterialId = x.MaterialId,
                                                                                         ValorEspecificacion = x.ValorEspecificacion,
                                                                                         Material = new Material
                                                                                         {
                                                                                             MaterialId = x.MaterialId,
                                                                                             TipoMaterial = new TipoMaterial
                                                                                             {
                                                                                                 DescripcionTipoMaterial = x.Material.TipoMaterial.DescripcionTipoMaterial,
                                                                                             }
                                                                                         }
                                                                                     }
                                                                        )
                                                                                    .FirstOrDefaultAsync();

                        if (m != null)
                        {
                            producto.MaterialId = m.MaterialId;
                            producto.ValorMaterial = m.ValorEspecificacion;
                            producto.Editable = false;
                            producto.DescripcionTipoMaterial = m.Material.TipoMaterial.DescripcionTipoMaterial;
                            listaNoBuscar.Add(m);
                        }
                    }

                }
            }

            var listasalida = listaEspecificaciones.OrderByDescending(x => x.Editable).ThenBy(x => x.DescripcionTipoMaterial).ToList();


            return Json(listasalida);
        }

        [HttpPost]
        public async Task<JsonResult> GetDatosProducto(int ProductoId)
        {
            var Producto = await db.Producto.Where(x => x.ProductoId == ProductoId).Select(x => new Producto
            {
                Grado = x.Grado,
                DescripcionProducto = x.DescripcionProducto,
                ObservacionProducto = x.ObservacionProducto,
                ClaseProducto = new ClaseProducto { ClaseDescripcion = x.ClaseProducto.ClaseDescripcion },
                Nominal = x.Nominal,
            }).FirstOrDefaultAsync();
            return Json(Producto);
        }


        public async Task<JsonResult> ListarMaterialPorTipo(int TipoMaterialId)
        {
            var ListaMateriales = await db.Material
                                          .Where(x => x.TipoMaterialId == TipoMaterialId
                                                      && x.Aprobado == true
                                                      && x.StockDisponible > Convert.ToDecimal(0.00))
                                                      .Select(x => new Material
                                                      {
                                                          MaterialId = x.MaterialId,
                                                          Identificador = x.Identificador,
                                                          CodigoIngreso = x.CodigoIngreso,
                                                          TipoNorma = new TipoNorma { Sae = x.TipoNorma.Sae },
                                                      }

                                                      ).ToListAsync();
            return Json(ListaMateriales);
        }

        // GET: Analisis/Details/5
        public async Task<IActionResult> Detalle(int id)
        {


            try
            {


                var Analisis = await db.Analisis
                                   .Select(x => new Analisis
                                   {
                                       AnalisisId = x.AnalisisId,
                                       FechaAnalisis = x.FechaAnalisis.Date,
                                       Maquina = new Maquina { MaquinaId = x.MaquinaId, NombreMaquina = x.Maquina.NombreMaquina },
                                       Cliente = new Cliente { ClienteId = x.ClienteId, CodigoCliente = x.Cliente.CodigoCliente, NombreCliente = x.Cliente.NombreCliente },
                                       NumeroOrden = x.NumeroOrden,
                                       Observaciones = x.Observaciones,
                                       Resultado = x.Resultado,
                                       Rollo = x.Rollo,
                                       Temperatura = x.Temperatura,
                                       Turno = x.Turno,
                                       NombreUsuario = x.NombreUsuario,
                                       Producto = new Producto
                                       {
                                           ClaseProducto = new ClaseProducto
                                           {
                                               ClaseProductoId = x.Producto.ClaseProducto.ClaseProductoId,
                                               ClaseDescripcion = x.Producto.ClaseProducto.ClaseDescripcion
                                           },

                                           CodigoProducto = x.Producto.CodigoProducto,
                                           DefUsuario1 = x.Producto.DefUsuario1,
                                           DefUsuario2 = x.Producto.DefUsuario2,
                                           DescripcionProducto = x.Producto.DescripcionProducto,

                                           DimensionMaxima = x.Producto.DimensionMaxima.Value,
                                           DimensionMinima = x.Producto.DimensionMinima.Value,
                                           Grado = x.Producto.Grado,
                                           Nominal = x.Producto.Nominal,
                                           ProductoId = x.ProductoId,
                                           ObservacionProducto = x.Producto.ObservacionProducto,
                                       },
                                       DetalleAnalisis = x.DetalleAnalisis.Select(y => new DetalleAnalisis
                                       {
                                           AnalisisId = x.AnalisisId,
                                           Aprobado = y.Aprobado,
                                           Resultado = y.Resultado,
                                           RangoReferenciaActual = y.RangoReferenciaActual,
                                           Especificacion = new Especificacion
                                           {
                                               Analisis = y.Especificacion.Analisis,
                                               Descripcion = y.Especificacion.Descripcion,
                                               TipoEspecificacion = y.Especificacion.TipoEspecificacion,
                                               ClaseEspecificacion = y.Especificacion.ClaseEspecificacion,
                                               EspecificacionId = y.Especificacion.EspecificacionId,
                                           },
                                           DetalleAnalisisId = y.DetalleAnalisisId,
                                       }
                                                                                    ).ToList(),
                                       ClienteId = x.ClienteId,
                                       MaquinaId = x.MaquinaId,
                                       ProductoId = x.ProductoId,
                                       AnalisisMaterial = x.AnalisisMaterial.Select(j => new AnalisisMaterial
                                       {
                                           AnalisisId = x.AnalisisId,
                                           MateriaId = j.MateriaId,
                                           AnalisisMateriaId = j.AnalisisMateriaId,
                                           Materia = new Material
                                           {
                                               CodigoIngreso = j.Materia.CodigoIngreso,
                                               Identificador = j.Materia.Identificador,
                                               MaterialId = j.Materia.MaterialId,
                                               UnidadMedida = j.Materia.UnidadMedida,
                                               StockDisponible = j.Materia.StockDisponible,
                                               Proveedor = new Proveedor
                                               {
                                                   ProveedorId = j.Materia.ProveedorId,
                                                   CodigoProveedor = j.Materia.Proveedor.CodigoProveedor,
                                                   NombreProveedor = j.Materia.Proveedor.NombreProveedor,
                                               },
                                               TipoMaterial = new TipoMaterial
                                               {
                                                   DescripcionTipoMaterial = j.Materia.TipoMaterial.DescripcionTipoMaterial,
                                                   TipoMaterialId = j.Materia.TipoMaterial.TipoMaterialId,
                                               },
                                               TipoNorma= new TipoNorma {Sae= j.Materia.TipoNorma.Sae },
                                           },
                                       }).ToList(),

                                       MaterialEspecificacion = db.MaterialEspecificacion
                                                                    .Where(h => h.EspecificacionId == Constantes.IdEspecificacionDiamtroMaterial && h.MaterialId == x.AnalisisMaterial.Where(g => g.Materia.TipoMaterialId == Constantes.IdAlambron).FirstOrDefault().Materia.MaterialId).FirstOrDefault(),


                                   }
                                   ).Where(x => x.AnalisisId == id).FirstOrDefaultAsync();

                return View(Analisis);
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void CargarCombox()
        {
            ViewData["ClienteId"] = new SelectList(db.Cliente.Select(x => new Cliente { ClienteId = x.ClienteId, NombreCliente = x.NombreCliente }), "ClienteId", "NombreCliente");
            ViewData["MaquinaId"] = new SelectList(db.Maquina.Select(x => new Maquina { MaquinaId = x.MaquinaId, NombreMaquina = x.NombreMaquina }), "MaquinaId", "NombreMaquina");
            ViewData["ProductoId"] = new SelectList(db.Producto.Select(x => new Producto { ProductoId = x.ProductoId, CodigoProducto = x.CodigoProducto }), "ProductoId", "CodigoProducto");
            ViewData["TipoMaterialId"] = new SelectList(db.TipoMaterial.Select(x => new TipoMaterial { TipoMaterialId = x.TipoMaterialId, DescripcionTipoMaterial = x.DescripcionTipoMaterial }), "TipoMaterialId", "DescripcionTipoMaterial");

        }

        // GET: Analisis/Create
        public async Task<IActionResult> Create()
        {
            CargarCombox();
            var ultimoRegistro = await db.Analisis.OrderByDescending(x => x.AnalisisId).FirstOrDefaultAsync();
            if (ultimoRegistro != null)
            {
                return View(new Analisis { FechaAnalisis = DateTime.Now.Date, Turno = ultimoRegistro.Turno, MaquinaId = ultimoRegistro.MaquinaId, ClienteId = ultimoRegistro.ClienteId, Temperatura = ultimoRegistro.Temperatura, Rollo = ultimoRegistro.Rollo, NumeroOrden = ultimoRegistro.NumeroOrden });
            }
            return View(new Analisis { FechaAnalisis = DateTime.Now.Date });

        }

        // POST: Analisis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<JsonResult> Create(List<DetalleAnalisis> listaDetalle, Analisis analisis, List<Material> listaMateriales)
        {

            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    var resultado = listaDetalle.Where(x => (x.Aprobado == false)).FirstOrDefault();

                    if (resultado != null)
                    {
                        analisis.Resultado = "NO CUMPLE";
                    }
                    else analisis.Resultado = "CUMPLE";

                    if (string.IsNullOrEmpty(analisis.Observaciones))
                    {
                        analisis.Observaciones = string.Empty;
                    }

                    analisis.NombreUsuario = User.Identity.Name;

                    var analisisCreate = await db.AddAsync(analisis);
                    await db.SaveChangesAsync();

                    var idAnalisisCreado = analisisCreate.Entity.AnalisisId;



                    var listaDetalleInsertar = listaDetalle.Select(x => new DetalleAnalisis { AnalisisId = idAnalisisCreado, Aprobado = x.Aprobado, EspecificacionId = x.EspecificacionId, RangoReferenciaActual = x.RangoReferenciaActual, Resultado = x.Resultado, }).ToList();
                    await db.AddRangeAsync(listaDetalleInsertar);

                    var listaAnalisisMaterialInsertar = listaMateriales.Select(x => new AnalisisMaterial { AnalisisId = idAnalisisCreado, MateriaId = x.MaterialId });
                    await db.AddRangeAsync(listaAnalisisMaterialInsertar);


                    await db.SaveChangesAsync();

                    if (analisis.Resultado == "NO CUMPLE")
                    {
                        var listadoEmails = new List<string>();
                        var listaAdministradores = await userManager.GetUsersInRoleAsync(Perfiles.Administracion);

                        foreach (var item in listaAdministradores)
                        {
                            listadoEmails.Add(item.Email);
                        }

                        var cuerpo =await emailSender.CuerpoAnalisisNoValido(analisisCreate.Entity);
                        emailSender.SendEmailAsync(listadoEmails, Mensaje.AsuntoCorreo, cuerpo);
                    }

                    transaction.Commit();

                    return Json(1);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return Json(-1);

                }
            }
        }

        [Authorize(Policy = "Administracion")]
        // GET: Analisis/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var analisis = await db.Analisis.SingleOrDefaultAsync(m => m.AnalisisId == id);
            if (analisis == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(db.Cliente, "ClienteId", "CodigoCliente", analisis.ClienteId);
            ViewData["MaquinaId"] = new SelectList(db.Maquina, "MaquinaId", "NombreMaquina", analisis.MaquinaId);
            ViewData["ProductoId"] = new SelectList(db.Producto, "ProductoId", "CodigoProducto", analisis.ProductoId);
            return View(analisis);
        }
    }
}
