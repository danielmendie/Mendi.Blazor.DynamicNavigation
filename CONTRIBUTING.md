# Contributing

Thanks for your interest in contributing to Mendi.Blazor.DynamicNavigation!

## How to get started

1. Fork this repository and clone your fork.
2. Install the required .NET SDK (see `global.json` or the project files).
3. Open the solution in your preferred IDE (e.g., Visual Studio, Rider, VS Code).
4. Restore and build:

   ```bash
   dotnet restore
   dotnet build
   ```
5. Run tests:
	```bash
    dotnet test
    ```

### Proposing changes
- Open an issue first for major features or breaking changes.

- For small fixes (typos, minor bugs), you can go straight to a pull request.

- Use a descriptive branch name, e.g. feature/add-xyz or fix/navigation-bug.

### When opening a pull request, please:

- Describe the change and why it is needed.

- Link to any related issues.

- Add or update tests when applicable.

- Update the README or docs when behavior or APIs change.

### Coding guidelines
- Follow standard C# conventions and .NET naming guidelines.

- Keep public APIs consistent with existing patterns in the library.

- Place tests in the corresponding project under Mendi.Blazor.DynamicNavigation.Tests and name test classes after the type or feature under test.

### Running the sample / CLI
- Use the CLI project to generate or update navigation for a sample Blazor app.

- Document any new CLI options or behaviors in the README, including examples.

### Communication
Be respectful in all interactions. For serious issues (security, conduct), please contact the maintainer directly instead of opening a public issue.

