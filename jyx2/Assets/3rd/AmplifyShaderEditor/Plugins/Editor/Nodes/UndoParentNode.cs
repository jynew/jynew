using UnityEditor;
using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class UndoParentNode : ScriptableObject
	{
		private const string MessageFormat = "Changing value {0} on node {1}";

		[SerializeField]
		protected NodeAttributes m_nodeAttribs;

		[SerializeField]
		protected ParentGraph m_containerGraph;

		public void UndoRecordObject( string name )
		{
			UIUtils.MarkUndoAction();
			Undo.RegisterCompleteObjectUndo( UIUtils.CurrentWindow, name );
			Undo.RecordObject( this, name );
		}

		public virtual void RecordObject( string Id )
		{
			Undo.RecordObject( this, Id );
		}
		public virtual void RecordObjectOnDestroy( string Id )
		{
			Undo.RecordObject( this, Id );
		}

		public string EditorGUILayoutStringField( string name, string value, params GUILayoutOption[] options )
		{
			string newValue = EditorGUILayout.TextField( name, value, options );
			if ( !newValue.Equals( value ) )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUILayoutStringField", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public string EditorGUILayoutTextField( GUIContent label, string text, params GUILayoutOption[] options )
		{
			string newValue = EditorGUILayout.TextField( label, text, options );
			if ( !text.Equals( newValue ) )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public string EditorGUILayoutTextField( string label, string text, params GUILayoutOption[] options )
		{
			string newValue = EditorGUILayout.TextField( label, text, options );
			if ( !text.Equals( newValue ) )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Enum EditorGUILayoutEnumPopup( GUIContent label, Enum selected, params GUILayoutOption[] options )
		{
			Enum newValue = EditorGUILayout.EnumPopup( label, selected, options );
			if ( !newValue.ToString().Equals( selected.ToString() ) )
			{
				UndoRecordObject(string.Concat( "Changing value ", label, " on node ", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
				//UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Enum EditorGUILayoutEnumPopup( string label, Enum selected, params GUILayoutOption[] options )
		{
			Enum newValue = EditorGUILayout.EnumPopup( label, selected, options );
			if ( !newValue.ToString().Equals( selected.ToString() ) )
			{
				UndoRecordObject(string.Concat( "Changing value ", label, " on node ", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
				//UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Enum EditorGUILayoutEnumPopup( Enum selected, params GUILayoutOption[] options )
		{
			Enum newValue = EditorGUILayout.EnumPopup( selected, options );
			if ( !newValue.ToString().Equals( selected.ToString() ) )
			{
				UndoRecordObject(string.Concat( "Changing value EditorGUILayoutEnumPopup on node ", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
				//UndoRecordObject(string.Format( MessageFormat, "EditorGUILayoutEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutIntPopup( string label, int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntPopup( label, selectedValue, displayedOptions, optionValues, options );
			if ( newValue != selectedValue )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}


		public int EditorGUILayoutPopup( string label, int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( label, selectedIndex, displayedOptions, style, options );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}


		public int EditorGUILayoutPopup( GUIContent label, int selectedIndex, GUIContent[] displayedOptions, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( label, selectedIndex, displayedOptions, options );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
		
		public int EditorGUILayoutPopup( GUIContent label, int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( label, selectedIndex, displayedOptions, style, options );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
		
		public int EditorGUILayoutPopup( int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( selectedIndex, displayedOptions, options );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUILayoutPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutPopup( string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.Popup( label, selectedIndex, displayedOptions, options );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutToggle( GUIContent label, bool value, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.Toggle( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutToggle( string label, bool value, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.Toggle( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutToggle( string label, bool value, GUIStyle style, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.Toggle( label, value, style, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutIntField( int value, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntField( value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUILayoutIntField", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutIntField( GUIContent label, int value, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntField( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutIntField( string label, int value, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntField( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public float EditorGUILayoutFloatField( GUIContent label, float value, params GUILayoutOption[] options )
		{
			float newValue = EditorGUILayout.FloatField( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public float EditorGUILayoutFloatField( string label, float value, params GUILayoutOption[] options )
		{
			float newValue = EditorGUILayout.FloatField( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Color EditorGUILayoutColorField( string label, Color value, params GUILayoutOption[] options )
		{
			Color newValue = EditorGUILayout.ColorField( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
#if UNITY_2018_1_OR_NEWER
		public Color EditorGUILayoutColorField( GUIContent label, Color value, bool showEyedropper, bool showAlpha, bool hdr, params GUILayoutOption[] options )
		{
			Color newValue = EditorGUILayout.ColorField( label, value, showEyedropper, showAlpha, hdr, options );
			if( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
#else
		public Color EditorGUILayoutColorField( GUIContent label, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig, params GUILayoutOption[] options )
		{
			Color newValue = EditorGUILayout.ColorField( label, value, showEyedropper, showAlpha, hdr, hdrConfig, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
#endif
		public float EditorGUILayoutSlider( string label, float value, float leftValue, float rightValue, params GUILayoutOption[] options )
		{
			float newValue = EditorGUILayout.Slider( label, value, leftValue, rightValue, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public float EditorGUILayoutSlider( GUIContent label, float value, float leftValue, float rightValue, params GUILayoutOption[] options )
		{
			float newValue = EditorGUILayout.Slider( label, value, leftValue, rightValue, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
		public UnityEngine.Object EditorGUILayoutObjectField( string label, UnityEngine.Object obj, System.Type objType, bool allowSceneObjects, params GUILayoutOption[] options )
		{
			UnityEngine.Object newValue = EditorGUILayout.ObjectField( label, obj, objType, allowSceneObjects, options );
			if ( newValue != obj )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Vector2 EditorGUIVector2Field( Rect position, string label, Vector2 value )
		{
			Vector2 newValue = EditorGUI.Vector2Field( position, label, value );
			if( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}


		public Vector2 EditorGUILayoutVector2Field( string label, Vector2 value, params GUILayoutOption[] options )
		{
			Vector2 newValue = EditorGUILayout.Vector2Field( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Vector3 EditorGUIVector3Field( Rect position, string label, Vector3 value )
		{
			Vector3 newValue = EditorGUI.Vector3Field( position, label, value );
			if( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Vector3 EditorGUILayoutVector3Field( string label, Vector3 value, params GUILayoutOption[] options )
		{
			Vector3 newValue = EditorGUILayout.Vector3Field( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Vector4 EditorGUIVector4Field( Rect position, string label, Vector4 value )
		{
			Vector4 newValue = EditorGUI.Vector4Field( position, label, value );
			if( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Vector4 EditorGUILayoutVector4Field( string label, Vector4 value, params GUILayoutOption[] options )
		{
			Vector4 newValue = EditorGUILayout.Vector4Field( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutIntSlider( GUIContent label, int value, int leftValue, int rightValue, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntSlider( label, value, leftValue, rightValue, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUILayoutIntSlider( string label, int value, int leftValue, int rightValue, params GUILayoutOption[] options )
		{
			int newValue = EditorGUILayout.IntSlider( label, value, leftValue, rightValue, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutToggleLeft( string label, bool value, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.ToggleLeft( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutToggleLeft( GUIContent label, bool value, params GUILayoutOption[] options )
		{
			bool newValue = EditorGUILayout.ToggleLeft( label, value, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public string EditorGUILayoutTextArea( string text, GUIStyle style, params GUILayoutOption[] options )
		{
			string newValue = EditorGUILayout.TextArea( text, style, options );
			if ( !newValue.Equals( text ) )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUILayoutTextArea", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUILayoutFoldout( bool foldout, string content )
		{
			bool newValue = EditorGUILayout.Foldout( foldout, content );
			if ( newValue != foldout )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUILayoutFoldout", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUIFoldout( Rect position, bool foldout, string content )
		{
			bool newValue = EditorGUI.Foldout( position, foldout, content );
			if( newValue != foldout )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUIFoldout", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public string EditorGUITextField( Rect position, string label, string text )
		{
			string newValue = EditorGUI.TextField( position, label, text );
			if( !newValue.Equals( text ) )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public string EditorGUITextField( Rect position, string label, string text, [UnityEngine.Internal.DefaultValue( "EditorStyles.textField" )] GUIStyle style )
		{
			string newValue = EditorGUI.TextField( position, label, text, style );
			if ( !newValue.Equals( text ) )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
#if UNITY_2018_1_OR_NEWER
		public Color EditorGUIColorField( Rect position, GUIContent label, Color value, bool showEyedropper, bool showAlpha, bool hdr )
		{
			Color newValue = EditorGUI.ColorField( position, label, value, showEyedropper, showAlpha, hdr );
			if( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
#else
		public Color EditorGUIColorField( Rect position, GUIContent label, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig )
		{
			Color newValue = EditorGUI.ColorField( position, label, value, showEyedropper, showAlpha, hdr, hdrConfig );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
#endif
		public Color EditorGUIColorField( Rect position, string label, Color value )
		{
			Color newValue = EditorGUI.ColorField( position, label, value );
			if( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}


		public int EditorGUIIntField( Rect position, string label, int value )
		{
			int newValue = EditorGUI.IntField( position, label, value );
			if( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUIIntField( Rect position, string label, int value, [UnityEngine.Internal.DefaultValue( "EditorStyles.numberField" )] GUIStyle style )
		{
			int newValue = EditorGUI.IntField( position, label, value, style );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public float EditorGUIFloatField( Rect position, string label, float value )
		{
			float newValue = EditorGUI.FloatField( position, label, value );
			if( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public float EditorGUIFloatField( Rect position, string label, float value, [UnityEngine.Internal.DefaultValue( "EditorStyles.numberField" )] GUIStyle style )
		{
			float newValue = EditorGUI.FloatField( position, label, value, style );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, label, ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public float EditorGUIFloatField( Rect position, float value, [UnityEngine.Internal.DefaultValue( "EditorStyles.numberField" )] GUIStyle style )
		{
			float newValue = EditorGUI.FloatField( position, value, style );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUIFloatField", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

        public float GUIHorizontalSlider( Rect position, float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb )
        {
            float newValue = GUI.HorizontalSlider( position, value, leftValue, rightValue, slider, thumb );
            if( newValue != value )
            {
                UndoRecordObject(string.Format( MessageFormat, "GUIHorizontalSlider", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
            }
            return newValue;
        }
		
		public Enum EditorGUIEnumPopup( Rect position, Enum selected )
		{
			Enum newValue = EditorGUI.EnumPopup( position, selected );
			if( !newValue.ToString().Equals( selected.ToString() ) )
			{
				UndoRecordObject(string.Concat( "Changing value EditorGUIEnumPopup on node ", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
				//UndoRecordObject(string.Format( MessageFormat, "EditorGUIEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public Enum EditorGUIEnumPopup( Rect position, Enum selected, [UnityEngine.Internal.DefaultValue( "EditorStyles.popup" )] GUIStyle style )
		{
			Enum newValue = EditorGUI.EnumPopup( position, selected, style );
			if ( !newValue.ToString().Equals( selected.ToString() ) )
			{
				UndoRecordObject(string.Concat( "Changing value EditorGUIEnumPopup on node ", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
				//UndoRecordObject(string.Format( MessageFormat, "EditorGUIEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
        
		public int EditorGUIIntPopup( Rect position, int selectedValue, GUIContent[] displayedOptions, int[] optionValues, [UnityEngine.Internal.DefaultValue( "EditorStyles.popup" )] GUIStyle style )
		{
			int newValue = EditorGUI.IntPopup( position, selectedValue, displayedOptions, optionValues, style );
			if ( newValue != selectedValue )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUIIntEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUIPopup( Rect position, string label, int selectedIndex, string[] displayedOptions)
		{
			int newValue = EditorGUI.Popup( position, label, selectedIndex, displayedOptions );
			if( newValue != selectedIndex )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUIEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUIPopup( Rect position, int selectedIndex, GUIContent[] displayedOptions, [UnityEngine.Internal.DefaultValue( "EditorStyles.popup" )] GUIStyle style )
		{
			int newValue = EditorGUI.Popup( position, selectedIndex, displayedOptions, style );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUIEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public int EditorGUIPopup( Rect position, int selectedIndex, string[] displayedOptions, [UnityEngine.Internal.DefaultValue( "EditorStyles.popup" )] GUIStyle style )
		{
			int newValue = EditorGUI.Popup( position, selectedIndex, displayedOptions, style );
			if ( newValue != selectedIndex )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUIEnumPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public UnityEngine.Object EditorGUIObjectField( Rect position, UnityEngine.Object obj, System.Type objType, bool allowSceneObjects )
		{
			UnityEngine.Object newValue = EditorGUI.ObjectField( position, obj, objType, allowSceneObjects );
			if ( newValue != obj )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUIObjectField", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}
		public int EditorGUIIntPopup( Rect position, int selectedValue, string[] displayedOptions, int[] optionValues, [UnityEngine.Internal.DefaultValue( "EditorStyles.popup" )] GUIStyle style )
		{
			int newValue = EditorGUI.IntPopup( position, selectedValue, displayedOptions, optionValues, style );
			if ( newValue != selectedValue )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUIIntPopup", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool EditorGUIToggle( Rect position, bool value )
		{
			bool newValue = EditorGUI.Toggle( position, value );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, "EditorGUIToggle", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public string GUITextField( Rect position, string text, GUIStyle style )
		{
			string newValue = GUI.TextField( position, text, style );
			if ( !newValue.Equals( text ) )
			{
				UndoRecordObject(string.Format( MessageFormat, "GUITextfield", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}


		public bool GUILayoutToggle( bool value, string text, GUIStyle style, params GUILayoutOption[] options )
		{
			bool newValue = GUILayout.Toggle( value, text, style, options );
			if ( newValue != value )
			{
				UndoRecordObject(string.Format( MessageFormat, "GUILayoutToggle", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return newValue;
		}

		public bool GUILayoutButton( string text, GUIStyle style, params GUILayoutOption[] options )
		{
			bool value = GUILayout.Button( text, style, options );
			if ( value )
			{
				UndoRecordObject(string.Format( MessageFormat, "GUILayoutButton", ( ( m_nodeAttribs != null ) ? m_nodeAttribs.Name : GetType().ToString() ) ) );
			}
			return value;
		}

		/// <summary>
		/// It's the graph the node exists in, this is set after node creation and it's not available on CommonInit
		/// </summary>
		public ParentGraph ContainerGraph
		{
			get { return m_containerGraph; }
			set { m_containerGraph = value; }
		}
	}
}

