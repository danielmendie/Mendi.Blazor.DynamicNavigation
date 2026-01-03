using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Enums;
using CountryApp.Abstractions.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CountryApp.Pages.Security
{
    public partial class SignInUser
    {
        LoginModel EditContext = new();
        bool IsBusy;

        async Task OnValidSubmit()
        {
            if (CurrentUser != null && !string.IsNullOrWhiteSpace(CurrentUser.Email) && !EditContext.Email.Equals(CurrentUser.Email, StringComparison.OrdinalIgnoreCase))
            {
                ShowNotification("Invalid profile email", MudBlazor.Severity.Error);
                return;
            }

            IsBusy = true;
            var activities = UserActivities;
            activities.Add(new Activity { ActivityType = EnumActivity.SignIn, CreatedOn = DateTime.UtcNow, Description = $"You signed in using {EditContext.Email}", CreatedBy = EditContext.Email });
            await LocalStorage.SetItemAsync(ConfigType.UserActivitiesStore, activities);

            await UpdateUserData(nameof(Abstractions.Models.Profile.IsLoggedIn), true);
            await UpdateUserData(nameof(Abstractions.Models.Profile.Email), EditContext.Email);
            await UpdateUserData(nameof(Abstractions.Models.Profile.LastLoggedIn), DateTime.UtcNow);

            var profiles = Profiles;
            if(!profiles.Any(g=>g.Email == EditContext.Email))
            {
                profiles.Add(CurrentUser!);
                await LocalStorage.SetItemAsync(ConfigType.UserProfilesStore, profiles);
            }

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
