using RimWorld;
using ScavengingExpansion.Defs.DefModExtensions;
using Verse;

namespace ScavengingExpansion.Utils.Filters
{
    public class FilterBasicSalvageable : CustomFilter
    {
        protected override bool filterCondition(ThingDef def)
        {
            
            ThingDef parentDef = def?.ingestible.sourceDef;
            if (parentDef != null && parentDef.HasModExtension<ScavengingYield>())
            {
                ScavengingYield scavengingYield = parentDef.GetModExtension<ScavengingYield>();
                bool result = scavengingYield.isMechanoid && scavengingYield.scrapYield > 0;
                return result;
            }
            return false;
        }
    }
}