using System.ComponentModel.DataAnnotations;

namespace QuanLyTrungTam.ViewModels 
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
