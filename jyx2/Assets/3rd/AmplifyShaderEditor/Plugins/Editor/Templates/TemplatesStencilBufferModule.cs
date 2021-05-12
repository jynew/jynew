using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{

    [Serializable]
    public sealed class TemplatesStencilBufferModule : TemplateModuleParent
    {
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

        private Dictionary<string, int> m_comparisonDict = new Dictionary<string, int>();
        private Dictionary<string, int> m_stencilOpsDict = new Dictionary<string, int>();

		[SerializeField]
		private bool m_active = true;

        [SerializeField]
        private InlineProperty m_reference = new InlineProperty();

        // Read Mask
        private const int ReadMaskDefaultValue = 255;
        [SerializeField]
        private InlineProperty m_readMask = new InlineProperty( ReadMaskDefaultValue );

        //Write Mask
        private const int WriteMaskDefaultValue = 255;
        [SerializeField]
        private InlineProperty m_writeMask = new InlineProperty( WriteMaskDefaultValue );

        //Comparison Function
        private const int ComparisonDefaultValue = 0;
        [SerializeField]
        private InlineProperty m_comparisonFunctionFrontIdx = new InlineProperty( ComparisonDefaultValue );

        [SerializeField]
        private InlineProperty m_comparisonFunctionBackIdx = new InlineProperty( ComparisonDefaultValue );

        //Pass Stencil Op
        private const int PassStencilOpDefaultValue = 0;
        [SerializeField]
        private InlineProperty m_passStencilOpFrontIdx = new InlineProperty( PassStencilOpDefaultValue );

        [SerializeField]
        private InlineProperty m_passStencilOpBackIdx = new InlineProperty( PassStencilOpDefaultValue );

        //Fail Stencil Op 
        private const int FailStencilOpDefaultValue = 0;

        [SerializeField]
        private InlineProperty m_failStencilOpFrontIdx = new InlineProperty( FailStencilOpDefaultValue );

        [SerializeField]
        private InlineProperty m_failStencilOpBackIdx = new InlineProperty( FailStencilOpDefaultValue );

        //ZFail Stencil Op
        private const int ZFailStencilOpDefaultValue = 0;
        [SerializeField]
        private InlineProperty m_zFailStencilOpFrontIdx = new InlineProperty( ZFailStencilOpDefaultValue );

        [SerializeField]
        private InlineProperty m_zFailStencilOpBackIdx = new InlineProperty( ZFailStencilOpDefaultValue );

        public TemplatesStencilBufferModule() : base("Stencil Buffer")
        {
            for( int i = 0; i < StencilBufferOpHelper.StencilComparisonValues.Length; i++ )
            {
                m_comparisonDict.Add( StencilBufferOpHelper.StencilComparisonValues[ i ].ToLower(), i );
            }

            for( int i = 0; i < StencilBufferOpHelper.StencilOpsValues.Length; i++ )
            {
                m_stencilOpsDict.Add( StencilBufferOpHelper.StencilOpsValues[ i ].ToLower(), i );
            }
        }

		public void CopyFrom( TemplatesStencilBufferModule other , bool allData )
		{
			if( allData )
				m_independentModule = other.IndependentModule;

			m_active = other.Active;
			m_reference.CopyFrom( other.Reference );
			m_readMask.CopyFrom( other.ReadMask );
			m_writeMask.CopyFrom( other.WriteMask );
			m_comparisonFunctionFrontIdx.CopyFrom( other.ComparisonFunctionIdx );
			m_comparisonFunctionBackIdx.CopyFrom( other.ComparisonFunctionBackIdx );
			m_passStencilOpFrontIdx.CopyFrom( other.PassStencilOpIdx );
			m_passStencilOpBackIdx.CopyFrom( other.PassStencilOpBackIdx );
			m_failStencilOpFrontIdx.CopyFrom( other.FailStencilOpIdx );
			m_failStencilOpBackIdx.CopyFrom( other.FailStencilOpBackIdx );
			m_zFailStencilOpFrontIdx.CopyFrom( other.ZFailStencilOpIdx );
			m_zFailStencilOpBackIdx.CopyFrom( other.ZFailStencilOpBackIdx );
		}

        public void ConfigureFromTemplateData( TemplateStencilData stencilData )
        {
			bool newValidData = ( stencilData.DataCheck == TemplateDataCheck.Valid );
			if( newValidData && m_validData != newValidData )
			{
				m_active = stencilData.Active;
				m_independentModule = stencilData.IndependentModule;
				if( string.IsNullOrEmpty( stencilData.ReferenceInline ) )
				{
					m_reference.IntValue = stencilData.Reference;
					m_reference.ResetProperty();
				}
				else
				{
					m_reference.SetInlineByName( stencilData.ReferenceInline );
				}

				if( string.IsNullOrEmpty( stencilData.ReadMaskInline ) )
				{
					m_readMask.IntValue = stencilData.ReadMask;
					m_readMask.ResetProperty();
				}
				else
				{
					m_readMask.SetInlineByName( stencilData.ReadMaskInline );
				}

				if( string.IsNullOrEmpty( stencilData.WriteMaskInline ) )
				{
					m_writeMask.IntValue = stencilData.WriteMask;
					m_writeMask.ResetProperty();
				}
				else
				{
					m_writeMask.SetInlineByName( stencilData.WriteMaskInline );
				}

				if( string.IsNullOrEmpty( stencilData.ComparisonFrontInline ) )
				{
					if( !string.IsNullOrEmpty( stencilData.ComparisonFront ) )
					{
						m_comparisonFunctionFrontIdx.IntValue = m_comparisonDict[ stencilData.ComparisonFront.ToLower() ];
					}
					else
					{
						m_comparisonFunctionFrontIdx.IntValue = m_comparisonDict[ "always" ];
					}
					m_comparisonFunctionFrontIdx.ResetProperty();
				}
				else
				{
					m_comparisonFunctionFrontIdx.SetInlineByName( stencilData.ComparisonFrontInline );
				}

				if( string.IsNullOrEmpty( stencilData.PassFrontInline ) )
				{
					if( !string.IsNullOrEmpty( stencilData.PassFront ) )
					{
						m_passStencilOpFrontIdx.IntValue = m_stencilOpsDict[ stencilData.PassFront.ToLower() ];
					}
					else
					{
						m_passStencilOpFrontIdx.IntValue = m_stencilOpsDict[ "keep" ];
					}
					m_passStencilOpFrontIdx.ResetProperty();
				}
				else
				{
					m_passStencilOpFrontIdx.SetInlineByName( stencilData.PassFrontInline ); 
				}

				if( string.IsNullOrEmpty( stencilData.FailFrontInline ) )
				{
					if( !string.IsNullOrEmpty( stencilData.FailFront ) )
					{
						m_failStencilOpFrontIdx.IntValue = m_stencilOpsDict[ stencilData.FailFront.ToLower() ];
					}
					else
					{
						m_failStencilOpFrontIdx.IntValue = m_stencilOpsDict[ "keep" ];
					}
					m_failStencilOpFrontIdx.ResetProperty();
				}
				else
				{
					m_failStencilOpFrontIdx.SetInlineByName( stencilData.FailFrontInline );
				}

				if( string.IsNullOrEmpty( stencilData.ZFailFrontInline ) )
				{
					if( !string.IsNullOrEmpty( stencilData.ZFailFront ) )
					{
						m_zFailStencilOpFrontIdx.IntValue = m_stencilOpsDict[ stencilData.ZFailFront.ToLower() ];
					}
					else
					{
						m_zFailStencilOpFrontIdx.IntValue = m_stencilOpsDict[ "keep" ];
					}
					m_zFailStencilOpFrontIdx.ResetProperty();
				}
				else
				{
					m_zFailStencilOpFrontIdx.SetInlineByName( stencilData.ZFailFrontInline );
				}

				if( string.IsNullOrEmpty( stencilData.ComparisonBackInline ) )
				{
					if( !string.IsNullOrEmpty( stencilData.ComparisonBack ) )
					{
						m_comparisonFunctionBackIdx.IntValue = m_comparisonDict[ stencilData.ComparisonBack.ToLower() ];
					}
					else
					{
						m_comparisonFunctionBackIdx.IntValue = m_comparisonDict[ "always" ];
					}
					m_comparisonFunctionBackIdx.ResetProperty();
				}
				else
				{
					m_comparisonFunctionBackIdx.SetInlineByName( stencilData.ComparisonBackInline );
				}

				if( string.IsNullOrEmpty( stencilData.PassBackInline ) )
				{

					if( !string.IsNullOrEmpty( stencilData.PassBack ) )
					{
						m_passStencilOpBackIdx.IntValue = m_stencilOpsDict[ stencilData.PassBack.ToLower() ];
					}
					else
					{
						m_passStencilOpBackIdx.IntValue = m_stencilOpsDict[ "keep" ];
					}
					m_passStencilOpBackIdx.ResetProperty();
				}
				else
				{
					m_passStencilOpBackIdx.SetInlineByName( stencilData.PassBackInline );
				}

				if( string.IsNullOrEmpty( stencilData.FailBackInline ) )
				{
					if( !string.IsNullOrEmpty( stencilData.FailBack ) )
					{
						m_failStencilOpBackIdx.IntValue = m_stencilOpsDict[ stencilData.FailBack.ToLower() ];
					}
					else
					{
						m_failStencilOpBackIdx.IntValue = m_stencilOpsDict[ "keep" ];
					}
					m_failStencilOpBackIdx.ResetProperty();
				}
				else
				{
					m_failStencilOpBackIdx.SetInlineByName( stencilData.FailBackInline );
				}


				if( string.IsNullOrEmpty( stencilData.ZFailBackInline ) )
				{
					if( !string.IsNullOrEmpty( stencilData.ZFailBack ) )
					{
						m_zFailStencilOpBackIdx.IntValue = m_stencilOpsDict[ stencilData.ZFailBack.ToLower() ];
					}
					else
					{
						m_zFailStencilOpBackIdx.IntValue = m_stencilOpsDict[ "keep" ];
					}
					m_zFailStencilOpBackIdx.ResetProperty();
				}
				else
				{
					m_zFailStencilOpBackIdx.SetInlineByName( stencilData.ZFailBackInline );
				}
			}
			m_validData = newValidData;
		}

		public string CreateStencilOp( CullMode cullMode )
		{
			if( !m_active )
				return string.Empty;

			string result = "Stencil\n{\n";
			result += string.Format( "\tRef {0}\n", m_reference.GetValueOrProperty() );
			if( m_readMask.IsValid || m_readMask.IntValue != ReadMaskDefaultValue )
			{
				result += string.Format( "\tReadMask {0}\n", m_readMask.GetValueOrProperty() );
			}

			if( m_writeMask.IsValid || m_writeMask.IntValue != WriteMaskDefaultValue )
			{
				result += string.Format( "\tWriteMask {0}\n", m_writeMask.GetValueOrProperty() );
			}

			if( cullMode == CullMode.Off &&
			   ( m_comparisonFunctionBackIdx.IsValid || m_comparisonFunctionBackIdx.IntValue != ComparisonDefaultValue ||
				m_passStencilOpBackIdx.IsValid || m_passStencilOpBackIdx.IntValue != PassStencilOpDefaultValue ||
				m_failStencilOpBackIdx.IsValid || m_failStencilOpBackIdx.IntValue != FailStencilOpDefaultValue ||
				m_zFailStencilOpBackIdx.IsValid || m_zFailStencilOpBackIdx.IntValue != ZFailStencilOpDefaultValue ) )
			{
				if( m_comparisonFunctionFrontIdx.IsValid || m_comparisonFunctionFrontIdx.IntValue != ComparisonDefaultValue )
					result += string.Format( "\tCompFront {0}\n", m_comparisonFunctionFrontIdx.GetValueOrProperty( StencilBufferOpHelper.StencilComparisonValues[ m_comparisonFunctionFrontIdx.IntValue ] ) );

				if( m_passStencilOpFrontIdx.IsValid || m_passStencilOpFrontIdx.IntValue != PassStencilOpDefaultValue )
					result += string.Format( "\tPassFront {0}\n", m_passStencilOpFrontIdx.GetValueOrProperty( StencilBufferOpHelper.StencilOpsValues[ m_passStencilOpFrontIdx.IntValue ] ) );

				if( m_failStencilOpFrontIdx.IsValid || m_failStencilOpFrontIdx.IntValue != FailStencilOpDefaultValue )
					result += string.Format( "\tFailFront {0}\n", m_failStencilOpFrontIdx.GetValueOrProperty( StencilBufferOpHelper.StencilOpsValues[ m_failStencilOpFrontIdx.IntValue ] ) );

				if( m_zFailStencilOpFrontIdx.IsValid || m_zFailStencilOpFrontIdx.IntValue != ZFailStencilOpDefaultValue )
					result += string.Format( "\tZFailFront {0}\n", m_zFailStencilOpFrontIdx.GetValueOrProperty( StencilBufferOpHelper.StencilOpsValues[ m_zFailStencilOpFrontIdx.IntValue ] ) );

				if( m_comparisonFunctionBackIdx.IsValid || m_comparisonFunctionBackIdx.IntValue != ComparisonDefaultValue )
					result += string.Format( "\tCompBack {0}\n", m_comparisonFunctionBackIdx.GetValueOrProperty( StencilBufferOpHelper.StencilComparisonValues[ m_comparisonFunctionBackIdx.IntValue ] ) );

				if( m_passStencilOpBackIdx.IsValid || m_passStencilOpBackIdx.IntValue != PassStencilOpDefaultValue )
					result += string.Format( "\tPassBack {0}\n", m_passStencilOpBackIdx.GetValueOrProperty( StencilBufferOpHelper.StencilOpsValues[ m_passStencilOpBackIdx.IntValue ] ) );

				if( m_failStencilOpBackIdx.IsValid || m_failStencilOpBackIdx.IntValue != FailStencilOpDefaultValue )
                    result += string.Format( "\tFailBack {0}\n", m_failStencilOpBackIdx.GetValueOrProperty( StencilBufferOpHelper.StencilOpsValues[ m_failStencilOpBackIdx.IntValue ] ));

                if( m_zFailStencilOpBackIdx.IsValid || m_zFailStencilOpBackIdx.IntValue != ZFailStencilOpDefaultValue )
                    result += string.Format( "\tZFailBack {0}\n", m_zFailStencilOpBackIdx.GetValueOrProperty( StencilBufferOpHelper.StencilOpsValues[ m_zFailStencilOpBackIdx.IntValue ] ));
            }
            else
            {
                if( m_comparisonFunctionFrontIdx.IsValid || m_comparisonFunctionFrontIdx.IntValue != ComparisonDefaultValue )
                    result += string.Format( "\tComp {0}\n", m_comparisonFunctionFrontIdx.GetValueOrProperty(StencilBufferOpHelper.StencilComparisonValues[ m_comparisonFunctionFrontIdx.IntValue ] ));
                if( m_passStencilOpFrontIdx.IsValid || m_passStencilOpFrontIdx.IntValue != PassStencilOpDefaultValue )
                    result += string.Format( "\tPass {0}\n", m_passStencilOpFrontIdx.GetValueOrProperty( StencilBufferOpHelper.StencilOpsValues[ m_passStencilOpFrontIdx.IntValue ] ));
                if( m_failStencilOpFrontIdx.IsValid || m_failStencilOpFrontIdx.IntValue != FailStencilOpDefaultValue )
                    result += string.Format( "\tFail {0}\n", m_failStencilOpFrontIdx.GetValueOrProperty( StencilBufferOpHelper.StencilOpsValues[ m_failStencilOpFrontIdx.IntValue ] ));
                if( m_zFailStencilOpFrontIdx.IsValid || m_zFailStencilOpFrontIdx.IntValue != ZFailStencilOpDefaultValue )
                    result += string.Format( "\tZFail {0}\n", m_zFailStencilOpFrontIdx.GetValueOrProperty(StencilBufferOpHelper.StencilOpsValues[ m_zFailStencilOpFrontIdx.IntValue ] ));
            }

            result += "}";
            return result;
        }

		public override void ShowUnreadableDataMessage( ParentNode owner )
		{
			bool foldout = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedStencilOptions;
			NodeUtils.DrawPropertyGroup( ref foldout, FoldoutLabelStr, base.ShowUnreadableDataMessage );
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedStencilOptions = foldout;
		}

        public void Draw( UndoParentNode owner, CullMode cullMode , bool style = true )
        {
			bool foldout = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedStencilOptions;
			if( style )
			{
				NodeUtils.DrawPropertyGroup( ref foldout, FoldoutLabelStr, () =>
				{
					DrawBlock( owner, cullMode );
				} );
			}
			else
			{
				NodeUtils.DrawNestedPropertyGroup( owner, ref foldout, ref m_active, FoldoutLabelStr, () =>
				{
					DrawBlock( owner, cullMode );
				} );
			}
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedStencilOptions = foldout;
		}

		void DrawBlock( UndoParentNode owner, CullMode cullMode )
		{
			bool guiEnabled = GUI.enabled;
			GUI.enabled = m_active;
			EditorGUI.BeginChangeCheck();
			{
				m_reference.IntSlider( ref owner, ReferenceValueContent, 0, 255 );
				m_readMask.IntSlider( ref owner, ReadMaskContent, 0, 255 );
				m_writeMask.IntSlider( ref owner, WriteMaskContent, 0, 255 );
				if( cullMode == CullMode.Off )
				{
					m_comparisonFunctionFrontIdx.EnumTypePopup( ref owner, ComparisonFrontStr, StencilBufferOpHelper.StencilComparisonLabels );
					m_passStencilOpFrontIdx.EnumTypePopup( ref owner, PassFrontStr, StencilBufferOpHelper.StencilOpsLabels );
					m_failStencilOpFrontIdx.EnumTypePopup( ref owner, FailFrontStr, StencilBufferOpHelper.StencilOpsLabels );
					m_zFailStencilOpFrontIdx.EnumTypePopup( ref owner, ZFailFrontStr, StencilBufferOpHelper.StencilOpsLabels );
					EditorGUILayout.Separator();
					m_comparisonFunctionBackIdx.EnumTypePopup( ref owner, ComparisonBackStr, StencilBufferOpHelper.StencilComparisonLabels );
					m_passStencilOpBackIdx.EnumTypePopup( ref owner, PassBackStr, StencilBufferOpHelper.StencilOpsLabels );
					m_failStencilOpBackIdx.EnumTypePopup( ref owner, FailBackStr, StencilBufferOpHelper.StencilOpsLabels );
					m_zFailStencilOpBackIdx.EnumTypePopup( ref owner, ZFailBackStr, StencilBufferOpHelper.StencilOpsLabels );
				}
				else
				{
					m_comparisonFunctionFrontIdx.EnumTypePopup( ref owner, ComparisonStr, StencilBufferOpHelper.StencilComparisonLabels );
					m_passStencilOpFrontIdx.EnumTypePopup( ref owner, PassFrontStr, StencilBufferOpHelper.StencilOpsLabels );
					m_failStencilOpFrontIdx.EnumTypePopup( ref owner, FailFrontStr, StencilBufferOpHelper.StencilOpsLabels );
					m_zFailStencilOpFrontIdx.EnumTypePopup( ref owner, ZFailFrontStr, StencilBufferOpHelper.StencilOpsLabels );
				}
			}
			if( EditorGUI.EndChangeCheck() )
			{
				m_isDirty = true;
			}
			GUI.enabled = guiEnabled;
		}

        public override void ReadFromString( ref uint index, ref string[] nodeParams )
        {
			bool validDataOnMeta = m_validData;
			if( UIUtils.CurrentShaderVersion() > TemplatesManager.MPShaderVersion )
			{
				validDataOnMeta = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( validDataOnMeta )
			{
				if( UIUtils.CurrentShaderVersion() > 15307 )
				{
					m_active = Convert.ToBoolean( nodeParams[ index++ ] );
				}

				if( UIUtils.CurrentShaderVersion() < 15304 )
				{
					m_reference.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_readMask.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_writeMask.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_comparisonFunctionFrontIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_passStencilOpFrontIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_failStencilOpFrontIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_zFailStencilOpFrontIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_comparisonFunctionBackIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_passStencilOpBackIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_failStencilOpBackIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
					m_zFailStencilOpBackIdx.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
				}
				else
				{
					m_reference.ReadFromString( ref index, ref nodeParams );
					m_readMask.ReadFromString( ref index, ref nodeParams );
					m_writeMask.ReadFromString( ref index, ref nodeParams );
					m_comparisonFunctionFrontIdx.ReadFromString( ref index, ref nodeParams );
					m_passStencilOpFrontIdx.ReadFromString( ref index, ref nodeParams );
					m_failStencilOpFrontIdx.ReadFromString( ref index, ref nodeParams );
					m_zFailStencilOpFrontIdx.ReadFromString( ref index, ref nodeParams );
					m_comparisonFunctionBackIdx.ReadFromString( ref index, ref nodeParams );
					m_passStencilOpBackIdx.ReadFromString( ref index, ref nodeParams );
					m_failStencilOpBackIdx.ReadFromString( ref index, ref nodeParams );
					m_zFailStencilOpBackIdx.ReadFromString( ref index, ref nodeParams );
				}
				
			}
        }

        public override void WriteToString( ref string nodeInfo )
        {
			IOUtils.AddFieldValueToString( ref nodeInfo, m_validData );
			if( m_validData )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_active );
				m_reference.WriteToString( ref nodeInfo );
				m_readMask.WriteToString( ref nodeInfo );
				m_writeMask.WriteToString( ref nodeInfo );
				m_comparisonFunctionFrontIdx.WriteToString( ref nodeInfo );
				m_passStencilOpFrontIdx.WriteToString( ref nodeInfo );
				m_failStencilOpFrontIdx.WriteToString( ref nodeInfo );
				m_zFailStencilOpFrontIdx.WriteToString( ref nodeInfo );
				m_comparisonFunctionBackIdx.WriteToString( ref nodeInfo );
				m_passStencilOpBackIdx.WriteToString( ref nodeInfo );
				m_failStencilOpBackIdx.WriteToString( ref nodeInfo );
				m_zFailStencilOpBackIdx.WriteToString( ref nodeInfo );
			}
        }

        public override void Destroy()
        {
            m_comparisonDict.Clear();
            m_comparisonDict = null;

            m_stencilOpsDict.Clear();
            m_stencilOpsDict = null;

			m_reference = null;
			m_readMask = null;
			m_writeMask = null;
			m_comparisonFunctionFrontIdx = null;
			m_passStencilOpFrontIdx = null;
			m_failStencilOpFrontIdx = null;
			m_zFailStencilOpFrontIdx = null;
			m_comparisonFunctionBackIdx = null;
			m_passStencilOpBackIdx = null;
			m_failStencilOpBackIdx = null;
			m_zFailStencilOpBackIdx = null;
		}
		public bool Active { get { return m_active; } }
		public InlineProperty Reference { get { return m_reference; } }
		public InlineProperty ReadMask { get { return m_readMask; } }
		public InlineProperty WriteMask { get { return m_writeMask; } }
		public InlineProperty ComparisonFunctionIdx { get { return m_comparisonFunctionFrontIdx; } }
		public InlineProperty ComparisonFunctionBackIdx { get { return m_comparisonFunctionBackIdx; } }
		public InlineProperty PassStencilOpIdx { get { return m_passStencilOpFrontIdx; } }
		public InlineProperty PassStencilOpBackIdx { get { return m_passStencilOpBackIdx; } }
		public InlineProperty FailStencilOpIdx { get { return m_failStencilOpFrontIdx; } }
		public InlineProperty FailStencilOpBackIdx { get { return m_failStencilOpBackIdx; } }
		public InlineProperty ZFailStencilOpIdx { get { return m_zFailStencilOpFrontIdx; } }
		public InlineProperty ZFailStencilOpBackIdx { get { return m_zFailStencilOpBackIdx; } }

	}
}
