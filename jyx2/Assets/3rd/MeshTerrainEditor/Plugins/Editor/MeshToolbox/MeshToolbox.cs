using MTE.Undo;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MTE
{
    internal class MeshToolbox : IEditor
    {
        public int Id { get; } = 9;

        public bool Enabled { get; set; } = true;

        public string Name { get; } = "MeshToolbox";

        public Texture Icon { get; } =
            EditorGUIUtility.IconContent("Mesh Icon").image;

        public bool WantMouseMove { get; } = false;

        public bool WillEditMesh { get; } = true;

        private enum SelectMode
        {
            Vertex,
            Quad,
        }

        private enum EditorState
        {
            Selecting,
            Action,
        }

        /// <summary>
        /// AABB that can contain only one point
        /// </summary>
        private class AABB
        {
            float xMin = float.NaN, yMin, zMin;
            float xMax, yMax, zMax;

            public Vector3 Min { get { return new Vector3(xMin, yMin, zMin); } }
            public Vector3 Max { get { return new Vector3(xMax, yMax, zMax); } }
            public Vector3 Center { get { return 0.5f*(Min + Max); } }
            public bool IsEmpty { get { return float.IsNaN(xMin); } }

            /// <summary>Reset to initial state (empty) </summary>
            public void Reset()
            {
                xMin = float.NaN;
            }

            public void AddPoint(Vector3 point)
            {
                if (IsEmpty)
                {
                    this.xMin = this.xMax = point.x;
                    this.yMin = this.yMax = point.y;
                    this.zMin = this.zMax = point.z;
                }

                if (point.x > xMax)
                {
                    this.xMax = point.x;
                }
                else if(point.x < xMin)
                {
                    this.xMin = point.x;
                }
                if (point.y > yMax)
                {
                    this.yMax = point.y;
                }
                else if (point.y < yMin)
                {
                    this.yMin = point.y;
                }
                if (point.z > zMax)
                {
                    this.zMax = point.z;
                }
                else if (point.z < zMin)
                {
                    this.zMin = point.z;
                }
            }
        }

        private class VertexModifyGroup
        {
            private readonly GameObject gameObject;
            private List<int> vertexIndexList;

            public GameObject Target { get { return gameObject; } }

            public List<int> VertexIndexList
            {
                get { return vertexIndexList;}
            }

            public VertexModifyGroup(GameObject gameObject)
            {
                this.gameObject = gameObject;
            }

            public void AppendVertices(IList<int> vertexIndexList)
            {
                if (this.vertexIndexList == null)
                {
                    this.vertexIndexList = new List<int>();
                }
                this.vertexIndexList.AddRange(vertexIndexList);
                this.vertexIndexList = this.vertexIndexList.Distinct().ToList();
            }

            public void RemoveVertices(IList<int> remove_vertexIndexList)
            {
                if (this.vertexIndexList == null)
                {
                    return;
                }

                for (var i = this.VertexIndexList.Count - 1; i >= 0; i--)
                {
                    var vertexIndex = this.VertexIndexList[i];
                    var foundIndex = remove_vertexIndexList.IndexOf(vertexIndex);
                    if (foundIndex >= 0)
                    {
                        this.VertexIndexList.RemoveAt(i);
                        remove_vertexIndexList.RemoveAt(foundIndex);
                    }
                }
            }

            public void Clear()
            {
                this.VertexIndexList.Clear();
            }
        }


        #region modfication

        private readonly List<VertexModifyGroup> vertexModifyGroups = new List<VertexModifyGroup>(4);

        private readonly AABB aabb = new AABB();

        private void AddVerticesToModifyGroup(GameObject gameObject, IList<int> vertexIndexList)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException("gameObject");
            }
            if (vertexIndexList == null)
            {
                throw new ArgumentNullException("vertexIndexList");
            }

            if (vertexIndexList.Count == 0)
            {
                return;
            }

            {
                var modifyGroup = vertexModifyGroups.Find(group => group.Target == gameObject);
                if (modifyGroup == null)
                {
                    modifyGroup = new VertexModifyGroup(gameObject);
                    vertexModifyGroups.Add(modifyGroup);
                }
                modifyGroup.AppendVertices(vertexIndexList);
            }
        }

        private void RemoveVerticesFromModifyGroup(GameObject gameObject, IList<int> vertexIndexList)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException("gameObject");
            }
            if (vertexIndexList == null)
            {
                throw new ArgumentNullException("vertexIndexList");
            }

            if (vertexIndexList.Count == 0)
            {
                return;
            }

            {
                var modifyGroup = vertexModifyGroups.Find(group => group.Target == gameObject);
                if (modifyGroup == null)
                {
                    modifyGroup = new VertexModifyGroup(gameObject);
                    vertexModifyGroups.Add(modifyGroup);
                }
                modifyGroup.RemoveVertices(vertexIndexList);
            }
        }

        private void RefreshAABB()
        {
            aabb.Reset();
            for (var i = 0; i < vertexModifyGroups.Count; i++)
            {
                var modifyGroup = vertexModifyGroups[i];
                var gameObject = modifyGroup.Target;
                if (!gameObject)
                {
                    continue;
                }
                var transform = gameObject.transform;
                var meshFilter = gameObject.GetComponent<MeshFilter>();
                if (!meshFilter)
                {
                    continue;
                }
                var mesh = meshFilter.sharedMesh;
                if (!mesh)
                {
                    continue;
                }
                var meshVertices = mesh.vertices;
                for (int j = 0; j < modifyGroup.VertexIndexList.Count; j++)
                {
                    var vertexIndex = modifyGroup.VertexIndexList[j];
                    if (vertexIndex >= meshVertices.Length)
                    {
                        MTEDebug.LogError(
                            $"Mesh {mesh.name}'s vertices on GameObject has been changed.");
                        continue;
                    }
                    var vertexPosition = meshVertices[vertexIndex];
                    var pWorld = transform.TransformPoint(vertexPosition);
                    aabb.AddPoint(pWorld);
                }
            }
        }

        #endregion

        #region Parameters

        #region Constant
        // default
        const float DefaultBrushSize = 4;
        // min/max
        const float MinBrushSize = 0.1f;
        const float MaxBrushSize = 10f;
        #endregion

        private float brushSize;

        /// <summary>
        /// Brush size (unit: meter)
        /// </summary>
        public float BrushSize
        {
            get { return brushSize; }
            set
            {
                value = Mathf.Clamp(value, MinBrushSize, MaxBrushSize);
                if (!MathEx.AmostEqual(value, brushSize))
                {
                    brushSize = value;
                    EditorPrefs.SetFloat("MTE_MeshToolbox.brushSize", value);
                }
            }
        }

        private float BrushSizeInU3D => BrushSize * Settings.BrushUnit;
        #endregion

        public MeshToolbox()
        {
            MTEContext.EnableEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadSavedParamter();
                }
            };

            MTEContext.EditTypeChangedEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadSavedParamter();
                }
            };

            brushSize = DefaultBrushSize;
        }
        
        public HashSet<Hotkey> DefineHotkeys()
        {
            return new HashSet<Hotkey>
            {
                new Hotkey(this, KeyCode.LeftBracket, () =>
                {
                    BrushSize -= 1;
                    MTEEditorWindow.Instance.Repaint();
                }),
                new Hotkey(this, KeyCode.RightBracket, () =>
                {
                    BrushSize += 1;
                    MTEEditorWindow.Instance.Repaint();
                })
            };
        }

        private void LoadSavedParamter()
        {
            // Load parameters from the EditorPrefs
            brushSize = EditorPrefs.GetFloat("MTE_MeshToolbox.brushSize", DefaultBrushSize);
        }

        public string Header{ get { return StringTable.Get(C.MeshMisc_Header); } }
        public string Description { get { return StringTable.Get(C.MeshMisc_Description); } }

        private static class Styles
        {
            private static bool unloaded= true;

            public static void Init()
            {
                if (!unloaded) return;
                //nothing for now
                unloaded = false;
            }
        }

        private bool pressedHotkeyOnEditor;
        public void DoArgsGUI()
        {
            Styles.Init();
            if (!Settings.CompactGUI)
            {
                GUILayout.Label(StringTable.Get(C.Settings), MTEStyles.SubHeader);
            }
            BrushSize = EditorGUILayoutEx.Slider(StringTable.Get(C.Size), "-", "+", BrushSize, MinBrushSize, MaxBrushSize);
            if (!Settings.CompactGUI)
            {
                EditorGUILayout.Space();
                GUILayout.Label(StringTable.Get(C.Tools), MTEStyles.SubHeader);
            }
            
            mode = (SelectMode)GUILayout.Toolbar((int)mode, MTEStyles.MeshSelectModeContents, Settings.CompactGUI ? GUILayout.Height(32) : GUILayout.Height(48));

            if (mode == SelectMode.Vertex)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUI.enabled = CanDelete();
                    if (GUILayout.Button(StringTable.Get(C.Delete), GUILayout.Width(100), GUILayout.Height(40)))
                    {
                        Delete();
                    }
                    GUI.enabled = true;
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField(StringTable.Get(C.Info_ToolDescription_DeleteVertices), MTEStyles.labelFieldWordwrap);
                }
                EditorGUILayout.EndHorizontal();
            }
            else if (mode == SelectMode.Quad)
            {
                EditorGUILayout.HelpBox(StringTable.Get(C.Info_SwapQuadSplitDirection), MessageType.Info);
                if (IsPressedHotkeyForQuadMode(Event.current))
                {
                    pressedHotkeyOnEditor = true;
                }
            }
        }

        private EditorState state = EditorState.Selecting;
        private SelectMode mode = SelectMode.Vertex;
        
        //debug only data
        private static Vector3 pTest;
        private static Vector3 pMaxEdgePoint0;
        private static Vector3 pMaxEdgePoint1;
        //debug only data

        public void OnSceneGUI()
        {
            var e = Event.current;
            if (e.commandName == "UndoRedoPerformed")
            {
                SceneView.RepaintAll();
                return;
            }

            if (mode == SelectMode.Vertex)
            {
                OnSceneGUIForVertex();
            }
            else if(mode == SelectMode.Quad)
            {
                OnSceneGUIForQuad();

                if (Settings.DebugMode)
                {
                    var oldHandlesColor = Handles.color;
                    Handles.color = Color.red;
                    var size = Utility.GetHandleSize(pTest);
                    Handles.DotHandleCap(0, pTest, Quaternion.identity, size * Settings.PointSize, EventType.Repaint);
                    Handles.DrawLine(pMaxEdgePoint0, pMaxEdgePoint1);
                    if (e.type == EventType.MouseDown && e.button == 1)
                    {
                        pMaxEdgePoint1 = pMaxEdgePoint0 = pTest = new Vector3(-9999, -9999, -9999);
                    }
                    Handles.color = oldHandlesColor;
                }
            }
        }

        private void OnSceneGUIForQuad()
        {
            var e = Event.current;

            if (!(EditorWindow.mouseOverWindow is SceneView))
            {
                return;
            }

            if (e.control || e.alt)
                return;
            
            HandleUtility.AddDefaultControl(0);
            RaycastHit raycastHit;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out raycastHit,
                    Mathf.Infinity,
                    1 << MTEContext.TargetLayer //only hit target layer
                ))
            {
                //check tag
                if (!raycastHit.transform.CompareTag(MTEContext.TargetTag))
                {
                    return;
                }
                
                var transform = raycastHit.transform;
                var target = transform.gameObject;
                var meshFilter = target.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    return;
                }
                var mesh = meshFilter.sharedMesh;
                if (mesh == null)
                {
                    return;
                }

                var triangleIndex0 = raycastHit.triangleIndex * 3 + 0;
                var triangleIndex1 = raycastHit.triangleIndex * 3 + 1;
                var triangleIndex2 = raycastHit.triangleIndex * 3 + 2;

                // highlight hovered quad
                if (QuadMap.FindQuad(target, triangleIndex0, triangleIndex1, triangleIndex2, out Quad quad))
                {
                    Handles.color = Settings.FlashAffectedVertex ? Utility.GetFlashingColor(0.583f) : Color.blue;
                    
                    var meshVertices = mesh.vertices;
                    int index0 = quad.index[0];
                    int index1 = quad.index[1];
                    int index2 = quad.index[2];
                    var p0 = transform.TransformPoint(meshVertices[index0]);
                    var p1 = transform.TransformPoint(meshVertices[index1]);
                    var p2 = transform.TransformPoint(meshVertices[index2]);

                    int index3 = quad.index[3];
                    int index4 = quad.index[4];
                    int index5 = quad.index[5];
                    var p3 = transform.TransformPoint(meshVertices[index3]);
                    var p4 = transform.TransformPoint(meshVertices[index4]);
                    var p5 = transform.TransformPoint(meshVertices[index5]);

                    var oldHandlesColor = Handles.color;
                    if (Settings.DebugMode)
                    {
                        Handles.color = Color.green;
                        Handles.DrawPolyLine(p0, p1, p2, p0);
                        Handles.color = Color.blue;
                        Handles.DrawPolyLine(p3, p4, p5, p3);
                    }
                    else
                    {
                        Handles.color = Utility.GetFlashingColor();
                        //Handles.DrawAAPolyLine(3, 4, p0, p1, p2, p0);
                        //Handles.DrawAAPolyLine(3, 4, p3, p4, p5, p3);
                        Handles.DrawPolyLine(p0, p1, p2, p0);
                        Handles.DrawPolyLine(p3, p4, p5, p3);
                    }

                    Handles.color = oldHandlesColor;
                }

                bool pressed = IsPressedHotkeyForQuadMode(e) || pressedHotkeyOnEditor;
                if (pressed)
                {
                    pressedHotkeyOnEditor = false;
                    ChangeQuadSplitDirection(target, triangleIndex0, triangleIndex1, triangleIndex2);

                    var meshCollider = target.GetComponent<MeshCollider>();
                    MTEEditorWindow.Instance.UpdateMeshColliderImmediately(meshCollider);
                    MTEEditorWindow.Instance.HandleMeshSave();
                }
            }

            SceneView.RepaintAll();
        }

        private bool IsPressedHotkeyForQuadMode(Event e)
        {
            return e.isKey && e.type == EventType.KeyUp && e.keyCode == KeyCode.S;
        }

        private void ChangeQuadSplitDirection(GameObject target, int index0, int index1, int index2)
        {
            if (!QuadMap.FindQuad(target, index0, index1, index2, out Quad quad))
            {
                return;
            }

            Quad newQuad = quad;
            newQuad.ChangeSplitDirection();
            int firstIndex = -1;
            if (index0 % 6 == 0)
            {
                firstIndex = index0;
            }
            else if (index0 % 6 == 3)
            {
                firstIndex = index0 - 3;
            }
            else
            {
                throw new MTEEditException("Argument index0 is not a valid first index of triangle");
            }

            Utility.RecordMeshIndices("Swap Quad Split Direction", target, () => { QuadMap.Rebuild(target); });

            QuadMap.ModifyQuad(target, firstIndex, newQuad);
        }

        private void OnSceneGUIForVertex()
        {
            var e = Event.current;
            // show modifying points
            if (vertexModifyGroups.Count != 0)
            {
                Handles.color = Settings.FlashAffectedVertex ? Utility.GetFlashingColor(0.583f) : Color.blue;
                for (var i = 0; i < vertexModifyGroups.Count; i++)
                {
                    var modifyGroup = vertexModifyGroups[i];
                    if (modifyGroup.VertexIndexList == null || modifyGroup.VertexIndexList.Count == 0) continue;
                    if (!modifyGroup.Target) continue;
                    var meshFilter = modifyGroup.Target.GetComponent<MeshFilter>();
                    if (meshFilter == null) continue;
                    var mesh = meshFilter.sharedMesh;
                    if (mesh == null) continue;
                    var meshVertices = mesh.vertices;
                    var transform = modifyGroup.Target.transform;
                    for (int j = 0; j < modifyGroup.VertexIndexList.Count; j++)
                    {
                        var vertexIndex = modifyGroup.VertexIndexList[j];
                        var vertexPosition = meshVertices[vertexIndex];
                        var p = transform.TransformPoint(vertexPosition);
                        var size = Utility.GetHandleSize(p);
                        Handles.DotHandleCap(0, p, Quaternion.identity, size * Settings.PointSize, EventType.Repaint);
                    }
                }
            }

            if (!(EditorWindow.mouseOverWindow is SceneView))
            {
                return;
            }

            if (e.button != 0 || e.control || e.alt)
                return;

            if (state == EditorState.Selecting)
            {
                HandleUtility.AddDefaultControl(0);
                RaycastHit raycastHit;
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                //Debug.Log(string.Format("mouse at ({0}, {1})", e.mousePosition.x, e.mousePosition.y));
                if (Physics.Raycast(ray, out raycastHit,
                        Mathf.Infinity,
                        1 << MTEContext.TargetLayer //only hit target layer
                    ))
                {
                    //check tag
                    if (!raycastHit.transform.CompareTag(MTEContext.TargetTag))
                    {
                        return;
                    }

                    if (Settings.ShowBrushRect)
                    {
                        Utility.ShowBrushRect(raycastHit.point, BrushSizeInU3D);
                    }

                    if (e.isKey && e.keyCode == KeyCode.Return && vertexModifyGroups.Count != 0)
                    {
                        RefreshAABB();
                        state = EditorState.Action;
                        return;
                    }

                    // collect modify group
                    foreach (var gameObject in MTEContext.Targets)
                    {
                        if (!gameObject)
                        {
                            continue;
                        }

                        var meshFilter = gameObject.GetComponent<MeshFilter>();
                        var mesh = meshFilter.sharedMesh;

                        var hitPointLocal = gameObject.transform.InverseTransformPoint(raycastHit.point);
                        List<int> vIndex;
                        List<float> vDistance;
                        VertexMap.GetAffectedVertex(gameObject, hitPointLocal, this.BrushSizeInU3D,
                            out vIndex, out vDistance);

                        if (Settings.ShowAffectedVertex)
                        {
                            Utility.ShowAffectedVertices(gameObject, mesh, vIndex);
                        }

                        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                        {
                            if (vIndex.Count != 0)
                            {
                                if (!e.shift)
                                {
                                    AddVerticesToModifyGroup(gameObject, vIndex);
                                }
                                else
                                {
                                    RemoveVerticesFromModifyGroup(gameObject, vIndex);
                                }
                            }
                        }
                    }
                }
            }

            if (state == EditorState.Action)
            {
                if (e.isKey && e.keyCode == KeyCode.Escape)
                {
                    state = EditorState.Selecting;
                    return;
                }

                //record undo operation for targets that to be modified
                if (e.type == EventType.MouseDown)
                {
                    Utility.Record("Mesh Toolbox: move",
                        Vector3.zero, this.BrushSizeInU3D, () =>
                        {
                            RefreshAABB();
                            for (var i = 0; i < vertexModifyGroups.Count; i++)
                            {
                                var modifyGroup = vertexModifyGroups[i];
                                if (!modifyGroup.Target)
                                {
                                    continue;
                                }

                                VertexMap.Rebuild(modifyGroup.Target);
                            }
                        });
                }

                if (e.type == EventType.MouseUp)
                {
                    MTEEditorWindow.Instance.UpdateDirtyMeshCollidersImmediately();
                    MTEEditorWindow.Instance.HandleMeshSave();
                }

                // execute the modification
                if (vertexModifyGroups.Count != 0)
                {
                    var newPostion = Handles.DoPositionHandle(this.aabb.Center, Quaternion.identity);
                    var offset = newPostion - this.aabb.Center;
                    TranslateVertices(offset);
                }
            }

            SceneView.RepaintAll();
        }

        private void TranslateVertices(Vector3 offset)
        {
            // check if offset is big enough
            if (!(Mathf.Abs(offset.x) > 0.001f) && !(Mathf.Abs(offset.y) > 0.001f) && !(Mathf.Abs(offset.z) > 0.001f)) return;

            for (var i = 0; i < vertexModifyGroups.Count; i++)
            {
                var modifyGroup = vertexModifyGroups[i];
                if (modifyGroup.VertexIndexList == null
                    || modifyGroup.VertexIndexList.Count == 0
                    || !modifyGroup.Target)
                {
                    continue;
                }

                var gameObject = modifyGroup.Target;
                var transform = gameObject.transform;
                var offsetLocal = transform.InverseTransformDirection(offset);
                var mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
                var meshCollider = modifyGroup.Target.GetComponent<MeshCollider>();
                var vertices = mesh.vertices;//TODO performance improvement: mesh.vertices will copy vertices
                for (int j = 0; j < modifyGroup.VertexIndexList.Count; j++)
                {
                    var index = modifyGroup.VertexIndexList[j];
                    vertices[index] += offsetLocal;
                }
                mesh.vertices = vertices;

                MTEEditorWindow.Instance.SetMeshDirty(gameObject);
                MTEEditorWindow.Instance.SetMeshColliderDirty(meshCollider, mesh.vertexCount);

                // Rebuild vertex map for this GameObject if its vertices have been translated in xOz.
                if (Math.Abs(offset.x) > 0.001 || Math.Abs(offset.z) > 0.001)
                {
                    VertexMap.Rebuild(gameObject);
                }
            }

            RefreshAABB();
        }

        bool CanDelete()
        {
            if (vertexModifyGroups.Count == 0)
            {
                return false;
            }

            for (var i = 0; i < vertexModifyGroups.Count; i++)
            {
                var modifyGroup = vertexModifyGroups[i];
                if (modifyGroup.VertexIndexList.Count != 0)
                {
                    return true;
                }
            }
            return false;
        }

        void Delete()
        {
            using (new Undo.UndoTransaction("Mesh Toolbox: delete"))
            {
                for (var i = 0; i < vertexModifyGroups.Count; i++)
                {
                    var modifyGroup = vertexModifyGroups[i];
                    var gameObject = modifyGroup.Target;
                    if (!gameObject)
                    {
                        continue;
                    }

                    if (gameObject.GetComponent<VertexColorInitializer>() != null)//ignore vertex colored GameObjects
                    {
                        continue;
                    }
                    var mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
                    var meshCollider = modifyGroup.Target.GetComponent<MeshCollider>();
                    var meshTriangles = mesh.triangles;
                    var indexList = modifyGroup.VertexIndexList;

                    // create new indexes
                    List<int> newMeshTriangles = new List<int>(meshTriangles);
                    // remove triangles that contains the removed vertices
                    for (var j = newMeshTriangles.Count - 1; j >= 0; j -= 3)
                    {
                        var index0 = newMeshTriangles[j];
                        var index1 = newMeshTriangles[j - 1];
                        var index2 = newMeshTriangles[j - 2];
                        if (indexList.Contains(index0) || indexList.Contains(index1) || indexList.Contains(index2))
                        {
                            newMeshTriangles.RemoveAt(j);
                            newMeshTriangles.RemoveAt(j - 1);
                            newMeshTriangles.RemoveAt(j - 2);
                        }
                    }
                    var newTriangles = newMeshTriangles.ToArray();

                    // record undo operation for targets that to be modified
                    var newIndices = mesh.triangles;
                    UndoRedoManager.Instance().Push(a =>
                    {
                        mesh.ModifyIndices("Mesh Toolbox: delete", meshCollider, a, () =>
                        {
                            VertexMap.Rebuild(gameObject);
                        });
                        meshCollider.enabled = false;
                        meshCollider.enabled = true;
                    }, newIndices, "Mesh Toolbox: delete");

                    // assign
                    mesh.triangles = newTriangles;
                    MTEEditorWindow.Instance.SetMeshDirty(gameObject);

                    // rebuild vertex map
                    VertexMap.Rebuild(gameObject);

                    modifyGroup.Clear();
                }
            }
        }

    }
}