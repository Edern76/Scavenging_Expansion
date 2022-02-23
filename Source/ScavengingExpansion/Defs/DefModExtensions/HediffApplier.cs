using System.Collections.Generic;
using ScavengingExpansion.Defs.Tags;
using Verse;

namespace ScavengingExpansion.Defs.DefModExtensions
{
    public class HediffApplier : DefModExtension
    {
        public HediffDef hediffDef;
        public float baseMinValue = 0f;
        public float baseMaxValue = 0f;
        public bool independantRolls = false;
        public List<RangeWithChance> additionalSeverityStages;
    }
}