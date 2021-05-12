// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

namespace AmplifyShaderEditor
{
	public sealed class NodeParametersWindow : MenuParent
	{
		private int m_lastSelectedNode = -1;
		private const string TitleStr = "Node Properties";
		private GUIStyle m_nodePropertiesStyle;
		private GUIContent m_dummyContent = new GUIContent();
		private GUIStyle m_propertyAdjustment;

		private ReorderableList m_functionInputsReordableList = null;
		private int m_functionInputsLastCount = 0;

		private ReorderableList m_functionSwitchesReordableList = null;
		private int m_functionSwitchesLastCount = 0;

		private ReorderableList m_functionOutputsReordableList = null;
		private int m_functionOutputsLastCount = 0;

		private ReorderableList m_propertyReordableList = null;
		private int m_lastCount = 0;

		private bool m_forceUpdate = false;

		[SerializeField]
		private List<PropertyNode> m_propertyReordableNodes = new List<PropertyNode>();

		// width and height are between [0,1] and represent a percentage of the total screen area
		public NodeParametersWindow( AmplifyShaderEditorWindow parentWindow ) : base( parentWindow, 0, 0, 265, 0, string.Empty, MenuAnchor.TOP_LEFT, MenuAutoSize.MATCH_VERTICAL )
		{
			SetMinimizedArea( -225, 0, 275, 0 );
		}

		public void OnShaderFunctionLoad()
		{
			m_functionInputsReordableList = null;
			m_functionSwitchesReordableList = null;
			m_functionOutputsReordableList = null;
		}

