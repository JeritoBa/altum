using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using main.Models;
using main.Services.Interfaces;

namespace main.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly IFavoriteService _favoriteService;
        private readonly UserManager<User> _userManager;

        public FavoriteController(IFavoriteService favoriteService, UserManager<User> userManager)
        {
            _favoriteService = favoriteService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? propertyId, string location, DateTime? availableDate)
        {
            var user = await _userManager.GetUserAsync(User);
            var response = await _favoriteService.GetFavoritesAsync(user.Id, propertyId, location, availableDate);

            ViewBag.PropertyId = propertyId;
            ViewBag.Location = location;
            ViewBag.AvailableDate = availableDate?.ToString("yyyy-MM-dd");

            return View(response.Data ?? new System.Collections.Generic.List<Favorite>());
        }

        [HttpPost]
        public async Task<IActionResult> Add(int propertyId, string returnUrl)
        {
            var user = await _userManager.GetUserAsync(User);
            var response = await _favoriteService.AddFavoriteAsync(user.Id, propertyId);
            
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Property");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            await _favoriteService.DeleteFavoriteAsync(id, user.Id);
            
            return RedirectToAction(nameof(Index));
        }
    }
}
