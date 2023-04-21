using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MeshCombineStudio
{
    public class BaseOctree
    {
        public class Cell
        {
            public Cell mainParent;
            public Cell parent;
            // public Cell[] cells;
            public bool[] cellsUsed;

            public Bounds bounds;
            public int cellIndex;
            public int cellCount;
            public int level = 0;
            public int maxLevels;

            public Cell() { }

            public Cell(Vector3 position, Vector3 size, int maxLevels)
            {
                bounds = new Bounds(position, size);
                this.maxLevels = maxLevels;
            }

            public Cell(Cell parent, int cellIndex, Bounds bounds)
            {
                if (parent != null)
                {
                    maxLevels = parent.maxLevels;
                    mainParent = parent.mainParent;
                    level = parent.level + 1;
                }

                this.parent = parent;
                this.cellIndex = cellIndex;
                this.bounds = bounds;
            }

            public void SetCell(Cell parent, int cellIndex, Bounds bounds)
            {
                if (parent != null)
                {
                    maxLevels = parent.maxLevels;
                    mainParent = parent.mainParent;
                    level = parent.level + 1;
                }

                this.parent = parent;
                this.cellIndex = cellIndex;
                this.bounds = bounds;
            }

            protected int AddCell<T, U>(ref T[] cells, Vector3 position, out bool maxCellCreated) where T : Cell, new() where U : Cell, new()
            {
                Vector3 localPos = position - bounds.min;

                int x = (int)(localPos.x / bounds.extents.x);
                int y = (int)(localPos.y / bounds.extents.y);
                int z = (int)(localPos.z / bounds.extents.z);

                int index = x + (y * 4) + (z * 2);

                AddCell<T, U>(ref cells, index, x, y, z, out maxCellCreated);

                return index;
            }

            protected T GetCell<T>(T[] cells, Vector3 position)
            {
                if (cells == null) return default(T);

                Vector3 localPos = position - bounds.min;

                int x = (int)(localPos.x / bounds.extents.x);
                int y = (int)(localPos.y / bounds.extents.y);
                int z = (int)(localPos.z / bounds.extents.z);

                int index = x + (y * 4) + (z * 2);

                return cells[index];
            }

            protected void AddCell<T, U>(ref T[] cells, int index, int x, int y, int z, out bool maxCellCreated) where T : Cell, new() where U : Cell, new()
            {
                if (cells == null) { cells = new T[8]; }
                if (cellsUsed == null) { cellsUsed = new bool[8]; }

                // Reporter.Log("index "+index+" position "+localPos+" x: "+x+" y: "+y+" z: "+z+" extents "+bounds.extents);

                if (!cellsUsed[index])
                {
                    Bounds subBounds = new Bounds(new Vector3(bounds.min.x + (bounds.extents.x * (x + 0.5f)), bounds.min.y + (bounds.extents.y * (y + 0.5f)), bounds.min.z + (bounds.extents.z * (z + 0.5f))), bounds.extents);

                    if (level == maxLevels - 1)
                    {
                        cells[index] = new U() as T;
                        cells[index].SetCell(this, index, subBounds);
                        maxCellCreated = true;
                    }
                    else
                    {
                        maxCellCreated = false;
                        cells[index] = new T();
                        cells[index].SetCell(this, index, subBounds);
                    }

                    cellsUsed[index] = true;
                    ++cellCount;
                }
                else maxCellCreated = false;
            }

            //public void RemoveCell(int index)
            //{
            //    cells[index] = null;
            //    cellsUsed[index] = false;
            //    --cellCount;
            //    if (cellCount == 0)
            //    {
            //        if (parent != null) parent.RemoveCell(cellIndex);
            //    }
            //}

            public bool InsideBounds(Vector3 position)
            {
                position -= bounds.min;
                if (position.x >= bounds.size.x || position.y >= bounds.size.y || position.z >= bounds.size.z || position.x <= 0 || position.y <= 0 || position.z <= 0) { return false; }
                return true;
            } //===============================================================================================================================

            public void Reset(ref Cell[] cells)
            {
                cells = null;
                cellsUsed = null;
            }
        }
    }
}