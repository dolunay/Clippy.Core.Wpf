# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade the two .NET Framework 4.8 WPF projects to .NET 10 (`net10.0-windows`).
**Scope**: Two legacy WPF projects (about 1.1K LOC), with a class-library-to-demo-app project reference.

### Selected Strategy
**Bottom-Up (Dependency-First)** — Upgrade from leaf nodes to root applications, tier by tier.
**Rationale**: Two projects with a two-tier dependency graph.

**Dependency graph**:
```
Tier 2: Clippy.Wpf.Demo (WPF demo application)
		 ↓
Tier 1: Wpf.Clippy (WPF class library)
```

**Per-tier summary**:
- **Tier 1 — Wpf.Clippy**: The leaf library; target both net48 and net10.0-windows temporarily. Completion requires both targets to build and the still-net48 demo to continue building against the library.
- **Tier 2 — Clippy.Wpf.Demo**: Depends on Wpf.Clippy; move the app to net10.0-windows and verify it resolves the modern library target.

## Tasks

### 01-prerequisites: Verify .NET upgrade prerequisites

Confirm the .NET 10 SDK and required Windows desktop targeting components are available, and check whether repository SDK pinning in `global.json` permits the selected target. The solution contains two legacy WPF projects, so tooling availability and project-system support are prerequisites before changing either project.

**Done when**: The required SDK/targeting packs are available or any missing prerequisite is clearly identified, and `global.json` does not block the selected .NET target.

---

### 02-sdk-projects: Convert WPF projects to SDK-style

Convert `Wpf.Clippy` and `Clippy.Wpf.Demo` from legacy project format to SDK-style while keeping both on .NET Framework 4.8. The assessment marks both as non-SDK-style; `Wpf.Clippy` also uses `packages.config`, which must be represented as `PackageReference` during conversion. Preserve WPF XAML, application resources, project references, and generated settings/resources behavior.

**Done when**: Both projects use SDK-style project files on net48, package references restore successfully, and the solution builds before any TFM upgrade.

---

### 03-wpf-clippy: Upgrade the WPF class library

Upgrade the Tier 1 `Wpf.Clippy` library after SDK-style conversion. It is the leaf project and has one .NET Framework consumer, `Clippy.Wpf.Demo`; keep net48 and add net10.0-windows during the transition. The assessment reports 9 package references in this project, including 4 package upgrades and 5 packages whose functionality is supplied by the framework; one package security-vulnerability finding must also be resolved. Review its WPF control/XAML and resource handling for the Windows-specific target.

**Done when**: Both net48 and net10.0-windows targets restore and build, recommended/security package updates and framework-provided package removals are addressed, and the still-net48 demo builds against the net48 library target.

---

### 04-wpf-demo: Upgrade the WPF demo application

Upgrade the Tier 2 `Clippy.Wpf.Demo` application to net10.0-windows after the library tier is validated. The assessment identifies a WPF project with no API migration incidents; preserve its application definition, XAML pages, resources, and project reference. Its App.config contains only the old .NET Framework startup selector and its settings collection is empty and unreferenced, so remove those obsolete artifacts rather than carrying them into modern .NET.

**Done when**: The demo restores and builds for net10.0-windows with its project reference resolving to Wpf.Clippy's modern target, and any compilation warnings in the modified project are resolved.

---

### 05-final-validation: Consolidate and validate the upgraded solution

Once the demo has moved to .NET 10, remove the transitional net48 target from `Wpf.Clippy` so both projects finish on net10.0-windows. Run a full solution restore/build and the available automated test suite; assessment found no API compatibility incidents and did not identify a separate test project.

**Done when**: Both projects target net10.0-windows only, the complete solution builds without errors or warnings, and all available tests pass (or the absence of tests is recorded).
