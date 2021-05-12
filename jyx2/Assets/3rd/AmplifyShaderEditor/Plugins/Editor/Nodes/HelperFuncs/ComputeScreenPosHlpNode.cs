// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using UnityEngine;
using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Compute Screen Pos", "Camera And Screen", "Computes texture coordinate for doing a screenspace-mapped texture sample. Input is clip space position" )]
	public sealed class ComputeScreenPosHlpNode : HelperParentNode
	{
		[SerializeField]
		private bool m_normalize = false;
		private string NormalizeStr = "Normalize";
		private readonly string[] NormalizeOps =
		{	"{0} = {0} / {0}.w;",
			"{0}.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? {0}.z : {0}.z* 0.5 + 0.5;"
		};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "ComputeScreenPos";
			m_funcHDFormatOverride = "ComputeScreenPos( {0} , _ProjectionParams.x )";
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_outputPorts[ 0 ].Name = "XYZW";
			m_autoWrapProperties = true;
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			m_localVarName = "computeScreenPos" + OutputId;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_normalize = EditorGUILayoutToggle( NormalizeStr, m_normalize );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string result = base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			if( m_normalize )
			{
				dataCollector.AddLocalVariable( UniqueId, string.Format( NormalizeOps[ 0 ], m_localVarName ) );
				dataCollector.AddLocalVariable( UniqueId, string.Format( NormalizeOps[ 1 ], m_localVarName ) );
			}
			return result;
		}
		
		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalize );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 15404 )
			{
				m_normalize = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			}
		}

	}
}
