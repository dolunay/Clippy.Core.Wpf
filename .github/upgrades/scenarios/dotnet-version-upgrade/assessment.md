# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
  - [Binding Redirect Configuration](#binding-redirect-configuration)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [Clippy.Wpf.Demo\Clippy.Wpf.Demo.csproj](#clippywpfdemoclippywpfdemocsproj)
  - [Wpf.Clippy\Wpf.Clippy.csproj](#wpfclippywpfclippycsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 2 | All require upgrade |
| Total NuGet Packages | 9 | 4 need upgrade |
| Total Code Files | 15 |  |
| Total Code Files with Incidents | 2 |  |
| Total Lines of Code | 1147 |  |
| Total Number of Issues | 14 |  |
| Estimated LOC to modify | 0+ | at least 0,0% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Binding Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| [Clippy.Wpf.Demo\Clippy.Wpf.Demo.csproj](#clippywpfdemoclippywpfdemocsproj) | net48 | 🟢 Low | 0 | 0 | 0 |  | ClassicWpf, Sdk Style = False |
| [Wpf.Clippy\Wpf.Clippy.csproj](#wpfclippywpfclippycsproj) | net48 | 🟢 Low | 10 | 0 | 0 |  | ClassicWpf, Sdk Style = False |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 5 | 55,6% |
| ⚠️ Incompatible | 0 | 0,0% |
| 🔄 Upgrade Recommended | 4 | 44,4% |
| ***Total NuGet Packages*** | ***9*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| Microsoft.Bcl.AsyncInterfaces | 8.0.0 | 10.0.12 | [Wpf.Clippy.csproj](#wpfclippywpfclippycsproj) | NuGet package upgrade is recommended |
| System.Buffers | 4.5.1 |  | [Wpf.Clippy.csproj](#wpfclippywpfclippycsproj) | NuGet package functionality is included with framework reference |
| System.Memory | 4.5.5 |  | [Wpf.Clippy.csproj](#wpfclippywpfclippycsproj) | NuGet package functionality is included with framework reference |
| System.Numerics.Vectors | 4.5.0 |  | [Wpf.Clippy.csproj](#wpfclippywpfclippycsproj) | NuGet package functionality is included with framework reference |
| System.Runtime.CompilerServices.Unsafe | 6.0.0 | 6.1.2 | [Wpf.Clippy.csproj](#wpfclippywpfclippycsproj) | NuGet package upgrade is recommended |
| System.Text.Encodings.Web | 8.0.0 | 10.0.12 | [Wpf.Clippy.csproj](#wpfclippywpfclippycsproj) | NuGet package upgrade is recommended |
| System.Text.Json | 8.0.3 | 10.0.12 | [Wpf.Clippy.csproj](#wpfclippywpfclippycsproj) | NuGet package upgrade is recommended |
| System.Threading.Tasks.Extensions | 4.5.4 |  | [Wpf.Clippy.csproj](#wpfclippywpfclippycsproj) | NuGet package functionality is included with framework reference |
| System.ValueTuple | 4.5.0 |  | [Wpf.Clippy.csproj](#wpfclippywpfclippycsproj) | NuGet package functionality is included with framework reference |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>⚙️&nbsp;Clippy.Wpf.Demo.csproj</b><br/><small>net48</small>"]
    P2["<b>⚙️&nbsp;Wpf.Clippy.csproj</b><br/><small>net48</small>"]
    P1 --> P2
    click P1 "#clippywpfdemoclippywpfdemocsproj"
    click P2 "#wpfclippywpfclippycsproj"

```

## Project Details

<a id="clippywpfdemoclippywpfdemocsproj"></a>
### Clippy.Wpf.Demo\Clippy.Wpf.Demo.csproj

#### Project Info

- **Current Target Framework:** net48
- **Proposed Target Framework:** net10.0-windows
- **SDK-style**: False
- **Project Kind:** ClassicWpf
- **Dependencies**: 1
- **Dependants**: 0
- **Number of Files**: 8
- **Number of Files with Incidents**: 1
- **Lines of Code**: 441
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["Clippy.Wpf.Demo.csproj"]
        MAIN["<b>⚙️&nbsp;Clippy.Wpf.Demo.csproj</b><br/><small>net48</small>"]
        click MAIN "#clippywpfdemoclippywpfdemocsproj"
    end
    subgraph downstream["Dependencies (1"]
        P2["<b>⚙️&nbsp;Wpf.Clippy.csproj</b><br/><small>net48</small>"]
        click P2 "#wpfclippywpfclippycsproj"
    end
    MAIN --> P2

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

<a id="wpfclippywpfclippycsproj"></a>
### Wpf.Clippy\Wpf.Clippy.csproj

#### Project Info

- **Current Target Framework:** net48
- **Proposed Target Framework:** net10.0-windows
- **SDK-style**: False
- **Project Kind:** ClassicWpf
- **Dependencies**: 0
- **Dependants**: 1
- **Number of Files**: 18
- **Number of Files with Incidents**: 1
- **Lines of Code**: 706
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P1["<b>⚙️&nbsp;Clippy.Wpf.Demo.csproj</b><br/><small>net48</small>"]
        click P1 "#clippywpfdemoclippywpfdemocsproj"
    end
    subgraph current["Wpf.Clippy.csproj"]
        MAIN["<b>⚙️&nbsp;Wpf.Clippy.csproj</b><br/><small>net48</small>"]
        click MAIN "#wpfclippywpfclippycsproj"
    end
    P1 --> MAIN

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

