using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using main.Data;
using main.Services.Interfaces;
using main.Services.Common;

namespace main.Services.Infrastructure
{
    public class ReportsService : IReportsService
    {
        private readonly AppDbContext _context;

        public ReportsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<byte[]>> GenerateBookingsReportAsync(int ownerId, int? propertyId, DateTime? startDate, DateTime? endDate)
        {
            var response = new ServiceResponse<byte[]>();
            try
            {
                // Constructing the needed query
                var query = _context.Bookings
                    .Include(b => b.Property)
                    .Include(b => b.Guest)
                    .Where(b => b.Property.OwnerId == ownerId);

                if (propertyId.HasValue)
                {
                    query = query.Where(b => b.PropertyId == propertyId.Value);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(b => b.CheckInDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(b => b.CheckInDate <= endDate.Value);
                }

                // Doing the query
                var bookings = await query.OrderBy(b => b.CheckInDate).ToListAsync();

                // Configuring EPPlus
                ExcelPackage.License.SetNonCommercialPersonal("Jeronimo");

                // Creating excel file
                using var package = new ExcelPackage();

                // Creating sheet named bookings report
                var worksheet = package.Workbook.Worksheets.Add("Bookings Report");

                // Headers
                worksheet.Cells[1, 1].Value = "Property Name";
                worksheet.Cells[1, 2].Value = "Check-in Date";
                worksheet.Cells[1, 3].Value = "Check-out Date";
                worksheet.Cells[1, 4].Value = "Total Price ($)";
                worksheet.Cells[1, 5].Value = "Guest Name";
                worksheet.Cells[1, 6].Value = "Guest Email";
                worksheet.Cells[1, 7].Value = "Booking Status";

                // Styling headers
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Adding data rows
                int row = 2;
                foreach (var b in bookings)
                {
                    worksheet.Cells[row, 1].Value = b.Property.Name;
                    worksheet.Cells[row, 2].Value = b.CheckInDate.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[row, 3].Value = b.CheckOutDate.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[row, 4].Value = b.TotalPrice;
                    worksheet.Cells[row, 5].Value = b.Guest?.FullName ?? "N/A";
                    worksheet.Cells[row, 6].Value = b.Guest?.Email ?? "N/A";
                    worksheet.Cells[row, 7].Value = b.State.ToString();
                    row++;
                }

                // Auto fit columns
                worksheet.Cells.AutoFitColumns();

                // Saving and returning file
                response.Data = package.GetAsByteArray();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error generating report: {ex.Message}";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
