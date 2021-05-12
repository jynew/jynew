// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;
using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class EditorVariable<T>
	{
		protected string m_labelName;
		protected string m_name;
		protected T m_value;
		protected T m_defaultValue;

		public EditorVariable( string name, string labelName, T defaultValue ) { m_name = name; m_labelName = labelName; m_defaultValue = defaultValue; m_value = defaultValue; }
		public string Name { get { return m_name; } }

		public virtual T Value
		{
			get { return m_value; }
			set
			{
				m_value = value;
			}
		}
		public string LabelName { get { return m_labelName; } }
	}

	public sealed class EditorVariableFloat : EditorVariable<float>
	{
		public EditorVariableFloat( string name, string labelName, float defaultValue ) : base( name, labelName, defaultValue )
		{
			m_value = EditorPrefs.GetFloat( name, m_defaultValue );
		}

		public override float Value
		{
			get { return m_value; }
			set
			{
				if( m_value != value )
				{
					m_value = value;
					EditorPrefs.SetFloat( m_name, m_value );
				}
			}
		}
	}

	public sealed class EditorVariableBool : EditorVariable<bool>
	{
		public EditorVariableBool( string name, string labelName, bool defaultValue ) : base( name, labelName, defaultValue )
		{
			m_value = EditorPrefs.GetBool( name, m_defaultValue );
		}

		public override bool Value
		{
			get { return m_value; }
			set
			{
				if( m_value != value )
				{
					m_value = value;
					EditorPrefs.SetBool( m_name, m_value );
				}
			}
		}
	}

	public sealed class EditorVariableInt : EditorVariable<int>
	{
		public EditorVariableInt( string name, string labelName, int defaultValue ) : base( name, labelName, defaultValue )
		{
			m_value = EditorPrefs.GetInt( name, m_defaultValue );
		}

		public override int Value
		{
			get { return m_value; }
			set
			{
				if( m_value != value )
				{
					m_value = value;
					EditorPrefs.SetInt( m_name, m_value );
				}
			}
		}
	}

	public sealed class EditorVariableString : EditorVariable<string>
	{
		public EditorVariableString( string name, string labelName, string defaultValue ) : base( name, labelName, defaultValue )
		{
			m_value = EditorPrefs.GetString( name, m_defaultValue );
		}

		public override string Value
		{
			get { return m_value; }
			set
			{
				if( !m_value.Equals( value ) )
				{
					m_value = value;
					EditorPrefs.SetString( m_name, m_value );
				}
			}
		}
	}

	public class EditorVariablesManager
	{
		public static EditorVariableBool LiveMode = new EditorVariableBool( "ASELiveMode", "LiveMode", false );
		public static EditorVariableBool OutlineActiveMode = new EditorVariableBool( "ASEOutlineActiveMode", " Outline", false );
		public static EditorVariableBool NodeParametersMaximized = new EditorVariableBool( "ASENodeParametersVisible", " NodeParameters", true );
		public static EditorVariableBool NodePaletteMaximized = new EditorVariableBool( "ASENodePaletteVisible", " NodePalette", true );
		public static EditorVariableBool ExpandedRenderingPlatforms = new EditorVariableBool( "ASEExpandedRenderingPlatforms", " ExpandedRenderingPlatforms", false );
		public static EditorVariableBool ExpandedRenderingOptions = new EditorVariableBool( "ASEExpandedRenderingOptions", " ExpandedRenderingPlatforms", false );
		public static EditorVariableBool ExpandedGeneralShaderOptions = new EditorVariableBool( "ASEExpandedGeneralShaderOptions", " ExpandedGeneralShaderOptions", false );
		public static EditorVariableBool ExpandedBlendOptions = new EditorVariableBool( "ASEExpandedBlendOptions", " ExpandedBlendOptions", false );
		public static EditorVariableBool ExpandedStencilOptions = new EditorVariableBool( "ASEExpandedStencilOptions", " ExpandedStencilOptions", false );
		public static EditorVariableBool ExpandedVertexOptions = new EditorVariableBool( "ASEExpandedVertexOptions", " ExpandedVertexOptions", false );
		public static EditorVariableBool ExpandedFunctionInputs = new EditorVariableBool( "ASEExpandedFunctionInputs", " ExpandedFunctionInputs", false );
		public static EditorVariableBool ExpandedFunctionSwitches = new EditorVariableBool( "ASEExpandedFunctionSwitches", " ExpandedFunctionSwitches", false );
		public static EditorVariableBool ExpandedFunctionOutputs = new EditorVariableBool( "ASEExpandedFunctionOutputs", " ExpandedFunctionOutputs", false );
		public static EditorVariableBool ExpandedAdditionalIncludes = new EditorVariableBool( "ASEExpandedAdditionalIncludes", " ExpandedAdditionalIncludes", false );
		public static EditorVariableBool ExpandedAdditionalDefines = new EditorVariableBool( "ASEExpandedAdditionalDefines", " ExpandedAdditionalDefines", false );
		public static EditorVariableBool ExpandedAdditionalDirectives = new EditorVariableBool( "ASEExpandedAdditionalDirectives", " ExpandedAdditionalDirectives", false );
		public static EditorVariableBool ExpandedCustomTags = new EditorVariableBool( "ASEExpandedCustomTags", " ExpandedCustomTags", false );
		public static EditorVariableBool ExpandedAdditionalPragmas = new EditorVariableBool( "ASEExpandedAdditionalPragmas", " ExpandedAdditionalPragmas", false );
		public static EditorVariableBool ExpandedDependencies = new EditorVariableBool( "ASEExpandedDependencies", " ExpandedDependencies", false );
		public static EditorVariableBool ExpandedDepth = new EditorVariableBool( "ASEExpandedDepth", " ExpandedDepth", false );
		public static EditorVariableBool ExpandedTesselation  = new EditorVariableBool( "ASEExpandedTesselation", " ExpandedTesselation", false );
		public static EditorVariableBool ExpandedProperties = new EditorVariableBool( "ASEExpandedProperties", " ExpandedProperties", false );
		public static EditorVariableBool ExpandedUsePass = new EditorVariableBool( "ASEUsePass", " UsePass", false );
		//Templates
		public static EditorVariableBool ExpandedBlendModeModule = new EditorVariableBool( "ASEExpandedBlendModeModule", " ExpandedBlendModeModule", false );
	}

	[Serializable]
	public class InnerWindowEditorVariables
	{
		[SerializeField]
		private bool m_liveMode = false;
		[SerializeField]
		private bool m_outlineActiveMode = false;
		[SerializeField]
		private bool m_nodeParametersMaximized = false;
		[SerializeField]
		private bool m_nodePaletteMaximized = false;
		[SerializeField]
		private bool m_expandedRenderingPlatforms = false;
		[SerializeField]
		private bool m_expandedRenderingOptions = false;
		[SerializeField]
		private bool m_expandedGeneralShaderOptions = false;
		[SerializeField]
		private bool m_expandedBlendOptions = false;
		[SerializeField]
		private bool m_expandedStencilOptions = false;
		[SerializeField]
		private bool m_expandedVertexOptions = false;
		[SerializeField]
		private bool m_expandedFunctionInputs = false;
		[SerializeField]
		private bool m_expandedFunctionSwitches = false;
		[SerializeField]
		private bool m_expandedFunctionOutputs = false;
		[SerializeField]
		private bool m_expandedAdditionalIncludes = false;
		[SerializeField]
		private bool m_expandedAdditionalDefines = false;
		[SerializeField]
		private bool m_expandedAdditionalDirectives = false;
		[SerializeField]
		private bool m_expandedCustomTags = false;
		[SerializeField]
		private bool m_expandedAdditionalPragmas = false;
		[SerializeField]
		private bool m_expandedDependencies = false;
		[SerializeField]
		private bool m_expandedBlendModeModule = false;
		[SerializeField]
		private bool m_expandedDepth = false;
		[SerializeField]
		private bool m_expandedTesselation = false;
		[SerializeField]
		private bool m_expandedProperties = false;
		[SerializeField]
		private bool m_expandedUsePass = false;

		public void Initialize()
		{
			m_liveMode = EditorVariablesManager.LiveMode.Value;
			m_outlineActiveMode = EditorVariablesManager.OutlineActiveMode.Value;
			m_nodeParametersMaximized = EditorVariablesManager.NodeParametersMaximized.Value;
			m_nodePaletteMaximized = EditorVariablesManager.NodePaletteMaximized.Value;
			m_expandedRenderingPlatforms = EditorVariablesManager.ExpandedRenderingPlatforms.Value;
			m_expandedRenderingOptions = EditorVariablesManager.ExpandedRenderingOptions.Value;
			m_expandedGeneralShaderOptions = EditorVariablesManager.ExpandedGeneralShaderOptions.Value;
			m_expandedBlendOptions = EditorVariablesManager.ExpandedBlendOptions.Value;
			m_expandedStencilOptions = EditorVariablesManager.ExpandedStencilOptions.Value;
			m_expandedVertexOptions = EditorVariablesManager.ExpandedVertexOptions.Value;
			m_expandedFunctionInputs = EditorVariablesManager.ExpandedFunctionInputs.Value;
			m_expandedFunctionSwitches = EditorVariablesManager.ExpandedFunctionSwitches.Value;
			m_expandedFunctionOutputs = EditorVariablesManager.ExpandedFunctionOutputs.Value;
			m_expandedAdditionalIncludes = EditorVariablesManager.ExpandedAdditionalIncludes.Value;
			m_expandedAdditionalDefines = EditorVariablesManager.ExpandedAdditionalDefines.Value;
			m_expandedAdditionalDirectives = EditorVariablesManager.ExpandedAdditionalDirectives.Value;
			m_expandedCustomTags = EditorVariablesManager.ExpandedCustomTags.Value;
			m_expandedAdditionalPragmas = EditorVariablesManager.ExpandedAdditionalPragmas.Value;
			m_expandedDependencies = EditorVariablesManager.ExpandedDependencies.Value;
			m_expandedBlendModeModule = EditorVariablesManager.ExpandedBlendModeModule.Value;
			m_expandedDepth = EditorVariablesManager.ExpandedDepth.Value;
			m_expandedTesselation = EditorVariablesManager.ExpandedTesselation.Value;
			m_expandedProperties = EditorVariablesManager.ExpandedProperties.Value;
			m_expandedUsePass = EditorVariablesManager.ExpandedUsePass.Value;
		}

		public bool LiveMode{ get { return m_liveMode; } set { m_liveMode = value; EditorVariablesManager.LiveMode.Value = value; } }
		public bool OutlineActiveMode { get { return m_outlineActiveMode; } set { m_outlineActiveMode = value; EditorVariablesManager.OutlineActiveMode.Value = value; } }
		public bool NodeParametersMaximized { get { return m_nodeParametersMaximized; } set { m_nodeParametersMaximized = value; EditorVariablesManager.NodeParametersMaximized.Value = value; } }
		public bool NodePaletteMaximized { get { return m_nodePaletteMaximized; } set { m_nodePaletteMaximized = value; EditorVariablesManager.NodePaletteMaximized.Value = value; } }
		public bool ExpandedRenderingPlatforms { get { return m_expandedRenderingPlatforms; } set { m_expandedRenderingPlatforms = value; EditorVariablesManager.ExpandedRenderingPlatforms.Value = value; } }
		public bool ExpandedRenderingOptions { get { return m_expandedRenderingOptions; } set { m_expandedRenderingOptions = value; EditorVariablesManager.ExpandedRenderingOptions.Value = value; } }
		public bool ExpandedGeneralShaderOptions { get { return m_expandedGeneralShaderOptions; } set { m_expandedGeneralShaderOptions = value; EditorVariablesManager.ExpandedGeneralShaderOptions.Value = value; } }
		public bool ExpandedBlendOptions { get { return m_expandedBlendOptions; } set { m_expandedBlendOptions = value; EditorVariablesManager.ExpandedBlendOptions.Value = value; } }
		public bool ExpandedStencilOptions { get { return m_expandedStencilOptions; } set { m_expandedStencilOptions = value; EditorVariablesManager.ExpandedStencilOptions.Value = value; } }
		public bool ExpandedVertexOptions { get { return m_expandedVertexOptions; } set { m_expandedVertexOptions = value; EditorVariablesManager.ExpandedVertexOptions.Value = value; } }
		public bool ExpandedFunctionInputs { get { return m_expandedFunctionInputs; } set { m_expandedFunctionInputs = value; EditorVariablesManager.ExpandedFunctionInputs.Value = value; } }
		public bool ExpandedFunctionSwitches { get { return m_expandedFunctionSwitches; } set { m_expandedFunctionSwitches = value; EditorVariablesManager.ExpandedFunctionSwitches.Value = value; } }
		public bool ExpandedFunctionOutputs { get { return m_expandedFunctionOutputs; } set { m_expandedFunctionOutputs = value; EditorVariablesManager.ExpandedFunctionOutputs.Value = value; } }
		public bool ExpandedAdditionalIncludes { get { return m_expandedAdditionalIncludes; } set { m_expandedAdditionalIncludes = value; EditorVariablesManager.ExpandedAdditionalIncludes.Value = value; } }
		public bool ExpandedAdditionalDefines { get { return m_expandedAdditionalDefines; } set { m_expandedAdditionalDefines = value; EditorVariablesManager.ExpandedAdditionalDefines.Value = value; } }
		public bool ExpandedAdditionalDirectives { get { return m_expandedAdditionalDirectives; } set { m_expandedAdditionalDirectives = value; EditorVariablesManager.ExpandedAdditionalDirectives.Value = value; } }
		public bool ExpandedCustomTags { get { return m_expandedCustomTags; } set { m_expandedCustomTags = value; EditorVariablesManager.ExpandedCustomTags.Value = value; } }
		public bool ExpandedAdditionalPragmas { get { return m_expandedAdditionalPragmas; } set { m_expandedAdditionalPragmas = value; EditorVariablesManager.ExpandedAdditionalPragmas.Value = value; } }
		public bool ExpandedDependencies { get { return m_expandedDependencies; } set { m_expandedDependencies = value; EditorVariablesManager.ExpandedDependencies.Value = value; } }
		public bool ExpandedBlendModeModule { get { return m_expandedBlendModeModule; } set { m_expandedBlendModeModule = value; EditorVariablesManager.ExpandedBlendModeModule.Value = value; } }
		public bool ExpandedDepth { get { return m_expandedDepth; } set { m_expandedDepth = value; EditorVariablesManager.ExpandedDepth.Value = value; } }
		public bool ExpandedTesselation { get { return m_expandedTesselation; } set { m_expandedTesselation = value; EditorVariablesManager.ExpandedTesselation.Value = value; } }
		public bool ExpandedProperties { get { return m_expandedProperties; } set { m_expandedProperties = value; EditorVariablesManager.ExpandedProperties.Value = value; } }
		public bool ExpandedUsePass { get { return m_expandedUsePass; } set { m_expandedUsePass = value; EditorVariablesManager.ExpandedUsePass.Value = value; } }
	}
}
