using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum ZWriteMode
	{
		On,
		Off
	}

	public enum ZTestMode
	{
		Less,
		Greater,
		LEqual,
		GEqual,
		Equal,
		NotEqual,
		Always
	}

	[Serializable]
	class ZBufferOpHelper
	{
		public static readonly string DepthParametersStr = " Depth";
		public static readonly string ZWriteModeStr = "ZWrite Mode";
		public static readonly string ZTestModeStr = "ZTest Mode";
		public static readonly string OffsetStr = "Offset";
		public static readonly string OffsetFactorStr = "Factor";
		public static readonly string OffsetUnitsStr = "Units";
		private const string ExtraDepthPassStr = "Extra Depth Pass";
		private const string DepthZTestStr = "Depth ZTest";

		public static readonly string[] ZTestModeLabels =
		{
			"<Default>",
			"Less",
			"Greater",
			"Less or Equal",
			"Greater or Equal",
			"Equal",
			"Not Equal",
			"Always"
		};

		public static readonly string[] ZTestModeValues =
		{
			"<Default>",
			"Less",
			"Greater",
			"LEqual",
			"GEqual",
			"Equal",
			"NotEqual",
			"Always"
		};

		public static readonly string[] ZWriteModeValues =
		{
			"<Default>",
			"On",
			"Off"
		};

		public static readonly Dictionary<ZTestMode, int> ZTestModeDict = new Dictionary<ZTestMode, int>
		{
			{ZTestMode.Less,1 },
			{ZTestMode.Greater,2},
			{ZTestMode.LEqual,3},
			{ZTestMode.GEqual,4},
			{ZTestMode.Equal,5},
			{ZTestMode.NotEqual,6},
			{ZTestMode.Always,7}
		};

		public static readonly Dictionary<ZWriteMode, int> ZWriteModeDict = new Dictionary<ZWriteMode, int>
		{
			{ ZWriteMode.On,1},
			{ ZWriteMode.Off,2}
		};


		[SerializeField]
		private InlineProperty m_zTestMode = new InlineProperty();

		[SerializeField]
		private InlineProperty m_zWriteMode = new InlineProperty();
		[SerializeField]
		private InlineProperty m_offsetFactor = new InlineProperty();

		[SerializeField]
		private InlineProperty m_offsetUnits = new InlineProperty();

		[SerializeField]
		private bool m_offsetEnabled;

		[SerializeField]
		private bool m_extraDepthPass;

		[SerializeField]
		private int m_extrazTestMode = 0;

		[SerializeField]
		private StandardSurfaceOutputNode m_parentSurface;

		public string CreateDepthInfo( bool outlineZWrite, bool outlineZTest )
		{
			string result = string.Empty;
			if( m_zWriteMode.IntValue != 0 || m_zWriteMode.Active )
			{
				MasterNode.AddRenderState( ref result, "ZWrite", m_zWriteMode.GetValueOrProperty( ZWriteModeValues[ m_zWriteMode.IntValue ] ) );
			}
			else if( outlineZWrite )
			{
				MasterNode.AddRenderState( ref result, "ZWrite", ZWriteModeValues[ 1 ] );
			}

			if( m_zTestMode.IntValue != 0 || m_zTestMode.Active )
			{
				MasterNode.AddRenderState( ref result, "ZTest", m_zTestMode.GetValueOrProperty( ZTestModeValues[ m_zTestMode.IntValue ] ) );
			}
			else if( outlineZTest )
			{
				MasterNode.AddRenderState( ref result, "ZTest", ZTestModeValues[ 3 ] );
			}

			if( m_offsetEnabled )
			{
				MasterNode.AddRenderState( ref result, "Offset ", m_offsetFactor.GetValueOrProperty() + " , " + m_offsetUnits.GetValueOrProperty() );
			}

			return result;
		}

		public void Draw( UndoParentNode owner, GUIStyle toolbarstyle, bool customBlendAvailable )
		{
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, 0.5f );
			EditorGUILayout.BeginHorizontal( toolbarstyle );
			GUI.color = cachedColor;
			EditorGUI.BeginChangeCheck();
			m_parentSurface.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedDepth = owner.GUILayoutToggle( m_parentSurface.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedDepth, DepthParametersStr, UIUtils.MenuItemToggleStyle );
			if( EditorGUI.EndChangeCheck() )
			{
				EditorPrefs.SetBool( "ExpandedDepth", m_parentSurface.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedDepth );
			}
			EditorGUILayout.EndHorizontal();

			if( m_parentSurface.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedDepth )
			{
				cachedColor = GUI.color;
				GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
				EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
				GUI.color = cachedColor;

				EditorGUI.indentLevel++;
				if( !customBlendAvailable )
					EditorGUILayout.HelpBox( "Depth Writing is only available for Opaque or Custom blend modes", MessageType.Warning );

				EditorGUILayout.Separator();
				EditorGUI.BeginDisabledGroup( !customBlendAvailable );

				m_zWriteMode.EnumTypePopup( ref owner, ZWriteModeStr, ZWriteModeValues );
				m_zTestMode.EnumTypePopup( ref owner, ZTestModeStr, ZTestModeLabels );
				//m_zWriteMode = owner.EditorGUILayoutPopup( ZWriteModeStr, m_zWriteMode, ZWriteModeValues );
				//m_zTestMode = owner.EditorGUILayoutPopup( ZTestModeStr, m_zTestMode, ZTestModeLabels );
				m_offsetEnabled = owner.EditorGUILayoutToggle( OffsetStr, m_offsetEnabled );
				if( m_offsetEnabled )
				{
					EditorGUI.indentLevel++;
					m_offsetFactor.FloatField( ref owner , OffsetFactorStr );
					m_offsetUnits.FloatField( ref owner , OffsetUnitsStr );
					EditorGUI.indentLevel--;
				}

				m_extraDepthPass = owner.EditorGUILayoutToggle( ExtraDepthPassStr, m_extraDepthPass );
				if( m_extraDepthPass )
				{
					EditorGUI.indentLevel++;
					m_extrazTestMode = owner.EditorGUILayoutPopup( DepthZTestStr, m_extrazTestMode, ZTestModeLabels );
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.Separator();
				EditorGUI.indentLevel--;
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndVertical();
			}

			EditorGUI.EndDisabledGroup();
		}

		public void DrawExtraDepthPass( ref string shaderBody )
		{
			if( m_extraDepthPass )
			{
				shaderBody += "\t\tPass\n";
				shaderBody += "\t\t{\n";
				shaderBody += "\t\t\tColorMask 0\n";
				if( m_extrazTestMode != 0 )
					shaderBody += "\t\t\tZTest " + ZTestModeValues[ m_extrazTestMode ] + "\n";
				shaderBody += "\t\t\tZWrite On\n";
				shaderBody += "\t\t}\n\n";
			}
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			if( UIUtils.CurrentShaderVersion() < 2502 )
			{
				string zWriteMode = nodeParams[ index++ ];
				m_zWriteMode.IntValue = zWriteMode.Equals( "Off" ) ? 2 : 0;

				string zTestMode = nodeParams[ index++ ];
				for( int i = 0; i < ZTestModeValues.Length; i++ )
				{
					if( zTestMode.Equals( ZTestModeValues[ i ] ) )
					{
						m_zTestMode.IntValue = i;
						break;
					}
				}
			}
			else
			{
				if( UIUtils.CurrentShaderVersion() > 14501 )
				{
					m_zWriteMode.ReadFromString( ref index, ref nodeParams );
					m_zTestMode.ReadFromString( ref index, ref nodeParams );
				}
				else
				{
					m_zWriteMode.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_zTestMode.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
				}
				m_offsetEnabled = Convert.ToBoolean( nodeParams[ index++ ] );

				if( UIUtils.CurrentShaderVersion() > 15303 )
				{
					m_offsetFactor.ReadFromString( ref index, ref nodeParams );
					m_offsetUnits.ReadFromString( ref index, ref nodeParams );
				}
				else
				{
					m_offsetFactor.FloatValue = Convert.ToSingle( nodeParams[ index++ ] );
					m_offsetUnits.FloatValue = Convert.ToSingle( nodeParams[ index++ ] );
				}

				if( UIUtils.CurrentShaderVersion() > 14202 )
				{
					m_extraDepthPass = Convert.ToBoolean( nodeParams[ index++ ] );
					m_extrazTestMode = Convert.ToInt32( nodeParams[ index++ ] );
				}
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			m_zWriteMode.WriteToString( ref nodeInfo );
			m_zTestMode.WriteToString( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_offsetEnabled );
			m_offsetFactor.WriteToString( ref nodeInfo );
			m_offsetUnits.WriteToString( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_extraDepthPass );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_extrazTestMode );
		}
		public bool IsActive { get { return m_zTestMode.IntValue != 0 || m_zWriteMode.IntValue != 0 || m_offsetEnabled || m_zTestMode.Active || m_zWriteMode.Active; } }
		public StandardSurfaceOutputNode ParentSurface { get { return m_parentSurface; } set { m_parentSurface = value; } }
	}
}
