using HarmonyLib;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.TemplateCollectionSystem;
using Timberborn.TemplateSystem;

namespace Calloatti.MoreDistrictCrossings
{
    [HarmonyPatch(typeof(TemplateCollectionService), nameof(TemplateCollectionService.Load))]
    public static class TemplateCollectionService_Load_Patch
    {
        private const string TubewayCrossingTemplateName = "TubewayDistrictCrossing.IronTeeth";
        private const string ZiplineCrossingTemplateName = "ZiplineDistrictCrossing.Folktails";
        private const string TubewayStationTemplateName = "TubewayStation.IronTeeth";
        private const string ZiplineStationTemplateName = "ZiplineStation.Folktails";
        private const string TubewayCrossingPath = "Buildings/DistrictManagement/TubewayDistrictCrossing/TubewayDistrictCrossing.IronTeeth.blueprint";
        private const string ZiplineCrossingPath = "Buildings/DistrictManagement/ZiplineDistrictCrossing/ZiplineDistrictCrossing.Folktails.blueprint";

        public static void Postfix(TemplateCollectionService __instance)
        {
            if (__instance?.AllTemplates == null) return;

            var specService = __instance._specService;
            if (specService == null) return;

            var allTemplates = __instance.AllTemplates.ToList();
            int initialCount = allTemplates.Count;

            if (HasTemplate(allTemplates, TubewayStationTemplateName) && !HasTemplate(allTemplates, TubewayCrossingTemplateName))
            {
                var bp = specService.GetBlueprint(TubewayCrossingPath);
                if (bp != null) allTemplates.Add(bp);
            }

            if (HasTemplate(allTemplates, ZiplineStationTemplateName) && !HasTemplate(allTemplates, ZiplineCrossingTemplateName))
            {
                var bp = specService.GetBlueprint(ZiplineCrossingPath);
                if (bp != null) allTemplates.Add(bp);
            }

            if (allTemplates.Count > initialCount)
            {
                __instance.AllTemplates = allTemplates.ToImmutableArray();
            }
        }

        private static bool HasTemplate(List<Blueprint> templates, string templateName)
        {
            return templates.Any(bp =>
            {
                var spec = bp.GetSpec<TemplateSpec>();
                return spec != null && spec.TemplateName == templateName;
            });
        }
    }
}