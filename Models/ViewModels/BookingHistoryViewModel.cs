using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VistaraAirLinesApp.Models.ViewModels
{
    public class BookingHistoryViewModel
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }

        public int FlightId { get; set; }

        public string FlightCode { get; set; }

        public string FlightName { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

        public DateTime TravelDate { get; set; }

        public decimal TotalFare { get; set; }

        public string BookingStatus { get; set; }

        public int PassengerCount { get; set; }

        public List<PassengerViewModel> Passengers { get; set; }
    }
}