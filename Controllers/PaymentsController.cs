using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using VistaraAirLinesApp.Helpers;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;

namespace VistaraAirLinesApp.Controllers
{
    [Authorize]
    public class PaymentsController : BaseController
    {

        // GET: Payments
        public ActionResult Checkout()
        {
            try
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
            catch (Exception ex)
            {
                return Content(ExceptionHelper.GetExceptionMessage(ex));
            }
        }

        [HttpPost]
        public ActionResult PaymentSuccess()
        {
            try
            {
                if (TempData["BookingData"] == null)
                {
                    return Content(TempData["BookingData"].ToString());
                    //return RedirectToAction("SearchFlights","Bookings");
                }
                return RedirectToAction("ConfirmBooking", "Bookings");
            }
            catch (Exception ex)
            {
                return Content(ExceptionHelper.GetExceptionMessage(ex));
            }
        }
    }
}