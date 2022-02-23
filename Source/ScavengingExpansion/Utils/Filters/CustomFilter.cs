using System;
using System.Linq;
using RimWorld;
using Verse;

namespace ScavengingExpansion.Utils.Filters
{
    public abstract class CustomFilter
    {
        protected abstract bool filterCondition(ThingDef def);
        
        protected ThingFilter getBaseFilter()
        {
            ThingFilter result = new ThingFilter();
            if (!ThingCategoryNodeDatabase.initialized)
            {
                Log.Error("Cannot get ThingCategories from unitialized database");
            }    
            foreach (ThingDef thingDef in ThingCategoryDefOf.Corpses.DescendantThingDefs.Where(def => this.filterCondition(def)))
            {
                result.SetAllow(thingDef, true);
            }

            result.ResolveReferences();
            return result;
        }

        public ThingFilter GetFixedFilter() => getBaseFilter();

        public ThingFilter GetDefaultFilter() => getBaseFilter();

    }
}