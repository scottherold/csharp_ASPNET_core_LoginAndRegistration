using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginAndRegistration.Models
{
    // Model used for registration. Sanatizes data for the controller to send to the DB.
    public class User
    {
        // Key sets the PK in the DB
        [Key]
        public int UserId { get; set; }
        
        // For First Name
        [Display(Name="First Name")]
        [Required(ErrorMessage="First name is required!")]
        [MinLength(2,ErrorMessage="First name must be at least two characters!")]
        public string FirstName { get; set; }

        // For Last Name
        [Display(Name="Last Name")]
        [Required(ErrorMessage="Last name is required!")]
        [MinLength(2,ErrorMessage="Last name must be at least two characters!")]
        public string LastName { get; set; }

        // For Email
        [Display(Name="Email")]
        [Required(ErrorMessage="Email is required!")]
        [EmailAddress(ErrorMessage="Email address must be in a valid format!")]
        public string Email { get; set; }

        // For Password
        [Display(Name="Password")]
        [Required(ErrorMessage="Password is required!")]
        [MinLength(8,ErrorMessage="Password must be at least 8 characters!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        // For Confirm
        // Not mapped due to being a confirm
        [NotMapped]
        [Compare("Password", ErrorMessage="Password confirmation must match Password field!")]
        [Required(ErrorMessage="Please confirm your password!")]
        public string Confirm { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}