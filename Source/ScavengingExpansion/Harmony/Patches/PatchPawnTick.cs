using HarmonyLib;
using ScavengingExpansion.Comps;
using Verse;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("Tick")]
    public class PatchPawnTick
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance)
        {
            if (__instance.equipment?.Primary?.TryGetComp<CompOverclockable>() is CompOverclockable comp)
            {
                comp.CompTick();
            }
        }
    }
}