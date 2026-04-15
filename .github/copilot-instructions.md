# .NET Repository

**Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively

### Essential Build Commands
- **Restore dependencies**: `dotnet restore`

- **Build the entire solution**: `dotnet build`

- **Run tests**: `dnx --yes retest`
  - Runs all unit tests across the solution
  - If tests fail due to Azure Storage, run the following commands and retry: `npm install azurite` and `npx azurite &`

### Build Validation and CI Requirements
- **Always run before committing**: 
  * `dnx --yes retest`
  * `dotnet format whitespace -v:diag --exclude ~/.nuget`
  * `dotnet format style -v:diag --exclude ~/.nuget`

### Project Structure and Navigation

| Directory | Description |
|-----------|-------------|
| `src/` | Contains the repo source code. |
| `bin/` | Contains built packages (if any) |

### Code Style and Formatting

#### EditorConfig Rules
The repository uses `.editorconfig` at the repo root for consistent code style.

- **Indentation**: 4 spaces for C# files, 2 spaces for XML/YAML/JSON
- **Line endings**: LF (Unix-style)
- **Sort using directives**: System.* namespaces first (`dotnet_sort_system_directives_first = true`)
- **Type references**: Prefer language keywords over framework type names (`int` vs `Int32`)
- **Modern C# features**: Use object/collection initializers, coalesce expressions when possible, use var when the type is apparent from the right-hand side of the assignment
- **Visibility modifiers**: only explicitly specify visibility when different from the default (e.g. `public` for classes, no `internal` for classes or `private` for fields, etc.)

#### Formatting Validation
- CI enforces formatting with `dotnet format whitespace` and `dotnet format style`
- Run locally: `dotnet format whitespace --verify-no-changes -v:diag --exclude ~/.nuget`
- Fix formatting: `dotnet format` (without `--verify-no-changes`)

### Testing Practices

#### Test Framework
- **xUnit** for all unit and integration tests
- **Moq** for mocking dependencies
- Located in `src/*.Tests/`

#### Test Attributes
Custom xUnit attributes are sometimes used for conditional test execution:
- `[SecretsFact("XAI_API_KEY")]` - Skips test if required secrets are missing from user secrets or environment variables
- `[LocalFact("SECRET")]` - Runs only locally (skips in CI), requires specified secrets
- `[CIFact]` - Runs only in CI environment

### Dependency Management

#### Adding Dependencies
- Add to appropriate `.csproj` file
- Run `dotnet restore` to update dependencies
- Ensure version consistency across projects where applicable

#### CI/CD Pipeline
- **Build workflow**: `.github/workflows/build.yml` - runs on PR and push to main/rel/feature branches
- **Publish workflow**: Publishes to Sleet feed when `SLEET_CONNECTION` secret is available
- **OS matrix**: Configured in `.github/workflows/os-matrix.json` (defaults to ubuntu-latest)

### Special Files and Tools

#### dnx Command
- **Purpose**: built-in tool for running arbitrary dotnet tools that are published on nuget.org. `--yes` auto-confirms install before run.
- **Example**: `dnx --yes retest` - runs tests with automatic retry on transient failures (retest being a tool package published at https://www.nuget.org/packages/retest)
- **In CI**: `dnx --yes retest -- --no-build` (skips build, runs tests only)

#### Directory.Build.rsp
- MSBuild response file with default build arguments
- `-nr:false` - disables node reuse
- `-m:1` - single-threaded build (for stability)
- `-v:m` - minimal verbosity

#### Code Quality
- All PRs must pass format validation
- Tests must pass on all target frameworks
- Follow existing patterns and conventions in the codebase

## Documenting Work

Project implemention details, design and key decisions should be documented in a top-level AGENTS.md file at the repo root. 
Keep this file updated whenever you make change significant changes for future reference.

User-facing features and APIs should be documented to highlight (not extensively, as an overview) key project features and capabilities, in the readme.md file at the repo root.
