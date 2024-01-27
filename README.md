> # Mendi.Blazor.DynamicNavigation
>>This package allows dynamic routing in your Blazor application. It configures your app to route to pages within your application without changing URLs, making it a single page. It is safe and also prevents URL tempering and hijacking

## 👏Features
- [x] Static URL - *Urls do not change when you navigate to any routable page*
- [x] Page Rentention - *It remembers the last page a user was on, even when the browser is refreshed or closed*
- [x] Previous Page History - *It can navigate back to previous pages visited*
- [x] Nav Menu Binding - *Navigate to any routable page from your nav menu*
- [x] Multi-App Switching - *Switch between multiple apps within your project. This is very useful for controlling the UI*

## 📖Installation
To begin, install the **Mendi.Blazor.DynamicNavigation** Nuget package from Visual Studio or use the CLI: 
`dotnet add package Mendi.Blazor.DynamicNavigation` 

## 🔧Configuration

Open your project's **Program.cs** file and replace this section ```await builder.RunAsync()``` with
```csharp
var app = await builder.UseDynamicNavigator();
await app.RunAsync();
```

Now create a `BaseComponent.cs` class in the pages folder or wherever it would be convenient for you. You can change the `BaseComponent.cs` name of course.
You'll have to inherit the `DynamicNavigatorComponentBase` class and add the `NavigatorBaseComponent` attribute. Your class should look similar to this:
```csharp
using Mendi.Blazor.DynamicNavigation;
using Mendi.Blazor.DynamicNavigation.Common;

namespace Test.Pages;
[NavigatorBaseComponent]
public class BaseComponent : DynamicNavigatorComponentBase
{

}
```
The `NavigatorBaseComponent` attribute should be specified on the class acting as your component base class **This is very important**

Open the `_Imports.razor` file and add the following lines of code
``` csharp
@using Mendi.Blazor.DynamicNavigation
@inherits BaseComponent
```
BaseComponent - This should be the name of your base component class(however you had called it)

After that, add the **BlazorDynamicPageNavigator** component to the Home.razor or Index.razor file `<BlazorDynamicPageNavigator NavigatorContainer="PageRouteContainer" NavigatorRegistry="PageRouteRegistry" />`

## 🚀Using It

The **Mendi.Blazor.DynamicNavigation** is merely used for configuring your project. You'll use class and property attributes to define your routable components and parameter properties. 
To define a routable component, decorate it with the `NavigatorRoutableComponent` attribute, and decorate your callback event properties with the `NavigatorClickEvent` attribute. Here's a typical example

``` csharp
using Mendi.Blazor.DynamicNavigation.Common;
using Microsoft.AspNetCore.Components;
using Test.Pages.HiJack;

namespace Test.Pages.SampleHere
{
    [NavigatorRoutableComponent(appName: "Home Page", isDefault: false, appId: 1)]
    public partial class IndexPage
    {
        [Parameter]
        [NavigatorClickEvent(gotoComponent: nameof(Calculator))]
        public EventCallback<Dictionary<string, string>> OnGotoCalculator { get; set; }

        async Task OnOpenCalculatorButtonClicked()
        {
            Dictionary<string, string> data = new()
            {
                { "DisplayName", "Daniel Mendie" },
                { "Operation", "Multiplication" }
            };
            await OnGotoCalculator.InvokeAsync(data);
        }
    }
}
```
**NavigatorRoutableComponent** - This attribute requires three parameters; appName(string: name of the component or app), isDefault(bool: indicates if the component is the default route for the app Id), appId(int: used to categorize your app into different sub apps)

**NavigatorClickEvent** - This attribute requires one parameter; gotoComponent(string: the name of the next routable component to navigate to). This attribute should be applied to EventCallback properties.

And here's what the `Calculator.razor.cs` file would look like
``` csharp
using Mendi.Blazor.DynamicNavigation.Common;
using Microsoft.AspNetCore.Components;
using Test.Pages.SampleHere;

namespace Test.Pages.HiJack
{
    [NavigatorRoutableComponent(appName: "Calculator", isDefault: false, appId: 1)]
    public partial class Calculator
    {
        [Parameter] public string DisplayName { get; set; }
        [Parameter] public string Operation { get; set; }

       [Parameter]
       [NavigatorClickEvent(gotoComponent: nameof(IndexPage))]
       public EventCallback<Dictionary<string, string>> OnGotoIndex { get; set; }

        async Task OnBackButtonClicked()
        {
            Dictionary<string, string> data = [];
            await OnGotoIndex.InvokeAsync(data);
        }
    }
}

```

Once your routable components are decorated. The rest is up to **Mendi.Blazor.DynamicNavigation.CLI** tool to complete😉



> # Mendi.Blazor.DynamicNavigation.CLI
>>Command line tool for generating page routes and building routes for the dynamic navigation use in your application

