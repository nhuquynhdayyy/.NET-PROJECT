// using System.ComponentModel.DataAnnotations;

// namespace QuanLyTrungTam.ViewModels 
// {
//     public class RegisterViewModel
//     {
//         [Required]
//         public string Username { get; set; }

//         [Required]
//         [DataType(DataType.Password)]
//         public string Password { get; set; }

//         [Required]
//         public string FullName { get; set; }

//         [EmailAddress]
//         public string Email { get; set; }

//         public string PhoneNumber { get; set; }

//         public DateTime DateOfBirth { get; set; }
//     }
// }
using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTrungTam.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
