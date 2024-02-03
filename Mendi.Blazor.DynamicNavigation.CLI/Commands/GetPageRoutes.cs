using Mendi.Blazor.DynamicNavigation.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Reflection;
using System.Text;

namespace Mendi.Blazor.DynamicNavigation.CLI
{
    public class GetPageRoutes
    {

        public async Task Execute(string path = null)
        {
            // Implement the logic for adding page routes
            Console.WriteLine(">>> Mendi SPA Navigation Engine <<<");

            if (string.IsNullOrWhiteSpace(path))
                path = Directory.GetCurrentDirectory();

            Console.WriteLine($">>> Project directory: {path}");
            Console.WriteLine($">>> Searching for routable components....");

            var routableComponents = ComponentHelper.GetRoutableComponents(path);
            if (routableComponents == null)
            {
                Console.WriteLine(">>> No routable components found");
            }
            else
            {
                Console.WriteLine($">>> Routable components found({routableComponents.Count()})");
                Console.WriteLine($">>> Searching for base components...");

                var baseComponent = ComponentHelper.GetBaseComponetByAttribute(path);
                if (string.IsNullOrWhiteSpace(baseComponent))
                    baseComponent = ComponentHelper.GetBaseComponetDefault(path, "BaseComponent.cs");

                if (string.IsNullOrWhiteSpace(baseComponent))
                {
                    Console.WriteLine(">>> No BaseComponent file found in project directory");
                    return;
                }

                await GenerateRoutes(routableComponents, path);

                Console.WriteLine(">>> Scaffold GetPageRoutes completed <<<");
            }
        }

