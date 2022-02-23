using HarmonyLib;
using ScavengingExpansion.DefOfs;
using Verse;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(ThingWithComps))]
    [HarmonyPatch("PreApplyDamage")]
    public class PatchPawnPreApplyDamage
    {
        [HarmonyPrefix]
        public static bool Prefix(ref DamageInfo dinfo, ThingWithComps __instance)
        {
            Pawn curPawn = __instance as Pawn;
            if (curPawn != null && dinfo.Instigator != null)
            {
                Pawn pawnInstigator = dinfo.Instigator as Pawn;
                if (pawnInstigator != null)
                {
                    float damageMultiplier = 1f;
                    if (curPawn.RaceProps.Humanlike &&
                        pawnInstigator.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.SE_SanguisProditoris) != null) //20% to humanoids
                    {
                        damageMultiplier += 0.2f;
                    }

                    if (curPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.SE_SanguisProditoris) != null &&
                        pawnInstigator.RaceProps.Humanlike) //20% from humanoids
                    {
                        damageMultiplier += 0.2f;
                    }

                    if (damageMultiplier > 1f)
                    {
                        dinfo.SetAmount(dinfo.Amount * damageMultiplier);
                    }    
                }
            }    
            return true; //Run vanilla method
        }
    }
}