using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using main.Models;
using main.Services.Common;

namespace main.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<ServiceResponse<IEnumerable<Favorite>>> GetFavoritesAsync(int userId, int? propertyId = null, string location = null, DateTime? availableDate = null);
        Task<ServiceResponse<Favorite>> AddFavoriteAsync(int userId, int propertyId);
        Task<ServiceResponse<bool>> DeleteFavoriteAsync(int id, int userId);
    }
}
