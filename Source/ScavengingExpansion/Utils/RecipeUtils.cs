using System;
using RimWorld;
using System.Collections.Generic;
using ScavengingExpansion.Defs.DefModExtensions;
using ScavengingExpansion.Defs.Tags;
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
                    foreach (AdvancedScavengingProduct product in ingredientYield.advancedYield )
                    {
                        float efficiency = recipeDef.efficiencyStat != null ? worker.GetStatValue(recipeDef.efficiencyStat) : 1f;
                        int additionalAmount = product.maxAmount - product.minAmount;
                        int finalAmount = GenMath.RoundRandom((float)additionalAmount * efficiency) + product.minAmount;
                        //TODO : If ingredient is hoarder corpse, take into account hoarding sack health for efficiency
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
    }
}