using HarmonyLib;
using RimWorld;
using ScavengingExpansion.Comps;
using Verse;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(StatExtension))]
    [HarmonyPatch("GetStatValue")]
    public class PatchGetStatValue
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Thing thing, StatDef stat)
        {
            if (thing.TryGetComp<CompOverclockable>() is CompOverclockable comp)
            {
                if (comp.OnCooldown)
                {
                    if (stat == StatDefOf.RangedWeapon_Cooldown) //Can't use switch because StatDefOf values aren't considered as constants
                    {
                        __result = __result * comp.Props.WeaponCooldownOffsetFactor;
                        
                    }
                    else if (stat == StatDefOf.AccuracyTouch
                             || stat == StatDefOf.AccuracyShort
                             || stat == StatDefOf.AccuracyMedium
                             || stat == StatDefOf.AccuracyLong)
                    {
                        __result = __result * comp.Props.WeaponAccuracyOffsetFactor;
                    }
                }
            }
            else if (thing is Pawn p && p.equipment?.Primary?.TryGetComp<CompOverclockable>() is CompOverclockable compEquipped)
            {
                if (compEquipped.Overclocked)
                {
                    if (stat == StatDefOf.AimingDelayFactor)
                    {
                        __result = __result * compEquipped.Props.AimingDelayOffsetFactor;
                    }
                }
            }
        }
        
    }
}