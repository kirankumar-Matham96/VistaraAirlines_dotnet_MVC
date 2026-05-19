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
using VistaraAirLinesApp.Helpers;

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
            try
            {
                var flightSearchViewModel = _bookingService.GetSearchFlightPageData();
                return View(flightSearchViewModel);

            }
            catch (Exception ex)
            {
                return Content(ExceptionHelper.GetExceptionMessage(ex));
            }
        }

        [HttpPost]
        public ActionResult SearchFlights(FlightSearchViewModel flightSearchViewModel)
        {
            try
            {
                var flights = _bookingService.SearchFlights(flightSearchViewModel);
                return View(flights);
            }
            catch (Exception ex)
            {
                return Content(ExceptionHelper.GetExceptionMessage(ex));
            }
        }

        public ActionResult AddBooking(int id)
        {
            try
            {
                var flightSearchViewModel = _bookingService.GetSearchFlightPageData();
                return View(flightSearchViewModel);

                var bookingVM = _bookingService.GetBookingPageData(id);

                if (bookingVM == null)
                {
                    return HttpNotFound();
                }

                return View(bookingVM);
            }
            catch (Exception ex)
            {
                return Content(ExceptionHelper.GetExceptionMessage(ex));
            }
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
                ModelState.AddModelError("", ExceptionHelper.GetExceptionMessage(ex));
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
                return Content(ExceptionHelper.GetExceptionMessage(ex));
            }
        }


        // Booking history
        public ActionResult BookingHistory()
        {
            try
            {
                List<BookingHistoryViewModel> bookings = _bookingService.GetBookingHistory(SessionHelper.UserId);
                return View(bookings);
            }
            catch (Exception ex)
            {
                return Content(ExceptionHelper.GetExceptionMessage(ex));
            }
        }

        //View booking details
        public ActionResult BookingDetails(int id)
        {

            try
            {
                var ticketViewModel = _bookingService.GetBookingDetails(id);

                if (ticketViewModel == null)
                {
                    return HttpNotFound();
                }

                return View(ticketViewModel);
            }
            catch (Exception ex)
            {
                return Content(ExceptionHelper.GetExceptionMessage(ex));
            }
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
                return Content(ExceptionHelper.GetExceptionMessage(ex));
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
                ModelState.AddModelError("", ExceptionHelper.GetExceptionMessage(ex));
                return View(cancellationViewModel);
            }
        }
    }
}