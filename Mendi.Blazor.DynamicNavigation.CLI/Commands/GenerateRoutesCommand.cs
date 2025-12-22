using Mendi.Blazor.DynamicNavigation.CLI.Helpers;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Text;

namespace Mendi.Blazor.DynamicNavigation.CLI.Commands
{
    public class GenerateRoutesCommand
    {
        public async Task<int> RunAsync(CommandOptions options)
        {
            UtilsHelper.Log("---------- Navigation Engine Started ----------");

            if (string.IsNullOrWhiteSpace(options.Path))
                options.Path = Directory.GetCurrentDirectory();

            UtilsHelper.Log($"Project directory: {options.Path}", options.Verbose);
            var projectInfo = ComponentHelper.GetProjectAssemblyInfo(options.Path);

            if (options.Force || projectInfo == null)
            {
                UtilsHelper.Log("Build started...", options.Verbose);
                var ok = UtilsHelper.BuildProject(options.Path, configuration: "Debug");
                if (!ok)
                {
                    UtilsHelper.Log("dotnet build failed. Aborting route generation.");
                    return 1;
                }
                UtilsHelper.Log("Build completed", options.Verbose);
            }

            UtilsHelper.Log($"Searching for routable components...", options.Verbose);
            var routes = ComponentHelper.GetRoutableComponents(options.Path);

            if (!routes.Any())
            {
                UtilsHelper.Log("No routable components found");
            }
            else
            {
                UtilsHelper.Log($"Found {routes.Count()} components");
                UtilsHelper.Log($"Searching for base components...", options.Verbose);
                await GeneratePageRoutes(routes, options);
            }

            UtilsHelper.Log("---------- Navigation Engine Completed ----------");
            return 0;
        }