		public bool Draw( Rect parentPosition, ParentNode selectedNode, Vector2 mousePosition, int mouseButtonId, bool hasKeyboardFocus )
		{
			bool changeCheck = false;
			base.Draw( parentPosition, mousePosition, mouseButtonId, hasKeyboardFocus );
			if ( m_nodePropertiesStyle == null )
			{
				m_nodePropertiesStyle = UIUtils.GetCustomStyle( CustomStyle.NodePropertiesTitle );
				m_nodePropertiesStyle.normal.textColor = m_nodePropertiesStyle.active.textColor = EditorGUIUtility.isProSkin ? new Color( 1f, 1f, 1f ) : new Color( 0f, 0f, 0f );
			}

			if ( m_isMaximized )
			{
				KeyCode key = Event.current.keyCode;
				if ( m_isMouseInside || hasKeyboardFocus )
				{
					if ( key == ShortcutsManager.ScrollUpKey )
					{
						m_currentScrollPos.y -= 10;
						if ( m_currentScrollPos.y < 0 )
						{
							m_currentScrollPos.y = 0;
						}
						Event.current.Use();
					}

					if ( key == ShortcutsManager.ScrollDownKey )
					{
						m_currentScrollPos.y += 10;
						Event.current.Use();
					}
				}

				if( m_forceUpdate )
				{
					if( m_propertyReordableList != null )
						m_propertyReordableList.ReleaseKeyboardFocus();
					m_propertyReordableList = null;

					if ( m_functionInputsReordableList != null )
						m_functionInputsReordableList.ReleaseKeyboardFocus();
					m_functionInputsReordableList = null;

					if( m_functionSwitchesReordableList != null )
						m_functionSwitchesReordableList.ReleaseKeyboardFocus();
					m_functionSwitchesReordableList = null;

					if ( m_functionOutputsReordableList != null )
						m_functionOutputsReordableList.ReleaseKeyboardFocus();
					m_functionOutputsReordableList = null;
					m_forceUpdate = false;
				}

				GUILayout.BeginArea( m_transformedArea, m_content, m_style );
				{
					//Draw selected node parameters
					if ( selectedNode != null )
					{
						// this hack is need because without it the several FloatFields/Textfields/... would show wrong values ( different from the ones they were assigned to show )
						if ( m_lastSelectedNode != selectedNode.UniqueId )
						{
							m_lastSelectedNode = selectedNode.UniqueId;
							GUI.FocusControl( "" );
						}

						EditorGUILayout.BeginVertical();
						{
							EditorGUILayout.Separator();
							if ( selectedNode.UniqueId == ParentWindow.CurrentGraph.CurrentMasterNodeId )
							{
								m_dummyContent.text = "Output Node";
							}
							else
							{
								if ( selectedNode.Attributes != null )
								{

									m_dummyContent.text = selectedNode.Attributes.Name;
								}
								else if ( selectedNode is CommentaryNode )
								{
									m_dummyContent.text = "Commentary";
								}
								else
								{
									m_dummyContent.text = TitleStr;
								}
							}

							EditorGUILayout.LabelField( m_dummyContent, m_nodePropertiesStyle );

							EditorGUILayout.Separator();
							//UIUtils.RecordObject( selectedNode , "Changing properties on node " + selectedNode.UniqueId);
							m_currentScrollPos = EditorGUILayout.BeginScrollView( m_currentScrollPos, GUILayout.Width( 0 ), GUILayout.Height( 0 ) );
							float labelWidth = EditorGUIUtility.labelWidth;
							if ( selectedNode.TextLabelWidth > 0 )
								EditorGUIUtility.labelWidth = selectedNode.TextLabelWidth;

							changeCheck = selectedNode.SafeDrawProperties();
							EditorGUIUtility.labelWidth = labelWidth;
							EditorGUILayout.EndScrollView();
						}
						EditorGUILayout.EndVertical();

						if ( changeCheck )
						{
							if ( selectedNode.ConnStatus == NodeConnectionStatus.Connected )
								ParentWindow.SetSaveIsDirty();
						}
					}
					else
					{
						//Draw Graph Params
						EditorGUILayout.BeginVertical();
						{
							EditorGUILayout.Separator();
							EditorGUILayout.LabelField( "Graph Properties", m_nodePropertiesStyle );
							EditorGUILayout.Separator();

							m_currentScrollPos = EditorGUILayout.BeginScrollView( m_currentScrollPos, GUILayout.Width( 0 ), GUILayout.Height( 0 ) );
							float labelWidth = EditorGUIUtility.labelWidth;
							EditorGUIUtility.labelWidth = 90;

							bool generalIsVisible = m_parentWindow.InnerWindowVariables.ExpandedGeneralShaderOptions;
							NodeUtils.DrawPropertyGroup( ref generalIsVisible, " General", DrawGeneralFunction );
							m_parentWindow.InnerWindowVariables.ExpandedGeneralShaderOptions = generalIsVisible;
							AmplifyShaderFunction function = ParentWindow.CurrentGraph.CurrentShaderFunction;
							if( function != null )
							{
								//function.AdditionalIncludes.Draw( ParentWindow.CurrentGraph.CurrentOutputNode );
								//function.AdditionalPragmas.Draw( ParentWindow.CurrentGraph.CurrentOutputNode );
								function.AdditionalDirectives.Draw( ParentWindow.CurrentGraph.CurrentOutputNode );
							}

							bool inputIsVisible = m_parentWindow.InnerWindowVariables.ExpandedFunctionInputs;
							NodeUtils.DrawPropertyGroup( ref inputIsVisible, " Function Inputs", DrawFunctionInputs );
							m_parentWindow.InnerWindowVariables.ExpandedFunctionInputs = inputIsVisible;

							bool swicthIsVisible = m_parentWindow.InnerWindowVariables.ExpandedFunctionSwitches;
							NodeUtils.DrawPropertyGroup( ref swicthIsVisible, " Function Switches", DrawFunctionSwitches );
							m_parentWindow.InnerWindowVariables.ExpandedFunctionSwitches = swicthIsVisible;

							bool outputIsVisible = m_parentWindow.InnerWindowVariables.ExpandedFunctionOutputs;
							NodeUtils.DrawPropertyGroup( ref outputIsVisible, " Function Outputs", DrawFunctionOutputs );
							m_parentWindow.InnerWindowVariables.ExpandedFunctionOutputs = outputIsVisible;

							bool properties = ParentWindow.InnerWindowVariables.ExpandedProperties;
							NodeUtils.DrawPropertyGroup( ref properties, " Material Properties", DrawFunctionProperties );
							ParentWindow.InnerWindowVariables.ExpandedProperties = properties;

							EditorGUIUtility.labelWidth = labelWidth;
							EditorGUILayout.EndScrollView();
						}
						EditorGUILayout.EndVertical();
					}
				}
				// Close window area
				GUILayout.EndArea();
			}

			PostDraw();
			return changeCheck;
		}

