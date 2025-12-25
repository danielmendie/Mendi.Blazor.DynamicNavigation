using DashboardApp.Abstractions.Constants;
using DashboardApp.Abstractions.Models;
using Mendi.Blazor.DynamicNavigation;
using Microsoft.AspNetCore.Components;

namespace DashboardApp.Pages.About
{
    [NavigatorRoutableComponent("About App", false)]
    public partial class AboutStart
    {
        MarkupString RenderTermsOfService = new();
        MarkupString RenderPrivacyPolicy = new();
        SettingGeneral GeneralData = null!;
        bool IsLoading;

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            GeneralData = await DocumentDataService.GetConfiguration<SettingGeneral>(ConfigType.GeneralSetting);
            var terms = await DocumentDataService.GetDocumentType(ConfigType.TermsOfService);
            var policy = await DocumentDataService.GetDocumentType(ConfigType.PrivacyPolicy);
            RenderTermsOfService = (MarkupString)terms;
            RenderPrivacyPolicy = (MarkupString)policy;
            IsLoading = false;
        }
    }
}
