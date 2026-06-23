using System;
using System.Threading.Tasks;
using main.Services.Common;
using main.ViewModels.Dashboard;

namespace main.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ServiceResponse<DashboardMetricsViewModel>> GetAsync(int ownerId);
        Task<ServiceResponse<DashboardMetricsViewModel>> GetByDatesAsync(int ownerId, DateTime startDate, DateTime endDate);
    }
}
