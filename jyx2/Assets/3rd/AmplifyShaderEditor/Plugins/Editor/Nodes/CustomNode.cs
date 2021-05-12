// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class CustomNodeOutputData
	{
		public string expression;
		public string name;
		public List<CustomNodeInputData> inputData;

		public CustomNodeOutputData( string newName, string newExpression )
		{
			name = newName;
			expression = newExpression;
			inputData = new List<CustomNodeInputData>();
		}
		public void Destroy()
		{
			inputData.Clear();
			inputData = null;
		}

		public override string ToString()
		{
			string result = "name: " + name + " outputExpression: " + expression + '\n';
			for ( int i = 0; i < inputData.Count; i++ )
			{
				result += inputData[ i ].ToString() + '\n';
			}
			return result;
		}
	}

	[Serializable]
	public class CustomNodeInputData
	{
		public int index;
		public int length;
		public string name;
		public CustomNodeInputData( int newIndex, int newLength, string newName )
		{
			index = newIndex;
			length = newLength;
			name = newName;

		}

		public override string ToString()
		{
			return "index: " + index + " length: " + length + " name: " + name;
		}
	}

	[Serializable]
	public class CustomNode : ParentNode
	{
		[SerializeField]
		private List<string> m_includes;

		[SerializeField]
		private List<CustomNodeOutputData> m_outputData;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_outputData = new List<CustomNodeOutputData>();
		}

		public void AddIncludes( string newInclude )
		{
			m_includes.Add( newInclude );
		}

		protected void AddOutputsFromString( string outputName, string output )
		{
			AddOutputPort( WirePortDataType.OBJECT, outputName );

			CustomNodeOutputData currOutputData = new CustomNodeOutputData( outputName, output );


			// Get existing input nodes so we can test for duplicates
			Dictionary<string, InputPort> existingPorts = InputPortsDict;

			// Create dictionary to prevent duplicates when dealing with expresssions with multiple occurences of an input 
			Dictionary<string, string> inputDuplicatePrevention = new Dictionary<string, string>();


			// Get all inputs on the expression and save their info  
			int[] indexes = output.AllIndexesOf( Constants.CNIP );
			for ( int i = 0; i < indexes.Length; i++ )
			{
				string name = output.Substring( indexes[ i ], Constants.CNIP.Length + 1 );
				currOutputData.inputData.Add( new CustomNodeInputData( indexes[ i ], Constants.CNIP.Length + 1, name ) );

				if ( !inputDuplicatePrevention.ContainsKey( name ) && !existingPorts.ContainsKey( name ) )
				{
					inputDuplicatePrevention.Add( name, name );
					AddInputPort( WirePortDataType.OBJECT, false, name );
				}
			}

			inputDuplicatePrevention.Clear();
			inputDuplicatePrevention = null;

			existingPorts.Clear();
			existingPorts = null;

			m_outputData.Add( currOutputData );

		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			if ( outputId < m_outputData.Count )
			{
				Dictionary<string, InputPort> inputs = InputPortsDict;

				string value = m_outputData[ outputId ].expression;
				for ( int i = 0; i < m_outputData[ outputId ].inputData.Count; i++ )
				{
					if ( inputs.ContainsKey( m_outputData[ outputId ].inputData[ i ].name ) )
					{
						InputPort inputPort = inputs[ m_outputData[ outputId ].inputData[ i ].name ];
						if ( inputPort != null )
						{
							string inputValue = inputPort.GenerateShaderForOutput( ref dataCollector, WirePortDataType.OBJECT, ignoreLocalvar );
							value = value.Replace( m_outputData[ outputId ].inputData[ i ].name, inputValue );
						}
						else
						{
							UIUtils.ShowMessage( m_outputData[ outputId ].inputData[ i ].name + " invalid on the inputs list", MessageSeverity.Error );
							return string.Empty;
						}
					}
					else
					{
						UIUtils.ShowMessage( m_outputData[ outputId ].inputData[ i ].name + " Not found on the inputs list", MessageSeverity.Error );
						return string.Empty;
					}
				}
				return value;

			}

			return string.Empty;
		}
		public void DumpOutputData()
		{
			for ( int i = 0; i < m_outputData.Count; i++ )
			{
				Debug.Log( m_outputData[ i ] );
			}
		}

		public override void Destroy()
		{
			base.Destroy();

			if ( m_outputData != null )
			{
				for ( int i = 0; i < m_outputData.Count; i++ )
				{
					m_outputData[ i ].Destroy();
				}
				m_outputData.Clear();
				m_outputData = null;
			}
			if ( m_includes != null )
			{
				m_includes.Clear();
				m_includes = null;
			}
		}

	}
}
