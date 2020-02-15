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
    public class ClientesController : Controller
    {
        private readonly CALIDADContext db;

        public ClientesController(CALIDADContext context)
        {
            db = context;

        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var lista = await db.Cliente.ToListAsync();
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
                    var Cliente = await db.Cliente.FirstOrDefaultAsync(c => c.ClienteId == Convert.ToInt32(id));
                    if (Cliente == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");

                    return View(Cliente);
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
        public async Task<IActionResult> Manage(Cliente Cliente)
        {
            try
            {
                ViewBag.accion = Cliente.ClienteId == 0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    var existeRegistro = false;
                    if (Cliente.ClienteId == 0)
                    {
                        if (!await db.Cliente.AnyAsync(c => c.NombreCliente.ToUpper().Trim() == Cliente.NombreCliente.ToUpper().Trim()))
                        {
                            await db.AddAsync(Cliente);
                        }

                        else
                            existeRegistro = true;
                    }
                    else
                    {
                        if (!await db.Cliente.Where(c => c.NombreCliente.ToUpper().Trim() == Cliente.NombreCliente.ToUpper().Trim()).AnyAsync(c => c.ClienteId != Cliente.ClienteId))
                        {
                            var CurrentCliente = await db.Cliente.Where(x => x.ClienteId == Cliente.ClienteId).FirstOrDefaultAsync();
                            CurrentCliente.NombreCliente = Cliente.NombreCliente;
                            CurrentCliente.CodigoCliente = null;
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
                    return View(Cliente);
                }

                return View(Cliente);

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
                var CurrentCliente = await db.Cliente.Where(x => x.ClienteId == id).FirstOrDefaultAsync();
                if (CurrentCliente != null)
                {
                    var result = db.Cliente.Remove(CurrentCliente);
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