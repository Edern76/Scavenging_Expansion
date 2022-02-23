using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using ScavengingExpansion.Defs;
using ScavengingExpansion.Defs.DefModExtensions;
using ScavengingExpansion.Enums;
using ScavengingExpansion.Utils;
using UnityEngine;
using ThingDefOf = ScavengingExpansion.DefOfs.ThingDefOf;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(GenRecipe))]
    [HarmonyPatch("MakeRecipeProducts")]
    public class PatchRecipeYield
    {
        [HarmonyPostfix]
        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result,
            RecipeDef recipeDef,
            Pawn worker,
            List<Thing> ingredients,
            Thing dominantIngredient,
            IBillGiver billGiver,
            Precept_ThingStyle precept
            )
        {
            if (__result != null)
            {
                foreach (Thing t in __result)
                {
                    yield return t; //Return all products from unpatched method before adding ours
                }    
            }

            if (recipeDef is ScavengingRecipeDef)
            {
                ScavengingRecipeDef scavengingDef = (ScavengingRecipeDef)recipeDef;
                if (scavengingDef.scavengingTypes.Contains(ScavengingRecipeType.Basic))
                {
                    foreach (Thing product in RecipeUtils.GetBasicSalvagingProducts(recipeDef, worker, ingredients))
                    {
                        yield return product;
                    }    
                }

                if (scavengingDef.scavengingTypes.Contains((ScavengingRecipeType.Alloy)))
                {
                    foreach (Thing product in RecipeUtils.GetAlloySalvagingProducts(recipeDef, worker, ingredients))
                    {
                        yield return product;
                    }    
                }    

                if (scavengingDef.scavengingTypes.Contains((ScavengingRecipeType.Advanced)))
                {
                    IEnumerable<Thing> salvagingProducts =
                        RecipeUtils.GetAdvancedSalvagingProducts(recipeDef, worker, ingredients);

                    if (salvagingProducts.Any())
                    {
                        foreach (Thing product in salvagingProducts)
                        {
                            yield return product;
                        }
                    }
                    else
                    {
                        IEnumerable<Thing> productedAlloy =
                            RecipeUtils.GetAlloySalvagingProducts(recipeDef, worker, ingredients);
                        int baseAmount = productedAlloy.Sum(thing => thing.stackCount);
                        int finalAmount = Mathf.RoundToInt(1 / 4 * baseAmount);
                        if (finalAmount > 0)
                        {
                            yield return ThingUtils.makeThingWithCount(ThingDefOf.SE_MechanoidAlloy, finalAmount);
                        }    
                    }
                }    
            }
        }
    }
}