using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using ScavengingExpansion.Comps;
using Verse.AI;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(Verb_Shoot))]
    [HarmonyPatch("TryCastShot")]
    public class PatchVerbShoot
    {

        private static void getLauncherAndEquipment(Verb_Shoot __instance, out Thing launcher, out Thing equipment)
        {
            CompMannable comp = __instance.caster.TryGetComp<CompMannable>();
            if (comp != null && comp.ManningPawn != null) //Needed in the case the shooting thing is a manned turret
            {
                launcher = (Thing)comp.ManningPawn;
                equipment = __instance.caster;
            }
            else
            {
                launcher = __instance.caster;
                equipment = (Thing)__instance.EquipmentSource;
            }
        }
        
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, Verb_Shoot __instance)
        {
            Thing launcher;
            Thing equipment;
            getLauncherAndEquipment(__instance, out launcher, out equipment);

            CompJammable compJam = equipment.TryGetComp<CompJammable>();
            if (compJam != null)
            {
                if (!compJam.Jammed)
                {
                    compJam.ShotsSinceLastBurst++;
                    if (compJam.ShotsSinceLastBurst >= __instance.verbProps.burstShotCount)
                    {
                        compJam.ShotsSinceLastBurst = 0;
                        float diceRoll = Rand.Value;
                        float chance = compJam.GetJamChance();
                        #if DEBUG
                            Log.Message($"DiceRoll : {diceRoll} | Chance : {chance}");
                        #endif
                        if (diceRoll >= chance)
                        {
                            return true; //No jamming, continue to vanilla method
                        }
                        else
                        {
                            MoteMaker.ThrowText(launcher.DrawPos, launcher.Map, "*click*");
                            compJam.setJammed();
                            __result = false; //Cannot shoot
                            
                            CompOverclockable compOverclockable = equipment.TryGetComp<CompOverclockable>();
                            if (compOverclockable != null && compOverclockable.Overclocked)
                            {
                                float burnChance = compOverclockable.Props.HandsBurningChance;
                                float burnDiceRoll = Rand.Value;
                                if (burnDiceRoll < burnChance)
                                {
                                    compOverclockable.GetWearer?.def.race.body
                                        .GetPartsWithDef(BodyPartDefOf.Hand).ToList().ForEach(hand =>
                                        {
                                            DamageInfo burnDamage = new DamageInfo(DamageDefOf.Burn,
                                                compOverclockable.Props.HandsDamage);
                                            burnDamage.SetHitPart(hand);
                                            compOverclockable.GetWearer?.TakeDamage(burnDamage);
                                        });
                                }
                            }
                            
                            float explosionChance = compJam.Props.ExplosionOnJamChance;
                            if (explosionChance != 0f)
                            {
                                float explosionDiceRoll = Rand.Value;
                                #if DEBUG
                                    Log.Message($"Explosion DiceRoll : {explosionDiceRoll} | Chance : {explosionChance}");
                                #endif
                                if (explosionDiceRoll < explosionChance)
                                {
                                    compJam.DoExplode();
                                }
                            }
                            

                            return false; //Bypasses vanilla method, *hopefully* this shouldn't break things.
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    MoteMaker.ThrowText(launcher.DrawPos, launcher.Map, "*click*");
                    __result = false;
                    return false;
                }
            }
            else
            {
                return true; //Let the vanilla method run without altering value
            }
        }

        public static void Postfix(ref bool __result, Verb_Shoot __instance)
        {
            Thing launcher;
            Thing equipment;
            getLauncherAndEquipment(__instance, out launcher, out equipment);
            CompJammable compJam = equipment.TryGetComp<CompJammable>();
            if (compJam != null && compJam.Jammed)
            {
                Pawn pawn = (Pawn)launcher;
                if (pawn.IsColonist && !compJam.AutoUnjam)
                {
                    return;
                }

                Job job = compJam.TryMakeJob();
                if (job != null)
                {
                    pawn.jobs.TryTakeOrderedJob(job);
                }
            }    
        }
    }
}