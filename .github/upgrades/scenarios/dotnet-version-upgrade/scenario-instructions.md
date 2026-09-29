# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0
- **Commit Strategy**: After Each Task

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: Bottom-Up

### Project Structure
- Project Approach: Class library multi-targeting during migration; WPF demo application upgraded in-place

## Strategy
**Selected**: Bottom-Up (Dependency-First)
**Rationale**: Two .NET Framework projects form a two-tier dependency graph; upgrade and validate the library before its WPF demo consumer.

### Execution Constraints
- Convert legacy project files separately from TFM changes.
- Upgrade the leaf library before the dependent WPF demo.
- Keep the library targeting net48 and net10.0-windows while the demo remains on .NET Framework.
- After the demo moves to net10.0-windows, remove the transitional net48 target from the library.
- Build and test each tier, including a check that the still-net48 demo builds after the library tier.

## Decisions
- Use net10.0 as the selected modern .NET target; WPF projects will use the Windows-specific net10.0-windows TFM.
- Multi-target Wpf.Clippy during the migration so the .NET Framework demo can consume it until the demo is upgraded.
- **NuGet package compatibility**: WPF.Clippy should continue supporting `net8.0-windows` and `net9.0-windows`, in addition to `net10.0-windows` (user decision, 2026-10-04).

## Build Tool Decisions
- **Wpf.Clippy.csproj**: Visual Studio MSBuild with `/restore /t:Build` for WPF/XAML and all target frameworks; the IDE build restored only the selected TFM during multi-targeting.
- **Clippy.Wpf.Demo.csproj**: Visual Studio MSBuild for WPF/XAML builds.
