using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ScavengingExpansion.Utils
{
    public static class MapUtils
    {
        public static List<IntVec3> FloodFill(IntVec3 StartPosition, Map Map, Predicate<IntVec3> ToCheck)
        {
            List<IntVec3> result = new List<IntVec3>();
            FloodFillR(StartPosition, CellRect.WholeMap(Map), ToCheck, ref result);
            return result;
        }

        private static void FloodFillR(IntVec3 Position, CellRect MapRect, Predicate<IntVec3> ToCheck,
            ref List<IntVec3> ResultArrray)
        {
            if (ResultArrray.Contains(Position) || !MapRect.Contains(Position) || !ToCheck(Position))
            {
                return;
            }
            else
            {
                ResultArrray.Add(Position);
                FloodFillR(Position + IntVec3.North, MapRect, ToCheck, ref ResultArrray);
                FloodFillR(Position + IntVec3.South, MapRect, ToCheck, ref ResultArrray);
                FloodFillR(Position + IntVec3.East, MapRect, ToCheck, ref ResultArrray);
                FloodFillR(Position + IntVec3.West, MapRect, ToCheck, ref ResultArrray);
            }
        }   
    }
}