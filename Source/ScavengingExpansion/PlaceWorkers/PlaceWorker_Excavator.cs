using System;
using System.Collections.Generic;
using System.Linq;
using ScavengingExpansion.Utils.Grids;
using UnityEngine;
using Verse;

namespace ScavengingExpansion.PlaceWorkers
{
    public class PlaceWorker_Excavator : PlaceWorker
    {
        public const float PLACEABLE_ROCK_THRESHOLD = 0.8f; //TODO : make this settable in building def or in mod settings

        //Adapted from https://github.com/Ogliss/Quarry/blob/master/1.3/Source/Quarry/PlaceWorkers/PlaceWorker_Quarry.cs
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null,
            Thing thing = null)
        {
            ExcavatorGrid grid = map.GetComponent<ExcavatorGrid>();

            if (grid == null)
            {
                throw new NullReferenceException("Current map has no excavator grid");
            }    
            
            IEnumerable<IntVec3> occupiedCells = GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size).ToList();
            IEnumerable<IntVec3> rockCells = occupiedCells.Where(cell => grid.GetCellBool(cell)).ToList();

            float rockRatio = (float)rockCells.Count() / occupiedCells.Count();
            if (rockRatio < PLACEABLE_ROCK_THRESHOLD)
            {
                String message = "There is not enough rocky ground at this location to place an excavator.";
                #if DEBUG
                    message += $" (RockCells : {rockCells.Count()} | OccupiedCells : {occupiedCells.Count()} | RockRatio : {rockRatio} | Threshold : {PLACEABLE_ROCK_THRESHOLD})";
                    Log.Message(message);
                #endif
                return message;
            }
            else
            {
                return true;
            }
        }

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            Find.CurrentMap.GetComponent<ExcavatorGrid>().MarkForDraw();
            GenDraw.DrawFieldEdges(GenAdj.CellsOccupiedBy(center, rot, def.Size).ToList());
        }
    }
}