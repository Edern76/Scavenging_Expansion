using Verse;

namespace ScavengingExpansion.Utils
{
    public static class ThingUtils
    {
        public static Thing makeThingWithCount(ThingDef def, int amount)
        {
            Thing toMake = ThingMaker.MakeThing(def);
            toMake.stackCount = amount;
            return toMake;
        }
    }
}