		public void DrawGeneralFunction()
		{
			AmplifyShaderFunction function = ParentWindow.CurrentGraph.CurrentShaderFunction;
			if ( function == null )
				return;

			float cacheWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 115;

			SerializedObject serializedObject = new UnityEditor.SerializedObject( function );

			if ( serializedObject != null )
			{
				SerializedProperty temo = serializedObject.FindProperty( "m_description" );
				EditorGUILayout.PropertyField( temo, new GUIContent( "    Description" ) );

				SerializedProperty cat = serializedObject.FindProperty( "m_nodeCategory" );
				SerializedProperty ppos = serializedObject.FindProperty( "m_previewPosition" );
				
				EditorGUILayout.PropertyField( ppos, new GUIContent( "Preview Position" ) );
				cat.intValue = ParentWindow.CurrentGraph.CurrentOutputNode.EditorGUILayoutPopup( "Category", cat.intValue, UIUtils.CategoryPresets );

				if( cat.enumValueIndex == 0 )
				{
					SerializedProperty custCat = serializedObject.FindProperty( "m_customNodeCategory" );
					EditorGUILayout.PropertyField( custCat, new GUIContent( "Custom" ) );
				}
				serializedObject.ApplyModifiedProperties();
			}
			EditorGUIUtility.labelWidth = cacheWidth;
		}


