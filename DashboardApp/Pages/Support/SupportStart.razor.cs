using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Models;
using CountryApp.Pages.Support.Pages;
using Mendi.Blazor.DynamicNavigation;
using Newtonsoft.Json;

namespace CountryApp.Pages.Support
{
    [NavigatorRoutableComponent("Support", false)]
    public partial class SupportStart
    {
        List<Article>? Articles;

        protected override async Task OnInitializedAsync()
        {
            Articles = await DocumentDataService.GetConfiguration<List<Article>>(ConfigType.HelpArticles);
        }

        async Task OpenArticleSection(int parentId)
        {
            Dictionary<string, string> PageInfo = new()
            {
                {"ParentId", JsonConvert.SerializeObject(parentId) }
            };
            await NavigateToAsync(nameof(ArticleView), PageInfo);
        }
    }
}
