#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SistemaCalidad.Data;
using SistemaCalidad.Models;
using SistemaCalidad.Models.BussinessViewModels;
using SistemaCalidad.Utils;

#endregion

namespace SistemaCalidad.Controllers
{
   
    public class ItemIndice
    {
        public int periodo { get; set; }
        public int dato { get; set; }
    }

    

    [Authorize]
    public class HomeController : Controller
    {
         public IConfiguration Configuration { get; }
   
        private readonly CALIDADContext db;

        public HomeController(CALIDADContext context, IConfiguration configuration)
        {
            db = context;
            Configuration = configuration;
        }

        public async  Task<IActionResult> Index()
        {

            IndiceEstudiosViewModel i = new IndiceEstudiosViewModel
            {
                Realizados = await db.Analisis.Where(x => x.Resultado != "").CountAsync(),
                Cumple = await db.Analisis.Where(x => x.Resultado == "CUMPLE").CountAsync(),
                NoCumple = await db.Analisis.Where(x => x.Resultado == "NO CUMPLE").CountAsync()
            };

            if (i.Realizados != 0)
            {
                i.CumplePorcentaje = Math.Round(Convert.ToDecimal(Convert.ToDecimal(i.Cumple * 100) / Convert.ToDecimal(i.Realizados)), 2);
                i.NoCumplePorcentaje = Math.Round(Convert.ToDecimal(Convert.ToDecimal(i.NoCumple * 100) / Convert.ToDecimal(i.Realizados)), 2);
            }
            else
            {
                i.CumplePorcentaje = 0;
                i.NoCumplePorcentaje = 0;
            }

            return View(i);
        }

        [HttpGet]
        public async Task<JsonResult> Indices()
        {
            Constantes.MesesGrafica = Convert.ToInt32(Configuration.GetSection("MesesGrafica").Value);
            var ListaAnalisis = new List<Analisis>();
            var fechaInicio = DateTime.Now.AddMonths(-Constantes.MesesGrafica).Date;
            var fechaFin = DateTime.Now.Date;
            ListaAnalisis = await db.Analisis.Where(x => x.FechaAnalisis >= fechaInicio && x.FechaAnalisis <= fechaFin).ToListAsync();
            var query = ListaAnalisis.GroupBy(a => a.FechaAnalisis.Date).Select(g => new { Realizados = g.Count(), Fecha = g.Key });

            var ListaAnalisisCumplidos = new List<Analisis>();
            var fechaInicioCumplidos = DateTime.Now.AddMonths(-Constantes.MesesGrafica).Date;
            var fechaFinCumplidos = DateTime.Now.Date;
            ListaAnalisisCumplidos = await db.Analisis.Where(x => x.FechaAnalisis >= fechaInicio && x.FechaAnalisis <= fechaFin && x.Resultado == "CUMPLE").ToListAsync();
            var queryCumplidos = ListaAnalisisCumplidos.GroupBy(a => a.FechaAnalisis.Date).Select(g => new { Realizados = g.Count(), Fecha = g.Key });

            var ListaAnalisisNoCumplidos = new List<Analisis>();
            var fechaInicioNoCumplidos = DateTime.Now.AddMonths(-Constantes.MesesGrafica).Date;
            var fechaFinNoCumplidos = DateTime.Now.Date;
            ListaAnalisisNoCumplidos = await db.Analisis.Where(x => x.FechaAnalisis >= fechaInicio && x.FechaAnalisis <= fechaFin && x.Resultado == "NO CUMPLE").ToListAsync();
            var queryNoCumplidos = ListaAnalisisNoCumplidos.GroupBy(a => a.FechaAnalisis.Date).Select(g => new { Realizados = g.Count(), Fecha = g.Key });

            return Json(new
            {
                indices = query.OrderBy(f => f.Fecha),
                indicesCumple = queryCumplidos.OrderBy(f => f.Fecha),
                indicesNoCumple = queryNoCumplidos.OrderBy(f => f.Fecha),
            });
        }

        [Route("dashboard-marketing")]
        public IActionResult DashboardMarketing() => View();

        [Route("dashboard-social")]
        public IActionResult SocialWall() => View();

        public IActionResult Inbox() => View();

        public IActionResult Chat() => View();

        public IActionResult Widgets() => View();

        public IActionResult Error() => View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
