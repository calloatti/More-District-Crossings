using Bindito.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.AssetSystem;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateCollectionSystem;

namespace Calloatti.MoreDistrictCrossings
{
  // 1. A singleton to capture IAssetLoader so our static Harmony patch can safely build blueprints
  public class DynamicCrossingContext : ILoadableSingleton
  {
    public static IAssetLoader AssetLoader { get; private set; }

    public DynamicCrossingContext(IAssetLoader assetLoader)
    {
      AssetLoader = assetLoader;
    }

    public void Load() { }
  }

  // 2. The Configurator binds our context and triggers Harmony when the game starts
  [Context("MainMenu")]
  [Context("Game")]
  [Context("MapEditor")]
  public class DynamicCrossingsConfigurator : Configurator
  {
    private static bool _patched = false;

    protected override void Configure()
    {
      // Ensure Harmony only patches once
      if (!_patched)
      {
        new Harmony("com.yourname.dynamiccrossings").PatchAll();
        _patched = true;
      }
      Bind<DynamicCrossingContext>().AsSingleton();
    }
  }

  // 3. The Harmony Patch that modifies the TemplateCollection array on-the-fly!
  [HarmonyPatch(typeof(TemplateCollectionSpec), "get_Blueprints")]
  public static class TemplateCollectionBlueprintsPatch
  {
    // Make sure these match the exact folder structure of your mod
    private static readonly string TubewayCrossingPath = "Buildings/DistrictManagement/TubewayDistrictCrossing/TubewayDistrictCrossing.IronTeeth.blueprint";
    private static readonly string ZiplineCrossingPath = "Buildings/DistrictManagement/ZiplineDistrictCrossing/ZiplineDistrictCrossing.Folktails.blueprint";

    public static void Postfix(TemplateCollectionSpec __instance, ref ImmutableArray<AssetRef<BlueprintAsset>> __result)
    {
      // Safety check to ensure the Dependency Injection has initialized our AssetLoader
      if (DynamicCrossingContext.AssetLoader == null) return;

      // We only care about modifying the main building collections
      if (__instance.CollectionId != "Buildings.IronTeeth" && __instance.CollectionId != "Buildings.Folktails") return;

      var list = __result.ToList();
      bool modified = false;

      // --- TUBEWAY CROSSING LOGIC ---
      // If the array contains the Tubeway Station, but NOT our Tubeway Crossing -> Add it!
      bool hasTubeway = list.Any(a => a.Path.Contains("TubewayStation"));
      bool hasTubewayCrossing = list.Any(a => a.Path == TubewayCrossingPath);

      if (hasTubeway && !hasTubewayCrossing)
      {
        list.Add(CreateAssetRef(TubewayCrossingPath));
        modified = true;
      }

      // --- ZIPLINE CROSSING LOGIC ---
      // If the array contains the Zipline Station, but NOT our Zipline Crossing -> Add it!
      bool hasZipline = list.Any(a => a.Path.Contains("ZiplineStation"));
      bool hasZiplineCrossing = list.Any(a => a.Path == ZiplineCrossingPath);

      if (hasZipline && !hasZiplineCrossing)
      {
        list.Add(CreateAssetRef(ZiplineCrossingPath));
        modified = true;
      }

      // If we injected a crossing, overwrite the result being returned to the game engine
      if (modified)
      {
        __result = list.ToImmutableArray();
      }
    }

    // Properly constructs the AssetRef so the game can lazy-load the actual JSON asset natively
    private static AssetRef<BlueprintAsset> CreateAssetRef(string path)
    {
      return new AssetRef<BlueprintAsset>(path, new Lazy<BlueprintAsset>(() => DynamicCrossingContext.AssetLoader.Load<BlueprintAsset>(path)));
    }
  }
}