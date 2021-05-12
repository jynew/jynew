// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public sealed class TemplateDepthModule : TemplateModuleParent
	{
		private const string ZWriteFormatter = "ZWrite {0}\n";
		private const string ZTestFormatter = "ZTest {0}\n";
		
		[ SerializeField]
		private bool m_validZTest = false;

		[SerializeField]
		private InlineProperty m_zTestMode = new InlineProperty(0);

		[SerializeField]
		private bool m_validZWrite = false;

		[SerializeField]
		private InlineProperty m_zWriteMode = new InlineProperty(0);

		[SerializeField]
		private InlineProperty m_offsetFactor = new InlineProperty(0);

		[SerializeField]
		private InlineProperty m_offsetUnits = new InlineProperty(0);

		[SerializeField]
		private bool m_offsetEnabled = false;

		[SerializeField]
		private bool m_validOffset = false;

		public TemplateDepthModule() : base( "Depth" ) { }

		public void CopyFrom( TemplateDepthModule other , bool allData )
		{
			if( allData )
			{
				m_independentModule = other.IndependentModule;
				m_validZTest = other.ValidZTest;
				m_validZWrite = other.ValidZWrite;
				m_validOffset = other.ValidOffset;
			}

			m_zTestMode.CopyFrom( other.ZTestMode );
			m_zWriteMode.CopyFrom( other.ZWriteMode );
			m_offsetFactor.CopyFrom( other.OffsetFactor );
			m_offsetUnits.CopyFrom( other.OffsetUnits );
			m_offsetEnabled = other.OffsetEnabled;
		}

		public void ConfigureFromTemplateData( TemplateDepthData depthData )
		{
			m_independentModule = depthData.IndependentModule;
			if( depthData.ValidZTest && m_validZTest != depthData.ValidZTest )
			{
				if( string.IsNullOrEmpty( depthData.ZTestInlineValue ) )
				{
					m_zTestMode.IntValue = ZBufferOpHelper.ZTestModeDict[ depthData.ZTestModeValue ];
					m_zTestMode.ResetProperty();
				}
				else
				{
					m_zTestMode.SetInlineByName( depthData.ZTestInlineValue );
				}
			}



			if( depthData.ValidZWrite && m_validZWrite != depthData.ValidZWrite )
			{
				if( string.IsNullOrEmpty( depthData.ZWriteInlineValue ) )
				{
					m_zWriteMode.IntValue = ZBufferOpHelper.ZWriteModeDict[ depthData.ZWriteModeValue ];
					m_zWriteMode.ResetProperty();
				}
				else
				{
					m_zWriteMode.SetInlineByName( depthData.ZWriteInlineValue );
				}
			}

			if( depthData.ValidOffset && m_validOffset != depthData.ValidOffset )
			{
				if( string.IsNullOrEmpty( depthData.OffsetFactorInlineValue ) )
				{
					m_offsetFactor.FloatValue = depthData.OffsetFactor;
					m_offsetFactor.ResetProperty();
				}
				else
				{
					m_offsetFactor.SetInlineByName( depthData.OffsetFactorInlineValue );
				}

				if( string.IsNullOrEmpty( depthData.OffsetUnitsInlineValue ) )
				{
					m_offsetUnits.FloatValue = depthData.OffsetUnits;
					m_offsetUnits.ResetProperty();
				}
				else
				{
					m_offsetUnits.SetInlineByName( depthData.OffsetUnitsInlineValue );
				}
				m_offsetEnabled = depthData.ValidOffset;
			}

			m_validZTest = depthData.ValidZTest;
			m_validZWrite = depthData.ValidZWrite;
			m_validOffset = depthData.ValidOffset;
			m_validData = m_validZTest || m_validZWrite || m_validOffset;
		}

		public override void ShowUnreadableDataMessage( ParentNode owner )
		{
			bool foldoutValue = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedDepth;
			NodeUtils.DrawPropertyGroup( ref foldoutValue, ZBufferOpHelper.DepthParametersStr, base.ShowUnreadableDataMessage );
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedDepth = foldoutValue;
		}

		public override void Draw( UndoParentNode owner , bool style = true)
		{
			bool foldout = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedDepth;
			if( style )
			{
				NodeUtils.DrawPropertyGroup( ref foldout, ZBufferOpHelper.DepthParametersStr, () =>
				{
					EditorGUI.indentLevel++;
					DrawBlock( owner );
					EditorGUI.indentLevel--;
				} );
			}
			else
			{
				NodeUtils.DrawNestedPropertyGroup( ref foldout, ZBufferOpHelper.DepthParametersStr, () =>
				{
					DrawBlock( owner );
				} );
			}
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedDepth = foldout;
		}

		void DrawBlock( UndoParentNode owner )
		{
			EditorGUI.BeginChangeCheck();
			Color cachedColor = GUI.color;
			GUI.color = new Color( cachedColor.r, cachedColor.g, cachedColor.b, ( EditorGUIUtility.isProSkin ? 0.5f : 0.25f ) );
			//EditorGUILayout.BeginVertical( UIUtils.MenuItemBackgroundStyle );
			GUI.color = cachedColor;
			
			EditorGUILayout.Separator();

			if( m_validZWrite )
				m_zWriteMode.EnumTypePopup( ref owner, ZBufferOpHelper.ZWriteModeStr, ZBufferOpHelper.ZWriteModeValues );

			if( m_validZTest )
				m_zTestMode.EnumTypePopup( ref owner, ZBufferOpHelper.ZTestModeStr, ZBufferOpHelper.ZTestModeLabels );


			if( m_validOffset )
			{
				m_offsetEnabled = owner.EditorGUILayoutToggle( ZBufferOpHelper.OffsetStr, m_offsetEnabled );
				if( m_offsetEnabled )
				{
					EditorGUI.indentLevel++;
					m_offsetFactor.FloatField( ref owner , ZBufferOpHelper.OffsetFactorStr );
					m_offsetUnits.FloatField( ref owner , ZBufferOpHelper.OffsetUnitsStr);
					EditorGUI.indentLevel--;
				}
			}
			EditorGUILayout.Separator();
			
			//EditorGUILayout.EndVertical();
			if( EditorGUI.EndChangeCheck() )
			{
				m_isDirty = true;
			}
		}
		
		public void ReadZWriteFromString( ref uint index, ref string[] nodeParams )
		{
			bool validDataOnMeta = m_validZWrite;
			if( UIUtils.CurrentShaderVersion() > TemplatesManager.MPShaderVersion )
			{
				validDataOnMeta = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( validDataOnMeta )
			{
				if( UIUtils.CurrentShaderVersion() < 15304 )
				{
					m_zWriteMode.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
				}
				else
				{
					m_zWriteMode.ReadFromString( ref index, ref nodeParams );
				}
			}
		}

		public void ReadZTestFromString( ref uint index, ref string[] nodeParams )
		{
			bool validDataOnMeta = m_validZTest;
			if( UIUtils.CurrentShaderVersion() > TemplatesManager.MPShaderVersion )
			{
				validDataOnMeta = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( validDataOnMeta )
			{
				if( UIUtils.CurrentShaderVersion() < 15304 )
				{
					m_zTestMode.IntValue = Convert.ToInt32( nodeParams[ index++ ] );
				}
				else
				{
					m_zTestMode.ReadFromString( ref index, ref nodeParams );
				}
			}
		}

		public void ReadOffsetFromString( ref uint index, ref string[] nodeParams )
		{
			bool validDataOnMeta = m_validOffset;
			if( UIUtils.CurrentShaderVersion() > TemplatesManager.MPShaderVersion )
			{
				validDataOnMeta = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( validDataOnMeta )
			{
				m_offsetEnabled = Convert.ToBoolean( nodeParams[ index++ ] );
				if( UIUtils.CurrentShaderVersion() < 15304 )
				{
					m_offsetFactor.FloatValue = Convert.ToSingle( nodeParams[ index++ ] );
					m_offsetUnits.FloatValue = Convert.ToSingle( nodeParams[ index++ ] );
				}
				else
				{
					m_offsetFactor.ReadFromString( ref index, ref nodeParams );
					m_offsetUnits.ReadFromString( ref index, ref nodeParams );
				}
			}
		}
		
		public override void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			ReadZWriteFromString( ref index, ref nodeParams );
			ReadZTestFromString( ref index, ref nodeParams );
			ReadOffsetFromString( ref index, ref nodeParams );
		}

		public void WriteZWriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_validZWrite );
			if( m_validZWrite )
				m_zWriteMode.WriteToString( ref nodeInfo );
		}

		public void WriteZTestToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_validZTest );
			if( m_validZTest )
				m_zTestMode.WriteToString( ref nodeInfo );
		}

		public void WriteOffsetToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_validOffset );
			if( m_validOffset )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_offsetEnabled );
				m_offsetFactor.WriteToString(ref nodeInfo);
				m_offsetUnits.WriteToString( ref nodeInfo );
			}
		}

		public override void WriteToString( ref string nodeInfo )
		{
			WriteZWriteToString( ref nodeInfo );
			WriteZTestToString( ref nodeInfo );
			WriteOffsetToString( ref nodeInfo );
		}

		public bool IsActive { get { return ( m_zTestMode.IsValid || m_zTestMode.IntValue != 0) || ( m_zWriteMode .IsValid || m_zWriteMode.IntValue != 0) || m_offsetEnabled; } }
		public string CurrentZWriteMode
		{
			get
			{
				if( m_zWriteMode.IsValid )
				{
					return string.Format( ZWriteFormatter, m_zWriteMode.GetValueOrProperty() ); ;
				}

				int finalZWrite = ( m_zWriteMode.IntValue == 0 ) ? 1 : m_zWriteMode.IntValue;
				return string.Format( ZWriteFormatter, ZBufferOpHelper.ZWriteModeValues[ finalZWrite ] ); ;
			}
		}
		public string CurrentZTestMode
		{
			get
			{
				if( m_zTestMode.IsValid )
					return string.Format( ZTestFormatter, m_zTestMode.GetValueOrProperty() );

				int finalZTestMode = ( m_zTestMode.IntValue == 0 )?3 : m_zTestMode.IntValue;
				return string.Format( ZTestFormatter,  ZBufferOpHelper.ZTestModeValues[ finalZTestMode ] );
			}
		}

		public string CurrentOffset
		{
			get
			{
				if( m_offsetEnabled )
					return "Offset " + m_offsetFactor.GetValueOrProperty() + " , " + m_offsetUnits.GetValueOrProperty() + "\n";
				else
					return "Offset 0,0\n";
			}
		}

		public bool ValidZTest { get { return m_validZTest; } }
		public bool ValidZWrite { get { return m_validZWrite; } }
		public bool ValidOffset { get { return m_validOffset; } }
		public InlineProperty ZTestMode { get { return m_zTestMode; } }
		public InlineProperty ZWriteMode { get { return m_zWriteMode; } }
		public InlineProperty OffsetFactor { get { return m_offsetFactor; } }
		public InlineProperty OffsetUnits { get { return m_offsetUnits; } }
		public bool OffsetEnabled { get { return m_offsetEnabled; } }

	}
}
