// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Component Mask", "Vector Operators", "Mask certain channels from vectors/color components", null, KeyCode.K )]
	public sealed class ComponentMaskNode : ParentNode
	{
		private const string OutputLocalVarName = "componentMask";
		[SerializeField]
		private bool[] m_selection = { true, true, true, true };

		[SerializeField]
		private int m_outputPortCount = 4;

		[SerializeField]
		private string[] m_labels;

		private int m_cachedOrderId = -1;
		private int m_cachedSingularId = -1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT4, false, Constants.EmptyPortValue );
			AddOutputPort( WirePortDataType.FLOAT4, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
			m_selectedLocation = PreviewLocation.TopCenter;
			m_labels = new string[] { "X", "Y", "Z", "W" };
			m_previewShaderGUID = "b78e2b295c265cd439c80d218fb3e88e";
			SetAdditonalTitleText( "Value( XYZW )" );
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();
			
			Vector4 order = new Vector4(-1,-1,-1,-1);
			int lastIndex = 0;
			int singularId = -1;
			if ( m_selection[ 0 ] )
			{
				order.Set( lastIndex, order.y , order.z , order.w );
				lastIndex++;
				singularId = 0;
			}
			if ( m_selection[ 1 ] )
			{
				order.Set( order.x, lastIndex, order.z, order.w );
				lastIndex++;
				singularId = 1;
			}
			if ( m_selection[ 2 ] )
			{
				order.Set( order.x, order.y, lastIndex, order.w );
				lastIndex++;
				singularId = 2;
			}
			if ( m_selection[ 3 ] )
			{
				order.Set( order.x, order.y, order.z, lastIndex );
				lastIndex++;
				singularId = 3;
			}

			if ( lastIndex != 1 )
				singularId = -1;

			if ( m_cachedOrderId == -1 )
				m_cachedOrderId = Shader.PropertyToID( "_Order" );

			if ( m_cachedSingularId == -1 )
				m_cachedSingularId = Shader.PropertyToID( "_Singular" );

			PreviewMaterial.SetVector( m_cachedOrderId, order );
			PreviewMaterial.SetFloat( m_cachedSingularId, singularId );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdatePorts();
			UpdateTitle();
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			UpdatePorts();
			UpdateTitle();
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			UpdateTitle();
		}

		void UpdatePorts()
		{
			m_inputPorts[ 0 ].MatchPortToConnection();
			int count = 0;
			switch ( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.OBJECT:
				case WirePortDataType.COLOR:
				{
					count = 4;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					count = 3;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					count = 2;
				}
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}

			int activeCount = 0;
			if ( count > 0 )
			{
				for ( int i = 0; i < count; i++ )
				{
					if ( m_selection[ i ] )
						activeCount += 1;
				}
			}

			m_outputPortCount = activeCount;
			switch ( activeCount )
			{
				case 0: ChangeOutputType( m_inputPorts[ 0 ].DataType, false ); break;
				case 1: ChangeOutputType( WirePortDataType.FLOAT, false ); break;
				case 2: ChangeOutputType( WirePortDataType.FLOAT2, false ); break;
				case 3: ChangeOutputType( WirePortDataType.FLOAT3, false ); break;
				case 4: ChangeOutputType( m_inputPorts[ 0 ].DataType, false ); break;
			}
			
		}
		
		private void UpdateTitle()
		{
			int count = 0;
			string additionalText = string.Empty;
			switch ( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.OBJECT:
				case WirePortDataType.COLOR:
				{
					count = 4;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					count = 3;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					count = 2;
				}
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				{
					count = 0;
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}

			if ( count > 0 )
			{
				for ( int i = 0; i < count; i++ )
				{
					if ( m_selection[ i ] )
					{
						additionalText += UIUtils.GetComponentForPosition( i, m_inputPorts[ 0 ].DataType ).ToUpper();
					}
				}
			}

			if ( additionalText.Length > 0 )
				SetAdditonalTitleText( "Value( " + additionalText + " )" );
			else
				SetAdditonalTitleText( string.Empty );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			EditorGUILayout.BeginVertical();

			int count = 0;
			switch ( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.OBJECT:
				case WirePortDataType.COLOR:
				{
					count = 4;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					count = 3;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					count = 2;
				}
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}

			int activeCount = 0;
			if ( count > 0 )
			{
				for ( int i = 0; i < count; i++ )
				{
					m_selection[ i ] = EditorGUILayoutToggleLeft( m_labels[i], m_selection[ i ] );
					m_labels[ i ] = UIUtils.GetComponentForPosition( i, m_inputPorts[ 0 ].DataType ).ToUpper();
					if ( m_selection[ i ] )
					{
						activeCount += 1;
					}
				}
			}

			if ( activeCount != m_outputPortCount )
			{
				m_outputPortCount = activeCount;
				switch ( activeCount )
				{
					case 0: ChangeOutputType( m_inputPorts[ 0 ].DataType, false ); break;
					case 1: ChangeOutputType( WirePortDataType.FLOAT, false ); break;
					case 2: ChangeOutputType( WirePortDataType.FLOAT2, false ); break;
					case 3: ChangeOutputType( WirePortDataType.FLOAT3, false ); break;
					case 4: ChangeOutputType( m_inputPorts[ 0 ].DataType, false ); break;
				}
				UpdateTitle();
				SetSaveIsDirty();
			}

			EditorGUILayout.EndVertical();
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{

			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string value = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );

			int count = 0;
			switch ( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.OBJECT:
				case WirePortDataType.COLOR:
				{
					count = 4;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					count = 3;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					count = 2;
				}
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				{
					count = 0;
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}

			if ( count > 0 )
			{
				bool firstElement = true;
				value = string.Format("({0})",value);
				for ( int i = 0; i < count; i++ )
				{
					if ( m_selection[ i ] )
					{
						if( firstElement )
						{
							firstElement = false;
							value += ".";
						}
						value += UIUtils.GetComponentForPosition( i, m_inputPorts[ 0 ].DataType );
					}
				}
			}

			return CreateOutputLocalVariable( 0, value, ref dataCollector );
		}

		public string GetComponentForPosition( int i )
		{
			switch ( i )
			{
				case 0:
				{
					return ( ( m_outputPorts[ 0 ].DataType == WirePortDataType.COLOR ) ? "r" : "x" );
				}
				case 1:
				{
					return ( ( m_outputPorts[ 0 ].DataType == WirePortDataType.COLOR ) ? "g" : "y" );
				}
				case 2:
				{
					return ( ( m_outputPorts[ 0 ].DataType == WirePortDataType.COLOR ) ? "b" : "z" );
				}
				case 3:
				{
					return ( ( m_outputPorts[ 0 ].DataType == WirePortDataType.COLOR ) ? "a" : "w" );
				}
			}
			return string.Empty;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			for ( int i = 0; i < 4; i++ )
			{
				m_selection[ i ] = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
			UpdateTitle();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			for ( int i = 0; i < 4; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_selection[ i ] );
			}
		}
	}
}
