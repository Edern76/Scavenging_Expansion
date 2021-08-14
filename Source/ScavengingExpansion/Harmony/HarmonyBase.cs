using HarmonyLib;

namespace ScavengingExpansion.Harmony
{
    public class HarmonyBase
    {
        private static HarmonyLib.Harmony harmonyInstance = new HarmonyLib.Harmony("com.scavengingexpansion.patch");

        public static void ApplyPatches()
        {
            harmonyInstance.PatchAll();
        }
    }
}