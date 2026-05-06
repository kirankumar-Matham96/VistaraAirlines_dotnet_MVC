using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;

namespace VistaraAirLinesApp.Controllers
{
    public class FlightController : Controller
    {
        VISTARA_DBEntities4 db;

        public FlightController() {
            db = new VISTARA_DBEntities4();
        }

        [NonAction]
        public void GetTimeRanges()
        {
            TempData["HHRange"] = Enumerable.Range(0, 24).ToList();
            TempData["MMRange"] = Enumerable.Range(0, 60).ToList();
            TempData["SSRange"] = Enumerable.Range(0, 60).ToList();
            
            //TempData.Keep("HHRange");
            //TempData.Keep("MMRange");
            //TempData.Keep("SSRange");
        }

        // GET: Flight
        public ActionResult AddFlight()
        {
            GetTimeRanges();
            return View();
        }

        [HttpPost]
        public ActionResult AddFlight(FlightViewModel flight)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // do something here
                }

                GetTimeRanges();
                return View(flight);
            }
            catch (Exception ex) {
                //ModelState.AddModelError("", "Something went wrong while adding flight");
                ModelState.AddModelError("", ex.Message);
                return View(flight);
            }
        }
    }
}