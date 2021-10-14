using System.Collections;
using System.Collections.Generic;
using ScavengingExpansion.Defs.Tags;
using Verse;

namespace ScavengingExpansion.Utils
{
    public static class ThingUtils
    {
        public static Thing makeThingWithCount(ThingDef def, int amount)
        {
            Thing toMake = ThingMaker.MakeThing(def);
            toMake.stackCount = amount;
            return toMake;
        }

        public static IEnumerable<Thing> getProbabilisticThingSet(IEnumerable<AdvancedScavengingProduct> pool,
            float efficiency = 1f)
        {
            foreach (AdvancedScavengingProduct product in pool)
            {
                int additionalAmount = product.maxAmount - product.minAmount;
                int finalAmount = GenMath.RoundRandom((float)additionalAmount * efficiency * Rand.Value) +
                                  product.minAmount;
                float finalChance = product.chance * efficiency;
                float diceRoll = Rand.Value;
#if DEBUG
                Log.Message(
                    $"Efficiency : {efficiency} | Base chance : {product.chance} | Final chance : {finalChance} | Dice Roll : {diceRoll} | Final amount : {finalAmount}");
#endif

                if (finalAmount > 0 && diceRoll < finalChance)
                {
                    ThingDef productDef = DefDatabase<ThingDef>.GetNamed(product.defRef);
                    if (productDef == null)
                    {
                        Log.Error($"Cannot find def named {product.defRef}");
                    }
                    else
                    {
                        yield return ThingUtils.makeThingWithCount(productDef, finalAmount);
                    }
                }
            }
        }
    }
}