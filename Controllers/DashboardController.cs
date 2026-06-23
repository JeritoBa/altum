using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using main.Models;
using main.Services.Interfaces;

namespace main.Controllers
{
    [Authorize(Roles = "Owner")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly UserManager<User> _userManager;

        public DashboardController(IDashboardService dashboardService, UserManager<User> userManager)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var user = await _userManager.GetUserAsync(User);
            
            main.Services.Common.ServiceResponse<main.ViewModels.Dashboard.DashboardMetricsViewModel> response;
            
            if (startDate.HasValue && endDate.HasValue)
            {
                response = await _dashboardService.GetByDatesAsync(user.Id, startDate.Value, endDate.Value);
            }
            else
            {
                response = await _dashboardService.GetAsync(user.Id);
            }

            return View(response.Data ?? new main.ViewModels.Dashboard.DashboardMetricsViewModel());
        }
    }
}
