using Mendi.Blazor.DynamicNavigation.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Text;

namespace Mendi.Blazor.DynamicNavigation.CLI
{
    public class BuildPageRoutes
    {

        public async Task Execute(bool force = false, string path = null)
        {
            // Implement the logic for adding page routes
            Console.WriteLine(">>> Mendi Blazor Dynamic Navigation Engine <<<");

            if (string.IsNullOrWhiteSpace(path))
                path = Directory.GetCurrentDirectory();

            Console.WriteLine($">>> Project directory: {path}");
            Console.WriteLine($">>> Searching for base components....");

            var baseComponent = ComponentHelper.GetBaseComponetByAttribute(path);
            if (string.IsNullOrWhiteSpace(baseComponent))
                baseComponent = ComponentHelper.GetBaseComponetDefault(path, "BaseComponent.cs");

            if (string.IsNullOrWhiteSpace(baseComponent))
            {
                Console.WriteLine(">>> No Base Component file found in project directory");
                return;
            }

            await BuildRoutes(baseComponent, path, force);

            Console.WriteLine(">>> Scaffold BuildPageRoutes completed <<<");
        }

        private async Task BuildRoutes(string basePath, string path, bool isForce)
        {
            string[] lines = File.ReadAllLines(basePath);

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

            if (startIndex != -1 && !isForce)
            {
                //The BuildPageRoute method if exists might have a devs custom code, so prevent scaffolding which will remove every
                //custom code and prompt the dev for response to carry on
                Console.WriteLine(">>> BuildPageRoutes exists already. To explicitly force a new scaffold without prompt, specify the -f or --force parameter");
                Console.WriteLine(">>> Do you want to continue with scaffold? (y/n)");
                var responseInput = Console.ReadLine();
                if (responseInput.ToLower().Equals("y"))
                {
                    Console.WriteLine(">>> Continuing to scaffolding... custom dev code may be lost");
                    goto ContinueProcess;
                }
                else
                {
                    return;
                }
            }

ContinueProcess:
            var helper = new ComponentHelper();
            var routableComponents = ComponentHelper.GetRoutableComponents(path);

            try
            {
                List<RoutableComponentInfo> componentInfoList = [];

                foreach (var componentPath in routableComponents)
                {
                    //read file content
                    var fileContent = File.ReadAllText(componentPath);

                    // Assuming .razor.cs file contains a class definition, extract its type.
                    var componentType = await ComponentHelper.ExtractComponentTypeAsync(fileContent, path);

                    if (componentType == null)
                    {
                        Console.WriteLine($">>> Failed to extract component type from the file content. Path: {componentPath}");
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
                            bool isDefault = attribute.IsDefaultPage;
                            string appName = attribute.AppName;

                            componentInfoList.Add(new RoutableComponentInfo
                            {
                                ComponentType = componentType,
                                AppId = appId,
                                IsDefault = isDefault,
                                FullName = fullyQualifiedName,
                                Name = className,
                                AppName = appName
                            });
                        }
                    }
                }

                #region CodeGenerator

                var groupedComponents = componentInfoList.GroupBy(info => info.AppId).OrderByDescending(f => f.Key);

                var sb = new StringBuilder();
                sb.AppendLine("SinglePageRoute = await IndexDbGetValue<DynamicNavigatorRoute>(IndexDbKeyTypes.Page);");
                sb.AppendLine("if (SinglePageRoute is null || string.IsNullOrWhiteSpace(SinglePageRoute?.Component))");
                sb.AppendLine("{");
                sb.AppendLine("    try");
                sb.AppendLine("    {");

                sb.AppendLine($"       var componentPath = SinglePageRoute != null ? PageRouteRegistry.DefaultsRoutes[SinglePageRoute.AppId] : PageRouteRegistry.DefaultsRoutes.FirstOrDefault().Value;");
                sb.AppendLine($"       PageRouteContainer.CurrentPageRoute = Type.GetType(componentPath);");
                sb.AppendLine($"       var getPage = PageRouteRegistry.ApplicationRoutes.FirstOrDefault(v => v.Value.ComponentPath.Equals(componentPath));");
                sb.AppendLine($"       var singlePageRoute = new DynamicNavigatorRoute{{AppId = getPage.Value.AppId,AppName = getPage.Value.AppName,Component = getPage.Value.ComponentName}};");

                sb.AppendLine("        await IndexDbAddValue(IndexDbKeyTypes.Page, singlePageRoute);");
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

                sb.AppendLine("        if (SinglePageRoute.Params.Any())");
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

                SaveGeneratedRoutes(sb.ToString(), basePath);

                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine($"<<<-------------------- {ex.Message} ----------------------->>>");
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

                FormatCode(filePath);
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
        }

        private static void FormatCode(string filePath)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath);

                // Parse the syntax tree from the file content
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
                CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

                // Add missing using statements
                var usings = UtilsHelper.AddMissingUsings(root,
                [
                    "Mendi.Blazor.DynamicNavigation"
                ]);

                // Format the syntax tree using Roslyn's Formatter
                SyntaxNode formattedRoot = Formatter.Format(usings, new AdhocWorkspace());
                SyntaxNode cleanedRoot = formattedRoot.NormalizeWhitespace();

                // Write the formatted content back to the file
                File.WriteAllText(filePath, cleanedRoot.ToFullString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error formatting code: {ex.Message}");
            }
        }

    }


}
