using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using ES3Types;
using System.IO;
using ES3Internal;
using System.Text.RegularExpressions;

namespace ES3Editor
{
	public class TypesWindow : SubWindow
	{
		TypeListItem[] types = null;
		const int recentTypeCount = 5;
		List<int> recentTypes = new List<int>(recentTypeCount);

		Vector2 typeListScrollPos = Vector2.zero;
		Vector2 typePaneScrollPos = Vector2.zero;
		int leftPaneWidth = 300;

		string searchFieldValue = "";

		int selectedType = -1;
		private ES3Reflection.ES3ReflectedMember[] fields = new ES3Reflection.ES3ReflectedMember[0];
		private bool[] fieldSelected = new bool[0];

		private Texture2D checkmark;
		//private Texture2D checkmarkSmall;

		private GUIStyle searchBarStyle;
		private GUIStyle searchBarCancelButtonStyle;
		private GUIStyle leftPaneStyle;
		private GUIStyle typeButtonStyle;
		private GUIStyle selectedTypeButtonStyle;
		private GUIStyle selectAllNoneButtonStyle;

		private string valueTemplateFile;
		private string classTemplateFile;
		private string componentTemplateFile;
		private string scriptableObjectTemplateFile;

		private bool unsavedChanges = false;

		public TypesWindow(EditorWindow window) : base("Types", window){}

		public override void OnGUI()
		{
			if(types == null)
				Init();

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginVertical(leftPaneStyle);
			SearchBar();
			TypeList();
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
			TypePane();
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
		}

		private void SearchBar()
		{
			var style = EditorStyle.Get;

			GUILayout.Label("Enter a type name in the field below\n* Type names are case-sensitive *", style.subheading2);

			EditorGUILayout.BeginHorizontal();

			// Set control name so we can force a Focus reset for it.
			string currentSearchFieldValue = EditorGUILayout.TextField(searchFieldValue, searchBarStyle);

			if(searchFieldValue != currentSearchFieldValue)
			{
				searchFieldValue = currentSearchFieldValue;
				PerformSearch(currentSearchFieldValue);
			}

			GUI.SetNextControlName("Clear");

			if(GUILayout.Button("x", searchBarCancelButtonStyle))
			{
				searchFieldValue = "";
				GUI.FocusControl("Clear");
				PerformSearch("");
			}

			EditorGUILayout.EndHorizontal();
		}

		private void RecentTypeList()
		{
			if(!string.IsNullOrEmpty(searchFieldValue) || recentTypes.Count == 0)
				return;

			for(int i=recentTypes.Count-1; i>-1; i--)
				TypeButton(recentTypes[i]);

			EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);

		}

		private void TypeList()
		{
			if(!string.IsNullOrEmpty(searchFieldValue))
				GUILayout.Label("Search Results", EditorStyles.boldLabel);

			typeListScrollPos = EditorGUILayout.BeginScrollView(typeListScrollPos);

			RecentTypeList();

			if(!string.IsNullOrEmpty(searchFieldValue))
				for(int i = 0; i < types.Length; i++)
					TypeButton(i);

			EditorGUILayout.EndScrollView();
		}

