using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;

namespace VistaraAirLinesApp.Controllers
{
    public class UserController : Controller
    {
        VISTARA_DBEntities4 db;

        public UserController()
        {
            db = new VISTARA_DBEntities4();
        }

        // GET: User
        public ActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser(UserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newUser = new User();

                    newUser.FullName = $"{user.Firstname} {user.Lastname}";
                    newUser.UserName = user.UserName;
                    newUser.Email = user.Email;
                    newUser.Role = user.Role;
                    newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, workFactor: 12);
                    newUser.CreatedAt = DateTime.Now;
                    newUser.UpdatedAt = DateTime.Now;

                    db.Users.Add(newUser);
                    db.SaveChanges();
                }

                return View(user);
            }
            catch (Exception ex) {
                var sqlExpetion = ex.InnerException.InnerException as System.Data.SqlClient.SqlException;
                if (sqlExpetion != null && sqlExpetion.Message.Contains("UQ_USER_EMAIL"))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                }
                else { 
                    ModelState.AddModelError("", "Something went wrong");
                }

                return View(user);
            }
        }

        public ActionResult LoginUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginUser(LoginViewModel user)
        {
            try {
                if (ModelState.IsValid)
                {
                    var userExists = db.Users.FirstOrDefault(u => (u.UserName == user.UserId || u.Email == user.UserId) && u.Role == user.Role);

                    if (userExists != null)
                    {
                        var passwordMatch = BCrypt.Net.BCrypt.Verify(user.Password, userExists.PasswordHash);

                        if (!passwordMatch) {
                            ModelState.AddModelError("Password", "Invalid Password");
                            return View(user);
                        }

                        RedirectToAction("");
                    }
                    else {
                        ModelState.AddModelError("UserId", "Invalid UserId");
                    }
                }

                return View(user);
            } catch (SqlException ex) {
                ModelState.AddModelError("", ex.Message);
                return View(user);
            }
        }

    }
}