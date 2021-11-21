using HarmonyLib;
using ScavengingExpansion.Defs.DefModExtensions;
using Verse;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(ResearchProjectDef))]
    [HarmonyPatch("CanStartNow", MethodType.Getter)]
    public class PatchResearchDefCanStartNow
    {
        [HarmonyPostfix]
        public static void postfix(ref bool __result, ResearchProjectDef __instance)
        {
            if (__instance.HasModExtension<ResearchBlocker>() && __instance.GetModExtension<ResearchBlocker>().disableNormalResearch)
            {
                __result = false;
            }    
        }
    }
}