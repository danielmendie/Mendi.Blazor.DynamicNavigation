using CountryApp.Abstractions.Models;
using Mendi.Blazor.DynamicNavigation;
using MudBlazor;

namespace CountryApp.Pages.Dashboard
{
    [NavigatorRoutableComponent("Overview", true)]
    public partial class StartPage
    {
        IEnumerable<Activity> CurrentUserActivities = [];
        ChartData GraphData = new ChartData();

        protected override void OnInitialized()
        {
            CurrentUserActivities = UserActivities.Where(k => k.CreatedBy == CurrentUser.Email).OrderByDescending(j => j.CreatedOn).Take(4);

            GraphData = BuildQuotaChartData();
        }

        public class ChartData
        {
            public double[] QuoataData = [];
            public string[] QuotaLabels = [];
            public List<ChartSeries> Series = [];
            public string[] SeriesLabels = [];
        }

        public ChartData BuildQuotaChartData()
        {
            var now = DateTime.UtcNow;   
            var start = new DateTime(now.Year, now.Month, 1).AddMonths(-11); 
            var months = Enumerable.Range(0, 12).Select(i => start.AddMonths(i)).ToList();
            var labels = months.Select(d => d.ToString("MMM")).ToArray();
            var grouped = UserQuota
                .Where(q => q.Owner.Equals(CurrentUser.Email, StringComparison.OrdinalIgnoreCase) && q.CreatedOn >= start && q.CreatedOn <= now)
                .GroupBy(q => new { q.CreatedOn.Year, q.CreatedOn.Month })
                .ToDictionary(
                    g => (g.Key.Year, g.Key.Month),
                    g => g.Count()
                );

            var current = grouped.TryGetValue((now.Year, now.Month), out var currentCount);
            var data = months .Select(m =>
                {
                    grouped.TryGetValue((m.Year, m.Month), out var count);
                    return (double)count;
                }).ToArray();
            var chartData = new ChartData
            {
                QuoataData = [currentCount, 15],
                QuotaLabels = ["Current", "Limit"],

                SeriesLabels = labels,
                Series = new List<ChartSeries>
                {
                    new ChartSeries
                    {
                        Name = now.Year.ToString(),
                        Data = data
                    }
                }
            };

            return chartData;
        }

    }
}
