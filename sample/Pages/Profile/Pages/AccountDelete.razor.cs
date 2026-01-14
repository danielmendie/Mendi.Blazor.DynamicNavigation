using PageFlow.Blazor;
using System.ComponentModel.DataAnnotations;

namespace CountryApp.Pages.Profile.Pages
{
    [PageFlowRoutableComponent("Account Deletion", false)]
    public partial class AccountDelete
    {
        bool IsLoading;
        bool Disabled;

        async Task HandleValidSubmit()
        {
            IsLoading = true;

            await LocalStorage.ClearAsync();
            NavigationManager.NavigateTo("", true);
            IsLoading = false;
        }

        string? _changed;
        string? UserName
        {
            get => _changed;
            set
            {
                _changed = value;
                EditContext.Email = value;
                Disabled = !CurrentUser.Email.ToLower().Equals(value?.ToLower());
            }
        }

        DeleteModel EditContext = new();
        class DeleteModel
        {
            [Required(ErrorMessage = "Email required")]
            public string? Email { get; set; }
        }
    }
}
