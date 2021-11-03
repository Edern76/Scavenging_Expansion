using RimWorld;
using ScavengingExpansion.Buildings;
using Verse;

namespace ScavengingExpansion.Comps
{
    public class CompProperties_AutoExcavator : CompProperties
    {
        public CompProperties_AutoExcavator()
        {
            this.compClass = typeof(CompAutoExcavator);
        }

        public bool AllowManualExcavating = false;
    }

    public class CompAutoExcavator : ThingComp
    {
        public CompProperties_AutoExcavator Props => (CompProperties_AutoExcavator)this.props;
        private int _ticksSinceLastExcav = 0;

        private int TicksSinceLastExcav
        {
            get { return _ticksSinceLastExcav; }
            set { _ticksSinceLastExcav = value; }
        }

        private Building_Excavator Excavator => (Building_Excavator)this.parent;

        public override void CompTick()
        {
            base.CompTick();
            CompPowerTrader powerTrader = Excavator.TryGetComp<CompPowerTrader>();
            if (powerTrader != null && !powerTrader.PowerOn)
            {
                return;
            }

            _ticksSinceLastExcav++;
            if (_ticksSinceLastExcav > Excavator.ExcavationTime.SecondsToTicks())
            {
                _ticksSinceLastExcav = 0;
                foreach (Thing product in Excavator.MakeRessources(null))
                {
                    GenPlace.TryPlaceThing(product, Excavator.Position, Excavator.Map, ThingPlaceMode.Near);
                }
            }    
        }

        public override string CompInspectStringExtra()
        {
            return $"Excavation progress {_ticksSinceLastExcav}/{Excavator.ExcavationTime.SecondsToTicks()}";
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref _ticksSinceLastExcav, "ticksSinceLastExcav", 0);
        }
    }    
}