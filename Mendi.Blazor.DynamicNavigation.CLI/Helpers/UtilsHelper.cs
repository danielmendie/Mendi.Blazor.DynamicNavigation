using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mendi.Blazor.DynamicNavigation.CLI
{
    public class UtilsHelper
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
            // Find existing using directives
            var existingUsings = root.Usings.Select(u => u.Name.ToString());

            // Identify missing using directives
            var missingUsings = namespaces.Except(existingUsings);

            // Add missing using directives
            if (missingUsings.Any())
            {
                root = root.AddUsings(
                    missingUsings.Select(namespaceName =>
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName)))
                        .ToArray());
            }

            return root;
        }
    }
}
