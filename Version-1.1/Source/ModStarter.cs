using HarmonyLib;
using Timberborn.ModManagerScene;

namespace Calloatti.MoreDistrictCrossings
{
  public class ModStarter : IModStarter
  {
    public void StartMod(IModEnvironment modEnvironment)
    {
      new Harmony("Calloatti.MoreDistrictCrossings").PatchAll();
    }
  }
}
