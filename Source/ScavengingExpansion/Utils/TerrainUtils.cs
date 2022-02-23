using RimWorld;
using Verse;

namespace ScavengingExpansion.Utils
{
    public static class TerrainUtils
    {
        //Adapted from https://github.com/Ogliss/Quarry/blob/master/1.3/Source/Quarry/Utils/QuarryUtility.cs
        public static bool IsRockyTerrain(TerrainDef terrainDef, bool allowGravel = false)
        {
            return (allowGravel && terrainDef == TerrainDefOf.Gravel)
                   || terrainDef.defName.EndsWith("_Rough")
                   || terrainDef.defName.EndsWith("_RoughHewn")
                   || terrainDef.defName.EndsWith("_Smooth");
        }
    }
}