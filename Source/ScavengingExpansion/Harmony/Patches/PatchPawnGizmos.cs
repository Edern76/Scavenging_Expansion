using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using ScavengingExpansion.Comps;
using Verse;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("GetGizmos")]
    public class PatchPawnGizmos
    {
        [HarmonyPostfix]
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
        {
            if (__result != null)
            {
                foreach (Gizmo g in __result)
                {
                    yield return g; //Return all gizmos from unpatched method before adding ours
                }    
            }

            CompJammable comp = __instance?.equipment?.Primary?.TryGetComp<CompJammable>();
            if (comp != null && __instance.Faction == Faction.OfPlayer)
            {
                foreach (Gizmo g in comp.CompGetGizmosExtra())
                {
                    yield return g;
                }
            }    
        }
    }
}