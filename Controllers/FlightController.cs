using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;
using VistaraAirLinesApp.Services;
using VistaraAirLinesApp.Services.Interfaces;

namespace VistaraAirLinesApp.Controllers
{
    public class FlightController : Controller
    {
        //VISTARA_DBEntities4 db;
        FlightService _service;

        public FlightController()
        {
            //db = new VISTARA_DBEntities4();
            _service = new FlightService();
        }

        [NonAction]
        public void GetTimeRanges()
        {
            var timeRange = _service.GetTimeRanges();

            TempData["HHRange"] = timeRange.HHRange;
            TempData["MMRange"] = timeRange.MMRange;
            TempData["SSRange"] = timeRange.SSRange;
        }

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
                    _service.AddFlight(flight);
                    return RedirectToAction(nameof(GetAllFlights));
                }

                GetTimeRanges();
                return View(flight);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(flight);
            }
        }

        public ActionResult GetAllFlights()
        {
            try
            {
                var flights = _service.GetAllFlights();
                return View(flights);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult GetFlightDetails(int id)
        {
            try
            {
                var flightData = _service.GetFullFlightDetails(id);
                return View(flightData);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public ActionResult UpdateFlightDetails(int id)
        {
            try
            {
                GetTimeRanges();
                var flightData = _service.GetFullFlightDetails(id);
                return View(flightData);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
        [HttpPost]
        public ActionResult UpdateFlightDetails(FlightViewModel flight)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _service.UpdateFlightDetails(flight);
                    return RedirectToAction(nameof(GetAllFlights));
                }
                return View(flight);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public ActionResult DeleteFlight(int id)
        {
            try
            {
                _service.DeleteFlight(id);
                return RedirectToAction("GetAllFlights");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

    }
}