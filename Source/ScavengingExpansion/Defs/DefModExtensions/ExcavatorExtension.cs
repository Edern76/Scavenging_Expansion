using System;
using System.Collections.Generic;
using RimWorld;
using ScavengingExpansion.Defs.Tags;
using Verse;

namespace ScavengingExpansion.Defs.DefModExtensions
{
    public class ExcavatorExtension : DefModExtension
    {
        public String thingSetDef = "SE_DefaultExcavatorThingSet";
        public float buildingEfficiency = 1f;
        public float excavationCycleTime = 30f;
    }
}