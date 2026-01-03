# Mendi.Blazor.DynamicNavigation

[![NuGet Version](https://img.shields.io/nuget/v/Mendi.Blazor.DynamicNavigation.svg?style=flat&label=nuget)](https://www.nuget.org/packages/Mendi.Blazor.DynamicNavigation)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Mendi.Blazor.DynamicNavigation.svg?style=flat)](https://www.nuget.org/packages/Mendi.Blazor.DynamicNavigation)

>This library lets you build **single‑URL, multi‑page** Blazor apps by routing between components without changing the browser address or exposing URL parameters. It also persists navigation state and helps prevent URL tampering and hijacking.

## 🧑‍💻Platforms
- [x] Blazor Server
- [x] Blazor WebAssembly
- [x] MAUI Blazor Hybrid

## 👏Features
- [x] Static URL - *Navigate between pages while the browser URL stays the same.*
- [x] Page Retention - *Restore the last visited page after refresh or browser reopen.*
- [x] Previous Page History - *Navigate back through previously visited dynamic pages.*
- [x] Nav Menu Binding - *Trigger navigation from any menu/button via attributes.*
- [x] Multi-App Switching - *Partition routes into logical “apps” using `appId`.*
- [x] Storage Option - *Persist navigation state via LocalStorage*
- [x] CLI Tool - *Generate and build route metadata from your project automatically.*

## 📖Installation
To begin, install the latest version of **Mendi.Blazor.DynamicNavigation** Nuget package from Visual Studio or use the Command-line tool: 
`dotnet add package Mendi.Blazor.DynamicNavigation` 

## 🔧Configuration

Open your project's **Program.cs** file and add this section 
```csharp
builder.Services.AddBlazorDynamicNavigator(option => option.IgnoreRouteHistory = false);
```
To prevent the app from remembering last visited page, set option **IgnoreRouteHistory** to True


**STEPS HIGHLIGHTED HERE CAN BE DONE AUTOMATICALLY USING THE CLI TOOL**

1. Create a base class in the root directory and inherit the `BlazorDynamicNavigatorBase` class then add the `NavigatorBaseComponent` attribute to your base component class. Your class should look similar to this:

```csharp
using Mendi.Blazor.DynamicNavigation;
using Microsoft.AspNetCore.Components;

namespace Test.Pages;
[NavigatorBaseComponent]
public class MyBaseComponent : BlazorDynamicNavigatorBase
{
}
```
The `NavigatorBaseComponent` attribute should be specified on the custom class acting as your component base class **This is very important**

2. Open the `_Imports.razor` file and add the following lines of code
``` csharp
@using Mendi.Blazor.DynamicNavigation
@inherits MyBaseComponent
```
MyBaseComponent - This should be the name of your base component class(however you had called it)

3. Add the **BlazorDynamicPageNavigator** component to the Home.razor or Index.razor file `<BlazorDynamicNavigator />`

4. In the Home.razor or Index.razor override OnInitializedAsync() and call the OnAppNavigationSetup(). You only have to add this call once.
```csharp
protected override async Task OnInitializedAsync()
{
    await OnAppNavigationSetup();
}
```

## 🚀 Using It

The **Mendi.Blazor.DynamicNavigation** is used for configuring your project. You'll use class and property attributes to define your routable components and parameter properties. 
To define a routable component, decorate it with the `NavigatorRoutableComponent` attribute. Here's a typical example

``` csharp
using Mendi.Blazor.DynamicNavigation;
using Microsoft.AspNetCore.Components;

namespace TestApp.Pages
{
    [NavigatorRoutableComponent(pageName: "Sample", isDefault: true)]
    public partial class Sample
    {
        [Parameter] public string Username { get; set; } = null!;

    }
}
```
**NavigatorRoutableComponent** - This attribute requires three parameters; pageName(string: name of the page), isDefault(bool: indicates if the component is the default route for the app Id), appId - optional(int: used to categorize your app into different sub apps)

To navigate to the sample page you call `NavigateToAsync` and pass the component name and its parameters if any
``` csharp
  await NavigateToAsync(nameof(Sample), new Dictionary<string, string> { { "Username", "Daniel Mendie" } });

```

Once your routable components are decorated. The rest is up to **Mendi.Blazor.DynamicNavigation.CLI** tool to complete😉



# Mendi.Blazor.DynamicNavigation.CLI
>Command line tool for generating page routes for the dynamic navigation use in your application

[![NuGet Version](https://img.shields.io/nuget/v/Mendi.Blazor.DynamicNavigation.CLI.svg?style=flat&label=nuget)](https://www.nuget.org/packages/Mendi.Blazor.DynamicNavigation.CLI)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Mendi.Blazor.DynamicNavigation.CLI.svg?style=flat)](https://www.nuget.org/packages/Mendi.Blazor.DynamicNavigation.CLI)

## 📖Installation
To install the latest version of **Mendi.Blazor.DynamicNavigation.CLI** tool, run `dotnet tool install -g Mendi.Blazor.DynamicNavigation.CLI` from command line


## 🚀 Using It

The CLI is responsible for generating route pages and binding them. To use it, open command-line tool to the directory of your project to run the following commands:

- To generate route pages
```
dynamicnav-engine routes generate
```

Other additional command flags include:

Point to a different project path using `-p` or `--path` 
```
dynamicnav-engine routes generate --path path\to\your\projet\directory
```
Force build project before generating routes using `-f ` or `--force`
```
dynamicnav-engine routes generate --force
```
Verbose logging using `-v` or `--verbose`
```
dynamicnav-engine routes generate --verbose
```

Once the command runs successfully, your BaseNavigator class will be populated with the routes.

Now run the project, and don't forget to check your browser's dev tool console for extra log information if you ever get into an issue


## 😎 Extras
Here are a couple of extra things you can do to make this package serve your business needs adequately. The following methods can be overridden based on your needs and logic

**OnAfterNavigateAsync** - Event triggered after a successful page navigation

Scenario: You want to persist user page session across multiple devices, get route metadata and send to your backend api
```csharp
protected override Task OnAfterNavigateAsync(RoutePageInfo route, Dictionary<string, string>? parameters)
{
    return base.OnAfterNavigateAsync(route, parameters);
}
```

**OnBeforeNavigateAsync** - Event triggered before a page navigation

Scenario: You want to do a pre-conditional check for feature accessibility or permission
```csharp
protected override Task OnBeforeNavigateAsync(RoutePageInfo route, Dictionary<string, string>? parameters)
{
    return base.OnBeforeNavigateAsync(route, parameters);
}
```

**OnNavigationFailedAsync** - Event triggered when page navigation fails
```csharp
protected override Task OnNavigationFailedAsync(Exception exception)
{
    return base.OnNavigationFailedAsync(exception);
}
```

**OnCannotGoBackAsync** - Event triggered when there is no page in history to return to
```csharp
protected override Task OnCannotGoBackAsync()
{
    return base.OnCannotGoBackAsync();
}
```

**RestoreOrNavigateToDefaultAsync** - Entry point for dynamic navigation to either restore from history or find default page based on generated pages

Scenario: Fetch previous route metadata from API(multiple devices) then restore page from the fetched data
```csharp
protected override Task RestoreOrNavigateToDefaultAsync()
{
    return base.RestoreOrNavigateToDefaultAsync();
}
```

Cheers!

## Contributing

Contributions are welcome! Please see [CONTRIBUTING](./CONTRIBUTING.md) for guidelines on setting up your environment, coding style, and making pull requests.

## License

This project is licensed under the **MIT** License. See [LICENSE](./LICENSE) for details.


[NavigatorSampleVideo.mp4](https://github.com/danielmendie/Mendi.Blazor.DynamicNavigation/assets/66223776/0e6f1a56-d133-4604-83e7-69207cab3be2)

