using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerSpatialPartitioningData<T> where T : GPUInstancerCell
    {   
        public int cellRowAndCollumnCountPerTerrain;
        public List<T> activeCellList;

        private Dictionary<int, T> _cellHashList;
        private List<T> _cellList;

        public GPUInstancerSpatialPartitioningData()
        {
            activeCellList = new List<T>();
            _cellHashList = new Dictionary<int, T>();
            _cellList = new List<T>();
        }

        public void AddCell(T cell)
        {
            _cellHashList.Add(cell.CalculateHash(), cell);
            _cellList.Add(cell);
        }

        public bool GetCell(int hash, out T cell)
        {
            return _cellHashList.TryGetValue(hash, out cell);
        }

        public List<T> GetCellList()
        {
            return _cellList;
        }

        public void GetCell(T cell)
        {
            _cellHashList.Add(cell.CalculateHash(), cell);
            _cellList.Add(cell);
        }

        public bool IsActiveCellUpdateRequired(Vector3 position)
        {
            return CalculateActiveCells(position);
        }

        public bool CalculateActiveCells(Vector3 position)
        {
            bool result = false;
            foreach (T cell in _cellList)
            {
                if (cell.cellBounds.Contains(position))
                {
                    if (!cell.isActive)
                    {
                        cell.isActive = true;
                        activeCellList.Add(cell);
                        result = true;
                    }
                }
                else
                {
                    if (cell.isActive)
                    {
                        cell.isActive = false;
                        GPUInstancerUtility.ReleaseSPCell(cell);
                        activeCellList.Remove(cell);
                        result = true;
                    }
                }
            }

            return result;
        }
    }
}