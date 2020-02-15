using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SistemaCalidad.Data;
using SistemaCalidad.Extensions;
using SistemaCalidad.Models;
using SistemaCalidad.Models.BussinessViewModels;
using SistemaCalidad.Models.Utiles;
using SistemaCalidad.Services;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Controllers
{
    [Authorize(Policy = "Gestion")]
    public class CertificadoController : Controller
    {
        private readonly CALIDADContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailSender emailSender;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUploadFileService uploadFileService;

        public CertificadoController(CALIDADContext context, IEmailSender _emailSender, IHostingEnvironment hostingEnvironment, IUploadFileService uploadFileService ,RoleManager<IdentityRole> _roleManager, UserManager<ApplicationUser> _userManager)
        {
            this.userManager = _userManager;
            this.roleManager = _roleManager;
            this.emailSender = _emailSender;
            db = context;
            _hostingEnvironment = hostingEnvironment;
            this.uploadFileService = uploadFileService;
        }
        public async Task<IActionResult> Index(int? idTipo)
        {
            try
            {
                var tipo = 1;
                tipo = idTipo == null || idTipo == 0 ? tipo = 1 : idTipo.Value;
                var certificacos = await db.Certificado.OrderByDescending(x => x.FechaGeneracion).Where(x=>x.Tipo==tipo)
                                       .Select(x => new Certificado
                                       {
                                           CertificadoId = x.CertificadoId,
                                           FechaGeneracion = x.FechaGeneracion,

                                           ProductoFinal =x.ProductoFinal !=null? new ProductoFinal
                                           {
                                               ProductoFinalId = x.ProductoFinal.ProductoFinalId,
                                               Codigo = x.ProductoFinal.Codigo,
                                               Descripcion = x.ProductoFinal.Descripcion
                                           } :new ProductoFinal
                                           {
                                               ProductoFinalId =0,
                                               Codigo = "N/A",
                                               Descripcion = "N/A",
                                           },
                                           OrdenFinal=x.OrdenFinal,
                                           CodigoCertificado = x.CodigoCertificado,
                                           FileUrl = x.FileUrl,
                                           NumeroOrden = x.NumeroOrden,
                                           NumeroGuia = x.NumeroGuia,
                                           ProductoFinalId = x.ProductoFinalId,
                                           Liberado=x.Liberado ==null || x.Liberado==false ? false :true,
                                           ArchivoCargado = x.ArchivoCargado,
                                           Tipo = x.Tipo,
                                         
            }).OrderByDescending(x=>x.CertificadoId).ToListAsync();

                var viewModel = new CertificadoViewModel { Tipo = tipo, ListaCertificados = certificacos };
                return View(viewModel);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(int idTipo)
        {
            try
            {
                var certificacos = await db.Certificado.OrderByDescending(x => x.FechaGeneracion).Where(x => x.Tipo == idTipo)
                                       .Select(x => new Certificado
                                       {
                                           CertificadoId = x.CertificadoId,
                                           FechaGeneracion = x.FechaGeneracion,

                                           ProductoFinal = x.ProductoFinal != null ? new ProductoFinal
                                           {
                                               ProductoFinalId = x.ProductoFinal.ProductoFinalId,
                                               Codigo = x.ProductoFinal.Codigo,
                                               Descripcion = x.ProductoFinal.Descripcion
                                           } : new ProductoFinal
                                           {
                                               ProductoFinalId = 0,
                                               Codigo = "N/A",
                                               Descripcion = "N/A",
                                           },
                                           CodigoCertificado = x.CodigoCertificado,
                                           FileUrl = x.FileUrl,
                                           NumeroOrden = x.NumeroOrden,
                                           NumeroGuia = x.NumeroGuia,
                                           ProductoFinalId = x.ProductoFinalId,
                                           Liberado = x.Liberado == null || x.Liberado == false ? false : true,
                                           ArchivoCargado = x.ArchivoCargado,
                                           Tipo = x.Tipo,
                                       }).OrderByDescending(x=>x.CertificadoId).ToListAsync();

                var viewModel = new CertificadoViewModel { Tipo = idTipo, ListaCertificados = certificacos };
                return View(viewModel);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> ValidarNumeroOrden(string[] ListaDetalle,DateTime fecha,int certificadoId)
        { 
            if (ListaDetalle.Length > 0)
            {
                var fechaActual = fecha.AddDays(Constantes.DiasEvaluarOrdenCertificado * -1);
                var listaFechas = new List<DateTime>();
                foreach (var item in ListaDetalle)
                {
                    var registro =await db.Analisis.Where(x => x.NumeroOrden == item).OrderBy(x => x.FechaAnalisis).FirstOrDefaultAsync();
                    listaFechas.Add(registro.FechaAnalisis.Date);
                }

                var FechaMasAntigua = listaFechas.OrderBy(x => x.Date).FirstOrDefault(); ;
                var certificadoUpdate = await db.Certificado.Where(x => x.CertificadoId == certificadoId).FirstOrDefaultAsync();
                if (fechaActual>FechaMasAntigua)
                {
                    if (!certificadoUpdate.Liberado.Value)
                    {
                        certificadoUpdate.Liberado = false;
                        await db.SaveChangesAsync();
                    }
                   
                    
                    return false;
                }

                certificadoUpdate.Liberado = true;
                await db.SaveChangesAsync();
                return true;
                
            }
            
            return false ;
        }


        private string[] CombertirOrdenesALista(string ordenes)
        {
            var lista = ordenes.Split(";", StringSplitOptions.RemoveEmptyEntries);
            var ListaOrdenes = new string[lista.Length];
            ListaOrdenes = lista;
            return ListaOrdenes;
        }

        public async Task<IActionResult> Create(int? id)
        {

            try
            {
                ViewData["IdProductoFinal"] = new SelectList(await db.ProductoFinal.Select(x => new ProductoFinal { ProductoFinalId = x.ProductoFinalId, Codigo = string.Format("{0} | {1}",x.Codigo,x.Descripcion)}).ToListAsync(), "ProductoFinalId", "Codigo");
                ViewData["IdCliente"] = new SelectList(await db.Cliente.Select(x => new Cliente { ClienteId = x.ClienteId, NombreCliente = x.NombreCliente }).ToListAsync(), "NombreCliente", "NombreCliente");
                ViewData["OrdenesId"] = new SelectList(await CargarComboOrdenes(), "NumeroOrden", "NumeroOrden");

                if (id == null || id == 0)
                {
                    return View(new Certificado {Liberado=false, CertificadoId = 0, Estado = id == null || id == 0 ? 0 : id.Value, FechaGeneracion = DateTime.Now.Date,VerMateriaPrima=true });
                }

                var certificado = await db.Certificado.Where(x => x.CertificadoId == id).FirstOrDefaultAsync();
                var ordenes = certificado.NumeroOrden.Split(";",StringSplitOptions.RemoveEmptyEntries);
                certificado.ListaOrdenes = new string[ordenes.Length];
                certificado.ListaOrdenes = ordenes;
                certificado.Estado = 1;
                return View(certificado);
            }
            catch (Exception ex)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");
            }
        }



        public async Task<IActionResult> DescargarArchivo(int id)
        {
            try
            {
                var documentoRequisitoTransfer = await uploadFileService.GetFileDocumentoRequisito(id);
                return File(documentoRequisitoTransfer.Fichero, MimeTypes.GetMimeType(documentoRequisitoTransfer.Nombre), documentoRequisitoTransfer.Nombre);
            }
            catch (Exception)
            {
                return StatusCode(400, "El archivo solicitado no está disponible, por favor comuníquese con el administrador para obtener  más información.");

            }
        }

        [Authorize(Policy = "Administracion")]
        public async Task<IActionResult> Liberar(int id)
        {

            try
            {
                var CurrentCertificado = await db.Certificado.Where(x => x.CertificadoId == id).FirstOrDefaultAsync();
                if (CurrentCertificado != null)
                {
                    CurrentCertificado.Liberado = true;
                    await db.SaveChangesAsync();
                    return this.Redireccionar("Certificado","Index", new { idTipo = CurrentCertificado.Tipo }, $"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}");
                }
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.BorradoNoSatisfactorio}");
            }
        }

        [Authorize(Policy = "Administracion")]
        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                var CurrentCertificado = await db.Certificado.Where(x => x.CertificadoId == id).FirstOrDefaultAsync();
                if (CurrentCertificado != null)
                {
                    uploadFileService.DeleteFile(CurrentCertificado.FileUrl);
                    var result = db.Certificado.Remove(CurrentCertificado);
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

        [HttpPost]
        public async Task<IActionResult> Finalizar(Certificado certificado, IFormFile file)
        {
            if (file==null)
            {
                this.TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.SeleccionarFichero}";
                var productoFinal = await db.ProductoFinal.Where(x => x.ProductoFinalId == certificado.ProductoFinalId).FirstOrDefaultAsync();
                certificado.ProductoFinal = productoFinal != null ? new ProductoFinal { Codigo = productoFinal.Codigo, Descripcion = productoFinal.Descripcion } : new ProductoFinal { Codigo = "N/A", Descripcion = "N/A" };
                certificado.Liberado = certificado.Liberado == null ? false : certificado.Liberado;
                return View(certificado);
            }

            var responseFile = false;
            if (file != null)
            {
                byte[] data;
                using (var br = new BinaryReader(file.OpenReadStream()))
                    data = br.ReadBytes((int)file.OpenReadStream().Length);

                if (data.Length > 0)
                {
                    var activoFijoDocumentoTransfer = new DocumentoTransfer { Nombre = file.FileName, Fichero = data, IdRequisito = certificado.CertificadoId };
                    responseFile = await uploadFileService.UploadFiles(activoFijoDocumentoTransfer);
                }
            }
            // var upload1file = await UploadFile.Services.UploadFileService.UploadFiles(files, "Certificados", _hostingEnvironment.WebRootPath);
            if (responseFile)
            {
                var certificadoUpdate = await db.Certificado.Where(x => x.CertificadoId == certificado.CertificadoId).FirstOrDefaultAsync();
                certificadoUpdate.ArchivoCargado = true;
                await db.SaveChangesAsync();
                return this.Redireccionar($"{Mensaje.MensajeSatisfactorio}|{Mensaje.Satisfactorio}", "Index");
            }
            return this.Redireccionar($"{Mensaje.Aviso}|{Mensaje.ErrorUploadFiles}", "Index");
        }

        public async Task<IActionResult> Finalizar(Certificado certificado)
        {
            var productoFinal = await db.ProductoFinal.Where(x => x.ProductoFinalId == certificado.ProductoFinalId).FirstOrDefaultAsync();
            certificado.ProductoFinal =productoFinal !=null ?  new ProductoFinal { Codigo = productoFinal.Codigo, Descripcion = productoFinal.Descripcion } : new ProductoFinal { Codigo ="N/A", Descripcion = "N/A" };
           // certificado.Liberado = certificado.Liberado;
            return View(certificado);

        }


        private async Task<List<Analisis>> CargarComboOrdenes()
        {
            var listadoOrdenes = await db.Analisis.Where(x => x.Resultado == "CUMPLE").Select(x => new Analisis { NumeroOrden = x.NumeroOrden ,Resultado=x.Resultado}).Distinct().ToListAsync();

            var listaOrdenesFinal = new List<Analisis>();
            foreach (var item in listadoOrdenes)
            {
                var cantidad = await db.Analisis.Where(x => x.NumeroOrden == item.NumeroOrden && x.Resultado=="CUMPLE").CountAsync();
                if (cantidad >= Constantes.AnalisisPrevios)
                {
                    listaOrdenesFinal.Add(item);
                }
            }
            return listaOrdenesFinal;
        } 

        [HttpPost]
        public async Task<IActionResult> Create(Certificado certificado)
        {

           

            if (certificado.ListaOrdenes==null)
            {
                

                ViewData["IdProductoFinal"] = new SelectList(await db.ProductoFinal.Select(x => new ProductoFinal { ProductoFinalId = x.ProductoFinalId, Codigo = string.Format("{0} | {1}", x.Codigo, x.Descripcion) }).ToListAsync(), "ProductoFinalId", "Codigo");
                ViewData["OrdenesId"] = new SelectList(await CargarComboOrdenes(), "NumeroOrden", "NumeroOrden");
                ViewData["IdCliente"] = new SelectList(await db.Cliente.Select(x => new Cliente { ClienteId = x.ClienteId, NombreCliente = x.NombreCliente }).ToListAsync(), "NombreCliente", "NombreCliente");
                this.TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.DebeSeleccionarOrdenes}";
                ModelState.AddModelError("ListaOrdenes", Mensaje.DebeSeleccionarOrdenes);
                return View(certificado);
               
            }

            for (int i = 0; i < certificado.ListaOrdenes.Length; i++)
            {
                if (i == certificado.ListaOrdenes.Length-1)
                {
                    certificado.NumeroOrden = certificado.NumeroOrden + certificado.ListaOrdenes[i];
                }
                else
                {
                    certificado.NumeroOrden = certificado.NumeroOrden + certificado.ListaOrdenes[i] + ";";
                }
            }

            

            if (certificado.Estado == 1)
            {
                var certificadoUpdate = await db.Certificado.Where(x => x.CertificadoId == certificado.CertificadoId).FirstOrDefaultAsync();

                certificadoUpdate.ProductoFinalId = certificado.ProductoFinalId == 0 ? null : certificado.ProductoFinalId;
                certificadoUpdate.CodigoCertificado = certificado.CodigoCertificado;
                certificadoUpdate.FechaGeneracion = certificado.FechaGeneracion;
                certificadoUpdate.NumeroOrden = certificado.NumeroOrden;
                certificadoUpdate.NumeroGuia = certificado.NumeroGuia;
                certificadoUpdate.Tipo = certificado.Tipo;
                certificadoUpdate.OrdenVenta = certificado.OrdenVenta;
                certificadoUpdate.PedidoVenta = certificado.PedidoVenta;
                certificadoUpdate.Valor = certificado.Valor;
                certificadoUpdate.Peso = certificado.Peso;
                certificadoUpdate.VerMateriaPrima = certificado.VerMateriaPrima;
                certificadoUpdate.ArchivoCargado = false;
                certificadoUpdate.OrdenCliente = certificado.OrdenCliente;
                certificadoUpdate.Referencia = certificado.Referencia;
                certificadoUpdate.NumeroFactura = certificado.NumeroFactura;
                certificadoUpdate.PartidaArancelaria = certificado.PartidaArancelaria;
                certificadoUpdate.Liberado = certificado.Liberado;
                certificadoUpdate.NombreCliente = certificado.NombreCliente;
                certificadoUpdate.OrdenFinal = certificado.OrdenFinal;

                if (certificadoUpdate.ArchivoCargado==true)
                {
                    uploadFileService.DeleteFile(certificadoUpdate.FileUrl);
                    certificadoUpdate.FileUrl = "";
                }
               
                await db.SaveChangesAsync();
            }
            else
            {
                certificado.ProductoFinalId = certificado.ProductoFinalId == 0 ? null : certificado.ProductoFinalId;
                certificado.NombreCliente = certificado.NombreCliente == "0" ? "" : certificado.NombreCliente;
                // certificado.Liberado = false;
                await db.Certificado.AddAsync(certificado);
                await db.SaveChangesAsync();
                certificado.CodigoCertificado = string.Format("{0}", certificado.CertificadoId);
                await db.SaveChangesAsync();
            }
            certificado.Estado = 1;
            certificado.CumpleFechaOrdenes = true;
            if (certificado.Tipo == 1)
            {
                certificado.CumpleFechaOrdenes = certificado.Liberado == false ? await ValidarNumeroOrden(certificado.ListaOrdenes, certificado.FechaGeneracion.Date, certificado.CertificadoId) : true;
            }
           
            

            if (certificado.CumpleFechaOrdenes==false)
            {
                var listadoEmails = new List<string>();
                var listaAdministradores = await userManager.GetUsersInRoleAsync(Perfiles.Administracion);


                foreach (var item in listaAdministradores)
                {
                    listadoEmails.Add(item.Email);
                }
                
                var cuerpo = emailSender.CuerpoCertificadoFueraDeFecha(User.Identity.Name, certificado);
                emailSender.SendEmailAsync(listadoEmails, Mensaje.AsuntoCorreo, cuerpo);
            }

            return RedirectToAction("Finalizar", certificado);

        }


        public IActionResult Editar(int id)
        {
            return RedirectToAction("Create",new { id });

        }
    }


}