		private void TypePane()
		{
			if(selectedType < 0)
				return;
			
			var style = EditorStyle.Get;

			var typeListItem = types[selectedType];
			var type = typeListItem.type;

			typePaneScrollPos = EditorGUILayout.BeginScrollView(typePaneScrollPos, style.area);

			GUILayout.Label(typeListItem.name, style.subheading);
			GUILayout.Label(typeListItem.namespaceName);

			EditorGUILayout.BeginVertical(style.area);

			bool hasParameterlessConstructor = ES3Reflection.HasParameterlessConstructor(type);
            bool isComponent = ES3Reflection.IsAssignableFrom(typeof(Component), type);

            string path = GetOutputPath(types[selectedType].type);
			// An ES3Type file already exists.
			if(File.Exists(path))
			{
				if(hasParameterlessConstructor || isComponent)
				{
					EditorGUILayout.BeginHorizontal();
					if(GUILayout.Button("Reset to Default"))
					{
						SelectNone(true, true);
						AssetDatabase.MoveAssetToTrash("Assets" + path.Remove(0, Application.dataPath.Length));
						SelectType(selectedType);
					}
					if(GUILayout.Button("Edit ES3Type Script"))
						AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath("Assets" + path.Remove(0, Application.dataPath.Length)));
					EditorGUILayout.EndHorizontal();
				}
				else
				{
					EditorGUILayout.HelpBox("This type has no public parameterless constructors.\n\nTo support this type you will need to modify the ES3Type script to use a specific constructor instead of the parameterless constructor.", MessageType.Info);
					if(GUILayout.Button("Click here to edit the ES3Type script"))
                        AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath("Assets" + path.Remove(0, Application.dataPath.Length)));
                    if (GUILayout.Button("Reset to Default"))
					{
						SelectAll(true, true);
						File.Delete(path);
						AssetDatabase.Refresh();
					}
				}
			}
			// No ES3Type file and no fields.
			else if(fields.Length == 0)
			{
				if(!hasParameterlessConstructor && !isComponent)
					EditorGUILayout.HelpBox("This type has no public parameterless constructors.\n\nTo support this type you will need to create an ES3Type script and modify it to use a specific constructor instead of the parameterless constructor.", MessageType.Info);
				
				if(GUILayout.Button("Create ES3Type Script"))
					Generate();
			}
			// No ES3Type file, but fields are selectable.
			else
			{
				if(!hasParameterlessConstructor && !isComponent)
				{
					EditorGUILayout.HelpBox("This type has no public parameterless constructors.\n\nTo support this type you will need to select the fields you wish to serialize below, and then modify the generated ES3Type script to use a specific constructor instead of the parameterless constructor.", MessageType.Info);
					if(GUILayout.Button("Select all fields and generate ES3Type script"))
					{
						SelectAll(true, false);
						Generate();
					}
				}
				else
				{
					if(GUILayout.Button("Create ES3Type Script"))
						Generate();
				}
			}
					
			EditorGUILayout.EndVertical();

			PropertyPane();

			EditorGUILayout.EndScrollView();
		}

		private void PropertyPane()
		{
			var style = EditorStyle.Get;

			EditorGUILayout.BeginVertical(style.area);

			GUILayout.Label("Fields", EditorStyles.boldLabel);

			DisplayFieldsOrProperties(true, false);
			EditorGUILayout.Space();

			GUILayout.Label("Properties", EditorStyles.boldLabel);

			DisplayFieldsOrProperties(false, true);
			EditorGUILayout.EndVertical();
		}

		private void DisplayFieldsOrProperties(bool showFields, bool showProperties)
		{
			// Get field and property counts.
			int fieldCount = 0;
			int propertyCount = 0;
			for(int i=0; i<fields.Length; i++)
			{
				if(fields[i].isProperty && showProperties)
					propertyCount++;
				else if((!fields[i].isProperty) && showFields)
					fieldCount++;
			}

			// If there is nothing to display, show message.
			if(showFields && showProperties && fieldCount == 0 && propertyCount == 0)
				GUILayout.Label("This type has no serializable fields or properties.");
			else if(showFields && fieldCount == 0)
				GUILayout.Label("This type has no serializable fields.");
			else if(showProperties && propertyCount == 0)
				GUILayout.Label("This type has no serializable properties.");

			// Display Select All/Select None buttons only if there are fields to display.
			if(fieldCount > 0 || propertyCount > 0)
			{
				EditorGUILayout.BeginHorizontal();

				if(GUILayout.Button("Select All", selectAllNoneButtonStyle))
				{
					SelectAll(showFields, showProperties);
					Generate();
				}

				if(GUILayout.Button("Select None", selectAllNoneButtonStyle))
				{
					SelectNone(showFields, showProperties);
					Generate();
				}
				EditorGUILayout.EndHorizontal();
			}

			for(int i=0; i<fields.Length; i++)
			{
				var field = fields[i];
				if((field.isProperty && !showProperties) || ((!field.isProperty) && !showFields))
					continue;

				EditorGUILayout.BeginHorizontal();

				var content = new GUIContent(field.Name);

				if(typeof(UnityEngine.Object).IsAssignableFrom(field.MemberType))
					content.tooltip = field.MemberType.ToString() + "\nSaved by reference";
				else
					content.tooltip = field.MemberType.ToString() + "\nSaved by value";



				bool selected = EditorGUILayout.ToggleLeft(content, fieldSelected[i]);
				if(selected != fieldSelected[i])
				{
					fieldSelected[i] = selected;
					unsavedChanges = true;
				}

				EditorGUILayout.EndHorizontal();
			}
		}

		// Selects all fields, properties or both.
		private void SelectAll(bool selectFields, bool selectProperties)
		{
			for(int i=0; i<fieldSelected.Length; i++)
				if((fields[i].isProperty && selectProperties) || (!fields[i].isProperty) && selectFields)
					fieldSelected[i] = true;
		}

		// Selects all fields, properties or both.
		private void SelectNone(bool selectFields, bool selectProperties)
		{
			for(int i=0; i<fieldSelected.Length; i++)
				if((fields[i].isProperty && selectProperties) || (!fields[i].isProperty) && selectFields)
					fieldSelected[i] = false;
		}

		public override void OnLostFocus()
		{
			if(unsavedChanges)
				Generate();
		}

		private void TypeButton(int i)
		{
			var type = types[i];
			if(!types[i].showInList)
				return;

			if(type.hasExplicitES3Type)
				EditorGUILayout.BeginHorizontal();


			var thisTypeButtonStyle = (i == selectedType) ? selectedTypeButtonStyle : typeButtonStyle;

			if(GUILayout.Button(new GUIContent(type.name, type.namespaceName), thisTypeButtonStyle))
				SelectType(i);

			// Set the cursor.
			var buttonRect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);



			if(type.hasExplicitES3Type)
			{
				GUILayout.Box(new GUIContent(checkmark, "Type is explicitly supported"), EditorStyles.largeLabel);
				EditorGUILayout.EndHorizontal();
			}
		}

		private void PerformSearch(string query)
		{
            var lowerCaseQuery = query.ToLowerInvariant();
			var emptyQuery = string.IsNullOrEmpty(query);

			for(int i=0; i<types.Length; i++)
				types[i].showInList = (emptyQuery || types[i].lowercaseName.Contains(lowerCaseQuery));
		}

        public void SelectType(Type type)
        {
            Init();
            for (int i = 0; i < types.Length; i++)
                if (types[i].type == type)
                    SelectType(i);
        }

		private void SelectType(int typeIndex)
		{
			selectedType = typeIndex;

			if(selectedType == -1)
			{
				SaveType("TypesWindowSelectedType",  -1);
				return;
			}

			SaveType("TypesWindowSelectedType",  selectedType);

			if(!recentTypes.Contains(typeIndex))
			{
				// If our recent type queue is full, remove an item before adding another.
				if(recentTypes.Count == recentTypeCount)
					recentTypes.RemoveAt(0);
				recentTypes.Add(typeIndex);
				for(int j=0; j<recentTypes.Count; j++)
					SaveType("TypesWindowRecentType"+j, recentTypes[j]); 
			}
				
			var type = types[selectedType].type;

			fields = ES3Reflection.GetSerializableMembers(type, false);
			fieldSelected = new bool[fields.Length];

			var es3Type = ES3TypeMgr.GetES3Type(type);
			// If there's no ES3Type for this, only select fields which are supported by reflection.
			if(es3Type == null)
			{
				var safeFields = ES3Reflection.GetSerializableMembers(type, true);
				for(int i=0; i<fields.Length; i++)
					fieldSelected[i] = safeFields.Any(item => item.Name == fields[i].Name);
				return;
			}

			// Get fields and whether they're selected.
			var selectedFields = new List<string>();
			var propertyAttributes = es3Type.GetType().GetCustomAttributes(typeof(ES3PropertiesAttribute), false);
			if(propertyAttributes.Length > 0)
				selectedFields.AddRange(((ES3PropertiesAttribute)propertyAttributes[0]).members);

			fieldSelected = new bool[fields.Length];

			for(int i=0; i<fields.Length; i++)
				fieldSelected[i] = selectedFields.Contains(fields[i].Name);
		}

		private void SaveType(string key, int typeIndex)
		{
			if(typeIndex == -1)
				return;
			SaveType(key, types[typeIndex].type);
		}

		private void SaveType(string key, Type type)
		{
			EditorPrefs.SetString(key, type.AssemblyQualifiedName);
		}

		private int LoadTypeIndex(string key)
		{
			string selectedTypeName = EditorPrefs.GetString(key, "");
			if(selectedTypeName != "")
			{
				var type = ES3Reflection.GetType(selectedTypeName);
				if(type != null)
				{
					int typeIndex = GetTypeIndex(type);
					if(typeIndex != -1)
						return typeIndex;
				}
			}
			return -1;
		}

		private int GetTypeIndex(Type type)
		{
			for(int i=0; i<types.Length; i++)
				if(types[i].type == type)
					return i;
			return -1;
		}

		private void Init()
		{
			componentTemplateFile = "ES3ComponentTypeTemplate.txt";
			classTemplateFile = "ES3ClassTypeTemplate.txt";
			valueTemplateFile = "ES3ValueTypeTemplate.txt";
			scriptableObjectTemplateFile = "ES3ScriptableObjectTypeTemplate.txt";

			// Init Type List
			var tempTypes = new List<TypeListItem> ();

			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.FullName.Contains("Editor") && assembly.FullName != "ES3" && !assembly.FullName.Contains("ES3")).OrderBy(assembly => assembly.GetName().Name).ToArray();

			foreach (var assembly in assemblies)
			{
				var assemblyTypes = assembly.GetTypes();

				for(int i = 0; i < assemblyTypes.Length; i++)
				{
					var type = assemblyTypes [i];
					if(type.IsGenericType || type.IsEnum || type.IsNotPublic || type.IsAbstract || type.IsInterface)
						continue;

					var typeName = type.Name;
					if(typeName [0] == '$' || typeName [0] == '_' || typeName [0] == '<')
						continue;

					var typeNamespace = type.Namespace;
					var namespaceName = typeNamespace == null ? "" : typeNamespace.ToString();

					tempTypes.Add(new TypeListItem (type.Name, namespaceName, type, true, HasExplicitES3Type(type)));
				}

			}
			types = tempTypes.OrderBy(type => type.name).ToArray();

			// Load types and recent types.
			if(Event.current.type == EventType.Layout)
			{
				recentTypes = new List<int>();
				for(int i=0; i<recentTypeCount; i++)
				{
					int typeIndex = LoadTypeIndex("TypesWindowRecentType"+i);
					if(typeIndex != -1)
						recentTypes.Add(typeIndex);
				}
				SelectType(LoadTypeIndex("TypesWindowSelectedType"));
			} 

			PerformSearch(searchFieldValue);

			// Init Assets.
			string es3FolderPath = ES3Settings.PathToEasySaveFolder();
			checkmark = AssetDatabase.LoadAssetAtPath<Texture2D>(es3FolderPath + "Editor/checkmark.png");
			//checkmarkSmall = AssetDatabase.LoadAssetAtPath<Texture2D>(es3FolderPath + "Editor/checkmarkSmall.png");

			// Init Styles.
			searchBarCancelButtonStyle = new GUIStyle(EditorStyles.miniButton);
			var cancelButtonSize = EditorStyles.miniTextField.CalcHeight(new GUIContent(""), 20);
			searchBarCancelButtonStyle.fixedWidth = cancelButtonSize;
			searchBarCancelButtonStyle.fixedHeight = cancelButtonSize;
			searchBarCancelButtonStyle.fontSize = 8;
			searchBarCancelButtonStyle.padding = new RectOffset();
			searchBarStyle = new GUIStyle(EditorStyles.toolbarTextField);
			searchBarStyle.stretchWidth = true;

			typeButtonStyle = new GUIStyle(EditorStyles.largeLabel);
			typeButtonStyle.alignment = TextAnchor.MiddleLeft;
			typeButtonStyle.stretchWidth = false;
			selectedTypeButtonStyle = new GUIStyle(typeButtonStyle);
			selectedTypeButtonStyle.fontStyle = FontStyle.Bold;

			leftPaneStyle = new GUIStyle();
			leftPaneStyle.fixedWidth = leftPaneWidth;
			leftPaneStyle.clipping = TextClipping.Clip;
			leftPaneStyle.padding = new RectOffset(10, 10, 10, 10);

			selectAllNoneButtonStyle = new GUIStyle(EditorStyles.miniButton);
			selectAllNoneButtonStyle.stretchWidth = false;
			selectAllNoneButtonStyle.margin = new RectOffset(0,0,0,10);
		}

		private void Generate()
		{
			var type = types[selectedType].type;
			if(type == null)
			{
				EditorUtility.DisplayDialog("Type not selected", "Type not selected. Please ensure you select a type", "Ok");
				return;
			}

			unsavedChanges = false;

			// Get the serializable fields of this class.
			//var fields = ES3Reflection.GetSerializableES3Fields(type);

			// The string that we suffix to the class name. i.e. UnityEngine_UnityEngine_Transform.
			string es3TypeSuffix = type.Name;
			// The string for the full C#-safe type name. This name must be suitable for going inside typeof().
			string fullType = GetFullTypeName(type);
			// The list of WriteProperty calls to write the properties of this type.
			string writes = GenerateWrites();
			// The list of case statements and Read calls to read the properties of this type.
			string reads = GenerateReads();
			// A comma-seperated string of fields we've supported in this type.
			string propertyNames = "";

			bool first = true;
			for(int i=0; i<fields.Length; i++)
			{
				if(!fieldSelected[i]) 
					continue;

				if(first)
					first = false;
				else
					propertyNames += ", ";
				propertyNames += "\"" + fields[i].Name + "\"";
			}

			var easySaveEditorPath = ES3Settings.PathToEasySaveFolder()+"Editor/";

			// Insert the relevant strings into the template.
			string template;
			if(typeof(Component).IsAssignableFrom(type))
				template = File.ReadAllText(easySaveEditorPath + componentTemplateFile);
			else if(ES3Reflection.IsValueType(type))
				template = File.ReadAllText(easySaveEditorPath + valueTemplateFile);
			else if(typeof(ScriptableObject).IsAssignableFrom(type))
				template = File.ReadAllText(easySaveEditorPath + scriptableObjectTemplateFile);
			else
				template = File.ReadAllText(easySaveEditorPath + classTemplateFile);
			template = template.Replace("[es3TypeSuffix]", es3TypeSuffix);
			template = template.Replace("[fullType]", fullType);
			template = template.Replace("[writes]", writes);
			template = template.Replace("[reads]", reads);
			template = template.Replace("[propertyNames]", propertyNames);

			// Create the output file.


			string outputFilePath = GetOutputPath(type);
			var fileInfo = new FileInfo(outputFilePath);
			fileInfo.Directory.Create();
			File.WriteAllText(outputFilePath, template);
			AssetDatabase.Refresh();
		}

		private string GenerateWrites()
		{
			var type = types[selectedType].type;
			bool isComponent = typeof(Component).IsAssignableFrom(type);
			string writes = "";

			for(int i=0; i<fields.Length; i++)
			{
				var field = fields[i];
				var selected = fieldSelected[i];
				var es3Type = ES3TypeMgr.GetES3Type(field.MemberType);

				if(!selected || isComponent && (field.Name == ES3Reflection.componentTagFieldName || field.Name == ES3Reflection.componentNameFieldName))
					continue;

				string writeByRef = ES3Reflection.IsAssignableFrom(typeof(UnityEngine.Object), field.MemberType) ? "ByRef" : "";
                string es3TypeParam = HasExplicitES3Type(es3Type) && writeByRef == "" ? ", " + es3Type.GetType().Name + ".Instance" : (writeByRef == "" ? ", ES3Internal.ES3TypeMgr.GetES3Type(typeof(" + GetFullTypeName(field.MemberType) + "))" : "");
                // If this is static, access the field through the class name rather than through an instance.
                string instance = (field.IsStatic) ? GetFullTypeName(type) : "instance";

				if(!field.IsPublic)
				{
					string memberType = field.isProperty ? "Property" : "Field";
					writes += String.Format("\r\n\t\t\twriter.WritePrivate{2}{1}(\"{0}\", instance);", field.Name, writeByRef, memberType);
				}
				else
					writes += String.Format("\r\n\t\t\twriter.WriteProperty{1}(\"{0}\", {3}.{0}{2});", field.Name, writeByRef, es3TypeParam, instance);
			}
			return writes;
		}

		private string GenerateReads()
		{
			var type = types[selectedType].type;
			bool isComponent = typeof(Component).IsAssignableFrom(type);
			string reads = "";

			for(int i=0; i<fields.Length; i++)
			{
				var field = fields[i];
				var selected = fieldSelected[i];

				if(!selected || isComponent && (field.Name == "tag" || field.Name == "name"))
					continue;

				string fieldTypeName = GetFullTypeName(field.MemberType);
				string es3TypeParam = HasExplicitES3Type(field.MemberType) ? ES3TypeMgr.GetES3Type(field.MemberType).GetType().Name+".Instance" : "";
				// If this is static, access the field through the class name rather than through an instance.
				string instance = (field.IsStatic) ? GetFullTypeName(type) : "instance";

				// If we're writing a private field or property, we need to write it using a different method.
				if(!field.IsPublic)
				{
					es3TypeParam = ", " + es3TypeParam;
					if(field.isProperty)
						reads += String.Format("\r\n\t\t\t\t\tcase \"{0}\":\r\n\t\t\t\t\treader.SetPrivateProperty(\"{0}\", reader.Read<{1}>(), instance);\r\n\t\t\t\t\tbreak;", field.Name, fieldTypeName);
					else
						reads += String.Format("\r\n\t\t\t\t\tcase \"{0}\":\r\n\t\t\t\t\treader.SetPrivateField(\"{0}\", reader.Read<{1}>(), instance);\r\n\t\t\t\t\tbreak;", field.Name, fieldTypeName);
				}
				else
					reads += String.Format("\r\n\t\t\t\t\tcase \"{0}\":\r\n\t\t\t\t\t\t{3}.{0} = reader.Read<{1}>({2});\r\n\t\t\t\t\t\tbreak;", field.Name, fieldTypeName, es3TypeParam, instance);
			}
			return reads;
		}

		private string GetOutputPath(Type type)
		{
			return Application.dataPath + "/Easy Save 3/Types/ES3UserType_"+type.Name+".cs";
		}

		/* Gets the full Type name, replacing any syntax (such as '+') with a dot to make it a valid type name */
		private static string GetFullTypeName(Type type)
		{
			string typeName = type.ToString();

			if(type.IsNested)
				typeName = typeName.Replace('+','.');

			// If it's a generic type, replace syntax with angled brackets.
			int genericArgumentCount = type.GetGenericArguments().Length;
			if(genericArgumentCount > 0)
			{
				return string.Format("{0}<{1}>", type.ToString().Split('`')[0], string.Join(", ", type.GetGenericArguments().Select(x => GetFullTypeName(x)).ToArray()));
			}

			return typeName;
		}

		/* Whether this type has an explicit ES3Type. For example, ES3ArrayType would return false, but ES3Vector3ArrayType would return true */
		private static bool HasExplicitES3Type(Type type)
		{
			var es3Type = ES3TypeMgr.GetES3Type(type);
			if(es3Type == null)
				return false;
			// If this ES3Type has a static Instance property, return true.
			if(es3Type.GetType().GetField("Instance", BindingFlags.Public | BindingFlags.Static) != null)
				return true;
			return false;
		}

		private static bool HasExplicitES3Type(ES3Type es3Type)
		{
			if(es3Type == null)
				return false;
			// If this ES3Type has a static Instance property, return true.
			if(es3Type.GetType().GetField("Instance", BindingFlags.Public | BindingFlags.Static) != null)
				return true;
			return false;
		}

		public class TypeListItem
		{
			public string name;
            public string lowercaseName;
			public string namespaceName;
			public Type type;
			public bool showInList;
			public bool hasExplicitES3Type;

			public TypeListItem(string name, string namespaceName, Type type, bool showInList, bool hasExplicitES3Type)
			{
				this.name = name;
                this.lowercaseName = name.ToLowerInvariant();
				this.namespaceName = namespaceName;
				this.type = type;
				this.showInList = showInList;
				this.hasExplicitES3Type = hasExplicitES3Type;
			}
		}
	}
}
