using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VistaraAirLinesApp.Models.ViewModels
{
    public class CancellationViewModel
    {
        public int CancellationId { get; set; }

        public int BookingId { get; set; }

        public DateTime CancelDate { get; set; }

        public decimal RefundAmount { get; set; }

        public decimal Deduction { get; set; }

        [Required]
        public string Reason { get; set; }

        //-----------------------------------
        // DISPLAY PURPOSE
        //-----------------------------------
        public decimal OriginalFare { get; set; }

        public string FlightCode { get; set; }

        public DateTime TravelDate { get; set; }
    }
}