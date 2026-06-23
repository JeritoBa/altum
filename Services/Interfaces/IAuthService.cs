using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using main.ViewModels.Auth;

namespace main.Services.Interfaces
{
    public interface IAuthService
    {
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task LogoutAsync();
    }
}
