using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using VistaraAirLinesApp.Models;
using VistaraAirLinesApp.Models.ViewModels;
using VistaraAirLinesApp.Services.Interfaces;

namespace VistaraAirLinesApp.Services
{
    public class BookingService : IBookingService,ICancellationService
    {
        private readonly VISTARA_DBEntities4 _db;
        private readonly decimal GST_PERCENT = 10;

        public BookingService()
        {
            _db = new VISTARA_DBEntities4();
        }

        public FlightSearchViewModel GetSearchFlightPageData()
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

            return flightSearchViewModel;
        }

        public FlightSearchViewModel SearchFlights(FlightSearchViewModel flightSearchViewModel)
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
                f.IsDeleted == false &&
                f.Source == flightSearchViewModel.Source &&
                f.Destination == flightSearchViewModel.Destination &&
                DbFunctions.TruncateTime(fi.TravelDate) == DbFunctions.TruncateTime(flightSearchViewModel.TravelDate)

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
            return flightSearchViewModel;
        }

        public BookingsViewModel GetBookingPageData(int flightId)
        {
            var inventory = _db.FlightInventories.FirstOrDefault(f => f.FlightId == flightId);

            if (inventory == null)
            {
                return null;
            }

            return new BookingsViewModel()
            {
                FlightId = flightId,
                TravelDate = inventory.TravelDate,
                ExecutiveFare = inventory.ExecutiveFare,
                BusinessFare = inventory.BusinessFare,
                EconomyFare = inventory.EconomyFare,
                PassengerList = new List<PassengerViewModel>()
                {
                    new PassengerViewModel()
                }
            };
        }

        private void ValidatePassengers(BookingsViewModel bookingVM)
        {
            if (bookingVM.PassengerList == null || !bookingVM.PassengerList.Any())
            {
                throw new Exception("At least one passenger is required.");
            }

            bookingVM.PassengerList = bookingVM.PassengerList
                .Where(p =>
                    !string.IsNullOrWhiteSpace(p.FullName) &&
                    !string.IsNullOrWhiteSpace(p.Email) &&
                    !string.IsNullOrWhiteSpace(p.Phone) &&
                    !string.IsNullOrWhiteSpace(p.SeatNo) &&
                    !string.IsNullOrWhiteSpace(p.Class))
                .ToList();

            if (!bookingVM.PassengerList.Any())
            {
                throw new Exception("Passenger details are required.");
            }
        }

        public bool ValidateSeatAvailability(BookingsViewModel bookingVM, out string message)
        {
            message = "";

            var inventory = _db.FlightInventories.FirstOrDefault(
                f => f.FlightId == bookingVM.FlightId &&
                DbFunctions.TruncateTime(f.TravelDate) == DbFunctions.TruncateTime(bookingVM.TravelDate));

            if (inventory == null)
            {
                message = "Flight inventory not found.";
                return false;
            }

            int economyCount = bookingVM.PassengerList.Count(p => p.Class == "ECONOMY");
            int businessCount = bookingVM.PassengerList.Count(p => p.Class == "BUSINESS");
            int executiveCount = bookingVM.PassengerList.Count(p => p.Class == "EXECUTIVE");

            if (economyCount > inventory.EconomySeats)
            {
                message = "Not enough Economy seats.";
                return false;
            }

            if (businessCount > inventory.BusinessSeats)
            {
                message = "Not enough Business seats.";
                return false;
            }

            if (executiveCount > inventory.ExecutiveSeats)
            {
                message = "Not enough Executive seats.";
                return false;
            }

            return true;
        }

        public decimal CalculateTotalFare(BookingsViewModel bookingVM)
        {
            decimal totalFare = 0;

            var inventory = _db.FlightInventories.FirstOrDefault(
                f => f.FlightId == bookingVM.FlightId &&
                DbFunctions.TruncateTime(f.TravelDate) ==
                DbFunctions.TruncateTime(bookingVM.TravelDate));

            foreach (var passenger in bookingVM.PassengerList)
            {
                switch (passenger.Class)
                {
                    case "ECONOMY":
                        totalFare += inventory.EconomyFare;
                        break;

                    case "BUSINESS":
                        totalFare += inventory.BusinessFare;
                        break;

                    case "EXECUTIVE":
                        totalFare += inventory.ExecutiveFare;
                        break;
                }
            }

            decimal gst = totalFare * GST_PERCENT / 100;

            return totalFare + gst;
        }

        public int CreateBooking(BookingsViewModel bookingVM)
        {
            Booking booking = new Booking()
            {
                FlightId = bookingVM.FlightId,
                TravelDate = bookingVM.TravelDate,
                TotalFare = bookingVM.TotalFare,
                BookingStatus = "BOOKED",
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _db.Bookings.Add(booking);

            _db.SaveChanges();

            foreach (var passenger in bookingVM.PassengerList)
            {
                PassengerDetail p = new PassengerDetail()
                {
                    BookingId = booking.BookingId,
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
                    UpdatedAt = DateTime.Now
                };

                _db.PassengerDetails.Add(p);
            }

            _db.SaveChanges();

            return booking.BookingId;
        }

        public void ConfirmBooking(BookingsViewModel bookingsViewModel)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    CreateBooking(bookingsViewModel);
                    ReduceSeats(bookingsViewModel);
                    _db.SaveChanges();
                    // COMMIT
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void ReduceSeats(BookingsViewModel bookingsViewModel)
        {
            // seat reduction after booking
            var flightInventory = _db.FlightInventories.FirstOrDefault(f => f.FlightId == bookingsViewModel.FlightId && DbFunctions.TruncateTime(f.TravelDate) == DbFunctions.TruncateTime(bookingsViewModel.TravelDate));

            foreach (var passenger in bookingsViewModel.PassengerList)
            {
                switch (passenger.Class)
                {
                    case "ECONOMY":
                        flightInventory.EconomySeats--;
                        break;
                    case "BUSINESS":
                        flightInventory.BusinessSeats--;
                        break;
                    case "EXECUTIVE":
                        flightInventory.ExecutiveSeats--;
                        break;
                }
            }
        }

        public void RestoreSeats(int bookingId)
        {
            // GET BOOKING
            var booking = _db.Bookings
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                throw new Exception("Booking not found.");
            }

            // GET INVENTORY
            var inventory = _db.FlightInventories
                .FirstOrDefault(fi =>
                    fi.FlightId == booking.FlightId &&
                    DbFunctions.TruncateTime(fi.TravelDate) ==
                    DbFunctions.TruncateTime(booking.TravelDate));

            if (inventory == null)
            {
                throw new Exception("Flight inventory not found.");
            }

            // GET PASSENGERS
            var passengers = _db.PassengerDetails
                .Where(p => p.BookingId == bookingId)
                .ToList();

            // RESTORE SEATS
            foreach (var passenger in passengers)
            {
                switch (passenger.Class)
                {
                    case "ECONOMY":
                        inventory.EconomySeats++;
                        break;

                    case "BUSINESS":
                        inventory.BusinessSeats++;
                        break;

                    case "EXECUTIVE":
                        inventory.ExecutiveSeats++;
                        break;
                }
            }
        }

        public TicketViewModel GetBookingDetails(int bookingId)
        {
            var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return null;
            }

            var flight = _db.Flights.FirstOrDefault(f => f.FlightId == booking.FlightId);

            TicketViewModel ticket = new TicketViewModel()
            {
                // BOOKING
                BookingId = booking.BookingId,
                BookingStatus = booking.BookingStatus,
                TravelDate = booking.TravelDate,
                TotalFare = booking.TotalFare,

                // FLIGHT
                FlightId = flight.FlightId,
                FlightCode = flight.FlightCode,
                FlightName = flight.FlightName,
                Source = flight.Source,
                Destination = flight.Destination,
                ArrivalTime = flight.ArrivalTime,
                DepartureTime = flight.DepartureTime,

                // PASSENGERS
                Passengers = booking.PassengerDetails
                    .Select(p => new PassengerViewModel()
                    {
                        PassengerId = p.PassengerId,
                        FullName = p.FullName,
                        Email = p.Email,
                        Phone = p.Phone,
                        SeatNo = p.SeatNo,
                        Class = p.Class
                    }).ToList()
            };

            return ticket;
        }

        public CancellationViewModel GetCancellationData(int bookingId)
        {
            var booking = _db.Bookings
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return null;
            }

            if (booking.BookingStatus == "CANCELLED")
            {
                throw new Exception("Booking already cancelled.");
            }

            var flight = _db.Flights
                .FirstOrDefault(f => f.FlightId == booking.FlightId);

            decimal deduction = booking.TotalFare * 10 / 100;
            decimal refund = booking.TotalFare - deduction;

            return new CancellationViewModel()
            {
                BookingId = booking.BookingId,
                FlightCode = flight.FlightCode,
                TravelDate = booking.TravelDate,
                OriginalFare = booking.TotalFare,
                Deduction = deduction,
                RefundAmount = refund,
                CancelDate = DateTime.Now
            };
        }

        public void CancelBooking(CancellationViewModel cancellationViewModel)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    // GET BOOKING
                    var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == cancellationViewModel.BookingId);

                    if (booking == null)
                    {
                        throw new Exception("Booking not found");
                    }

                    // UPDATE BOOKING STATUS
                    booking.BookingStatus = "CANCELLED";
                    booking.UpdatedAt = DateTime.Now;

                    // CREATE CANCELLATION RECORD
                    Cancellation cancellation = new Cancellation()
                    {
                        BookingId = cancellationViewModel.BookingId,
                        CancelDate = DateTime.Now,
                        RefundAmount = cancellationViewModel.RefundAmount,
                        Deduction = cancellationViewModel.Deduction,
                        Reason = cancellationViewModel.Reason,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _db.Cancellations.Add(cancellation);
                    
                    RestoreSeats(cancellationViewModel.BookingId);

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

        public List<BookingHistoryViewModel> GetBookingHistory(int id)
        {
            var bookings =
                (
                    from b in _db.Bookings
                    join f in _db.Flights
                    on b.FlightId equals f.FlightId
                    where b.IsDeleted == false && b.UserId == id

                    select new BookingHistoryViewModel()
                    {
                        BookingId = b.BookingId,
                        UserId = b.UserId,
                        FlightId = b.FlightId,
                        FlightCode = f.FlightCode,
                        FlightName = f.FlightName,
                        Source = f.Source,
                        Destination = f.Destination,
                        TravelDate = b.TravelDate,
                        TotalFare = b.TotalFare,
                        BookingStatus = b.BookingStatus,
                        PassengerCount = b.PassengerDetails.Count(),

                        Passengers = b.PassengerDetails.Select(p =>
                            new PassengerViewModel()
                            {
                                PassengerId = p.PassengerId,
                                FullName = p.FullName,
                                SeatNo = p.SeatNo,
                                Class = p.Class,
                                Phone = p.Phone,
                                Email = p.Email
                            }).ToList()
                    }
                ).ToList();

            return bookings;
        }

        public BookingsViewModel AddBooking(BookingsViewModel bookingViewModel)
        {
            ValidatePassengers(bookingViewModel);

            string message;

            bool isAvailable = ValidateSeatAvailability(bookingViewModel, out message);

            if (!isAvailable)
            {
                throw new Exception(message);
            }

            bookingViewModel.TotalFare = CalculateTotalFare(bookingViewModel);

            return bookingViewModel;
        }

    }
}