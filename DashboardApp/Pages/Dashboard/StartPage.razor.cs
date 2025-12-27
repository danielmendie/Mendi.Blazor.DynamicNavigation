using CountryApp.Abstractions.Models;
using Mendi.Blazor.DynamicNavigation;

namespace CountryApp.Pages.Dashboard
{
    [NavigatorRoutableComponent("Overview", true)]
    public partial class StartPage
    {
        List<Activity> CurrentUserActivities = [];

        protected override void OnInitialized()
        {
            CurrentUserActivities = UserActivities.Where(k => k.CreatedBy == CurrentUser.Email).OrderByDescending(j => j.CreatedOn).Take(3).ToList();
        }
    }
}
