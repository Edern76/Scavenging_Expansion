using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using ScavengingExpansion.Comps;
using ScavengingExpansion.Defs.DefModExtensions;
using ScavengingExpansion.Utils;
using UnityEngine;
using Verse;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(BodyPartDef))]
    [HarmonyPatch("GetMaxHealth")]
    public class PatchPartMaxHealth
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, BodyPartDef __instance, Pawn pawn)
        {
            if (pawn != null && pawn.health != null && pawn.health.hediffSet != null &&
                pawn.health.hediffSet.hediffs != null)
            {
                IEnumerable<Hediff> partHediffs = HediffUtils.GetPartHediffsFromDef(pawn, __instance);
                if (partHediffs == null) //Should theoretically never happen but keeping it just in case
                {
                    Log.Error("Part hediffs null !"); 
                    return;
                }

                IEnumerable<Hediff> hediffsWithExtension =
                    partHediffs.Where(hediff => hediff.def.HasModExtension<HealthModifier>());

                int totalOffset = hediffsWithExtension
                    .Select(hediff => hediff.def.GetModExtension<HealthModifier>().bodyPartHealthOffset)
                    .DefaultIfEmpty(0)
                    .Sum(); //Doing it in 3 stages so that we can debug intermediary enumerators if shit goes wrong
                if (totalOffset != 0)
                {
                    float toAdd = (float)Mathf.CeilToInt((float)totalOffset * pawn.HealthScale);
                    //Log.Message($"Setting result on part {__instance.label} (offset = {totalOffset} | toAdd = {toAdd} | before = {__result})"); //Don't uncomment unless absolutely necessary, this makes the console implode
                    __result += toAdd;
                }
            }
        }
    }
}