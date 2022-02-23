using System.Linq;
using RimWorld;
using ScavengingExpansion.Defs;
using ScavengingExpansion.Harmony;
using Verse;

namespace ScavengingExpansion
{
    [StaticConstructorOnStartup]
    public static class Bootstrap
    {
        static Bootstrap()
        {
            HarmonyBase.ApplyPatches();
            foreach (ScavengingRecipeDef def in DefDatabase<RecipeDef>.AllDefs.Where(def => def is ScavengingRecipeDef)
                .Cast<ScavengingRecipeDef>())
            {
                def.ResolveFilters();
            }    
            Log.Message("Hello World !");
        }
    }
}