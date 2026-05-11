using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VistaraAirLinesApp.Models.ViewModels
{
    public class BookingsViewModel
    {
        public BookingsViewModel()
        {
            PassengerList = new List<PassengerViewModel>();
        }

        [Key]
        public int BookingId { get; set; }

        [Required]
        public int FlightId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TravelDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal ExecutiveFare { get; set; }

        [DataType(DataType.Currency)]
        public decimal BusinessFare { get; set; }

        [DataType(DataType.Currency)]
        public decimal EconomyFare { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalFare { get; set; }

        //[Required]
        public List<PassengerViewModel> PassengerList { get; set; }

        public string BookingStatus { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}