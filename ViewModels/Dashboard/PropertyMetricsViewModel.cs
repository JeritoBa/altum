namespace main.ViewModels.Dashboard
{
    public class PropertyMetricsViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public double OccupancyRate { get; set; }
        public int BookingsCount { get; set; }
    }
}
