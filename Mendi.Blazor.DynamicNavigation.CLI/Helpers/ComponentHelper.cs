using Mendi.Blazor.DynamicNavigation.CLI.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using System.Xml.Linq;

namespace Mendi.Blazor.DynamicNavigation.CLI.Helpers
{
    public class ComponentHelper
    {
        public static (string ProjectName, string TargetFramework, string DllPath)? GetProjectAssemblyInfo(string directory)
        {
            var csprojPath = Directory.EnumerateFiles(directory, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (csprojPath is null)
            {
                return null;
            }

            var doc = XDocument.Load(csprojPath);
            var ns = doc.Root?.Name.Namespace ?? XNamespace.None;
            var projectName = Path.GetFileNameWithoutExtension(csprojPath);
            var propertyGroup = doc.Descendants(ns + "PropertyGroup").FirstOrDefault();
            if (propertyGroup is null)
            {
                return null;
            }

            var tfmElement = propertyGroup.Element(ns + "TargetFramework");
            var tfmsElement = propertyGroup.Element(ns + "TargetFrameworks");

            string targetFramework;
            if (tfmElement != null && !string.IsNullOrWhiteSpace(tfmElement.Value))
            {
                targetFramework = tfmElement.Value.Trim();
            }
            else if (tfmsElement != null && !string.IsNullOrWhiteSpace(tfmsElement.Value))
            {
                targetFramework = tfmsElement.Value.Split(';', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            }
            else
            {
                return null;
            }

            var projectDir = Path.GetDirectoryName(csprojPath)!;
            var configuration = "Debug";

            var dllPath = Path.Combine(projectDir, "bin", configuration, targetFramework, projectName + ".dll");

            if (!File.Exists(dllPath))
            {
                return null;
            }

            return (projectName, targetFramework, dllPath);
        }

        public static IEnumerable<string> GetRoutableComponents(string directory)
        {
            string attributeName = ConstantHelper.AttributeNavigatorRoutableComponent;
            var csFiles = Directory.EnumerateFiles(directory, "*.razor.cs", SearchOption.AllDirectories);

            foreach (var filePath in csFiles)
            {
                if (HasAttribute(filePath, attributeName))
                {
                    yield return filePath;
                }
            }
        }

        public static string GetBaseComponentByAttribute(CommandOptions option)
        {
            var csFiles = Directory.EnumerateFiles(option.Path, "*.cs", SearchOption.AllDirectories);

            foreach (var filePath in csFiles)
            {
                if (HasAttribute(filePath, ConstantHelper.AttributeNavigatorBaseComponent))
                {
                    return filePath;
                }
            }

            var defaultBase = GetDefaultBaseNavigator(option.Path, ConstantHelper.BaseNavigatorComponentName, option.DryRun);
            return defaultBase;
        }

        public static string GetIndexComponentByAttribute(string directory)
        {
            string attributeName = ConstantHelper.AttributeNavigatorIndexComponent;
            var csFiles = Directory.EnumerateFiles(directory, "*.razor.cs", SearchOption.AllDirectories);

            foreach (var filePath in csFiles)
            {
                if (HasAttribute(filePath, attributeName))
                {
                    return filePath;
                }
            }

            var indexFiles = GetRootRouteRazorPage(directory);
            return indexFiles;
        }

        public static string? GetRootRouteRazorPage(string directory)
        {
            const string target = "@page \"/\"";

            foreach (var file in Directory.EnumerateFiles(directory, "*.razor", SearchOption.AllDirectories))
            {
                // Skip _Imports.razor and similar non-page files if you want
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith("_"))
                    continue;

                // Read lazily line by line
                foreach (var line in File.ReadLines(file))
                {
                    if (line.TrimStart().StartsWith(target, StringComparison.Ordinal))
                    {
                        return file;
                    }
                }
            }

            return null;
        }

        public static string GetDefaultBaseNavigator(string directory, string fileName, bool skip = false)
        {
            var csFiles = Directory.EnumerateFiles(directory, fileName, SearchOption.AllDirectories);

            if (!csFiles.Any())
            {
                if (skip)
                {
                    UtilityHelper.Log($"Dry Run - Skipping BaseNavigator file creation.");
                    return string.Empty;
                }

                var projectInfo = GetProjectAssemblyInfo(directory);
                var baseNavigatorPath = Path.Combine(directory, ConstantHelper.BaseNavigatorComponentName);

                string fileContents =
                @$"
                namespace {projectInfo.Value.ProjectName};

                public class BaseNavigator: BlazorDynamicNavigatorBase
                {{
                }}
                ";

                File.WriteAllText(baseNavigatorPath, fileContents);
                UtilityHelper.FormatCode(baseNavigatorPath, []);
                EnsureImportsInheritsBaseNavigator(directory);
                return baseNavigatorPath;
            }

            return csFiles.FirstOrDefault();
        }

        private static void EnsureImportsInheritsBaseNavigator(string directory)
        {
            var importsPath = Directory.EnumerateFiles(directory, "_Imports.razor", SearchOption.AllDirectories).FirstOrDefault();
            if (importsPath is null)
            {
                return;
            }

            var lines = File.ReadAllLines(importsPath).ToList();
            const string inheritsLine = "@inherits BaseNavigator";
            var exists = lines.Any(l => string.Equals(l.Trim(), inheritsLine, StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                return;
            }

            lines.Add(inheritsLine);
            File.WriteAllLines(importsPath, lines);
        }

        static bool HasAttribute(string filePath, string attributeName)
        {
            string code = File.ReadAllText(filePath);
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();
            var attributeNodes = root.DescendantNodes()
                .OfType<AttributeSyntax>()
                .Where(attr => attr.Name.ToString() == attributeName);
            return attributeNodes.Any();
        }

        public static async Task<(Type? componentType, string? nameSpace)> ExtractComponentTypeAsync(string fileContent, string path)
        {
            try
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
                var root = await syntaxTree.GetRootAsync();
                var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
                if (classDeclaration != null)
                {
                    var className = classDeclaration.Identifier.Text;
                    var namespaceName = (classDeclaration.Parent as NamespaceDeclarationSyntax)?.Name?.ToString();
                    var fullyQualifiedName = string.IsNullOrEmpty(namespaceName) ? className : $"{namespaceName}.{className}";

                    var projectInfo = GetProjectAssemblyInfo(path);
                    if (string.IsNullOrWhiteSpace(projectInfo.Value.DllPath))
                    {
                        Console.WriteLine($">>> Project target assembly could not be found.");
                    }
                    else
                    {
                        var targetAssembly = Assembly.LoadFrom(projectInfo.Value.DllPath);
                        var targetType = targetAssembly?.GetType(fullyQualifiedName);

                        if (targetType != null)
                        {
                            return (targetType, namespaceName);
                        }
                        else
                        {
                            Console.WriteLine($">>> Component type {fullyQualifiedName} not found in the target assembly.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Error extracting component type: {ex.Message}");
            }

            return (null, null);
        }

        public static async Task<string> ExtractComponentClassName(string fileContent, bool withNameSpace = true)
        {
            try
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
                var root = await syntaxTree.GetRootAsync();
                var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

                if (classDeclaration != null)
                {
                    var className = classDeclaration.Identifier.Text;
                    if (!withNameSpace)
                    {
                        return className;
                    }

                    var namespaceName = (classDeclaration.Parent as NamespaceDeclarationSyntax)?.Name?.ToString();
                    var fullyQualifiedName = string.IsNullOrEmpty(namespaceName) ? className : $"{namespaceName}.{className}";
                    return className;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Error extracting component type: {ex.Message}");
            }

            return null;
        }

    }
}
