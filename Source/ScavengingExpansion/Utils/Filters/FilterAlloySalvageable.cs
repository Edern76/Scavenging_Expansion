using RimWorld;
using ScavengingExpansion.Defs.DefModExtensions;
using Verse;

namespace ScavengingExpansion.Utils.Filters
{
    public class FilterAlloySalvageable : CustomFilter
    {
        protected override bool filterCondition(ThingDef def)
        {
            ThingDef parentDef = def?.ingestible.sourceDef;
            
            if (parentDef != null &&parentDef.HasModExtension<ScavengingYield>())
            {
                ScavengingYield scavengingYield = parentDef.GetModExtension<ScavengingYield>();
                return scavengingYield.isMechanoid && scavengingYield.alloyYield.max > 0;
            }

            return false;
        }
    }
}