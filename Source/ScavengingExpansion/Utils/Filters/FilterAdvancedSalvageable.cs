using RimWorld;
using ScavengingExpansion.Defs.DefModExtensions;
using Verse;

namespace ScavengingExpansion.Utils.Filters
{
    public class FilterAdvancedSalvageable : CustomFilter
    {
        protected override bool filterCondition(ThingDef def)
        {
            
            ThingDef parentDef = def?.ingestible.sourceDef;
            if (parentDef != null && parentDef.HasModExtension<ScavengingYield>())
            {
                ScavengingYield scavengingYield = parentDef.GetModExtension<ScavengingYield>();
                return scavengingYield.isMechanoid && scavengingYield.advancedYield.Any();
            }

            return false;
        }
    }
}