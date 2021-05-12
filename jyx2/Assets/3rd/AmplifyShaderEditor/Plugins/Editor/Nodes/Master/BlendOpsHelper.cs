// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public enum AvailableBlendFactor
	{
		One = 1,
		Zero = 0,
		SrcColor = 3,
		SrcAlpha = 5,
		DstColor = 2,
		DstAlpha = 7,
		OneMinusSrcColor = 6,
		OneMinusSrcAlpha = 10,
		OneMinusDstColor = 4,
		OneMinusDstAlpha = 8,
		SrcAlphaSaturate = 9
	};

	public enum AvailableBlendOps
	{
		OFF = 0,
		Add,
		Sub,
		RevSub,
		Min,
		Max,
		//Direct X11 only
		LogicalClear,
		LogicalSet,
		LogicalCopy,
		LogicalCopyInverted,
		LogicalNoop,
		LogicalInvert,
		LogicalAnd,
		LogicalNand,
		LogicalOr,
		LogicalNor,
		LogicalXor,
		LogicalEquiv,
		LogicalAndReverse,
		LogicalAndInverted,
		LogicalOrReverse,
		LogicalOrInverted
	};

	public class CommonBlendTypes
	{
		public string Name;
		public AvailableBlendFactor SourceFactor;
		public AvailableBlendFactor DestFactor;
		public CommonBlendTypes( string name, AvailableBlendFactor sourceFactor, AvailableBlendFactor destFactor )
		{
			Name = name;
			SourceFactor = sourceFactor;
			DestFactor = destFactor;
		}
	}

	[Serializable]
	public class BlendOpsHelper
	{
		public static readonly string[] BlendOpsLabels =
		{
			"<OFF>",
			"Add",
			"Sub",
			"RevSub",
			"Min",
			"Max",
			"LogicalClear ( DX11.1 Only )",
			"LogicalSet ( DX11.1 Only )",
			"LogicalCopy ( DX11.1 Only )",
			"LogicalCopyInverted ( DX11.1 Only )",
			"LogicalNoop ( DX11.1 Only )",
			"LogicalInvert ( DX11.1 Only )",
			"LogicalAnd ( DX11.1 Only )",
			"LogicalNand ( DX11.1 Only )",
			"LogicalOr ( DX11.1 Only )",
			"LogicalNor ( DX11.1 Only )",
			"LogicalXor ( DX11.1 Only )",
			"LogicalEquiv ( DX11.1 Only )",
			"LogicalAndReverse ( DX11.1 Only )",
			"LogicalAndInverted ( DX11.1 Only )",
			"LogicalOrReverse ( DX11.1 Only )",
			"LogicalOrInverted ( DX11.1 Only )"
		};

		private const string BlendModesRGBStr = "Blend RGB";
		private const string BlendModesAlphaStr = "Blend Alpha";

		private const string BlendOpsRGBStr = "Blend Op RGB";
		private const string BlendOpsAlphaStr = "Blend Op Alpha";

		private const string SourceFactorStr = "Src";
		private const string DstFactorStr = "Dst";

		private const string SingleBlendFactorStr = "Blend {0} {1}";
		private const string SeparateBlendFactorStr = "Blend {0} {1} , {2} {3}";

		private const string SingleBlendOpStr = "BlendOp {0}";
		private const string SeparateBlendOpStr = "BlendOp {0} , {1}";

		private string[] m_commonBlendTypesArr;
		private List<CommonBlendTypes> m_commonBlendTypes = new List<CommonBlendTypes> {    new CommonBlendTypes("<OFF>",               AvailableBlendFactor.Zero,              AvailableBlendFactor.Zero ),
																							new CommonBlendTypes("Custom",              AvailableBlendFactor.Zero,              AvailableBlendFactor.Zero ) ,
																							new CommonBlendTypes("Alpha Blend",         AvailableBlendFactor.SrcAlpha,          AvailableBlendFactor.OneMinusSrcAlpha ) ,
																							new CommonBlendTypes("Premultiplied",       AvailableBlendFactor.One,               AvailableBlendFactor.OneMinusSrcAlpha ),
																							new CommonBlendTypes("Additive",            AvailableBlendFactor.One,               AvailableBlendFactor.One ),
																							new CommonBlendTypes("Soft Additive",       AvailableBlendFactor.OneMinusDstColor,  AvailableBlendFactor.One ),
																							new CommonBlendTypes("Multiplicative",      AvailableBlendFactor.DstColor,          AvailableBlendFactor.Zero ),
																							new CommonBlendTypes("2x Multiplicative",   AvailableBlendFactor.DstColor,          AvailableBlendFactor.SrcColor ),
																							new CommonBlendTypes("Particle Additive",   AvailableBlendFactor.SrcAlpha,          AvailableBlendFactor.One ),};

		[SerializeField]
		private bool m_enabled = false;

		// Blend Factor
		// RGB
		[SerializeField]
		private int m_currentIndex = 0;


		[SerializeField]
		private InlineProperty m_sourceFactorRGB = new InlineProperty( 0 );

		[SerializeField]
		private InlineProperty m_destFactorRGB = new InlineProperty( 0 );

		// Alpha
		[SerializeField]
		private int m_currentAlphaIndex = 0;

		[SerializeField]
		private InlineProperty m_sourceFactorAlpha = new InlineProperty( 0 );

		[SerializeField]
		private InlineProperty m_destFactorAlpha = new InlineProperty( 0 );

		//Blend Ops
		[SerializeField]
		private bool m_blendOpEnabled = false;

		[SerializeField]
		private InlineProperty m_blendOpRGB = new InlineProperty( 0 );

		[SerializeField]
		private InlineProperty m_blendOpAlpha = new InlineProperty( 0 );

		public BlendOpsHelper()
		{
			m_commonBlendTypesArr = new string[ m_commonBlendTypes.Count ];
			for( int i = 0; i < m_commonBlendTypesArr.Length; i++ )
			{
				m_commonBlendTypesArr[ i ] = m_commonBlendTypes[ i ].Name;
			}
		}

		public void Draw( UndoParentNode owner, bool customBlendAvailable )
		{
			m_enabled = customBlendAvailable;

			// RGB
			EditorGUI.BeginChangeCheck();
			m_currentIndex = owner.EditorGUILayoutPopup( BlendModesRGBStr, m_currentIndex, m_commonBlendTypesArr );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_currentIndex > 1 )
				{
					m_sourceFactorRGB.IntValue = (int)m_commonBlendTypes[ m_currentIndex ].SourceFactor;
					m_sourceFactorRGB.SetInlineNodeValue();

					m_destFactorRGB.IntValue = (int)m_commonBlendTypes[ m_currentIndex ].DestFactor;
					m_destFactorRGB.SetInlineNodeValue();
				}
			}
			EditorGUI.BeginDisabledGroup( m_currentIndex == 0 );

			EditorGUI.BeginChangeCheck();
			float cached = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 40;

			EditorGUILayout.BeginHorizontal();
			AvailableBlendFactor tempCast = (AvailableBlendFactor)m_sourceFactorRGB.IntValue;
			m_sourceFactorRGB.CustomDrawer( ref owner, ( x ) => { tempCast = (AvailableBlendFactor)x.EditorGUILayoutEnumPopup( SourceFactorStr, tempCast ); }, SourceFactorStr );
			m_sourceFactorRGB.IntValue = (int)tempCast;
			EditorGUI.indentLevel--;
			EditorGUIUtility.labelWidth = 25;
			tempCast = (AvailableBlendFactor)m_destFactorRGB.IntValue;
			m_destFactorRGB.CustomDrawer( ref owner, ( x ) => { tempCast = (AvailableBlendFactor)x.EditorGUILayoutEnumPopup( DstFactorStr, tempCast ); }, DstFactorStr );
			m_destFactorRGB.IntValue = (int)tempCast;
			EditorGUI.indentLevel++;
			EditorGUILayout.EndHorizontal();

			EditorGUIUtility.labelWidth = cached;
			if( EditorGUI.EndChangeCheck() )
			{
				CheckRGBIndex();
			}

			// Both these tests should be removed on a later stage
			// ASE v154dev004 changed AvailableBlendOps.OFF value from -1 to 0
			// If importing the new package into an already opened ASE window makes 
			// hotcode to preserve the -1 value on these variables
			if( m_blendOpRGB.FloatValue < 0 )
				m_blendOpRGB.FloatValue = 0;

			if( m_blendOpAlpha.FloatValue < 0 )
				m_blendOpAlpha.FloatValue = 0;

			EditorGUI.BeginChangeCheck();
			//AvailableBlendOps tempOpCast = (AvailableBlendOps)m_blendOpRGB.IntValue;
			m_blendOpRGB.CustomDrawer( ref owner, ( x ) => { m_blendOpRGB.IntValue = x.EditorGUILayoutPopup( BlendOpsRGBStr, m_blendOpRGB.IntValue, BlendOpsLabels ); }, BlendOpsRGBStr );
			//m_blendOpRGB.IntValue = (int)tempOpCast;
			if( EditorGUI.EndChangeCheck() )
			{
				m_blendOpEnabled = ( !m_blendOpRGB.Active && m_blendOpRGB.IntValue > -1 ) || ( m_blendOpRGB.Active && m_blendOpRGB.NodeId > -1 );//AvailableBlendOps.OFF;
				m_blendOpRGB.SetInlineNodeValue();
			}

			EditorGUI.EndDisabledGroup();

			// Alpha
			EditorGUILayout.Separator();

			EditorGUI.BeginChangeCheck();
			m_currentAlphaIndex = owner.EditorGUILayoutPopup( BlendModesAlphaStr, m_currentAlphaIndex, m_commonBlendTypesArr );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_currentAlphaIndex > 0 )
				{
					m_sourceFactorAlpha.IntValue = (int)m_commonBlendTypes[ m_currentAlphaIndex ].SourceFactor;
					m_sourceFactorAlpha.SetInlineNodeValue();

					m_destFactorAlpha.IntValue = (int)m_commonBlendTypes[ m_currentAlphaIndex ].DestFactor;
					m_destFactorAlpha.SetInlineNodeValue();
				}
			}
			EditorGUI.BeginDisabledGroup( m_currentAlphaIndex == 0 );

			EditorGUI.BeginChangeCheck();
			cached = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 40;
			EditorGUILayout.BeginHorizontal();
			tempCast = (AvailableBlendFactor)m_sourceFactorAlpha.IntValue;
			m_sourceFactorAlpha.CustomDrawer( ref owner, ( x ) => { tempCast = (AvailableBlendFactor)x.EditorGUILayoutEnumPopup( SourceFactorStr, tempCast ); }, SourceFactorStr );
			m_sourceFactorAlpha.IntValue = (int)tempCast;
			EditorGUI.indentLevel--;
			EditorGUIUtility.labelWidth = 25;
			tempCast = (AvailableBlendFactor)m_destFactorAlpha.IntValue;
			m_destFactorAlpha.CustomDrawer( ref owner, ( x ) => { tempCast = (AvailableBlendFactor)x.EditorGUILayoutEnumPopup( DstFactorStr, tempCast ); }, DstFactorStr );
			m_destFactorAlpha.IntValue = (int)tempCast;
			EditorGUI.indentLevel++;
			EditorGUILayout.EndHorizontal();
			EditorGUIUtility.labelWidth = cached;

			if( EditorGUI.EndChangeCheck() )
			{
				CheckAlphaIndex();
			}
			EditorGUI.BeginChangeCheck();
			//tempOpCast = (AvailableBlendOps)m_blendOpAlpha.IntValue;
			m_blendOpAlpha.CustomDrawer( ref owner, ( x ) => { m_blendOpAlpha.IntValue = x.EditorGUILayoutPopup( BlendOpsAlphaStr, m_blendOpAlpha.IntValue, BlendOpsLabels ); }, BlendOpsAlphaStr );
			//m_blendOpAlpha.IntValue = (int)tempOpCast;
			if( EditorGUI.EndChangeCheck() )
			{
				m_blendOpAlpha.SetInlineNodeValue();
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.Separator();
		}

		void CheckRGBIndex()
		{
			int count = m_commonBlendTypes.Count;
			m_currentIndex = 1;
			for( int i = 1; i < count; i++ )
			{
				if( m_commonBlendTypes[ i ].SourceFactor == (AvailableBlendFactor)m_sourceFactorRGB.IntValue && m_commonBlendTypes[ i ].DestFactor == (AvailableBlendFactor)m_destFactorRGB.IntValue )
				{
					m_currentIndex = i;
					return;
				}
			}

		}

		void CheckAlphaIndex()
		{
			int count = m_commonBlendTypes.Count;
			m_currentAlphaIndex = 1;
			for( int i = 1; i < count; i++ )
			{
				if( m_commonBlendTypes[ i ].SourceFactor == (AvailableBlendFactor)m_sourceFactorAlpha.IntValue && m_commonBlendTypes[ i ].DestFactor == (AvailableBlendFactor)m_destFactorAlpha.IntValue )
				{
					m_currentAlphaIndex = i;
					if( m_currentAlphaIndex > 0 && m_currentIndex == 0 )
						m_currentIndex = 1;
					return;
				}
			}

			if( m_currentAlphaIndex > 0 && m_currentIndex == 0 )
				m_currentIndex = 1;
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_currentIndex = Convert.ToInt32( nodeParams[ index++ ] );
			if( UIUtils.CurrentShaderVersion() > 15103 )
			{
				m_sourceFactorRGB.ReadFromString( ref index, ref nodeParams );
				m_destFactorRGB.ReadFromString( ref index, ref nodeParams );
			}
			else
			{
				m_sourceFactorRGB.IntValue = (int)(AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), nodeParams[ index++ ] );
				m_destFactorRGB.IntValue = (int)(AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), nodeParams[ index++ ] );
			}

			m_currentAlphaIndex = Convert.ToInt32( nodeParams[ index++ ] );
			if( UIUtils.CurrentShaderVersion() > 15103 )
			{
				m_sourceFactorAlpha.ReadFromString( ref index, ref nodeParams );
				m_destFactorAlpha.ReadFromString( ref index, ref nodeParams );

				m_blendOpRGB.ReadFromString( ref index, ref nodeParams );
				m_blendOpAlpha.ReadFromString( ref index, ref nodeParams );
				if( UIUtils.CurrentShaderVersion() < 15404 )
				{
					// Now BlendOps enum starts at 0 and not -1
					m_blendOpRGB.FloatValue += 1;
					m_blendOpAlpha.FloatValue += 1;
				}
			}
			else
			{
				m_sourceFactorAlpha.IntValue = (int)(AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), nodeParams[ index++ ] );
				m_destFactorAlpha.IntValue = (int)(AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), nodeParams[ index++ ] );
				m_blendOpRGB.IntValue = (int)(AvailableBlendOps)Enum.Parse( typeof( AvailableBlendOps ), nodeParams[ index++ ] );
				m_blendOpAlpha.IntValue = (int)(AvailableBlendOps)Enum.Parse( typeof( AvailableBlendOps ), nodeParams[ index++ ] );
			}

			m_enabled = ( m_currentIndex > 0 || m_currentAlphaIndex > 0 );
			m_blendOpEnabled = ( !m_blendOpRGB.Active && m_blendOpRGB.IntValue > -1 ) || ( m_blendOpRGB.Active && m_blendOpRGB.NodeId > -1 );
		}


		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentIndex );
			m_sourceFactorRGB.WriteToString( ref nodeInfo );
			m_destFactorRGB.WriteToString( ref nodeInfo );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentAlphaIndex );
			m_sourceFactorAlpha.WriteToString( ref nodeInfo );
			m_destFactorAlpha.WriteToString( ref nodeInfo );

			m_blendOpRGB.WriteToString( ref nodeInfo );
			m_blendOpAlpha.WriteToString( ref nodeInfo );
		}

		public void SetBlendOpsFromBlendMode( AlphaMode mode, bool customBlendAvailable )
		{
			switch( mode )
			{
				case AlphaMode.Transparent:
				m_currentIndex = 2;
				m_sourceFactorRGB.IntValue = (int)m_commonBlendTypes[ m_currentIndex ].SourceFactor;
				m_destFactorRGB.IntValue = (int)m_commonBlendTypes[ m_currentIndex ].DestFactor;
				break;
				case AlphaMode.Masked:
				case AlphaMode.Translucent:
				m_currentIndex = 0;
				break;
				case AlphaMode.Premultiply:
				m_currentIndex = 3;
				m_sourceFactorRGB.IntValue = (int)m_commonBlendTypes[ m_currentIndex ].SourceFactor;
				m_destFactorRGB.IntValue = (int)m_commonBlendTypes[ m_currentIndex ].DestFactor;
				break;
			}
			m_enabled = customBlendAvailable;
		}

		public string CreateBlendOps()
		{

			string result = "\t\t" + CurrentBlendFactor + "\n";
			if( m_blendOpEnabled )
			{
				result += "\t\t" + CurrentBlendOp + "\n";
			}
			return result;
		}

		public string CurrentBlendRGB { get { return m_commonBlendTypes[ m_currentIndex ].Name; } }

		public string CurrentBlendFactorSingle { get { return string.Format( SingleBlendFactorStr, m_sourceFactorRGB.GetValueOrProperty( ( (AvailableBlendFactor)m_sourceFactorRGB.IntValue ).ToString() ), m_destFactorRGB.GetValueOrProperty( ( (AvailableBlendFactor)m_destFactorRGB.IntValue ).ToString() ) ); } }
		//public string CurrentBlendFactorSingleAlpha { get { return string.Format(SeparateBlendFactorStr, m_sourceFactorRGB, m_destFactorRGB, m_sourceFactorAlpha, m_destFactorAlpha); } }
		public string CurrentBlendFactorSeparate
		{
			get
			{
				string src = ( m_currentIndex > 0 ? m_sourceFactorRGB.GetValueOrProperty( ( (AvailableBlendFactor)m_sourceFactorRGB.IntValue ).ToString() ) : AvailableBlendFactor.One.ToString() );
				string dst = ( m_currentIndex > 0 ? m_destFactorRGB.GetValueOrProperty( ( (AvailableBlendFactor)m_destFactorRGB.IntValue ).ToString() ) : AvailableBlendFactor.Zero.ToString() );
				string srca = m_sourceFactorAlpha.GetValueOrProperty( ( (AvailableBlendFactor)m_sourceFactorAlpha.IntValue ).ToString() );
				string dsta = m_destFactorAlpha.GetValueOrProperty( ( (AvailableBlendFactor)m_destFactorAlpha.IntValue ).ToString() );
				return string.Format( SeparateBlendFactorStr, src, dst, srca, dsta );
			}
		}
		public string CurrentBlendFactor { get { return ( ( m_currentAlphaIndex > 0 ) ? CurrentBlendFactorSeparate : CurrentBlendFactorSingle ); } }

		public string CurrentBlendOpSingle
		{
			get
			{
				string value = m_blendOpRGB.GetValueOrProperty( ( (AvailableBlendOps)m_blendOpRGB.IntValue ).ToString() );
				if( value.Equals( ( AvailableBlendOps.OFF ).ToString() ) )
					return string.Empty;
				
				return string.Format( SingleBlendOpStr, value );
			}
		}
		public string CurrentBlendOpSeparate
		{
			get
			{
				string rgbValue = m_blendOpRGB.GetValueOrProperty( ( (AvailableBlendOps)m_blendOpRGB.IntValue ).ToString() );

				if( rgbValue.Equals( ( AvailableBlendOps.OFF ).ToString() ))
					rgbValue = "Add";

				string alphaValue = m_blendOpAlpha.GetValueOrProperty( ( (AvailableBlendOps)m_blendOpAlpha.IntValue ).ToString() );
				return string.Format( SeparateBlendOpStr, ( m_currentIndex > 0 ? rgbValue : AvailableBlendOps.Add.ToString() ), alphaValue );
			}
		}
		public string CurrentBlendOp { get { return ( ( m_currentAlphaIndex > 0 && m_blendOpAlpha.GetValueOrProperty( ( (AvailableBlendOps)m_blendOpAlpha.IntValue ).ToString() ) != AvailableBlendOps.OFF.ToString() ) ? CurrentBlendOpSeparate : CurrentBlendOpSingle ); } }

		public bool Active { get { return m_enabled && ( m_currentIndex > 0 || m_currentAlphaIndex > 0 ); } }
	}
}
