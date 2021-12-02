using System.Collections.Generic;
using RimWorld;
using ScavengingExpansion.Enums;
using ScavengingExpansion.Utils.Caches;
using Verse;


namespace ScavengingExpansion.Defs
{
    public class ScavengingRecipeDef : RecipeDef
    {
        public List<ScavengingRecipeType> scavengingTypes;

        public void ResolveFilters()
        {
            base.ResolveReferences();
            ThingFilter toApply = null;
            if (scavengingTypes.Contains(ScavengingRecipeType.Advanced))
            {
                toApply = FilterCache.AdvancedSalvageableFixedFilter;
            }    
            else if (scavengingTypes.Contains(ScavengingRecipeType.Alloy))
            {
                toApply = FilterCache.AlloySalvageableFixedFilter;
            }    
            else if (scavengingTypes.Contains(ScavengingRecipeType.Basic))
            {
                toApply = FilterCache.BasicSalvageableFixedFilter;
            }    
            
            if (toApply != null)
            {
                if ((this.fixedIngredientFilter is null || this.fixedIngredientFilter.AllowedDefCount == 0) && (this.defaultIngredientFilter is null || this.defaultIngredientFilter.AllowedDefCount == 0))
                {
                    this.fixedIngredientFilter = toApply;
                    this.defaultIngredientFilter = toApply;
                }
            }    
            
        }
    }
}