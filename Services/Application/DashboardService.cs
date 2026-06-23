using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using main.Data;
using main.Models.Enums;
using main.Services.Interfaces;
using main.Services.Common;
using main.ViewModels.Dashboard;

namespace main.Services.Application
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<DashboardMetricsViewModel>> GetAsync(int ownerId)
        {
            var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return await GetByDatesAsync(ownerId, startDate, endDate);
        }

        public async Task<ServiceResponse<DashboardMetricsViewModel>> GetByDatesAsync(int ownerId, DateTime startDate, DateTime endDate)
        {
            var response = new ServiceResponse<DashboardMetricsViewModel>();
            try
            {
                var properties = await _context.Properties
                    .Where(p => p.OwnerId == ownerId)
                    .ToListAsync();

                if (!properties.Any())
                {
                    response.Data = new DashboardMetricsViewModel { StartDate = startDate, EndDate = endDate };
                    response.Message = "No properties found.";
                    return response;
                }

                var propertyIds = properties.Select(p => p.Id).ToList();

                var bookings = await _context.Bookings
                    .Where(b => propertyIds.Contains(b.PropertyId) && 
                                b.State != BookingState.Cancelled && 
                                b.CheckInDate <= endDate && b.CheckOutDate >= startDate)
                    .ToListAsync();

                var dashboardMetrics = new DashboardMetricsViewModel
                {
                    StartDate = startDate,
                    EndDate = endDate
                };

                double totalPossibleDays = (endDate - startDate).TotalDays + 1;
                if (totalPossibleDays <= 0) totalPossibleDays = 1;
                
                int totalProperties = properties.Count;
                double totalOccupiedDays = 0;

                foreach (var prop in properties)
                {
                    var propBookings = bookings.Where(b => b.PropertyId == prop.Id).ToList();
                    
                    decimal propIncome = 0;
                    double propOccupiedDays = 0;

                    foreach (var booking in propBookings)
                    {
                        var overlapStart = booking.CheckInDate > startDate ? booking.CheckInDate : startDate;
                        var overlapEnd = booking.CheckOutDate < endDate ? booking.CheckOutDate : endDate;
                        var overlapDays = (overlapEnd - overlapStart).TotalDays;
                        
                        if (overlapDays > 0)
                        {
                            propOccupiedDays += overlapDays;
                            
                            var totalBookingDays = (booking.CheckOutDate - booking.CheckInDate).TotalDays;
                            if (totalBookingDays > 0)
                            {
                                var dailyRate = booking.TotalPrice / totalBookingDays;
                                propIncome += (decimal)(dailyRate * overlapDays);
                            }
                        }
                    }

                    var propMetrics = new PropertyMetricsViewModel
                    {
                        PropertyId = prop.Id,
                        PropertyName = prop.Name,
                        Income = propIncome,
                        BookingsCount = propBookings.Count,
                        OccupancyRate = totalPossibleDays > 0 ? (propOccupiedDays / totalPossibleDays) * 100 : 0
                    };

                    dashboardMetrics.PropertyMetrics.Add(propMetrics);
                    dashboardMetrics.TotalIncome += propIncome;
                    dashboardMetrics.TotalBookings += propBookings.Count;
                    totalOccupiedDays += propOccupiedDays;
                }

                dashboardMetrics.OccupancyRate = (totalPossibleDays * totalProperties) > 0 
                    ? (totalOccupiedDays / (totalPossibleDays * totalProperties)) * 100 
                    : 0;

                response.Data = dashboardMetrics;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error generating dashboard metrics: {ex.Message}";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
