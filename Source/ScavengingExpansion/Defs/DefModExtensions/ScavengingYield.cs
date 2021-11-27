using System.Collections.Generic;
using RimWorld;
using ScavengingExpansion.Defs.Tags;
using Verse;

namespace ScavengingExpansion.Defs.DefModExtensions
{
    public class ScavengingYield : DefModExtension
    {
        public int scrapYield = 0;
        public List<AdvancedScavengingProduct> advancedYield = new List<AdvancedScavengingProduct>();
        public bool isMechanoid = true;
    }
}