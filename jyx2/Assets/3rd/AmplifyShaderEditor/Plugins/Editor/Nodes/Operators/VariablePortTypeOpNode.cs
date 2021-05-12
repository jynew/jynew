// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public class VariablePortTypeOpNode : ParentNode
	{
		private const string InputTypeStr = "Input type";

		[SerializeField]
		protected WirePortDataType m_selectedType = WirePortDataType.FLOAT;

		[SerializeField]
		protected WirePortDataType m_lastSelectedType = WirePortDataType.FLOAT;

		[SerializeField]
		protected int _inputAmount = 1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddPorts();
		}

		void AddPorts()
		{
			for ( int i = 0; i < _inputAmount; i++ )
			{
				AddInputPort( m_selectedType, true, i.ToString() );
			}
			AddOutputPort( m_selectedType, Constants.EmptyPortValue );
			m_sizeIsDirty = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.LabelField( InputTypeStr );
				m_selectedType = ( WirePortDataType ) EditorGUILayoutEnumPopup( m_selectedType );
			}
			EditorGUILayout.EndVertical();
			if ( m_selectedType != m_lastSelectedType )
			{
				m_lastSelectedType = m_selectedType;

				DeleteAllInputConnections( true );
				DeleteAllOutputConnections( true );

				AddPorts();

			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedType );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedType = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
			m_lastSelectedType = m_selectedType;
			DeleteAllInputConnections( true );
			DeleteAllOutputConnections( true );
			AddPorts();
		}
	}
}
