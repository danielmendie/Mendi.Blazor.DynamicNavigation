using CountryApp.Layout;
using Mendi.Blazor.DynamicNavigation;
using Microsoft.AspNetCore.Components.Forms;

namespace CountryApp.Pages.Profile
{
    [NavigatorRoutableComponent("Account Setting", false)]
    public partial class AccountStart
    {
        ScriptLoader scriptLoader = null!;
        string? UploadedImage;

        string ImagePath()
        {
            if (!string.IsNullOrWhiteSpace(UploadedImage))
                return UploadedImage;

            var path = string.IsNullOrWhiteSpace(CurrentUser.DisplayImage)
                  ? "assets/images/user/guest.bmp"
                  : CurrentUser.DisplayImage;

            return path;
        }

        async Task OnMediaUpload(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (!file.ContentType.StartsWith("image/") || file.Size > 2 * 1024 * 1024)
            {
                ShowNotification("Max image size is 2mb", MudBlazor.Severity.Error);
                return;
            }

            using var stream = file.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024); // 2MB
            using var ms = new MemoryStream();

            await stream.CopyToAsync(ms);
            var bytes = ms.ToArray();

            var base64 = Convert.ToBase64String(bytes);
            UploadedImage = $"data:{file.ContentType};base64,{base64}";
            await UpdateUserData(nameof(Abstractions.Models.Profile.DisplayImage), UploadedImage);
            ShowNotification("Display image uploaded successfully", MudBlazor.Severity.Success);
        }

        async Task OpenFilePicker() => await scriptLoader.InvokeVoidAsync("OpenFilePicker");

        async Task Goto(string page) => await NavigateToAsync(page);
    }
}
