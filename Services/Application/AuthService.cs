using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using main.Models;
using main.ViewModels.Auth;
using main.Services.Interfaces;

namespace main.Services.Application
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Identification = model.Identification,
                BirthDate = model.BirthDate,
                Role = model.Role,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign role in Identity based on the enum selection
                await _userManager.AddToRoleAsync(user, model.Role.ToString());
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return result;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
