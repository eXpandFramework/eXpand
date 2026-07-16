---
name: expand
description: Reference for the eXpandFramework (eXpand) project — the original DevExpress XAF extension library with 30+ classic modules, WorldCreator, ModelDifference, ExcelImporter, and the parent project of Reactive.XAF
---

# eXpandFramework Reference

The original open-source (~15-year-old) **DevExpress XAF** extension library. It is the parent/container project of the eXpandFramework organization, alongside **Reactive.XAF** (standalone low-dependency modules) and **XpandPwsh** (PowerShell build tools). All classic controller-based modules here are being gradually replaced by standalone Reactive.XAF modules.

**Build rule:** All compilations use `-WarnAsError` — any warning fails the build. Keep code warning-clean.

**Default working directory:** `C:\Work\expand\` — project root.

> **⚠️ -WarnAsError is baked into ALL compilations.** Builds fail on any warning. Code must stay warning-clean at all times.

## Quick Start

### Build (`bx`)

```powershell
bx Release        # Release build (nuget.org)
bx lab            # Lab build (lab feed)
bx CompileModules # Fast compile-only: restore, version, build AllModules.sln
```

### Publish (`px`)

```powershell
px             # Publish to Lab pipeline
px -Release    # Publish to Release pipeline
px -SkipStage  # Skip git stage step
```

### Raw Build Commands

```powershell
# Full build (Release)
.\build.ps1

# Build just the primary solution
dotnet build Xpand\Xpand.ExpressApp.Modules\AllModules.sln

# EasyTest run
& .\Support\Build\RunEasyTests.ps1
```

## Architecture

### Three-Layer Dependency Chain

```
Layer 1 (Base):
  Xpand.Utils          — general utilities (Reflection, Enumerable, String, etc.)
  Xpand.Xpo            — XPO extensions (ConnectionProviders, Converters, CustomFunctions, DB, Filtering, MetaData, Parser)

Layer 2 (Persistent):
  Xpand.Persistent.Base     — interfaces, base BOs, 25+ sub-namespaces (Logic, Security, Validation, ModelDifference, WorldCreator, etc.)
  Xpand.Persistent.BaseImpl — concrete: PersistentMetaData, AuditTrail, ModelDifference, etc.

Layer 3 (ExpressApp Core):
  Xpand.ExpressApp     — XpandSystemModule, 50+ controllers, model node updaters, editors
  Xpand.ExpressApp.Win — Windows-specific controllers, editors, property editors, services

Layer 4 (Feature Modules):
  30+ modules in Xpand.ExpressApp.Modules/ — each with Core/Win/Web variants
