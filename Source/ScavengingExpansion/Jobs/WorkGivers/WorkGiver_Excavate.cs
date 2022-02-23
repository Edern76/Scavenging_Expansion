using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using ScavengingExpansion.Buildings;
using ScavengingExpansion.Comps;
using ScavengingExpansion.DefOfs;
using Verse;
using Verse.AI;
using JobDefOf = ScavengingExpansion.DefOfs.JobDefOf;

namespace ScavengingExpansion.Jobs.WorkGivers
{
    public class WorkGiver_Excavate : WorkGiver_Scanner
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t == null)
            {
                Log.Error("Thing is null in WorkGiver excavator !");
                return null;
            }
            
            if (pawn == null)
            {
                Log.Error("Pawn is null in WorkGiver excavator !");
                return null;
            }
            
            
            Building_Excavator excavator = t as Building_Excavator;
            if (pawn.workSettings == null || !pawn.workSettings.WorkIsActive(JobDefOf.Excavating))
            {
                return null;
            }

            CompPowerTrader powerTrader = excavator.TryGetComp<CompPowerTrader>();
            if (powerTrader != null && !powerTrader.PowerOn)
            {
                #if DEBUG
                    Log.Message($"Excavator {excavator} is turned off.");
                #endif
                return null;
            }
            
            if (excavator == null)
            {
                return null;
            }

            CompAutoExcavator compAutoExcavator = excavator.TryGetComp<CompAutoExcavator>();
            if (compAutoExcavator != null && !compAutoExcavator.Props.AllowManualExcavating)
            {
                return null;
            }    

            if (!pawn.CanReserve(excavator, 1))
            {
                return null;
            }    
            
            return new Job(JobDefOf.SE_Excavate, excavator);
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            ListerBuildings buildings = pawn.Map.listerBuildings;
            return buildings.AllBuildingsColonistOfDef(BuildingDefOf.SE_Excavator).Cast<Thing>();
        }
    }
}