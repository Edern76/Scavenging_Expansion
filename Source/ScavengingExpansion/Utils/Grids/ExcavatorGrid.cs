using UnityEngine;
using Verse;

namespace ScavengingExpansion.Utils.Grids
{
    //Adapated from https://github.com/Ogliss/Quarry/blob/master/1.3/Source/Quarry/Utils/QuarryGrid.cs
    public class ExcavatorGrid : MapComponent, ICellBoolGiver, IExposable
    {
        private BoolGrid _boolGrid;
        private CellBoolDrawer _drawer;

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
            _drawer.SetDirty();
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

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
            _drawer.CellBoolDrawerUpdate();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref _boolGrid, "boolGrid", new Object[0]); //Save/Load grid with current save
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