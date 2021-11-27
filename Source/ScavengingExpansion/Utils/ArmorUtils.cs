using RimWorld;
using UnityEngine;
using Verse;

namespace ScavengingExpansion.Utils
{
    public static class ArmorUtils
    {
        public static void SimpleApplyArmor(
            ref float damAmount,
            float armorPenetration,
            float armorRating,
            ref DamageDef damageDef)
        // Adapted from the game's code to allow us to call it from our mod since it is set to private
        {
            double num1 = (double) Mathf.Max(armorRating - armorPenetration, 0.0f);
            float num2 = Rand.Value;
            float num3 = (float) (num1 * 0.5);
            float num4 = (float) num1;
            if ((double) num2 < (double) num3)
            {
                damAmount = 0.0f;
            }
            else
            {
                if ((double) num2 >= (double) num4)
                    return;
                damAmount = (float) GenMath.RoundRandom(damAmount / 2f);
                if (damageDef.armorCategory != DamageArmorCategoryDefOf.Sharp)
                    return;
                damageDef = DamageDefOf.Blunt;
            }
        }
    }
}