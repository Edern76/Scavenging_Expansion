using System;
using RimWorld;
using System.Collections.Generic;
using ScavengingExpansion.Defs.DefModExtensions;
using ScavengingExpansion.Defs.Tags;
using Verse;

namespace ScavengingExpansion.Utils
{
    public static class RecipeUtils
    {
        public static IEnumerable<Thing> GetBasicSalvagingProducts(RecipeDef recipeDef, Pawn worker,
            List<Thing> ingredients)
        {
            foreach (Thing ing in ingredients)
            {
                Thing ingredient = ing;
                if (ingredient is Corpse)
                {
                    ingredient = ((Corpse)ing).InnerPawn; //Use the pawn def instead of the corpse def to be able to access mod extensions
                }    
                if (ingredient.def.HasModExtension<ScavengingYield>())
                {
                    int scrapYield = ingredient.def.GetModExtension<ScavengingYield>().scrapYield;
                    
                    float efficiency = recipeDef.efficiencyStat != null ? worker.GetStatValue(recipeDef.efficiencyStat) : 1f; //Same formula as for vanilla butchering/smelting
                    int finalAmount = GenMath.RoundRandom((float)scrapYield * efficiency);
                    if (finalAmount > 0)
                    {
                        ThingDef scrapDef = DefDatabase<ThingDef>.GetNamed("SE_MechanoidScrap");
                        if (scrapDef == null)
                        {
                            Log.Error("Cannot find def for MechanoidScrap !!");
                            throw new NullReferenceException("Could not find def for SE_MechanoidScrap");
                        }
                        else
                        {
                            yield return ThingUtils.makeThingWithCount(scrapDef, finalAmount);
                        }
                    }
                }
            }
        }

        public static IEnumerable<Thing> GetAdvancedSalvagingProducts(RecipeDef recipeDef, Pawn worker,
            List<Thing> ingredients)
        {
            foreach (Thing ing in ingredients)
            {
                Thing ingredient = ing;
                if (ingredient is Corpse)
                {
                    ingredient = ((Corpse)ing).InnerPawn;
                }

                if (ingredient.def.HasModExtension<ScavengingYield>())
                {
                    ScavengingYield ingredientYield = ingredient.def.GetModExtension<ScavengingYield>();
                    float efficiency = recipeDef.efficiencyStat != null ? worker.GetStatValue(recipeDef.efficiencyStat) : 1f;  //TODO : If ingredient is hoarder corpse, take into account hoarding sack health for efficiency
                    foreach (Thing product in ThingUtils.getProbabilisticThingSet(ingredientYield.advancedYield,
                        efficiency))
                    {
                        yield return product;
                    }
                }    
            }    
        }
    }
}