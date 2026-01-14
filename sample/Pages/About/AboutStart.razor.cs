using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Models;
using Microsoft.AspNetCore.Components;
using PageFlow.Blazor;

namespace CountryApp.Pages.About
{
    [PageFlowRoutableComponent("About App", false)]
    public partial class AboutStart
    {
        MarkupString RenderTermsOfService = new();
        MarkupString RenderPrivacyPolicy = new();
        SettingGeneral? GeneralData;
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
