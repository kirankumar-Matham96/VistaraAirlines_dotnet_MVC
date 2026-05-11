using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VistaraAirLinesApp.Models.ViewModels
{
    public class PassengerViewModel
    {

        public int PassengerId { get; set; }

        [Required]
        [Display(Name = "Booking Id")]
        public int BookingId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Phone No")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Flight Id")]
        public int FlightId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Travel Date")]
        public DateTime TravelDate { get; set; }

        [Required]
        [Display(Name = "Seat No")]
        public string SeatNo { get; set; }

        [Required]
        public string Class { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
