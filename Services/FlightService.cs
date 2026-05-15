using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;
using VistaraAirLinesApp.Services.Interfaces;

namespace VistaraAirLinesApp.Services
{
    public class FlightService : IFlightService
    {
        private readonly VISTARA_DBEntities4 _db;

        public FlightService()
        {
            _db = new VISTARA_DBEntities4();
        }

        public (List<int> HHRange, List<int> MMRange, List<int> SSRange) GetTimeRanges()
        {
            return (
            Enumerable.Range(0, 13).ToList(),
            Enumerable.Range(0, 60).ToList(),
            Enumerable.Range(0, 60).ToList()
                );
        }

        public void AddFlight(FlightViewModel flight)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
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
                    _db.Flights.Add(flightData);

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
                    _db.FlightInventories.Add(flightInventoryData);

                    _db.SaveChanges();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public List<FlightViewModel> GetAllFlights()
        {
            try
            {
                var flights = (
                    from f in _db.Flights
                    join fi in _db.FlightInventories
                    on f.FlightId equals fi.FlightId
                    where f.IsDeleted == false
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

                        TravelDate = fi.TravelDate,
                        ExecutiveSeats = fi.ExecutiveSeats,
                        ExecutiveFare = fi.ExecutiveFare,
                        BusinessSeats = fi.BusinessSeats,
                        BusinessFare = fi.BusinessFare,
                        EconomySeats = fi.EconomySeats,
                        EconomyFare = fi.EconomyFare
                    }
                    ).ToList();
                return flights;
            }
            catch
            {
                throw;
            }
        }
        public Flight GetFlightById(int id)
        {
            try
            {
                var flight = _db.Flights.Find(id);
                if (flight == null)
                {
                    throw new Exception("Flight not found");
                }
                return flight;
            }
            catch
            {
                throw;
            }
        }

        public FlightInventory GetFlightInventoryById(int id)
        {
            try
            {
                var flightInventory = _db.FlightInventories.FirstOrDefault(f => f.FlightId == id);

                if (flightInventory == null)
                {
                    throw new Exception("Flight Inventory Not found");
                }

                return flightInventory;
            }
            catch
            {
                throw;
            }
        }

        public FlightViewModel GetFullFlightDetails(int id)
        {
            try
            {
                var flight = GetFlightById(id);
                var flightInventory = GetFlightInventoryById(id);

                if (flight == null)
                {
                    throw new Exception("Flight not found");
                }

                if (flightInventory == null)
                {
                    throw new Exception("FlightInventory not found");
                }

                var flightData = new FlightViewModel();

                flightData.FlightId = flight.FlightId;
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

                return flightData;
            }
            catch
            {
                throw;
            }
        }

        public void UpdateFlightDetails(FlightViewModel flight)
        {
            try
            {
                // update db here
                int arrivalHrs = flight.ArrivalAmpm == "AM" ? flight.ArrivalHrs : flight.ArrivalHrs + 12;
                int departureHrs = flight.DepartureAmpm == "AM" ? flight.DepartureHrs : flight.DepartureHrs + 12;

                int flightId = Convert.ToInt32(flight.FlightId);

                var flightToBeUpdated = GetFlightById(flightId);


                flightToBeUpdated.FlightCode = flight.FlightCode;
                flightToBeUpdated.FlightName = flight.FlightName;
                flightToBeUpdated.Source = flight.Source;
                flightToBeUpdated.Destination = flight.Destination;
                flightToBeUpdated.ArrivalTime = new TimeSpan(arrivalHrs, flight.ArrivalMin, flight.ArrivalSec);
                flightToBeUpdated.DepartureTime = new TimeSpan(departureHrs, flight.DepartureMin, flight.DepartureSec);
                flightToBeUpdated.IsDeleted = false;
                flightToBeUpdated.UpdatedAt = DateTime.Now;
                _db.SaveChanges();

                var flightInventoryToBeUpdated = _db.FlightInventories.FirstOrDefault(f => f.FlightId == flightId);

                flightInventoryToBeUpdated.TravelDate = flight.TravelDate;
                flightInventoryToBeUpdated.ExecutiveSeats = flight.ExecutiveSeats;
                flightInventoryToBeUpdated.ExecutiveFare = (Decimal)flight.ExecutiveFare;
                flightInventoryToBeUpdated.BusinessSeats = flight.BusinessSeats;
                flightInventoryToBeUpdated.BusinessFare = (Decimal)flight.BusinessFare;
                flightInventoryToBeUpdated.EconomySeats = flight.EconomySeats;
                flightInventoryToBeUpdated.EconomyFare = (Decimal)flight.EconomyFare;
                flightInventoryToBeUpdated.UpdatedAt = DateTime.Now;
                _db.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void DeleteFlight(int id)
        {
            try
            {
                var flight = GetFlightById(id);
                flight.IsDeleted = true;
                _db.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}