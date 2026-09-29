# 03-wpf-clippy: Upgrade the WPF class library

## Objective
Upgrade the leaf `Wpf.Clippy` WPF library to temporarily multi-target net48 and net10.0-windows so the still-net48 demo can consume it during migration. Resolve the assessed package updates and security finding while keeping net48 compatibility intact.

## Scope Inventory
- **Project changed**: `Wpf.Clippy\Wpf.Clippy.csproj` and, only if a target-specific API fix is required, files under `Wpf.Clippy\`.
- **Dependent validation project**: `Clippy.Wpf.Demo\Clippy.Wpf.Demo.csproj` remains net48 and must continue building against the library's net48 target; do not modify it in this task.
- **Framework setup**: `get_project_dependencies` reports no imported local MSBuild targets or central package management. `TargetFramework` is project-local in `Wpf.Clippy.csproj`.
- **Build tool**: Visual Studio MSBuild for WPF/XAML and the net48 target; build the library for both TFMs and then build the full solution to verify the dependent demo.

## Assessment Findings
- `Wpf.Clippy` currently targets net48 and is now SDK-style; assessment reports zero API compatibility incidents and no test-coverage recommendation.
- Package references are declared in `Wpf.Clippy.csproj`; target/package recommendations are:
  - Update `Microsoft.Bcl.AsyncInterfaces` 8.0.0 → 10.0.12.
  - Update `System.Runtime.CompilerServices.Unsafe` 6.0.0 → 6.1.2.
  - Update `System.Text.Encodings.Web` 8.0.0 → 10.0.12.
  - Update `System.Text.Json` 8.0.3 → 10.0.12; this also resolves the assessment's security-vulnerability finding.
  - `System.Buffers` 4.5.1, `System.Memory` 4.5.5, `System.Numerics.Vectors` 4.5.0, `System.Threading.Tasks.Extensions` 4.5.4, and `System.ValueTuple` 4.5.0 are supplied by the modern framework but are still needed for net48; retain them conditionally for net48 only.
- Supported-version lookups returned the recommended versions (10.0.12 or 6.1.2) when queried for both net48 and net10.0-windows.
- The first net48 restore after applying those four package versions exposed NU1605 dependency downgrades: `System.Text.Encodings.Web` 10.0.12 requires System.Buffers ≥4.6.1 and System.Memory ≥4.6.3; `System.Text.Json` 10.0.12 requires System.Threading.Tasks.Extensions ≥4.6.3 and System.ValueTuple ≥4.6.2. System.Memory 4.6.3 in turn requires System.Numerics.Vectors ≥4.6.1. Net48 supported-version lookups confirm 4.6.1, 4.6.3, 4.6.3, 4.6.2, and 4.6.1 respectively. Keep these dependencies in the net48-only group at those versions; exclude them from net10.0-windows where framework functionality supplies them.
- `ClippyViewModel.LoadCharacterData` uses `JsonSerializer.Deserialize<CharacterData>(Stream, JsonSerializerOptions)` over embedded JSON resources. No assessed API migration is required. WPF bitmap resources load through a `pack://application` URI.
- Assessment detects no test project; conversion-task validation previously confirmed the net48 demo builds.

## Execution Constraints
- Use `TargetFrameworks` with net10.0-windows first and net48 second.
- Preserve package availability for net48 and condition the five framework-provided packages so they apply only to net48.
- Keep the four assessed package updates at the recommended versions and avoid unrelated source changes.
- Build both library TFMs, then verify the still-net48 demo and full solution build.

## Done when
- `Wpf.Clippy` targets both net10.0-windows and net48, with conditional package references preserving net48 while avoiding redundant package references on modern .NET.
- All four package updates are applied, and the System.Text.Json vulnerability finding is resolved.
- Both library targets build without warnings; the still-net48 demo and full solution build successfully.
