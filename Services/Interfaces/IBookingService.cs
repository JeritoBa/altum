using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using main.Models;
using main.Services.Common;

namespace main.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ServiceResponse<IEnumerable<Booking>>> GetBookingsAsync(int? propertyId = null, bool? activesOnly = null, string location = null, DateTime? date = null, int? guestId = null, int? ownerId = null);
        Task<ServiceResponse<Booking>> GetBookingByIdAsync(int id);
        Task<ServiceResponse<Booking>> CreateBookingAsync(Booking booking);
        Task<ServiceResponse<Booking>> UpdateBookingAsync(Booking booking);
        Task<ServiceResponse<bool>> DeleteBookingAsync(int id, int userId, bool isOwner);
    }
}