		public void DrawFunctionInputs()
		{
			List<FunctionInput> functionInputNodes = UIUtils.FunctionInputList();

			if ( m_functionInputsReordableList == null || functionInputNodes.Count != m_functionInputsLastCount )
			{
				functionInputNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

				m_functionInputsReordableList = new ReorderableList( functionInputNodes, typeof( FunctionInput ), true, false, false, false );
				m_functionInputsReordableList.headerHeight = 0;
				m_functionInputsReordableList.footerHeight = 0;
				m_functionInputsReordableList.showDefaultBackground = false;

				m_functionInputsReordableList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
				{
					EditorGUI.LabelField( rect, functionInputNodes[ index ].InputName );
				};

				m_functionInputsReordableList.onChangedCallback = ( list ) =>
				{
					//for ( int i = 0; i < functionInputNodes.Count; i++ )
					//{
					//	functionInputNodes[ i ].OrderIndex = i;
					//}
					ForceInputReorder( ref functionInputNodes );
				};

				m_functionInputsLastCount = m_functionInputsReordableList.count;
			}

			if ( m_functionInputsReordableList != null )
			{
				if ( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.BeginVertical( m_propertyAdjustment );
				m_functionInputsReordableList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
		}

		public void ForceInputReorder( ref List<FunctionInput> functionInputNodes )
		{
			for( int i = 0; i < functionInputNodes.Count; i++ )
			{
				functionInputNodes[ i ].OrderIndex = i;
			}
		}

		public void DrawFunctionSwitches()
		{
			List<FunctionSwitch> functionSwitchNodes = UIUtils.FunctionSwitchList();

			if( m_functionSwitchesReordableList == null || functionSwitchNodes.Count != m_functionSwitchesLastCount )
			{
				functionSwitchNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

				UIUtils.UpdateFunctionSwitchArr();
				
				m_functionSwitchesReordableList = new ReorderableList( functionSwitchNodes, typeof( FunctionSwitch ), true, false, false, false );
				m_functionSwitchesReordableList.headerHeight = 0;
				m_functionSwitchesReordableList.footerHeight = 0;
				m_functionSwitchesReordableList.showDefaultBackground = false;

				m_functionSwitchesReordableList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
				{
					EditorGUI.LabelField( rect, functionSwitchNodes[ index ].OptionLabel );
				};

				m_functionSwitchesReordableList.onChangedCallback = ( list ) =>
				{
					ForceSwitchesReorder(ref functionSwitchNodes );
				};

				m_functionSwitchesLastCount = m_functionSwitchesReordableList.count;
			}

			if( m_functionSwitchesReordableList != null )
			{
				if( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.BeginVertical( m_propertyAdjustment );
				m_functionSwitchesReordableList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
		}

		public void ForceSwitchesReorder( ref List<FunctionSwitch> functionSwitchNodes )
		{
			for( int i = 0; i < functionSwitchNodes.Count; i++ )
			{
				functionSwitchNodes[ i ].OrderIndex = i;
			}

			UIUtils.UpdateFunctionSwitchArr();
		}

		public void DrawFunctionOutputs()
		{
			List<FunctionOutput> functionOutputNodes = UIUtils.FunctionOutputList();

			if ( m_functionOutputsReordableList == null || functionOutputNodes.Count != m_functionOutputsLastCount )
			{
				functionOutputNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

				m_functionOutputsReordableList = new ReorderableList( functionOutputNodes, typeof( FunctionOutput ), true, false, false, false );
				m_functionOutputsReordableList.headerHeight = 0;
				m_functionOutputsReordableList.footerHeight = 0;
				m_functionOutputsReordableList.showDefaultBackground = false;

				m_functionOutputsReordableList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
				{
					EditorGUI.LabelField( rect, functionOutputNodes[ index ].OutputName );
				};

				m_functionOutputsReordableList.onChangedCallback = ( list ) =>
				{
					for ( int i = 0; i < functionOutputNodes.Count; i++ )
					{
						functionOutputNodes[ i ].OrderIndex = i;
					}
				};

				m_functionOutputsLastCount = m_functionOutputsReordableList.count;
			}

			if ( m_functionOutputsReordableList != null )
			{
				if ( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.BeginVertical( m_propertyAdjustment );
				m_functionOutputsReordableList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
		}

		public void DrawFunctionProperties()
		{
			List<PropertyNode> nodes = UIUtils.PropertyNodesList();
			if ( m_propertyReordableList == null || nodes.Count != m_lastCount )
			{
				m_propertyReordableNodes.Clear();

				for ( int i = 0; i < nodes.Count; i++ )
				{
					ReordenatorNode rnode = nodes[ i ] as ReordenatorNode;
					if ( ( rnode == null || !rnode.IsInside ) && ( !m_propertyReordableNodes.Exists( x => x.PropertyName.Equals( nodes[ i ].PropertyName ) ) ) )
						m_propertyReordableNodes.Add( nodes[ i ] );
				}

				m_propertyReordableNodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

				m_propertyReordableList = new ReorderableList( m_propertyReordableNodes, typeof( PropertyNode ), true, false, false, false );
				m_propertyReordableList.headerHeight = 0;
				m_propertyReordableList.footerHeight = 0;
				m_propertyReordableList.showDefaultBackground = false;

				m_propertyReordableList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
				{
					EditorGUI.LabelField( rect, /*m_propertyReordableNodes[ index ].OrderIndex + " " + */m_propertyReordableNodes[ index ].PropertyInspectorName );
				};

				m_propertyReordableList.onChangedCallback = ( list ) =>
				{
					ReorderList( ref nodes );
				};

				ReorderList( ref nodes );

				m_lastCount = m_propertyReordableList.count;
			}

			if ( m_propertyReordableList != null )
			{
				if ( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.BeginVertical( m_propertyAdjustment );
				m_propertyReordableList.DoLayoutList();
				EditorGUILayout.EndVertical();
			}
		}

		public void ForceReordering()
		{
			List<PropertyNode> nodes = UIUtils.PropertyNodesList();
			ReorderList( ref nodes );

			List<FunctionInput> functionInputNodes = UIUtils.FunctionInputList();
			ForceInputReorder( ref functionInputNodes );

			List<FunctionSwitch> functionSwitchNodes = UIUtils.FunctionSwitchList();
			ForceSwitchesReorder( ref functionSwitchNodes );
			//RecursiveLog();
		}

		private void RecursiveLog()
		{
			List<PropertyNode> nodes = UIUtils.PropertyNodesList();
			nodes.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );
			for( int i = 0; i < nodes.Count; i++ )
			{
				if( ( nodes[ i ] is ReordenatorNode ) )
					( nodes[ i ] as ReordenatorNode ).RecursiveLog();
				else
					Debug.Log( nodes[ i ].OrderIndex + " " + nodes[ i ].PropertyName );
			}
		}


		private void ReorderList( ref List<PropertyNode> nodes )
		{
			// clear lock list before reordering because of multiple sf being used
			for( int i = 0; i < nodes.Count; i++ )
			{
				ReordenatorNode rnode = nodes[ i ] as ReordenatorNode;
				if ( rnode != null )
					rnode.RecursiveClear();
			}

			int propoffset = 0;
			int count = 0;
			for ( int i = 0; i < m_propertyReordableNodes.Count; i++ )
			{
				ReordenatorNode renode = m_propertyReordableNodes[ i ] as ReordenatorNode;
				if ( renode != null )
				{
					if ( !renode.IsInside )
					{
						m_propertyReordableNodes[ i ].OrderIndex = count + propoffset;

						if ( renode.PropertyListCount > 0 )
						{
							propoffset += renode.RecursiveCount();

							// the same reordenator can exist multiple times, apply ordering to all of them
							for( int j = 0; j < nodes.Count; j++ )
							{
								ReordenatorNode pnode = ( nodes[ j ] as ReordenatorNode );
								if ( pnode != null && pnode.PropertyName.Equals( renode.PropertyName ) )
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
						m_propertyReordableNodes[ i ].OrderIndex = 0;
					}
				}
				else
				{
					m_propertyReordableNodes[ i ].OrderIndex = count + propoffset;
					count++;
				}
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_functionInputsReordableList = null;
			m_functionOutputsReordableList = null;
			m_propertyReordableList = null;
		}

		public bool ForceUpdate
		{
			get { return m_forceUpdate; }
			set { m_forceUpdate = value; }
		}
	}
}
