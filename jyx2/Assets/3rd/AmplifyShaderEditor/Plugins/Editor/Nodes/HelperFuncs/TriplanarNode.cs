using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Triplanar Sample", "Textures", "Triplanar Mapping" )]
	public sealed class TriplanarNode : ParentNode
	{
		[SerializeField]
		private string m_uniqueName;

		private bool m_editPropertyNameMode = false;
		[SerializeField]
		private string m_propertyInspectorName = "Triplanar Sampler";

		private enum TriplanarType { Spherical, Cylindrical }

		[SerializeField]
		private TriplanarType m_selectedTriplanarType = TriplanarType.Spherical;

		private enum TriplanarSpace { Object, World }

		[SerializeField]
		private TriplanarSpace m_selectedTriplanarSpace = TriplanarSpace.World;

		[SerializeField]
		private bool m_normalCorrection = false;

		[SerializeField]
		private bool m_arraySupport = false;

		[SerializeField]
		private TexturePropertyNode m_topTexture;
		[SerializeField]
		private TexturePropertyNode m_midTexture;
		[SerializeField]
		private TexturePropertyNode m_botTexture;

		bool m_texturesInitialize = false;

		[SerializeField]
		private string m_tempTopInspectorName = string.Empty;
		[SerializeField]
		private string m_tempTopName = string.Empty;
		private TexturePropertyValues m_tempTopDefaultValue = TexturePropertyValues.white;
		private int m_tempTopOrderIndex = -1;
		private Texture2D m_tempTopDefaultTexture = null;

		private string m_tempMidInspectorName = string.Empty;
		private string m_tempMidName = string.Empty;
		private TexturePropertyValues m_tempMidDefaultValue = TexturePropertyValues.white;
		private int m_tempMidOrderIndex = -1;
		private Texture2D m_tempMidDefaultTexture = null;

		private string m_tempBotInspectorName = string.Empty;
		private string m_tempBotName = string.Empty;
		private TexturePropertyValues m_tempBotDefaultValue = TexturePropertyValues.white;
		private int m_tempBotOrderIndex = -1;
		private Texture2D m_tempBotDefaultTexture = null;

		private bool m_topTextureFoldout = true;
		private bool m_midTextureFoldout = true;
		private bool m_botTextureFoldout = true;

		private InputPort m_topTexPort;
		private InputPort m_midTexPort;
		private InputPort m_botTexPort;
		private InputPort m_tilingPort;
		private InputPort m_falloffPort;
		private InputPort m_topIndexPort;
		private InputPort m_midIndexPort;
		private InputPort m_botIndexPort;
		private InputPort m_scalePort;


		private readonly string m_functionCall = "TriplanarSampling{0}( {1} )";
		private readonly string m_functionHeader = "inline {0} TriplanarSampling{1}( {2}float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 normalScale, float3 index )";

		private readonly string m_singularTexture = "sampler2D topTexMap, ";
		private readonly string m_topmidbotTexture = "sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, ";

		private readonly string m_singularArrayTexture = "UNITY_ARGS_TEX2DARRAY( topTexMap ), ";
		private readonly string m_topmidbotArrayTexture = "UNITY_ARGS_TEX2DARRAY( topTexMap ), UNITY_ARGS_TEX2DARRAY( midTexMap ), UNITY_ARGS_TEX2DARRAY( botTexMap ), ";

		private readonly List<string> m_functionSamplingBodyProj = new List<string>() {
			"float3 projNormal = ( pow( abs( worldNormal ), falloff ) );",
			"projNormal /= projNormal.x + projNormal.y + projNormal.z;",
			"float3 nsign = sign( worldNormal );"
		};

		private readonly List<string> m_functionSamplingBodyNegProj = new List<string>() {
			"float negProjNormalY = max( 0, projNormal.y * -nsign.y );",
			"projNormal.y = max( 0, projNormal.y * nsign.y );"
		};

		// Sphere sampling
		private readonly List<string> m_functionSamplingBodySampSphere = new List<string>() {
			"half4 xNorm; half4 yNorm; half4 zNorm;",
			"xNorm = ( {0}( topTexMap, {1}tilling * worldPos.zy * float2( nsign.x, 1.0 ){2} ) );",
			"yNorm = ( {0}( topTexMap, {1}tilling * worldPos.xz * float2( nsign.y, 1.0 ){2} ) );",
			"zNorm = ( {0}( topTexMap, {1}tilling * worldPos.xy * float2( -nsign.z, 1.0 ){2} ) );"
		};

		// Cylinder sampling
		private readonly List<string> m_functionSamplingBodySampCylinder = new List<string>() {
			"half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;",
			"xNorm = ( {0}( midTexMap, {1}tilling * worldPos.zy * float2( nsign.x, 1.0 ){3} ) );",
			"yNorm = ( {0}( topTexMap, {1}tilling * worldPos.xz * float2( nsign.y, 1.0 ){2} ) );",
			"yNormN = ( {0}( botTexMap, {1}tilling * worldPos.xz * float2( nsign.y, 1.0 ){4} ) );",
			"zNorm = ( {0}( midTexMap, {1}tilling * worldPos.xy * float2( -nsign.z, 1.0 ){3} ) );"
		};

		private readonly List<string> m_functionSamplingBodySignsSphere = new List<string>() {
			"xNorm.xyz = half3( {0}( xNorm{1} ).xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;",
			"yNorm.xyz = half3( {0}( yNorm{1} ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;",
			"zNorm.xyz = half3( {0}( zNorm{1} ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;"
		};

		private readonly List<string> m_functionSamplingBodySignsSphereScale = new List<string>() {
			"xNorm.xyz = half3( {0}( xNorm, normalScale.y ).xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;",
			"yNorm.xyz = half3( {0}( yNorm, normalScale.x ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;",
			"zNorm.xyz = half3( {0}( zNorm, normalScale.y ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;"
		};

		private readonly List<string> m_functionSamplingBodySignsCylinder = new List<string>() {
			"yNormN.xyz = half3( {0}( yNormN {1} ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;"
		};

		private readonly List<string> m_functionSamplingBodySignsCylinderScale = new List<string>() {
			"yNormN.xyz = half3( {0}( yNormN, normalScale.z ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;"
		};

		private readonly List<string> m_functionSamplingBodyReturnSphereNormalize = new List<string>() {
			"return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );"
		};

		private readonly List<string> m_functionSamplingBodyReturnCylinderNormalize = new List<string>() {
			"return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + yNormN.xyz * negProjNormalY + zNorm.xyz * projNormal.z );"
		};

		private readonly List<string> m_functionSamplingBodyReturnSphere = new List<string>() {
			"return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;"
		};

		private readonly List<string> m_functionSamplingBodyReturnCylinder = new List<string>() {
			"return xNorm * projNormal.x + yNorm * projNormal.y + yNormN * negProjNormalY + zNorm * projNormal.z;"
		};

		private Rect m_allPicker;
		private Rect m_startPicker;
		private Rect m_pickerButton;
		private bool m_editing;

		void ConvertListTo( MasterNodeDataCollector dataCollector, bool scaleInfo , List<string> original , List<string> dest )
		{
			int count = original.Count;
			string scale = string.Empty;
			string func = string.Empty;
			bool applyScale = false;
			if( dataCollector.IsTemplate && dataCollector.IsSRP )
			{
				func = "UnpackNormalmapRGorAG";
				if( !scaleInfo )
				{
					scale = " , 1.0";
					applyScale = true;
				}
			}
			else
			{
				func = scaleInfo? "UnpackScaleNormal": "UnpackNormal";
				applyScale = !scaleInfo;
			}

			for(int i = 0; i < count; i++ )
			{
				if( applyScale )
					dest.Add( string.Format( original[ i ], func, scale ));
				else
					dest.Add( string.Format( original[ i ], func ) );
			}
		}

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.SAMPLER2D, true, "Top", -1, MasterNodePortCategory.Fragment, 0 );
			AddInputPort( WirePortDataType.FLOAT, true, "Top Index", -1, MasterNodePortCategory.Fragment, 5 );
			AddInputPort( WirePortDataType.SAMPLER2D, true, "Middle", -1, MasterNodePortCategory.Fragment, 1 );
			AddInputPort( WirePortDataType.FLOAT, true, "Mid Index", -1, MasterNodePortCategory.Fragment, 6 );
			AddInputPort( WirePortDataType.SAMPLER2D, true, "Bottom", -1, MasterNodePortCategory.Fragment, 2 );
			AddInputPort( WirePortDataType.FLOAT, true, "Bot Index", -1, MasterNodePortCategory.Fragment, 7 );
			AddInputPort( WirePortDataType.FLOAT3, true, "Scale", -1, MasterNodePortCategory.Fragment, 8 );
			AddInputPort( WirePortDataType.FLOAT, true, "Tiling", -1, MasterNodePortCategory.Fragment, 3 );
			AddInputPort( WirePortDataType.FLOAT, true, "Falloff", -1, MasterNodePortCategory.Fragment, 4 );
			AddOutputColorPorts( "RGBA" );
			m_useInternalPortData = true;
			m_topTexPort = InputPorts[ 0 ];
			m_topIndexPort = InputPorts[ 1 ];
			m_midTexPort = InputPorts[ 2 ];
			m_midIndexPort = InputPorts[ 3 ];
			m_botTexPort = InputPorts[ 4 ];
			m_botIndexPort = InputPorts[ 5 ];
			m_scalePort = InputPorts[ 6 ];
			m_tilingPort = InputPorts[ 7 ];
			m_falloffPort = InputPorts[ 8 ];

			m_scalePort.Visible = false;
			m_scalePort.Vector3InternalData = Vector3.one;
			m_tilingPort.FloatInternalData = 1;
			m_topIndexPort.FloatInternalData = 1;
			m_falloffPort.FloatInternalData = 1;
			m_topIndexPort.Visible = false;
			m_selectedLocation = PreviewLocation.TopCenter;
			m_marginPreviewLeft = 43;
			m_drawPreviewAsSphere = true;
			m_drawPreviewExpander = false;
			m_drawPreview = true;
			m_showPreview = true;
			m_autoDrawInternalPortData = false;
			m_textLabelWidth = 125;
			//m_propertyInspectorName = "Triplanar Sampler";
			m_previewShaderGUID = "8723015ec59743143aadfbe480e34391";
		}

		public void ReadPropertiesData()
		{
			// Top
			if( UIUtils.IsUniformNameAvailable( m_tempTopName ) )
			{
				UIUtils.ReleaseUniformName( UniqueId, m_topTexture.PropertyName );
				if( !string.IsNullOrEmpty( m_tempTopInspectorName ) )
				{
					m_topTexture.SetInspectorName( m_tempTopInspectorName );
				}
				if( !string.IsNullOrEmpty( m_tempTopName ) )
					m_topTexture.SetPropertyName( m_tempTopName );
				UIUtils.RegisterUniformName( UniqueId, m_topTexture.PropertyName );
			}
			m_topTexture.DefaultTextureValue = m_tempTopDefaultValue;
			m_topTexture.OrderIndex = m_tempTopOrderIndex;
			m_topTexture.DefaultValue = m_tempTopDefaultTexture;
			//m_topTexture.SetMaterialMode( UIUtils.CurrentWindow.CurrentGraph.CurrentMaterial, true );

			// Mid
			if( UIUtils.IsUniformNameAvailable( m_tempMidName ) )
			{
				UIUtils.ReleaseUniformName( UniqueId, m_midTexture.PropertyName );
				if( !string.IsNullOrEmpty( m_tempMidInspectorName ) )
					m_midTexture.SetInspectorName( m_tempMidInspectorName );
				if( !string.IsNullOrEmpty( m_tempMidName ) )
					m_midTexture.SetPropertyName( m_tempMidName );
				UIUtils.RegisterUniformName( UniqueId, m_midTexture.PropertyName );
			}
			m_midTexture.DefaultTextureValue = m_tempMidDefaultValue;
			m_midTexture.OrderIndex = m_tempMidOrderIndex;
			m_midTexture.DefaultValue = m_tempMidDefaultTexture;

			// Bot
			if( UIUtils.IsUniformNameAvailable( m_tempBotName ) )
			{
				UIUtils.ReleaseUniformName( UniqueId, m_botTexture.PropertyName );
				if( !string.IsNullOrEmpty( m_tempBotInspectorName ) )
					m_botTexture.SetInspectorName( m_tempBotInspectorName );
				if( !string.IsNullOrEmpty( m_tempBotName ) )
					m_botTexture.SetPropertyName( m_tempBotName );
				UIUtils.RegisterUniformName( UniqueId, m_botTexture.PropertyName );
			}
			m_botTexture.DefaultTextureValue = m_tempBotDefaultValue;
			m_botTexture.OrderIndex = m_tempBotOrderIndex;
			m_botTexture.DefaultValue = m_tempBotDefaultTexture;
		}

		public override void SetMaterialMode( Material mat, bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat, fetchMaterialValues );

			if( !m_texturesInitialize )
				return;

			m_topTexture.SetMaterialMode( mat, fetchMaterialValues );
			m_midTexture.SetMaterialMode( mat, fetchMaterialValues );
			m_botTexture.SetMaterialMode( mat, fetchMaterialValues );
		}

		public void Init()
		{
			if( m_texturesInitialize )
				return;
			else
				m_texturesInitialize = true;

			// Top
			if( m_topTexture == null )
			{
				m_topTexture = ScriptableObject.CreateInstance<TexturePropertyNode>();
			}
			m_topTexture.ContainerGraph = ContainerGraph;
			m_topTexture.CustomPrefix = "Top Texture ";
			m_topTexture.UniqueId = UniqueId;
			m_topTexture.DrawAutocast = false;
			m_topTexture.CurrentParameterType = PropertyType.Property;

			// Mid
			if( m_midTexture == null )
			{
				m_midTexture = ScriptableObject.CreateInstance<TexturePropertyNode>();
			}
			m_midTexture.ContainerGraph = ContainerGraph;
			m_midTexture.CustomPrefix = "Mid Texture ";
			m_midTexture.UniqueId = UniqueId;
			m_midTexture.DrawAutocast = false;
			m_midTexture.CurrentParameterType = PropertyType.Property;

			// Bot
			if( m_botTexture == null )
			{
				m_botTexture = ScriptableObject.CreateInstance<TexturePropertyNode>();
			}
			m_botTexture.ContainerGraph = ContainerGraph;
			m_botTexture.CustomPrefix = "Bot Texture ";
			m_botTexture.UniqueId = UniqueId;
			m_botTexture.DrawAutocast = false;
			m_botTexture.CurrentParameterType = PropertyType.Property;

			if( m_materialMode )
				SetDelayedMaterialMode( ContainerGraph.CurrentMaterial );

			if( m_nodeAttribs != null )
				m_uniqueName = m_nodeAttribs.Name + UniqueId;

			ConfigurePorts();

			ReRegisterPorts();
		}

		public override void Destroy()
		{
			base.Destroy();

			//UIUtils.UnregisterPropertyNode( m_topTexture );
			//UIUtils.UnregisterTexturePropertyNode( m_topTexture );

			//UIUtils.UnregisterPropertyNode( m_midTexture );
			//UIUtils.UnregisterTexturePropertyNode( m_midTexture );

			//UIUtils.UnregisterPropertyNode( m_botTexture );
			//UIUtils.UnregisterTexturePropertyNode( m_botTexture );
			if( m_topTexture != null )
				m_topTexture.Destroy();
			m_topTexture = null;
			if( m_midTexture != null )
				m_midTexture.Destroy();
			m_midTexture = null;
			if( m_botTexture != null )
				m_botTexture.Destroy();
			m_botTexture = null;

			m_tempTopDefaultTexture = null;
			m_tempMidDefaultTexture = null;
			m_tempBotDefaultTexture = null;

			m_topTexPort = null;
			m_midTexPort = null;
			m_botTexPort = null;
			m_tilingPort = null;
			m_falloffPort = null;
			m_topIndexPort = null;
			m_midIndexPort = null;
			m_botIndexPort = null;
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();
			if( m_topTexture == null )
				return;


			if( m_topTexPort.IsConnected )
			{
				PreviewMaterial.SetTexture( "_A", m_topTexPort.InputPreviewTexture );
			}
			else
			{
				PreviewMaterial.SetTexture( "_A", m_topTexture.Value );
			}
			if( m_selectedTriplanarType == TriplanarType.Cylindrical && m_midTexture != null )
			{
				if( m_midTexPort.IsConnected )
					PreviewMaterial.SetTexture( "_B", m_midTexPort.InputPreviewTexture );
				else
					PreviewMaterial.SetTexture( "_B", m_midTexture.Value );
				if( m_botTexPort.IsConnected )
					PreviewMaterial.SetTexture( "_C", m_botTexPort.InputPreviewTexture );
				else
					PreviewMaterial.SetTexture( "_C", m_botTexture.Value );
			}

			PreviewMaterial.SetFloat( "_IsNormal", ( m_normalCorrection ? 1 : 0 ) );
			PreviewMaterial.SetFloat( "_IsSpherical", ( m_selectedTriplanarType == TriplanarType.Spherical ? 1 : 0 ) );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			if( m_texturesInitialize )
				ReRegisterPorts();
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			if( m_texturesInitialize )
				ReRegisterPorts();
		}

		public void ReRegisterPorts()
		{
			if( m_topTexPort.IsConnected )
			{
				UIUtils.UnregisterPropertyNode( m_topTexture );
				UIUtils.UnregisterTexturePropertyNode( m_topTexture );
			}
			else if( m_topTexPort.Visible )
			{
				UIUtils.RegisterPropertyNode( m_topTexture );
				UIUtils.RegisterTexturePropertyNode( m_topTexture );
			}

			if( m_midTexPort.IsConnected || m_selectedTriplanarType == TriplanarType.Spherical )
			{
				UIUtils.UnregisterPropertyNode( m_midTexture );
				UIUtils.UnregisterTexturePropertyNode( m_midTexture );
			}
			else if( m_midTexPort.Visible && m_selectedTriplanarType == TriplanarType.Cylindrical )
			{
				UIUtils.RegisterPropertyNode( m_midTexture );
				UIUtils.RegisterTexturePropertyNode( m_midTexture );
			}

			if( m_botTexPort.IsConnected || m_selectedTriplanarType == TriplanarType.Spherical )
			{
				UIUtils.UnregisterPropertyNode( m_botTexture );
				UIUtils.UnregisterTexturePropertyNode( m_botTexture );
			}
			else if( m_botTexPort.Visible && m_selectedTriplanarType == TriplanarType.Cylindrical )
			{
				UIUtils.RegisterPropertyNode( m_botTexture );
				UIUtils.RegisterTexturePropertyNode( m_botTexture );
			}
		}

		public void ConfigurePorts()
		{
			switch( m_selectedTriplanarType )
			{
				case TriplanarType.Spherical:
				m_topTexPort.Name = "Tex";
				m_midTexPort.Visible = false;
				m_botTexPort.Visible = false;
				m_scalePort.ChangeType( WirePortDataType.FLOAT, false );
				break;
				case TriplanarType.Cylindrical:
				m_topTexPort.Name = "Top";
				m_midTexPort.Visible = true;
				m_botTexPort.Visible = true;
				m_scalePort.ChangeType( WirePortDataType.FLOAT3, false );
				break;
			}

			if( m_normalCorrection )
			{
				m_outputPorts[ 0 ].ChangeProperties( "XYZ", WirePortDataType.FLOAT3, false );
				m_outputPorts[ 1 ].ChangeProperties( "X", WirePortDataType.FLOAT, false );
				m_outputPorts[ 2 ].ChangeProperties( "Y", WirePortDataType.FLOAT, false );
				m_outputPorts[ 3 ].ChangeProperties( "Z", WirePortDataType.FLOAT, false );

				m_outputPorts[ 4 ].Visible = false;

				m_scalePort.Visible = true;
			}
			else
			{
				m_outputPorts[ 0 ].ChangeProperties( "RGBA", WirePortDataType.FLOAT4, false );
				m_outputPorts[ 1 ].ChangeProperties( "R", WirePortDataType.FLOAT, false );
				m_outputPorts[ 2 ].ChangeProperties( "G", WirePortDataType.FLOAT, false );
				m_outputPorts[ 3 ].ChangeProperties( "B", WirePortDataType.FLOAT, false );
				m_outputPorts[ 4 ].ChangeProperties( "A", WirePortDataType.FLOAT, false );

				m_outputPorts[ 4 ].Visible = true;

				m_scalePort.Visible = false;
			}

			if( m_arraySupport )
			{
				m_topIndexPort.Visible = true;
				if( m_selectedTriplanarType == TriplanarType.Cylindrical )
				{
					m_midIndexPort.Visible = true;
					m_botIndexPort.Visible = true;
				}
			}
			else
			{
				m_topIndexPort.Visible = false;
				m_midIndexPort.Visible = false;
				m_botIndexPort.Visible = false;
			}

			m_outputPorts[ 0 ].DirtyLabelSize = true;
			m_sizeIsDirty = true;
		}

		public override void PropagateNodeData( NodeData nodeData, ref MasterNodeDataCollector dataCollector )
		{
			base.PropagateNodeData( nodeData, ref dataCollector );
			dataCollector.DirtyNormal = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, "Parameters", DrawMainOptions );
			DrawInternalDataGroup();
			if( m_selectedTriplanarType == TriplanarType.Spherical )
				NodeUtils.DrawPropertyGroup( ref m_topTextureFoldout, "Texture", DrawTopTextureOptions );
			else
				NodeUtils.DrawPropertyGroup( ref m_topTextureFoldout, "Top Texture", DrawTopTextureOptions );

			if( m_selectedTriplanarType == TriplanarType.Cylindrical )
			{
				NodeUtils.DrawPropertyGroup( ref m_midTextureFoldout, "Middle Texture", DrawMidTextureOptions );
				NodeUtils.DrawPropertyGroup( ref m_botTextureFoldout, "Bottom Texture", DrawBotTextureOptions );
			}
		}

		void DrawMainOptions()
		{
			EditorGUI.BeginChangeCheck();
			m_propertyInspectorName = EditorGUILayoutTextField( "Name", m_propertyInspectorName );

			m_selectedTriplanarType = (TriplanarType)EditorGUILayoutEnumPopup( "Mapping", m_selectedTriplanarType );

			m_selectedTriplanarSpace = (TriplanarSpace)EditorGUILayoutEnumPopup( "Space", m_selectedTriplanarSpace );

			m_normalCorrection = EditorGUILayoutToggle( "Normal Map", m_normalCorrection );

			m_arraySupport = EditorGUILayoutToggle( "Use Texture Array", m_arraySupport );
			if( m_arraySupport )
				EditorGUILayout.HelpBox( "Please connect all texture ports to a Texture Object node with a texture array asset for this option to work correctly", MessageType.Info );

			if( EditorGUI.EndChangeCheck() )
			{
				SetTitleText( m_propertyInspectorName );
				ConfigurePorts();
				ReRegisterPorts();
			}
		}

		void DrawTopTextureOptions()
		{
			EditorGUI.BeginChangeCheck();
			m_topTexture.ShowPropertyInspectorNameGUI();
			m_topTexture.ShowPropertyNameGUI( true );
			m_topTexture.ShowToolbar();
			if( EditorGUI.EndChangeCheck() )
			{
				m_topTexture.BeginPropertyFromInspectorCheck();
				if( m_materialMode )
					m_requireMaterialUpdate = true;
			}

			m_topTexture.CheckPropertyFromInspector();
		}

		void DrawMidTextureOptions()
		{
			if( m_midTexture == null )
				return;
			EditorGUI.BeginChangeCheck();
			m_midTexture.ShowPropertyInspectorNameGUI();
			m_midTexture.ShowPropertyNameGUI( true );
			m_midTexture.ShowToolbar();
			if( EditorGUI.EndChangeCheck() )
			{
				m_midTexture.BeginPropertyFromInspectorCheck();
				if( m_materialMode )
					m_requireMaterialUpdate = true;
			}

			m_midTexture.CheckPropertyFromInspector();
		}

		void DrawBotTextureOptions()
		{
			if( m_botTexture == null )
				return;

			EditorGUI.BeginChangeCheck();
			m_botTexture.ShowPropertyInspectorNameGUI();
			m_botTexture.ShowPropertyNameGUI( true );
			m_botTexture.ShowToolbar();
			if( EditorGUI.EndChangeCheck() )
			{
				m_botTexture.BeginPropertyFromInspectorCheck();
				if( m_materialMode )
					m_requireMaterialUpdate = true;
			}

			m_botTexture.CheckPropertyFromInspector();
		}

		public override void OnEnable()
		{
			base.OnEnable();
			//if( !m_afterDeserialize )
			//Init(); //Generate texture properties
			//else
			//m_afterDeserialize = false;

			//if( m_topTexture != null )
			//	m_topTexture.ReRegisterName = true;

			//if( m_selectedTriplanarType == TriplanarType.Cylindrical )
			//{
			//	if( m_midTexture != null )
			//		m_midTexture.ReRegisterName = true;

			//	if( m_botTexture != null )
			//		m_botTexture.ReRegisterName = true;
			//}
		}

		//bool m_afterDeserialize = false;

		//public override void OnAfterDeserialize()
		//{
		//	base.OnAfterDeserialize();
		//	m_afterDeserialize = true;
		//}


		public override void OnNodeLogicUpdate( DrawInfo drawInfo )
		{
			base.OnNodeLogicUpdate( drawInfo );

			Init();

			if( m_topTexture.ReRegisterName )
			{
				m_topTexture.ReRegisterName = false;
				UIUtils.RegisterUniformName( UniqueId, m_topTexture.PropertyName );
			}

			m_topTexture.CheckDelayedDirtyProperty();
			m_topTexture.CheckPropertyFromInspector();

			if( m_selectedTriplanarType == TriplanarType.Cylindrical )
			{
				if( m_midTexture.ReRegisterName )
				{
					m_midTexture.ReRegisterName = false;
					UIUtils.RegisterUniformName( UniqueId, m_midTexture.PropertyName );
				}

				m_midTexture.CheckDelayedDirtyProperty();
				m_midTexture.CheckPropertyFromInspector();

				if( m_botTexture.ReRegisterName )
				{
					m_botTexture.ReRegisterName = false;
					UIUtils.RegisterUniformName( UniqueId, m_botTexture.PropertyName );
				}

				m_botTexture.CheckDelayedDirtyProperty();
				m_botTexture.CheckPropertyFromInspector();
			}
		}

		public override void OnNodeLayout( DrawInfo drawInfo )
		{
			base.OnNodeLayout( drawInfo );

			m_allPicker = m_previewRect;
			m_allPicker.x -= 43 * drawInfo.InvertedZoom;
			m_allPicker.width = 43 * drawInfo.InvertedZoom;

			m_startPicker = m_previewRect;
			m_startPicker.x -= 43 * drawInfo.InvertedZoom;
			m_startPicker.width = 43 * drawInfo.InvertedZoom;
			m_startPicker.height = 43 * drawInfo.InvertedZoom;

			m_pickerButton = m_startPicker;
			m_pickerButton.width = 30 * drawInfo.InvertedZoom;
			m_pickerButton.x = m_startPicker.xMax - m_pickerButton.width - 2;
			m_pickerButton.height = 10 * drawInfo.InvertedZoom;
			m_pickerButton.y = m_startPicker.yMax - m_pickerButton.height - 2;
		}



		public override void DrawGUIControls( DrawInfo drawInfo )
		{
			base.DrawGUIControls( drawInfo );

			if( !( drawInfo.CurrentEventType == EventType.MouseDown || drawInfo.CurrentEventType == EventType.MouseUp || drawInfo.CurrentEventType == EventType.ExecuteCommand || drawInfo.CurrentEventType == EventType.DragPerform ) )
				return;

			bool insideBox = m_allPicker.Contains( drawInfo.MousePosition );

			if( insideBox )
			{
				m_editing = true;
			}
			else if( m_editing && !insideBox && drawInfo.CurrentEventType != EventType.ExecuteCommand )
			{
				GUI.FocusControl( null );
				m_editing = false;
			}
		}
		private int m_pickId = 0;
		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			Rect pickerButtonClone = m_pickerButton;
			Rect startPickerClone = m_startPicker;

			if( m_editing )
			{
				if( GUI.Button( pickerButtonClone, string.Empty, GUIStyle.none ) )
				{
					int controlID = EditorGUIUtility.GetControlID( FocusType.Passive );
					EditorGUIUtility.ShowObjectPicker<Texture2D>( m_topTexture.Value, false, "", controlID );
					m_pickId = 0;
				}

				if( m_selectedTriplanarType == TriplanarType.Cylindrical )
				{
					pickerButtonClone.y += startPickerClone.height;
					if( GUI.Button( pickerButtonClone, string.Empty, GUIStyle.none ) )
					{
						int controlID = EditorGUIUtility.GetControlID( FocusType.Passive );
						EditorGUIUtility.ShowObjectPicker<Texture2D>( m_midTexture.Value, false, "", controlID );
						m_pickId = 1;
					}

					pickerButtonClone.y += startPickerClone.height;
					if( GUI.Button( pickerButtonClone, string.Empty, GUIStyle.none ) )
					{
						int controlID = EditorGUIUtility.GetControlID( FocusType.Passive );
						EditorGUIUtility.ShowObjectPicker<Texture2D>( m_botTexture.Value, false, "", controlID );
						m_pickId = 2;
					}
				}

				string commandName = Event.current.commandName;
				UnityEngine.Object newValue = null;
				if( commandName.Equals( "ObjectSelectorUpdated" ) || commandName.Equals( "ObjectSelectorClosed" ) )
				{
					newValue = EditorGUIUtility.GetObjectPickerObject();
					if( m_pickId == 2 )
					{
						if( newValue != (UnityEngine.Object)m_botTexture.Value )
						{
							UndoRecordObject( "Changing value EditorGUIObjectField on node Triplanar Node" );
							m_botTexture.Value = newValue != null ? (Texture2D)newValue : null;

							if( m_materialMode )
								m_requireMaterialUpdate = true;
						}
					}
					else if( m_pickId == 1 )
					{
						if( newValue != (UnityEngine.Object)m_midTexture.Value )
						{
							UndoRecordObject( "Changing value EditorGUIObjectField on node Triplanar Node" );
							m_midTexture.Value = newValue != null ? (Texture2D)newValue : null;

							if( m_materialMode )
								m_requireMaterialUpdate = true;
						}
					}
					else
					{
						if( newValue != (UnityEngine.Object)m_topTexture.Value )
						{
							UndoRecordObject( "Changing value EditorGUIObjectField on node Triplanar Node" );
							m_topTexture.Value = newValue != null ? (Texture2D)newValue : null;

							if( m_materialMode )
								m_requireMaterialUpdate = true;
						}
					}

					if( commandName.Equals( "ObjectSelectorClosed" ) )
						m_editing = false;
				}

				if( GUI.Button( startPickerClone, string.Empty, GUIStyle.none ) )
				{
					if( m_topTexPort.IsConnected )
					{
						UIUtils.FocusOnNode( m_topTexPort.GetOutputNode( 0 ), 1, true );
					}
					else
					{
						if( m_topTexture.Value != null )
						{
							Selection.activeObject = m_topTexture.Value;
							EditorGUIUtility.PingObject( Selection.activeObject );
						}
					}
					m_editing = false;
				}

				if( m_selectedTriplanarType == TriplanarType.Cylindrical )
				{
					startPickerClone.y += startPickerClone.height;
					if( GUI.Button( startPickerClone, string.Empty, GUIStyle.none ) )
					{
						if( m_midTexPort.IsConnected )
						{
							UIUtils.FocusOnNode( m_midTexPort.GetOutputNode( 0 ), 1, true );
						}
						else
						{
							if( m_midTexture.Value != null )
							{
								Selection.activeObject = m_midTexture.Value;
								EditorGUIUtility.PingObject( Selection.activeObject );
							}
						}
						m_editing = false;
					}

					startPickerClone.y += startPickerClone.height;
					if( GUI.Button( startPickerClone, string.Empty, GUIStyle.none ) )
					{
						if( m_botTexPort.IsConnected )
						{
							UIUtils.FocusOnNode( m_botTexPort.GetOutputNode( 0 ), 1, true );
						}
						else
						{
							if( m_botTexture.Value != null )
							{
								Selection.activeObject = m_botTexture.Value;
								EditorGUIUtility.PingObject( Selection.activeObject );
							}
						}
						m_editing = false;
					}
				}
			}

			pickerButtonClone = m_pickerButton;
			startPickerClone = m_startPicker;

			if( drawInfo.CurrentEventType == EventType.Repaint )
			{
				// Top
				if( m_topTexPort.IsConnected )
				{
					EditorGUI.DrawPreviewTexture( startPickerClone, m_topTexPort.GetOutputConnection( 0 ).OutputPreviewTexture, null, ScaleMode.ScaleAndCrop );
				}
				else if( m_topTexture.Value != null )
				{
					EditorGUI.DrawPreviewTexture( startPickerClone, m_topTexture.Value, null, ScaleMode.ScaleAndCrop );
					if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
						GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
				}
				else
				{
					GUI.Label( startPickerClone, string.Empty, UIUtils.ObjectFieldThumb );
					if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
					{
						GUI.Label( startPickerClone, "None (Texture2D)", UIUtils.MiniObjectFieldThumbOverlay );
						GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
					}
				}
				GUI.Label( startPickerClone, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );

				if( m_selectedTriplanarType == TriplanarType.Cylindrical )
				{
					// Mid
					startPickerClone.y += startPickerClone.height;
					pickerButtonClone.y += startPickerClone.height;
					if( m_midTexPort.IsConnected )
					{
						EditorGUI.DrawPreviewTexture( startPickerClone, m_midTexPort.GetOutputConnection( 0 ).OutputPreviewTexture, null, ScaleMode.ScaleAndCrop );
					}
					else if( m_midTexture.Value != null )
					{
						EditorGUI.DrawPreviewTexture( startPickerClone, m_midTexture.Value, null, ScaleMode.ScaleAndCrop );
						if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
							GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
					}
					else
					{
						GUI.Label( startPickerClone, string.Empty, UIUtils.ObjectFieldThumb );
						if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
						{
							GUI.Label( startPickerClone, "None (Texture2D)", UIUtils.MiniObjectFieldThumbOverlay );
							GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
						}
					}
					GUI.Label( startPickerClone, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );

					// Bot
					startPickerClone.y += startPickerClone.height;
					startPickerClone.height = 42 * drawInfo.InvertedZoom;
					pickerButtonClone.y += startPickerClone.height;
					if( m_botTexPort.IsConnected )
					{
						EditorGUI.DrawPreviewTexture( startPickerClone, m_botTexPort.GetOutputConnection( 0 ).OutputPreviewTexture, null, ScaleMode.ScaleAndCrop );
					}
					else if( m_botTexture.Value != null )
					{
						EditorGUI.DrawPreviewTexture( startPickerClone, m_botTexture.Value, null, ScaleMode.ScaleAndCrop );
						if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
							GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
					}
					else
					{
						GUI.Label( startPickerClone, string.Empty, UIUtils.ObjectFieldThumb );
						if( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
						{
							GUI.Label( startPickerClone, "None (Texture2D)", UIUtils.MiniObjectFieldThumbOverlay );
							GUI.Label( pickerButtonClone, "Select", UIUtils.MiniSamplerButton );
						}
					}
					GUI.Label( startPickerClone, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
				}
			}
		}

		public override void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			if( currentMousePos2D.y - m_globalPosition.y > Constants.NODE_HEADER_HEIGHT + Constants.NODE_HEADER_EXTRA_HEIGHT )
			{
				ContainerGraph.ParentWindow.ParametersWindow.IsMaximized = !ContainerGraph.ParentWindow.ParametersWindow.IsMaximized;
			}
			else
			{
				m_editPropertyNameMode = true;
				GUI.FocusControl( m_uniqueName );
				TextEditor te = (TextEditor)GUIUtility.GetStateObject( typeof( TextEditor ), GUIUtility.keyboardControl );
				if( te != null )
				{
					te.SelectAll();
				}
			}
		}

		public override void OnNodeSelected( bool value )
		{
			base.OnNodeSelected( value );
			if( !value )
				m_editPropertyNameMode = false;
		}

		public override void DrawTitle( Rect titlePos )
		{
			if( m_editPropertyNameMode )
			{
				titlePos.height = Constants.NODE_HEADER_HEIGHT;
				EditorGUI.BeginChangeCheck();
				GUI.SetNextControlName( m_uniqueName );
				m_propertyInspectorName = GUITextField( titlePos, m_propertyInspectorName, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
				if( EditorGUI.EndChangeCheck() )
				{
					SetTitleText( m_propertyInspectorName );
				}

				if( Event.current.isKey && ( Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter ) )
				{
					m_editPropertyNameMode = false;
					GUIUtility.keyboardControl = 0;
				}
			}
			else
			{
				base.DrawTitle( titlePos );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			//ConfigureFunctions();
			dataCollector.AddPropertyNode( m_topTexture );
			dataCollector.AddPropertyNode( m_midTexture );
			dataCollector.AddPropertyNode( m_botTexture );

			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Tessellation || dataCollector.PortCategory == MasterNodePortCategory.Vertex );

			string texTop = string.Empty;
			string texMid = string.Empty;
			string texBot = string.Empty;

			if( m_topTexPort.IsConnected )
			{
				texTop = m_topTexPort.GeneratePortInstructions( ref dataCollector );
			}
			else
			{
				dataCollector.AddToUniforms( UniqueId, m_topTexture.GetTexture2DUniformValue() );
				dataCollector.AddToProperties( UniqueId, m_topTexture.GetTexture2DPropertyValue(), m_topTexture.OrderIndex );
				texTop = m_topTexture.PropertyName;
			}

			if( m_selectedTriplanarType == TriplanarType.Spherical )
			{
				texMid = texTop;
				texBot = texTop;
			}
			else
			{
				if( m_midTexPort.IsConnected )
				{
					texMid = m_midTexPort.GeneratePortInstructions( ref dataCollector );
				}
				else
				{
					dataCollector.AddToUniforms( UniqueId, m_midTexture.GetTexture2DUniformValue() );
					dataCollector.AddToProperties( UniqueId, m_midTexture.GetTexture2DPropertyValue(), m_midTexture.OrderIndex );
					texMid = m_midTexture.PropertyName;
				}

				if( m_botTexPort.IsConnected )
				{
					texBot = m_botTexPort.GeneratePortInstructions( ref dataCollector );
				}
				else
				{
					dataCollector.AddToUniforms( UniqueId, m_botTexture.GetTexture2DUniformValue() );
					dataCollector.AddToProperties( UniqueId, m_botTexture.GetTexture2DPropertyValue(), m_botTexture.OrderIndex );
					texBot = m_botTexture.PropertyName;
				}
			}

			if( !isVertex )
			{
				dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_POS );
				dataCollector.AddToInput( UniqueId, SurfaceInputs.WORLD_NORMAL, m_currentPrecisionType );
				dataCollector.AddToInput( UniqueId, SurfaceInputs.INTERNALDATA, addSemiColon: false );
				dataCollector.ForceNormal = true;
			}

			string topIndex = "0";
			string midIndex = "0";
			string botIndex = "0";

			if( m_arraySupport && ( !m_topTexPort.IsConnected && m_selectedTriplanarType == TriplanarType.Spherical
				|| m_selectedTriplanarType == TriplanarType.Cylindrical && !( m_topTexPort.IsConnected && m_midTexPort.IsConnected && m_botTexPort.IsConnected ) ) )
				m_arraySupport = false;

			if( m_arraySupport )
			{
				topIndex = m_topIndexPort.GeneratePortInstructions( ref dataCollector );
				if( m_selectedTriplanarType == TriplanarType.Cylindrical )
				{
					midIndex = m_midIndexPort.GeneratePortInstructions( ref dataCollector );
					botIndex = m_botIndexPort.GeneratePortInstructions( ref dataCollector );
				}
			}

			string tilling = m_tilingPort.GeneratePortInstructions( ref dataCollector );
			string falloff = m_falloffPort.GeneratePortInstructions( ref dataCollector );

			bool scaleNormals = false;
			if( m_scalePort.IsConnected || ( m_scalePort.IsConnected && ( m_scalePort.Vector3InternalData == Vector3.one || m_scalePort.FloatInternalData == 1 ) ) )
				scaleNormals = true;

			string samplingTriplanar = string.Empty;
			string headerID = string.Empty;
			string header = string.Empty;
			string callHeader = string.Empty;
			string samplers = string.Empty;
			string extraArguments = string.Empty;
			List<string> triplanarBody = new List<string>();

			triplanarBody.AddRange( m_functionSamplingBodyProj );
			if( m_selectedTriplanarType == TriplanarType.Spherical )
			{
				headerID += "S";
				samplers = m_arraySupport ? m_singularArrayTexture : m_singularTexture;

				triplanarBody.AddRange( m_functionSamplingBodySampSphere );

				if( m_normalCorrection )
				{
					headerID += "N";
					if( scaleNormals )
					{
						ConvertListTo( dataCollector, true, m_functionSamplingBodySignsSphereScale, triplanarBody );
						//triplanarBody.AddRange( m_functionSamplingBodySignsSphereScale );
					}
					else
					{
						ConvertListTo( dataCollector, false, m_functionSamplingBodySignsSphere, triplanarBody );
						//triplanarBody.AddRange( m_functionSamplingBodySignsSphere );
					}
					triplanarBody.AddRange( m_functionSamplingBodyReturnSphereNormalize );
				}
				else
				{
					triplanarBody.AddRange( m_functionSamplingBodyReturnSphere );
				}
			}
			else
			{
				headerID += "C";
				samplers = m_arraySupport ? m_topmidbotArrayTexture : m_topmidbotTexture;
				extraArguments = ", {7}, {8}";
				triplanarBody.AddRange( m_functionSamplingBodyNegProj );

				triplanarBody.AddRange( m_functionSamplingBodySampCylinder );

				if( m_normalCorrection )
				{
					headerID += "N";
					if( scaleNormals )
					{
						//triplanarBody.AddRange( m_functionSamplingBodySignsSphereScale );
						ConvertListTo( dataCollector, true, m_functionSamplingBodySignsSphereScale, triplanarBody );
						ConvertListTo( dataCollector, true, m_functionSamplingBodySignsCylinderScale, triplanarBody );
						//triplanarBody.AddRange( m_functionSamplingBodySignsCylinderScale );
					}
					else
					{
						//triplanarBody.AddRange( m_functionSamplingBodySignsSphere );
						ConvertListTo( dataCollector, false, m_functionSamplingBodySignsSphere, triplanarBody );
						ConvertListTo( dataCollector, false, m_functionSamplingBodySignsCylinder, triplanarBody );
						//triplanarBody.AddRange( m_functionSamplingBodySignsCylinder );
					}
					triplanarBody.AddRange( m_functionSamplingBodyReturnCylinderNormalize );
				}
				else
				{
					triplanarBody.AddRange( m_functionSamplingBodyReturnCylinder );
				}
			}

			if( isVertex )
			{
				if( m_arraySupport )
				{
					headerID += "VA";
					for( int i = 0; i < triplanarBody.Count; i++ )
						triplanarBody[ i ] = string.Format( triplanarBody[ i ], "UNITY_SAMPLE_TEX2DARRAY_LOD", "float3( ", ", 0 ), index.x", ", 0 ), index.y", ", 0 ), index.z" );
				}
				else
				{
					headerID += "V";
					for( int i = 0; i < triplanarBody.Count; i++ )
						triplanarBody[ i ] = string.Format( triplanarBody[ i ], "tex2Dlod", "float4( ", ", 0, 0 )", ", 0, 0 )", ", 0, 0 )" );
				}
			}
			else
			{
				if( m_arraySupport )
				{
					headerID += "FA";
					for( int i = 0; i < triplanarBody.Count; i++ )
						triplanarBody[ i ] = string.Format( triplanarBody[ i ], "UNITY_SAMPLE_TEX2DARRAY", "float3( ", ", index.x )", ", index.y )", ", index.z )" );
				}
				else
				{
					headerID += "F";
					for( int i = 0; i < triplanarBody.Count; i++ )
						triplanarBody[ i ] = string.Format( triplanarBody[ i ], "tex2D", "", "", "", "" );
				}
			}

			string type = UIUtils.WirePortToCgType( m_outputPorts[ 0 ].DataType );
			header = string.Format( m_functionHeader, type, headerID, samplers );
			callHeader = string.Format( m_functionCall, headerID, "{0}, {1}, {2}, {3}, {4}, {5}, {6}" + extraArguments );

			IOUtils.AddFunctionHeader( ref samplingTriplanar, header );
			foreach( string line in triplanarBody )
				IOUtils.AddFunctionLine( ref samplingTriplanar, line );
			IOUtils.CloseFunctionBody( ref samplingTriplanar );

			string pos = GeneratorUtils.GenerateWorldPosition( ref dataCollector, UniqueId );
			string norm = GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
			string worldToTangent = string.Empty;
			if( m_normalCorrection )
				worldToTangent = GeneratorUtils.GenerateWorldToTangentMatrix( ref dataCollector, UniqueId, m_currentPrecisionType );

			if( m_selectedTriplanarSpace == TriplanarSpace.Object )
			{
				string worldToObjectMatrix = ( dataCollector.IsTemplate && dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD ) ? "GetWorldToObjectMatrix()" : "unity_WorldToObject";

				if( m_normalCorrection )
				{
					dataCollector.AddLocalVariable( UniqueId, "float3 localTangent = mul( "+ worldToObjectMatrix + " , float4( " + GeneratorUtils.WorldTangentStr + ", 0 ) );" );
					dataCollector.AddLocalVariable( UniqueId, "float3 localBitangent = mul( " + worldToObjectMatrix + ", float4( " + GeneratorUtils.WorldBitangentStr + ", 0 ) );" );
					dataCollector.AddLocalVariable( UniqueId, "float3 localNormal = mul( " + worldToObjectMatrix + ", float4( " + GeneratorUtils.WorldNormalStr + ", 0 ) );" );
					norm = "localNormal";
					dataCollector.AddLocalVariable( UniqueId, "float3x3 objectToTangent = float3x3(localTangent, localBitangent, localNormal);" );
					dataCollector.AddLocalVariable( UniqueId, "float3 localPos = mul( " + worldToObjectMatrix + ", float4( " + pos + ", 1 ) );" );
					pos = "localPos";
					worldToTangent = "objectToTangent";
				}
				else
				{
					dataCollector.AddToLocalVariables( dataCollector.PortCategory, UniqueId, "float3 localPos = mul( " + worldToObjectMatrix + ", float4( " + pos + ", 1 ) );" );
					pos = "localPos";
					dataCollector.AddToLocalVariables( dataCollector.PortCategory, UniqueId, "float3 localNormal = mul( " + worldToObjectMatrix + ", float4( " + norm + ", 0 ) );" );
					norm = "localNormal";
				}
			}

			string call = string.Empty;

			if( m_arraySupport )
			{
				texTop = "UNITY_PASS_TEX2DARRAY(" + texTop + ")";
				texMid = "UNITY_PASS_TEX2DARRAY(" + texMid + ")";
				texBot = "UNITY_PASS_TEX2DARRAY(" + texBot + ")";
			}

			string normalScale = m_scalePort.GeneratePortInstructions( ref dataCollector );

			if( m_selectedTriplanarType == TriplanarType.Spherical )
				call = dataCollector.AddFunctions( callHeader, samplingTriplanar, texTop, pos, norm, falloff, tilling, normalScale, topIndex );
			else
				call = dataCollector.AddFunctions( callHeader, samplingTriplanar, texTop, texMid, texBot, pos, norm, falloff, tilling, normalScale, "float3(" + topIndex + "," + midIndex + "," + botIndex + ")" );
			dataCollector.AddToLocalVariables( dataCollector.PortCategory, UniqueId, type + " triplanar" + OutputId + " = " + call + ";" );
			if( m_normalCorrection )
			{
				dataCollector.AddToLocalVariables( dataCollector.PortCategory, UniqueId, "float3 tanTriplanarNormal" + OutputId + " = mul( " + worldToTangent + ", triplanar" + OutputId + " );" );
				return GetOutputVectorItem( 0, outputId, "tanTriplanarNormal" + OutputId );
			}
			else
			{
				return GetOutputVectorItem( 0, outputId, "triplanar" + OutputId );
			}
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			m_topTexture.OnPropertyNameChanged();
			if( mat.HasProperty( m_topTexture.PropertyName ) && !InsideShaderFunction )
			{
				mat.SetTexture( m_topTexture.PropertyName, m_topTexture.MaterialValue );
			}

			m_midTexture.OnPropertyNameChanged();
			if( mat.HasProperty( m_midTexture.PropertyName ) && !InsideShaderFunction )
			{
				mat.SetTexture( m_midTexture.PropertyName, m_midTexture.MaterialValue );
			}

			m_botTexture.OnPropertyNameChanged();
			if( mat.HasProperty( m_botTexture.PropertyName ) && !InsideShaderFunction )
			{
				mat.SetTexture( m_botTexture.PropertyName, m_botTexture.MaterialValue );
			}
		}

		public void SetDelayedMaterialMode( Material mat )
		{
			m_topTexture.SetMaterialMode( mat, false );
			if( mat.HasProperty( m_topTexture.PropertyName ) )
			{
				m_topTexture.MaterialValue = mat.GetTexture( m_topTexture.PropertyName );
			}

			m_midTexture.SetMaterialMode( mat, false );
			if( mat.HasProperty( m_midTexture.PropertyName ) )
			{
				m_midTexture.MaterialValue = mat.GetTexture( m_midTexture.PropertyName );
			}

			m_botTexture.SetMaterialMode( mat, false );
			if( mat.HasProperty( m_botTexture.PropertyName ) )
			{
				m_botTexture.MaterialValue = mat.GetTexture( m_botTexture.PropertyName );
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			base.ForceUpdateFromMaterial( material );
			if( material.HasProperty( m_topTexture.PropertyName ) )
			{
				m_topTexture.MaterialValue = material.GetTexture( m_topTexture.PropertyName );
			}

			if( material.HasProperty( m_midTexture.PropertyName ) )
			{
				m_midTexture.MaterialValue = material.GetTexture( m_midTexture.PropertyName );
			}

			if( material.HasProperty( m_botTexture.PropertyName ) )
			{
				m_botTexture.MaterialValue = material.GetTexture( m_botTexture.PropertyName );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedTriplanarType = (TriplanarType)Enum.Parse( typeof( TriplanarType ), GetCurrentParam( ref nodeParams ) );
			m_selectedTriplanarSpace = (TriplanarSpace)Enum.Parse( typeof( TriplanarSpace ), GetCurrentParam( ref nodeParams ) );
			m_normalCorrection = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			m_tempTopInspectorName = GetCurrentParam( ref nodeParams );
			m_tempTopName = GetCurrentParam( ref nodeParams );
			m_tempTopDefaultValue = (TexturePropertyValues)Enum.Parse( typeof( TexturePropertyValues ), GetCurrentParam( ref nodeParams ) );
			m_tempTopOrderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_tempTopDefaultTexture = AssetDatabase.LoadAssetAtPath<Texture2D>( GetCurrentParam( ref nodeParams ) );

			m_tempMidInspectorName = GetCurrentParam( ref nodeParams );
			m_tempMidName = GetCurrentParam( ref nodeParams );
			m_tempMidDefaultValue = (TexturePropertyValues)Enum.Parse( typeof( TexturePropertyValues ), GetCurrentParam( ref nodeParams ) );
			m_tempMidOrderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_tempMidDefaultTexture = AssetDatabase.LoadAssetAtPath<Texture2D>( GetCurrentParam( ref nodeParams ) );

			m_tempBotInspectorName = GetCurrentParam( ref nodeParams );
			m_tempBotName = GetCurrentParam( ref nodeParams );
			m_tempBotDefaultValue = (TexturePropertyValues)Enum.Parse( typeof( TexturePropertyValues ), GetCurrentParam( ref nodeParams ) );
			m_tempBotOrderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_tempBotDefaultTexture = AssetDatabase.LoadAssetAtPath<Texture2D>( GetCurrentParam( ref nodeParams ) );

			if( UIUtils.CurrentShaderVersion() > 6102 )
				m_propertyInspectorName = GetCurrentParam( ref nodeParams );

			if( UIUtils.CurrentShaderVersion() > 13701 )
				m_arraySupport = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			SetTitleText( m_propertyInspectorName );

			ConfigurePorts();
		}

		public override void RefreshExternalReferences()
		{
			base.RefreshExternalReferences();

			Init();

			ReadPropertiesData();

			ConfigurePorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedTriplanarType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedTriplanarSpace );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_normalCorrection );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_topTexture.PropertyInspectorName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_topTexture.PropertyName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_topTexture.DefaultTextureValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_topTexture.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_topTexture.DefaultValue != null ) ? AssetDatabase.GetAssetPath( m_topTexture.DefaultValue ) : Constants.NoStringValue );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_midTexture.PropertyInspectorName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_midTexture.PropertyName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_midTexture.DefaultTextureValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_midTexture.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_midTexture.DefaultValue != null ) ? AssetDatabase.GetAssetPath( m_midTexture.DefaultValue ) : Constants.NoStringValue );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_botTexture.PropertyInspectorName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_botTexture.PropertyName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_botTexture.DefaultTextureValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_botTexture.OrderIndex.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_botTexture.DefaultValue != null ) ? AssetDatabase.GetAssetPath( m_botTexture.DefaultValue ) : Constants.NoStringValue );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_propertyInspectorName );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_arraySupport );
		}
		public override void RefreshOnUndo()
		{
			base.RefreshOnUndo();
			if( m_topTexture != null )
			{
				m_topTexture.BeginPropertyFromInspectorCheck();
			}

			if( m_midTexture != null )
			{
				m_midTexture.BeginPropertyFromInspectorCheck();
			}

			if( m_botTexture != null )
			{
				m_botTexture.BeginPropertyFromInspectorCheck();
			}
		}
	}
}
