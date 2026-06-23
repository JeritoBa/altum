using System;
using System.Collections.Generic;

namespace main.ViewModels.Dashboard
{
    public class DashboardMetricsViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalIncome { get; set; }
        public double OccupancyRate { get; set; }
        public int TotalBookings { get; set; }
        public List<PropertyMetricsViewModel> PropertyMetrics { get; set; } = new List<PropertyMetricsViewModel>();
    }
}