```

### Module Pattern (Classic Controller-Based)

Unlike Reactive.XAF's functional/reactive paradigm, these modules follow the traditional XAF pattern:

1. Inherit from `XpandModuleBase` (which extends `ModuleBase`)
2. Have traditional XAF Controllers in a `Controllers/` folder
3. Have platform-specific variants (Core, Win, Web subfolders)
4. Some now also depend on standalone Reactive.XAF modules via NuGet

### Project Output & References

- **Output path:** `Xpand.DLL\` at repo root (no target framework subdirectory)
- **Reference style:** `<Reference>` with `<HintPath>` to `..\..\..\Xpand.DLL\` — **NOT** `<ProjectReference>`
- **Build order is manual** — defined in `Xpand.projects` XML, not inferred from MSBuild project references
- **All assemblies are strong-named** with `Xpand\Xpand.Key\Xpand.snk`
- **Central Package Management:** `Directory.Packages.props` pins all NuGet versions

### Key Dependencies

| Category | Packages |
|----------|----------|
| **DevExpress XAF** | DevExpress.ExpressApp.*, DevExpress.Xpo, DevExpress.Win.* at **26.1.3** |
| **Reactive.XAF Extensions** | Xpand.Extensions 4.261.1, Xpand.Extensions.Reactive 4.261.1, Xpand.Extensions.XAF 4.261.1, Xpand.Extensions.XAF.Xpo 4.261.1, Xpand.Extensions.Mono.Cecil 4.261.1 |
| **Reactive.XAF Modules** | Xpand.XAF.Modules.* 4.261.1 (AutoCommit, CloneModelView, HideToolBar, Reactive, MasterDetail, etc.) |
| **Reactive** | System.Reactive 6.0.1, System.Interactive 6.0.1, akarnokd.reactive_extensions 0.0.27-alpha |
| **Code Generation** | Microsoft.CodeAnalysis 5.0.0, Microsoft.CodeAnalysis.CSharp 5.0.0, Mono.Cecil 0.11.6 |
| **Testing** | NUnit 4.4.0, Shouldly 4.3.0, Moq 4.20.72, DevExpress.EasyTest 26.1.3 |
| **Utilities** | Fasterflect.Xpand 2.0.7, Lib.Harmony 2.4.2, RazorLight 2.3.1, MagicOnion 2.6.3 |

## Build System

### Entry Point

`build.ps1` calls `Support\Build\go.ps1` with the **Release** task list:

```
Clean → Version → Init → CompileModules → CheckStandalonePackageVersions → VSIX → IndexSources → Finalize → CreateNuGets → Installer
```

The **Lab** task is shorter: `Clean → Version → Init → CompileModules`

### Key Build Details

- **psake-based** — PowerShell build framework (v4.7.4)
- **go.ps1** auto-installs XpandPwsh (v1.252.0.6) and psake modules
- **CompileModules** builds `AllModules.sln` with MSBuild, outputting to `Xpand.DLL\`
- **Post-build:** `Directory.Build.targets` runs `ReplaceNuget.ps1` to copy assemblies into local NuGet cache
- **CreateNuGets.ps1** creates 37 NuGet packages from 37 `.nuspec` files in `Support\Nuspec\`
- **modules.json** defines 74 module entries (Core/Win/Web variants) with Name, FullName, Assembly, Platform
- **Xpand.projects** XML lists all: CoreProjects, ModuleProjects, DemoWinSolutions, EasyTestProjects

### NuGet Sources

- `https://api.nuget.org/v3/index.json`
- `https://xpandnugetserver.azurewebsites.net/nuget`
- `C:\Program Files\DevExpress 26.1\Components\System\Components\packages`

### CI/CD

Azure DevOps pipeline (`Azure-pipeline.yml`) on self-hosted agent pool. Triggers on master branch.

## Modules Overview

### Classic Module Directories (Xpand.ExpressApp.Modules)

| Module Directory | Description |
|------------------|-------------|
| AdditionalViewControlsProvider | Additional custom controls in views |
| AuditTrail | Track object changes |
| Chart.Win | Chart control integration |
| Dashboard | XAF dashboard features |
| Email | Send business objects as email |
| ExcelImporter | Import Excel/CSV files with mapping UI |
| FileAttachment | File attachment features |
| IO | Export/import object graphs with configurable serialization |
| JobScheduler | Background job scheduling |
| Logic | Conditional architecture engine powering many other modules |
| MapView | Map control integration |
| MasterDetail | Master-detail view management |
| ModelArtifactState | Apply conditional logic to model artifacts (Controllers, Actions) |
| ModelDifference | Store application model in DB, combine user/role models, multiple models at design time |
| NCarousel | Carousel control for Web |
| PivotChart | Pivot + Chart integration |
| PivotGrid.Win | PivotGrid features for WinForms |
| Reports / ReportsV2.Win | Reporting module |
| Scheduler | Scheduler control integration |
| Security | Extended security (permissions, authentication providers) |
| StateMachine | XAF State Machine features |
| TreeListEditors | TreeList editor features |
| Validation | Extended validation rules, permission validation, warning/info support |
| ViewVariants | View variant management |
| WizardUI.Win | Wizard-style UI |
| Workflow | Business process workflow |
| WorldCreator | Dynamic assembly generation at runtime (Mono.Cecil + Roslyn) |
| XtraDashboard | Dashboard designer integration |

### Standalone Reactive.XAF Modules (consumed via NuGet)