        static List<string> ProjectNameSpaces = [];
        private async Task GeneratePageRoutes(IEnumerable<string> routeFilePaths, CommandOptions option)
        {
            try
            {
                var basePath = ComponentHelper.GetBaseComponentByAttribute(option);
                if (option.DryRun)
                {
                    UtilsHelper.Log($"Dry Run - skipping {routeFilePaths.Count()} route file updates.");
                    return;
                }

                #region CodeGenerator

                List<RoutePageInfo> componentList = [];
                foreach (var componentPath in routeFilePaths)
                {
                    //read file content
                    UtilsHelper.Log($"Processing route for: {componentPath}", option.Verbose);
                    var fileContent = File.ReadAllText(componentPath);

                    // Assuming .razor.cs file contains a class definition, extract its type.
                    var (componentType, nameSpace) = await ComponentHelper.ExtractComponentTypeAsync(fileContent, option.Path);
                    if (componentType == null)
                    {
                        UtilsHelper.Log($"Failed to extract at path: {componentPath}", option.Verbose);
                        continue;
                    }

                    var className = componentType.Name;
                    var fullyQualifiedName = componentType.FullName;

                    var attributes = componentType.GetCustomAttributes(typeof(NavigatorRoutableComponentAttribute), false);
                    if (attributes.Any())
                    {
                        foreach (NavigatorRoutableComponentAttribute attribute in attributes.Cast<NavigatorRoutableComponentAttribute>())
                        {
                            int appId = attribute.AppId;
                            bool isDefault = attribute.IsDefault;
                            string pageName = attribute.PageName;

                            componentList.Add(new RoutePageInfo
                            {
                                ComponentType = componentType,
                                AppId = appId,
                                IsDefault = isDefault,
                                Component = className,
                                PageName = pageName
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(nameSpace))
                            ProjectNameSpaces.Add(nameSpace);
                    }
                }

                var sb = new StringBuilder();
                sb.AppendLine(" NavigatorRegistry = new NavigatorRegistry");
                sb.AppendLine("{");
                sb.AppendLine("//routable components found for your app");
                sb.AppendLine("    Routes = new()");
                sb.AppendLine("    {");

                var totalComs = componentList.Count();
                var loopComs = 1;
                foreach (var component in componentList)
                {
                    var properties = component.ComponentType.GetProperties()
                        .Where(p => p.GetCustomAttributes<ParameterAttribute>().Any())
                        .Select(p => p);

                    //sb.AppendLine($"    {{");
                    //sb.AppendLine($"        nameof({component.Component}),");
                    sb.AppendLine($"        new RoutePageInfo");
                    sb.AppendLine($"        {{");
                    sb.AppendLine($"            AppId = {component.AppId},");
                    sb.AppendLine($"            PageName =  \"{component.PageName}\",");
                    sb.AppendLine($"            Component =  nameof({component.Component}),");
                    sb.AppendLine($"            ComponentType =  typeof({component.Component}),");
                    sb.AppendLine($"            IsDefault =  {(component.IsDefault ? "true" : "false")},");

                    if (properties.Any())
                    {
                        sb.AppendLine($"            Params = new()");
                        sb.AppendLine($"            {{");

                        var totalProps = properties.Count();
                        var loopProps = 1;
                        foreach (var property in properties)
                        {
                            var attributes = property.GetCustomAttributes(typeof(NavigatorClickEventAttribute), false);

                            if (attributes.Any())
                            {
                                //if (property.PropertyType.IsGenericType)
                                //{
                                //    Type genericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                                //    if (genericTypeDefinition == typeof(EventCallback<>))
                                //    {
                                //        Type innerType = property.PropertyType.GetGenericArguments()[0];

                                //        // Check if the inner type is Dictionary<string, string>
                                //        if (innerType == typeof(Dictionary<string, string>))
                                //        {
                                //            foreach (NavigatorClickEventAttribute attribute in attributes)
                                //            {
                                //                string nextRoutablePage = attribute.NextRoutablePage;
                                //                sb.AppendLine($"                {{");
                                //                sb.AppendLine($"                    \"{property.Name}\", EventCallback.Factory.Create<Dictionary<string, string>>(this, e => OnMapPageItemClicked(e, nameof({nextRoutablePage})))");
                                //                if (loopProps != totalProps)
                                //                {
                                //                    sb.AppendLine($"                }},");
                                //                }
                                //                else
                                //                {
                                //                    sb.AppendLine($"                }}");
                                //                }
                                //            }
                                //        }
                                //    }
                                //}
                            }
                            else
                            {
                                sb.AppendLine($"                {{");
                                sb.AppendLine($"                    \"{property.Name}\", \"Id\"  //value for Id will be auto substituted during navigation.");
                                if (loopProps != totalProps)
                                {
                                    sb.AppendLine($"                }},");
                                }
                                else
                                {
                                    sb.AppendLine($"                }}");
                                }
                            }

                            loopProps++;
                        }

                        sb.AppendLine($"            }}");
                    }

                    //sb.AppendLine($"        }}");
                    if (loopComs != totalComs)
                    {
                        sb.AppendLine($"    }},");
                    }
                    else
                    {
                        sb.AppendLine($"    }}");
                    }

                    loopComs++;
                }

                sb.AppendLine("    }");
                //sb.AppendLine("    },");
                //sb.AppendLine("    DefaultsRoutes = new()");
                //sb.AppendLine("    {");

                //var groupedComponents = componentList.GroupBy(info => info.AppId).OrderByDescending(f => f.Key);
                //var totalApps = groupedComponents.Count();
                //var loopTrail = 1;
                //foreach (var group in groupedComponents)
                //{
                //    var defaultAppComponent = group.FirstOrDefault(info => info.IsDefault) ?? group.First();
                //    if (totalApps == 1)
                //    {
                //        sb.AppendLine("    {");
                //        sb.AppendLine($"        {defaultAppComponent.AppId}, \"{defaultAppComponent.FullName}\"");
                //        sb.AppendLine("    }");
                //    }
                //    else
                //    {
                //        if (loopTrail != totalApps)
                //        {
                //            sb.AppendLine("    {");
                //            sb.AppendLine($"        {defaultAppComponent.AppId}, \"{defaultAppComponent.FullName}\"");
                //            sb.AppendLine("    },");
                //        }
                //        else
                //        {
                //            sb.AppendLine("    {");
                //            sb.AppendLine($"        {defaultAppComponent.AppId}, \"{defaultAppComponent.FullName}\"");
                //            sb.AppendLine("    }");
                //        }
                //    }

                //    loopTrail++;
                //}

                //sb.AppendLine("    }");
                sb.AppendLine("};");
                //sb.AppendLine("await DynamicNavigatorIndexDbAddValue(DynamicNavigatorIndexDbKeyTypes.Routes, PageRouteRegistry);");
                //sb.AppendLine("await BuildPageRoutes();");

                //CreatePageRouteContainer(basePath);
                CreatePageRouteRegistry(basePath);
                CreateVirtualOnAppNavigationSetup(basePath);
                AppendUIComponent(option.Path);
                //CreateSinglePageRoute(basePath);
                //await CreateOnBackToPreviousPageClicked(basePath);
                //await CreateOnMapNavMenuClicked(basePath);
                //await CreateOnMapPageItemClicked(basePath);
                //await CreateOnMapAppSwitchClicked(basePath);
                SaveGeneratedRoutes(sb.ToString(), basePath);

                #endregion

            }
            catch (Exception ex)
            {
                UtilsHelper.Log($"******* {ex.Message} ");
            }
        }

        #region Helpers

        private protected static void CreateVirtualOnAppNavigationSetup(string filePath)
        {
            filePath = filePath.Replace('/', '\\');

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                // Find the starting line of the OnInitialized method
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("protected override async Task OnAppNavigationSetup()"))
                    {
                        startIndex = i;
                        break;
                    }
                }

                bool proceed = true;
                if (startIndex != -1) proceed = false;
                if (startIndex != -1)
                {
                    // Find the real ending brace of the OnInitialized method
                    int endIndex = UtilsHelper.FindMatchingClosingBrace(lines, startIndex);

                    // Remove existing OnInitialized method
                    if (endIndex != -1)
                    {
                        lines = lines.Take(startIndex).Concat(lines.Skip(endIndex + 1)).ToArray();
                    }
                }

                if (proceed)
                {
                    // Find the index where the OnInitialized method was removed
                    int insertIndex = startIndex;

                    // Generate the new method code
                    StringBuilder newMethodCode = new StringBuilder();
                    newMethodCode.AppendLine("protected override async Task OnAppNavigationSetup()");
                    newMethodCode.AppendLine("{");
                    newMethodCode.AppendLine("GetPageRoutes();");
                    newMethodCode.AppendLine("await base.OnAppNavigationSetup();");
                    newMethodCode.AppendLine("}");

                    if (startIndex != -1)
                    {
                        // Insert the new method code
                        lines = lines.Take(insertIndex).Concat(newMethodCode.ToString().Split('\n')).Concat(lines.Skip(insertIndex)).ToArray();
                    }
                    else
                    {
                        lines = lines.Take(lines.Length - 1).Concat(newMethodCode.ToString().Split('\n')).Concat(new[] { "}" }).ToArray();
                    }

                    // Write the modified content back to the file
                    File.WriteAllLines(filePath, lines);

                    UtilsHelper.FormatCode(filePath, ProjectNameSpaces);
                }
            }
            else
            {
                UtilsHelper.Log($"File not found: {filePath}");
            }
        }

