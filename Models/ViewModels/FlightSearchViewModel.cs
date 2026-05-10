using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VistaraAirLinesApp.Models.ViewModels
{
    public class FlightSearchViewModel
    {
        [Required]
        public string Source { get; set; }

        [Required]
        public string Destination { get; set; }
        
        public DateTime TravelDate { get; set; }
        
        public List<FlightViewModel> FlightsList { get; set; }

        public IEnumerable<string> Sources { get; set; }
        public IEnumerable<string> Destinations { get; set; }
        public IEnumerable<DateTime> TravelDates { get; set; }
    }
}