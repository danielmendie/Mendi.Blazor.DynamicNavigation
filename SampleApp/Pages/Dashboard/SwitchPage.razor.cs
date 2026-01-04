using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Enums;
using CountryApp.Abstractions.Models;
using Mendi.Blazor.DynamicNavigation;
using System.ComponentModel.DataAnnotations;

namespace CountryApp.Pages.Dashboard
{
    [NavigatorRoutableComponent("Switch Apps", false)]
    public partial class SwitchPage
    {
        bool ShowPassword;
        Abstractions.Models.Profile SelectedProfile = new();
        List<Abstractions.Models.Profile> DisplayProfiles = [];
        VerificationModel EditContext = new();

        protected override void OnInitialized()
        {
            DisplayProfiles = Profiles;
        }

        async Task OnSwitchAccount(Abstractions.Models.Profile profile)
        {
            SelectedProfile = profile;
            if (profile.RequireApproval)
            {
                EditContext = new();
                ShowPassword = true;
                return;
            }

            await OnValidSubmit();
        }

        async Task OnValidSubmit()
        {
            SelectedProfile.LastLoggedIn = DateTime.UtcNow;
            SelectedProfile.IsLoggedIn = true;

            await UpdateUserData(nameof(Abstractions.Models.Profile.IsLoggedIn), false);
            var activities = UserActivities;
            activities.Add(new Activity { ActivityType = EnumActivity.SignOut, CreatedOn = DateTime.UtcNow, Description = $"You switched account from {CurrentUser.Email}", CreatedBy = CurrentUser.Email });
            activities.Add(new Activity { ActivityType = EnumActivity.SignIn, CreatedOn = DateTime.UtcNow, Description = $"You switched account to {SelectedProfile.Email}", CreatedBy = SelectedProfile.Email });
            await LocalStorage.SetItemAsync(ConfigType.UserActivitiesStore, activities);
            await LocalStorage.SetItemAsync(ConfigType.IdentityUserStore, SelectedProfile);
            await NavigateToAsync(nameof(StartPage));
        }

        string ImagePath(string? displayData) => string.IsNullOrWhiteSpace(displayData)
                  ? "assets/images/user/guest.bmp"
                  : displayData;

        class VerificationModel
        {
            [Required(ErrorMessage = "Random password is required")]
            public string Password { get; set; } = null!;
        }
    }
}
