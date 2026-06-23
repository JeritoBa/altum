using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using main.Models;
using main.Models.Enums;
using main.Services.Interfaces;
using main.ViewModels.Booking;

namespace main.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IPropertyService _propertyService;
        private readonly UserManager<User> _userManager;

        public BookingController(IBookingService bookingService, IPropertyService propertyService, UserManager<User> userManager)
        {
            _bookingService = bookingService;
            _propertyService = propertyService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? propertyId, bool? activesOnly, string location, DateTime? date)
        {
            var user = await _userManager.GetUserAsync(User);
            var isOwner = User.IsInRole("Owner");

            int? guestIdFilter = isOwner ? null : user.Id;
            int? ownerIdFilter = isOwner ? user.Id : null;

            var response = await _bookingService.GetBookingsAsync(propertyId, activesOnly, location, date, guestIdFilter, ownerIdFilter);

            ViewBag.PropertyId = propertyId;
            ViewBag.ActivesOnly = activesOnly;
            ViewBag.Location = location;
            ViewBag.Date = date?.ToString("yyyy-MM-dd");

            return View(response.Data ?? new System.Collections.Generic.List<Booking>());
        }

        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> Create(int propertyId)
        {
            var propResponse = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (!propResponse.Success || propResponse.Data == null) return NotFound();

            var model = new BookingFormViewModel
            {
                PropertyId = propertyId,
                PropertyName = propResponse.Data.Name
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> Create(BookingFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            
            var propResponse = await _propertyService.GetPropertyByIdAsync(model.PropertyId);
            if (!propResponse.Success || propResponse.Data == null)
            {
                ModelState.AddModelError(string.Empty, "Property not found.");
                return View(model);
            }
            
            var nights = (float)Math.Ceiling((model.CheckOutDate - model.CheckInDate).TotalHours / 24.0);
            if (nights < 1) nights = 1;

            var booking = new Booking
            {
                PropertyId = model.PropertyId,
                GuestId = user.Id,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                GuestsCount = model.GuestsCount,
                TotalPrice = nights * (float)propResponse.Data.PricePerNight
            };

            var response = await _bookingService.CreateBookingAsync(booking);

            if (!response.Success)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "Error creating booking");
                
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var isOwner = User.IsInRole("Owner");
            var response = await _bookingService.GetBookingByIdAsync(id);
            
            if (response.Success && response.Data != null)
            {
                var booking = response.Data;
                bool canCancel = (isOwner && booking.Property.OwnerId == user.Id) || (!isOwner && booking.GuestId == user.Id);
                
                if (canCancel)
                {
                    booking.State = BookingState.Cancelled;
                    await _bookingService.UpdateBookingAsync(booking);
                }
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
