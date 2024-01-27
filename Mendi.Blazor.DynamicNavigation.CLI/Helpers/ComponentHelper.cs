using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Configuration;
using System.Reflection;

namespace Mendi.Blazor.DynamicNavigation.CLI
{
    public class ComponentHelper
    {
        public static IEnumerable<string> GetRoutableComponents(string directory)
        {
            string attributeName = "NavigatorRoutableComponent";
            var csFiles = Directory.EnumerateFiles(directory, "*.razor.cs", SearchOption.AllDirectories);

            foreach (var filePath in csFiles)
            {
                if (HasAttribute(filePath, attributeName))
                {
                    yield return filePath;
                }
            }
        }

        public static string GetBaseComponetByAttribute(string directory)
        {
            string attributeName = "NavigatorBaseComponent";
            var csFiles = Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories);

            foreach (var filePath in csFiles)
            {
                if (HasAttribute(filePath, attributeName))
                {
                    return filePath;
                }
            }

            var baseComponentPath = GetBaseComponetDefault(directory, "BaseComponent.cs");
            return baseComponentPath;
        }

        public static string GetIndexComponetByAttribute(string directory)
        {
            string attributeName = "NavigatorIndexComponent";
            var csFiles = Directory.EnumerateFiles(directory, "*.razor.cs", SearchOption.AllDirectories);

            foreach (var filePath in csFiles)
            {
                if (HasAttribute(filePath, attributeName))
                {
                    return filePath;
                }
            }

            var baseComponentPath = GetBaseComponetDefault(directory, "BaseComponent.cs");
            return baseComponentPath;
        }

        public static string GetBaseComponetDefault(string directory, string fileName)
        {
            var csFiles = Directory.EnumerateFiles(directory, fileName, SearchOption.AllDirectories);
            return csFiles.FirstOrDefault();
        }

        public static string GetAppSettingTargetPathValue(string filePath)
        {
            try
            {
                var configFilePath = GetBaseComponetDefault(filePath, "DynamicNavigator.config");
                if (configFilePath == null)
                {
                    Console.WriteLine(">>> 'DynamicNavigator.config' file not found in project directory.");
                    return null;
                }

                var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };
                var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                // Access settings in appSettings section
                var configOption = config.AppSettings.Settings["ProjectsTargetAssemblyPath"]?.Value;
                if (configOption != null)
                {
                    return configOption;
                }
                else
                {
                    Console.WriteLine(">>> 'ProjectsTargetAssemblyPath' config value is missig'.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Error reading appSettings.json: {ex.Message}");
            }

            return null;
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

        public static async Task<Type?> ExtractComponentTypeAsync(string fileContent, string path)
        {
            try
            {
                // Parse the syntax tree from the file content
                var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
                var root = await syntaxTree.GetRootAsync();

                // Find the first class declaration
                var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

                if (classDeclaration != null)
                {
                    // Get the class name
                    var className = classDeclaration.Identifier.Text;

                    // Get the namespace name (optional)
                    var namespaceName = (classDeclaration.Parent as NamespaceDeclarationSyntax)?.Name?.ToString();

                    // Build the fully qualified name
                    var fullyQualifiedName = string.IsNullOrEmpty(namespaceName) ? className : $"{namespaceName}.{className}";

                    var targetAssemblyPath = GetAppSettingTargetPathValue(path);
                    if (string.IsNullOrWhiteSpace(targetAssemblyPath))
                    {
                        Console.WriteLine($">>> Project target assembly could not be found.");
                    }
                    else
                    {
                        // Load the target assembly dynamically
                        var targetAssembly = Assembly.LoadFrom(targetAssemblyPath);

                        // Try to find the type in the target assembly
                        var targetType = targetAssembly?.GetType(fullyQualifiedName);

                        if (targetType != null)
                        {
                            return targetType;
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

            return null;
        }

        public static async Task<string> ExtractComponentClassName(string fileContent)
        {
            try
            {
                // Parse the syntax tree from the file content
                var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
                var root = await syntaxTree.GetRootAsync();

                // Find the first class declaration
                var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

                if (classDeclaration != null)
                {
                    // Get the class name
                    var className = classDeclaration.Identifier.Text;

                    // Get the namespace name (optional)
                    var namespaceName = (classDeclaration.Parent as NamespaceDeclarationSyntax)?.Name?.ToString();

                    // Build the fully qualified name
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

        public static async Task<string?> ExtractBaseClassNameAsync(string fileContent)
        {
            try
            {
                // Parse the syntax tree from the file content
                var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
                var root = await syntaxTree.GetRootAsync();

                // Find the first class declaration
                var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

                if (classDeclaration != null)
                {
                    // Get the class name
                    return classDeclaration.Identifier.Text;
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
