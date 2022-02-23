using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;
using PawnKindDefOf = ScavengingExpansion.DefOfs.PawnKindDefOf;

namespace ScavengingExpansion.GenSteps
{
    public class GenStep_MechanoidProbe : GenStep
    {
        public const int RECT_DIVIDER = 10;
        public const int RECT_SIZE_FACTOR = 8;
        public const int ACTUAL_RECT_SIZE = 20;
        public const int FORAGERS_NUMBER = 6;
        
        public override int SeedPart => 73326129;


        protected CellRect GetRandomRectInsideSpawnArea(CellRect spawnRect) => new CellRect(
            Rand.RangeInclusive(spawnRect.minX + 10, spawnRect.maxX - (ACTUAL_RECT_SIZE + 10)),
            Rand.RangeInclusive(spawnRect.minZ + 10, spawnRect.maxZ - (ACTUAL_RECT_SIZE + 10)), ACTUAL_RECT_SIZE,
            ACTUAL_RECT_SIZE);
        
        public override void Generate(Map map, GenStepParams parms)
        {
            BaseGen.globalSettings.map = map;
            
            CellRect spawnRect = new CellRect(map.Size.x / RECT_DIVIDER, map.Size.z / RECT_DIVIDER,
                RECT_SIZE_FACTOR * map.Size.x / RECT_DIVIDER, RECT_SIZE_FACTOR * map.Size.z / RECT_DIVIDER);
            spawnRect.ClipInsideMap(map);
            CellRect actualRect = GetRandomRectInsideSpawnArea(spawnRect);
            actualRect.ClipInsideMap(map);
            ResolveParams gathererParams = GenerateGatherer(map, actualRect);

            BaseGen.symbolStack.Push("pawn", gathererParams);

            for (int i = 0; i < FORAGERS_NUMBER; i++)
            {
                ResolveParams foragerParams = GenerateForager(map, actualRect);
                BaseGen.symbolStack.Push($"pawn", foragerParams);
            }

            BaseGen.Generate();
        }

        private ResolveParams GenerateMech(PawnKindDef pawnKind, Map map, CellRect rect)
        {
            ResolveParams result = default(ResolveParams);
            
            Faction faction = Faction.OfMechanoids;
            Pawn pawn = PawnGenerator.GeneratePawn(pawnKind, faction);
            LordJob lordJob = new LordJob_DefendPoint(rect.CenterCell, null, false, false);
            Lord lord = LordMaker.MakeNewLord(faction, lordJob, map, null);
            
            
            result.rect = rect;
            result.faction = faction;
            result.singlePawnToSpawn = pawn;
            result.singlePawnLord = lord;
            
            return result;
        }
        
        protected ResolveParams GenerateGatherer(Map map, CellRect rect)
        {
            return GenerateMech(PawnKindDefOf.SE_Mech_Gatherer, map, rect);
        }

        protected ResolveParams GenerateForager(Map map, CellRect rect)
        {
            return GenerateMech(PawnKindDefOf.SE_Mech_Forager, map, rect);
        }
    }
}