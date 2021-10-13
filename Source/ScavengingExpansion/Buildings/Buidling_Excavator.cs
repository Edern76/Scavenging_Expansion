using Rimworld;
using Verse;
using ScavengingExpansion.Defs.DefModExtensions;
using ScavengingExpansion.Defs.Tags;
using ScavengingExpansion.Defs;

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
                    ExcavatorThingSet thingSet = (ExcavatorThingSet)DefDatabase<ThingDef>.GetNamed(excavatorExtension.thingSetDef)
                    this._possibleProducts = thingSet.excavatingProducts;
                }
                return this._possibleProducts;
            }
        }

        public CompAssignableToPawn compAssignable
        {
            get
            {
                return this.TryGetComp<CompAssignableToPawn>();
            }
        }
        public CompForbiddable forbiddable
        {
            get
            {
                return this.TryGetComp<CompForbiddable>();
            }
        }

        public IEnumerable<Pawn> AssignedPawns
        {
            get
            {
                return compAssignable.AssignedPawnsForReading;
            }
        }



        public IEnumerable<Thing> MakeRessources(Pawn worker)
        {
            foreach (AdvancedScavengingProduct product in this.possibleProducts)
            {
                
            }
        } 
    }
}