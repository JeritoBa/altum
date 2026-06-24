using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using main.Models.Enums;

namespace main.Models
{
    public class User : IdentityUser<int>
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

        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
