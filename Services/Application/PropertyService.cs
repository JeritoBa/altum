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
    public class PropertyService : IPropertyService
    {
        private readonly AppDbContext _context;

        public PropertyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<IEnumerable<Property>>> GetPropertiesAsync(string location = null, string name = null, DateTime? availableDate = null, int? ownerId = null)
        {
            var response = new ServiceResponse<IEnumerable<Property>>();
            
            try
            {
                var query = _context.Properties.AsQueryable();

                if (ownerId.HasValue)
                {
                    query = query.Where(p => p.OwnerId == ownerId.Value);
                }

                if (!string.IsNullOrWhiteSpace(location))
                {
                    query = query.Where(p => p.City.Contains(location) || p.Country.Contains(location));
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(p => p.Name.Contains(name));
                }

                if (availableDate.HasValue)
                {
                    query = query.Where(p => p.IsAvailable);
                }

                response.Data = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving properties.";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse<Property>> GetPropertyByIdAsync(int id)
        {
            var response = new ServiceResponse<Property>();
            var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == id);
            
            if (property == null)
            {
                response.Success = false;
                response.Message = "Property not found.";
                return response;
            }

            response.Data = property;
            return response;
        }

        public async Task<ServiceResponse<Property>> CreatePropertyAsync(Property property)
        {
            var response = new ServiceResponse<Property>();
            
            try
            {
                _context.Properties.Add(property);
                await _context.SaveChangesAsync();
                response.Data = property;
                response.Message = "Property created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error creating property.";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse<Property>> UpdatePropertyAsync(Property property)
        {
            var response = new ServiceResponse<Property>();
            
            try
            {
                var existing = await _context.Properties.FindAsync(property.Id);

                if (existing == null)
                {
                    response.Success = false;
                    response.Message = "Property not found.";
                    return response;
                }

                if (existing.OwnerId != property.OwnerId)
                {
                    response.Success = false;
                    response.Message = "Unauthorized to update this property.";
                    return response;
                }

                existing.Name = property.Name;
                existing.Description = property.Description;
                existing.Address = property.Address;
                existing.City = property.City;
                existing.Country = property.Country;
                existing.MaxGuests = property.MaxGuests;
                existing.Bedrooms = property.Bedrooms;
                existing.Bathrooms = property.Bathrooms;
                existing.MainImageUrl = property.MainImageUrl;
                existing.IsAvailable = property.IsAvailable;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.Properties.Update(existing);
                await _context.SaveChangesAsync();

                response.Data = existing;
                response.Message = "Property updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error updating property.";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeletePropertyAsync(int id, int ownerId)
        {
            var response = new ServiceResponse<bool>();
            
            try
            {
                var property = await _context.Properties.FindAsync(id);

                if (property == null)
                {
                    response.Success = false;
                    response.Message = "Property not found.";
                    return response;
                }

                if (property.OwnerId != ownerId)
                {
                    response.Success = false;
                    response.Message = "Unauthorized to delete this property.";
                    return response;
                }

                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();

                response.Data = true;
                response.Message = "Property deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error deleting property.";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
