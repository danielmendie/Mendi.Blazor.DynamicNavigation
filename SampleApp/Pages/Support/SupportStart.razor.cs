using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Models;
using CountryApp.Pages.Support.Pages;
using Newtonsoft.Json;
using PageFlow.Blazor;

namespace CountryApp.Pages.Support
{
    [PageFlowRoutableComponent("Support", false)]
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
