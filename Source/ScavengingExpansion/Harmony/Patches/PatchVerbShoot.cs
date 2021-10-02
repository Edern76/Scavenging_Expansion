using RimWorld;
using Verse;
using HarmonyLib;
using ScavengingExpansion.Comps;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(Verb_Shoot))]
    [HarmonyPatch("TryCastShot")]
    public class PatchVerbShoot
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, Verb_Shoot __instance)
        {
            Thing launcher;
            Thing equipment;
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

            CompJammable compJam = equipment.TryGetComp<CompJammable>();
            if (compJam != null)
            {
                float diceRoll = Rand.Value;
                float chance = compJam.Props.JamChancePerShot;
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
                    __result = false; //Cannot shoot
                    return false; //Bypasses vanilla method, *hopefully* this shouldn't break things.
                }
            }
            else
            {
                return true; //Let the vanilla method run without altering value
            }
        }
            
    }
}