AutoCommit, Blazor, BulkObjectUpdate, CloneMemberValue, CloneModelView, GridListEditor, HideToolBar, JobScheduler.Hangfire, MasterDetail, ModelMapper, ModelViewInheritance, OneView, PositionInListView, RazorView, Reactive.Logger, Reactive.Logger.Hub, Reactive.Rest, RefreshView, SequenceGenerator, SpellChecker, StoreToDisk, SuppressConfirmation, TenantManager, ViewEditMode, ViewItemValue, Windows, and more.

## WorldCreator Module

The most architecturally significant module. **Dynamic assembly generation at runtime** — design persistent classes, generate C# code, compile with Roslyn, and load as runtime modules, all from the XAF UI.

- Uses `Mono.Cecil` + `Microsoft.CodeAnalysis` (Roslyn) for code generation and compilation
- DBPersistentClasses map from WorldCreator abstractions to real database tables
- DBMapper module maps schemas from 14 database types into WorldCreator objects

## ExcelImporter Module

Import Excel/CSV files with mapping UI. Depends on standalone Reactive.XAF modules (`AutoCommitModule`, `ViewEditModeModule`, `SuppressConfirmationModule`, etc.). Uses `ExcelDataReader` for file parsing.

## ModelDifference Module

Store and manage XAF Application Model differences in the database. Supports:
- Combining user-specific, role-specific, and global model layers
- Multiple models at design time
- Runtime model merging
- Persistent storage of model aspects via `PersistentBaseImpl`

## Logic Module

Conditional architecture engine that powers ModelArtifactState and other modules. Provides a framework for defining rule-based conditional behavior on model artifacts.

## Testing

- **NUnit 4.4.0** with **Shouldly 4.3.0** for assertions
- **Moq 4.20.72** for mocking
- **Microsoft.Reactive.Testing 6.0.1** + **akarnokd.reactive_extensions** for reactive test schedulers
- **DevExpress.EasyTest 26.1.3** with WinAdapter + BlazorAdapter for scripted UI tests
- **RunEasyTests.ps1** — builds with `Configuration=EasyTest` and executes EasyTest scripts

## Relationship to Reactive.XAF

eXpandFramework is the parent of three separate repos:
1. **eXpand** (this repo, `C:\Work\expand\`) — the original container/umbrella project
2. **Reactive.XAF** (`C:\Work\Reactive.XAF\`) — standalone low-dependency modules using functional/reactive paradigm
3. **XpandPwsh** — PowerShell module for development/build tasks

### Layered Architecture (Not Predecessor/Successor)

eXpandFramework is the **upper-layer consumer**, not legacy. Reactive.XAF is the **lower-layer reactive engine**. eXpandFramework depends on Reactive.XAF via NuGet:

- `Xpand.Extensions.Reactive`
- `Xpand.XAF.Modules.Reactive`
- `Xpand.XAF.Modules.Reactive.Logger`

### Extraction Timeline

Starting ~March 2019, modules were extracted out of eXpandFramework into standalone Reactive.XAF packages:
- CloneMemberValue was the first (`33c8272f8`, Mar 2019): deleted 101 lines of Controller code, replaced with `Xpand.XAF.Modules.CloneMemberValue` NuGet reference
- HideToolBar followed (`eb1b81f2a`)
- The trend continued — classic controller-based modules → standalone reactive modules

### Commit Cadence

The commit log is dominated by two patterns:
1. **Version bumps** tracking DevExpress releases (e.g. "26.1.301", "25.2.8")
2. **"Update Reactive.XAF packages"** — pulls in new Reactive.XAF releases

Feature work happens in **Reactive.XAF**; **eXpandFramework** pulls it in and provides the 30+ end-user modules. 6,694 commits, dating back to DevExpress 14.2.

### Shared Issue Tracker

Both eXpandFramework and Reactive.XAF share a single GitHub issues repository: https://github.com/eXpandFramework/eXpand/issues. Reactive.XAF does not have its own issue tracker — all issues, whether they originate in Reactive.XAF or eXpandFramework, are tracked there.

## Tools

- **`Xpand.VersionConverter`** — modifies DevExpress references to match target project's DevExpress version
- **`CommonLibs`** — shared PowerShell utilities for the build pipeline
