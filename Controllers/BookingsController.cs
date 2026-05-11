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

namespace VistaraAirLinesApp.Controllers
{
    public class BookingsController : Controller
    {
        VISTARA_DBEntities4 _db = new VISTARA_DBEntities4();

        private readonly decimal GST_PERCENT = 10;
        
        //Search available flights(get flights with filter)
        public ActionResult SearchFlights()
        {
            FlightSearchViewModel flightSearchViewModel = new FlightSearchViewModel();

            flightSearchViewModel.Sources = _db.Flights.Select(f => f.Source).Distinct().ToList();
            flightSearchViewModel.Destinations = _db.Flights.Select(f => f.Destination).Distinct().ToList();
            flightSearchViewModel.TravelDates = _db.FlightInventories.Select(f => f.TravelDate).Distinct().ToList();
            flightSearchViewModel.FlightsList = (
                from f in _db.Flights
                join fi in _db.FlightInventories
                on f.FlightId equals fi.FlightId

                select new FlightViewModel()
                {
                    FlightId = f.FlightId,
                    FlightCode = f.FlightCode,
                    FlightName = f.FlightName,
                    Source = f.Source,
                    Destination = f.Destination,

                    ArrivalHrs = f.ArrivalTime.Hours,
                    ArrivalMin = f.ArrivalTime.Minutes,
                    ArrivalSec = f.ArrivalTime.Seconds,
                    ArrivalAmpm = f.ArrivalTime.Hours > 12 ? "PM" : "AM",

                    DepartureHrs = f.DepartureTime.Hours,
                    DepartureMin = f.DepartureTime.Minutes,
                    DepartureSec = f.DepartureTime.Seconds,
                    DepartureAmpm = f.DepartureTime.Hours > 12 ? "PM" : "AM",

                    ExecutiveSeats = fi.ExecutiveSeats,
                    ExecutiveFare = fi.ExecutiveFare,
                    BusinessSeats = fi.BusinessSeats,
                    BusinessFare = fi.BusinessFare,
                    EconomySeats = fi.EconomySeats,
                    EconomyFare = fi.EconomyFare,
                }
             ).ToList();

            return View(flightSearchViewModel);
        }

        [HttpPost]
        public ActionResult SearchFlights(FlightSearchViewModel flightSearchViewModel)
        {
            flightSearchViewModel.Sources = _db.Flights.Select(f => f.Source).Distinct().ToList();
            flightSearchViewModel.Destinations = _db.Flights.Select(f => f.Destination).Distinct().ToList();
            flightSearchViewModel.TravelDates = _db.FlightInventories.Select(f => f.TravelDate).Distinct().ToList();

            flightSearchViewModel.FlightsList =
            (
                from f in _db.Flights
                join fi in _db.FlightInventories
                on f.FlightId equals fi.FlightId
                where
                    f.Source == flightSearchViewModel.Source &&
                    f.Destination == flightSearchViewModel.Destination &&
                    DbFunctions.TruncateTime(fi.TravelDate) == DbFunctions.TruncateTime(flightSearchViewModel.TravelDate)

                select new FlightViewModel()
                {
                    FlightCode = f.FlightCode,
                    FlightName = f.FlightName,
                    Source = f.Source,
                    Destination = f.Destination,

                    ArrivalHrs = f.ArrivalTime.Hours,
                    ArrivalMin = f.ArrivalTime.Minutes,
                    ArrivalSec = f.ArrivalTime.Seconds,
                    ArrivalAmpm = f.ArrivalTime.Hours > 12 ? "PM" : "AM",

                    DepartureHrs = f.DepartureTime.Hours,
                    DepartureMin = f.DepartureTime.Minutes,
                    DepartureSec = f.DepartureTime.Seconds,
                    DepartureAmpm = f.DepartureTime.Hours > 12 ? "PM" : "AM",

                    ExecutiveSeats = fi.ExecutiveSeats,
                    ExecutiveFare = fi.ExecutiveFare,
                    BusinessSeats = fi.BusinessSeats,
                    BusinessFare = fi.BusinessFare,
                    EconomySeats = fi.EconomySeats,
                    EconomyFare = fi.EconomyFare,
                }
             ).ToList();

            return View(flightSearchViewModel);
        }

        //BookingController
        //Create booking
        public ActionResult AddBooking(int id)
        {
            ViewBag.GST_PERCENT = GST_PERCENT;
            var flightInventory = _db.FlightInventories.FirstOrDefault(f => f.FlightId == id);

            if (flightInventory == null)
                return HttpNotFound();

            BookingsViewModel boomkingsVM = new BookingsViewModel()
            {
                FlightId = id,
                TravelDate = flightInventory.TravelDate,
                ExecutiveFare = flightInventory.ExecutiveFare,
                BusinessFare = flightInventory.BusinessFare,
                EconomyFare = flightInventory.EconomyFare,
                PassengerList = new List<PassengerViewModel>()
                {
                    new PassengerViewModel() // This creates the FIRST passenger form automatically.
                }
            };

            return View(boomkingsVM);
        }

        [HttpPost]
        public ActionResult AddBooking(BookingsViewModel bookingVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(bookingVM);
                }

                // calculate total fare
                decimal totalFare = 0;

                var flightInventory = _db.FlightInventories.FirstOrDefault(
                    f => f.FlightId == bookingVM.FlightId && DbFunctions.TruncateTime(f.TravelDate) ==
                    DbFunctions.TruncateTime(bookingVM.TravelDate));


                foreach (var passenger in bookingVM.PassengerList)
                {

                    if (flightInventory == null)
                    {
                        ModelState.AddModelError("", "Flight inventory not found");
                        return View(bookingVM);
                    }

                    switch (passenger.Class)
                    {
                        case "ECONOMY":
                            totalFare += flightInventory.EconomyFare + flightInventory.EconomyFare * GST_PERCENT / 100;
                            break;
                        case "BUSINESS":
                            totalFare += flightInventory.BusinessFare + flightInventory.BusinessFare * GST_PERCENT / 100;
                            break;
                        case "EXECUTIVE":
                            totalFare += flightInventory.ExecutiveFare + flightInventory.ExecutiveFare * GST_PERCENT / 100;
                            break;
                    }
                }

                // create booking
                Booking newBooking = new Booking()
                {
                    FlightId = bookingVM.FlightId,
                    TravelDate = bookingVM.TravelDate,
                    TotalFare = totalFare,
                    BookingStatus = "BOOKED",
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _db.Bookings.Add(newBooking);
                _db.SaveChanges();

                // create passenger
                foreach (var passenger in bookingVM.PassengerList)
                {
                    PassengerDetail newPassenger = new PassengerDetail()
                    {
                        BookingId = newBooking.BookingId,
                        FullName = passenger.FullName,
                        Address = passenger.Address,
                        Phone = passenger.Phone,
                        Email = passenger.Email,
                        FlightId = bookingVM.FlightId,
                        TravelDate = bookingVM.TravelDate,
                        SeatNo = passenger.SeatNo,
                        Class = passenger.Class,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    _db.PassengerDetails.Add(newPassenger);
                }
                _db.SaveChanges();

                return RedirectToAction(nameof(SearchFlights));
            }
            //catch (Exception ex)
            //{
            //    return Content(ex.Message);
            //}

            catch (Exception ex)
            {
                Exception inner = ex;

                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }

                return Content(inner.Message);
            }
        }


        //View booking details
        //Booking history
        //Seat selection
        //Fare calculation
        //Booking status

        //PassengerController
        //Seat Selection
        //PaymentController
        //Booking Confirmation
        //CancellationController
    }
}