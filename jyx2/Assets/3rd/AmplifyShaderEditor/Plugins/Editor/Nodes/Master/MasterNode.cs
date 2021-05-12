// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AmplifyShaderEditor
{
	public enum PrecisionType
	{
		Float = 0,
		Half,
		Fixed
	}

	public enum AvailableShaderTypes
	{
		SurfaceShader = 0,
		Template
	}

	[Serializable]
	public class MasterNodeCategoriesData
	{
		public AvailableShaderTypes Category;
		public string Name;
		public MasterNodeCategoriesData( AvailableShaderTypes category, string name ) { Category = category; Name = name; }
	}

	[Serializable]
	public class MasterNode : OutputNode
	{
		protected const string CustomInspectorStr = "Custom Editor";
		protected const string CustomInspectorFormat = "CustomEditor \"{0}\"";

		private const string PropertyOrderFoldoutStr = " Material Properties";
		private const string PropertyOrderTemplateFoldoutStr = "Material Properties";

		protected MasterNodeDataCollector m_currentDataCollector;

		protected const string ShaderNameStr = "Shader Name";
		protected GUIContent m_shaderNameContent;

		private const string IndentationHelper = "\t\t{0}\n";
		private const string ShaderLODFormat = "\t\tLOD {0}\n";

		public delegate void OnMaterialUpdated( MasterNode masterNode );
		public event OnMaterialUpdated OnMaterialUpdatedEvent;
		public event OnMaterialUpdated OnShaderUpdatedEvent;

		protected const string GeneralFoldoutStr = " General";

		protected readonly string[] ShaderModelTypeArr = { "2.0", "2.5", "3.0", "3.5", "4.0", "4.5", "4.6", "5.0" };
		private const string ShaderKeywordsStr = "Shader Keywords";

		[SerializeField]
		protected int m_shaderLOD = 0;

		[SerializeField]
		protected int m_shaderModelIdx = 2;

		[SerializeField]
		protected Shader m_currentShader;

		[SerializeField]
		protected Material m_currentMaterial;

		//[SerializeField]
		//private bool m_isMainMasterNode = false;

		[SerializeField]
		private Rect m_masterNodeIconCoords;

		[SerializeField]
		protected string m_shaderName = Constants.DefaultShaderName;

		[SerializeField]
		protected string m_croppedShaderName = Constants.DefaultShaderName;

		[SerializeField]
		protected string m_customInspectorName = Constants.DefaultCustomInspector;

		[SerializeField]
		protected int m_masterNodeCategory = 0;// MasterNodeCategories.SurfaceShader;

		[SerializeField]
		protected string m_currentShaderData = string.Empty;

		private Texture2D m_masterNodeOnTex;
		private Texture2D m_masterNodeOffTex;

		private Texture2D m_gpuInstanceOnTex;
		private Texture2D m_gpuInstanceOffTex;

		// Shader Keywords
		[SerializeField]
		private List<string> m_shaderKeywords = new List<string>();

		[SerializeField]
		private bool m_shaderKeywordsFoldout = true;

		private GUIStyle m_addShaderKeywordStyle;
		private GUIStyle m_removeShaderKeywordStyle;
		private GUIStyle m_smallAddShaderKeywordItemStyle;
		private GUIStyle m_smallRemoveShaderKeywordStyle;
		private const float ShaderKeywordButtonLayoutWidth = 15;


		public MasterNode() : base() { CommonInit(); }
		public MasterNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { CommonInit(); }

		protected GUIContent m_categoryLabel = new GUIContent( "Shader Type ", "Specify the shader type you want to be working on" );

		protected GUIContent[] m_availableCategoryLabels;
		protected MasterNodeCategoriesData[] m_availableCategories;

		[SerializeField]
		private List<PropertyNode> m_propertyNodesVisibleList = new List<PropertyNode>();

		private ReorderableList m_propertyReordableList;
		protected bool m_propertyOrderChanged = false;
		//private int m_availableCount = 0;
		private int m_lastCount = 0;

		private GUIStyle m_propertyAdjustment;
		protected bool m_shaderNameIsTitle = true;

		void CommonInit()
		{
			m_currentMaterial = null;
			m_masterNodeIconCoords = new Rect( 0, 0, 64, 64 );
			m_isMainOutputNode = false;
			m_connStatus = NodeConnectionStatus.Connected;
			m_activeType = GetType();
			m_currentPrecisionType = PrecisionType.Float;
			m_textLabelWidth = 120;
			m_shaderNameContent = new GUIContent( ShaderNameStr, string.Empty );

			AddMasterPorts();
		}

		void InitAvailableCategories()
		{
			int templateCount =  m_containerGraph.ParentWindow.TemplatesManagerInstance.TemplateCount;
			m_availableCategories = new MasterNodeCategoriesData[ templateCount + 1 ];
			m_availableCategoryLabels = new GUIContent[ templateCount + 1 ];

			m_availableCategories[ 0 ] = new MasterNodeCategoriesData( AvailableShaderTypes.SurfaceShader, string.Empty );
			m_availableCategoryLabels[ 0 ] = new GUIContent( "Surface" );

			for( int i = 0; i < templateCount; i++ )
			{
				int idx = i + 1;
				TemplateDataParent templateData = m_containerGraph.ParentWindow.TemplatesManagerInstance.GetTemplate( i );
				m_availableCategories[ idx ] = new MasterNodeCategoriesData( AvailableShaderTypes.Template, templateData.GUID );
				m_availableCategoryLabels[ idx ] = new GUIContent( templateData.Name );
			}
		}

		public override void SetupNodeCategories()
		{
			//base.SetupNodeCategories();
			ContainerGraph.ResetNodesData();
			int count = m_inputPorts.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_inputPorts[ i ].IsConnected )
				{
					NodeData nodeData = new NodeData( m_inputPorts[ i ].Category );
					ParentNode node = m_inputPorts[ i ].GetOutputNode();
					node.PropagateNodeData( nodeData, ref m_currentDataCollector );
				}
				else if( m_inputPorts[ i ].HasExternalLink )
				{
					InputPort linkedPort = m_inputPorts[ i ].ExternalLink;
					if( linkedPort != null && linkedPort.IsConnected )
					{
						NodeData nodeData = new NodeData( linkedPort.Category );
						ParentNode node = linkedPort.GetOutputNode();
						node.PropagateNodeData( nodeData, ref m_currentDataCollector );
					}
				}
			}
		}

		public virtual void RefreshAvailableCategories()
		{
			InitAvailableCategories();
		}

		public virtual void AddMasterPorts() { }

		public virtual void ForcePortType() { }

		public virtual void UpdateMasterNodeMaterial( Material material ) { }

		public virtual void SetName( string name ) { }

		public void CopyFrom( MasterNode other )
		{
			Vec2Position = other.Vec2Position;
			CurrentShader = other.CurrentShader;
			CurrentMaterial = other.CurrentMaterial;
			ShaderName = other.ShaderName;
			m_masterNodeCategory = other.CurrentMasterNodeCategoryIdx;
		}

		protected void DrawCurrentShaderType()
		{
			if( m_availableCategories == null )
				InitAvailableCategories();

			int oldType = m_masterNodeCategory;
			m_masterNodeCategory = EditorGUILayoutPopup( m_categoryLabel, m_masterNodeCategory, m_availableCategoryLabels );
			if( oldType != m_masterNodeCategory )
			{
				m_containerGraph.ParentWindow.ReplaceMasterNode( m_availableCategories[ m_masterNodeCategory ] , false );
			}
		}

		protected void DrawCustomInspector( )
		{
			EditorGUILayout.BeginHorizontal();
			m_customInspectorName = EditorGUILayoutTextField( CustomInspectorStr, m_customInspectorName );
			if( GUILayoutButton( string.Empty, UIUtils.GetCustomStyle( CustomStyle.ResetToDefaultInspectorButton ), GUILayout.Width( 15 ), GUILayout.Height( 15 ) ) )
			{
				GUIUtility.keyboardControl = 0;
				m_customInspectorName = Constants.DefaultCustomInspector;
			}
			EditorGUILayout.EndHorizontal();
		}

		protected void DrawShaderName()
		{
			EditorGUI.BeginChangeCheck();
			string newShaderName = EditorGUILayoutTextField( m_shaderNameContent, m_shaderName );
			if( EditorGUI.EndChangeCheck() )
			{
				if( newShaderName.Length > 0 )
				{
					newShaderName = UIUtils.RemoveShaderInvalidCharacters( newShaderName );
				}
				else
				{
					newShaderName = Constants.DefaultShaderName;
				}
				ShaderName = newShaderName;
				ContainerGraph.ParentWindow.UpdateTabTitle( ShaderName, true );
			}
			m_shaderNameContent.tooltip = m_shaderName;
		}

		public void DrawShaderKeywords()
		{
			if( m_addShaderKeywordStyle == null )
				m_addShaderKeywordStyle = UIUtils.PlusStyle;

			if( m_removeShaderKeywordStyle == null )
				m_removeShaderKeywordStyle = UIUtils.MinusStyle;

			if( m_smallAddShaderKeywordItemStyle == null )
				m_smallAddShaderKeywordItemStyle = UIUtils.PlusStyle;

			if( m_smallRemoveShaderKeywordStyle == null )
				m_smallRemoveShaderKeywordStyle = UIUtils.MinusStyle;

			EditorGUILayout.BeginHorizontal();
			{
				m_shaderKeywordsFoldout = EditorGUILayout.Foldout( m_shaderKeywordsFoldout, ShaderKeywordsStr );

				// Add keyword
				if( GUILayout.Button( string.Empty, m_addShaderKeywordStyle ) )
				{
					m_shaderKeywords.Insert( 0, "" );
				}

				//Remove keyword
				if( GUILayout.Button( string.Empty, m_removeShaderKeywordStyle ) )
				{
					m_shaderKeywords.RemoveAt( m_shaderKeywords.Count - 1 );
				}
			}
			EditorGUILayout.EndHorizontal();

			if( m_shaderKeywordsFoldout )
			{
				EditorGUI.indentLevel += 1;
				int itemCount = m_shaderKeywords.Count;
				int markedToDelete = -1;
				for( int i = 0; i < itemCount; i++ )
				{
					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Label( " " );
						// Add new port
						if( GUILayoutButton( string.Empty, m_smallAddShaderKeywordItemStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
						{
							m_shaderKeywords.Insert( i, "" );
						}

						//Remove port
						if( GUILayoutButton( string.Empty, m_smallRemoveShaderKeywordStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
						{
							markedToDelete = i;
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				if( markedToDelete > -1 )
				{
					m_shaderKeywords.RemoveAt( markedToDelete );
				}
				EditorGUI.indentLevel -= 1;
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			if( m_availableCategories == null )
				InitAvailableCategories();

			base.Draw( drawInfo );
		}

		public override void OnNodeRepaint( DrawInfo drawInfo )
		{
			base.OnNodeRepaint( drawInfo );

			if( m_isMainOutputNode )
			{
				if( m_masterNodeOnTex == null )
				{
					m_masterNodeOnTex = UIUtils.MasterNodeOnTexture;
				}

				if( m_masterNodeOffTex == null )
				{
					m_masterNodeOffTex = UIUtils.MasterNodeOffTexture;
				}

				if( m_gpuInstanceOnTex == null )
				{
					m_gpuInstanceOnTex = UIUtils.GPUInstancedOnTexture;
				}

				if( m_gpuInstanceOffTex == null )
				{
					m_gpuInstanceOffTex = UIUtils.GPUInstancedOffTexture;
				}

				m_masterNodeIconCoords = m_globalPosition;
				m_masterNodeIconCoords.x += m_globalPosition.width - m_masterNodeOffTex.width * drawInfo.InvertedZoom;
				m_masterNodeIconCoords.y += m_globalPosition.height - m_masterNodeOffTex.height * drawInfo.InvertedZoom;
				m_masterNodeIconCoords.width = m_masterNodeOffTex.width * drawInfo.InvertedZoom;
				m_masterNodeIconCoords.height = m_masterNodeOffTex.height * drawInfo.InvertedZoom;

				GUI.DrawTexture( m_masterNodeIconCoords, m_masterNodeOffTex );

				if( m_gpuInstanceOnTex == null )
				{
					m_gpuInstanceOnTex = UIUtils.GPUInstancedOnTexture;
				}
			}
		}

		protected void DrawInstancedIcon( DrawInfo drawInfo )
		{
			if( m_gpuInstanceOffTex == null || drawInfo.CurrentEventType != EventType.Repaint )
				return;

			m_masterNodeIconCoords = m_globalPosition;
			m_masterNodeIconCoords.x += m_globalPosition.width - 5 - m_gpuInstanceOffTex.width * drawInfo.InvertedZoom;
			m_masterNodeIconCoords.y += m_headerPosition.height;
			m_masterNodeIconCoords.width = m_gpuInstanceOffTex.width * drawInfo.InvertedZoom;
			m_masterNodeIconCoords.height = m_gpuInstanceOffTex.height * drawInfo.InvertedZoom;
			GUI.DrawTexture( m_masterNodeIconCoords, m_gpuInstanceOffTex );
		}
		//public override void DrawProperties()
		//{
		//	base.DrawProperties();
		//	//EditorGUILayout.LabelField( _shaderTypeLabel );
		//}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			//IOUtils.AddFieldValueToString( ref nodeInfo, m_isMainMasterNode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_shaderModelIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentPrecisionType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_customInspectorName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_shaderLOD );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_masterNodeCategory );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 21 )
			{
				m_shaderModelIdx = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				m_currentPrecisionType = (PrecisionType)Enum.Parse( typeof( PrecisionType ), GetCurrentParam( ref nodeParams ) );
				if( m_currentPrecisionType == PrecisionType.Fixed )
				{
					m_currentPrecisionType = PrecisionType.Half;
				}
			}

			if( UIUtils.CurrentShaderVersion() > 2404 )
			{
				m_customInspectorName = GetCurrentParam( ref nodeParams );
			}

			if( UIUtils.CurrentShaderVersion() > 6101 )
			{
				m_shaderLOD = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}

			if( UIUtils.CurrentShaderVersion() >= 13001 )
			{
				//Debug.LogWarning( "Add correct version as soon as it is merged into master" );
				m_masterNodeCategory = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			if( activateNode )
			{
				InputPort port = GetInputPortByUniqueId( portId );
				port.GetOutputNode().ActivateNode( UniqueId, portId, m_activeType );
			}
		}

		public void FireMaterialChangedEvt()
		{
			if( OnMaterialUpdatedEvent != null )
			{
				OnMaterialUpdatedEvent( this );
			}
		}

		public void FireShaderChangedEvt()
		{
			if( OnShaderUpdatedEvent != null )
				OnShaderUpdatedEvent( this );
		}

		public void RegisterStandaloneFuntions()
		{
			List<CustomExpressionNode> nodes = m_containerGraph.CustomExpressionOnFunctionMode.NodesList;
			int count = nodes.Count;
			Dictionary<int, CustomExpressionNode> examinedNodes = new Dictionary<int, CustomExpressionNode>();
			for( int i = 0; i < count; i++ )
			{
				if( nodes[ i ].AutoRegisterMode )
				{
					nodes[ i ].CheckDependencies( ref m_currentDataCollector, ref examinedNodes);
				}
			}
			examinedNodes.Clear();
			examinedNodes = null;
		} 

		// What operation this node does
		public virtual void Execute( Shader selectedShader )
		{
			Execute( AssetDatabase.GetAssetPath( selectedShader ), false );
		}

		public virtual Shader Execute( string pathname, bool isFullPath )
		{
			ContainerGraph.ResetNodesLocalVariables();
			m_currentDataCollector = new MasterNodeDataCollector( this );
			return null;
		}

		protected void SortInputPorts( ref List<InputPort> vertexPorts, ref List<InputPort> fragmentPorts )
		{
			for( int i = 0; i < m_inputPorts.Count; i++ )
			{
				if( m_inputPorts[ i ].Category == MasterNodePortCategory.Fragment || m_inputPorts[ i ].Category == MasterNodePortCategory.Debug )
				{
					if( fragmentPorts != null )
						fragmentPorts.Add( m_inputPorts[ i ] );
				}
				else
				{
					if( vertexPorts != null )
						vertexPorts.Add( m_inputPorts[ i ] );
				}
			}

			if( fragmentPorts.Count > 0 )
			{
				fragmentPorts.Sort( ( x, y ) => x.OrderId.CompareTo( y.OrderId ) );
			}

			if( vertexPorts.Count > 0 )
			{
				vertexPorts.Sort( ( x, y ) => x.OrderId.CompareTo( y.OrderId ) );
			}
		}

		protected void UpdateShaderAsset( ref string pathname, ref string shaderBody, bool isFullPath )
		{
			// Generate Graph info
			shaderBody += ContainerGraph.ParentWindow.GenerateGraphInfo();

			//TODO: Remove current SaveDebugShader and uncomment SaveToDisk as soon as pathname is editable
			if( !String.IsNullOrEmpty( pathname ) )
			{
				IOUtils.StartSaveThread( shaderBody, ( isFullPath ? pathname : ( IOUtils.dataPath + pathname ) ) );
			}
			else
			{
				IOUtils.StartSaveThread( shaderBody, Application.dataPath + "/AmplifyShaderEditor/Samples/Shaders/" + m_shaderName + ".shader" );
			}


			if( CurrentShader == null )
			{
				AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
				CurrentShader = Shader.Find( ShaderName );
			}
			//else
			//{
			//	// need to always get asset datapath because a user can change and asset location from the project window 
			//	AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( m_currentShader ) );
			//	//ShaderUtil.UpdateShaderAsset( m_currentShader, ShaderBody );
			//	//ShaderImporter importer = (ShaderImporter)ShaderImporter.GetAtPath( AssetDatabase.GetAssetPath( CurrentShader ) );
			//	//importer.SaveAndReimport();
			//}

			if( m_currentShader != null )
			{
				m_currentDataCollector.UpdateShaderImporter( ref m_currentShader );
				if( m_currentMaterial != null )
				{
					if( m_currentMaterial.shader != m_currentShader )
						m_currentMaterial.shader = m_currentShader;

					m_currentDataCollector.UpdateMaterialOnPropertyNodes( m_currentMaterial );
					FireMaterialChangedEvt();
					// need to always get asset datapath because a user can change and asset location from the project window
					//AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( m_currentMaterial ) );
				}

			}

			m_currentDataCollector.Destroy();
			m_currentDataCollector = null;
		}


		public void InvalidateMaterialPropertyCount()
		{
			m_lastCount = -1;
		}

		private void RefreshVisibleList( ref List<PropertyNode> allNodes )
		{
			// temp reference for lambda expression
			List<PropertyNode> nodes = allNodes;
			m_propertyNodesVisibleList.Clear();

			for( int i = 0; i < nodes.Count; i++ )
			{
				ReordenatorNode rnode = nodes[ i ] as ReordenatorNode;
				if( ( rnode == null || !rnode.IsInside ) && ( !m_propertyNodesVisibleList.Exists( x => x.PropertyName.Equals( nodes[ i ].PropertyName ) ) ) )
					m_propertyNodesVisibleList.Add( nodes[ i ] );
			}

			m_propertyNodesVisibleList.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );
		}

		public void DrawMaterialInputs( GUIStyle toolbarstyle , bool style = true)
		{
			m_propertyOrderChanged = false;
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( toolbarstyle );
			GUI.color = cachedColor;

			EditorGUI.BeginChangeCheck();
			if( style )
			{
				ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedProperties = GUILayoutToggle( ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedProperties, PropertyOrderFoldoutStr, UIUtils.MenuItemToggleStyle );
			}
			else
			{
				ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedProperties = GUILayoutToggle( ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedProperties, PropertyOrderTemplateFoldoutStr, UIUtils.MenuItemToggleStyle );
			}

			if( EditorGUI.EndChangeCheck() )
			{
				EditorPrefs.SetBool( "ExpandedProperties", ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedProperties );
			}

			EditorGUILayout.EndHorizontal();
			if( !ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedProperties )
				return;

			cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
			EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
			GUI.color = cachedColor;

			List<PropertyNode> nodes = UIUtils.PropertyNodesList();

			if( nodes.Count != m_lastCount )
			{
				RefreshVisibleList( ref nodes );
				m_lastCount = nodes.Count;
			}

			if( m_propertyReordableList == null )
			{
				m_propertyReordableList = new ReorderableList( m_propertyNodesVisibleList, typeof( PropertyNode ), true, false, false, false )
				{
					headerHeight = 0,
					footerHeight = 0,
					showDefaultBackground = false,

					drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
					{
						EditorGUI.LabelField( rect, m_propertyNodesVisibleList[ index ].PropertyInspectorName );
					},

					onReorderCallback = ( list ) =>
					{
						ReorderList( ref nodes );
						m_propertyOrderChanged = true;
						//RecursiveLog();
					}
				};
				ReorderList( ref nodes );
			}

			if( m_propertyReordableList != null )
			{
				if( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.BeginVertical( m_propertyAdjustment );
				m_propertyReordableList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		}

		public void ForceReordering()
		{
			List<PropertyNode> nodes = UIUtils.PropertyNodesList();

			if( nodes.Count != m_lastCount )
			{
				RefreshVisibleList( ref nodes );
				m_lastCount = nodes.Count;
			}

			ReorderList( ref nodes );
			//RecursiveLog();
		}

		private void ReorderList( ref List<PropertyNode> nodes )
		{
			// clear lock list before reordering because of multiple sf being used
			for( int i = 0; i < nodes.Count; i++ )
			{
				ReordenatorNode rnode = nodes[ i ] as ReordenatorNode;
				if( rnode != null )
					rnode.RecursiveClear();
			}

			int propoffset = 0;
			int count = 0;
			for( int i = 0; i < m_propertyNodesVisibleList.Count; i++ )
			{
				ReordenatorNode renode = m_propertyNodesVisibleList[ i ] as ReordenatorNode;
				if( renode != null )
				{
					if( !renode.IsInside )
					{
						m_propertyNodesVisibleList[ i ].OrderIndex = count + propoffset;

						if( renode.PropertyListCount > 0 )
						{
							propoffset += renode.RecursiveCount();
							// the same reordenator can exist multiple times, apply ordering to all of them
							for( int j = 0; j < nodes.Count; j++ )
							{
								ReordenatorNode pnode = ( nodes[ j ] as ReordenatorNode );
								if( pnode != null && pnode.PropertyName.Equals( renode.PropertyName ) )
								{
									pnode.OrderIndex = renode.RawOrderIndex;
									pnode.RecursiveSetOrderOffset( renode.RawOrderIndex, true );
								}
							}
						}
						else
						{
							count++;
						}
					}
					else
					{
						m_propertyNodesVisibleList[ i ].OrderIndex = 0;
					}
				}
				else
				{
					m_propertyNodesVisibleList[ i ].OrderIndex = count + propoffset;
					count++;
				}
			}
		}

		public void CopyPropertyListFrom( MasterNode masterNode )
		{
			m_lastCount = masterNode.ReordableListLastCount;
			m_propertyNodesVisibleList.Clear();
			m_propertyNodesVisibleList.AddRange( masterNode.PropertyNodesVisibleList );
		}

		public virtual void UpdateFromShader( Shader newShader ) { }

		public void ClearUpdateEvents()
		{
			OnShaderUpdatedEvent = null;
			OnMaterialUpdatedEvent = null;
		}

		public Material CurrentMaterial { get { return m_currentMaterial; } set { m_currentMaterial = value; } }
		public Shader CurrentShader
		{
			set
			{
				if( value != null )
				{
					SetName( value.name );
				}

				m_currentShader = value;
				FireShaderChangedEvt();
			}
			get { return m_currentShader; }
		}
		public virtual void OnRefreshLinkedPortsComplete() { }
		public virtual void ReleaseResources() { }
		public override void Destroy()
		{
			base.Destroy();
			OnMaterialUpdatedEvent = null;
			OnShaderUpdatedEvent = null;
			m_masterNodeOnTex = null;
			m_masterNodeOffTex = null;
			m_gpuInstanceOnTex = null;
			m_gpuInstanceOffTex = null;
			m_addShaderKeywordStyle = null;
			m_removeShaderKeywordStyle = null;
			m_smallAddShaderKeywordItemStyle = null;
			m_smallRemoveShaderKeywordStyle = null;
			m_shaderKeywords.Clear();
			m_shaderKeywords = null;
			m_propertyReordableList = null;
			m_propertyAdjustment = null;
			if( m_currentDataCollector != null )
			{
				m_currentDataCollector.Destroy();
				m_currentDataCollector = null;
			}
		}

		public static void OpenShaderBody( ref string result, string name )
		{
			result += string.Format( "Shader \"{0}\"\n", name ) + "{\n";
		}

		public static void CloseShaderBody( ref string result )
		{
			result += "}\n";
		}

		public static void OpenSubShaderBody( ref string result )
		{
			result += "\n\tSubShader\n\t{\n";
		}

		public static void CloseSubShaderBody( ref string result )
		{
			result += "\t}\n";
		}

		public static void AddShaderProperty( ref string result, string name, string value )
		{
			result += string.Format( "\t{0} \"{1}\"\n", name, value );
		}

		public static void AddShaderPragma( ref string result, string value )
		{
			result += string.Format( "\t\t#pragma {0}\n", value );
		}

		public static void AddRenderState( ref string result, string state, string stateParams )
		{
			result += string.Format( "\t\t{0} {1}\n", state, stateParams );
		}

		public static void AddRenderTags( ref string result, string tags )
		{
			result += string.Format( IndentationHelper, tags ); ;
		}

		public static void AddShaderLOD( ref string result, int shaderLOD )
		{
			if( shaderLOD > 0 )
			{
				result += string.Format( ShaderLODFormat, shaderLOD );
			}
		}

		public static void AddMultilineBody( ref string result, string[] lines )
		{
			for( int i = 0; i < lines.Length; i++ )
			{
				result += string.Format( IndentationHelper, lines[ i ] );
			}
		}

		public static void OpenCGInclude( ref string result )
		{
			result += "\t\tCGINCLUDE\n";
		}

		public static void OpenCGProgram( ref string result )
		{
			result += "\t\tCGPROGRAM\n";
		}

		public static void CloseCGProgram( ref string result )
		{
			result += "\n\t\tENDCG\n";
		}

		public string ShaderName
		{
			//get { return ( ( _isHidden ? "Hidden/" : string.Empty ) + ( String.IsNullOrEmpty( _shaderCategory ) ? "" : ( _shaderCategory + "/" ) ) + _shaderName ); }
			get { return m_shaderName; }
			set
			{
				m_shaderName = value;
				string[] shaderNameArr = m_shaderName.Split( '/' );
				m_croppedShaderName = shaderNameArr[ shaderNameArr.Length - 1 ];

				if( m_shaderNameIsTitle )
					m_content.text = GenerateClippedTitle( m_croppedShaderName );

				m_sizeIsDirty = true;
			}
		}
		public string CustomInspectorFormatted { get { return string.Format( CustomInspectorFormat, m_customInspectorName ); } }
		public string CroppedShaderName { get { return m_croppedShaderName; } }
		public AvailableShaderTypes CurrentMasterNodeCategory { get { return ( m_masterNodeCategory == 0 ) ? AvailableShaderTypes.SurfaceShader : AvailableShaderTypes.Template; } }
		public int CurrentMasterNodeCategoryIdx { get { return m_masterNodeCategory; } }
		public MasterNodeDataCollector CurrentDataCollector { get { return m_currentDataCollector; } set { m_currentDataCollector = value; } }
		public List<PropertyNode> PropertyNodesVisibleList { get { return m_propertyNodesVisibleList; } }
		public ReorderableList PropertyReordableList { get { return m_propertyReordableList; } }
		public int ReordableListLastCount { get { return m_lastCount; } }
	}
}
