using Verse;

namespace ScavengingExpansion.Comps
{
    public class CompProperties_HediffWhenHeld : CompProperties
    {
        public HediffDef Hediff = null;
        public CompProperties_HediffWhenHeld()
        {
            this.compClass = typeof(CompHediffWhenHeld);
        }    
    }

    public class CompHediffWhenHeld : ThingComp
    {
        public CompProperties_HediffWhenHeld Props => (CompProperties_HediffWhenHeld)this.props;

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);
            if (Props.Hediff == null || pawn.health.hediffSet.GetFirstHediffOfDef(Props.Hediff) != null) return;

            pawn.health.AddHediff(Props.Hediff);
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            base.Notify_Unequipped(pawn);
            Hediff hediff;
            if (Props.Hediff == null || (hediff = pawn.health.hediffSet.GetFirstHediffOfDef(Props.Hediff)) == null) return;
            
            pawn.health.RemoveHediff(hediff);
        }
    }
}