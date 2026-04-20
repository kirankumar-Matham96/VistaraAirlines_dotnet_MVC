using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;

namespace VistaraAirLinesApp.Controllers
{
    public class UserController : Controller
    {
        VISTARA_DBEntities db;

        public UserController() {
            db = new VISTARA_DBEntities();
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
            if (ModelState.IsValid) {
                return RedirectToAction("IndexEmployee");
            }

            return View(user);
        }
    }
}