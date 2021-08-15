using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using ScavengingExpansion.Defs;
using ScavengingExpansion.Enums;
using ScavengingExpansion.Utils;

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

                if (scavengingDef.scavengingTypes.Contains((ScavengingRecipeType.Advanced)))
                {
                    foreach (Thing product in RecipeUtils.GetAdvancedSalvagingProducts(recipeDef, worker, ingredients))
                    {
                        yield return product;
                    }
                }    
            }
        }
    }
}