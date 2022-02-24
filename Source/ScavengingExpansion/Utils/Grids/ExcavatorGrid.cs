using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ScavengingExpansion.Utils.Grids
{
    //Adapated from https://github.com/Ogliss/Quarry/blob/master/1.3/Source/Quarry/Utils/QuarryGrid.cs
    public class ExcavatorGrid : MapComponent, ICellBoolGiver, IExposable
    {
        private BoolGrid _boolGrid;
        private CellBoolDrawer _drawer;
        private int _mapMaxClusters;

        public Color Color => Color.green; //Color to use for available cells display when placing building
        public bool GetCellBool(int index) => _boolGrid[index];
        public bool GetCellBool(IntVec3 cell) => _boolGrid[cell];
        public Color GetCellExtraColor(int index) => Color.white;
        

        public ExcavatorGrid(Map map) : base(map)
        {
            _boolGrid = new BoolGrid(map);
            _drawer = new CellBoolDrawer(this, map.Size.x, map.Size.z);
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            _mapMaxClusters = Rand.RangeInclusive(1, 3); //TODO : Set min and max range in mod settings
            #if DEBUG
                Log.Message($"Max map clusters : {_mapMaxClusters}");
            #endif
            if (_boolGrid.TrueCount == 0)
            {
                CalculateBoolGrid();
            }    
        }

        private void CalculateBoolGrid()
        {
            foreach (IntVec3 cell in map.AllCells)
            {
                if (TerrainUtils.IsRockyTerrain(map.terrainGrid.TerrainAt(cell), false))
                {
                    AddToGrid(cell, false); //TODO : Make it so only a few subsets of rocky tiles are valid
                }    
            }

            if (_boolGrid.TrueCount > 0)
            {
                List<List<IntVec3>> clusters = ExtractClusters();
                List<IntVec3> finalCells = clusters.Take(_mapMaxClusters).SelectMany(x => x).ToList();

                foreach (IntVec3 cell in GetCellsInGrid().Where(c => !finalCells.Contains(c)))
                {
                    RemoveFromGrid(cell, false);
                }
            }

            _drawer.SetDirty();
        }

        private List<List<IntVec3>> ExtractClusters()
        {
            List<List<IntVec3>> clusters = new List<List<IntVec3>>();
            List<IntVec3> clustersFlattened = new List<IntVec3>();
            bool clustersModified = false;
            foreach (IntVec3 cell in GetCellsInGrid())
            {
                if (clustersModified) //Avoids having to recalculate flatList for every cell
                {
                    clustersFlattened = clusters.SelectMany(x => x).ToList();
                    clustersModified = false;
                }

                if (!clustersFlattened.Contains(cell))
                {
                    List<IntVec3> currentCluster = MapUtils.FloodFill(cell, map, GetCellBool);
                    clusters.Add(currentCluster);
                    clustersModified = true;
                }    
                   
            }

            return clusters;
        }

        public void AddToGrid(IntVec3 cell, bool isDirty = true)
        {
            _boolGrid.Set(cell, true);
            if (isDirty)
            {
                _drawer.SetDirty();
            }    
        }

        public void AddToGrid(CellRect rect, bool isDirty = true)
        {
            foreach (IntVec3 cell in rect.Cells)
            {
                AddToGrid(cell, false); //Ensures we set the drawer to dirty only once
            }

            if (isDirty)
            {
                _drawer.SetDirty();
            }    
        }

        public void RemoveFromGrid(IntVec3 cell, bool isDirty = true)
        {
            _boolGrid.Set(cell, false);
            if (isDirty)
            {
                _drawer.SetDirty();
            }    
        }

        public void RemoveFromGrid(CellRect rect, bool isDirty = true)
        {
            foreach (IntVec3 cell in rect.Cells)
            {
                RemoveFromGrid(cell, false);
            }

            if (isDirty)
            {
                _drawer.SetDirty();
            }    
        }

        public IEnumerable<IntVec3> GetCellsInGrid()
        {
            return map.AllCells.Where(c => GetCellBool(c));
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
            _drawer.CellBoolDrawerUpdate();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref _boolGrid, "boolGrid", new Object[0]); //Save/Load grid with current save
            Scribe_Values.Look(ref _mapMaxClusters, "mapMaxClusters");
        }

        public void MarkForDraw()
        {
            if (map == Find.CurrentMap)
            {
                _drawer.MarkForDraw();
            }    
        }
    }
}