        private async Task GenerateRoutes(IEnumerable<string> routeFilePaths, string path)
        {
            try
            {
                var basePath = ComponentHelper.GetBaseComponetByAttribute(path);
                if (string.IsNullOrWhiteSpace(basePath))
                {
                    Console.WriteLine($">>> BaseComponent.cs file not found in project directory");
                    return;
                }

                //read base content
                var baseContent = File.ReadAllText(basePath);
                // Assuming .razor.cs file contains a class definition, you can extract its type.
                var baseClassName = await ComponentHelper.ExtractBaseClassNameAsync(baseContent);

                if (baseClassName == null)
                {
                    Console.WriteLine($">>> Failed to extract type from the base component. Path: {basePath}. Be sure that the project has been build and the specified component is included");
                    return;
                }

                #region CodeGenerator


                List<RoutableComponentInfo> componentInfoList = [];
                foreach (var componentPath in routeFilePaths)
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

                var sb = new StringBuilder();
                sb.AppendLine(" PageRouteRegistry = new DynamicNavigatorRegistry");
                sb.AppendLine("{");
                sb.AppendLine("    ApplicationRoutes = new()");
                sb.AppendLine("    {");

                var totalComs = componentInfoList.Count();
                var loopComs = 1;
                foreach (var component in componentInfoList)
                {
                    var properties = component.ComponentType.GetProperties()
                        .Where(p => p.GetCustomAttributes<ParameterAttribute>().Any())
                        .Select(p => p);

                    sb.AppendLine($"    {{");
                    sb.AppendLine($"        nameof({component.Name}),");
                    sb.AppendLine($"        new DynamicNavigatorMetadata");
                    sb.AppendLine($"        {{");
                    sb.AppendLine($"            AppId = {component.AppId},");
                    sb.AppendLine($"            AppName =  \"{component.AppName}\",");
                    sb.AppendLine($"            ComponentName =  nameof({component.Name}),");

                    if (properties.Any())
                    {
                        sb.AppendLine($"            ComponentPath =  \"{component.FullName}\",");
                        sb.AppendLine($"            ComponentParameters = new()");
                        sb.AppendLine($"            {{");

                        var totalProps = properties.Count();
                        var loopProps = 1;
                        foreach (var property in properties)
                        {
                            var attributes = property.GetCustomAttributes(typeof(NavigatorClickEventAttribute), false);

                            if (attributes.Any())
                            {
                                if (property.PropertyType.IsGenericType)
                                {
                                    Type genericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                                    if (genericTypeDefinition == typeof(EventCallback<>))
                                    {
                                        Type innerType = property.PropertyType.GetGenericArguments()[0];

                                        // Check if the inner type is Dictionary<string, string>
                                        if (innerType == typeof(Dictionary<string, string>))
                                        {
                                            foreach (NavigatorClickEventAttribute attribute in attributes)
                                            {
                                                string nextRoutablePage = attribute.NextRoutablePage;
                                                sb.AppendLine($"                {{");
                                                sb.AppendLine($"                    \"{property.Name}\", EventCallback.Factory.Create<Dictionary<string, string>>(this, e => OnMapPageItemClicked(e, nameof({nextRoutablePage})))");
                                                if (loopProps != totalProps)
                                                {
                                                    sb.AppendLine($"                }},");
                                                }
                                                else
                                                {
                                                    sb.AppendLine($"                }}");
                                                }
                                            }
                                        }
                                    }
                                }
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
                    else
                    {
                        sb.AppendLine($"            ComponentPath =  \"{component.FullName}\"");
                    }

                    sb.AppendLine($"        }}");
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

                sb.AppendLine("    },");
                sb.AppendLine("    DefaultsRoutes = new()");
                sb.AppendLine("    {");

                var groupedComponents = componentInfoList.GroupBy(info => info.AppId).OrderByDescending(f => f.Key);
                var totalApps = groupedComponents.Count();
                var loopTrail = 1;
                foreach (var group in groupedComponents)
                {
                    var defaultAppComponent = group.FirstOrDefault(info => info.IsDefault) ?? group.First();
                    if (totalApps == 1)
                    {
                        sb.AppendLine("    {");
                        sb.AppendLine($"        {defaultAppComponent.AppId}, \"{defaultAppComponent.FullName}\"");
                        sb.AppendLine("    }");
                    }
                    else
                    {
                        if (loopTrail != totalApps)
                        {
                            sb.AppendLine("    {");
                            sb.AppendLine($"        {defaultAppComponent.AppId}, \"{defaultAppComponent.FullName}\"");
                            sb.AppendLine("    },");
                        }
                        else
                        {
                            sb.AppendLine("    {");
                            sb.AppendLine($"        {defaultAppComponent.AppId}, \"{defaultAppComponent.FullName}\"");
                            sb.AppendLine("    }");
                        }
                    }

                    loopTrail++;
                }

                sb.AppendLine("    }");
                sb.AppendLine("};");
                sb.AppendLine("await DynamicNavigatorIndexDbAddValue(DynamicNavigatorIndexDbKeyTypes.Routes, PageRouteRegistry);");
                sb.AppendLine("await BuildPageRoutes();");

                CreatePageRouteContainer(basePath);
                CreatePageRouteRegistry(basePath);
                CreateSinglePageRoute(basePath);
                await CreateOnBackToPreviousPageClicked(basePath, path);
                await CreateOnMapNavMenuClicked(basePath, path);
                await CreateOnMapPageItemClicked(basePath, path);
                SaveGeneratedRoutes(sb.ToString(), basePath);

                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine($"<<<-------------------- {ex.Message} ----------------------->>>");
            }
        }

        private protected async Task CreateOnMapNavMenuClicked(string filePath, string path)
        {
            var sort = filePath;
            filePath = filePath.Replace('/', '\\');

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                // Find the starting line of the GetPageRoutes method
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("OnMapNavMenuClicked(string page)"))
                    {
                        startIndex = i;
                        break;
                    }
                }

                if (startIndex != -1) return;
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

                //read file content
                var fileContent = File.ReadAllText(sort);

                // Assuming .razor.cs file contains a class definition, extract its type.
                var className = await ComponentHelper.ExtractComponentClassName(fileContent);

                // Find the index where the GetPageRoutes method was removed
                int insertIndex = startIndex;

                // Generate the new method code
                StringBuilder newMethodCode = new StringBuilder();
                newMethodCode.AppendLine("public async Task OnMapNavMenuClicked(string page)");
                newMethodCode.AppendLine("{");
                newMethodCode.AppendLine($"  PageRouteContainer = await OnNavMenuItemCliked(page, PageRouteContainer, PageRouteRegistry, typeof({className}));");
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

        private protected async Task CreateOnMapPageItemClicked(string filePath, string path)
        {
            var sort = filePath;
            filePath = filePath.Replace('/', '\\');

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                // Find the starting line of the GetPageRoutes method
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("OnMapPageItemClicked(Dictionary<string, string> parameters, string nextPage)"))
                    {
                        startIndex = i;
                        break;
                    }
                }

                if (startIndex != -1) return;
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

                //read file content
                var fileContent = File.ReadAllText(sort);

                // Assuming .razor.cs file contains a class definition, extract its type.
                var className = await ComponentHelper.ExtractComponentClassName(fileContent);

                if (className == null)
                {
                    Console.WriteLine($">>> Failed to extract component type from the file content. Path: {filePath}");
                    return;
                }

                // Find the index where the GetPageRoutes method was removed
                int insertIndex = startIndex;

                // Generate the new method code
                StringBuilder newMethodCode = new StringBuilder();
                newMethodCode.AppendLine("public async Task OnMapPageItemClicked(Dictionary<string, string> parameters, string nextPage)");
                newMethodCode.AppendLine("{");
                newMethodCode.AppendLine($"  PageRouteContainer = await OnPageItemClicked(parameters, nextPage, PageRouteContainer, PageRouteRegistry, typeof({className}));");
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

        private protected async Task CreateOnBackToPreviousPageClicked(string filePath, string path)
        {
            var sort = filePath;
            filePath = filePath.Replace('/', '\\');

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                // Find the starting line of the GetPageRoutes method
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("OnMapBackToPreviousPageClicked()"))
                    {
                        startIndex = i;
                        break;
                    }
                }

                if (startIndex != -1) return;
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

                //read file content
                var fileContent = File.ReadAllText(sort);

                // Assuming .razor.cs file contains a class definition, extract its type.
                var className = await ComponentHelper.ExtractComponentClassName(fileContent);

                if (className == null)
                {
                    Console.WriteLine($">>> Failed to extract component type from the file content. Path: {filePath}");
                    return;
                }

                // Find the index where the GetPageRoutes method was removed
                int insertIndex = startIndex;

                // Generate the new method code
                StringBuilder newMethodCode = new StringBuilder();
                newMethodCode.AppendLine("public async Task OnMapBackToPreviousPageClicked()");
                newMethodCode.AppendLine("{");
                newMethodCode.AppendLine($"  PageRouteContainer = await OnBackToPreviousPageClicked(PageRouteContainer, PageRouteRegistry, typeof({className}));");
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
                    if (lines[i].Contains("public DynamicNavigatorRegistry PageRouteRegistry { get; set; }"))
                    {
                        startIndex = i;
                        break;
                    }
                }

