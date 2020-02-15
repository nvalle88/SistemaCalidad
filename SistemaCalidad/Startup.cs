using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaCalidad.Data;
using SistemaCalidad.Models;
using SistemaCalidad.Services;
using SistemaCalidad.Utils;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using NumberGenerate;
using EnviarCorreo;
using Microsoft.Extensions.Options;
using SistemaCalidad.Models.Utiles;

namespace SistemaCalidad
{
    public class Startup
    {
       
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            Constantes.ConexionDB = "Server=DESKTOP-95R0B9N\\MSSQLSERVER2017;Database=CALIDAD;User Id=sa;password=nvr1604;MultipleActiveResultSets=true";
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Constantes.ConexionDB));
            //Configuration.GetConnectionString("DefaultConnection"))
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddDbContext<CALIDADContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            GenerateNumber.Lower = Convert.ToInt32(Configuration.GetSection("LowerRandom").Value);
            GenerateNumber.Top = Convert.ToInt32(Configuration.GetSection("TopRandom").Value);

            
            Constantes.IdEspecificacionDiamtroMaterial = Convert.ToInt32(Configuration.GetSection("IdEspecificacionDiamtroMaterial").Value);
            Constantes.IdAlambron = Convert.ToInt32(Configuration.GetSection("IdAlambron").Value);

            Constantes.MesesListadoAnalisis = Convert.ToInt32(Configuration.GetSection("MesesListadoAnalisis").Value);
            Constantes.MesesGrafica= Convert.ToInt32(Configuration.GetSection("MesesGrafica").Value);
            Constantes.AnalisisPrevios = Convert.ToInt32(Configuration.GetSection("AnalisisPrevios").Value);
            Constantes.DiasEvaluarOrdenCertificado = Convert.ToInt32(Configuration.GetSection("DiasEvaluarOrdenCertificado").Value);

            ConstanteEspecificaciones.Bit= Configuration.GetSection("Bit").Value;
            ConstanteEspecificaciones.Numero = Configuration.GetSection("Numero").Value;
            ConstanteEspecificaciones.Rango = Configuration.GetSection("Rango").Value;
            ConstanteEspecificaciones.Texto = Configuration.GetSection("Texto").Value;

            Mensaje.CarpertaHost = Configuration.GetSection("CarpetaHost").Value;
            Mensaje.AsuntoCorreo = Configuration.GetSection("AsuntoCorreo").Value;

            MailConfig.HostUri = Configuration.GetSection("Smtp").Value;
            MailConfig.PrimaryPort = Convert.ToInt32(Configuration.GetSection("PrimaryPort").Value);
            MailConfig.SecureSocketOptions = Convert.ToInt32(Configuration.GetSection("SecureSocketOptions").Value);
            MailConfig.RequireAuthentication = Convert.ToBoolean(Configuration.GetSection("RequireAuthentication").Value);

            MailConfig.UserName = Configuration.GetSection("UsuarioCorreo").Value;
            MailConfig.Password = Configuration.GetSection("PasswordCorreo").Value;

            MailConfig.EmailFrom = Configuration.GetSection("EmailFrom").Value;
            MailConfig.NameFrom = Configuration.GetSection("NameFrom").Value;


            ReportConfig.DefaultNetworkCredentials = Convert.ToBoolean(Configuration.GetSection("DefaultNetworkCredentials").Value);

            if (!ReportConfig.DefaultNetworkCredentials)
            {
                ReportConfig.UserName = Configuration.GetSection("UserNameReport").Value;
                ReportConfig.Password = Configuration.GetSection("PasswordReport").Value;
                ReportConfig.CustomDomain = Configuration.GetSection("CustomDomain").Value;
            }
            ReportConfig.ReportServerUrl = Configuration.GetSection("ReportServerUrl").Value;
            ReportConfig.ReportFolderPath = Configuration.GetSection("ReportFolderPath").Value;
            ReportConfig.ProjectReportUrl = Configuration.GetSection("ProjectReportUrl").Value;
            ReportConfig.CompletePath = string.Format("{0}{1}", ReportConfig.ReportServerUrl, ReportConfig.ReportFolderPath);


            services.AddTransient<ApplicationDbSeeder>();   


            var TiempoVidaCookie = Convert.ToDouble(Configuration.GetSection("TiempoVidaCookie").Value);

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 4;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(TiempoVidaCookie);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(TiempoVidaCookie);
                // If the LoginPath isn't set, ASP.NET Core defaults 
                // the path to /Account/Login.
                options.LoginPath = "/Account/Login";
                // If the AccessDeniedPath isn't set, ASP.NET Core defaults 
                // the path to /Account/AccessDenied.
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
            });

            // Add application services.
            //services.AddTransient<IEmailSender, EmailSender>();


            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("Administracion", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Administración");
                });

                opts.AddPolicy("Gestion", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Gestión", "Administración");
                });

                opts.AddPolicy("Planificacion", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Planificación", "Administración");
                });

                opts.AddPolicy("Laboratorio", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Laboratorio", "Administración");
                });
            });

            services.AddMvc();

            services.AddTransient<IEmailSender, AuthMessageSender>();
           

            services.AddScoped<IUploadFileService, UploadFileService>();

            services.AddScoped<IReporteServicio, ReporteServicio>();
            services.AddScoped<IEncriptarServicio, EncriptarServicio>();
            services.AddMemoryCache();
            services.AddSession();
            services.AddDistributedMemoryCache();
            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbSeeder dbSeeder)
        {

            var defaultCulture = new CultureInfo("es-EC");
            defaultCulture.NumberFormat.NumberDecimalSeparator = ".";
            defaultCulture.NumberFormat.CurrencyDecimalSeparator = ".";
            //defaultCulture.DateTimeFormat = DateTimeFormatInfo.CurrentInfo;
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture },
                FallBackToParentCultures = false,
                FallBackToParentUICultures = false,
                RequestCultureProviders = new List<IRequestCultureProvider> { }
            });
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            if (!env.IsProduction())
                // Ensure we have the default user added to the store
                dbSeeder.EnsureSeed().GetAwaiter().GetResult();

            app.UseStaticFiles();

            app.UseAuthentication();
           

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseResponseCaching();
        }
    }
}
