using DashboardApp;
using DashboardApp.Abstractions.Models;
using DashboardApp.Abstractions.Services.Data;
using DashboardApp.Abstractions.Services.Providers;
using DashboardApp.Business;
using Mendi.Blazor.DynamicNavigation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var appSettings = builder.Configuration.Get<AppSettings>() ?? new AppSettings();
builder.Services.AddSingleton(appSettings);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//add dynamic navigation services
builder.Services.AddBlazorDynamicNavigator(opt => opt.IgnoreRouteHistory = false);
//add mudblazor services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = false;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
    config.SnackbarConfiguration.MaxDisplayedSnackbars = 1;
});

builder.Services.AddScoped<JsLoaderService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddHttpClient(string.Empty);
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient());
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<IAccessTokenProvider, AuthStateProvider>();
builder.Services.AddScoped<IDocumentDataService, DocumentDataService>();

await builder.Build().RunAsync();
