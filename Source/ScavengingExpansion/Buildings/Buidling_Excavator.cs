using System.Collections.Generic;
using RimWorld;
using Verse;
using ScavengingExpansion.Defs.DefModExtensions;
using ScavengingExpansion.Defs.Tags;
using ScavengingExpansion.Defs;
using ScavengingExpansion.Utils;

namespace ScavengingExpansion.Buildings
{
    public class Building_Excavator : Building
    {

        private List<AdvancedScavengingProduct> _possibleProducts;

        private ExcavatorExtension excavatorExtension
        {
            get
            {
                return this.def.GetModExtension<ExcavatorExtension>();
            }
        }

        private List<AdvancedScavengingProduct> possibleProducts
        {
            get
            {
                if (_possibleProducts == null)
                {
                    ProbabilisticThingSetDef thingSetDef =
                        (ProbabilisticThingSetDef)DefDatabase<ThingDef>.GetNamed(excavatorExtension.thingSetDef);
                    this._possibleProducts = thingSetDef.products;
                }
                return this._possibleProducts;
            }
        }

        public float ExcavationTime => excavatorExtension.excavationCycleTime;


        public IEnumerable<Thing> MakeRessources(Pawn worker)
        {
            float efficiency = worker.GetStatValue(StatDefOf.MiningYield) * excavatorExtension.buildingEfficiency;
            foreach (Thing product in ThingUtils.getProbabilisticThingSet(possibleProducts, efficiency))
            {
                yield return product;
            }    
        } 
    }
}