# 01-prerequisites: Verify .NET upgrade prerequisites

## Objective
Verify the .NET 10 SDK and Windows desktop build prerequisites, and confirm repository SDK pinning will not block the selected target before changing project files.

## Research Findings
- `validate_dotnet_sdk_installation` reports a compatible SDK for `net10.0`.
- `validate_dotnet_sdk_in_globaljson` reports no `global.json` configuration to constrain SDK selection.
- The solution contains two legacy WPF projects: leaf class library `Wpf.Clippy` and dependent application `Clippy.Wpf.Demo`; their WPF/XAML flavor requires Visual Studio MSBuild for reliable build validation.
- No source-code or project-file changes are in scope for this prerequisite task. The repository has no repo-root `.github/copilot-instructions.md`.

## Scope Inventory
- **Projects affected**: None; this is a tooling-readiness check for both WPF projects.
- **Distinct concerns**: .NET SDK availability, SDK pinning, baseline solution build.
- **Assessment signals**: Both projects target net48 and use non-SDK-style project files; the dependency graph has two tiers and no API incidents.

## Done when
- A compatible .NET 10 SDK is available.
- `global.json` does not prevent the selected target (or the absence of the file is confirmed).
- Baseline solution build status is recorded.
