using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using main.Models.Enums;

namespace main.Models
{
    public class Booking
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Check-in date is required.")]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required.")]
        public DateTime CheckOutDate { get; set; }

        [Range(1, 1000, ErrorMessage = "Guests count must be between 1 and 1000.")]
        public int GuestsCount { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Total price must be a positive value.")]
        public float TotalPrice { get; set; }
        public BookingState State { get; set; }
        public bool Paid { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        // Foreign Keys
        public int GuestId { get; set; }
        public int PropertyId { get; set; }

        // Navigation Properties
        public User Guest { get; set; } = null!;
        public Property Property { get; set; } = null!;
    }
}
