using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Models;
using Microsoft.AspNetCore.Components;
using PageFlow.Blazor;

namespace CountryApp.Pages.Support.Pages
{
    [PageFlowRoutableComponent("Help Article", false)]
    public partial class ArticleView
    {
        [Parameter] public string ParentId { get; set; } = null!;
        bool IsReader;

        List<Article> Articles = [];
        string Header = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            var articles = await DocumentDataService.GetConfiguration<List<Article>>(ConfigType.HelpArticles);
            Articles = [.. articles!.Where(p => p.ParentId == Convert.ToInt32(ParentId))];
            Header = Articles.First().Category;
        }

        MarkupString RenderRawHtml = new();
        Article? SelectedArticle;
        async Task OnReadArticle(Article article)
        {
            SelectedArticle = article;
            RenderRawHtml = (MarkupString)article.Data;
            IsReader = true;
            await InvokeAsync(StateHasChanged);
        }

    }
}
