using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using main.Models;
using main.Services.Interfaces;
using main.ViewModels.Property;

namespace main.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly UserManager<User> _userManager;

        public PropertyController(IPropertyService propertyService, UserManager<User> userManager)
        {
            _propertyService = propertyService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string location, string name, DateTime? availableDate, bool myPropertiesOnly = false)
        {
            int? ownerIdFilter = null;

            // Checking if the user is an Owner and wants to see only their properties
            if (myPropertiesOnly && User.IsInRole("Owner"))
            {
                var user = await _userManager.GetUserAsync(User);
                ownerIdFilter = user?.Id;
            }

            var response = await _propertyService.GetPropertiesAsync(location, name, availableDate, ownerIdFilter);
            
            ViewBag.Location = location;
            ViewBag.Name = name;
            ViewBag.AvailableDate = availableDate?.ToString("yyyy-MM-dd");
            ViewBag.MyPropertiesOnly = myPropertiesOnly;

            // If success is false, we could show an error, but for Index we just return an empty list or Data.
            return View(response.Data ?? new System.Collections.Generic.List<Property>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _propertyService.GetPropertyByIdAsync(id);
            
            if (!response.Success || response.Data == null)
            {
                return NotFound();
            }

            return View(response.Data);
        }

        [Authorize(Roles = "Owner")]
        public IActionResult Create()
        {
            return View(new PropertyFormViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Create(PropertyFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            var property = new Property
            {
                Name = model.Name,
                Description = model.Description,
                Address = model.Address,
                City = model.City,
                Country = model.Country,
                MaxGuests = model.MaxGuests,
                Bedrooms = model.Bedrooms,
                Bathrooms = model.Bathrooms,
                MainImageUrl = model.MainImageUrl,
                IsAvailable = model.IsAvailable,
                OwnerId = user.Id
            };

            var response = await _propertyService.CreatePropertyAsync(property);

            if (!response.Success)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "Error creating property");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _propertyService.GetPropertyByIdAsync(id);
            if (!response.Success || response.Data == null) return NotFound();

            var property = response.Data;
            var user = await _userManager.GetUserAsync(User);
            if (property.OwnerId != user.Id) return Unauthorized();

            var model = new PropertyFormViewModel
            {
                Id = property.Id,
                Name = property.Name,
                Description = property.Description,
                Address = property.Address,
                City = property.City,
                Country = property.Country,
                MaxGuests = property.MaxGuests,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                MainImageUrl = property.MainImageUrl,
                IsAvailable = property.IsAvailable
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(PropertyFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            
            var property = new Property
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Address = model.Address,
                City = model.City,
                Country = model.Country,
                MaxGuests = model.MaxGuests,
                Bedrooms = model.Bedrooms,
                Bathrooms = model.Bathrooms,
                MainImageUrl = model.MainImageUrl,
                IsAvailable = model.IsAvailable,
                OwnerId = user.Id
            };

            var response = await _propertyService.UpdatePropertyAsync(property);

            if (!response.Success)
            {
                // Here the controller knows exactly how to react to the error
                ModelState.AddModelError(string.Empty, response.Message ?? "Error updating property");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var response = await _propertyService.DeletePropertyAsync(id, user.Id);
            
            if (!response.Success)
            {
                // Could return a specific view or flash message
                TempData["ErrorMessage"] = response.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}