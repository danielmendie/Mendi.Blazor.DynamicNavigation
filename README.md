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
The `NavigatorBaseComponent` attribute should be specified on the class acting as your component base class - **Important**



> # Mendi.Blazor.DynamicNavigation.CLI
>>Command line tool for generating page routes and building routes for the dynamic navigation use in your application

