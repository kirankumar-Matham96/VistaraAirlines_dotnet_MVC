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

                    return RedirectToAction("GetAllFlights");
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
                var flights = db.Flights.Where(f => f.IsDeleted == false).ToList();

                /* Use the below code once the app is read. Make use of partial views to reuse the code */
                //var flights = (
                //    from f in db.Flights
                //    join fi in db.FlightInventories
                //    on f.FlightId equals fi.FlightId
                //    where f.IsDeleted == false
                //    select new FlightViewModel()
                //    {
                //        FlightCode = f.FlightCode,
                //        FlightName = f.FlightName,
                //        Source = f.Source,
                //        Destination = f.Destination,

                //        ArrivalHrs = f.ArrivalTime.Hours,
                //        ArrivalMin = f.ArrivalTime.Minutes,
                //        ArrivalSec = f.ArrivalTime.Seconds,
                //        ArrivalAmpm = f.ArrivalTime.Hours > 12 ? "PM" : "AM",

                //        DepartureHrs = f.DepartureTime.Hours,
                //        DepartureMin = f.DepartureTime.Minutes,
                //        DepartureSec = f.DepartureTime.Seconds,
                //        DepartureAmpm = f.DepartureTime.Hours > 12 ? "PM" : "AM",

                //        TravelDate = fi.TravelDate,
                //        ExecutiveSeats = fi.ExecutiveSeats,
                //        ExecutiveFare = fi.ExecutiveFare,
                //        BusinessSeats = fi.BusinessSeats,
                //        BusinessFare = fi.BusinessFare,
                //        EconomySeats = fi.EconomySeats,
                //        EconomyFare = fi.EconomyFare
                //    }
                //    );

                return View(flights);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public void GetFullFlightDetails(int id)
        {
            var flight = db.Flights.Find(id);
            var flightInventory = db.FlightInventories.FirstOrDefault(f => f.FlightId == id);

            var flightData = new FlightViewModel();

            flightData.FlightCode = flight.FlightCode;
            flightData.FlightName = flight.FlightName;
            flightData.Source = flight.Source;
            flightData.Destination = flight.Destination;
            flightData.ArrivalHrs = flight.ArrivalTime.Hours > 12 ? flight.ArrivalTime.Hours - 12 : flight.ArrivalTime.Hours;
            flightData.ArrivalMin = flight.ArrivalTime.Minutes;
            flightData.ArrivalSec = flight.ArrivalTime.Seconds;
            flightData.ArrivalAmpm = flight.ArrivalTime.Hours > 12 ? "PM" : "AM";
            flightData.DepartureHrs = flight.DepartureTime.Hours > 12 ? flight.DepartureTime.Hours - 12 : flight.DepartureTime.Hours;
            flightData.DepartureMin = flight.DepartureTime.Minutes;
            flightData.DepartureSec = flight.DepartureTime.Seconds;
            flightData.DepartureAmpm = flight.DepartureTime.Hours > 12 ? "PM" : "AM";

            flightData.TravelDate = flightInventory.TravelDate;
            flightData.ExecutiveSeats = flightInventory.ExecutiveSeats;
            flightData.ExecutiveFare = flightInventory.ExecutiveFare;
            flightData.BusinessSeats = flightInventory.BusinessSeats;
            flightData.BusinessFare = flightInventory.BusinessFare;
            flightData.EconomySeats = flightInventory.EconomySeats;
            flightData.EconomyFare = flightInventory.EconomyFare;

            TempData["flightId"] = flight.FlightId;
            TempData.Keep("flightId");
            TempData["flightData"] = flightData;
            TempData.Keep("flightData");
        }

        public ActionResult GetFlightDetails(int id)
        {
            try
            {
                GetFullFlightDetails(id);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // continue with the update and delete operations
        public ActionResult UpdateFlightDetails(int id)
        {
            try
            {
                GetTimeRanges();
                GetFullFlightDetails(id);
                return View(TempData["flightData"]);
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
                    // update db here
                    int arrivalHrs = flight.ArrivalAmpm == "AM" ? flight.ArrivalHrs : flight.ArrivalHrs + 12;
                    int departureHrs = flight.DepartureAmpm == "AM" ? flight.DepartureHrs : flight.DepartureHrs + 12;

                    int flightId = Convert.ToInt32(TempData["flightId"].ToString());

                    var flightToBeUpdated = db.Flights.Find(flightId);

                    flightToBeUpdated.FlightCode = flight.FlightCode;
                    flightToBeUpdated.FlightName = flight.FlightName;
                    flightToBeUpdated.Source = flight.Source;
                    flightToBeUpdated.Destination = flight.Destination;
                    flightToBeUpdated.ArrivalTime = new TimeSpan(arrivalHrs, flight.ArrivalMin, flight.ArrivalSec);
                    flightToBeUpdated.DepartureTime = new TimeSpan(departureHrs, flight.DepartureMin, flight.DepartureSec);
                    flightToBeUpdated.IsDeleted = false;
                    flightToBeUpdated.UpdatedAt = DateTime.Now;
                    db.SaveChanges();

                    var flightInventoryToBeUpdated = db.FlightInventories.FirstOrDefault(f => f.FlightId == flightId);

                    flightInventoryToBeUpdated.TravelDate = flight.TravelDate;
                    flightInventoryToBeUpdated.ExecutiveSeats = flight.ExecutiveSeats;
                    flightInventoryToBeUpdated.ExecutiveFare = (Decimal)flight.ExecutiveFare;
                    flightInventoryToBeUpdated.BusinessSeats = flight.BusinessSeats;
                    flightInventoryToBeUpdated.BusinessFare = (Decimal)flight.BusinessFare;
                    flightInventoryToBeUpdated.EconomySeats = flight.EconomySeats;
                    flightInventoryToBeUpdated.EconomyFare = (Decimal)flight.EconomyFare;
                    flightInventoryToBeUpdated.UpdatedAt = DateTime.Now;
                    db.SaveChanges();

                    return RedirectToAction("GetAllFlights");
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
                var flight = db.Flights.Find(id);
                flight.IsDeleted = true;
                db.SaveChanges();

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