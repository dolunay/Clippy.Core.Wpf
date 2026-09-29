## Detected Hints

### hint: multi-project-dependency-ordering
- **Status**: resolved
- **Priority**: MUST (only when there are 3+ projects in the chain)
- **Evidence**: This task covers two linked projects; the topological order is Wpf.Clippy followed by Clippy.Wpf.Demo.
- **Detected**: During task 02-sdk-projects research

### hint: large-package-replacement-batch
- **Status**: resolved
- **Priority**: SHOULD
- **Evidence**: No incompatible package replacements are part of this SDK-format-only task; package-version changes belong to the later library upgrade task.
- **Detected**: During task 02-sdk-projects research

### hint: system-web-dependency
- **Status**: resolved
- **Priority**: SHOULD
- **Evidence**: Both projects are WPF; assessment and project metadata do not indicate System.Web dependencies.
- **Detected**: During task 02-sdk-projects research

### hint: windows-api-isolation
- **Status**: resolved
- **Priority**: SHOULD
- **Evidence**: SDK-style conversion only; no Windows-native API incompatibility signal was found in the assessment.
- **Detected**: During task 02-sdk-projects research

## Breakdown Decisions

### task: 02-sdk-projects
- Not decomposed. Only two projects require format conversion; the SDK-style conversion guidance prescribes one task with sequential conversions for 1-3 projects. Keep leaf-to-root ordering and validate each project before proceeding.

### task: 03-wpf-clippy
- Not decomposed. One project has a bounded multi-targeting/package scope with explicit assessment recommendations and supported versions; no unknown replacement package, System.Web, API, or test-project work was detected. Validate both targets and the net48 consumer as gates within the tier task.

### task: 04-wpf-demo
- Not decomposed. One WPF application has a single TFM change and no package/API incidents; the dependency library is already upgraded and validated. Empty unused Framework settings/config files are identified for cleanup within the same project migration.
