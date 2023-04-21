using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class SectorGrid3D<T>
    {
        public FastIndexList<Sector3D<T>> sectorList = new FastIndexList<Sector3D<T>>();
        public Sector3D<T>[,,] sectors;
        public Rect rect;

        public Int3 sectorCount;

        public Vector3 sectorGridOffset;
        public Vector3 sectorSize, halfSectorSize;
        public Vector3 invSectorSize;
        public Vector3 totalSize;
        public Vector3 halfTotalSize;

        public SectorGrid3D(Int3 sectorCount, Vector3 sectorSize, Vector3 sectorGridOffset)
        {
            sectors = new Sector3D<T>[sectorCount.x, sectorCount.y, sectorCount.z];
            this.sectorCount = sectorCount;
            this.sectorSize = sectorSize;
            this.sectorGridOffset = sectorGridOffset;

            invSectorSize = Mathw.Divide(1.0f, sectorSize);
            halfSectorSize = sectorSize / 2;

            totalSize = Mathw.Scale(sectorSize, sectorCount);
            halfTotalSize = totalSize * 0.5f;

            rect = new Rect(sectorGridOffset - halfTotalSize, totalSize);
        }

        public void GetSectors(FastList<Sector3D<T>> list, Vector3 pos, float radius)
        {
            list.FastClear();

            Int3 startSector = GetSectorIndex(new Vector3(pos.x - radius, pos.y - radius, pos.z - radius));
            Int3 endSector = GetSectorIndex(new Vector3(pos.x + radius, pos.y + radius, pos.z + radius));

            for (int z = startSector.z; z < endSector.z; z++)
            {
                for (int y = startSector.y; y <= endSector.y; y++)
                {
                    for (int x = startSector.x; x <= endSector.x; x++)
                    {
                        if (sectors[x, y, z] == null) continue;

                        list.Add(sectors[x, y, z]);
                    }
                }
            }
        }

        public void GetOrCreateSector(Vector3 pos, out Sector3D<T> sector)
        {
            Int3 s = GetSectorIndex(pos);

            // Debug.Log("Cell " + c.ToString() + " " + pos);

            sector = sectors[s.x, s.y, s.z];
            if (sector == null) sector = CreateSector(ref s);
        }

        public Int3 GetSectorIndex(Vector3 pos)
        {
            pos += -sectorGridOffset + halfTotalSize + halfSectorSize;
            pos.x *= invSectorSize.x; pos.y *= invSectorSize.y; pos.z *= invSectorSize.z;

            return new Int3((int)pos.x, (int)pos.y, (int)pos.z);
        }

        public Sector3D<T> GetSector(Vector3 pos)
        {
            Int3 s = GetSectorIndex(pos);

            return sectors[s.x, s.y, s.z];
        }

        public Sector3D<T> CreateSector(ref Int3 s)
        {
            var sector = new Sector3D<T>();
            sector.bounds = new Bounds(new Vector3(s.x * sectorSize.x, s.y * sectorSize.y, s.z * sectorSize.z) + (sectorGridOffset - halfTotalSize), sectorSize);
            
            sectors[s.x, s.y, s.z] = sector;
            sectorList.Add(sector);
            return sector;
        }

        public void RemoveSector(Vector3 pos)
        {
            Int3 s = GetSectorIndex(pos);
            sectorList.Remove(sectors[s.x, s.y, s.z]);
            sectors[s.x, s.y, s.z] = null;
        }

        public void RemoveSector(Int3 sectorIndex)
        {
            sectorList.Remove(sectors[sectorIndex.x, sectorIndex.y, sectorIndex.z]);
            sectors[sectorIndex.x, sectorIndex.y, sectorIndex.z] = null;
        }

        public void Reset()
        {
            //for (int y = 0; y < sectorCount.y; y++)
            //{
            //    for (int x = 0; x < sectorCount.x; x++)
            //    {
            //        sectors[x, y] = null;
            //    }
            //}
            sectors = new Sector3D<T>[sectorCount.y, sectorCount.x, sectorCount.z];

            sectorList.Clear();
        }

        public void Draw()
        {
            // Gizmos.DrawWireCube(new Vector3(rect.center.x, 220, rect.center.y), new Vector3(rect.size.x, 0, rect.size.y));

            DrawSectors(sectorList, Color.white);
        }

        public void DrawSectors(FastList<Sector3D<T>> sectors, Color color)
        {
            Gizmos.color = color;

            for (int i = 0; i < sectors.Count; i++)
            {
                Sector3D<T> sector = sectors.items[i];
                Bounds bounds = sector.bounds;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }

    public class Sector3D<T> : FastIndex
    {
        public T list;
        public Bounds bounds;
    }
}