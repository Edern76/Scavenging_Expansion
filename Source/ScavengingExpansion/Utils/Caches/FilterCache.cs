using ScavengingExpansion.Utils.Filters;
using Verse;

namespace ScavengingExpansion.Utils.Caches
{
    public static class FilterCache
    {
        private static ThingFilter _basicSalvageableFilter = null;
        private static ThingFilter _alloySalvageableFilter = null;
        private static ThingFilter _advancedSalvageableFilter = null;
        
        public static ThingFilter BasicSalvageableFixedFilter
        {
            get
            {
                if (_basicSalvageableFilter is null)
                {
                    _basicSalvageableFilter = new FilterBasicSalvageable().GetFixedFilter();
                }

                return _basicSalvageableFilter;
            }
        }
        
        public static ThingFilter AlloySalvageableFixedFilter
        {
            get
            {
                if (_alloySalvageableFilter is null)
                {
                    _alloySalvageableFilter = new FilterAlloySalvageable().GetFixedFilter();
                }

                return _alloySalvageableFilter;
            }
        }
        
        public static ThingFilter AdvancedSalvageableFixedFilter
        {
            get
            {
                if (_advancedSalvageableFilter is null)
                {
                    _advancedSalvageableFilter = new FilterAdvancedSalvageable().GetFixedFilter();
                }

                return _advancedSalvageableFilter;
            }
        }
    }
}