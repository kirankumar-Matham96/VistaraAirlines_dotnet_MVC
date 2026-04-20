using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VistaraAirLinesApp.Models.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        [StringLength(50, MinimumLength = 3)]
        [Remote("CheckUserName", "User", AdditionalFields = "UserId", ErrorMessage = "This Username is taken. Please try another one!")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*_\-+/.]{8,16}$", ErrorMessage = "Password must be 8-16 characters and can include letters, numbers, and symbols")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string CPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Lastname { get; set; }

        [RegularExpression("MANAGER|EMPLOYEE", ErrorMessage = "Invalid Role")]
        public string Role { get; set; } = "EMPLOYEE";
    }
}