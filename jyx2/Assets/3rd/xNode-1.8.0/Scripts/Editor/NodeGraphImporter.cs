using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using XNode;

namespace XNodeEditor {
    /// <summary> Deals with modified assets </summary>
    class NodeGraphImporter : AssetPostprocessor {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            foreach (string path in importedAssets) {
                // Skip processing anything without the .asset extension
                if (Path.GetExtension(path) != ".asset") continue;

                // Get the object that is requested for deletion
                NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(path);
                if (graph == null) continue;

                // Get attributes
                Type graphType = graph.GetType();
                NodeGraph.RequireNodeAttribute[] attribs = Array.ConvertAll(
                    graphType.GetCustomAttributes(typeof(NodeGraph.RequireNodeAttribute), true), x => x as NodeGraph.RequireNodeAttribute);

                Vector2 position = Vector2.zero;
                foreach (NodeGraph.RequireNodeAttribute attrib in attribs) {
                    if (attrib.type0 != null) AddRequired(graph, attrib.type0, ref position);
                    if (attrib.type1 != null) AddRequired(graph, attrib.type1, ref position);
                    if (attrib.type2 != null) AddRequired(graph, attrib.type2, ref position);
                }
            }
        }

        private static void AddRequired(NodeGraph graph, Type type, ref Vector2 position) {
            if (!graph.nodes.Any(x => x.GetType() == type)) {
                XNode.Node node = graph.AddNode(type);
                node.position = position;
                position.x += 200;
                if (node.name == null || node.name.Trim() == "") node.name = NodeEditorUtilities.NodeDefaultName(type);
                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(graph))) AssetDatabase.AddObjectToAsset(node, graph);
            }
        }
    }
}