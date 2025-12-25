using DashboardApp.Abstractions.Constants;
using DashboardApp.Abstractions.Models;
using DashboardApp.Pages.Support.Pages;
using Mendi.Blazor.DynamicNavigation;
using Newtonsoft.Json;

namespace DashboardApp.Pages.Support
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
