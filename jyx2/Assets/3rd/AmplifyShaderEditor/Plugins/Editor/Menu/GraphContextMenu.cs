// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace AmplifyShaderEditor
{
	public class ShortcutKeyData
	{
		public bool IsPressed;
		public System.Type NodeType;
		public string Name;
		public ShortcutKeyData( System.Type type, string name )
		{
			NodeType = type;
			Name = name;
			IsPressed = false;
		}
	}

	public class GraphContextMenu
	{
		private List<ContextMenuItem> m_items;
		private List<ContextMenuItem> m_itemFunctions;
		private Dictionary<System.Type, NodeAttributes> m_itemsDict;
		private Dictionary<System.Type, NodeAttributes> m_deprecatedItemsDict;
		private Dictionary<System.Type, System.Type> m_castTypes;
		private Dictionary<KeyCode, ShortcutKeyData> m_shortcutTypes;

		private KeyCode m_lastKeyPressed;
		private ParentGraph m_currentGraph;
		private bool m_correctlyLoaded = false;

		public GraphContextMenu( ParentGraph currentGraph )
		{
			m_currentGraph = currentGraph;
			m_correctlyLoaded = RefreshNodes( currentGraph );
		}

		private Type[] GetTypesInNamespace( Assembly assembly, string nameSpace )
		{
			return assembly.GetTypes().Where( t => String.Equals( t.Namespace, nameSpace, StringComparison.Ordinal ) ).ToArray();
		}

		public bool RefreshNodes( ParentGraph currentGraph )
		{
			if( m_items != null )
			{
				m_items.Clear();
				m_items = null;
			}

			if( m_itemFunctions != null )
			{
				m_itemFunctions.Clear();
				m_itemFunctions = null;
			}

			m_items = new List<ContextMenuItem>();
			m_itemFunctions = new List<ContextMenuItem>();

			if( m_itemsDict != null )
				m_itemsDict.Clear();

			m_itemsDict = new Dictionary<System.Type, NodeAttributes>();

			if( m_deprecatedItemsDict != null )
				m_deprecatedItemsDict.Clear();

			m_deprecatedItemsDict = new Dictionary<System.Type, NodeAttributes>();

			if( m_castTypes != null )
				m_castTypes.Clear();

			m_castTypes = new Dictionary<System.Type, System.Type>();

			if( m_shortcutTypes != null )
				m_shortcutTypes.Clear();

			m_shortcutTypes = new Dictionary<KeyCode, ShortcutKeyData>();

			m_lastKeyPressed = KeyCode.None;

			// Fetch all available nodes by their attributes
			try
			{
				//IEnumerable<System.Type> availableTypes = AppDomain.CurrentDomain.GetAssemblies().ToList().SelectMany( type => type.GetTypes() );
				Type[] availableTypes = GetTypesInNamespace( Assembly.GetExecutingAssembly(), "AmplifyShaderEditor" );
				foreach( System.Type type in availableTypes )
				{
					foreach( NodeAttributes attribute in Attribute.GetCustomAttributes( type ).OfType<NodeAttributes>() )
					{
						if( attribute.Available && !attribute.Deprecated )
						{
							//if ( !UIUtils.CurrentWindow.IsShaderFunctionWindow && attribute.AvailableInFunctionsOnly )
							//	continue;

							if( !UIUtils.HasColorCategory( attribute.Category ) )
							{
								if( !String.IsNullOrEmpty( attribute.CustomCategoryColor ) )
								{
									try
									{
										Color color = new Color();
										ColorUtility.TryParseHtmlString( attribute.CustomCategoryColor, out color );
										UIUtils.AddColorCategory( attribute.Category, color );
									}
									catch( Exception e )
									{
										Debug.LogException( e );
										UIUtils.AddColorCategory( attribute.Category, Constants.DefaultCategoryColor );
									}
								}
								//else
								//{
								//	UIUtils.AddColorCategory( attribute.Category, Constants.DefaultCategoryColor );
								//}
							}

							if( attribute.CastType != null && attribute.CastType.Length > 0 && type != null )
							{
								for( int i = 0; i < attribute.CastType.Length; i++ )
								{
									m_castTypes.Add( attribute.CastType[ i ], type );
								}
							}

							if( attribute.ShortcutKey != KeyCode.None && type != null )
								m_shortcutTypes.Add( attribute.ShortcutKey, new ShortcutKeyData( type, attribute.Name ) );

							ContextMenuItem newItem = new ContextMenuItem( attribute, type, attribute.Name, attribute.Category, attribute.Description, null, attribute.ShortcutKey );
							if( UIUtils.GetNodeAvailabilityInBitArray( attribute.NodeAvailabilityFlags, NodeAvailability.SurfaceShader ) )
								m_items.Add( newItem );
							else if( UIUtils.GetNodeAvailabilityInBitArray( attribute.NodeAvailabilityFlags, currentGraph.ParentWindow.CurrentNodeAvailability ) )
								m_items.Add( newItem );
							else if( UIUtils.GetNodeAvailabilityInBitArray( attribute.NodeAvailabilityFlags, currentGraph.CurrentCanvasMode ) )
								m_items.Add( newItem );

							m_itemsDict.Add( type, attribute );
							m_itemFunctions.Add( newItem );
						}
						else
						{
							m_deprecatedItemsDict.Add( type, attribute );
						}
					}
				}
			}
			catch( ReflectionTypeLoadException exception )
			{
				Debug.LogException( exception );
				return false;
			}

			string[] guids = AssetDatabase.FindAssets( "t:AmplifyShaderFunction" );
			List<AmplifyShaderFunction> allFunctions = new List<AmplifyShaderFunction>();

			for( int i = 0; i < guids.Length; i++ )
			{
				allFunctions.Add( AssetDatabase.LoadAssetAtPath<AmplifyShaderFunction>( AssetDatabase.GUIDToAssetPath( guids[ i ] ) ) );
			}

			int functionCount = allFunctions.Count;
			if( functionCount > 0 )
			{
				m_castTypes.Add( typeof( AmplifyShaderFunction ), typeof( FunctionNode ) );
			}

			for( int i = 0; i < functionCount; i++ )
			{
				NodeAttributes attribute = new NodeAttributes( allFunctions[ i ].FunctionName, allFunctions[ i ].CustomNodeCategory, allFunctions[ i ].Description, KeyCode.None, true, 0, int.MaxValue, typeof( AmplifyShaderFunction ) );
				System.Type type = typeof( FunctionNode );

				ContextMenuItem newItem = new ContextMenuItem( attribute, type, attribute.Name, attribute.Category, attribute.Description, allFunctions[ i ], attribute.ShortcutKey );
				m_items.Add( newItem );
				m_itemFunctions.Add( newItem );
			}

			//Sort out the final list by name
			m_items.Sort( ( x, y ) => x.Category.CompareTo( y.Category ) );
			m_itemFunctions.Sort( ( x, y ) => x.Category.CompareTo( y.Category ) );
			return true;
		}

		public void Destroy()
		{
			for( int i = 0; i < m_items.Count; i++ )
			{
				m_items[ i ].Destroy();
			}

			for( int i = 0; i < m_itemFunctions.Count; i++ )
			{
				if( m_itemFunctions[ i ] != null )
					m_itemFunctions[ i ].Destroy();
			}

			m_items.Clear();
			m_items = null;

			m_itemFunctions.Clear();
			m_itemFunctions = null;

			m_itemsDict.Clear();
			m_itemsDict = null;

			m_deprecatedItemsDict.Clear();
			m_deprecatedItemsDict = null;

			m_castTypes.Clear();
			m_castTypes = null;

			m_shortcutTypes.Clear();
			m_shortcutTypes = null;

		}

		public NodeAttributes GetNodeAttributesForType( System.Type type )
		{
			if( type == null )
			{
				Debug.LogError( "Invalid type detected" );
				return null;
			}

			if( m_itemsDict.ContainsKey( type ) )
				return m_itemsDict[ type ];
			return null;
		}

		public NodeAttributes GetDeprecatedNodeAttributesForType( System.Type type )
		{
			if( m_deprecatedItemsDict.ContainsKey( type ) )
				return m_deprecatedItemsDict[ type ];
			return null;
		}

		public void UpdateKeyPress( KeyCode key )
		{
			if( key == KeyCode.None )
				return;

			m_lastKeyPressed = key;
			if( m_shortcutTypes.ContainsKey( key ) )
			{
				m_shortcutTypes[ key ].IsPressed = true;
			}
		}

		public void UpdateKeyReleased( KeyCode key )
		{
			if( key == KeyCode.None )
				return;

			if( m_shortcutTypes.ContainsKey( key ) )
			{
				m_shortcutTypes[ key ].IsPressed = false;
			}
		}

		public ParentNode CreateNodeFromCastType( System.Type type )
		{
			if( m_castTypes.ContainsKey( type ) )
			{
				ParentNode newNode = (ParentNode)ScriptableObject.CreateInstance( m_castTypes[ type ] );
				return newNode;
			}
			return null;
		}


		public ParentNode CreateNodeFromShortcutKey()
		{
			if( m_lastKeyPressed == KeyCode.None )
				return null;

			if( m_shortcutTypes.ContainsKey( m_lastKeyPressed ) && m_shortcutTypes[ m_lastKeyPressed ].IsPressed )
			{
				ParentNode newNode = (ParentNode)ScriptableObject.CreateInstance( m_shortcutTypes[ m_lastKeyPressed ].NodeType );
				return newNode;
			}
			return null;
		}

		public bool CheckShortcutKey()
		{
			if( m_lastKeyPressed == KeyCode.None )
				return false;

			if( m_shortcutTypes.ContainsKey( m_lastKeyPressed ) && m_shortcutTypes[ m_lastKeyPressed ].IsPressed )
			{
				return true;
			}
			return false;
		}

		public List<ContextMenuItem> MenuItems
		{
			get
			{
				if( m_currentGraph.ParentWindow.IsShaderFunctionWindow )
					return m_itemFunctions;
				else
					return m_items;
			}
		}

		public List<ContextMenuItem> ItemFunctions { get { return m_itemFunctions; } }
		public KeyCode LastKeyPressed
		{
			get { return m_lastKeyPressed; }
		}

		public Dictionary<KeyCode, ShortcutKeyData> NodeShortcuts { get { return m_shortcutTypes; } }
		public bool CorrectlyLoaded { get { return m_correctlyLoaded; } }
	}
}
