using System;
using System.Collections.Generic;
using main.Models.Enums;

namespace main.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int GuestsCount { get; set; }
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
