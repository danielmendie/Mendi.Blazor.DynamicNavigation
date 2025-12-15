using Mendi.Blazor.DynamicNavigation.CLI.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using System.Text;

namespace Mendi.Blazor.DynamicNavigation.CLI.Commands
{
    public class BuildRoutesCommand
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
                UtilsHelper.Log("Force build - Building project before route generation...", options.Verbose);
                var ok = UtilsHelper.BuildProject(options.Path, configuration: "Debug");
                if (!ok)
                {
                    UtilsHelper.Log("dotnet build failed. Aborting route generation.");
                    return 1;
                }
            }

            await BuildPageRoutes(options);
            UtilsHelper.Log("---------- Navigation Engine Completed ----------");
            return 0;
        }

        private async Task BuildPageRoutes(CommandOptions options)
        {
            var baseComponent = ComponentHelper.GetBaseComponentByAttribute(options);
            string[] lines = File.ReadAllLines(baseComponent);

            // Find the starting line of the BuildPageRoutes method
            int startIndex = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("public override async task BuildPageRoutes()"))
                {
                    startIndex = i;
                    break;
                }
            }

            if (startIndex != -1 && !options.Force)
            {
                //The BuildPageRoute method if exists might have a devs custom code, so prevent scaffolding which will remove every custom code and prompt the dev for response to carry on
                UtilsHelper.Log("BuildPageRoutes exists already. To explicitly force a new scaffold without prompt, specify the -f or --force parameter");
                UtilsHelper.Log("Do you want to continue with scaffold? (y/n)");
                var responseInput = Console.ReadLine();
                if (responseInput.ToLower().Equals("y"))
                {
                    UtilsHelper.Log("Continuing to scaffolding... custom dev code may be lost", options.Verbose);
                    goto ContinueProcess;
                }
                else
                {
                    return;
                }
            }

ContinueProcess:
            if (options.DryRun)
            {
                UtilsHelper.Log($"Dry Run - skipping BuildPageRoutes method update.");
                return;
            }

            var helper = new ComponentHelper();
            var routableComponents = ComponentHelper.GetRoutableComponents(options.Path);

            try
            {
                List<RoutePageInfo> componentInfoList = [];

                foreach (var componentPath in routableComponents)
                {
                    //read file content
                    var fileContent = File.ReadAllText(componentPath);

                    // Assuming .razor.cs file contains a class definition, extract its type.
                    var (componentType, nameSpace) = await ComponentHelper.ExtractComponentTypeAsync(fileContent, options.Path);

                    if (componentType == null)
                    {
                        UtilsHelper.Log($"Component type extraction failed. Path: {componentPath}");
                        return;
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
                            string appName = attribute.PageName;

                            componentInfoList.Add(new RoutePageInfo
                            {
                                ComponentType = componentType,
                                AppId = appId,
                                IsDefault = isDefault,
                                Component = className,
                                PageName = appName
                            });
                        }
                    }
                }

                #region CodeGenerator

                var groupedComponents = componentInfoList.GroupBy(info => info.AppId).OrderByDescending(f => f.Key);

                var sb = new StringBuilder();
                sb.AppendLine("//add code logic before, after or between these lines depending on your use case");
                sb.AppendLine("SinglePageRoute = await DynamicNavigatorGetStorageItem<DynamicNavigatorRoute>(DynamicNavigatorStorageKeyNameType.Page);");
                sb.AppendLine("if (SinglePageRoute is null || string.IsNullOrWhiteSpace(SinglePageRoute?.Component))");
                sb.AppendLine("{");
                sb.AppendLine("    try");
                sb.AppendLine("    {");

                sb.AppendLine($"       var componentPath = SinglePageRoute != null ? PageRouteRegistry.DefaultsRoutes[SinglePageRoute.AppId] : PageRouteRegistry.DefaultsRoutes.FirstOrDefault().Value;");
                sb.AppendLine($"       PageRouteContainer.CurrentPageRoute = Type.GetType(componentPath);");
                sb.AppendLine($"       var getPage = PageRouteRegistry.ApplicationRoutes.FirstOrDefault(v => v.Value.ComponentPath.Equals(componentPath));");
                sb.AppendLine($"       var singlePageRoute = new DynamicNavigatorRoute{{AppId = getPage.Value.AppId,AppName = getPage.Value.AppName,Component = getPage.Value.ComponentName}};");

                sb.AppendLine("        await DynamicNavigatorAddStorageItem(DynamicNavigatorStorageKeyNameType.Page, singlePageRoute);");
                sb.AppendLine("    }");
                sb.AppendLine("    catch (Exception ex)");
                sb.AppendLine("    {");
                sb.AppendLine("        Console.WriteLine(ex.ToString());");
                sb.AppendLine("    }");

                sb.AppendLine("}");
                sb.AppendLine("else");
                sb.AppendLine("{");
                sb.AppendLine("    try");
                sb.AppendLine("    {");

                sb.AppendLine("        if (SinglePageRoute.Params.Count != 0)");
                sb.AppendLine("        {");
                sb.AppendLine("            foreach (var item in SinglePageRoute.Params)");
                sb.AppendLine("            {");
                sb.AppendLine("                PageRouteRegistry.ApplicationRoutes[$\"{SinglePageRoute.Component}\"].ComponentParameters[item.Key] = item.Value;");
                sb.AppendLine("            }");
                sb.AppendLine("        }");
                sb.AppendLine(" ");
                sb.AppendLine("        var comInfo = PageRouteRegistry.ApplicationRoutes[$\"{SinglePageRoute.Component}\"];");
                sb.AppendLine("        Type? page = Type.GetType(comInfo.ComponentPath);");
                sb.AppendLine("        PageRouteContainer.CurrentPageRoute = page;");

                sb.AppendLine("    }");
                sb.AppendLine("    catch (Exception)");
                sb.AppendLine("    {");
                sb.AppendLine($"        PageRouteContainer.CurrentPageRoute = Type.GetType(PageRouteRegistry.DefaultsRoutes[SinglePageRoute.AppId]);");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                SaveGeneratedRoutes(sb.ToString(), baseComponent);

                #endregion

            }
            catch (Exception ex)
            {
                UtilsHelper.Log($"******* {ex.Message}", options.Verbose);
            }

        }

        protected private void SaveGeneratedRoutes(string code, string filePath)
        {
            filePath = filePath.Replace('/', '\\');

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                // Find the starting line of the BuildPageRoutes method
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("public override async Task BuildPageRoutes()"))
                    {
                        startIndex = i;
                        break;
                    }
                }

                if (startIndex != -1)
                {
                    // Find the real ending brace of the BuildPageRoutes method
                    int endIndex = UtilsHelper.FindMatchingClosingBrace(lines, startIndex);

                    // Remove existing BuildPageRoutes method
                    if (endIndex != -1)
                    {
                        lines = lines.Take(startIndex).Concat(lines.Skip(endIndex + 1)).ToArray();
                    }
                }

                // Find the index where the BuildPageRoutes method was removed
                int insertIndex = startIndex;

                // Generate the new method code
                StringBuilder newMethodCode = new StringBuilder();
                newMethodCode.AppendLine("public override async Task BuildPageRoutes()");
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

                UtilsHelper.FormatCode(filePath, []);
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
        }

    }
}
