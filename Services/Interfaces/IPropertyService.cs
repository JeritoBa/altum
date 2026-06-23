using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using main.Services.Common;
using main.Models;

namespace main.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<ServiceResponse<IEnumerable<Property>>> GetPropertiesAsync(string location = null, string name = null, DateTime? availableDate = null, int? ownerId = null);
        Task<ServiceResponse<Property>> GetPropertyByIdAsync(int id);
        Task<ServiceResponse<Property>> CreatePropertyAsync(Property property);
        Task<ServiceResponse<Property>> UpdatePropertyAsync(Property property);
        Task<ServiceResponse<bool>> DeletePropertyAsync(int id, int ownerId);
    }
}
