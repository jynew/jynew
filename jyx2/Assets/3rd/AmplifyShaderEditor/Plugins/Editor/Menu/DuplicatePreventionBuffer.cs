// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class DuplicatePreventionBuffer
	{
		private const string VectorNameStr = "Vector ";
		private const string TextureSampleNameStr = "Texture Sample ";
		private const string MatrixNameStr = "Matrix ";
		private const string IntNameStr = "Int ";
		private const string FloatNameStr = "Float ";
		private const string ColorNameStr = "Color ";
		
		[SerializeField]
		private int[] m_availableUVChannelsArray = { -1, -1, -1, -1 };
		private string[] m_availableUVChannelsNamesArray = { "null",
															"null",
															"null",
															"null" };

		private Dictionary<string, int> m_availablePropertyNames = new Dictionary<string, int>();
		private Dictionary<string, int> m_availableUniformNames = new Dictionary<string, int>();
		private Dictionary<string, int> m_availableLocalVariableNames = new Dictionary<string, int>();

		public void ReleaseAllUVChannels()
		{
			for ( int i = 0; i < m_availableUVChannelsArray.Length; i++ )
			{
				m_availableUVChannelsArray[ i ] = -1;
			}
		}

		public bool RegisterUVChannel( int nodeId, int channelId, string name )
		{
			if ( channelId < 0 ||
					channelId > ( m_availableUVChannelsArray.Length - 1 ) ||
					m_availableUVChannelsArray[ channelId ] >= 0 )
			{
				return false;
			}

			m_availableUVChannelsArray[ channelId ] = nodeId;
			m_availableUVChannelsNamesArray[ channelId ] = name;
			return true;
		}


		public bool ReleaseUVChannel( int nodeId, int channelId )
		{
			if ( channelId < 0 ||
				channelId > ( m_availableUVChannelsArray.Length - 1 ) )
			{
				return false;
			}

			if ( m_availableUVChannelsArray[ channelId ] == nodeId )
			{
				m_availableUVChannelsArray[ channelId ] = -1;
				return true;
			}
			return false;
		}

		public int RegisterFirstAvailableChannel( int nodeId , string name)
		{
			for ( int i = 0; i < m_availableUVChannelsArray.Length; i++ )
			{
				if ( m_availableUVChannelsArray[ i ] == -1 )
				{
					m_availableUVChannelsArray[ i ] = nodeId;
					m_availableUVChannelsNamesArray[ i ] = name;
					return i;
				}
			}
			return -1;
		}

		public bool IsChannelAvailable( int channelId )
		{
			if ( channelId < 0 ||
				channelId > ( m_availableUVChannelsArray.Length - 1 ) )
			{
				return false;
			}

			return ( m_availableUVChannelsArray[ channelId ] < 0 );
		}

		public int GetFirstOccupiedChannel()
		{
			for ( int i = 0; i < 4; i++ )
			{
				if ( m_availableUVChannelsArray[ i ] > -1 )
					return i;
			}
			return -1;
		}

		public string GetChannelName( int channelId )
		{
			if ( channelId < 0 ||
				channelId > ( m_availableUVChannelsArray.Length - 1 ) )
			{
				return string.Empty;
			}

			return m_availableUVChannelsNamesArray[ channelId ] ;
		}

		public void SetChannelName( int channelId , string name )
		{
			if ( channelId < 0 ||
				channelId > ( m_availableUVChannelsArray.Length - 1 ) )
			{
				return;
			}
			 m_availableUVChannelsNamesArray[ channelId ] = name;
		}

		public bool RegisterLocalVariableName( int nodeId, string name )
		{
			if ( name.Length == 0 )
				return false;

			if ( m_availableLocalVariableNames.ContainsKey( name ) )
			{
				if ( m_availableLocalVariableNames[ name ] > -1 )
				{
					return false;
				}
				else
				{
					m_availableLocalVariableNames[ name ] = nodeId;
					return true;
				}
			}

			m_availableLocalVariableNames.Add( name, nodeId );
			return true;
		}

		public int CheckUniformNameOwner( string name )
		{
			if ( name.Length == 0 )
				return -1;

			if ( m_availableUniformNames.ContainsKey( name ) )
			{
				return m_availableUniformNames[ name ];
			}

			return -1;
		}

		public bool RegisterUniformName( int nodeId, string name )
		{
			if ( name.Length == 0 )
				return false;

			if ( m_availableUniformNames.ContainsKey( name ) )
			{
				if ( m_availableUniformNames[ name ] > -1 )
				{
					return false;
				}
				else
				{
					m_availableUniformNames[ name ] = nodeId;
					return true;
				}
			}
			
			m_availableUniformNames.Add( name, nodeId );
			return true;
		}

		public void DumpUniformNames()
		{
			string val = "CONTENTS\n";
			foreach ( KeyValuePair<string, int> kvp in m_availableUniformNames )
			{
				val += ( "key " + kvp.Key + " : value " + kvp.Value + "\n" );
			}
		}

		public void DumpLocalVariableNames()
		{
			string val = "CONTENTS\n";
			foreach ( KeyValuePair<string, int> kvp in m_availableLocalVariableNames )
			{
				val += ( "key " + kvp.Key + " : value " + kvp.Value + "\n" );
			}
		}


		public bool ReleaseUniformName( int nodeId, string name )
		{
			if ( !string.IsNullOrEmpty(name) && name.Length == 0 )
				return false;

			if ( m_availableUniformNames.ContainsKey( name ) )
			{
				if ( m_availableUniformNames[ name ] == nodeId )
				{
					m_availableUniformNames.Remove( name );
					return true;
				}
			}
			return false;
		}

		public bool ReleaseLocalVariableName( int nodeId, string name )
		{
			if ( name.Length == 0 )
				return false;

			if ( m_availableLocalVariableNames.ContainsKey( name ) )
			{
				if ( m_availableLocalVariableNames[ name ] == nodeId )
				{
					m_availableLocalVariableNames.Remove( name );
					return true;
				}
			}
			return false;
		}

		public void ReleaseAllUniformNames()
		{
			m_availableUniformNames.Clear();
		}

		public void ReleaseAllLocalVariableNames()
		{
			m_availableLocalVariableNames.Clear();
		}

		public void GetFirstAvailableName( int nodeId, WirePortDataType type , out string outProperty , out string outInspector, bool useCustomPrefix = false, string customPrefix = null)
		{
			string name = string.Empty;
			if ( useCustomPrefix && customPrefix != null )
			{
				name = customPrefix;
			}
			else
			{
				switch ( type )
				{
					case WirePortDataType.OBJECT:
					case WirePortDataType.FLOAT:
					{
						name = FloatNameStr;
					}
					break;
					case WirePortDataType.INT:
					{
						name = IntNameStr;
					}
					break;
					case WirePortDataType.FLOAT2:
					case WirePortDataType.FLOAT3:
					case WirePortDataType.FLOAT4:
					{
						name = VectorNameStr;
					}
					break;
					case WirePortDataType.FLOAT3x3:
					case WirePortDataType.FLOAT4x4:
					{
						name = MatrixNameStr;
					}
					break;
					case WirePortDataType.COLOR:
					{
						name = ColorNameStr;
					}
					break;
				}
			}

			int count = 0;
			bool foundName = false;
			while ( !foundName )
			{
				string inspectorName = name + count;
				string propertyName =  UIUtils.GeneratePropertyName( inspectorName , PropertyType.Property );
				
				if ( IsUniformNameAvailable( propertyName ) )
				{
					outInspector = inspectorName;
					outProperty = propertyName;
					RegisterUniformName( nodeId, propertyName );
					return;
				}
				count += 1;
			}
			outProperty = string.Empty;
			outInspector = string.Empty;
			UIUtils.ShowMessage( "Could not find a valid name " + MessageSeverity.Warning );
		}

		public bool IsUniformNameAvailable( string name )
		{
			if ( m_availableUniformNames.ContainsKey( name ) && m_availableUniformNames[ name ] > -1 )
				return false;
			return true;
		}

		public bool IsLocalvariableNameAvailable( string name )
		{
			if ( m_availableLocalVariableNames.ContainsKey( name ) && m_availableLocalVariableNames[ name ] > -1 )
				return false;
			return true;
		}

		public bool GetPropertyName( int nodeId, string name )
		{
			if ( m_availablePropertyNames.ContainsKey( name ) )
			{
				if ( m_availablePropertyNames[ name ] > -1 )
				{
					return false;
				}
				else
				{
					m_availablePropertyNames[ name ] = nodeId;
					return true;
				}
			}

			m_availablePropertyNames.Add( name, nodeId );
			return true;
		}


		public bool ReleasePropertyName( int nodeId, string name )
		{
			if ( m_availablePropertyNames.ContainsKey( name ) )
			{
				if ( m_availablePropertyNames[ name ] == nodeId )
				{
					m_availablePropertyNames[ name ] = -1;
					return true;
				}
			}
			return false;
		}

		public void ReleaseAllPropertyNames()
		{
			m_availablePropertyNames.Clear();
		}

		public bool IsPropertyNameAvailable( string name )
		{
			if ( m_availablePropertyNames.ContainsKey( name ) && m_availablePropertyNames[ name ] > -1 )
				return false;
			return true;
		}

		public void ReleaseAllData()
		{
			ReleaseAllUVChannels();
			ReleaseAllUniformNames();
			ReleaseAllPropertyNames();
			ReleaseAllLocalVariableNames();
		}
	}
}
