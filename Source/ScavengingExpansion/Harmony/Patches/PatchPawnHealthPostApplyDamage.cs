using System.Linq;
using HarmonyLib;
using ScavengingExpansion.Defs.DefModExtensions;
using ScavengingExpansion.Defs.Tags;
using ScavengingExpansion.Utils;
using Verse;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("PostApplyDamage")]
    public class PatchPawnHealthPostApplyDamage
    {
        [HarmonyPostfix]
        public static void Postfix(DamageInfo dinfo, float totalDamageDealt, Pawn __instance)
        {
            if (!(__instance.Dead || __instance.Destroyed || dinfo.Weapon == null))
            {
                if (dinfo.Weapon.HasModExtension<HediffApplier>())
                {
                    HediffApplier applier = dinfo.Weapon.GetModExtension<HediffApplier>();
                    float totalSeverity = Rand.Range(applier.baseMinValue, applier.baseMaxValue);
                    if (applier.additionalSeverityStages != null)
                    {
                        foreach (RangeWithChance stage in applier.additionalSeverityStages.OrderByDescending(s =>
                            s.chance))
                        {
                            if (Rand.Chance(stage.chance))
                            {
                                totalSeverity += Rand.Range(stage.minValue, stage.maxValue);
                                if (!applier.independantRolls)
                                {
                                    break;
                                }    
                            }    
                        }    
                    }
                    HediffUtils.AddOrUpdateHediffWithSeverity(__instance, applier.hediffDef, totalSeverity);
                }    
            }
        }
    }
}