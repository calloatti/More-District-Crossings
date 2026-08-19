Include ..\AGENTS.md

# More District Crossings — Mod-Specific Agent Instructions

## Identity
- **Assembly:** `moredistrictcrossings`
- **Namespace:** `Calloatti.MoreDistrictCrossings`
- **ModId:** `Calloatti.MoreDistrictCrossings`
- **Framework:** Harmony, Bindito DI
- **Min Game Version:** 1.0.12.5 — uses `timberborn-decompiled-1.0.*`

## What This Mod Does
Adds tubeway and zipline district crossing buildings to the game. Uses a Harmony patch to dynamically inject blueprint references into the game's building template collections (IronTeeth and Folktails factions) when their respective stations are present.

## Source Architecture (`Version-1.0/Source/`)

| File | Role |
|---|---|
| `ModStarter.cs` | `IModStarter` — applies all Harmony patches on mod load |
| `AssetLoaderConfigurator.cs` | Binds `AssetLoaderService` as singleton in Bindito DI (provides `IAssetLoader` to Harmony patch) |
| `TemplateCollectionBlueprintsPatch.cs` | Harmony patch — intercepts `TemplateCollectionSpec.get_Blueprints` to inject crossing blueprints at runtime |

## Hard Rule
DO NOT EVER TOUCH THE DEPLOY FOLDER.

BUILD DOES EVERYTHING, NEVER EVER MESS WITH THE DEPLOY PROCESS.
