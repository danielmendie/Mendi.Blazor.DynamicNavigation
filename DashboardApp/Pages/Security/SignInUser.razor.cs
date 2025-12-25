using DashboardApp.Abstractions.Constants;
using DashboardApp.Abstractions.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DashboardApp.Pages.Security
{
    public partial class SignInUser
    {
        LoginModel EditContext = new();
        bool IsBusy;

        async Task OnValidSubmit()
        {
            IsBusy = true;
            var modelData = new AuthUserData { IsLoggedIn = true, Email = EditContext.Email };
            await LocalStorage.SetItemAsync(ConfigType.IdentityUserStore, modelData);
            NavigationManager.NavigateTo("/", true);
            IsBusy = false;
        }

        class LoginModel
        {
            [Required(ErrorMessage = "Random email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; } = null!;
            [Required(ErrorMessage = "Random password is required")]
            public string Password { get; set; } = null!;
        }
    }
}
