# Progress Details — 02-sdk-projects

## What changed
- Converted `Wpf.Clippy/Wpf.Clippy.csproj` and `Clippy.Wpf.Demo/Clippy.Wpf.Demo.csproj` using the dedicated SDK-style conversion tool, in topological order (library first, demo second).
- Both projects remain targeted at `net48`; no target framework or package version was changed.
- Migrated the library's package entries to `PackageReference` and removed `Wpf.Clippy/packages.config`.
- SDK-style project files retain WPF support; the library's WPF resources and the demo's project reference are included by the converted project configuration.
- Recorded task research and breakdown assessment in `tasks/02-sdk-projects/task.md` and `breakdown-context.md`; saved Visual Studio MSBuild decisions in `scenario-instructions.md`.

## Validation
- Direct Wpf.Clippy build: passed.
- Direct Clippy.Wpf.Demo build: passed.
- Full solution build after library conversion: passed.
- Full solution build after both conversions: passed.
- Debug outputs verified: `Wpf.Clippy/bin/Debug/Wpf.Clippy.dll` and `Clippy.Wpf.Demo/bin/Debug/Clippy.Wpf.Demo.exe` exist.
- Target-framework checks: both converted project files still specify `net48`.
- Package configuration: `Wpf.Clippy/packages.config` no longer exists; its packages are `PackageReference` entries.
- Stub scan: no `// STUB:` markers found in either project's C# files.
- Test discovery: no separate test project was returned; assessment had no test coverage recommendation.
- Builds reported success without warnings.

## Issues and resolution
- Initial output checks looked under `bin/Release`, but the IDE build generated the artifacts under `bin/Debug`; the correct output paths were checked and confirmed.

## Deviations
- None. SDK-style conversion was kept separate from TFM and package-version upgrades as planned.
