# Upgrade Options — Clippy.Core.Wpf

Assessment: 2 .NET Framework 4.8 WPF projects; both use legacy project format, and `Wpf.Clippy` is a library consumed by the demo application.

## Strategy

### Upgrade Strategy
Two .NET Framework projects are connected by a project reference, so upgrade in dependency order and validate each tier.

| Value | Description |
|-------|-------------|
| **Bottom-Up** (selected) | Upgrade and validate leaf libraries before dependent applications; fixed for multi-project .NET Framework migrations. |

## Project Structure

### Project Approach
`Wpf.Clippy` is a class library referenced by `Clippy.Wpf.Demo`; the demo remains on .NET Framework while the library tier is upgraded, so the library must support both targets during the transition. The WPF demo application itself will move in-place.

| Value | Description |
|-------|-------------|
| **Multi-targeting** (selected) | Keep the library compatible with .NET Framework and add the modern Windows target during migration. |
| In-place | Replace the library target directly; all consumers must move first. |
