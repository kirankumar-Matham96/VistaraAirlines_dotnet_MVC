using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using VistaraAirLinesApp.Controllers;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;
using VistaraAirLinesApp.Services.Interfaces;
using VistaraAirLinesApp.Services;

namespace VistaraAirLinesApp.Controllers
{
    [Authorize]
    public class BookingsController : BaseController
    {
        IBookingService _bookingService;
        ICancellationService _cancellationService;

        public BookingsController()
        {
            _bookingService = new BookingService();
        }

        //Search available flights(get flights with filter)
        public ActionResult SearchFlights()
        {
            var flightSearchViewModel = _bookingService.GetSearchFlightPageData();

            return View(flightSearchViewModel);
        }

        [HttpPost]
        public ActionResult SearchFlights(FlightSearchViewModel flightSearchViewModel)
        {
            var flights = _bookingService.SearchFlights(flightSearchViewModel);

            return View(flights);
        }

        public ActionResult AddBooking(int id)
        {
            var bookingVM = _bookingService.GetBookingPageData(id);

            if (bookingVM == null)
            {
                return HttpNotFound();
            }

            return View(bookingVM);
        }

        [HttpPost]
        public ActionResult AddBooking(BookingsViewModel bookingViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(bookingViewModel);
                }

                var bookingModel = _bookingService.AddBooking(bookingViewModel);

                // STORE BOOKING TEMPORARILY
                TempData["BookingData"] = Newtonsoft.Json.JsonConvert.SerializeObject(bookingModel);

                return RedirectToAction("Checkout", "Payments");
            }
            catch (Exception ex)
            {
                Exception inner = ex;

                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }

                ModelState.AddModelError("", inner.Message);

                return View(bookingViewModel);
            }


        }

        //Booking Confirmation after payment success
        public ActionResult ConfirmBooking()
        {
            if (TempData["BookingData"] == null)
            {
                return RedirectToAction("SearchFlights", "Bookings");
            }

            var bookingsViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<BookingsViewModel>(TempData["BookingData"].ToString());
            try
            {
                _bookingService.ConfirmBooking(bookingsViewModel);
                return RedirectToAction("BookingHistory", "Bookings");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        // Booking history
        public ActionResult BookingHistory()
        {
            var bookings = _bookingService.GetBookingHistory();
            return View(bookings);
        }

        //View booking details
        public ActionResult BookingDetails(int id)
        {
            var ticketViewModel = _bookingService.GetBookingDetails(id);

            if (ticketViewModel == null)
            {
                return HttpNotFound();
            }

            return View(ticketViewModel);
        }

        //CancellationController
        public ActionResult CancelBooking(int id)
        {
            try
            {
                var cancellationViewModel = _cancellationService.GetCancellationData(id);

                if (cancellationViewModel == null)
                {
                    return HttpNotFound();
                }

                return View(cancellationViewModel);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult CancelBooking(CancellationViewModel cancellationViewModel)
        {
            try
            {
                _cancellationService.CancelBooking(cancellationViewModel);
                return RedirectToAction(nameof(BookingHistory));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View(cancellationViewModel);
            }
        }
    }
}