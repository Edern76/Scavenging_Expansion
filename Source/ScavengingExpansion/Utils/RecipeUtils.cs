using System;
using RimWorld;
using System.Collections.Generic;
using ScavengingExpansion.Defs.DefModExtensions;
using Verse;

namespace ScavengingExpansion.Utils
{
    public class RecipeUtils
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
                            Thing scrapThing = ThingMaker.MakeThing(scrapDef);
                            scrapThing.stackCount = finalAmount;
                            yield return scrapThing;
                        }
                    }
                }
            }
        }
    }
}