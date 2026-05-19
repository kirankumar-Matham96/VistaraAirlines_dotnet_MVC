using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using VistaraAirLinesApp.Helpers;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;
using VistaraAirLinesApp.Services;

namespace VistaraAirLinesApp.Controllers
{
    public class UserController : BaseController
    {
        private readonly UserService _service;

        public UserController()
        {
            _service = new UserService();
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
                    _service.AddUser(user);
                    return RedirectToAction("LoginUser");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ExceptionHelper.GetExceptionMessage(ex));
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
            try
            {
                if (ModelState.IsValid)
                {
                    var loggediUser = _service.UserLogin(user);

                    SessionHelper.UserId = loggediUser.UserId;
                    SessionHelper.UserName = loggediUser.UserName;
                    SessionHelper.UserRole = loggediUser.Role;

                    FormsAuthentication.SetAuthCookie(loggediUser.UserName, false);

                    if (loggediUser.Role == "MANAGER")
                    {
                        return RedirectToAction("GetAllFlights", "Flight");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                
                ModelState.AddModelError("UserId", "Invalid UserId");
                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ExceptionHelper.GetExceptionMessage(ex));
                return View(user);
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            FormsAuthentication.SignOut();

            return RedirectToAction(nameof(LoginUser));
        }
    }
}