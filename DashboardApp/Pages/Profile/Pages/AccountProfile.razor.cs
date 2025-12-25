using DashboardApp.Abstractions.Models;
using Mendi.Blazor.DynamicNavigation;
using System.ComponentModel.DataAnnotations;

namespace DashboardApp.Pages.Profile.Pages
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

            await UpdateUserData(nameof(AuthUserData.Email), EditContext.Email);
            await UpdateUserData(nameof(AuthUserData.Name), EditContext.Name);

            ShowNotification("Profile updated successfully", MudBlazor.Severity.Success);
            IsLoading = false;
        }

        class ProfileModel
        {
            [Required(ErrorMessage = "Your full name is required")]
            public string Name { get; set; } = null!;
            [Required(ErrorMessage = "Your email is required")]
            public string Email { get; set; } = null!;
        }

    }
}
