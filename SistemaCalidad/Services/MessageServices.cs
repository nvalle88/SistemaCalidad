using EnviarCorreo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendMails.methods;
using SistemaCalidad.Data;
using SistemaCalidad.Models;
using SistemaCalidad.Models.Utiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaCalidad.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {

        public IConfiguration Configuration { get; }
        private readonly CALIDADContext db;


        public AuthMessageSender(IConfiguration configuration,  CALIDADContext context)
        {
            this.db = context;
            Configuration = configuration;
        }

       


        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }

        public async Task SendEmailAsync(List<string> emailsTo, string subject, string message)
        {
            try
            {
                List<Mail> mails = new List<Mail>();
                foreach (var item in emailsTo)
                {
                    mails.Add(new Mail
                    {
                        Body = message,
                        EmailTo = item,
                        NameTo = "Laboratorio de Calidad",
                        Subject = subject
                    });
                }
                await Emails.SendEmailAsync(mails);
            }
            catch (Exception)
            { }
        }


        public async Task<string> CuerpoAnalisisNoValido(Analisis analisis)
        {
            
            var mensaje= Configuration.GetSection("CreacionAnalisisNoValido").Value;
            mensaje = mensaje.Contains("@FechaAnalisis") ? mensaje.Replace("@FechaAnalisis", analisis.FechaAnalisis.ToString("dd/MM/yyyy")) : mensaje;
            mensaje = mensaje.Contains("@NumeroOrden") ? mensaje.Replace("@NumeroOrden", analisis.NumeroOrden) : mensaje;
            var producto =await db.Producto.Where(x => x.ProductoId == analisis.ProductoId).FirstOrDefaultAsync();
            mensaje = mensaje.Contains("@CodigoProducto") ? mensaje.Replace("@CodigoProducto", string.Format("{0} | {1}",producto?.CodigoProducto,producto.DescripcionProducto)) : mensaje;
            mensaje = mensaje.Contains("@Observaciones") ? mensaje.Replace("@Observaciones", analisis.Observaciones) : mensaje;
            mensaje = mensaje.Contains("@NombreUsuario") ? mensaje.Replace("@NombreUsuario", analisis.NombreUsuario) : mensaje;
            return mensaje;

        }

        public string CuerpoCertificadoFueraDeFecha(string nombreUsuario, Certificado certificado)
        {
            var mensaje = Configuration.GetSection("CertificadoFueraDeFecha").Value;
            mensaje = mensaje.Contains("@FechaGeneracion") ? mensaje.Replace("@FechaGeneracion", certificado.FechaGeneracion.ToString("dd/MM/yyyy")) : mensaje;
            mensaje = mensaje.Contains("@CertificadoId") ? mensaje.Replace("@CertificadoId", certificado.CertificadoId.ToString()) : mensaje;
            mensaje = mensaje.Contains("@NombreUsuario") ? mensaje.Replace("@NombreUsuario",nombreUsuario) : mensaje;
            return mensaje;

        }

        public string CuerpoCreateUser(string titulo,string nombreApellido, string email, string password, string emailLink)
        {
            var mensaje = Configuration.GetSection("CuerpoUsuarioCreate").Value;
            mensaje = mensaje.Contains("@nombreApellido") ? mensaje.Replace("@nombreApellido", nombreApellido) : mensaje;
            mensaje = mensaje.Contains("@Usuario") ? mensaje.Replace("@Usuario", email) : mensaje;
            mensaje = mensaje.Contains("@password") ? mensaje.Replace("@password", password) : mensaje;
            mensaje = mensaje.Contains("@emailLink") ? mensaje.Replace("@emailLink", emailLink) : mensaje;
            mensaje = mensaje.Contains("@titulo") ? mensaje.Replace("@titulo", titulo) : mensaje;
            return mensaje;
        }
    }
}
