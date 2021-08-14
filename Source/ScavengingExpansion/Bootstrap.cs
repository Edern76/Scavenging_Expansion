using RimWorld;
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
            Log.Message("Hello World !");
        }
    }
}