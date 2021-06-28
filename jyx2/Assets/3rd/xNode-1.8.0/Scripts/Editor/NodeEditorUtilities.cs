using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XNodeEditor {
    /// <summary> A set of editor-only utilities and extensions for xNode </summary>
    public static class NodeEditorUtilities {

        /// <summary>C#'s Script Icon [The one MonoBhevaiour Scripts have].</summary>
        private static Texture2D scriptIcon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);

        /// Saves Attribute from Type+Field for faster lookup. Resets on recompiles.
        private static Dictionary<Type, Dictionary<string, Dictionary<Type, Attribute>>> typeAttributes = new Dictionary<Type, Dictionary<string, Dictionary<Type, Attribute>>>();

        /// Saves ordered PropertyAttribute from Type+Field for faster lookup. Resets on recompiles.
        private static Dictionary<Type, Dictionary<string, List<PropertyAttribute>>> typeOrderedPropertyAttributes = new Dictionary<Type, Dictionary<string, List<PropertyAttribute>>>();

        public static bool GetAttrib<T>(Type classType, out T attribOut) where T : Attribute {
            object[] attribs = classType.GetCustomAttributes(typeof(T), false);
            return GetAttrib(attribs, out attribOut);
        }

        public static bool GetAttrib<T>(object[] attribs, out T attribOut) where T : Attribute {
            for (int i = 0; i < attribs.Length; i++) {
                if (attribs[i] is T) {
                    attribOut = attribs[i] as T;
                    return true;
                }
            }
            attribOut = null;
            return false;
        }

        public static bool GetAttrib<T>(Type classType, string fieldName, out T attribOut) where T : Attribute {
            // If we can't find field in the first run, it's probably a private field in a base class.
            FieldInfo field = classType.GetFieldInfo(fieldName);
            // This shouldn't happen. Ever.
            if (field == null) {
                Debug.LogWarning("Field " + fieldName + " couldnt be found");
                attribOut = null;
                return false;
            }
            object[] attribs = field.GetCustomAttributes(typeof(T), true);
            return GetAttrib(attribs, out attribOut);
        }

        public static bool HasAttrib<T>(object[] attribs) where T : Attribute {
            for (int i = 0; i < attribs.Length; i++) {
                if (attribs[i].GetType() == typeof(T)) {
                    return true;
                }
            }
            return false;
        }

        public static bool GetCachedAttrib<T>(Type classType, string fieldName, out T attribOut) where T : Attribute {
            Dictionary<string, Dictionary<Type, Attribute>> typeFields;
            if (!typeAttributes.TryGetValue(classType, out typeFields)) {
                typeFields = new Dictionary<string, Dictionary<Type, Attribute>>();
                typeAttributes.Add(classType, typeFields);
            }

            Dictionary<Type, Attribute> typeTypes;
            if (!typeFields.TryGetValue(fieldName, out typeTypes)) {
                typeTypes = new Dictionary<Type, Attribute>();
                typeFields.Add(fieldName, typeTypes);
            }

            Attribute attr;
            if (!typeTypes.TryGetValue(typeof(T), out attr)) {
                if (GetAttrib<T>(classType, fieldName, out attribOut)) {
                    typeTypes.Add(typeof(T), attribOut);
                    return true;
                } else typeTypes.Add(typeof(T), null);
            }

            if (attr == null) {
                attribOut = null;
                return false;
            }

            attribOut = attr as T;
            return true;
        }

        public static List<PropertyAttribute> GetCachedPropertyAttribs(Type classType, string fieldName) {
            Dictionary<string, List<PropertyAttribute>> typeFields;
            if (!typeOrderedPropertyAttributes.TryGetValue(classType, out typeFields)) {
                typeFields = new Dictionary<string, List<PropertyAttribute>>();
                typeOrderedPropertyAttributes.Add(classType, typeFields);
            }

            List<PropertyAttribute> typeAttributes;
            if (!typeFields.TryGetValue(fieldName, out typeAttributes)) {
                FieldInfo field = classType.GetFieldInfo(fieldName);
                object[] attribs = field.GetCustomAttributes(typeof(PropertyAttribute), true);
                typeAttributes = attribs.Cast<PropertyAttribute>().Reverse().ToList(); //Unity draws them in reverse
                typeFields.Add(fieldName, typeAttributes);
            }

            return typeAttributes;
        }

        public static bool IsMac() {
#if UNITY_2017_1_OR_NEWER
            return SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX;
#else
            return SystemInfo.operatingSystem.StartsWith("Mac");
#endif
        }

        /// <summary> Returns true if this can be casted to <see cref="Type"/></summary>
        public static bool IsCastableTo(this Type from, Type to) {
            if (to.IsAssignableFrom(from)) return true;
            var methods = from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(
                    m => m.ReturnType == to &&
                    (m.Name == "op_Implicit" ||
                        m.Name == "op_Explicit")
                );
            return methods.Count() > 0;
        }

        /// <summary> Return a prettiefied type name. </summary>
        public static string PrettyName(this Type type) {
            if (type == null) return "null";
            if (type == typeof(System.Object)) return "object";
            if (type == typeof(float)) return "float";
            else if (type == typeof(int)) return "int";
            else if (type == typeof(long)) return "long";
            else if (type == typeof(double)) return "double";
            else if (type == typeof(string)) return "string";
            else if (type == typeof(bool)) return "bool";
            else if (type.IsGenericType) {
                string s = "";
                Type genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(List<>)) s = "List";
                else s = type.GetGenericTypeDefinition().ToString();

                Type[] types = type.GetGenericArguments();
                string[] stypes = new string[types.Length];
                for (int i = 0; i < types.Length; i++) {
                    stypes[i] = types[i].PrettyName();
                }
                return s + "<" + string.Join(", ", stypes) + ">";
            } else if (type.IsArray) {
                string rank = "";
                for (int i = 1; i < type.GetArrayRank(); i++) {
                    rank += ",";
                }
                Type elementType = type.GetElementType();
                if (!elementType.IsArray) return elementType.PrettyName() + "[" + rank + "]";
                else {
                    string s = elementType.PrettyName();
                    int i = s.IndexOf('[');
                    return s.Substring(0, i) + "[" + rank + "]" + s.Substring(i);
                }
            } else return type.ToString();
        }

        /// <summary> Returns the default name for the node type. </summary>
        public static string NodeDefaultName(Type type) {
            string typeName = type.Name;
            // Automatically remove redundant 'Node' postfix
            if (typeName.EndsWith("Node")) typeName = typeName.Substring(0, typeName.LastIndexOf("Node"));
            typeName = UnityEditor.ObjectNames.NicifyVariableName(typeName);
            return typeName;
        }

        /// <summary> Returns the default creation path for the node type. </summary>
        public static string NodeDefaultPath(Type type) {
            string typePath = type.ToString().Replace('.', '/');
            // Automatically remove redundant 'Node' postfix
            if (typePath.EndsWith("Node")) typePath = typePath.Substring(0, typePath.LastIndexOf("Node"));
            typePath = UnityEditor.ObjectNames.NicifyVariableName(typePath);
            return typePath;
        }

        /// <summary>Creates a new C# Class.</summary>
        [MenuItem("Assets/Create/xNode/Node C# Script", false, 89)]
        private static void CreateNode() {
            string[] guids = AssetDatabase.FindAssets("xNode_NodeTemplate.cs");
            if (guids.Length == 0) {
                Debug.LogWarning("xNode_NodeTemplate.cs.txt not found in asset database");
                return;
            }
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            CreateFromTemplate(
                "NewNode.cs",
                path
            );
        }

        /// <summary>Creates a new C# Class.</summary>
        [MenuItem("Assets/Create/xNode/NodeGraph C# Script", false, 89)]
        private static void CreateGraph() {
            string[] guids = AssetDatabase.FindAssets("xNode_NodeGraphTemplate.cs");
            if (guids.Length == 0) {
                Debug.LogWarning("xNode_NodeGraphTemplate.cs.txt not found in asset database");
                return;
            }
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            CreateFromTemplate(
                "NewNodeGraph.cs",
                path
            );
        }

        public static void CreateFromTemplate(string initialName, string templatePath) {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<DoCreateCodeFile>(),
                initialName,
                scriptIcon,
                templatePath
            );
        }

        /// Inherits from EndNameAction, must override EndNameAction.Action
        public class DoCreateCodeFile : UnityEditor.ProjectWindowCallback.EndNameEditAction {
            public override void Action(int instanceId, string pathName, string resourceFile) {
                Object o = CreateScript(pathName, resourceFile);
                ProjectWindowUtil.ShowCreatedAsset(o);
            }
        }

        /// <summary>Creates Script from Template's path.</summary>
        internal static UnityEngine.Object CreateScript(string pathName, string templatePath) {
            string className = Path.GetFileNameWithoutExtension(pathName).Replace(" ", string.Empty);
            string templateText = string.Empty;

            UTF8Encoding encoding = new UTF8Encoding(true, false);

            if (File.Exists(templatePath)) {
                /// Read procedures.
                StreamReader reader = new StreamReader(templatePath);
                templateText = reader.ReadToEnd();
                reader.Close();

                templateText = templateText.Replace("#SCRIPTNAME#", className);
                templateText = templateText.Replace("#NOTRIM#", string.Empty);
                /// You can replace as many tags you make on your templates, just repeat Replace function
                /// e.g.:
                /// templateText = templateText.Replace("#NEWTAG#", "MyText");

                /// Write procedures.

                StreamWriter writer = new StreamWriter(Path.GetFullPath(pathName), false, encoding);
                writer.Write(templateText);
                writer.Close();

                AssetDatabase.ImportAsset(pathName);
                return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            } else {
                Debug.LogError(string.Format("The template file was not found: {0}", templatePath));
                return null;
            }
        }
    }
}
