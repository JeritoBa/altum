using System;
using System.Threading.Tasks;
using main.Services.Common;

namespace main.Services.Interfaces
{
    public interface IReportsService
    {
        Task<ServiceResponse<byte[]>> GenerateBookingsReportAsync(int ownerId, int? propertyId, DateTime? startDate, DateTime? endDate);
    }
}