        private protected static void CreatePageRouteRegistry(string filePath)
        {
            filePath = filePath.Replace('/', '\\');

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                // Find the starting line of the GetPageRoutes method
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("NavigatorRegistry NavigatorRegistry { get; set; }"))
                    {
                        startIndex = i;
                        break;
                    }
                }

                bool proceed = true;
                if (startIndex != -1) proceed = false;
                if (startIndex != -1)
                {
                    // Find the real ending brace of the GetPageRoutes method
                    int endIndex = UtilsHelper.FindMatchingClosingBrace(lines, startIndex);

                    // Remove existing GetPageRoutes method
                    if (endIndex != -1)
                    {
                        lines = lines.Take(startIndex).Concat(lines.Skip(endIndex + 1)).ToArray();
                    }
                }

                if (proceed)
                {
                    // Find the index where the GetPageRoutes method was removed
                    int insertIndex = startIndex;

                    // Generate the new method code
                    StringBuilder newMethodCode = new StringBuilder();
                    newMethodCode.AppendLine("[Inject] public NavigatorRegistry NavigatorRegistry { get; set; } = null!;");

                    if (startIndex != -1)
                    {
                        // Insert the new method code
                        lines = lines.Take(insertIndex).Concat(newMethodCode.ToString().Split('\n')).Concat(lines.Skip(insertIndex)).ToArray();
                    }
                    else
                    {
                        lines = lines.Take(lines.Length - 1).Concat(newMethodCode.ToString().Split('\n')).Concat(new[] { "}" }).ToArray();
                    }

                    // Write the modified content back to the file
                    File.WriteAllLines(filePath, lines);

                    UtilsHelper.FormatCode(filePath, ProjectNameSpaces, true);
                }
            }
            else
            {
                UtilsHelper.Log($"File not found: {filePath}");
            }
        }

        private protected static void AppendUIComponent(string directory)
        {
            var indexPath = ComponentHelper.GetIndexComponentByAttribute(directory);

            if (indexPath is null)
            {
                return; // nothing to do if there's no Home.razor
            }

            indexPath = indexPath.EndsWith(".cs") ? indexPath.Replace(".cs", "") : indexPath;
            var lines = File.ReadAllLines(indexPath).ToList();
            const string UIRender = "<Mendi.Blazor.DynamicNavigation.BlazorDynamicNavigator />";

            var exists = lines.Any(l => string.Equals(l.Trim(), UIRender, StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                return;
            }

            // Append as a new line at the bottom
            lines.Add(UIRender);
            File.WriteAllLines(indexPath, lines);
        }

        private protected static void SaveGeneratedRoutes(string code, string filePath)
        {
            filePath = filePath.Replace('/', '\\');

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                // Find the starting line of the GetPageRoutes method
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("private void GetPageRoutes()"))
                    {
                        startIndex = i;
                        break;
                    }
                }

                if (startIndex != -1)
                {
                    // Find the real ending brace of the GetPageRoutes method
                    int endIndex = UtilsHelper.FindMatchingClosingBrace(lines, startIndex);

                    // Remove existing GetPageRoutes method
                    if (endIndex != -1)
                    {
                        lines = lines.Take(startIndex).Concat(lines.Skip(endIndex + 1)).ToArray();
                    }
                }

                // Find the index where the GetPageRoutes method was removed
                int insertIndex = startIndex;

                // Generate the new method code
                StringBuilder newMethodCode = new StringBuilder();
                newMethodCode.AppendLine("private void GetPageRoutes()");
                newMethodCode.AppendLine("{");
                newMethodCode.AppendLine($"   {code}");
                newMethodCode.AppendLine("}");

                if (startIndex != -1)
                {
                    // Insert the new method code
                    lines = lines.Take(insertIndex).Concat(newMethodCode.ToString().Split('\n')).Concat(lines.Skip(insertIndex)).ToArray();
                }
                else
                {
                    lines = lines.Take(lines.Length - 1).Concat(newMethodCode.ToString().Split('\n')).Concat(new[] { "}" }).ToArray();
                }

                // Write the modified content back to the file
                File.WriteAllLines(filePath, lines);

                UtilsHelper.FormatCode(filePath, ProjectNameSpaces);
            }
            else
            {
                UtilsHelper.Log($"File not found: {filePath}");
            }

        }

        #endregion
    }
}
