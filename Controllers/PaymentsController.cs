using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;

namespace VistaraAirLinesApp.Controllers
{
    public class PaymentsController : Controller
    {
        VISTARA_DBEntities4 _db = new VISTARA_DBEntities4();

        // GET: Payments
        public ActionResult Checkout()
        {
            if (TempData["BookingData"] == null)
            {
                return RedirectToAction("SearchFlights", "Bookings");
            }

            var bookingViewModel = JsonConvert.DeserializeObject<BookingsViewModel>(TempData["BookingData"].ToString());
            TempData.Keep("BookingData");

            // here add the razorpay

            return View(bookingViewModel);
        }

        [HttpPost]
        public ActionResult PaymentSuccess()
        {
            if (TempData["BookingData"] == null)
            {
                return RedirectToAction("SearchFlights","Bookings");
            }


            return RedirectToAction("ConfirmBooking", "Bookings");
        }
    }
}