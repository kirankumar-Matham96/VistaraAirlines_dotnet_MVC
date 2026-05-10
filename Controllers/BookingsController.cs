using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using VistaraAirLinesApp.Controllers;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;

namespace VistaraAirLinesApp.Controllers
{
    public class BookingsController : Controller
    {
        VISTARA_DBEntities4 _db = new VISTARA_DBEntities4();

        //Search available flights(get flights with filter)
        public ActionResult SearchFlights()
        {
            FlightSearchViewModel flightSearchViewModel = new FlightSearchViewModel();

            flightSearchViewModel.Sources = _db.Flights.Select(f => f.Source).Distinct().ToList();
            flightSearchViewModel.Destinations = _db.Flights.Select(f => f.Destination).Distinct().ToList();
            flightSearchViewModel.TravelDates = _db.FlightInventories.Select(f => f.TravelDate).Distinct().ToList();


            return View(flightSearchViewModel);
        }

        [HttpPost]
        public ActionResult SearchFlights(FlightSearchViewModel flightSearchModel)
        {

            flightSearchModel.Sources = _db.Flights.Select(f => f.Source).Distinct().ToList();
            flightSearchModel.Destinations = _db.Flights.Select(f => f.Destination).Distinct().ToList();
            flightSearchModel.TravelDates = _db.FlightInventories.Select(f => f.TravelDate).Distinct().ToList();
                    //fi.TravelDate == flightSearchModel.TravelDate



            flightSearchModel.FlightsList =
            (
                from f in _db.Flights
                join fi in _db.FlightInventories
                on f.FlightId equals fi.FlightId
                where
                    f.Source == flightSearchModel.Source &&
                    f.Destination == flightSearchModel.Destination &&
                    DbFunctions.TruncateTime(fi.TravelDate) == DbFunctions.TruncateTime(flightSearchModel.TravelDate)

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

            return View(flightSearchModel);
        }

        //Create booking
        public ActionResult AddBooking(int id)
        {
            return View();
        }


        //View booking details
        //Booking history
        //Seat selection
        //Fare calculation
        //Booking status
    }
}