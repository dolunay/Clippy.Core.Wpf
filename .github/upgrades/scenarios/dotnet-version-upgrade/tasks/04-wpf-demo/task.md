# 04-wpf-demo: Upgrade the WPF demo application

## Objective
Retarget the SDK-style WPF demo from net48 to net10.0-windows and consume the modern target of `Wpf.Clippy`, which currently multi-targets net10.0-windows and net48.

## Scope Inventory
- **Project changed**: `Clippy.Wpf.Demo\Clippy.Wpf.Demo.csproj` and `Clippy.Wpf.Demo\App.xaml.cs` (only for removing obsolete unused imports).
- **Dependency**: `Wpf.Clippy\Wpf.Clippy.csproj` is already validated for both TFMs; do not modify it here.
- **Property location**: `get_project_dependencies` found no imported local MSBuild targets; `<TargetFramework>` is defined in the demo project file.
- **Packages**: No NuGet packages are declared for the demo; project dependencies are the Windows Desktop SDK and the Wpf.Clippy project reference.
- **Build approach**: Visual Studio MSBuild `/restore /t:Build` is required for WPF/XAML. Build the project, then the full solution.

## Assessment and Source Findings
- Assessment: demo currently net48, SDK-style, ClassicWpf, with only the mandatory TFM issue; 0 package issues, 0 API incidents, and no test-coverage recommendation.
- Project reference points to `Wpf.Clippy`; the library's net10.0-windows target is available, and its net48 target is retained until final cleanup.
- The app uses `App.xaml`, `MainWindow.xaml`, and a `.resx` resources designer; these must compile under the Windows-specific TFM.
- `App.config` contains only the .NET Framework 4.8 `<supportedRuntime>` selector. The SDK supplies a runtimeconfig for modern .NET; there are no app-specific settings in that file.
- `Settings.settings` has an empty `<Settings />` collection; its generated `Settings.Designer.cs` is the only `System.Configuration.ApplicationSettingsBase` usage, with no application code references to `Settings.Default`. These unused .NET Framework artifacts can be removed rather than adding a configuration package.
- The demo's `App.xaml.cs` imports several namespaces that are unused; retain only the `System.Windows` import if that file is changed.
- No `// STUB:`, System.Web, P/Invoke, Registry, System.Drawing, or Windows Forms usages were found in demo C# files.

## Execution Constraints
- Change the project TFM to `net10.0-windows` (keep the singular `TargetFramework` property because the demo is not multi-targeted).
- Remove legacy explicit framework assembly references if they cause duplicate-reference warnings; SDK framework references provide them.
- Keep WPF, app definition, XAML views, resources, and the project reference intact.
- Remove only the empty generated settings artifacts and the obsolete Framework-only startup config; do not add unnecessary configuration packages.

## Done when
- The demo targets net10.0-windows and resolves Wpf.Clippy's modern target.
- WPF/XAML, application resources, and resource designer compile; old-only empty settings/startup config artifacts are not carried forward.
- The demo and full solution build without errors or warnings, and any available tests pass (assessment found no test project).
