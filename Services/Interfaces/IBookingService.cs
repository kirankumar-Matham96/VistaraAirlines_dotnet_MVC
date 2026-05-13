using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VistaraAirLinesApp.Models.ViewModels;

namespace VistaraAirLinesApp.Services.Interfaces
{
    internal interface IBookingService
    {
        FlightSearchViewModel SearchFlights(FlightSearchViewModel flightSearchViewModel);
        FlightSearchViewModel GetSearchFlightPageData();
        BookingsViewModel GetBookingPageData(int flightId);
        List<BookingHistoryViewModel> GetBookingHistory();
        BookingsViewModel AddBooking(BookingsViewModel bookingViewModel);
        decimal CalculateTotalFare(BookingsViewModel bookingVM);
        bool ValidateSeatAvailability(BookingsViewModel bookingVM, out string message);
        int CreateBooking(BookingsViewModel bookingVM);
        void ConfirmBooking(BookingsViewModel bookingsViewModel);
        TicketViewModel GetBookingDetails(int bookingId);
        void RestoreSeats(int bookingId);
        void CancelBooking(CancellationViewModel cancellationViewModel);
        CancellationViewModel GetCancellationData(int bookingId);
    }
}
