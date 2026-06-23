using System;
using System.ComponentModel.DataAnnotations;

namespace main.ViewModels.Booking
{
    public class BookingFormViewModel
    {
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        public string? PropertyName { get; set; }

        [Required(ErrorMessage = "Check-in date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime CheckInDate { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "Check-out date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime CheckOutDate { get; set; } = DateTime.Now.AddDays(2);

        [Required(ErrorMessage = "Number of guests is required.")]
        [Range(1, 100, ErrorMessage = "Must have at least 1 guest.")]
        public int GuestsCount { get; set; }
    }
}
