using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using main.Data;
using main.Models;
using main.Services.Interfaces;
using main.Services.Common;

namespace main.Services.Application
{
    public class FavoriteService : IFavoriteService
    {
        private readonly AppDbContext _context;

        public FavoriteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<IEnumerable<Favorite>>> GetFavoritesAsync(int userId, int? propertyId = null, string location = null, DateTime? availableDate = null)
        {
            var response = new ServiceResponse<IEnumerable<Favorite>>();
            try
            {
                var query = _context.Favorites
                    .Include(f => f.Property)
                    .Where(f => f.UserId == userId)
                    .AsQueryable();

                if (propertyId.HasValue)
                {
                    query = query.Where(f => f.PropertyId == propertyId.Value);
                }

                if (!string.IsNullOrWhiteSpace(location))
                {
                    query = query.Where(f => f.Property.City.Contains(location) || f.Property.Country.Contains(location));
                }

                if (availableDate.HasValue)
                {
                    // Filter: Property is active AND does not have a conflicting booking for the specific date
                    query = query.Where(f => f.Property.IsAvailable);
                    
                    var requestedDate = availableDate.Value.Date;
                    var conflictedPropertyIds = _context.Bookings
                        .Where(b => (b.State == main.Models.Enums.BookingState.Pending || b.State == main.Models.Enums.BookingState.InProgress)
                                    && b.CheckInDate.Date <= requestedDate && b.CheckOutDate.Date > requestedDate)
                        .Select(b => b.PropertyId)
                        .Distinct();

                    query = query.Where(f => !conflictedPropertyIds.Contains(f.PropertyId));
                }

                response.Data = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving favorites: {ex.Message}";
                response.Errors.Add(ex.Message);
            }
            return response;
        }

        public async Task<ServiceResponse<Favorite>> AddFavoriteAsync(int userId, int propertyId)
        {
            var response = new ServiceResponse<Favorite>();
            try
            {
                var existing = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.PropertyId == propertyId);
                if (existing != null)
                {
                    response.Success = false;
                    response.Message = "Property is already in favorites.";
                    return response;
                }

                var property = await _context.Properties.FindAsync(propertyId);
                if (property == null)
                {
                    response.Success = false;
                    response.Message = "Property not found.";
                    return response;
                }

                var favorite = new Favorite
                {
                    UserId = userId,
                    PropertyId = propertyId
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();

                response.Data = favorite;
                response.Message = "Added to favorites.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error adding favorite: {ex.Message}";
                response.Errors.Add(ex.Message);
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteFavoriteAsync(int id, int userId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Id == id);
                if (favorite == null)
                {
                    response.Success = false;
                    response.Message = "Favorite not found.";
                    return response;
                }

                if (favorite.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Unauthorized.";
                    return response;
                }

                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                
                response.Data = true;
                response.Message = "Removed from favorites.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error deleting favorite: {ex.Message}";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
