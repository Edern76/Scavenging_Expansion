using Verse;

namespace ScavengingExpansion.Comps
{
    public class CompProperties_Jammable : CompProperties
    {
        public CompProperties_Jammable()
        {
            this.compClass = typeof(CompJammable);
        }

        public float JamChancePerShot = 0.05f;
    }

    public class CompJammable : ThingComp
    {
        public CompProperties_Jammable Props => (CompProperties_Jammable)this.props;
    }    
}