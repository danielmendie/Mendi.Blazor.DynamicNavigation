using Blazored.LocalStorage;
using CountryApp;
using CountryApp.Abstractions.Models;
using CountryApp.Abstractions.Services.Data;
using CountryApp.Abstractions.Services.Providers;
using CountryApp.Business;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using PageFlow.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var appSettings = builder.Configuration.Get<AppSettings>() ?? new AppSettings();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//add dynamic navigation services
builder.Services.AddBlazorPageFlow();
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
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    config.SnackbarConfiguration.MaxDisplayedSnackbars = 1;
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton(appSettings);
builder.Services.AddScoped<DataModule>();
builder.Services.AddScoped<JsLoaderService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<IAccessTokenProvider, AuthStateProvider>();
builder.Services.AddScoped<IDocumentDataService, DocumentDataService>();

await builder.Build().RunAsync();
