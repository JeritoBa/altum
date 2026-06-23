using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using main.Data;
using main.Models;
using main.Models.Enums;
using main.Services.Interfaces;
using main.Services.Common;

namespace main.Services.Application
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<IEnumerable<Booking>>> GetBookingsAsync(int? propertyId = null, bool? activesOnly = null, string location = null, DateTime? date = null, int? guestId = null, int? ownerId = null)
        {
            var response = new ServiceResponse<IEnumerable<Booking>>();

            try
            {
                var query = _context.Bookings.Include(b => b.Property).AsQueryable();

                if (propertyId.HasValue)
                {
                    query = query.Where(b => b.PropertyId == propertyId.Value);
                }

                if (activesOnly.HasValue && activesOnly.Value)
                {
                    query = query.Where(b => b.State == BookingState.InProgress || b.State == BookingState.Pending);
                }

                if (!string.IsNullOrWhiteSpace(location))
                {
                    query = query.Where(b => b.Property.City.Contains(location) || b.Property.Country.Contains(location));
                }

                if (date.HasValue)
                {
                    query = query.Where(b => b.CheckInDate <= date.Value && b.CheckOutDate >= date.Value);
                }

                if (guestId.HasValue)
                {
                    query = query.Where(b => b.GuestId == guestId.Value);
                }

                if (ownerId.HasValue)
                {
                    query = query.Where(b => b.Property.OwnerId == ownerId.Value);
                }

                response.Data = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving bookings.";
                response.Errors.Add(ex.Message);
            }
            return response;
        }

        public async Task<ServiceResponse<Booking>> GetBookingByIdAsync(int id)
        {
            var response = new ServiceResponse<Booking>();
            var booking = await _context.Bookings.Include(b => b.Property).FirstOrDefaultAsync(b => b.Id == id);
            
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }

            response.Data = booking;
            return response;
        }

        public async Task<ServiceResponse<Booking>> CreateBookingAsync(Booking booking)
        {
            var response = new ServiceResponse<Booking>();

            try
            {
                var validation = await ValidateBookingAsync(booking);

                if (!validation.Success) return validation;

                booking.State = BookingState.Pending;
                booking.Paid = false;
                booking.CreatedAt = DateTime.UtcNow;

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                response.Data = booking;
                response.Message = "Booking created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error creating booking, {ex.Message}";
                response.Errors.Add(ex.Message);
            }
            return response;
        }

        public async Task<ServiceResponse<Booking>> UpdateBookingAsync(Booking booking)
        {
            var response = new ServiceResponse<Booking>();
            try
            {
                var existing = await _context.Bookings.Include(b => b.Property).FirstOrDefaultAsync(b => b.Id == booking.Id);

                if (existing == null)
                {
                    response.Success = false;
                    response.Message = "Booking not found.";
                    return response;
                }

                // If dates changed, validate again
                if (existing.CheckInDate != booking.CheckInDate || existing.CheckOutDate != booking.CheckOutDate)
                {
                    var tempBooking = new Booking 
                    { 
                        Id = booking.Id,
                        PropertyId = existing.PropertyId,
                        GuestId = existing.GuestId,
                        CheckInDate = booking.CheckInDate,
                        CheckOutDate = booking.CheckOutDate
                    };
                    var validation = await ValidateBookingAsync(tempBooking);
                    if (!validation.Success) return validation;
                }

                existing.CheckInDate = booking.CheckInDate;
                existing.CheckOutDate = booking.CheckOutDate;
                existing.GuestsCount = booking.GuestsCount;
                existing.State = booking.State;
                existing.Paid = booking.Paid;
                
                if (booking.State == BookingState.Cancelled && existing.CancelledAt == null)
                {
                    existing.CancelledAt = DateTime.UtcNow;
                }

                _context.Bookings.Update(existing);
                await _context.SaveChangesAsync();

                response.Data = existing;
                response.Message = "Booking updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error updating booking.";
                response.Errors.Add(ex.Message);
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteBookingAsync(int id, int userId, bool isOwner)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var booking = await _context.Bookings.Include(b => b.Property).FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                {
                    response.Success = false;
                    response.Message = "Booking not found.";
                    return response;
                }

                if (!isOwner && booking.GuestId != userId)
                {
                    response.Success = false;
                    response.Message = "Unauthorized to delete this booking.";
                    return response;
                }

                if (isOwner && booking.Property.OwnerId != userId)
                {
                    response.Success = false;
                    response.Message = "Unauthorized to delete this booking.";
                    return response;
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                response.Data = true;
                response.Message = "Booking deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error deleting booking.";
                response.Errors.Add(ex.Message);
            }
            return response;
        }

        private async Task<ServiceResponse<Booking>> ValidateBookingAsync(Booking booking)
        {
            var response = new ServiceResponse<Booking>();

            // Minimum 1 day
            if ((booking.CheckOutDate - booking.CheckInDate).TotalHours < 24)
            {
                response.Success = false;
                response.Message = "Booking must be at least 1 day long.";

                return response;
            }

            // Property must exist and be active
            var property = await _context.Properties.FindAsync(booking.PropertyId);
            if (property == null || !property.IsAvailable)
            {
                response.Success = false;
                response.Message = "Property is not available.";

                return response;
            }

            // Cannot conflict with other bookings (status pending or in_progress)
            // A conflict is when: ExistingStart < NewEnd AND ExistingEnd > NewStart
            var conflict = await _context.Bookings.AnyAsync(b => 
                b.PropertyId == booking.PropertyId && 
                b.Id != booking.Id &&
                (b.State == BookingState.Pending || b.State == BookingState.InProgress) &&
                b.CheckInDate < booking.CheckOutDate && 
                b.CheckOutDate > booking.CheckInDate);

            if (conflict)
            {
                response.Success = false;
                response.Message = "The property is already booked for the selected dates.";

                return response;
            }

            // Guest cannot have more than 3 pending unpaid bookings
            var pendingUnpaidCount = await _context.Bookings.CountAsync(b => 
                b.GuestId == booking.GuestId && 
                b.Id != booking.Id &&
                b.State == BookingState.Pending && 
                !b.Paid);

            if (pendingUnpaidCount >= 3)
            {
                response.Success = false;
                response.Message = "You cannot have more than 3 pending unpaid bookings.";

                return response;
            }

            response.Success = true;
            return response;
        }
    }
}
