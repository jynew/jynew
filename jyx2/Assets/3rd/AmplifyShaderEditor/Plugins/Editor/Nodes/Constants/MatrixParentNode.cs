// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class MatrixParentNode : PropertyNode
	{
		private readonly string[] AvailablePropertyTypeLabels = { PropertyType.Constant.ToString(), PropertyType.Global.ToString(), "Instanced" };
		private readonly int[] AvailablePropertyTypeValues = { (int)PropertyType.Constant, (int)PropertyType.Global , (int)PropertyType.InstancedProperty };

		protected bool m_isEditingFields;

		[SerializeField]
		protected Matrix4x4 m_defaultValue = Matrix4x4.identity;

		[SerializeField]
		protected Matrix4x4 m_materialValue = Matrix4x4.identity;

		[NonSerialized]
		protected Matrix4x4 m_previousValue;

		private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();

		public MatrixParentNode() : base() { }
		public MatrixParentNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_freeType = false;

		}

		public override void AfterCommonInit()
		{
			base.AfterCommonInit();
			m_hasLeftDropdown = true;
			m_drawAttributes = false;
			m_availableAttribs.Clear();

			if( PaddingTitleLeft == 0 )
			{
				PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
				if( PaddingTitleRight == 0 )
					PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
			}
		}

		protected void DrawParameterType()
		{
			PropertyType parameterType = (PropertyType)EditorGUILayoutIntPopup( ParameterTypeStr, (int)m_currentParameterType, AvailablePropertyTypeLabels, AvailablePropertyTypeValues );
			if( parameterType != m_currentParameterType )
			{
				ChangeParameterType( parameterType );
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			PropertyType parameterType = (PropertyType)m_upperLeftWidget.DrawWidget( this, (int)m_currentParameterType, AvailablePropertyTypeLabels, AvailablePropertyTypeValues );
			if( parameterType != m_currentParameterType )
			{
				ChangeParameterType( parameterType );
			}
		}

		public override void DrawMainPropertyBlock()
		{
			DrawParameterType();
			base.DrawMainPropertyBlock();
		}

		public override void Destroy()
		{
			base.Destroy();
			m_upperLeftWidget = null;
		}

		public override void SetGlobalValue() { Shader.SetGlobalMatrix( m_propertyName, m_defaultValue ); }
		public override void FetchGlobalValue() { m_materialValue = Shader.GetGlobalMatrix( m_propertyName ); }
	}
}
