# Progress Details — 03-wpf-clippy

## What changed
- Updated `Wpf.Clippy/Wpf.Clippy.csproj` to multi-target `net10.0-windows;net48`, with the modern target first.
- Updated `Microsoft.Bcl.AsyncInterfaces` to 10.0.12, `System.Runtime.CompilerServices.Unsafe` to 6.1.2, `System.Text.Encodings.Web` to 10.0.12, and `System.Text.Json` to 10.0.12. The System.Text.Json update addresses the assessment security finding.
- Kept the five framework-provided packages in the net48 conditional group only. Raised net48 transitive dependency pins to System.Buffers 4.6.1, System.Memory 4.6.3, System.Numerics.Vectors 4.6.1, System.Threading.Tasks.Extensions 4.6.3, and System.ValueTuple 4.6.2 to satisfy the updated JSON package requirements.
- Removed redundant explicit references to System.Data.DataSetExtensions, Microsoft.CSharp, and System.Net.Http; the SDK framework references supply them and their presence caused MSB3243 warnings on net10.0-windows.
- Updated task research, breakdown context, and the cached MSBuild decision in scenario instructions.

## Validation
- Visual Studio MSBuild `/restore /t:Build` for `Wpf.Clippy.csproj`: passed for net10.0-windows and net48 with no warnings.
- Full solution Visual Studio MSBuild `/restore /t:Build`: passed. Both Wpf.Clippy targets built and the still-net48 Clippy.Wpf.Demo built successfully against net48.
- Verified `project.assets.json` contains both target graphs. The net48 graph retains all required package references; the net10.0-windows graph contains only the four upgraded packages.
- Verified outputs exist for net10.0-windows and net48 library builds and the net48 demo.
- Tests: no test project found in the assessed solution; no test-coverage recommendation was made.

## Issues and resolution
- Initial global package updates produced NU1605 downgrades for dependencies of System.Text.Encodings.Web/System.Text.Json. Updated the net48 dependency pins to the required supported versions.
- The first IDE build after multi-targeting had assets only for the selected net10.0-windows target (NETSDK1005 and dependent NU1201). An explicit Visual Studio MSBuild restore/build restored both TFMs.
- That build reported MSB3243 for three redundant explicit framework references; removed the references and revalidated both TFMs and the solution warning-free.

## Deviations
- None. The library remains multi-targeted during the transition; `net48` will be removed after the demo app upgrades.