                if (startIndex != -1) return;
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
                newMethodCode.AppendLine("[Inject] public DynamicNavigatorRegistry PageRouteRegistry { get; set; } = null!;");

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

        private protected static void CreateSinglePageRoute(string filePath)
        {
            filePath = filePath.Replace('/', '\\');

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                // Find the starting line of the GetPageRoutes method
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("public DynamicNavigatorRoute SinglePageRoute { get; set; }"))
                    {
                        startIndex = i;
                        break;
                    }
                }

                if (startIndex != -1) return;
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
                newMethodCode.AppendLine("public DynamicNavigatorRoute SinglePageRoute { get; set; } = null!;");

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

        private protected static void CreatePageRouteContainer(string filePath)
        {
            filePath = filePath.Replace('/', '\\');

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                // Find the starting line of the GetPageRoutes method
                int startIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("public DynamicNavigatorContainer PageRouteContainer { get; set; }"))
                    {
                        startIndex = i;
                        break;
                    }
                }

                if (startIndex != -1) return;
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
                newMethodCode.AppendLine("[Inject] public DynamicNavigatorContainer PageRouteContainer { get; set; } = null!;");

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
                    if (lines[i].Contains("public override async Task GetPageRoutes()"))
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
                newMethodCode.AppendLine("public override async Task GetPageRoutes()");
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
                    "Microsoft.AspNetCore.Components",
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
