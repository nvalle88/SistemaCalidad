﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnviarCorreo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NumberGenerate;
using SendMails.methods;
using SistemaCalidad.Data;
using SistemaCalidad.Extensions;
using SistemaCalidad.Models;
using SistemaCalidad.Services;
using SistemaCalidad.Utils;

namespace SistemaCalidad.Controllers
{
    [Authorize(Policy = "Administracion")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailSender emailSender;
        public IConfiguration Configuration { get; }
        public UsersController(IEmailSender _emailSender,IConfiguration configuration,ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.emailSender = _emailSender;
            this.userManager = userManager;
            this.roleManager = roleManager;
            db = context;
            Configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var lista = await db.Users.ToListAsync();
                return View(lista);
            }
            catch (Exception)
            {
                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ErrorListado}";
                return View();
            }

        }

        public async Task<IActionResult> Manage(string id)
        {
            try
            {
                ViewBag.accion =string.IsNullOrEmpty(id) == true ? "Crear" : "Editar";
                ViewData["IdRol"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await db.Roles.ToListAsync(), "Name", "Name");
                if (id != null)
                {
                    var user = await db.Users.FirstOrDefaultAsync(c => c.Id == id);
                    if (user == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");

                   
                    var rol=await db.UserRoles.Where(x => x.UserId == id).FirstOrDefaultAsync();
                    var currentRol =await db.Roles.Where(x=>x.Id==rol.RoleId).FirstOrDefaultAsync();
                    return View(new RegisterViewModel { Status = user.Status, IdRol=currentRol.Name,Id=user.Id,Address=user.Address,Email=user.Email,LastName=user.LastName,Name=user.Name,PhoneNumber=user.PhoneNumber});
                }
                return View(new RegisterViewModel { Status = true, });
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.ErrorCargarDatos}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(RegisterViewModel user)
        {
            try
            {
                ViewBag.accion = string.IsNullOrEmpty(user.Id) == true ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    var existeRegistro = false;
                    if (string.IsNullOrEmpty(user.Id) == true)
                    {
                        if (!await db.Users.AnyAsync(c => c.UserName.ToUpper().Trim() == user.Email.ToUpper().Trim()))
                        {
                            var RegistredUser = new ApplicationUser
                            {
                                Name = user.Name,
                                LastName = user.LastName,
                                Address = user.Address,
                                PhoneNumber = user.PhoneNumber,
                                UserName = user.Email,
                                Email = user.Email,
                                EmailConfirmed = false,
                                Status=user.Status,
                            };
                            var password = "Bekaert"+ GenerateNumber.Generate().ToString();
                            var z= await userManager.CreateAsync(RegistredUser, password);
                            var userd =await userManager.FindByEmailAsync(user.Email);
                            if (!await userManager.IsInRoleAsync(userd, user.IdRol))
                            {
                                await userManager.AddToRoleAsync(userd, user.IdRol);
                            }




                            var listadoEmails = new List<string>();
                            listadoEmails.Add(user.Email);
                            var cuerpo = emailSender.CuerpoCreateUser(Configuration.GetSection("CreateUsuarioCorreo").Value, string.Format("{0} {1}",userd.Name,userd.LastName),
                                                                      userd.Email,password, Configuration.GetSection("EmailLink").Value);
                            emailSender.SendEmailAsync(listadoEmails, Mensaje.AsuntoCorreo, cuerpo);

                           
                        }
                           
                        else
                            existeRegistro = true;
                    }
                    else
                    {
                        if (!await db.Users.Where(c => c.UserName.ToUpper().Trim() == user.Email.ToUpper().Trim()).AnyAsync(c => c.Id != user.Id))
                        {
                          var CurrentUser=await   userManager.FindByIdAsync(user.Id);
                            CurrentUser.LastName = user.LastName;
                            CurrentUser.Name = user.Name;
                            CurrentUser.Address = user.Address;
                            CurrentUser.PhoneNumber = user.PhoneNumber;
                            CurrentUser.UserName = user.Email;
                            CurrentUser.Email = user.Email;
                            CurrentUser.Status = user.Status;

                            var RolsUser= await db.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();

                            db.UserRoles.RemoveRange(RolsUser);
                            await db.SaveChangesAsync();

                            if (!await userManager.IsInRoleAsync(CurrentUser, user.IdRol))
                            {
                                await userManager.AddToRoleAsync(CurrentUser, user.IdRol);
                            }
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
                        this.TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ExisteRegistro}";
                    ViewData["IdRol"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await db.Roles.ToListAsync(), "Name", "Name",user.IdRol);
                    return View(user);
                }
                this.TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.CorregirFormulario}";
                ViewData["IdRol"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await db.Roles.ToListAsync(), "Name", "Name",user.IdRol);
                return View(user);

            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
            }
        }


        
        public async Task<IActionResult> Delete(string id)
        {
            
            try
            {
                var CurrentUser = await userManager.FindByIdAsync(id);
                if (CurrentUser != null)
                {
                   var result =await userManager.DeleteAsync(CurrentUser);
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