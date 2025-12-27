using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Diagnostics;

namespace Mendi.Blazor.DynamicNavigation.CLI.Helpers
{
    public class UtilityHelper
    {
        public static int FindMatchingClosingBrace(string[] lines, int startIndex)
        {
            int braceCount = 0;

            for (int i = startIndex; i < lines.Length; i++)
            {
                foreach (char c in lines[i])
                {
                    if (c == '{')
                    {
                        braceCount++;
                    }
                    else if (c == '}')
                    {
                        braceCount--;

                        if (braceCount == 0)
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }

        public static CompilationUnitSyntax AddMissingUsings(CompilationUnitSyntax root, List<string> namespaces)
        {
            var existingUsings = root.Usings.Select(u => u.Name.ToString());
            var missingUsings = namespaces.Except(existingUsings);
            if (missingUsings.Any())
            {
                root = root.AddUsings(
                    missingUsings.Select(namespaceName =>
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName)))
                        .ToArray());
            }

            return root;
        }

        public static void Log(string message, bool verbose = true)
        {
            if (verbose)
            {
                Console.WriteLine($"{DateTime.Now:hh:ss tt}: {message}");
            }
        }

        public static void FormatCode(string filePath, List<string> projectNameSpaces, bool includeComponentNamespace = false)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath);

                // Parse the syntax tree from the file content
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
                CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

                var namespaces = new List<string>()
                {
                    new("Mendi.Blazor.DynamicNavigation"),
                };

                if (includeComponentNamespace)
                {
                    namespaces.Add("Microsoft.AspNetCore.Components");
                }

                if (projectNameSpaces is not null && projectNameSpaces.Count != 0)
                    namespaces.AddRange(projectNameSpaces.Distinct());

                // Add missing using statements
                var usings = UtilityHelper.AddMissingUsings(root, namespaces);

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

        public static bool BuildProject(string projectPath, string configuration = "Debug")
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build \"{projectPath}\" --configuration {configuration}",
                WorkingDirectory = Path.GetDirectoryName(projectPath) ?? projectPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                return false;
            }

            // Optional: read logs if you want to show or store them
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            // ExitCode 0 = success
            return process.ExitCode == 0;
        }
    }
}