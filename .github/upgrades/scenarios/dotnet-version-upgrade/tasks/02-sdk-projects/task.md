# 02-sdk-projects: Convert WPF projects to SDK-style

## Objective
Convert both legacy WPF projects to SDK-style without changing either project's current net48 target. Conversion must be performed leaf-to-root, using the dedicated conversion tool and validating each converted project before moving to its dependent.

## Scope Inventory
- **Projects affected**: `Wpf.Clippy\Wpf.Clippy.csproj` (Tier 1 class library), then `Clippy.Wpf.Demo\Clippy.Wpf.Demo.csproj` (Tier 2 WPF app).
- **Distinct concerns**: Preserve WPF/XAML and resource metadata; migrate `Wpf.Clippy\packages.config` to `PackageReference`; retain the existing net48 target and project reference.
- **Ordering**: `get_projects_in_topological_order` returned Wpf.Clippy first, then Clippy.Wpf.Demo.
- **Build approach**: These are WPF/XAML projects; use Visual Studio MSBuild for direct per-project builds. The IDE baseline solution build passed before this task.

## Assessment Findings
- **Wpf.Clippy**: net48, non-SDK-style, 12 assessed issues (1 project-format issue, 1 TFM issue reserved for the later TFM task, 4 package upgrade recommendations, 5 framework-provided package removals, and 1 System.Text.Json 8.0.3 security finding). No API issues or test-coverage recommendation. It is a WPF class library consumed by the demo.
- **Clippy.Wpf.Demo**: net48, non-SDK-style, 2 project-format/TFM issues; no package or API incidents and no test-coverage recommendation. It references Wpf.Clippy.
- `Wpf.Clippy` contains `packages.config` and WPF `Page`/`Resource`/`EmbeddedResource` items. The demo includes `App.xaml`, XAML pages, `.resx`, generated resources/settings, and `App.config`.

## Conversion Constraints
- Use `convert_project_to_sdk_style` for each project; do not rewrite project XML manually.
- Convert one project at a time, starting with Wpf.Clippy; directly build it before converting Clippy.Wpf.Demo.
- Do not change `TargetFramework`/`TargetFrameworks` or package versions in this task. Resolve only conversion-caused issues.
- After conversion, verify `Wpf.Clippy\packages.config` is removed and references are represented in the converted project.

## Done when
- Both projects are SDK-style and remain targeted at net48.
- Each converted project builds directly with Visual Studio MSBuild, and the full solution builds with no new errors or warnings.
- The library's packages.config has been migrated/removed, project reference and WPF items are preserved, and any tests associated with the projects pass (assessment found no test project).
