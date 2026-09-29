# 05-final-validation: Consolidate and validate the upgraded solution

## Objective
Remove the transitional `net48` target from the Wpf.Clippy library now that the demo app targets net10.0-windows, then validate the finished solution.

## Research and scope
- Projects: `Wpf.Clippy` (remove transitional TFM) and `Clippy.Wpf.Demo` (modern consumer already upgraded in task 04).
- Project graph: the demo references Wpf.Clippy. Both now need to target only net10.0-windows.
- Both projects are SDK-style WPF projects with XAML; build via Visual Studio MSBuild using `/restore /t:Build`.
- Prior successful task 04 full-solution build showed the demo and library modern target compile, while the library still additionally built net48.
- Assessment found no test project and no API compatibility incidents.

## Validation steps
1. Remove `net48`-only package references/conditions from Wpf.Clippy if the project file retains them after the TFM switch; keep only dependencies required by net10.0-windows.
2. Restore/build the library and full solution with Visual Studio MSBuild.
3. Verify both project files and assets/outputs target only net10.0-windows and there are no warnings.
4. Discover/run tests if any exist and record that no test project is found if applicable.

## Done when
- Both WPF projects target net10.0-windows only.
- Full solution restore/build succeeds warning-free, and all available tests pass (or absence is documented).
