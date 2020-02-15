using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SistemaCalidad.Data;
using SistemaCalidad.Services;

namespace SistemasLegales.Controllers
{
    public class ReportController : Controller
    {
        private readonly CALIDADContext db;
        private readonly IReporteServicio reporteServicio;
        public IConfiguration Configuration { get; }
        public ReportController(IReporteServicio reporteServicio, IConfiguration Configuration, CALIDADContext context)
        {
            this.Configuration = Configuration;
            this.reporteServicio = reporteServicio;
            this.db = context;
        }
        public IActionResult RepCompatibilidadMateriaPrima()
        {
            var parametersToAddNacional = reporteServicio.GetDefaultParameters(Configuration.GetSection("RepCompatibilidadMateriaPrima").Value);
            var newUri = reporteServicio.GenerateUri(parametersToAddNacional);
            return Redirect(newUri);
        }

        public IActionResult RepAlambonesProducto()
        {
            var parametersToAddNacional = reporteServicio.GetDefaultParameters(Configuration.GetSection("RepAlambonesProducto").Value);
            var newUri = reporteServicio.GenerateUri(parametersToAddNacional);
            return Redirect(newUri);
        }

        public IActionResult RepMateriaPrima()
        {
            var parametersToAddNacional = reporteServicio.GetDefaultParameters(Configuration.GetSection("RepMateriaPrima").Value);
            var newUri = reporteServicio.GenerateUri(parametersToAddNacional);
            return Redirect(newUri);
        }

        public IActionResult RepProductos()
        {
            var parametersToAddNacional = reporteServicio.GetDefaultParameters(Configuration.GetSection("RepProductos").Value);
            var newUri = reporteServicio.GenerateUri(parametersToAddNacional);
            return Redirect(newUri);
        }

        public IActionResult RepAnalisis()
        {
            var parametersToAddNacional = reporteServicio.GetDefaultParameters(Configuration.GetSection("RepAnalisis").Value);
            var newUri = reporteServicio.GenerateUri(parametersToAddNacional);
            return Redirect(newUri);
        }

        public IActionResult RepCompatibilidadMateriaEspec()
        {
            var parametersToAddNacional = reporteServicio.GetDefaultParameters(Configuration.GetSection("RepCompatibilidadMateriaEspec").Value);
            var newUri = reporteServicio.GenerateUri(parametersToAddNacional);
            return Redirect(newUri);
        }


        public async Task<IActionResult> Certificado(int id)
        {
            var certificado = await db.Certificado.Where(x => x.CertificadoId == id).FirstOrDefaultAsync();

            switch (certificado.Tipo)
            {
                case 1:
                    var parametersToAddNacional = reporteServicio.GetDefaultParameters(Configuration.GetSection("CertificadoNacional").Value);
                    var param1Nacional = reporteServicio.AddParameters("Id", Convert.ToString(certificado.CertificadoId), parametersToAddNacional);
                    var newUriNacional = reporteServicio.GenerateUri(param1Nacional);
                    return Redirect(newUriNacional);
                case 2:
                    var parametersToAddExtranjero = reporteServicio.GetDefaultParameters(Configuration.GetSection("CertificadoExtranjero").Value);
                    var param1Extranjero = reporteServicio.AddParameters("Id", Convert.ToString(certificado.CertificadoId), parametersToAddExtranjero);
                    var newUriExtranjero = reporteServicio.GenerateUri(param1Extranjero);
                    return Redirect(newUriExtranjero);
                case 3:
                    var parametersToAddMexico = reporteServicio.GetDefaultParameters(Configuration.GetSection("CertificadoMexico").Value);
                    var param1Mexico = reporteServicio.AddParameters("Id", Convert.ToString(certificado.CertificadoId), parametersToAddMexico);
                    var newUriMexico = reporteServicio.GenerateUri(param1Mexico);
                    return Redirect(newUriMexico);

                case 4:
                    var parametersToAddIngles = reporteServicio.GetDefaultParameters(Configuration.GetSection("CertificadoIngles").Value);
                    var param1Ingles = reporteServicio.AddParameters("Id", Convert.ToString(certificado.CertificadoId), parametersToAddIngles);
                    var newUriIngles = reporteServicio.GenerateUri(param1Ingles);
                    return Redirect(newUriIngles);
            }

            return StatusCode(400, "Los parámetros solicitados no cumplen para la generación del certificado, por favor comuníquese con el administrador para obtener  más información.");
        }
    }
}

