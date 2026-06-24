using System.ComponentModel.DataAnnotations;
using main.Models.Enums;

namespace main.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Identification is required.")]
        [StringLength(50, ErrorMessage = "Identification cannot exceed 50 characters.")]
        public string Identification { get; set; } = string.Empty;

        [Required(ErrorMessage = "Birth Date is required.")]
        [DataType(DataType.Date)]
        public System.DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        public UserRole Role { get; set; }
    }
}