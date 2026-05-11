using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VistaraAirLinesApp.Models.ViewModels
{
    public class FlightViewModel
    {
        [Key]
        public int FlightId { get; set; }

        [Required]
        [Display(Name = "Flight Code")]
        public string FlightCode { get; set; }
        
        [Required]
        [Display(Name = "Flight Name")]
        public string FlightName { get; set; }

        [Required]
        public string Source { get; set; }

        [Required] 
        public string Destination { get; set; }

        [Required]
        [Display(Name = "Travel Date")]
        [DataType(DataType.Date)]
        public DateTime TravelDate { get; set; }

        [Required]
        public int ArrivalHrs { get; set; }
        [Required]
        public int ArrivalMin { get; set; }
        [Required]
        public int ArrivalSec { get; set; }
        [Required]
        public string ArrivalAmpm { get; set; }

        [Required]
        public int DepartureHrs { get; set; }
        [Required]
        public int DepartureMin { get; set; }
        [Required]
        public int DepartureSec { get; set; }
        [Required]
        public string DepartureAmpm { get; set; }

        [Required]
        [Display(Name = "Executive Seats")]
        [Range(0,int.MaxValue)]
        public int ExecutiveSeats { get; set; }
        
        [Required]
        [Display(Name = "Executive Fare")]
        [Range(0,int.MaxValue)]
        public decimal ExecutiveFare { get; set; }

        [Required] 
        [Display(Name = "Business Seats")]
        [Range(0,int.MaxValue)]
        public int BusinessSeats { get; set; }

        [Required] 
        [Display(Name = "Business Fare")]
        [Range(0,int.MaxValue)]
        public decimal BusinessFare { get; set; }

        [Required] 
        [Display(Name = "Economy Seats")]
        [Range(0, int.MaxValue)]
        public int EconomySeats { get; set; }

        [Required] 
        [Display(Name = "Economy Fare")]
        [Range(0, int.MaxValue)]
        public decimal EconomyFare { get; set; }
    }
}