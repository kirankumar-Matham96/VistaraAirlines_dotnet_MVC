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

        public FlightController()
        {
            db = new VISTARA_DBEntities4();
        }

        [NonAction]
        public void GetTimeRanges()
        {
            TempData["HHRange"] = Enumerable.Range(0, 13).ToList();
            TempData["MMRange"] = Enumerable.Range(0, 60).ToList();
            TempData["SSRange"] = Enumerable.Range(0, 60).ToList();
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
                    // create object to suit the Flight table columns

                    int arrivalHrs = flight.ArrivalAmpm == "AM" ? flight.ArrivalHrs : flight.ArrivalHrs + 12;
                    int departureHrs = flight.DepartureAmpm == "AM" ? flight.DepartureHrs : flight.DepartureHrs + 12;

                    var flightData = new Flight()
                    {
                        FlightCode = flight.FlightCode,
                        FlightName = flight.FlightName,
                        Source = flight.Source,
                        Destination = flight.Destination,
                        ArrivalTime = new TimeSpan(arrivalHrs, flight.ArrivalMin, flight.ArrivalSec),
                        DepartureTime = new TimeSpan(departureHrs, flight.DepartureMin, flight.DepartureSec),
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    // Save the flight data first and get the id from the db.
                    db.Flights.Add(flightData);
                    db.SaveChanges();

                    var flightInventoryData = new FlightInventory()
                    {
                        FlightId = flightData.FlightId, // this FligtId is auto populated to the object by EF
                        TravelDate = flight.TravelDate,
                        ExecutiveSeats = flight.ExecutiveSeats,
                        ExecutiveFare = (Decimal)flight.ExecutiveFare,
                        BusinessSeats = flight.BusinessSeats,
                        BusinessFare = (Decimal)flight.BusinessFare,
                        EconomySeats = flight.EconomySeats,
                        EconomyFare = (Decimal)flight.EconomyFare,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    // Insert the details to the DB
                    db.FlightInventories.Add(flightInventoryData);
                    db.SaveChanges();

                    // Use transaction here to be safe side****
                }

                GetTimeRanges();
                return View(flight);
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", "Something went wrong while adding flight");
                ModelState.AddModelError("", ex.Message);
                return View(flight);
            }
        }
    }
}