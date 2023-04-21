using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class CombinedLODManager : MonoBehaviour
    {
        public enum LodMode { Automatic, DebugLod }
        public enum LodDistanceMode { Automatic, Manual }

        public bool drawGizmos = true;
        public LOD[] lods;
        public float[] distances;
        public LodDistanceMode lodDistanceMode;
        public LodMode lodMode;
        public int showLod = 0;
        public bool lodCulled;
        public float lodCullDistance = 500;
        
        public Vector3 octreeCenter = Vector3.zero;
        public Vector3 octreeSize = new Vector3(256, 256, 256);
        public int maxLevels = 4;
        public bool search = true;
        Cell octree;

        Transform cameraMainT;

        void Awake()
        {
            cameraMainT = Camera.main.transform;
        }

        void InitOctree()
        {
            octree = new Cell(octreeCenter, octreeSize, maxLevels);
        }

        private void Start()
        {
            if (search)
            {
                search = false;
                InitOctree();
                Search();
            }
        }

        void Update()
        {
            if (octree.cellsUsed != null) Lod(lodMode);
        }
        
        public void UpdateLods(MeshCombiner meshCombiner, int lodAmount)
        {
            if (lods != null && lods.Length == lodAmount) return;
            
            lods = new LOD[lodAmount];
            float[] newDistances = new float[lodAmount];

            for (int i = 0; i < lods.Length; i++)
            {
                lods[i] = new LOD();

                if (lodDistanceMode == LodDistanceMode.Automatic) newDistances[i] = meshCombiner.cellSize * i;
                else if (distances != null && i < distances.Length) newDistances[i] = distances[i];
            }

            distances = newDistances;
        }

        public void UpdateDistances(MeshCombiner meshCombiner)
        {
            if (lodDistanceMode != LodDistanceMode.Automatic) return;

            for (int i = 0; i < distances.Length; i++) distances[i] = meshCombiner.cellSize * i;
        }

        public void Search()
        {
            for (int i = 0; i < lods.Length; i++)
            {
                lods[i].searchParent.gameObject.SetActive(true);
                MeshRenderer[] mrs = lods[i].searchParent.GetComponentsInChildren<MeshRenderer>();

                for (int j = 0; j < mrs.Length; j++) octree.AddMeshRenderer(mrs[j], mrs[j].transform.position, i, lods.Length);
            }
        }

        public void ResetOctree()
        {
            if (octree == null) return;
            octree.cells = null;
            octree.cellsUsed = null;

            for (int i = 0; i < lods.Length; i++)
            {
                if (lods[i].searchParent != null) Destroy(lods[i].searchParent.gameObject);
            }
        }

        public void Lod(LodMode lodMode)
        {
            Vector3 cameraPosition = cameraMainT.position;

            for (int i = 0; i < lods.Length - 1; i++)
            {
                lods[i].sphere.center = cameraPosition;
                lods[i].sphere.radius = distances[i + 1];
            }

            // if (Benchmark.active) lodBenchmark.Start();
            if (lodMode == LodMode.Automatic) octree.AutoLodInternal(lods, lodCulled ? lodCullDistance : -1);
            else octree.LodInternal(lods, showLod);
            // if (Benchmark.active) lodBenchmark.Stop();
        }

        private void OnDrawGizmosSelected()
        {
            if (drawGizmos && octree != null && octree.cells != null) octree.DrawGizmos(lods);
        }

        [System.Serializable]
        public class LOD
        {
            public Transform searchParent;
            public Sphere3 sphere = new Sphere3();

            public LOD() { }
            public LOD(Transform searchParent)
            {
                this.searchParent = searchParent;
            }
        }

        public class Cell : BaseOctree.Cell
        {
            public Cell[] cells;
            AABB3 box;

            public Cell() { }
            public Cell(Vector3 position, Vector3 size, int maxLevels) : base(position, size, maxLevels) { }

            public void AddMeshRenderer(MeshRenderer mr, Vector3 position, int lodLevel, int lodLevels)
            {
                if (InsideBounds(position)) AddMeshRendererInternal(mr, position, lodLevel, lodLevels);
            }

            void AddMeshRendererInternal(MeshRenderer mr, Vector3 position, int lodLevel, int lodLevels)
            {
                if (level == maxLevels)
                {
                    MaxCell thisCell = (MaxCell)this;
                    if (thisCell.mrList == null) thisCell.mrList = new List<MeshRenderer>[lodLevels];
                    List<MeshRenderer>[] mrList = thisCell.mrList;

                    if (mrList[lodLevel] == null) mrList[lodLevel] = new List<MeshRenderer>();
                    mrList[lodLevel].Add(mr);
                    thisCell.currentLod = -1;
                }
                else
                {
                    bool maxCellCreated;
                    int index = AddCell<Cell, MaxCell>(ref cells, position, out maxCellCreated);
                    cells[index].box = new AABB3(cells[index].bounds.min, cells[index].bounds.max);
                    cells[index].AddMeshRendererInternal(mr, position, lodLevel, lodLevels);
                }
            }

            public void AutoLodInternal(LOD[] lods, float lodCulledDistance)
            {
                if (level == maxLevels)
                {
                    MaxCell thisCell = (MaxCell)this;
                    if (lodCulledDistance != -1)
                    {
                        float squareDistance = (bounds.center - lods[0].sphere.center).sqrMagnitude;
                        if (squareDistance > lodCulledDistance * lodCulledDistance)
                        {
                            if (thisCell.currentLod != -1)
                            {
                                for (int i = 0; i < lods.Length; i++)
                                {
                                    for (int j = 0; j < thisCell.mrList[i].Count; j++) thisCell.mrList[i][j].enabled = false;
                                }
                                thisCell.currentLod = -1;
                            }
                            return;
                        }
                    }

                    for (int lodIndex = 0; lodIndex < lods.Length; lodIndex++)
                    {
                        bool intersect;
                        if (lodIndex < lods.Length - 1) intersect = Mathw.IntersectAABB3Sphere3(box, lods[lodIndex].sphere);
                        else intersect = true;

                        if (intersect)
                        {
                            if (thisCell.currentLod != lodIndex)
                            {
                                for (int i = 0; i < lods.Length; i++)
                                {
                                    bool active = (i == lodIndex);
                                    for (int j = 0; j < thisCell.mrList[i].Count; j++) thisCell.mrList[i][j].enabled = active;
                                }
                                thisCell.currentLod = lodIndex;
                            }
                            break;
                        }
                    }
                }
                else for (int i = 0; i < 8; ++i) if (cellsUsed[i]) cells[i].AutoLodInternal(lods, lodCulledDistance);
            }

            public void LodInternal(LOD[] lods, int lodLevel)
            {
                if (level == maxLevels)
                {
                    MaxCell thisCell = (MaxCell)this;
                    if (thisCell.currentLod != lodLevel)
                    {
                        for (int i = 0; i < lods.Length; i++)
                        {
                            bool active = (i == lodLevel);
                            for (int j = 0; j < thisCell.mrList[i].Count; j++) thisCell.mrList[i][j].enabled = active;
                        }

                        thisCell.currentLod = lodLevel;
                    }
                }
                else for (int i = 0; i < 8; ++i) if (cellsUsed[i]) cells[i].LodInternal(lods, lodLevel);
            } //===============================================================================================================================

            public void DrawGizmos(LOD[] lods)
            {
                for (int i = 0; i < lods.Length; i++)
                {
                    if (i == 0) Gizmos.color = Color.red;
                    else if (i == 1) Gizmos.color = Color.green;
                    else if (i == 2) Gizmos.color = Color.yellow;
                    else if (i == 3) Gizmos.color = Color.blue;

                    Gizmos.DrawWireSphere(lods[i].sphere.center, lods[i].sphere.radius);
                }

                DrawGizmosInternal();
            }

            public void DrawGizmosInternal()
            {
                if (level == maxLevels)
                {
                    MaxCell thisCell = (MaxCell)this;
                    if (thisCell.currentLod == 0) Gizmos.color = Color.red;
                    else if (thisCell.currentLod == 1) Gizmos.color = Color.green;
                    else if (thisCell.currentLod == 2) Gizmos.color = Color.yellow;
                    else if (thisCell.currentLod == 3) Gizmos.color = Color.blue;

                    Gizmos.DrawWireCube(bounds.center, bounds.size - new Vector3(0.25f, 0.25f, 0.25f));

                    Gizmos.color = Color.white;
                }
                else for (int i = 0; i < 8; ++i) if (cellsUsed[i]) cells[i].DrawGizmosInternal();
            }
        }

        public class MaxCell : Cell
        {
            public List<MeshRenderer>[] mrList;
            public int currentLod;
        }
    }
}