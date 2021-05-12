using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{

	[Serializable]
	public class StencilBufferOpHelper
	{
		public static readonly string[] StencilComparisonValues =
		{
			"<Default>",
			"Greater" ,
			"GEqual" ,
			"Less" ,
			"LEqual" ,
			"Equal" ,
			"NotEqual" ,
			"Always" ,
			"Never"
		};

		public static readonly string[] StencilComparisonLabels =
		{
			"<Default>",
			"Greater" ,
			"Greater or Equal" ,
			"Less" ,
			"Less or Equal" ,
			"Equal" ,
			"Not Equal" ,
			"Always" ,
			"Never"
		};


		public static readonly string[] StencilOpsValues =
		{
			"<Default>",
			"Keep",
			"Zero",
			"Replace",
			"IncrSat",
			"DecrSat",
			"Invert",
			"IncrWrap",
			"DecrWrap"
		};

		public static readonly string[] StencilOpsLabels =
		{
			"<Default>",
			"Keep",
			"Zero",
			"Replace",
			"IncrSat",
			"DecrSat",
			"Invert",
			"IncrWrap",
			"DecrWrap"
		};


		private const string FoldoutLabelStr = " Stencil Buffer";
		private GUIContent ReferenceValueContent = new GUIContent( "Reference", "The value to be compared against (if Comparison is anything else than always) and/or the value to be written to the buffer (if either Pass, Fail or ZFail is set to replace)" );
		private GUIContent ReadMaskContent = new GUIContent( "Read Mask", "An 8 bit mask as an 0-255 integer, used when comparing the reference value with the contents of the buffer (referenceValue & readMask) comparisonFunction (stencilBufferValue & readMask)" );
		private GUIContent WriteMaskContent = new GUIContent( "Write Mask", "An 8 bit mask as an 0-255 integer, used when writing to the buffer" );
		private const string ComparisonStr = "Comparison";
		private const string PassStr = "Pass";
		private const string FailStr = "Fail";
		private const string ZFailStr = "ZFail";

		private const string ComparisonFrontStr = "Comp. Front";
		private const string PassFrontStr = "Pass Front";
		private const string FailFrontStr = "Fail Front";
		private const string ZFailFrontStr = "ZFail Front";

		private const string ComparisonBackStr = "Comp. Back";
		private const string PassBackStr = "Pass Back";
		private const string FailBackStr = "Fail Back";
		private const string ZFailBackStr = "ZFail Back";

		private const int ReadMaskDefaultValue = 255;
		private const int WriteMaskDefaultValue = 255;
		private const int ComparisonDefaultValue = 0;
		private const int PassStencilOpDefaultValue = 0;
		private const int FailStencilOpDefaultValue = 0;
		private const int ZFailStencilOpDefaultValue = 0;

		[SerializeField]
		private bool m_active;

		[SerializeField]
		private InlineProperty m_refValue = new InlineProperty();
		[SerializeField]
		private InlineProperty m_readMask = new InlineProperty( ReadMaskDefaultValue );
		[SerializeField]
		private InlineProperty m_writeMask = new InlineProperty( WriteMaskDefaultValue );

		//Comparison Function
		[SerializeField]
		private InlineProperty m_comparisonFunctionIdx = new InlineProperty( ComparisonDefaultValue );
		[SerializeField]
		private InlineProperty m_comparisonFunctionBackIdx = new InlineProperty( ComparisonDefaultValue );

		//Pass Stencil Op
		[SerializeField]
		private InlineProperty m_passStencilOpIdx = new InlineProperty( PassStencilOpDefaultValue );
		[SerializeField]
		private InlineProperty m_passStencilOpBackIdx = new InlineProperty( PassStencilOpDefaultValue );

		//Fail Stencil Op 
		[SerializeField]
		private InlineProperty m_failStencilOpIdx = new InlineProperty( FailStencilOpDefaultValue );
		[SerializeField]
		private InlineProperty m_failStencilOpBackIdx = new InlineProperty( FailStencilOpDefaultValue );

		//ZFail Stencil Op
		[SerializeField]
		private InlineProperty m_zFailStencilOpIdx = new InlineProperty( ZFailStencilOpDefaultValue );
		[SerializeField]
		private InlineProperty m_zFailStencilOpBackIdx = new InlineProperty( ZFailStencilOpDefaultValue );

		public string CreateStencilOp( UndoParentNode owner )
		{
			string result = "\t\tStencil\n\t\t{\n";
			result += string.Format( "\t\t\tRef {0}\n", m_refValue.GetValueOrProperty() );
			if( m_readMask.Active || m_readMask.IntValue != ReadMaskDefaultValue )
			{
				result += string.Format( "\t\t\tReadMask {0}\n", m_readMask.GetValueOrProperty() );
			}

			if( m_writeMask.Active || m_writeMask.IntValue != WriteMaskDefaultValue )
			{
				result += string.Format( "\t\t\tWriteMask {0}\n", m_writeMask.GetValueOrProperty() );
			}

			if( ( owner as StandardSurfaceOutputNode ).CurrentCullMode == CullMode.Off )
			{
				if( m_comparisonFunctionIdx.IntValue != ComparisonDefaultValue || m_comparisonFunctionIdx.Active )
					result += string.Format( "\t\t\tCompFront {0}\n", m_comparisonFunctionIdx.GetValueOrProperty( StencilComparisonValues[ m_comparisonFunctionIdx.IntValue ] ) );
				if( m_passStencilOpIdx.IntValue != PassStencilOpDefaultValue || m_passStencilOpIdx.Active )
					result += string.Format( "\t\t\tPassFront {0}\n", m_passStencilOpIdx.GetValueOrProperty( StencilOpsValues[ m_passStencilOpIdx.IntValue ] ) );
				if( m_failStencilOpIdx.IntValue != FailStencilOpDefaultValue || m_failStencilOpIdx.Active )
					result += string.Format( "\t\t\tFailFront {0}\n", m_failStencilOpIdx.GetValueOrProperty( StencilOpsValues[ m_failStencilOpIdx.IntValue ] ) );
				if( m_zFailStencilOpIdx.IntValue != ZFailStencilOpDefaultValue || m_zFailStencilOpIdx.Active )
					result += string.Format( "\t\t\tZFailFront {0}\n", m_zFailStencilOpIdx.GetValueOrProperty( StencilOpsValues[ m_zFailStencilOpIdx.IntValue ] ) );

				if( m_comparisonFunctionBackIdx.IntValue != ComparisonDefaultValue || m_comparisonFunctionBackIdx.Active )
					result += string.Format( "\t\t\tCompBack {0}\n", m_comparisonFunctionBackIdx.GetValueOrProperty( StencilComparisonValues[ m_comparisonFunctionBackIdx.IntValue ] ) );
				if( m_passStencilOpBackIdx.IntValue != PassStencilOpDefaultValue || m_passStencilOpBackIdx.Active )
					result += string.Format( "\t\t\tPassBack {0}\n", m_passStencilOpBackIdx.GetValueOrProperty( StencilOpsValues[ m_passStencilOpBackIdx.IntValue ] ) );
				if( m_failStencilOpBackIdx.IntValue != FailStencilOpDefaultValue || m_failStencilOpBackIdx.Active )
					result += string.Format( "\t\t\tFailBack {0}\n", m_failStencilOpBackIdx.GetValueOrProperty( StencilOpsValues[ m_failStencilOpBackIdx.IntValue ] ) );
				if( m_zFailStencilOpBackIdx.IntValue != ZFailStencilOpDefaultValue || m_zFailStencilOpBackIdx.Active )
					result += string.Format( "\t\t\tZFailBack {0}\n", m_zFailStencilOpBackIdx.GetValueOrProperty( StencilOpsValues[ m_zFailStencilOpBackIdx.IntValue ] ) );
			}
			else
			{
				if( m_comparisonFunctionIdx.IntValue != ComparisonDefaultValue || m_comparisonFunctionIdx.Active )
					result += string.Format( "\t\t\tComp {0}\n", m_comparisonFunctionIdx.GetValueOrProperty( StencilComparisonValues[ m_comparisonFunctionIdx.IntValue ] ) );
				if( m_passStencilOpIdx.IntValue != PassStencilOpDefaultValue || m_passStencilOpIdx.Active )
					result += string.Format( "\t\t\tPass {0}\n", m_passStencilOpIdx.GetValueOrProperty( StencilOpsValues[ m_passStencilOpIdx.IntValue ] ) );
				if( m_failStencilOpIdx.IntValue != FailStencilOpDefaultValue || m_failStencilOpIdx.Active )
					result += string.Format( "\t\t\tFail {0}\n", m_failStencilOpIdx.GetValueOrProperty( StencilOpsValues[ m_failStencilOpIdx.IntValue ] ) );
				if( m_zFailStencilOpIdx.IntValue != ZFailStencilOpDefaultValue || m_zFailStencilOpIdx.Active )
					result += string.Format( "\t\t\tZFail {0}\n", m_zFailStencilOpIdx.GetValueOrProperty( StencilOpsValues[ m_zFailStencilOpIdx.IntValue ] ) );
			}


			result += "\t\t}\n";
			return result;
		}

		public void Draw( UndoParentNode owner )
		{
			bool foldoutValue = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedStencilOptions;
			NodeUtils.DrawPropertyGroup( owner, ref foldoutValue, ref m_active, FoldoutLabelStr, () =>
			{
				float cache = EditorGUIUtility.labelWidth;
				float cache2 = EditorGUIUtility.fieldWidth;
				EditorGUIUtility.labelWidth = 110;
				EditorGUIUtility.fieldWidth = 30;
				m_refValue.IntSlider( ref owner, ReferenceValueContent, 0, 255 );
				m_readMask.IntSlider( ref owner, ReadMaskContent, 0, 255 );
				m_writeMask.IntSlider( ref owner, WriteMaskContent, 0, 255 );
				//EditorGUIUtility.labelWidth = cache;
				EditorGUIUtility.fieldWidth = cache2;
				if( ( owner as StandardSurfaceOutputNode ).CurrentCullMode == CullMode.Off )
				{
					m_comparisonFunctionIdx.EnumTypePopup( ref owner, ComparisonFrontStr, StencilComparisonLabels );
					m_passStencilOpIdx.EnumTypePopup( ref owner, PassFrontStr, StencilOpsLabels );
					m_failStencilOpIdx.EnumTypePopup( ref owner, FailFrontStr, StencilOpsLabels );
					m_zFailStencilOpIdx.EnumTypePopup( ref owner, ZFailFrontStr, StencilOpsLabels );
					EditorGUILayout.Separator();
					m_comparisonFunctionBackIdx.EnumTypePopup( ref owner, ComparisonBackStr, StencilComparisonLabels );
					m_passStencilOpBackIdx.EnumTypePopup( ref owner, PassBackStr, StencilOpsLabels );
					m_failStencilOpBackIdx.EnumTypePopup( ref owner, FailBackStr, StencilOpsLabels );
					m_zFailStencilOpBackIdx.EnumTypePopup( ref owner, ZFailBackStr, StencilOpsLabels );
				}
				else
				{
					m_comparisonFunctionIdx.EnumTypePopup( ref owner, ComparisonStr, StencilComparisonLabels );
					m_passStencilOpIdx.EnumTypePopup( ref owner, PassFrontStr, StencilOpsLabels );
					m_failStencilOpIdx.EnumTypePopup( ref owner, FailFrontStr, StencilOpsLabels );
					m_zFailStencilOpIdx.EnumTypePopup( ref owner, ZFailFrontStr, StencilOpsLabels );
				}
				EditorGUIUtility.labelWidth = cache;
			} );
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedStencilOptions = foldoutValue;
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_active = Convert.ToBoolean( nodeParams[ index++ ] );
			if( UIUtils.CurrentShaderVersion() > 14501 )
			{
				m_refValue.ReadFromString( ref index, ref nodeParams );
				m_readMask.ReadFromString( ref index, ref nodeParams );
				m_writeMask.ReadFromString( ref index, ref nodeParams );
				m_comparisonFunctionIdx.ReadFromString( ref index, ref nodeParams );
				m_passStencilOpIdx.ReadFromString( ref index, ref nodeParams );
				m_failStencilOpIdx.ReadFromString( ref index, ref nodeParams );
				m_zFailStencilOpIdx.ReadFromString( ref index, ref nodeParams );
			}
			else
			{
				m_refValue.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
				m_readMask.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
				m_writeMask.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
				m_comparisonFunctionIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
				m_passStencilOpIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
				m_failStencilOpIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
				m_zFailStencilOpIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
			}

			if( UIUtils.CurrentShaderVersion() > 13203 )
			{
				if( UIUtils.CurrentShaderVersion() > 14501 )
				{
					m_comparisonFunctionBackIdx.ReadFromString( ref index, ref nodeParams );
					m_passStencilOpBackIdx.ReadFromString( ref index, ref nodeParams );
					m_failStencilOpBackIdx.ReadFromString( ref index, ref nodeParams );
					m_zFailStencilOpBackIdx.ReadFromString( ref index, ref nodeParams );
				}
				else
				{
					m_comparisonFunctionBackIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_passStencilOpBackIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_failStencilOpBackIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_zFailStencilOpBackIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
				}
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_active );
			m_refValue.WriteToString( ref nodeInfo );
			m_readMask.WriteToString( ref nodeInfo );
			m_writeMask.WriteToString( ref nodeInfo );
			m_comparisonFunctionIdx.WriteToString( ref nodeInfo );
			m_passStencilOpIdx.WriteToString( ref nodeInfo );
			m_failStencilOpIdx.WriteToString( ref nodeInfo );
			m_zFailStencilOpIdx.WriteToString( ref nodeInfo );
			m_comparisonFunctionBackIdx.WriteToString( ref nodeInfo );
			m_passStencilOpBackIdx.WriteToString( ref nodeInfo );
			m_failStencilOpBackIdx.WriteToString( ref nodeInfo );
			m_zFailStencilOpBackIdx.WriteToString( ref nodeInfo );
		}

		public bool Active
		{
			get { return m_active; }
			set { m_active = value; }
		}
	}
}
