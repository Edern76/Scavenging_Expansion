using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Verse;

namespace ScavengingExpansion.Utils
{
    public static class HediffUtils
    {
        public static IEnumerable<Hediff> GetPartHediffsFromDef(Pawn pawn, BodyPartDef partDef)
        {
            if (partDef == null)
            {
                Log.Error("PartDef is null !");
                return new List<Hediff>();
            }

            if (pawn == null || pawn.health == null || pawn.health.hediffSet == null ||
                pawn.health.hediffSet.hediffs == null)
            {
                Log.Error("Error with pawn or pawn health");
                return new List<Hediff>();
            }    
            return pawn.health.hediffSet.hediffs.Where(hediff => hediff != null & hediff.Part != null && hediff.Part.def != null && hediff.Part.def == partDef);
        }

        public static void AddOrUpdateHediffWithSeverity(Pawn pawn, HediffDef hediff, float severity)
        {
            Pawn_HealthTracker health = pawn.health;
            HediffSet hediffSet = health.hediffSet;
            if (hediffSet.GetFirstHediffOfDef(hediff) == null)
            {
                health.AddHediff(hediff);
            }

            hediffSet.GetFirstHediffOfDef(hediff).Severity += severity;

        }
    }
}