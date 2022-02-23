using HarmonyLib;
using ScavengingExpansion.Defs.DefModExtensions;
using ScavengingExpansion.Utils;
using Verse;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(ArmorUtility))]
    [HarmonyPatch("GetPostArmorDamage")]
    public class PatchPostArmorDamage
    {
        public static void Postfix(ref float __result, Pawn pawn, float armorPenetration, BodyPartRecord part, ref DamageDef damageDef)
        {
            if (pawn != null && pawn.health != null && pawn.health.hediffSet != null &&
                pawn.health.hediffSet.hediffs != null)
            {
                float armoredDamageAmount = __result;
                foreach (Hediff hediff in HediffUtils.GetPartHediffsFromDef(pawn, part.def))
                {
                    if (hediff.def.HasModExtension<HealthModifier>())
                    {
                        HealthModifier healthModifier = hediff.def.GetModExtension<HealthModifier>();
                        if (healthModifier.bodyPartArmorOffset != 0f)
                        {
                            ArmorUtils.SimpleApplyArmor(ref armoredDamageAmount, armorPenetration, healthModifier.bodyPartArmorOffset, ref damageDef);
                        }    
                    }    
                }

                __result = armoredDamageAmount;
            }    
        }
    }
}