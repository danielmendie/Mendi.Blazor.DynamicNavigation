using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Enums;
using CountryApp.Abstractions.Models;
using Mendi.Blazor.DynamicNavigation;
using System.ComponentModel.DataAnnotations;

namespace CountryApp.Pages.Profile.Pages
{
    [NavigatorRoutableComponent("Profile Update", false)]
    public partial class AccountProfile
    {
        bool IsLoading;
        ProfileModel EditContext = new();

        protected override void OnInitialized()
        {
            EditContext = new()
            {
                Email = CurrentUser.Email,
                Name = CurrentUser.Name
            };
        }

        async Task HandleValidSubmit()
        {
            IsLoading = true;

            await UpdateUserData(nameof(Abstractions.Models.Profile.Email), EditContext.Email);
            await UpdateUserData(nameof(Abstractions.Models.Profile.Name), EditContext.Name!);
            var activities = UserActivities;
            activities.Add(new Activity { ActivityType = EnumActivity.Profile, CreatedOn = DateTime.UtcNow, Description = $"Your profile detail for {CurrentUser.Email} was updated", CreatedBy = EditContext.Email });
            await LocalStorage.SetItemAsync(ConfigType.UserActivitiesStore, activities);
            ShowNotification("Profile updated successfully", MudBlazor.Severity.Success);
            IsLoading = false;
        }

        class ProfileModel
        {
            [Required(ErrorMessage = "Your full name is required")]
            public string? Name { get; set; }
            [Required(ErrorMessage = "Your email is required")]
            public string Email { get; set; } = null!;
        }

    }
}
