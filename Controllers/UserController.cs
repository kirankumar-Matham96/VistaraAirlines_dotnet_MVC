using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;
using VistaraAirLinesApp.Services;

namespace VistaraAirLinesApp.Controllers
{
    public class UserController : Controller
    {
        UserService _service;

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

                var inner = ex.InnerException;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                    ModelState.AddModelError("", inner.Message);
                }

                //var sqlExpetion = ex.InnerException.InnerException as SqlException;
                //if (sqlExpetion != null && sqlExpetion.Message.Contains("UQ_USER_EMAIL"))
                //{
                //    ModelState.AddModelError("Email", "Email already exists");
                //}
                //else
                //{
                //    ModelState.AddModelError("", "Something went wrong");
                //}

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
                    _service.UserLogin(user);

                    if (user.Role == "MANAGER")
                    {
                        return RedirectToAction("GetAllFlights", "Flight");
                    }
                    else
                    {
                        ModelState.AddModelError("UserId", "Invalid UserId");
                    }

                    return RedirectToAction("Index", "Home");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                var inner = ex;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }
                ModelState.AddModelError("", inner.Message);
                return View(user);
            }
        }

    }
}