// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateVertexData
	{
		[SerializeField]
		private TemplateSemantics m_semantics = TemplateSemantics.NONE;
		[SerializeField]
		private WirePortDataType m_dataType = WirePortDataType.OBJECT;
		[SerializeField]
		private string m_varName = string.Empty;
		[SerializeField]
		private TemplateInfoOnSematics m_dataInfo = TemplateInfoOnSematics.NONE;
		[SerializeField]
		private string m_dataSwizzle = string.Empty;
		[SerializeField]
		private bool m_available = false;
		[SerializeField]
		private string m_varNameWithSwizzle = string.Empty;
		[SerializeField]
		private bool m_isSingleComponent = true;
		[SerializeField]
		private string[] m_components = { "0", "0", "0", "0" };
		[SerializeField]
		private bool[] m_componentUsage = { false, false,false,false };

		public TemplateVertexData( TemplateSemantics semantics, WirePortDataType dataType, string varName )
		{
			m_semantics = semantics;
			m_dataType = dataType;
			m_varName = varName;
			m_varNameWithSwizzle = varName;
		}

		public TemplateVertexData( TemplateSemantics semantics, WirePortDataType dataType, string varName, string dataSwizzle )
		{
			m_semantics = semantics;
			m_dataType = dataType;
			m_varName = varName;
			m_dataSwizzle = dataSwizzle;
			m_varNameWithSwizzle = varName + dataSwizzle;
		}

		public TemplateVertexData( TemplateVertexData other )
		{
			m_semantics = other.m_semantics;
			m_dataType = other.m_dataType;
			m_varName = other.m_varName;
			m_dataInfo = other.m_dataInfo;
			m_dataSwizzle = other.m_dataSwizzle;
			m_available = other.m_available;
			m_varNameWithSwizzle = other.m_varNameWithSwizzle;
			m_isSingleComponent = other.IsSingleComponent;
			for( int i = 0; i < 4; i++ )
			{
				m_components[ i ] = other.Components[ i ];
			}
		}

		public void RegisterComponent( char channelId, string value )
		{
			int channelIdInt = -1;
			switch( channelId )
			{
				case 'r':
				case 'x': channelIdInt = 0; break;
				case 'g':
				case 'y': channelIdInt = 1; break;
				case 'b':
				case 'z': channelIdInt = 2; break;
				case 'a':
				case 'w': channelIdInt = 3; break;
			}

			if( channelId < 0 )
			{
				Debug.LogWarning( "Attempting to create interpolated data from invalid channel " + channelId );
				return;
			}

			RegisterComponent( channelIdInt, value );
		}

		public void RegisterComponent( int channelId, string value )
		{
			channelId = Mathf.Clamp( channelId, 0, 3 );
			m_components[ channelId ] = value;
			m_componentUsage[ channelId ] = true;
			m_isSingleComponent = false;
		}

		public void BuildVar( PrecisionType precisionType = PrecisionType.Float )
		{
			if( m_isSingleComponent )
				return;
			WirePortDataType dataType = WirePortDataType.FLOAT;
			if( m_componentUsage[ 3 ] )
			{
				dataType = WirePortDataType.FLOAT4;
			}
			else if( m_componentUsage[ 2 ] )
			{
				dataType = WirePortDataType.FLOAT3;
			}
			else if( m_componentUsage[ 1 ] )
			{
				dataType = WirePortDataType.FLOAT2;
			}
			
			string newVar = UIUtils.FinalPrecisionWirePortToCgType( precisionType, dataType );
			newVar += "( ";
			switch( dataType )
			{
				default: newVar += "0"; break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				{
					newVar += "{0}."+Components[ 0 ];
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					newVar +=	"{0}." + Components[ 0 ] + ", " +
								"{0}." + Components[ 1 ];
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					newVar +=	"{0}." + Components[ 0 ] + ", " +
								"{0}." + Components[ 1 ] + ", " +
								"{0}." + Components[ 2 ];
				}
				break;
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				{
					newVar +=	"{0}." + Components[ 0 ] + ", " +
								"{0}." + Components[ 1 ] + ", " +
								"{0}." + Components[ 2 ] + ", " +
								"{0}." + Components[ 3 ];
				}
				break;

			}
			newVar += " )";
			m_varName = newVar;
			m_varNameWithSwizzle = newVar;
		}

		public bool IsSingleComponent { get { return m_isSingleComponent; } }
		public string[] Components { get { return m_components; } }
		public TemplateSemantics Semantics { get { return m_semantics; } }
		public WirePortDataType DataType { get { return m_dataType; } }
		public string VarName { get { return m_varName; } set { m_varName = value; m_varNameWithSwizzle = value + m_dataSwizzle; } }
		public string DataSwizzle { get { return m_dataSwizzle; } set { m_dataSwizzle = value; m_varNameWithSwizzle = m_varName + value; } }
		public TemplateInfoOnSematics DataInfo { get { return m_dataInfo; } set { m_dataInfo = value; } }
		public bool Available { get { return m_available; } set { m_available = value; } }
		public string VarNameWithSwizzle { get { return m_varNameWithSwizzle; } }
		public WirePortDataType SwizzleType
		{
			get
			{
				if ( string.IsNullOrEmpty( m_dataSwizzle ) )
					return m_dataType;

				WirePortDataType newType = m_dataType;
				switch ( m_dataSwizzle.Length )
				{
					case 2: newType = WirePortDataType.FLOAT;break;
					case 3: newType = WirePortDataType.FLOAT2; break;
					case 4: newType = WirePortDataType.FLOAT3; break;
					case 5: newType = WirePortDataType.FLOAT4; break;
				}

				return newType;
			}
		}

	}
}
