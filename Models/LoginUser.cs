using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginAndRegistration.Models
{
    // Model used for login
    public class LoginUser
    {
        // Email validators
        [Display(Name="Email")]
        [Required(ErrorMessage="Please enter your Email address!")]
        [EmailAddress(ErrorMessage="Email address must be in a valid format!")]
        public string Email { get; set; }

        // Password validators
        [Display(Name="Password")]
        [Required(ErrorMessage="Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}