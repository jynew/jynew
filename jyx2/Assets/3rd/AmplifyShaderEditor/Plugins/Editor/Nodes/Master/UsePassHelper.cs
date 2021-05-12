using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AmplifyShaderEditor
{
	public enum UsePassLocation
	{
		Above,
		Below
	}

	[Serializable]
	public class UsePassItem : ScriptableObject
	{
		public UsePassLocation Location;
		public string Value;
		public UsePassItem()
		{
			Location = UsePassLocation.Above;
			Value = string.Empty;
		}

		public UsePassItem( UsePassLocation location, string name )
		{
			Location = location;
			Value = name;
		}

	}

	[Serializable]
	public class UsePassHelper : ScriptableObject
	{
		private const string UseGrabFormatNewLine = "UsePass \"{0}\"\n";
		private const string UseGrabFormat = "UsePass \"{0}\"";
		private const float ShaderKeywordButtonLayoutWidth = 15;
		private const string ShaderPoputContext = "CONTEXT/ShaderPopup";

		[SerializeField]
		private List<UsePassItem> m_items = new List<UsePassItem>();

		[SerializeField]
		private UndoParentNode m_owner = null;

		[SerializeField]
		protected bool m_isDirty = false;

		[SerializeField]
		protected string m_moduleName = string.Empty;

		private ReorderableList m_reordableList = null;
		private ReordableAction m_actionType = ReordableAction.None;
		private int m_actionIndex = 0;
		private GUIStyle m_propertyAdjustment;

		private Material m_dummyMaterial;
		private MenuCommand m_dummyCommand;
		private int m_currentUsePassIdx = 0;
		
		void DrawButtons()
		{
			EditorGUILayout.Separator();

			// Add keyword
			if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				UsePassItem newItem = ScriptableObject.CreateInstance<UsePassItem>();
				newItem.hideFlags = HideFlags.HideAndDontSave;
				m_items.Add( newItem );
				EditorGUI.FocusTextInControl( null );
				m_isDirty = true;
			}

			//Remove keyword
			if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				if( m_items.Count > 0 )
				{
					UsePassItem itemToDelete = m_items[ m_items.Count - 1 ];
					m_items.RemoveAt( m_items.Count - 1 );
					ScriptableObject.DestroyImmediate( itemToDelete );
					EditorGUI.FocusTextInControl( null );
				}
				m_isDirty = true;
			}
		}

		public void Draw( UndoParentNode owner, bool style = true )
		{
			if( m_owner == null )
				m_owner = owner;

			if( m_reordableList == null )
			{
				m_reordableList = new ReorderableList( m_items, typeof( UsePassItem ), true, false, false, false )
				{
					headerHeight = 0,
					footerHeight = 0,
					showDefaultBackground = false,
					drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
					{
						if( m_items[ index ] != null )
						{
							float labelWidthMultiplier;
							float popUpWidth;
							float shaderSelectorMultiplier;
							float buttonPlusPosMultiplier;
							if( style )
							{
								rect.x -= 10;
								labelWidthMultiplier = 0.9f;
								popUpWidth = 0.31f;
								shaderSelectorMultiplier = 1.01f;
								buttonPlusPosMultiplier = 0.78f;
							}
							else
							{
								rect.x -= 1;
								labelWidthMultiplier = 1.01f;
								popUpWidth = 0.25f;
								shaderSelectorMultiplier = 1.0f;
								buttonPlusPosMultiplier = 0.55f;
							}

							Rect popupPos = new Rect( rect.x, rect.y + 2, popUpWidth * rect.width, rect.height );
							Rect labelPos = new Rect( rect.x + popupPos.width * labelWidthMultiplier, rect.y, 0.59f * rect.width, rect.height );

							Rect shaderSelectorPos = new Rect( labelPos.x + labelPos.width* shaderSelectorMultiplier, rect.y, 15, rect.height );

							Rect buttonPlusPos = new Rect( shaderSelectorPos.x + shaderSelectorPos.width * buttonPlusPosMultiplier, rect.y, ShaderKeywordButtonLayoutWidth, rect.height );
							Rect buttonMinusPos = new Rect( buttonPlusPos.x + buttonPlusPos.width, rect.y, ShaderKeywordButtonLayoutWidth, rect.height );

							EditorGUI.BeginChangeCheck();
							m_items[ index ].Location = (UsePassLocation)owner.EditorGUIEnumPopup( popupPos, m_items[ index ].Location );

							if( EditorGUI.EndChangeCheck() && m_items[ index ].Location == UsePassLocation.Below && m_owner != null && m_owner.ContainerGraph.CurrentCanvasMode == NodeAvailability.TemplateShader )
							{
								m_items[ index ].Location = UsePassLocation.Above;
								UIUtils.ShowMessage( "Below option still not available on templates" );
							}
							m_items[ index ].Value = owner.EditorGUITextField( labelPos, string.Empty, m_items[ index ].Value );

							if( GUI.Button( shaderSelectorPos, string.Empty, UIUtils.InspectorPopdropdownFallback ) )
							{
								EditorGUI.FocusTextInControl( null );
								GUI.FocusControl( null );
								m_currentUsePassIdx = index;
								DisplayShaderContext( owner, GUILayoutUtility.GetRect( GUIContent.none, EditorStyles.popup ) );
							}

							if( GUI.Button( buttonPlusPos, string.Empty, UIUtils.PlusStyle ) )
							{
								m_actionType = ReordableAction.Add;
								m_actionIndex = index;
							}

							if( GUI.Button( buttonMinusPos, string.Empty, UIUtils.MinusStyle ) )
							{
								m_actionType = ReordableAction.Remove;
								m_actionIndex = index;
							}
						}
					}
				};
			}

			if( m_actionType != ReordableAction.None )
			{
				switch( m_actionType )
				{
					case ReordableAction.Add:
					UsePassItem newItem = ScriptableObject.CreateInstance<UsePassItem>();
					newItem.hideFlags = HideFlags.HideAndDontSave;
					m_items.Insert( m_actionIndex + 1, newItem );
					break;
					case ReordableAction.Remove:
					UsePassItem itemToDelete = m_items[ m_actionIndex ];
					m_items.RemoveAt( m_actionIndex );
					ScriptableObject.DestroyImmediate( itemToDelete );
					break;
				}
				m_isDirty = true;
				m_actionType = ReordableAction.None;
				EditorGUI.FocusTextInControl( null );
			}
			bool foldoutValue = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedUsePass;
			if( style )
			{
				NodeUtils.DrawPropertyGroup( ref foldoutValue, m_moduleName, DrawReordableList, DrawButtons );
			}
			else
			{
				NodeUtils.DrawNestedPropertyGroup( ref foldoutValue, m_moduleName, DrawReordableList, DrawButtons );
			}
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedUsePass = foldoutValue;
		}
		
		private void DisplayShaderContext( UndoParentNode node, Rect r )
		{
			if( m_dummyCommand == null )
				m_dummyCommand = new MenuCommand( this, 0 );

			if( m_dummyMaterial == null )
				m_dummyMaterial = new Material( Shader.Find( "Hidden/ASESShaderSelectorUnlit" ) );

#pragma warning disable 0618
			UnityEditorInternal.InternalEditorUtility.SetupShaderMenu( m_dummyMaterial );
#pragma warning restore 0618
			EditorUtility.DisplayPopupMenu( r, ShaderPoputContext, m_dummyCommand );
		}

		private void OnSelectedShaderPopup( string command, Shader shader )
		{
			if( shader != null )
			{
				UIUtils.MarkUndoAction();
				Undo.RecordObject( m_owner, "Selected Use Pass shader" );
				m_items[ m_currentUsePassIdx ].Value = shader.name;
			}
		}

		void DrawReordableList()
		{
			if( m_reordableList != null )
			{
				if( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				EditorGUILayout.Space();

				if( m_items.Count == 0 )
				{
					EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add one.", MessageType.Info );
				}
				else
				{
					m_reordableList.DoLayoutList();
				}
				EditorGUILayout.Space();
			}
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			try
			{
				int count = Convert.ToInt32( nodeParams[ index++ ] );
				for( int i = 0; i < count; i++ )
				{
					string locationValue = nodeParams[ index++ ];
					// REMOVE THIS TEST AFTER A COUPLE OF VERSIONS (curr v1.5.6 r02)
					if( locationValue.Equals( "Bellow" ) ) locationValue = "Below";

					UsePassLocation location = (UsePassLocation)Enum.Parse( typeof( UsePassLocation ), locationValue );
					string name = nodeParams[ index++ ];
					UsePassItem newItem = ScriptableObject.CreateInstance<UsePassItem>();
					newItem.hideFlags = HideFlags.HideAndDontSave;
					newItem.Location = location;
					newItem.Value = name;
					m_items.Add( newItem );
				}
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_items.Count );
			for( int i = 0; i < m_items.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_items[ i ].Location );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_items[ i ].Value );
			}
		}

		public void BuildUsePassInfo( ref string aboveItems, ref string bellowItems, string tabs )
		{
			int count = m_items.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_items[ i ].Location == UsePassLocation.Above )
				{
					aboveItems += tabs + string.Format( UseGrabFormatNewLine, m_items[ i ].Value );
				}
				else
				{
					bellowItems += tabs + string.Format( UseGrabFormatNewLine, m_items[ i ].Value );
				}
			}
		}

		public void BuildUsePassInfo( ref List<PropertyDataCollector> aboveItems, ref List<PropertyDataCollector> bellowItems )
		{
			int count = m_items.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_items[ i ].Location == UsePassLocation.Above )
				{
					aboveItems.Add( new PropertyDataCollector(-1,string.Format( UseGrabFormat, m_items[ i ].Value )));
				}
				else
				{
					bellowItems.Add( new PropertyDataCollector( -1, string.Format( UseGrabFormat, m_items[ i ].Value ) ) );
				}
			}
		}

		public string ModuleName { set { m_moduleName = value; } }
		public void Destroy()
		{
			m_owner = null;
			m_items.Clear();
			m_items = null;
			m_reordableList = null;
			m_dummyMaterial = null;
			m_dummyCommand = null;
		}
	}
}
