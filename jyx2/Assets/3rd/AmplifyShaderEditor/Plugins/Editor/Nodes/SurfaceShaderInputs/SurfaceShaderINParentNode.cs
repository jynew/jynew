// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class SurfaceShaderINParentNode : ParentNode
	{
		[SerializeField]
		protected SurfaceInputs m_currentInput;

		[SerializeField]
		protected string m_currentInputValueStr;

		[SerializeField]
		protected string m_currentInputDecStr;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = SurfaceInputs.UV_COORDS;
			m_textLabelWidth = 65;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			DrawPrecisionProperty();
		}
		//This needs to be called on the end of the CommonInit on all children
		protected void InitialSetup()
		{
			m_currentInputValueStr = Constants.InputVarStr + "." + UIUtils.GetInputValueFromType( m_currentInput );
			
			string outputName = "Out";
			switch ( m_currentInput )
			{
				case SurfaceInputs.DEPTH:
				{
					AddOutputPort( WirePortDataType.FLOAT, outputName );
				}
				break;
				case SurfaceInputs.UV_COORDS:
				{
					outputName = "UV";
					AddOutputVectorPorts( WirePortDataType.FLOAT2, outputName );
				}
				break;
				case SurfaceInputs.UV2_COORDS:
				{
					outputName = "UV";
					AddOutputVectorPorts( WirePortDataType.FLOAT2, outputName );
				}
				break;
				case SurfaceInputs.VIEW_DIR:
				{
					outputName = "XYZ";
					AddOutputVectorPorts( WirePortDataType.FLOAT3, outputName );
				}
				break;
				case SurfaceInputs.COLOR:
				{
					outputName = "RGBA";
					AddOutputVectorPorts( WirePortDataType.FLOAT4, outputName );
				}
				break;
				case SurfaceInputs.SCREEN_POS:
				{
					outputName = "XYZW";
					AddOutputVectorPorts( WirePortDataType.FLOAT4, outputName );
				}
				break;
				case SurfaceInputs.WORLD_POS:
				{
					outputName = "XYZ";
					AddOutputVectorPorts( WirePortDataType.FLOAT3, outputName );
				}
				break;
				case SurfaceInputs.WORLD_REFL:
				{
					outputName = "XYZ";
					AddOutputVectorPorts( WirePortDataType.FLOAT3, outputName );
				}
				break;
				case SurfaceInputs.WORLD_NORMAL:
				{
					outputName = "XYZ";
					AddOutputVectorPorts( WirePortDataType.FLOAT3, outputName );
				}
				break;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			dataCollector.AddToInput( UniqueId, m_currentInput, m_currentPrecisionType );
			switch ( m_currentInput )
			{
				case SurfaceInputs.VIEW_DIR:
				case SurfaceInputs.WORLD_REFL:
				case SurfaceInputs.WORLD_NORMAL:
				{
					dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
				}
				break;
				case SurfaceInputs.WORLD_POS:
				case SurfaceInputs.DEPTH:
				case SurfaceInputs.UV_COORDS:
				case SurfaceInputs.UV2_COORDS:
				case SurfaceInputs.COLOR:
				case SurfaceInputs.SCREEN_POS: break;
			};

			return GetOutputVectorItem( 0, outputId, m_currentInputValueStr );
		}

	}
}
