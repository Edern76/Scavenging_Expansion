using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using ScavengingExpansion.Defs.DefModExtensions;
using Verse;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(StunHandler))]
    [HarmonyPatch("StunFor")]
    public class PatchStunHandler
    {
        [HarmonyPrefix]
        public static bool Prefix(StunHandler __instance)
        {
            Pawn pawn = __instance.parent as Pawn;
            if (pawn != null && pawn.health != null && pawn.health.hediffSet != null &&
                pawn.health.hediffSet.hediffs != null)
            {
                IEnumerable<Hediff> hediffsWithExtension =
                    pawn.health.hediffSet.hediffs.Where(hediff => hediff.def.HasModExtension<StunImmuniser>());

                foreach (Hediff h in hediffsWithExtension)
                {
                    if (h.def.GetModExtension<StunImmuniser>().stunImmune)
                    {
                        return false;
                    }    
                }
            }
            return true;
        }
    }
}