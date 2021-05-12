// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

//https://docs.unity3d.com/Manual/SL-BuiltinFunctions.html

using System;
using UnityEngine;
namespace AmplifyShaderEditor
{
	[Serializable]
	public class HelperParentNode : ParentNode
	{
		[SerializeField]
		protected string m_funcType = string.Empty;

		[SerializeField]
		protected string m_funcLWFormatOverride = string.Empty;

		[SerializeField]
		protected string m_funcHDFormatOverride = string.Empty;

		protected string m_localVarName = null;
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, Constants.EmptyPortValue );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
		}

		public override string GetIncludes()
		{
			return Constants.UnityCgLibFuncs;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );

			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );

			if( !( dataCollector.IsTemplate && dataCollector.IsSRP ) )
				dataCollector.AddToIncludes( UniqueId, Constants.UnityCgLibFuncs );

			string concatResults = string.Empty;
			for( int i = 0; i < m_inputPorts.Count; i++ )
			{
				string result = string.Empty;
				if( m_inputPorts[ i ].IsConnected )
				{
					result = m_inputPorts[ i ].GeneratePortInstructions( ref dataCollector );
				}
				else
				{
					result = m_inputPorts[ i ].WrappedInternalData;
				}

				concatResults += result;
				if( i != ( m_inputPorts.Count - 1 ) )
					concatResults += " , ";
			}
			string finalResult = m_funcType + "( " + concatResults + " )";
			if( dataCollector.IsTemplate )
			{
				if( dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.Lightweight && !string.IsNullOrEmpty( m_funcLWFormatOverride ) )
				{
					finalResult = string.Format( m_funcLWFormatOverride, concatResults );
				}
				else if( dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD && !string.IsNullOrEmpty( m_funcHDFormatOverride ) )
				{
					finalResult = string.Format( m_funcHDFormatOverride, concatResults );
				}

			}

			RegisterLocalVariable( 0, finalResult, ref dataCollector, m_localVarName );
			return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory ) );
		}
	}
}
