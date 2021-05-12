// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

using System.Globalization;
using System.Text.RegularExpressions;

namespace AmplifyShaderEditor
{
	public enum ASEColorSpace
	{
		Auto,
		Gamma,
		Linear
	}

	public enum SurfaceInputs
	{
		DEPTH = 0,
		UV_COORDS,
		UV2_COORDS,
		VIEW_DIR,
		COLOR,
		SCREEN_POS,
		WORLD_POS,
		WORLD_REFL,
		WORLD_NORMAL,
		VFACE,
		INTERNALDATA
	}

	public enum CustomStyle
	{
		NodeWindowOff = 0,
		NodeWindowOn,
		NodeTitle,
		NodeHeader,
		CommentaryHeader,
		ShaderLibraryTitle,
		ShaderLibraryAddToList,
		ShaderLibraryRemoveFromList,
		ShaderLibraryOpenListed,
		ShaderLibrarySelectionAsTemplate,
		ShaderLibraryItem,
		CommentaryTitle,
		PortEmptyIcon,
		PortFullIcon,
		InputPortlabel,
		OutputPortLabel,
		CommentaryResizeButton,
		CommentaryResizeButtonInv,
		CommentaryBackground,
		MinimizeButton,
		MaximizeButton,
		NodePropertiesTitle,
		ShaderModeTitle,
		MaterialModeTitle,
		ShaderNoMaterialModeTitle,
		PropertyValuesTitle,
		ShaderModeNoShader,
		MainCanvasTitle,
		ShaderBorder,
		MaterialBorder,
		SamplerTextureRef,
		SamplerTextureIcon,
		CustomExpressionAddItem,
		CustomExpressionRemoveItem,
		CustomExpressionSmallAddItem,
		CustomExpressionSmallRemoveItem,
		ResetToDefaultInspectorButton,
		SliderStyle,
		ObjectPicker,
		NodePropertyPicker,
		NodePreviewExpander,
		NodePreviewCollapser,
		SamplerButton,
		SamplerFrame,
		CommentarySuperTitle,
		MiniButtonTopLeft,
		MiniButtonTopMid,
		MiniButtonTopRight,
		ShaderFunctionBorder,
		ShaderFunctionMode,
		RightShaderMode,
		FlatBackground,
		DocumentationLink,
		GraphButtonIcon,
		GraphButton,
		NodeWindowOffSquare,
		NodeHeaderSquare,
		NodeWindowOnSquare
	}

	public enum MasterNodePortCategory
	{
		Vertex = 1 << 0,
		Fragment = 1 << 1,
		Tessellation = 1 << 2,
		Debug = 1 << 3
	}

	public enum PortGenType
	{
		NonCustomLighting,
		//Normal = 1 << 1,
		//Emission = 1 << 2,
		//Metallic = 1 << 3,
		//Specular = 1 << 4,
		CustomLighting
	}

	public struct NodeData
	{
		public MasterNodePortCategory Category;
		public int OrderIndex;
		public int GraphDepth;
		public NodeData( MasterNodePortCategory category )
		{
			Category = category;
			OrderIndex = 0;
			GraphDepth = -1;
		}
	}

	public struct NodeCastInfo
	{
		public int NodeId;
		public int PortId;
		public NodeCastInfo( int nodeId, int portId )
		{
			NodeId = nodeId;
			PortId = portId;
		}
		public override string ToString()
		{
			return NodeId.ToString() + PortId.ToString();
		}
	};

	public struct ButtonClickId
	{
		public const int LeftMouseButton = 0;
		public const int RightMouseButton = 1;
		public const int MiddleMouseButton = 2;
	}

	public enum ASESelectionMode
	{
		Shader = 0,
		Material,
		ShaderFunction
	}

	public enum DrawOrder
	{
		Background,
		Default
	}

	public enum NodeConnectionStatus
	{
		Not_Connected = 0,
		Connected,
		Error,
		Island
	}

	public enum InteractionMode
	{
		Target,
		Other,
		Both
	}

	public enum FunctionNodeCategories
	{
		Custom,
		CameraAndScreen,
		ConstantsAndProperties,
		Functions,
		ImageEffects,
		Light,
		LogicalOperators,
		MathOperators,
		MatrixOperators,
		Miscellaneous,
		ObjectTransform,
		SurfaceData,
		Textures,
		Time,
		TrignometryOperators,
		UVCoordinates,
		VectorOperators,
		VertexData
	}

	public enum TransformSpace
	{
		Object = 0,
		World,
		View,
		Clip,
		Tangent
		//Screen??
	}

	public class UIUtils
	{
		public static int SerializeHelperCounter = 0;
		public static bool IgnoreDeselectAll = false;

		public static bool DirtyMask = true;
		public static bool Initialized = false;
		public static float HeaderMaxHeight;
		public static float CurrentHeaderHeight;
		public static GUISkin MainSkin = null;
		public static GUIStyle PlusStyle;
		public static GUIStyle MinusStyle;
		public static GUIStyle RangedFloatSliderStyle;
		public static GUIStyle RangedFloatSliderThumbStyle;
		public static GUIStyle SwitchNodePopUp;
		public static GUIStyle PropertyPopUp;
		public static GUIStyle ObjectField;
		public static GUIStyle PreviewExpander;
		public static GUIStyle PreviewCollapser;
		public static GUIStyle ObjectFieldThumb;
		public static GUIStyle ObjectFieldThumbOverlay;
		public static GUIStyle InspectorPopdropdownStyle;
		public static GUIStyle InspectorPopdropdownFallback;
		public static GUIStyle BoldErrorStyle;
		public static GUIStyle BoldWarningStyle;
		public static GUIStyle BoldInfoStyle;
		public static GUIStyle Separator;
		public static GUIStyle ToolbarSearchTextfield;
		public static GUIStyle ToolbarSearchCancelButton;
		public static GUIStyle MiniButtonTopLeft;
		public static GUIStyle MiniButtonTopMid;
		public static GUIStyle MiniButtonTopRight;

		public static GUIStyle CommentaryTitle;
		public static GUIStyle InputPortLabel;
		public static GUIStyle OutputPortLabel;

		public static GUIStyle MiniObjectFieldThumbOverlay;
		public static GUIStyle MiniSamplerButton;

		public static GUIStyle NodeWindowOffSquare;
		public static GUIStyle NodeHeaderSquare;
		public static GUIStyle NodeWindowOnSquare;
		public static GUIStyle InternalDataOnPort;
		public static GUIStyle InternalDataBackground;

		public static GUIStyle GraphButtonIcon;
		public static GUIStyle GraphButton;
		public static GUIStyle GraphDropDown;

		public static GUIStyle EmptyStyle = new GUIStyle();

		public static GUIStyle TooltipBox;
		public static GUIStyle Box;
		public static GUIStyle Button;
		public static GUIStyle TextArea;
		public static GUIStyle Label;
		public static GUIStyle Toggle;
		public static GUIStyle Textfield;

		public static GUIStyle UnZoomedNodeTitleStyle;
		public static GUIStyle UnZoomedPropertyValuesTitleStyle;
		public static GUIStyle UnZoomedInputPortStyle;
		public static GUIStyle UnZoomedOutputPortPortStyle;

		// Node Property Menu items
		public static GUIStyle MenuItemToggleStyle;
		public static GUIStyle MenuItemEnableStyle;
		public static GUIStyle MenuItemBackgroundStyle;
		public static GUIStyle MenuItemToolbarStyle;
		public static GUIStyle MenuItemInspectorDropdownStyle;

		public static GUIStyle FloatIntPickerONOFF;

		public static bool UsingProSkin = false;

		public static Texture ShaderIcon { get { return EditorGUIUtility.IconContent( "Shader Icon" ).image; } }
		public static Texture MaterialIcon { get { return EditorGUIUtility.IconContent( "Material Icon" ).image; } }

		//50be8291f9514914aa55c66c49da67cf
		public static Texture ShaderFunctionIcon { get { return AssetDatabase.LoadAssetAtPath<Texture>( AssetDatabase.GUIDToAssetPath( "50be8291f9514914aa55c66c49da67cf" ) ); } }

		public static Texture2D WireNodeSelection = null;
		public static Texture2D SliderButton = null;

		public static Texture2D SmallErrorIcon = null;
		public static Texture2D SmallWarningIcon = null;
		public static Texture2D SmallInfoIcon = null;

		public static Texture2D CheckmarkIcon = null;
		public static Texture2D PopupIcon = null;

		public static Texture2D MasterNodeOnTexture = null;
		public static Texture2D MasterNodeOffTexture = null;

		public static Texture2D GPUInstancedOnTexture = null;
		public static Texture2D GPUInstancedOffTexture = null;

		public static GUIContent LockIconOpen = null;
		public static GUIContent LockIconClosed = null;

		public static GUIContent FloatIntIconON = null;
		public static GUIContent FloatIntIconOFF = null;

		public static bool ShowContextOnPick = true;

		private static AmplifyShaderEditorWindow m_currentWindow = null;
		public static AmplifyShaderEditorWindow CurrentWindow
		{
			get
			{
				if( m_currentWindow == null )
				{
					for( int i = 0; i < IOUtils.AllOpenedWindows.Count; i++ )
					{
						if( IOUtils.AllOpenedWindows[ i ] != null )
						{
							m_currentWindow = IOUtils.AllOpenedWindows[ i ];
						}
						else
						{
							//Debug.Log("No Window Found!");
						}
					}
				}
				return m_currentWindow;
			}
			set { m_currentWindow = value; }
		}

		public static Vector2 PortsSize;
		public static Vector3 PortsDelta;
		public static Vector3 ScaledPortsDelta;

		public static RectOffset RectOffsetZero;
		public static RectOffset RectOffsetOne;
		public static RectOffset RectOffsetTwo;
		public static RectOffset RectOffsetThree;
		public static RectOffset RectOffsetFour;
		public static RectOffset RectOffsetFive;
		public static RectOffset RectOffsetSix;

		public static Material LinearMaterial = null;
		public static Shader IntShader = null;
		public static Shader FloatShader = null;
		public static Shader Vector2Shader = null;
		public static Shader Vector3Shader = null;
		public static Shader Vector4Shader = null;
		public static Shader ColorShader = null;
		public static Shader Texture2DShader = null;
		public static Shader MaskingShader = null;

		public static bool InhibitMessages = false;


		private static int m_shaderIndentLevel = 0;
		private static string m_shaderIndentTabs = string.Empty;

		//Label Vars

		private static TextAnchor m_alignment;
		private static TextClipping m_clipping;
		private static bool m_wordWrap;
		private static int m_fontSize;
		private static Color m_fontColor;
		private static FontStyle m_fontStyle;


		private static string NumericNamePattern = @"^\d";
		private static System.Globalization.TextInfo m_textInfo;
		private static string m_latestOpenedFolder = string.Empty;
		private static Dictionary<int, UndoParentNode> m_undoHelper = new Dictionary<int, UndoParentNode>();

		private static Dictionary<string, int> AvailableKeywordsDict = new Dictionary<string, int>();
		public static readonly string[] AvailableKeywords =
		{
			"Custom",
			"ETC1_EXTERNAL_ALPHA",
			"PIXELSNAP_ON",
			"UNITY_PASS_FORWARDBASE",
			"UNITY_PASS_FORWARDADD",
			"UNITY_PASS_DEFERRED",
			"UNITY_PASS_SHADOWCASTER"
		};

		public static readonly string[] CategoryPresets =
		{
			"<Custom>",
			"Camera And Screen",
			"Constants And Properties",
			"Functions",
			"Image Effects",
			"Light",
			"Logical Operators",
			"Math Operators",
			"Matrix Operators",
			"Miscellaneous",
			"Object Transform",
			"Surface Data",
			"Textures",
			"Time",
			"Trignometry Operators",
			"UV Coordinates",
			"Vector Operators",
			"Vertex Data"
		};

		private static Dictionary<MasterNodePortCategory, int> m_portCategoryToArrayIdx = new Dictionary<MasterNodePortCategory, int>
		{
			{ MasterNodePortCategory.Vertex,0},
			{ MasterNodePortCategory.Tessellation,0},
			{ MasterNodePortCategory.Fragment,1},
			{ MasterNodePortCategory.Debug,1}
		};

		private static Dictionary<string, string> m_reservedPropertyNames = new Dictionary<string, string>
		{
			{ "UNITY_MATRIX_MVP", string.Empty},
			{ "UNITY_MATRIX_MV", string.Empty},
			{ "UNITY_MATRIX_V", string.Empty},
			{ "UNITY_MATRIX_P", string.Empty},
			{ "UNITY_MATRIX_VP", string.Empty},
			{ "UNITY_MATRIX_T_MV", string.Empty},
			{ "UNITY_MATRIX_IT_MV", string.Empty},
			{ "UNITY_MATRIX_TEXTURE0", string.Empty},
			{ "UNITY_MATRIX_TEXTURE1", string.Empty},
			{ "UNITY_MATRIX_TEXTURE2", string.Empty},
			{ "UNITY_MATRIX_TEXTURE3", string.Empty},
			{ "_Object2World", string.Empty},
			{ "_WorldSpaceCameraPos", string.Empty},
			{ "unity_Scale", string.Empty},
			{ "_ModelLightColor", string.Empty},
			{ "_SpecularLightColor", string.Empty},
			{ "_ObjectSpaceLightPos", string.Empty},
			{ "_Light2World", string.Empty},
			{ "_World2Light", string.Empty},
			{ "_Object2Light", string.Empty},
			{ "_Time", string.Empty},
			{ "_SinTime", string.Empty},
			{ "_CosTime", string.Empty},
			{ "unity_DeltaTime", string.Empty},
			{ "_ProjectionParams", string.Empty},
			{ "_ScreenParams", string.Empty}
		};

		private static Dictionary<string, string> m_exampleMaterialIDs = new Dictionary<string, string>()
		{
			//Community
			{"2Sided",                      "8ebbbf2c99a544ca780a2573ef1450fc" },
			{"DissolveBurn",                "f144f2d7ff3daf349a2b7f0fd81ec8ac" },
			{"MourEnvironmentGradient",     "b64adae401bc073408ac7bff0993c107" },
			{"ForceShield",                 "0119aa6226e2a4cfdb6c9a5ba9df7820" },
			{"HighlightAnimated",           "3d232e7526f6e426cab994cbec1fc287" },
			{"Hologram",                    "b422c600f1c3941b8bc7e95db33476ad" },
			{"LowPolyWater",                 "0557703d3791a4286a62f8ee709d5bef"},
			//Official
			{"AnimatedFire",                "63ea5eae6d954a14292033589d0d4275" },
			{"AnimatedFire-ShaderFunction", "9c6c9fcb82afe874a825a9e680e694b2" },
			{"BurnEffect",                  "0b019675a8064414b97862a02f644166" },
			{"CubemapReflections",          "2c299f827334e9c459a60931aea62260" },
			{"DitheringFade",               "610507217b7dcad4d97e6e03e9844171" },
			{"DoubleLayerCustomSurface",    "846aec4914103104d99e9e31a217b548" },
			{"NormalExtrusion",             "70a5800fbba039f46b438a2055bc6c71" },
			{"MatcapSample",                "da8aaaf01fe8f2b46b2fbcb803bd7af4" },
			{"ParallaxMappingIterations",   "a0cea9c3f318ac74d89cd09134aad000" },
			{"SandPOM",                     "905481dc696211145b88dc4bac2545f3" },
			{"ParallaxWindow",              "63ad0e7afb1717b4e95adda8904ab0c3" },
			{"LocalPosCutoff",              "fed8c9d33a691084c801573feeed5a62" },
			{"ImprovedReadFromAtlasTiled",  "941b31b251ea8e74f9198d788a604c9b" },
			{"ReadFromAtlasTiled",          "2d5537aa702f24645a1446dc3be92bbf" },
			{"ReflectRefractSoapBubble",    "a844987c9f2e7334abaa34f12feda3b9" },
			{"RimLight",                    "e2d3a4d723cf1dc4eab1d919f3324dbc" },
			{"RefractedShadows",            "11818aa28edbeb04098f3b395a5bfc1d" },
			{"TextureArray",                "0f572993ab788a346aea45f2f797b7fa" },
			{"ObjectNormalRefraction",      "f1a0a645876302547b608ce881c94e6d" },
			{"ShaderBallInterior",          "e47ee174f55b6144b9c1a942bb23d82a" },
			{"ScreenSpaceCurvature",        "2e794cb9b3900b043a37ba28cdc2f907" },
			{"ScreenSpaceDetail",           "3a0163d12fede4d47a1f818a66a115de" },
			{"SimpleNoise",                 "cc167bc6c2063a14f84a5a77be541194" },
			{"SimpleBlur",                  "1d283ff911af20e429180bb15d023661" },
			{"SimpleGPUInstancing",         "9d609a7c8d00c7c4c9bdcdcdba154b81" },
			{"SimpleLambert",               "54b29030f7d7ffe4b84f2f215dede5ac" },
			{"SimpleRefraction",            "58c94d2f48acdc049a53b4ca53d6d98a" },
			{"SimpleTexture",               "9661085a7d249a54c95078ac8e7ff004" },
			{"SnowAccum",                   "e3bd639f50ae1a247823079047a8dc01" },
			{"StencilDiffuse01",            "9f47f529fdeddd948a2d2722f73e6ac4" },
			{"StencilMask01",               "6f870834077d59b44ac421c36f619d59" },
			{"StencilDiffuse02",            "11cdb862d5ba68c4eae526765099305b" },
			{"StencilMask02",               "344696733b065c646b18c1aa2eacfdb7" },
			{"StencilDiffuse03",            "75e851f6c686a5f42ab900222b29355b" },
			{"StencilMask03",               "c7b3018ad495c6b479f2e3f8564aa6dc" },
			{"SubstanceExample",            "a515e243b476d7e4bb37eb9f82c87a12" },
			{"AnimatedRefraction",          "e414af1524d258047bb6b82b8860062c" },
			{"Tessellation",                "efb669a245f17384c88824d769d0087c" },
			{"Translucency",                "842ba3dcdd461ea48bdcfcea316cbcc4" },
			{"Transmission",                "1b21506b7afef734facfc42c596caa7b" },
			{"Transparency",                "e323a62068140c2408d5601877e8de2c" },
			{"TriplanarProjection",         "663d512de06d4e24db5205c679f394cb" },
			{"TwoSideWithFace",             "c953c4b601ba78e4f870d24d038b67f6" },
			{"Ground",                      "48df9bdf7b922d94bb3167e6db39c943" },
			{"WaterSample",                 "288137d67ce790e41903020c572ab4d7" },
			{"WorldPosSlices",              "013cc03f77f3d034692f902db8928787" }
		};

		private static Dictionary<TextureType, string> m_textureTypeToCgType = new Dictionary<TextureType, string>()
		{
			{TextureType.Texture1D,			"sampler1D" },
			{TextureType.Texture2D,			"sampler2D" },
			{TextureType.Texture3D,			"sampler3D" },
			{TextureType.Cube ,				"samplerCUBE"},
			{TextureType.Texture2DArray,	"sampler2D" },
			{TextureType.ProceduralTexture,	"sampler2D" }
		};


		private static Dictionary<string, Color> m_nodeCategoryToColor = new Dictionary<string, Color>()
		{
			{ "Master",								new Color( 0.6f, 0.52f, 0.43f, 1.0f )},
			{ "Default",                            new Color( 0.26f, 0.35f, 0.44f, 1.0f )},
			{ "Vertex Data",                        new Color( 0.8f, 0.07f, 0.18f, 1.0f)},//new Color( 0.75f, 0.10f, 0.30f, 1.0f )},
			{ "Math Operators",                     new Color( 0.26f, 0.35f, 0.44f, 1.0f )},//new Color( 0.10f, 0.27f, 0.45f, 1.0f) },
			{ "Logical Operators",                  new Color( 0.0f, 0.55f, 0.45f, 1.0f)},//new Color( 0.11f, 0.28f, 0.47f, 1.0f) },
			{ "Trigonometry Operators",             new Color( 0.1f, 0.20f, 0.35f, 1.0f)},//new Color( 0.8f, 0.07f, 0.18f, 1.0f)},
			{ "Image Effects",                      new Color( 0.5f, 0.2f, 0.90f, 1.0f)},//new Color( 0.12f, 0.47f, 0.88f, 1.0f)},
			{ "Miscellaneous",                      new Color( 0.49f, 0.32f, 0.60f, 1.0f)},
			{ "Camera And Screen",                  new Color( 0.75f, 0.10f, 0.30f, 1.0f )},//new Color( 0.17f, 0.22f, 0.07f, 1.0f) },
			{ "Constants And Properties",           new Color( 0.42f, 0.70f, 0.22f, 1.0f) },
			{ "Surface Data",                       new Color( 0.92f, 0.73f, 0.03f, 1.0f)},
			{ "Matrix Transform",                   new Color( 0.09f, 0.43f, 0.2f, 1.0f) },
			{ "Time",                               new Color( 0.25f, 0.25f, 0.25f, 1.0f)},//new Color( 0.89f, 0.59f, 0.0f, 1.0f) },
			{ "Functions",                          new Color( 1.00f, 0.4f, 0.0f, 1.0f) },
			{ "Vector Operators",                   new Color( 0.22f, 0.20f, 0.45f, 1.0f)},
			{ "Matrix Operators",                   new Color( 0.45f, 0.9f, 0.20f, 1.0f) },
			{ "Light",                              new Color( 1.0f, 0.9f, 0.0f, 1.0f) },
			{ "Textures",                           new Color( 0.15f, 0.40f, 0.8f, 1.0f)},
			{ "Commentary",                         new Color( 0.7f, 0.7f, 0.7f, 1.0f)},
			{ "UV Coordinates",                     new Color( 0.89f, 0.59f, 0.0f, 1.0f) },
			{ "Object Transform",                   new Color( 0.15f, 0.4f, 0.49f, 1.0f)},
			{ "Vertex Transform",                   new Color( 0.15f, 0.4f, 0.49f, 1.0f)}
		};

		private static Dictionary<ToolButtonType, List<string>> m_toolButtonTooltips = new Dictionary<ToolButtonType, List<string>>
		{
			{ ToolButtonType.New,              new List<string>() { "Create new shader." } },
			{ ToolButtonType.Open,             new List<string>() { "Open existing shader." } },
			{ ToolButtonType.Save,             new List<string>() { "No changes to save.", "Save current changes." } },
			{ ToolButtonType.Library,          new List<string>() { "Lists custom shader selection." } },
			{ ToolButtonType.Options,          new List<string>() { "Open Options menu." } },
			{ ToolButtonType.Update,           new List<string>() { "Open or create a new shader first.", "Click to enable to update current shader.", "Shader up-to-date." } },
			{ ToolButtonType.Live,             new List<string>() { "Open or create a new shader first.", "Click to enable live shader preview", "Click to enable live shader and material preview." , "Live preview active, click to disable." } },
			{ ToolButtonType.CleanUnusedNodes, new List<string>() { "No unconnected nodes to clean.", "Remove all nodes not connected( directly or indirectly) to the master node." }},
			{ ToolButtonType.Help,             new List<string>() { "Show help window." } },
			{ ToolButtonType.FocusOnMasterNode,new List<string>() { "Focus on active master node." } },
			{ ToolButtonType.FocusOnSelection, new List<string>() { "Focus on selection fit to screen ( if none selected )." } }
		};

		private static Color[] m_dataTypeToColorMonoMode = { new Color( 0.5f, 0.5f, 0.5f, 1.0f ), Color.white };
		private static Dictionary<WirePortDataType, Color> m_dataTypeToColor = new Dictionary<WirePortDataType, Color>( new WirePortDataTypeComparer() )
		{
			{  WirePortDataType.OBJECT,		Color.white},
			{ WirePortDataType.FLOAT,		Color.gray},
			{ WirePortDataType.FLOAT2,		new Color(1f,1f,0f,1f)},
			{ WirePortDataType.FLOAT3,		new Color(0.5f,0.5f,1f,1f)},
			{ WirePortDataType.FLOAT4,		new Color(1f,0,1f,1f)},
			{ WirePortDataType.FLOAT3x3,	new Color(0.5f,1f,0.5f,1f)},
			{ WirePortDataType.FLOAT4x4,	new Color(0.5f,1f,0.5f,1f)},
			{ WirePortDataType.COLOR,		new Color(1f,0,1f,1f)},
			{ WirePortDataType.INT,			Color.gray},
			{ WirePortDataType.SAMPLER1D,	new Color(1f,0.5f,0f,1f)},
			{ WirePortDataType.SAMPLER2D,	new Color(1f,0.5f,0f,1f)},
			{ WirePortDataType.SAMPLER3D,	new Color(1f,0.5f,0f,1f)},
			{ WirePortDataType.SAMPLERCUBE,	new Color(1f,0.5f,0f,1f)}
		};

		private static Dictionary<WirePortDataType, string> m_dataTypeToName = new Dictionary<WirePortDataType, string>()
		{
			{ WirePortDataType.OBJECT,		"Generic Object"},
			{ WirePortDataType.FLOAT,		"Float"},
			{ WirePortDataType.FLOAT2,		"Vector2"},
			{ WirePortDataType.FLOAT3,		"Vector3"},
			{ WirePortDataType.FLOAT4,		"Vector4"},
			{ WirePortDataType.FLOAT3x3,	"3x3 Matrix"},
			{ WirePortDataType.FLOAT4x4,	"4x4 Matrix"},
			{ WirePortDataType.COLOR,		"Color"},
			{ WirePortDataType.INT,			"Int"},
			{ WirePortDataType.SAMPLER1D,	"Sampler1D"},
			{ WirePortDataType.SAMPLER2D,	"Sampler2D"},
			{ WirePortDataType.SAMPLER3D,	"Sampler3D"},
			{ WirePortDataType.SAMPLERCUBE,	"SamplerCUBE"}
		};

		private static Dictionary<SurfaceInputs, string> m_inputTypeDeclaration = new Dictionary<SurfaceInputs, string>()
		{
			{ SurfaceInputs.DEPTH, "{0} Depth : SV_Depth"},
			{ SurfaceInputs.UV_COORDS, "{0}2 uv"},// texture uv must have uv or uv2 followed by the texture name
			{ SurfaceInputs.UV2_COORDS, "{0}2 uv2"},
			{ SurfaceInputs.VIEW_DIR, "{0}3 viewDir"},
			{ SurfaceInputs.COLOR, Constants.ColorInput},
			{ SurfaceInputs.SCREEN_POS, "{0}4 screenPos"},
			{ SurfaceInputs.WORLD_POS, "{0}3 worldPos"},
			{ SurfaceInputs.WORLD_REFL, "{0}3 worldRefl"},
			{ SurfaceInputs.WORLD_NORMAL,"{0}3 worldNormal"},
			{ SurfaceInputs.VFACE, Constants.VFaceInput},
			{ SurfaceInputs.INTERNALDATA, Constants.InternalData}
		};

		private static Dictionary<SurfaceInputs, string> m_inputTypeName = new Dictionary<SurfaceInputs, string>()
		{
			{ SurfaceInputs.DEPTH, "Depth"},
			{ SurfaceInputs.UV_COORDS, "uv"},// texture uv must have uv or uv2 followed by the texture name
			{ SurfaceInputs.UV2_COORDS, "uv2"},
			{ SurfaceInputs.VIEW_DIR, "viewDir"},
			{ SurfaceInputs.COLOR, Constants.ColorVariable},
			{ SurfaceInputs.SCREEN_POS, "screenPos"},
			{ SurfaceInputs.WORLD_POS, "worldPos"},
			{ SurfaceInputs.WORLD_REFL, "worldRefl"},
			{ SurfaceInputs.WORLD_NORMAL, "worldNormal"},
			{ SurfaceInputs.VFACE, Constants.VFaceVariable},
		};

		private static Dictionary<PrecisionType, string> m_precisionTypeToCg = new Dictionary<PrecisionType, string>()
		{
			{PrecisionType.Float,	"float"},
			{PrecisionType.Half,	"half"},
			{PrecisionType.Fixed,	"half"}
		};

		private static Dictionary<VariableQualifiers, string> m_qualifierToCg = new Dictionary<VariableQualifiers, string>()
		{
			{ VariableQualifiers.In,	string.Empty},
			{VariableQualifiers.Out,	"out"},
			{VariableQualifiers.InOut,	"inout"}
		};

		private static Dictionary<WirePortDataType, string> m_precisionWirePortToCgType = new Dictionary<WirePortDataType, string>()
		{
			{WirePortDataType.FLOAT,       "{0}"},
			{WirePortDataType.FLOAT2,       "{0}2"},
			{WirePortDataType.FLOAT3,       "{0}3"},
			{WirePortDataType.FLOAT4,       "{0}4"},
			{WirePortDataType.FLOAT3x3,     "{0}3x3"},
			{WirePortDataType.FLOAT4x4,     "{0}4x4"},
			{WirePortDataType.COLOR,        "{0}4"},
			{WirePortDataType.INT,          "int"},
			{WirePortDataType.SAMPLER1D,    "sampler1D"},
			{WirePortDataType.SAMPLER2D,    "sampler2D"},
			{WirePortDataType.SAMPLER3D,    "sampler3D"},
			{WirePortDataType.SAMPLERCUBE,  "samplerCUBE"}
		};

		private static Dictionary<WirePortDataType, string> m_wirePortToCgType = new Dictionary<WirePortDataType, string>()
		{
			{ WirePortDataType.FLOAT,		"float"},
			{WirePortDataType.FLOAT2,		"float2"},
			{WirePortDataType.FLOAT3,		"float3"},
			{WirePortDataType.FLOAT4,		"float4"},
			{WirePortDataType.FLOAT3x3,		"float3x3"},
			{WirePortDataType.FLOAT4x4,		"float4x4"},
			{WirePortDataType.COLOR,		"float4"},
			{WirePortDataType.INT,			"int"},
			{WirePortDataType.SAMPLER1D,	"sampler1D"},
			{WirePortDataType.SAMPLER2D,	"sampler2D"},
			{WirePortDataType.SAMPLER3D,	"sampler3D"},
			{WirePortDataType.SAMPLERCUBE,	"samplerCUBE"},
			{WirePortDataType.UINT,			"uint"}
		};

		private static Dictionary<KeyCode, string> m_keycodeToString = new Dictionary<KeyCode, string>()
		{
			{KeyCode.Alpha0,"0" },
			{KeyCode.Alpha1,"1" },
			{KeyCode.Alpha2,"2" },
			{KeyCode.Alpha3,"3" },
			{KeyCode.Alpha4,"4" },
			{KeyCode.Alpha5,"5" },
			{KeyCode.Alpha6,"6" },
			{KeyCode.Alpha7,"7" },
			{KeyCode.Alpha8,"8" },
			{KeyCode.Alpha9,"9" }
		};
		
		private static Dictionary<WireStatus, Color> m_wireStatusToColor = new Dictionary<WireStatus, Color>()
		{
			{ WireStatus.Default,new Color(0.7f,0.7f,0.7f,1.0f) },
			{WireStatus.Highlighted,Color.yellow },
			{WireStatus.Selected,Color.white}
		};

		private static Dictionary<WirePortDataType, string> m_autoSwizzle = new Dictionary<WirePortDataType, string>()
		{
			{WirePortDataType.FLOAT,    ".x"},
			{WirePortDataType.FLOAT2,   ".xy"},
			{WirePortDataType.FLOAT3,   ".xyz"},
			{WirePortDataType.FLOAT4,   ".xyzw"}
		};

		private static Dictionary<string, bool> m_unityNativeShaderPaths = new Dictionary<string, bool>
		{
			{ "Resources/unity_builtin_extra", true },
			{ "Library/unity default resources", true }
		};
		
		private static Dictionary<WirePortDataType, int> m_portPriority = new Dictionary<WirePortDataType, int>()
		{
			{ WirePortDataType.OBJECT,      0},
			{WirePortDataType.SAMPLER1D,    0},
			{WirePortDataType.SAMPLER2D,    0},
			{WirePortDataType.SAMPLER3D,    0},
			{WirePortDataType.SAMPLERCUBE,  0},
			{WirePortDataType.FLOAT3x3,     1},
			{WirePortDataType.FLOAT4x4,     2},
			{WirePortDataType.INT,          3},
			{WirePortDataType.FLOAT,        4},
			{WirePortDataType.FLOAT2,       5},
			{WirePortDataType.FLOAT3,       6},
			{WirePortDataType.FLOAT4,       7},
			{WirePortDataType.COLOR,        7}
		};

		private static readonly string IncorrectInputConnectionErrorMsg = "Input Port {0} from node {1} has type {2}\nwhich is incompatible with connection of type {3} from port {4} on node {5}";
		private static readonly string IncorrectOutputConnectionErrorMsg = "Output Port {0} from node {1} has type {2}\nwhich is incompatible with connection of type {3} from port {4} on node {5}";
		private static readonly string NoVertexModeNodeWarning = "{0} is unable to generate code in vertex function";

		private static float SwitchFixedHeight;
		private static float SwitchFontSize;
		private static RectOffset SwitchNodeBorder;
		private static RectOffset SwitchNodeMargin;
		private static RectOffset SwitchNodeOverflow;
		private static RectOffset SwitchNodePadding;
		
		public static void ForceExampleShaderCompilation()
		{
			CurrentWindow.ForceMaterialsToUpdate( ref m_exampleMaterialIDs );

		}

		public static void Destroy()
		{
			if( IOUtils.AllOpenedWindows != null && IOUtils.AllOpenedWindows.Count > 0 )
			{
				return;
			}
			else
			{
				IOUtils.AllOpenedWindows.Clear();
			}

			Initialized = false;
			PlusStyle = null;
			MinusStyle = null;
			m_textInfo = null;
			RangedFloatSliderStyle = null;
			RangedFloatSliderThumbStyle = null;
			PropertyPopUp = null;
			ObjectField = null;
			PreviewExpander = null;
			PreviewCollapser = null;
			MenuItemToggleStyle = null;
			MenuItemEnableStyle = null;
			MenuItemBackgroundStyle = null;
			MenuItemToolbarStyle = null;
			MenuItemInspectorDropdownStyle = null;
			ObjectFieldThumb = null;
			ObjectFieldThumbOverlay = null;
			InspectorPopdropdownStyle = null;
			InspectorPopdropdownFallback = null;
			TooltipBox = null;
			UnZoomedNodeTitleStyle = null;
			UnZoomedPropertyValuesTitleStyle = null;
			UnZoomedInputPortStyle = null;
			UnZoomedOutputPortPortStyle = null;
			ToolbarSearchTextfield = null;
			ToolbarSearchCancelButton = null;
			FloatIntPickerONOFF = null;
			Box = null;
			Button = null;
			TextArea = null;
			Label = null;
			Toggle = null;
			Textfield = null;

			CommentaryTitle = null;
			InputPortLabel = null;
			OutputPortLabel = null;

			IntShader = null;
			FloatShader = null;
			Vector2Shader = null;
			Vector3Shader = null;
			Vector4Shader = null;
			ColorShader = null;
			Texture2DShader = null;

			MaskingShader = null;

			BoldErrorStyle = null;
			BoldWarningStyle = null;
			BoldInfoStyle = null;
			Separator = null;

			GraphButtonIcon = null;
			GraphButton = null;
			GraphDropDown = null;

			MiniButtonTopLeft = null;
			MiniButtonTopMid = null;
			MiniButtonTopRight = null;

			NodeWindowOffSquare = null;
			NodeHeaderSquare = null;
			NodeWindowOnSquare = null;
			InternalDataOnPort = null;
			InternalDataBackground = null;

			MiniObjectFieldThumbOverlay = null;
			MiniSamplerButton = null;

			Resources.UnloadAsset( SmallErrorIcon );
			SmallErrorIcon = null;

			Resources.UnloadAsset( SmallWarningIcon );
			SmallWarningIcon = null;

			Resources.UnloadAsset( SmallInfoIcon );
			SmallInfoIcon = null;

			LockIconOpen = null;
			LockIconClosed = null;

			FloatIntIconON = null;
			FloatIntIconOFF = null;

			Resources.UnloadAsset( CheckmarkIcon );
			CheckmarkIcon = null;

			Resources.UnloadAsset( PopupIcon );
			PopupIcon = null;

			Resources.UnloadAsset( MasterNodeOnTexture );
			MasterNodeOnTexture = null;

			Resources.UnloadAsset( MasterNodeOffTexture );
			MasterNodeOffTexture = null;

			Resources.UnloadAsset( GPUInstancedOnTexture );
			GPUInstancedOnTexture = null;

			Resources.UnloadAsset( GPUInstancedOffTexture );
			GPUInstancedOffTexture = null;

			MainSkin = null;

			if( LinearMaterial != null )
				GameObject.DestroyImmediate( LinearMaterial );

			LinearMaterial = null;

			if( m_undoHelper == null )
			{
				m_undoHelper.Clear();
				m_undoHelper = null;
			}
			ASEMaterialInspector.Instance = null;
		}

		public static void ResetMainSkin()
		{
			if( (object)MainSkin != null )
			{
				CurrentHeaderHeight = HeaderMaxHeight;
				ScaledPortsDelta = PortsDelta;
				MainSkin.textField.fontSize = (int)( Constants.TextFieldFontSize );
				MainSkin.label.fontSize = (int)( Constants.DefaultFontSize );
				MainSkin.customStyles[ (int)CustomStyle.NodeTitle ].fontSize = (int)( Constants.DefaultTitleFontSize );

				InputPortLabel.fontSize = (int)( Constants.DefaultFontSize );
				OutputPortLabel.fontSize = (int)( Constants.DefaultFontSize );
				CommentaryTitle.fontSize = (int)( Constants.DefaultFontSize );
			}
		}

		public static void InitMainSkin()
		{
			MainSkin = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( IOUtils.MainSkinGUID ), typeof( GUISkin ) ) as GUISkin;
			Initialized = true;
			Texture2D portTex = GetCustomStyle( CustomStyle.PortEmptyIcon ).normal.background;
			PortsSize = new Vector2( portTex.width, portTex.height );
			PortsDelta = new Vector3( 0.5f * PortsSize.x, 0.5f * PortsSize.y );
			HeaderMaxHeight = MainSkin.customStyles[ (int)CustomStyle.NodeHeader ].normal.background.height;

			RectOffsetZero = new RectOffset( 0, 0, 0, 0 );
			RectOffsetOne = new RectOffset( 1, 1, 1, 1 );
			RectOffsetTwo = new RectOffset( 2, 2, 2, 2 );
			RectOffsetThree = new RectOffset( 3, 3, 3, 3 );
			RectOffsetFour = new RectOffset( 4, 4, 4, 4 );
			RectOffsetFive = new RectOffset( 5, 5, 5, 5 );
			RectOffsetSix = new RectOffset( 6, 6, 6, 6 );

			PropertyPopUp = GetCustomStyle( CustomStyle.NodePropertyPicker );
			ObjectField = new GUIStyle( (GUIStyle)"ObjectField" );
			PreviewExpander = GetCustomStyle( CustomStyle.NodePreviewExpander );
			PreviewCollapser = GetCustomStyle( CustomStyle.NodePreviewCollapser );

			WireNodeSelection = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( "bfe0b03d5d60cea4f9d4b2d1d121e592" ), typeof( Texture2D ) ) as Texture2D;
			SliderButton = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( "dd563e33152bb6443b099b4139ceecb9" ), typeof( Texture2D ) ) as Texture2D;

			SmallErrorIcon = EditorGUIUtility.Load( "icons/d_console.erroricon.sml.png" ) as Texture2D;
			SmallWarningIcon = EditorGUIUtility.Load( "icons/d_console.warnicon.sml.png" ) as Texture2D;
			SmallInfoIcon = EditorGUIUtility.Load( "icons/d_console.infoicon.sml.png" ) as Texture2D;
			
			LockIconOpen = new GUIContent( EditorGUIUtility.IconContent( "LockIcon-On" ) );
			LockIconOpen.tooltip = "Click to unlock and customize the variable name";
			LockIconClosed = new GUIContent( EditorGUIUtility.IconContent( "LockIcon" ) );
			LockIconClosed.tooltip = "Click to lock and auto-generate the variable name";

			if( UsingProSkin )
			{
				FloatIntIconON = new GUIContent( EditorGUIUtility.IconContent( "CircularToggle_ON" ) );
				FloatIntIconOFF = new GUIContent( EditorGUIUtility.IconContent( "CircularToggle_OFF" ) );
			}
			else
			{
				FloatIntIconON = new GUIContent( ( AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( "ac0860a6a77e29d4091ba790a17daa0f" ), typeof( Texture2D ) ) as Texture2D ) );
				FloatIntIconOFF = new GUIContent( ( AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( "1aaca50d084b0bb43854f075ce2f302b" ), typeof( Texture2D ) ) as Texture2D ) );
			}

			CommentaryTitle = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.CommentaryTitle ] );
			InputPortLabel = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.InputPortlabel ] );
			OutputPortLabel = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.OutputPortLabel ] );

			CheckmarkIcon = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( "e9c4642eaa083a54ab91406d8449e6ac" ), typeof( Texture2D ) ) as Texture2D;
			PopupIcon = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( "d2384a227b4ac4943b73c8151393e502" ), typeof( Texture2D ) ) as Texture2D;

			BoldErrorStyle = new GUIStyle( (GUIStyle)"BoldLabel" );
			BoldErrorStyle.normal.textColor = Color.red;
			BoldErrorStyle.alignment = TextAnchor.MiddleCenter;
			BoldWarningStyle = new GUIStyle( (GUIStyle)"BoldLabel" );
			BoldWarningStyle.normal.textColor = Color.yellow;
			BoldWarningStyle.alignment = TextAnchor.MiddleCenter;
			BoldInfoStyle = new GUIStyle( (GUIStyle)"BoldLabel" );
			BoldInfoStyle.normal.textColor = Color.white;
			BoldInfoStyle.alignment = TextAnchor.MiddleCenter;

			Separator = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.FlatBackground ] );
			MiniButtonTopLeft = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.MiniButtonTopLeft ] );
			MiniButtonTopMid = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.MiniButtonTopMid ] );
			MiniButtonTopRight = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.MiniButtonTopRight ] );

			InternalDataOnPort = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.NodeTitle ] );
			InternalDataOnPort.fontSize = 8;
			InternalDataOnPort.fontStyle = FontStyle.BoldAndItalic;
			InternalDataBackground = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.NodeWindowOffSquare ] );
			InternalDataBackground.normal.background = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( "330fd0c8f074a3c4f8042114a61a73d9" ), typeof( Texture2D ) ) as Texture2D;
			InternalDataBackground.overflow = RectOffsetOne;

			MiniObjectFieldThumbOverlay = new GUIStyle( (GUIStyle)"ObjectFieldThumbOverlay" );
			MiniSamplerButton = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.SamplerButton ] );
			
			m_textInfo = new System.Globalization.CultureInfo( "en-US", false ).TextInfo;
			RangedFloatSliderStyle = new GUIStyle( GUI.skin.horizontalSlider );
			RangedFloatSliderThumbStyle = new GUIStyle( GUI.skin.horizontalSliderThumb );
			RangedFloatSliderThumbStyle.normal.background = SliderButton;
			RangedFloatSliderThumbStyle.active.background = null;
			RangedFloatSliderThumbStyle.hover.background = null;
			RangedFloatSliderThumbStyle.focused.background = null;

			SwitchNodePopUp = new GUIStyle( (GUIStyle)"Popup" );
			// RectOffset cannot be initiliazed on constructor
			SwitchNodeBorder = new RectOffset( 4, 15, 3, 3 );
			SwitchNodeMargin = new RectOffset( 4, 4, 3, 3 );
			SwitchNodeOverflow = new RectOffset( 0, 0, -1, 2 );
			SwitchNodePadding = new RectOffset( 6, 14, 2, 3 );
			SwitchFixedHeight = 18;
			SwitchFontSize = 10;

			GraphButtonIcon = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.GraphButtonIcon ] );
			GraphButton = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.GraphButton ] );
			GraphDropDown = new GUIStyle( MainSkin.customStyles[ (int)CustomStyle.GraphButton ] );
			GraphDropDown.padding.right = 20;

			Box = new GUIStyle( GUI.skin.box );
			Button = new GUIStyle( GUI.skin.button );
			TextArea = new GUIStyle( GUI.skin.textArea );
			Label = new GUIStyle( GUI.skin.label );
			Toggle = new GUIStyle( GUI.skin.toggle );
			Textfield = new GUIStyle( GUI.skin.textField );
			//ShaderIcon = EditorGUIUtility.IconContent( "Shader Icon" ).image;
			//MaterialIcon = EditorGUIUtility.IconContent( "Material Icon" ).image;

			NodeWindowOffSquare = GetCustomStyle( CustomStyle.NodeWindowOffSquare );
			NodeHeaderSquare = GetCustomStyle( CustomStyle.NodeHeaderSquare );
			NodeWindowOnSquare = GetCustomStyle( CustomStyle.NodeWindowOnSquare );

			UnZoomedNodeTitleStyle = new GUIStyle( GetCustomStyle( CustomStyle.NodeTitle ) );
			UnZoomedNodeTitleStyle.fontSize = 13;

			UnZoomedPropertyValuesTitleStyle = new GUIStyle( GetCustomStyle( CustomStyle.PropertyValuesTitle ) );
			UnZoomedPropertyValuesTitleStyle.fontSize = 11;

			UnZoomedInputPortStyle = new GUIStyle( InputPortLabel );
			UnZoomedInputPortStyle.fontSize = (int)Constants.DefaultFontSize;

			UnZoomedOutputPortPortStyle = new GUIStyle( OutputPortLabel );
			UnZoomedOutputPortPortStyle.fontSize = (int)Constants.DefaultFontSize;

			ObjectFieldThumb = new GUIStyle( (GUIStyle)"ObjectFieldThumb" );
			ObjectFieldThumbOverlay = new GUIStyle( (GUIStyle)"ObjectFieldThumbOverlay" );

			FloatIntPickerONOFF = new GUIStyle( "metimelabel" );
			FloatIntPickerONOFF.padding.left = -2;
			FloatIntPickerONOFF.margin = new RectOffset(0,2,2,2);

			TooltipBox = new GUIStyle( (GUIStyle)"Tooltip" );
			TooltipBox.richText = true;

			MasterNodeOnTexture = AssetDatabase.LoadAssetAtPath<Texture2D>( AssetDatabase.GUIDToAssetPath( IOUtils.MasterNodeOnTextureGUID ) );
			MasterNodeOffTexture = AssetDatabase.LoadAssetAtPath<Texture2D>( AssetDatabase.GUIDToAssetPath( IOUtils.MasterNodeOnTextureGUID ) );

			GPUInstancedOnTexture = AssetDatabase.LoadAssetAtPath<Texture2D>( AssetDatabase.GUIDToAssetPath( IOUtils.GPUInstancedOnTextureGUID ) );
			GPUInstancedOffTexture = AssetDatabase.LoadAssetAtPath<Texture2D>( AssetDatabase.GUIDToAssetPath( IOUtils.GPUInstancedOffTextureGUID ) );

			CheckNullMaterials();

			UsingProSkin = EditorGUIUtility.isProSkin;
			FetchMenuItemStyles();
		}

		public static bool IsLoading
		{
			get { return CurrentWindow.OutsideGraph.IsLoading; }
		}

		public static void CheckNullMaterials()
		{
			if( LinearMaterial == null )
			{
				Shader linearShader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "e90ef6ea05743b84baf9549874c52e47" ) ); //linear previews
				LinearMaterial = new Material( linearShader );
			}

			if( IntShader == null )
				IntShader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "0f64d695b6ffacc469f2dd31432a232a" ) ); //int
			if( FloatShader == null )
				FloatShader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "d9ca47581ac157145bff6f72ac5dd73e" ) ); //ranged float
			if( Vector2Shader == null )
				Vector2Shader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "88b4191eb06084d4da85d1dd2f984085" ) ); //vector2
			if( Vector3Shader == null )
				Vector3Shader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "8a44d38f06246bf48944b3f314bc7920" ) ); //vector3
			if( Vector4Shader == null )
				Vector4Shader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "aac241d0e47a5a84fbd2edcd640788dc" ) ); //vector4
			if( ColorShader == null )
				ColorShader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "6cf365ccc7ae776488ae8960d6d134c3" ) ); //color node
			if( MaskingShader == null )
				MaskingShader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "9c34f18ebe2be3e48b201b748c73dec0" ) ); //masking shader
			if( Texture2DShader == null )
				Texture2DShader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "13bd295c44d04e1419f20f792d331e33" ) ); //texture2d shader
		}

		private static void FetchMenuItemStyles()
		{
			ObjectFieldThumb = new GUIStyle( (GUIStyle)"ObjectFieldThumb" );
			ObjectFieldThumbOverlay = new GUIStyle( (GUIStyle)"ObjectFieldThumbOverlay" );
			MenuItemToggleStyle = new GUIStyle( (GUIStyle)"foldout" );
			MenuItemEnableStyle = UsingProSkin ? new GUIStyle( (GUIStyle)"OL ToggleWhite" ) : new GUIStyle( (GUIStyle)"OL Toggle" );
			MenuItemBackgroundStyle = new GUIStyle( (GUIStyle)"TE NodeBackground" );
			MenuItemToolbarStyle = new GUIStyle( (GUIStyle)"toolbarbutton" ) { fixedHeight = 20 };
			MenuItemInspectorDropdownStyle = new GUIStyle( (GUIStyle)"toolbardropdown" ) { fixedHeight = 20 };
			MenuItemInspectorDropdownStyle.margin.bottom = 2;


			InspectorPopdropdownStyle = new GUIStyle( GUI.skin.GetStyle( "PopupCurveDropdown" ) );
			InspectorPopdropdownStyle.alignment = TextAnchor.MiddleRight;
			InspectorPopdropdownStyle.border.bottom = 16;

			InspectorPopdropdownFallback = new GUIStyle( InspectorPopdropdownStyle );
			InspectorPopdropdownFallback.overflow = new RectOffset( 0, -5, 0, 0 );

			PlusStyle = ( EditorGUIUtility.isProSkin ) ? new GUIStyle( GetCustomStyle( CustomStyle.CustomExpressionAddItem ) ) : new GUIStyle( (GUIStyle)"OL Plus" );
			PlusStyle.imagePosition = ImagePosition.ImageOnly;
			PlusStyle.overflow = new RectOffset( -2, 0, -4, 0 );

			MinusStyle = ( EditorGUIUtility.isProSkin ) ? new GUIStyle( GetCustomStyle( CustomStyle.CustomExpressionRemoveItem ) ) : new GUIStyle( (GUIStyle)"OL Minus" );
			MinusStyle.contentOffset = Vector2.zero;
			MinusStyle.imagePosition = ImagePosition.ImageOnly;
			MinusStyle.overflow = new RectOffset( -2, 0, -4, 0 );

			ToolbarSearchTextfield = new GUIStyle( (GUIStyle)"ToolbarSeachTextField" );
			ToolbarSearchCancelButton = new GUIStyle( (GUIStyle)"ToolbarSeachCancelButton" );
		}

		public static void UpdateMainSkin( DrawInfo drawInfo )
		{
			CurrentHeaderHeight = HeaderMaxHeight * drawInfo.InvertedZoom;
			ScaledPortsDelta = drawInfo.InvertedZoom * PortsDelta;
			MainSkin.textField.fontSize = (int)( Constants.TextFieldFontSize * drawInfo.InvertedZoom );
			MainSkin.label.fontSize = (int)( Constants.DefaultFontSize * drawInfo.InvertedZoom );

			MainSkin.customStyles[ (int)CustomStyle.NodeTitle ].fontSize = (int)( Constants.DefaultTitleFontSize * drawInfo.InvertedZoom );
			MainSkin.customStyles[ (int)CustomStyle.PropertyValuesTitle ].fontSize = (int)( Constants.PropertiesTitleFontSize * drawInfo.InvertedZoom );

			InputPortLabel.fontSize = (int)( Constants.DefaultFontSize * drawInfo.InvertedZoom );
			OutputPortLabel.fontSize = (int)( Constants.DefaultFontSize * drawInfo.InvertedZoom );
			CommentaryTitle.fontSize = (int)( Constants.DefaultFontSize * drawInfo.InvertedZoom );

			RangedFloatSliderStyle.fixedHeight = 18 * drawInfo.InvertedZoom;
			RangedFloatSliderThumbStyle.fixedHeight = 12 * drawInfo.InvertedZoom;
			RangedFloatSliderThumbStyle.fixedWidth = 10 * drawInfo.InvertedZoom;
			RangedFloatSliderThumbStyle.overflow.left = (int)( 1 * drawInfo.InvertedZoom );
			RangedFloatSliderThumbStyle.overflow.right = (int)( 1 * drawInfo.InvertedZoom );
			RangedFloatSliderThumbStyle.overflow.top = (int)( -4 * drawInfo.InvertedZoom );
			RangedFloatSliderThumbStyle.overflow.bottom = (int)( 4 * drawInfo.InvertedZoom );

			SwitchNodePopUp.fixedHeight = SwitchFixedHeight * drawInfo.InvertedZoom;

			SwitchNodePopUp.border.left = (int)( SwitchNodeBorder.left * drawInfo.InvertedZoom );
			SwitchNodePopUp.border.right = (int)( SwitchNodeBorder.right * drawInfo.InvertedZoom );
			SwitchNodePopUp.border.top = (int)( SwitchNodeBorder.top * drawInfo.InvertedZoom );
			SwitchNodePopUp.border.bottom = (int)( SwitchNodeBorder.bottom * drawInfo.InvertedZoom );

			SwitchNodePopUp.margin.left = (int)( SwitchNodeMargin.left * drawInfo.InvertedZoom );
			SwitchNodePopUp.margin.right = (int)( SwitchNodeMargin.right * drawInfo.InvertedZoom );
			SwitchNodePopUp.margin.top = (int)( SwitchNodeMargin.top * drawInfo.InvertedZoom );
			SwitchNodePopUp.margin.bottom = (int)( SwitchNodeMargin.bottom * drawInfo.InvertedZoom );

			SwitchNodePopUp.overflow.left = (int)( SwitchNodeOverflow.left * drawInfo.InvertedZoom );
			SwitchNodePopUp.overflow.right = (int)( SwitchNodeOverflow.right * drawInfo.InvertedZoom );
			SwitchNodePopUp.overflow.top = (int)( SwitchNodeOverflow.top * drawInfo.InvertedZoom );
			SwitchNodePopUp.overflow.bottom = (int)( SwitchNodeOverflow.bottom * drawInfo.InvertedZoom );

			SwitchNodePopUp.padding.left = (int)( SwitchNodePadding.left * drawInfo.InvertedZoom );
			SwitchNodePopUp.padding.right = (int)( SwitchNodePadding.right * drawInfo.InvertedZoom );
			SwitchNodePopUp.padding.top = (int)( SwitchNodePadding.top * drawInfo.InvertedZoom );
			SwitchNodePopUp.padding.bottom = (int)( SwitchNodePadding.bottom * drawInfo.InvertedZoom );

			SwitchNodePopUp.fontSize = (int)( SwitchFontSize * drawInfo.InvertedZoom );

			BoldErrorStyle.fontSize = (int)( 12 * drawInfo.InvertedZoom );
			BoldWarningStyle.fontSize = (int)( 12 * drawInfo.InvertedZoom );
			BoldInfoStyle.fontSize = (int)( 12 * drawInfo.InvertedZoom );

			PropertyPopUp.fixedHeight = Constants.PropertyPickerHeight * drawInfo.InvertedZoom;
			PropertyPopUp.fixedWidth = Constants.PropertyPickerWidth * drawInfo.InvertedZoom;
			if( UsingProSkin != EditorGUIUtility.isProSkin )
			{
				UsingProSkin = EditorGUIUtility.isProSkin;
				FetchMenuItemStyles();
			}

			GraphDropDown.padding.left = (int)( 2 * drawInfo.InvertedZoom + 2 );
			GraphDropDown.padding.right = (int)( 20 * drawInfo.InvertedZoom );
			GraphDropDown.fontSize = (int)( 10 * drawInfo.InvertedZoom );

			PreviewExpander.fixedHeight = Constants.PreviewExpanderHeight * drawInfo.InvertedZoom;
			PreviewExpander.fixedWidth = Constants.PreviewExpanderWidth * drawInfo.InvertedZoom;

			PreviewCollapser.fixedHeight = Constants.PreviewExpanderHeight * drawInfo.InvertedZoom;
			PreviewCollapser.fixedWidth = Constants.PreviewExpanderWidth * drawInfo.InvertedZoom;

			MainSkin.customStyles[ (int)CustomStyle.SamplerButton ].fontSize = (int)( 9 * drawInfo.InvertedZoom );
			ObjectFieldThumbOverlay.fontSize = (int)( 9 * drawInfo.InvertedZoom );
			MiniButtonTopLeft.fontSize = (int)( 9 * drawInfo.InvertedZoom );
			MiniButtonTopMid.fontSize = (int)( 9 * drawInfo.InvertedZoom );
			MiniButtonTopRight.fontSize = (int)( 9 * drawInfo.InvertedZoom );

			MiniObjectFieldThumbOverlay.fontSize = (int)( 7 * drawInfo.InvertedZoom );
			MiniSamplerButton.fontSize = (int)( 8 * drawInfo.InvertedZoom );

			InternalDataOnPort.fontSize = (int)( 8 * drawInfo.InvertedZoom );

			CheckNullMaterials();
		}

		public static void CacheLabelVars()
		{
			m_alignment = GUI.skin.label.alignment;
			m_clipping = GUI.skin.label.clipping;
			m_wordWrap = GUI.skin.label.wordWrap;
			m_fontSize = GUI.skin.label.fontSize;
			m_fontStyle = GUI.skin.label.fontStyle;
			m_fontColor = GUI.skin.label.normal.textColor;
		}

		public static void RestoreLabelVars()
		{
			GUI.skin.label.alignment = m_alignment;
			GUI.skin.label.clipping = m_clipping;
			GUI.skin.label.wordWrap = m_wordWrap;
			GUI.skin.label.fontSize = m_fontSize;
			GUI.skin.label.fontStyle = m_fontStyle;
			GUI.skin.label.normal.textColor = m_fontColor;
		}

		public static string GetTooltipForToolButton( ToolButtonType toolButtonType, int state ) { return m_toolButtonTooltips[ toolButtonType ][ state ]; }

		public static string KeyCodeToString( KeyCode keyCode )
		{
			if( m_keycodeToString.ContainsKey( keyCode ) )
				return m_keycodeToString[ keyCode ];

			return keyCode.ToString();
		}

		public static string TextureTypeToCgType( TextureType type ) { return m_textureTypeToCgType[ type ]; }

		public static string QualifierToCg( VariableQualifiers qualifier )
		{
			return m_qualifierToCg[ qualifier ];
		}

		public static string WirePortToCgType( WirePortDataType type )
		{
			if( type == WirePortDataType.OBJECT )
				return string.Empty;

			return m_wirePortToCgType[ type ];
		}

		public static string FinalPrecisionWirePortToCgType( PrecisionType precisionType, WirePortDataType type )
		{
			PrecisionType finalPrecision = GetFinalPrecision( precisionType );
			if( type == WirePortDataType.OBJECT )
				return string.Empty;

			if( type == WirePortDataType.INT )
				return m_wirePortToCgType[ type ];

			if( type == WirePortDataType.UINT )
				return m_wirePortToCgType[ type ];

			return string.Format( m_precisionWirePortToCgType[ type ], m_precisionTypeToCg[ finalPrecision ] );
		}

		public static string PrecisionWirePortToCgType( PrecisionType precisionType, WirePortDataType type )
		{
			if( type == WirePortDataType.OBJECT )
				return string.Empty;

			if( type == WirePortDataType.INT )
				return m_wirePortToCgType[ type ];

			if( type == WirePortDataType.UINT )
				return m_wirePortToCgType[ type ];

			return string.Format( m_precisionWirePortToCgType[ type ], m_precisionTypeToCg[ precisionType ] );
		}

		public static string GetAutoSwizzle( WirePortDataType type )
		{
			return m_autoSwizzle[ type ];
		}

		public static Color GetColorForDataType( WirePortDataType dataType, bool monochromeMode = true, bool isInput = true )
		{
			if( monochromeMode )
			{
				return isInput ? m_dataTypeToColorMonoMode[ 0 ] : m_dataTypeToColorMonoMode[ 1 ];
			}
			else
			{
				if ( m_dataTypeToColor.ContainsKey( dataType ) )
					return m_dataTypeToColor[ dataType ];
			}
			return m_dataTypeToColor[ WirePortDataType.OBJECT ];
		}

		public static bool IsValidType( WirePortDataType type )
		{
			switch ( type )
			{
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				case WirePortDataType.SAMPLER1D:
				case WirePortDataType.SAMPLER2D:
				case WirePortDataType.SAMPLER3D:
				case WirePortDataType.SAMPLERCUBE:
				return true;
			}
			return false;
		}
		public static string GetNameForDataType( WirePortDataType dataType ) { return m_dataTypeToName[ dataType ]; }

		public static string GetInputDeclarationFromType( PrecisionType precision, SurfaceInputs inputType )
		{
			string precisionStr = m_precisionTypeToCg[ precision ];
			return string.Format( m_inputTypeDeclaration[ inputType ], precisionStr );
		}

		public static string GetInputValueFromType( SurfaceInputs inputType ) { return m_inputTypeName[ inputType ]; }
		private static string CreateLocalValueName( PrecisionType precision, WirePortDataType dataType, string localOutputValue, string value ) { return string.Format( Constants.LocalValueDecWithoutIdent, PrecisionWirePortToCgType( precision, dataType ), localOutputValue, value ); }

		public static string CastPortType( ref MasterNodeDataCollector dataCollector, PrecisionType nodePrecision, NodeCastInfo castInfo, object value, WirePortDataType oldType, WirePortDataType newType, string parameterName = null )
		{
			if( oldType == newType || newType == WirePortDataType.OBJECT )
			{
				return ( parameterName != null ) ? parameterName : value.ToString();
			}

			PrecisionType currentPrecision = GetFinalPrecision( nodePrecision );
			string precisionStr = m_precisionTypeToCg[ currentPrecision ];
			string newTypeStr = m_wirePortToCgType[ newType ];
			newTypeStr = m_textInfo.ToTitleCase( newTypeStr );
			int castId = ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation ) ? dataCollector.AvailableVertexTempId : dataCollector.AvailableFragTempId;
			string localVarName = "temp_cast_" + castId;//m_wirePortToCgType[ oldType ] + "To" + newTypeStr + "_" + castInfo.ToString();
			string result = string.Empty;
			bool useRealValue = ( parameterName == null );

			switch( oldType )
			{
				case WirePortDataType.FLOAT:
				{
					switch( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? value.ToString() : parameterName; break;
						case WirePortDataType.FLOAT2:
						{
							string localVal = CreateLocalValueName( currentPrecision, newType, localVarName, string.Format( Constants.CastHelper, ( ( useRealValue ) ? value.ToString() : parameterName ), "xx" ) );
							dataCollector.AddToLocalVariables( dataCollector.PortCategory, -1, localVal );
							result = localVarName;
						}
						break;
						case WirePortDataType.FLOAT3:
						{
							string localVal = CreateLocalValueName( currentPrecision, newType, localVarName, string.Format( Constants.CastHelper, ( ( useRealValue ) ? value.ToString() : parameterName ), "xxx" ) );
							dataCollector.AddToLocalVariables( dataCollector.PortCategory, -1, localVal );
							result = localVarName;
						}
						break;
						case WirePortDataType.COLOR:
						{
							string localVal = CreateLocalValueName( currentPrecision, newType, localVarName, string.Format( Constants.CastHelper, ( ( useRealValue ) ? value.ToString() : parameterName ), "xxxx" ) );
							dataCollector.AddToLocalVariables( dataCollector.PortCategory, -1, localVal );
							result = localVarName;
						}
						break;
						case WirePortDataType.FLOAT4:
						{
							string localVal = CreateLocalValueName( currentPrecision, newType, localVarName, string.Format( Constants.CastHelper, ( ( useRealValue ) ? value.ToString() : parameterName ), "xxxx" ) );
							dataCollector.AddToLocalVariables( dataCollector.PortCategory, -1, localVal );
							result = localVarName;
						}
						break;
						case WirePortDataType.FLOAT3x3:
						{
							string localVal = CreateLocalValueName( currentPrecision, newType, localVarName, ( ( useRealValue ) ? value.ToString() : parameterName ) );
							dataCollector.AddToLocalVariables( dataCollector.PortCategory, -1, localVal );
							result = localVarName;
						}
						break;
						case WirePortDataType.FLOAT4x4:
						{
							string localVal = CreateLocalValueName( currentPrecision, newType, localVarName, ( ( useRealValue ) ? value.ToString() : parameterName ) );
							dataCollector.AddToLocalVariables( dataCollector.PortCategory, -1, localVal );
							result = localVarName;
						}
						break;
						case WirePortDataType.INT:
						{
							result = ( useRealValue ) ? ( (int)value ).ToString() : "(int)" + parameterName;
						}
						break;
					}
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					Vector2 vecVal = useRealValue ? (Vector2)value : Vector2.zero;
					switch( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? precisionStr + "2( " + vecVal.x + " , " + vecVal.y + " )" : parameterName; break;
						case WirePortDataType.FLOAT:
						{
							result = ( useRealValue ) ? vecVal.x.ToString() : parameterName + ".x";
						}
						break;
						case WirePortDataType.FLOAT3:
						{
							result = ( useRealValue ) ? precisionStr + "3( " + vecVal.x + " , " + vecVal.y + " , " + " 0.0 )" : precisionStr + "3( " + parameterName + " ,  0.0 )";
						}
						break;
						case WirePortDataType.COLOR:
						case WirePortDataType.FLOAT4:
						{
							result = ( useRealValue ) ? precisionStr + "4( " + vecVal.x + " , " + vecVal.y + " , " + " 0.0 , 0.0 )" : precisionStr + "4( " + parameterName + ", 0.0 , 0.0 )";
						}
						break;
					}
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					Vector3 vecVal = useRealValue ? (Vector3)value : Vector3.zero;
					switch( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? precisionStr + "3( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " )" : parameterName; break;
						case WirePortDataType.FLOAT:
						{
							result = ( useRealValue ) ? vecVal.x.ToString() : parameterName + ".x";
						}
						break;
						case WirePortDataType.FLOAT2:
						{
							result = ( useRealValue ) ? precisionStr + "2( " + vecVal.x + " , " + vecVal.y + " )" : parameterName + ".xy";
						}
						break;
						case WirePortDataType.COLOR:
						case WirePortDataType.FLOAT4:
						{
							result = ( useRealValue ) ? precisionStr + "4( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " , 0.0 )" : precisionStr + "4( " + parameterName + " , 0.0 )";
						}
						break;
						//case WirePortDataType.FLOAT3x3:
						//{
						//	if ( useRealValue )
						//	{
						//		result = precisionStr + "3x3( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " , " +
						//									vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " , " +
						//									vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " )";
						//	}
						//	else
						//	{
						//		string localVal = CreateLocalValueName( currentPrecision, newType, localVarName, parameterName );
						//		CurrentDataCollector.AddToLocalVariables( portCategory, -1, localVal );
						//		result = precisionStr + "3x3( " + localVarName + ".x , " + localVarName + ".y , " + localVarName + ".x , " +
						//							   localVarName + ".x , " + localVarName + ".y , " + localVarName + ".y , " +
						//							   localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z )";
						//	}
						//}
						//break;
						//case WirePortDataType.FLOAT4x4:
						//{
						//	if ( useRealValue )
						//	{
						//		result = precisionStr + "4x4( " + vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , 0 , " +
						//								vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , 0 , " +
						//								vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , 0 , " +
						//								vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , 0 )";
						//	}
						//	else
						//	{
						//		string localVal = CreateLocalValueName( currentPrecision, newType, localVarName, parameterName );
						//		CurrentDataCollector.AddToLocalVariables( portCategory, -1, localVal );
						//		result = precisionStr + "4x4( " + localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , 0 , " +
						//								localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , 0 , " +
						//								localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , 0 , " +
						//								localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , 0 )";
						//	}
						//}
						//break;
					}
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					Vector4 vecVal = useRealValue ? (Vector4)value : Vector4.zero;
					switch( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? precisionStr + "4( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " , " + vecVal.w + " )" : parameterName; break;
						case WirePortDataType.FLOAT:
						{
							result = ( useRealValue ) ? vecVal.x.ToString() : parameterName + ".x";
						}
						break;
						case WirePortDataType.FLOAT2:
						{
							result = ( useRealValue ) ? precisionStr + "2( " + vecVal.x + " , " + vecVal.y + " )" : parameterName + ".xy";
						}
						break;
						case WirePortDataType.FLOAT3:
						{
							result = ( useRealValue ) ? precisionStr + "3( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " )" : parameterName + ".xyz";
						}
						break;
						//case WirePortDataType.FLOAT4x4:
						//{
						//	if ( useRealValue )
						//	{
						//		result = precisionStr + "4x4( " + vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , " + vecVal + ".w , " +
						//								vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , " + vecVal + ".w , " +
						//								vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , " + vecVal + ".w , " +
						//								vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , " + vecVal + ".w )";
						//	}
						//	else
						//	{
						//		string localVal = CreateLocalValueName( currentPrecision, newType, localVarName, parameterName );
						//		CurrentDataCollector.AddToLocalVariables( portCategory, -1, localVal );
						//		result = precisionStr + "4x4( " + localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
						//								localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
						//								localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
						//								localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w )";
						//	}
						//}
						//break;
						case WirePortDataType.COLOR:
						{
							result = useRealValue ? precisionStr + "4( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " , " + vecVal.w + " )" : parameterName;
						}
						break;
					}
				}
				break;
				case WirePortDataType.FLOAT3x3:
				{
					//Matrix4x4 matrixVal = useRealValue ? ( Matrix4x4 ) value : Matrix4x4.identity;
					//switch ( newType )
					//{
					//	case WirePortDataType.OBJECT:
					//	case WirePortDataType.FLOAT4x4:
					//	{
					//		result = ( useRealValue ) ? precisionStr + "4x4(" + matrixVal.m00 + " , " + matrixVal.m01 + " , " + matrixVal.m02 + " , " + matrixVal.m03 + " , " +
					//													matrixVal.m10 + " , " + matrixVal.m11 + " , " + matrixVal.m12 + " , " + matrixVal.m10 + " , " +
					//													matrixVal.m20 + " , " + matrixVal.m21 + " , " + matrixVal.m22 + " , " + matrixVal.m20 + " , " +
					//													matrixVal.m30 + " , " + matrixVal.m31 + " , " + matrixVal.m32 + " , " + matrixVal.m30 + " )" : precisionStr + "4x4(" + parameterName + ")";
					//	}
					//	break;
					//}
				}
				break;
				case WirePortDataType.FLOAT4x4:
				{
					Matrix4x4 matrixVal = useRealValue ? (Matrix4x4)value : Matrix4x4.identity;
					switch( newType )
					{
						case WirePortDataType.OBJECT:
						{
							result = ( useRealValue ) ? precisionStr + "4x4(" + matrixVal.m00 + " , " + matrixVal.m01 + " , " + matrixVal.m02 + " , " + matrixVal.m03 + " , " +
																		matrixVal.m10 + " , " + matrixVal.m11 + " , " + matrixVal.m12 + " , " + matrixVal.m10 + " , " +
																		matrixVal.m20 + " , " + matrixVal.m21 + " , " + matrixVal.m22 + " , " + matrixVal.m20 + " , " +
																		matrixVal.m30 + " , " + matrixVal.m31 + " , " + matrixVal.m32 + " , " + matrixVal.m30 + " )" : parameterName;
						}
						break;
					}
				}
				break;
				case WirePortDataType.COLOR:
				{
					Color colorValue = ( useRealValue ) ? (Color)value : Color.black;
					switch( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? precisionStr + "4( " + colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " )" : parameterName; break;
						case WirePortDataType.FLOAT:
						{
							result = ( useRealValue ) ? colorValue.r.ToString() : parameterName + ".r";
						}
						break;
						case WirePortDataType.FLOAT2:
						{
							result = ( useRealValue ) ? precisionStr + "2( " + colorValue.r + " , " + colorValue.g + " )" : parameterName + ".rg";
						}
						break;
						case WirePortDataType.FLOAT3:
						{
							result = ( useRealValue ) ? precisionStr + "3( " + colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " )" : parameterName + ".rgb";
						}
						break;
						case WirePortDataType.FLOAT4:
						{
							result = useRealValue ? precisionStr + "4( " + colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " )" : parameterName;
						}
						break;
						//case WirePortDataType.FLOAT4x4:
						//{
						//	if ( useRealValue )
						//	{
						//		result = precisionStr + "4x4( " + colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " , " +
						//											colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " , " +
						//											colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " , " +
						//											colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " )";
						//	}
						//	else
						//	{
						//		string localVal = CreateLocalValueName( currentPrecision, newType, localVarName, parameterName );
						//		CurrentDataCollector.AddToLocalVariables( portCategory, -1, localVal );

						//		result = precisionStr + "4x4( " + localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
						//								localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
						//								localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
						//								localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w )";
						//	}
						//}
						//break;
					}
				}
				break;
				case WirePortDataType.INT:
				{
					switch( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? value.ToString() : parameterName; break;
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT3:
						case WirePortDataType.COLOR:
						case WirePortDataType.FLOAT4:
						{
							string localVal = CreateLocalValueName( currentPrecision, newType, localVarName, ( ( useRealValue ) ? value.ToString() : parameterName ) );
							dataCollector.AddToLocalVariables( dataCollector.PortCategory, -1, localVal );
							result = localVarName;
						}
						break;
						case WirePortDataType.FLOAT3x3:
						{
							string localVal = CreateLocalValueName( currentPrecision, oldType, localVarName, ( ( useRealValue ) ? value.ToString() : parameterName ) );
							dataCollector.AddToLocalVariables( dataCollector.PortCategory, -1, localVal );
							result = localVarName;
						}
						break;
						case WirePortDataType.FLOAT4x4:
						{
							string localVal = CreateLocalValueName( currentPrecision, oldType, localVarName, ( ( useRealValue ) ? value.ToString() : parameterName ) );
							dataCollector.AddToLocalVariables( dataCollector.PortCategory, -1, localVal );
							result = localVarName;
						}
						break;
						case WirePortDataType.FLOAT:
						{
							result = ( useRealValue ) ? ( (int)value ).ToString() : "(float)" + parameterName;
						}
						break;
					}
				}
				break;

			}
			if( result.Equals( string.Empty ) )
			{
				result = "0";
				string warningStr = string.Format( "Unable to cast from {0} to {1}. Generating dummy data ( {2} )", oldType, newType, result );

				if( oldType == WirePortDataType.SAMPLER1D || oldType == WirePortDataType.SAMPLER2D || oldType == WirePortDataType.SAMPLER3D || oldType == WirePortDataType.SAMPLERCUBE )
				{
					warningStr = string.Format( "Unable to cast from {0} to {1}. You might want to use a Texture Sample node and connect it to the 'Tex' port. Generating dummy data ( {2} )", oldType, newType, result );
				}
				ShowMessage( warningStr, MessageSeverity.Warning );
			}
			return result;
		}

		public static bool CanCast( WirePortDataType from, WirePortDataType to )
		{
			if( from == WirePortDataType.OBJECT || to == WirePortDataType.OBJECT || from == to )
				return true;

			switch( from )
			{
				case WirePortDataType.FLOAT:
				{
					if( to == WirePortDataType.INT )
						return true;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					return false;
				}
				case WirePortDataType.FLOAT3:
				{
					if( to == WirePortDataType.COLOR ||
						to == WirePortDataType.FLOAT4 )
						return true;
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					if( to == WirePortDataType.FLOAT3 ||
						to == WirePortDataType.COLOR )
						return true;
				}
				break;
				case WirePortDataType.FLOAT3x3:
				{
					if( to == WirePortDataType.FLOAT4x4 )
						return true;
				}
				break;
				case WirePortDataType.FLOAT4x4:
				{
					if( to == WirePortDataType.FLOAT3x3 )
						return true;
				}
				break;
				case WirePortDataType.COLOR:
				{
					if( to == WirePortDataType.FLOAT3 ||
						to == WirePortDataType.FLOAT4 )
						return true;

				}
				break;
				case WirePortDataType.INT:
				{
					if( to == WirePortDataType.FLOAT )
						return true;
				}
				break;
			}

			return false;
		}

		public static int GetChannelsAmount( WirePortDataType type )
		{
			switch( type )
			{
				case WirePortDataType.OBJECT: return 0;
				case WirePortDataType.FLOAT: return 1;
				case WirePortDataType.FLOAT2: return 2;
				case WirePortDataType.FLOAT3: return 3;
				case WirePortDataType.FLOAT4: return 4;
				case WirePortDataType.FLOAT3x3: return 9;
				case WirePortDataType.FLOAT4x4: return 16;
				case WirePortDataType.COLOR: return 4;
				case WirePortDataType.INT: return 1;
				case WirePortDataType.UINT: return 1;
			}
			return 0;
		}

		public static WirePortDataType GetWireTypeForChannelAmount( int channelAmount )
		{
			switch( channelAmount )
			{
				case 1: return WirePortDataType.FLOAT;
				case 2: return WirePortDataType.FLOAT2;
				case 3: return WirePortDataType.FLOAT3;
				case 4: return WirePortDataType.FLOAT4;
				case 9: return WirePortDataType.FLOAT3x3;
				case 16: return WirePortDataType.FLOAT4x4;
			}
			return WirePortDataType.FLOAT;
		}

		public static string GenerateUniformName( WirePortDataType dataType, string dataName ) { return string.Format( Constants.UniformDec, WirePortToCgType( dataType ), dataName ); }
		public static string GenerateUniformName( string dataType, string dataName ) { return string.Format( Constants.UniformDec, dataType, dataName ); }

		public static string GeneratePropertyName( string name, PropertyType propertyType, bool forceUnderscore = false )
		{
			if( string.IsNullOrEmpty( name ) )
				return name;

			name = RemoveInvalidCharacters( name );
			if( propertyType != PropertyType.Global || forceUnderscore )
			{
				if( name[ 0 ] != '_' )
				{
					name = '_' + name;
				}
			}

			return name;
		}

		public static string UrlReplaceInvalidStrings( string originalString )
		{
			foreach( KeyValuePair<string, string> kvp in Constants.UrlReplacementStringValues )
			{
				originalString = originalString.Replace( kvp.Key, kvp.Value );
			}
			return originalString;
		}

		public static string ReplaceInvalidStrings( string originalString )
		{
			foreach( KeyValuePair<string, string> kvp in Constants.ReplacementStringValues )
			{
				originalString = originalString.Replace( kvp.Key, kvp.Value );
			}
			return originalString;
		}

		public static string RemoveWikiInvalidCharacters( string originalString )
		{
			for( int i = 0; i < Constants.WikiInvalidChars.Length; i++ )
			{
				originalString = originalString.Replace( Constants.WikiInvalidChars[ i ], string.Empty );
			}
			return originalString;
		}

		public static string RemoveInvalidEnumCharacters( string originalString )
		{
			for( int i = 0; i < Constants.EnumInvalidChars.Length; i++ )
			{
				originalString = originalString.Replace( Constants.EnumInvalidChars[ i ], string.Empty );
			}
			return originalString;
		}

		public static string RemoveInvalidAttrCharacters( string originalString )
		{
			for( int i = 0; i < Constants.AttrInvalidChars.Length; i++ )
			{
				originalString = originalString.Replace( Constants.AttrInvalidChars[ i ], string.Empty );
			}
			return originalString;
		}

		public static string RemoveInvalidCharacters( string originalString )
		{
			for( int i = 0; i < Constants.OverallInvalidChars.Length; i++ )
			{
				originalString = originalString.Replace( Constants.OverallInvalidChars[ i ], string.Empty );
			}
			return originalString;
		}

		public static string RemoveShaderInvalidCharacters( string originalString )
		{
			originalString = originalString.Replace( '\\', '/' );
			for( int i = 0; i < Constants.ShaderInvalidChars.Length; i++ )
			{
				originalString = originalString.Replace( Constants.ShaderInvalidChars[ i ], string.Empty );
			}
			return originalString;
		}

		public static bool IsUnityNativeShader( Shader shader )
		{
			string pathName = AssetDatabase.GetAssetPath( shader );

			if( pathName.Contains( "unity_builtin_extra") || 
				pathName.Contains( "unity default resources" ))
			return true;

			return false;
		}
		public static bool IsUnityNativeShader( string path ) { return m_unityNativeShaderPaths.ContainsKey( path ); }

		public static string GetComponentForPosition( int pos, WirePortDataType type, bool addDot = false )
		{
			string result = addDot ? "." : string.Empty;
			switch( pos )
			{
				case 0:
				{
					return ( ( type == WirePortDataType.COLOR ) ? ( result + "r" ) : ( result + "x" ) );
				}
				case 1:
				{
					return ( ( type == WirePortDataType.COLOR ) ? ( result + "g" ) : ( result + "y" ) );
				}
				case 2:
				{
					return ( ( type == WirePortDataType.COLOR ) ? ( result + "b" ) : ( result + "z" ) );
				}
				case 3:
				{
					return ( ( type == WirePortDataType.COLOR ) ? ( result + "a" ) : ( result + "w" ) );
				}
			}
			return string.Empty;
		}

		public static string InvalidParameter( ParentNode node )
		{
			ShowMessage( "Invalid entrance type on node" + node, MessageSeverity.Error );
			return "0";
		}

		public static string NoConnection( ParentNode node )
		{
			ShowMessage( "No Input connection on node" + node, MessageSeverity.Error );
			return "0";
		}

		public static string UnknownError( ParentNode node )
		{
			ShowMessage( "Unknown error on node" + node, MessageSeverity.Error );
			return "0";
		}

		public static string GetTex2DProperty( string name, TexturePropertyValues defaultValue ) { return name + "(\"" + name + "\", 2D) = \"" + defaultValue + "\" {}"; }
		public static string AddBrackets( string value ) { return "( " + value + " )"; }
		public static Color GetColorFromWireStatus( WireStatus status ) { return m_wireStatusToColor[ status ]; }
		public static bool HasColorCategory( string category ) { return m_nodeCategoryToColor.ContainsKey( category ); }
		public static void AddColorCategory( string category, Color color )
        {
            m_nodeCategoryToColor.Add( category, color );
        }

        public static Color AddColorCategory( string category, string hexColor )
        {
            try
            {
                Color color = new Color();
                ColorUtility.TryParseHtmlString( hexColor, out color );
                m_nodeCategoryToColor.Add( category, color );
                return color;
            }
            catch( System.Exception e )
            {
                Debug.LogException( e );    
            }
            return m_nodeCategoryToColor[ "Default" ];
        }

        public static Color GetColorFromCategory( string category )
		{
			if( m_nodeCategoryToColor.ContainsKey( category ) )
				return m_nodeCategoryToColor[ category ];


            if(DebugConsoleWindow.DeveloperMode) 
			    Debug.LogWarning( category + " category does not contain an associated color" );

			return m_nodeCategoryToColor[ "Default" ];
		}

		public static string LatestOpenedFolder
		{
			get { return m_latestOpenedFolder; }
			set { m_latestOpenedFolder = value; }
		}

		public static Shader CreateNewUnlit()
		{
			if( CurrentWindow == null )
				return null;

			string shaderName;
			string pathName;
			Shader newShader = null;
			IOUtils.GetShaderName( out shaderName, out pathName, "MyUnlitShader", m_latestOpenedFolder );
			if( !System.String.IsNullOrEmpty( shaderName ) && !System.String.IsNullOrEmpty( pathName ) )
			{
				CurrentWindow.CreateNewGraph( shaderName );
				CurrentWindow.PreMadeShadersInstance.FlatColorSequence.Execute();

				CurrentWindow.CurrentGraph.CurrentMasterNode.SetName( shaderName );
				newShader = CurrentWindow.CurrentGraph.FireMasterNode( pathName, true );
				AssetDatabase.Refresh();
			}
			return newShader;
		}

		public static Shader CreateNewEmpty( string customPath = null )
		{
			if( CurrentWindow == null )
				return null;

			string shaderName;
			string pathName;
			Shader newShader = null;


			string path = AssetDatabase.GetAssetPath( Selection.activeObject );
			if( path == "" )
			{
				path = "Assets";
			}
			else if( System.IO.Path.GetExtension( path ) != "" )
			{
				path = path.Replace( System.IO.Path.GetFileName( AssetDatabase.GetAssetPath( Selection.activeObject ) ), "" );
			}


			if( string.IsNullOrEmpty( customPath ) )
			{
				IOUtils.GetShaderName( out shaderName, out pathName, Constants.DefaultShaderName, m_latestOpenedFolder );
			}
			else
			{
				pathName = customPath;
				shaderName = Constants.DefaultShaderName;
				int indexOfAssets = pathName.IndexOf( "Assets" );
				string uniquePath = ( indexOfAssets > 0 )? pathName.Remove( 0, indexOfAssets ) : pathName;
				string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath( uniquePath + shaderName + ".shader" );
				pathName = assetPathAndName;
				shaderName = assetPathAndName.Remove( 0, assetPathAndName.IndexOf( shaderName ) );
				shaderName = shaderName.Remove( shaderName.Length - 7 );
			}
			if( !System.String.IsNullOrEmpty( shaderName ) && !System.String.IsNullOrEmpty( pathName ) )
			{
				m_latestOpenedFolder = pathName;

				CurrentWindow.titleContent.text = AmplifyShaderEditorWindow.GenerateTabTitle( shaderName );
				CurrentWindow.titleContent.image = ShaderIcon;
				CurrentWindow.CreateNewGraph( shaderName );
				CurrentWindow.LastOpenedLocation = pathName;
				CurrentWindow.CurrentGraph.CurrentMasterNode.SetName( shaderName );
				newShader = CurrentWindow.CurrentGraph.FireMasterNode( pathName, true );
				AssetDatabase.Refresh();
			}

			return newShader;
		}


		public static Shader CreateNewEmptyTemplate( string templateGUID, string customPath = null )
		{
			if( CurrentWindow == null )
				return null;

			string shaderName;
			string pathName;
			Shader newShader = null;


			string path = AssetDatabase.GetAssetPath( Selection.activeObject );
			if( path == "" )
			{
				path = "Assets";
			}
			else if( System.IO.Path.GetExtension( path ) != "" )
			{
				path = path.Replace( System.IO.Path.GetFileName( AssetDatabase.GetAssetPath( Selection.activeObject ) ), "" );
			}


			if( string.IsNullOrEmpty( customPath ) )
			{
				IOUtils.GetShaderName( out shaderName, out pathName, Constants.DefaultShaderName, m_latestOpenedFolder );
			}
			else
			{
				pathName = customPath;
				shaderName = Constants.DefaultShaderName;
				int indexOfAssets = pathName.IndexOf( "Assets" );
				string uniquePath = ( indexOfAssets > 0 ) ? pathName.Remove( 0, indexOfAssets ) : pathName;
				string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath( uniquePath + shaderName + ".shader" );
				pathName = assetPathAndName;
				shaderName = assetPathAndName.Remove( 0, assetPathAndName.IndexOf( shaderName ) );
				shaderName = shaderName.Remove( shaderName.Length - 7 );
			}
			if( !System.String.IsNullOrEmpty( shaderName ) && !System.String.IsNullOrEmpty( pathName ) )
			{
				m_latestOpenedFolder = pathName;

				CurrentWindow.titleContent.text = AmplifyShaderEditorWindow.GenerateTabTitle( shaderName );
				CurrentWindow.titleContent.image = UIUtils.ShaderIcon;
				CurrentWindow.CreateNewTemplateGraph( templateGUID );
				CurrentWindow.CurrentGraph.CurrentMasterNode.SetName( shaderName );
				newShader = CurrentWindow.CurrentGraph.FireMasterNode( pathName, true );
				AssetDatabase.Refresh();
			}

			return newShader;
		}


		public static void SetDelayedMaterialMode( Material material )
		{
			if( CurrentWindow == null )
				return;
			CurrentWindow.SetDelayedMaterialMode( material );
		}

		public static void CreateEmptyFromInvalid( Shader shader )
		{
			if( CurrentWindow == null )
				return;

			CurrentWindow.CreateNewGraph( shader );
			CurrentWindow.ForceRepaint();
		}

		public static void CreateEmptyFunction( AmplifyShaderFunction shaderFunction )
		{
			if( CurrentWindow == null )
				return;

			CurrentWindow.CreateNewFunctionGraph( shaderFunction );
			CurrentWindow.SaveToDisk( false );
			CurrentWindow.ForceRepaint();
		}

		public static void DrawFloat( UndoParentNode owner, ref Rect propertyDrawPos, ref float value, float newLabelWidth = 8 )
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = newLabelWidth;
			value = owner.EditorGUIFloatField( propertyDrawPos, "  ", value, UIUtils.MainSkin.textField );
			EditorGUIUtility.labelWidth = labelWidth;
		}

		public static GUIStyle GetCustomStyle( CustomStyle style )
		{
			return ( Initialized ) ? MainSkin.customStyles[ (int)style ] : null;
		}

		public static void SetCustomStyle( CustomStyle style, GUIStyle guiStyle )
		{
			if( MainSkin != null )
				MainSkin.customStyles[ (int)style ] = new GUIStyle( guiStyle );
		}

		public static void OpenFile()
		{
			if( CurrentWindow == null )
				return;
			string newShader = EditorUtility.OpenFilePanel( "Select Shader to open", m_latestOpenedFolder, "shader" );
			if( !System.String.IsNullOrEmpty( newShader ) )
			{
				m_latestOpenedFolder = newShader.Substring( 0, newShader.LastIndexOf( '/' ) + 1 );
				int relFilenameId = newShader.IndexOf( Application.dataPath );
				if( relFilenameId > -1 )
				{
					string relFilename = newShader.Substring( relFilenameId + Application.dataPath.Length - 6 );// -6 need to also copy the assets/ part
					CurrentWindow.LoadFromDisk( relFilename );
				}
				else
				{
					ShowMessage( "Can only load shaders\nfrom inside the projects folder", MessageSeverity.Error );
				}
			}
		}

		public static bool DetectNodeLoopsFrom( ParentNode node, Dictionary<int, int> currentNodes )
		{
			if( currentNodes.ContainsKey( node.UniqueId ) )
			{
				currentNodes.Clear();
				currentNodes = null;
				return true;
			}

			currentNodes.Add( node.UniqueId, 1 );
			bool foundLoop = false;
			for( int i = 0; i < node.InputPorts.Count; i++ )
			{
				if( node.InputPorts[ i ].IsConnected )
				{
					ParentNode newNode = node.InputPorts[ i ].GetOutputNode();
					if( newNode.InputPorts.Count > 0 )
					{
						Dictionary<int, int> newDict = new Dictionary<int, int>();
						foreach( KeyValuePair<int, int> entry in currentNodes )
						{
							newDict.Add( entry.Key, entry.Value );
						}
						foundLoop = foundLoop || DetectNodeLoopsFrom( newNode, newDict );
						if( foundLoop )
							break;
					}
				}
			}

			currentNodes.Clear();
			currentNodes = null;

			return foundLoop;
		}

		public static ParentNode CreateNode( System.Type type, bool registerUndo, Vector2 pos, int nodeId = -1, bool addLast = true )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.CreateNode( type, registerUndo, pos, nodeId, addLast );
			}
			return null;
		}

		public static void DestroyNode( int nodeId )
		{
			if( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.DestroyNode( nodeId );
			}
		}

		public static void ShowMessage( string message, MessageSeverity severity = MessageSeverity.Normal, bool registerTimestamp = true )
		{
			if( CurrentWindow != null )
			{
				CurrentWindow.ShowMessage( message, severity, registerTimestamp );
			}
		}

		public static ParentNode GetNode( int nodeId )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.GetNode( nodeId );
			}
			return null;
		}

		public static PropertyNode GetInternalTemplateNode( int nodeId )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.GetInternalTemplateNode( nodeId );
			}
			return null;
		}

		public static void DeleteConnection( bool isInput, int nodeId, int portId, bool registerOnLog, bool propagateCallback )
		{
			if( CurrentWindow != null )
			{
				CurrentWindow.DeleteConnection( isInput, nodeId, portId, registerOnLog, propagateCallback );
			}
		}

		public static void ConnectInputToOutput( int inNodeId, int inPortId, int outNodeId, int outPortId )
		{
			if( CurrentWindow != null )
			{
				CurrentWindow.ConnectInputToOutput( inNodeId, inPortId, outNodeId, outPortId );
			}
		}

		public static Shader CreateNewGraph( string name )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.CreateNewGraph( name );
			}
			return null;
		}
		public static void SetConnection( int InNodeId, int InPortId, int OutNodeId, int OutPortId )
		{
			if( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.SetConnection( InNodeId, InPortId, OutNodeId, OutPortId );
			}
		}

		public static bool IsChannelAvailable( int channelId )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.IsChannelAvailable( channelId );
			}
			return false;
		}

		public static bool ReleaseUVChannel( int nodeId, int channelId )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.ReleaseUVChannel( nodeId, channelId );
			}
			return false;
		}

		public static bool RegisterUVChannel( int nodeId, int channelId, string name )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.RegisterUVChannel( nodeId, channelId, name );
			}
			return false;
		}

		public static void GetFirstAvailableName( int nodeId, WirePortDataType type, out string outProperty, out string outInspector, bool useCustomPrefix = false, string customPrefix = null )
		{
			outProperty = string.Empty;
			outInspector = string.Empty;
			if( CurrentWindow != null )
			{
				CurrentWindow.DuplicatePrevBufferInstance.GetFirstAvailableName( nodeId, type, out outProperty, out outInspector, useCustomPrefix, customPrefix );
			}
		}

		
		public static bool IsNumericName( string name )
		{
			Match match = Regex.Match( name, NumericNamePattern );
			if( match != null && match.Success )
				return true;
			return false;
		}

		public static bool CheckInvalidUniformName( string name )
		{
			if( m_reservedPropertyNames.ContainsKey( name ) )
			{
				ShowMessage( string.Format( Constants.ReservedPropertyNameStr, name ) );
				return true;
			}
			
			if( IsNumericName( name ))
			{
				ShowMessage( string.Format( Constants.NumericPropertyNameStr, name ) );
				return true;
			}

			return false;
		}

		public static bool RegisterUniformName( int nodeId, string name )
		{
			if( CheckInvalidUniformName( name ) )
			{
				return false;
			}
			
			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.RegisterUniformName( nodeId, name );
			}
			return false;
		}

		public static bool ReleaseUniformName( int nodeId, string name )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.ReleaseUniformName( nodeId, name );
			}
			return false;
		}

		public static bool IsUniformNameAvailable( string name )
		{
			if( CheckInvalidUniformName( name ) )
			{
				return false;
			}

			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.IsUniformNameAvailable( name );
			}
			return false;
		}

		public static int CheckUniformNameOwner( string name )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.CheckUniformNameOwner( name );
			}
			return -1;
		}

		public static bool RegisterLocalVariableName( int nodeId, string name )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.RegisterLocalVariableName( nodeId, name );
			}
			return false;
		}

		public static bool ReleaseLocalVariableName( int nodeId, string name )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.ReleaseLocalVariableName( nodeId, name );
			}
			return false;
		}

		public static bool IsLocalvariableNameAvailable( string name )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.IsLocalvariableNameAvailable( name );
			}
			return false;
		}

		public static string GetChannelName( int channelId )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.GetChannelName( channelId );
			}
			return string.Empty;
		}

		public static void SetChannelName( int channelId, string name )
		{
			if( CurrentWindow != null )
			{
				CurrentWindow.DuplicatePrevBufferInstance.SetChannelName( channelId, name );
			}
		}

		public static int RegisterFirstAvailableChannel( int nodeId, string name )
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.RegisterFirstAvailableChannel( nodeId, name );
			}
			return -1;
		}

		public static int PortCategorytoAttayIdx( MasterNodePortCategory category )
		{
			if( m_portCategoryToArrayIdx.ContainsKey( category ))
				return m_portCategoryToArrayIdx[category];

			return m_portCategoryToArrayIdx[ MasterNodePortCategory.Fragment ];
		} 

		public static bool DisplayDialog( string shaderPath )
		{
			string value = System.String.Format( "Save changes to the shader {0} before closing?", shaderPath );
			return EditorUtility.DisplayDialog( "Load selected", value, "Yes", "No" );
		}

		public static void ForceUpdateFromMaterial()
		{
			if( CurrentWindow != null )
			{
				//				CurrentWindow.Focus();
				CurrentWindow.ForceUpdateFromMaterial();
			}
		}

		public static void MarkToRepaint() { if( CurrentWindow != null ) CurrentWindow.MarkToRepaint(); }
		public static void RequestSave() { if( CurrentWindow != null ) CurrentWindow.RequestSave(); }
		public static string FloatToString( float value )
		{
			string floatStr = value.ToString();
			if( value % 1 == 0 )
			{
				floatStr += ".0";
			}
			return floatStr;
		}

		public static int CurrentShaderVersion()
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.LoadedShaderVersion;
			}
			return -1;
		}

		public static bool IsProperty( PropertyType type ) { return ( type == PropertyType.Property || type == PropertyType.InstancedProperty ); }

		public static MasterNode CurrentMasterNode()
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.CurrentMasterNode;
			}
			return null;
		}

		public static void AddVirtualTextureCount() { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.AddVirtualTextureCount(); } }

		public static bool HasVirtualTexture()
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.HasVirtualTexture;
			}
			return false;
		}

		public static void RemoveVirtualTextureCount() { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.RemoveVirtualTextureCount(); } }

		//public static void AddInstancePropertyCount() { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.AddInstancePropertyCount(); } }

		public static bool IsInstancedShader()
		{
			if( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.IsInstancedShader;
			}
			return false;
		}

		//public static void RemoveInstancePropertyCount() { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.RemoveInstancePropertyCount(); } }
		//public static void AddNormalDependentCount() { if ( CurrentWindow != null ) { CurrentWindow.CurrentGraph.AddNormalDependentCount(); } }
		//public static void RemoveNormalDependentCount() { if ( CurrentWindow != null ) { CurrentWindow.CurrentGraph.RemoveNormalDependentCount(); } }
		//public static bool IsNormalDependent()
		//{
		//	if ( CurrentWindow != null )
		//	{
		//		return CurrentWindow.CurrentGraph.IsNormalDependent;
		//	}
		//	return false;
		//}

		public static void CopyValuesFromMaterial( Material mat )
		{
			if( CurrentWindow != null && CurrentWindow.CurrentMaterial == mat )
			{
				CurrentWindow.CurrentGraph.CopyValuesFromMaterial( mat );
			}
			else
			{
				int aseWindowCount = IOUtils.AllOpenedWindows.Count;
				for( int i = 0; i < aseWindowCount; i++ )
				{
					if( IOUtils.AllOpenedWindows[ i ] != m_currentWindow && IOUtils.AllOpenedWindows[ i ].CurrentMaterial == mat )
					{
						IOUtils.AllOpenedWindows[ i ].CurrentGraph.CopyValuesFromMaterial( mat );
						break;
					}
				}
			}
		}

		// Sampler Node
		public static void RegisterSamplerNode( SamplerNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.SamplerNodes.AddNode( node ); } }
		public static void UnregisterSamplerNode( SamplerNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.SamplerNodes.RemoveNode( node ); } }
		public static string[] SamplerNodeArr() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.SamplerNodes.NodesArr; } return null; }
		public static SamplerNode GetSamplerNode( int idx ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.SamplerNodes.GetNode( idx ); } return null; }
		public static void UpdateSamplerDataNode( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.SamplerNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static int GetSamplerNodeRegisterId( int uniqueId ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.SamplerNodes.GetNodeRegisterIdx( uniqueId ); } return -1; }
		public static int GetSamplerNodeAmount() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.SamplerNodes.NodesList.Count; } return -1; }

		// Float Node
		public static void RegisterFloatIntNode( PropertyNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FloatIntNodes.AddNode( node ); } }
		public static void UnregisterFloatIntNode( PropertyNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FloatIntNodes.RemoveNode( node ); } }
		public static string[] FloatIntNodeArr() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FloatIntNodes.NodesArr; } return null; }
		public static int[] FloatIntNodeIds() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FloatIntNodes.NodeIds; } return null; }
		public static PropertyNode GetFloatIntNode( int idx ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FloatIntNodes.GetNode( idx ); } return null; }
		public static void UpdateFloatIntDataNode( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FloatIntNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static int GetFloatIntNodeRegisterId( int uniqueId ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FloatIntNodes.GetNodeRegisterIdx( uniqueId ); } return -1; }
		public static int GetNodeIdByName( string name )
		{
			if( CurrentWindow != null )
			{
				UsageListFloatIntNodes list = CurrentWindow.CurrentGraph.FloatIntNodes;
				int count = list.Count;
				for( int i = 0; i < count; i++ )
				{
					if( list.NodesList[ i ].PropertyName.Equals( name ) )
						return list.NodesList[ i ].UniqueId;
				}
			}
			return -1;
		}
		public static PropertyNode GetFloatIntNodeByUniqueId( int uniqueId ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FloatIntNodes.GetNodeByUniqueId( uniqueId ); } return null; }
		//public static int GetFloatNodeAmount() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FloatNodes.NodesList.Count; } return -1; }

		// Texture Property
		public static void RegisterTexturePropertyNode( TexturePropertyNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.TexturePropertyNodes.AddNode( node ); } }
		public static void UnregisterTexturePropertyNode( TexturePropertyNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.TexturePropertyNodes.RemoveNode( node ); } }
		public static string[] TexturePropertyNodeArr() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.TexturePropertyNodes.NodesArr; } return null; }
		public static TexturePropertyNode GetTexturePropertyNode( int idx ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.TexturePropertyNodes.GetNode( idx ); } return null; }
		public static void UpdateTexturePropertyDataNode( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.TexturePropertyNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static int GetTexturePropertyNodeRegisterId( int uniqueId ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.TexturePropertyNodes.GetNodeRegisterIdx( uniqueId ); } return -1; }
		public static int GetTexturePropertyNodeAmount() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.TexturePropertyNodes.NodesList.Count; } return -1; }

		// Texture Array
		public static void RegisterTextureArrayNode( TextureArrayNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.TextureArrayNodes.AddNode( node ); } }
		public static void UnregisterTextureArrayNode( TextureArrayNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.TextureArrayNodes.RemoveNode( node ); } }
		public static string[] TextureArrayNodeArr() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.TextureArrayNodes.NodesArr; } return null; }
		public static TextureArrayNode GetTextureArrayNode( int idx ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.TextureArrayNodes.GetNode( idx ); } return null; }
		public static void UpdateTextureArrayDataNode( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.TextureArrayNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static int GetTextureArrayNodeRegisterId( int uniqueId ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.TextureArrayNodes.GetNodeRegisterIdx( uniqueId ); } return -1; }
		public static int GetTextureArrayNodeAmount() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.TextureArrayNodes.NodesList.Count; } return -1; }

		// Raw Property Node
		public static void RegisterRawPropertyNode( PropertyNode node ) { if( CurrentWindow != null ) { CurrentWindow.OutsideGraph.RawPropertyNodes.AddNode( node ); } }
		public static void UnregisterRawPropertyNode( PropertyNode node ) { if( CurrentWindow != null ) { CurrentWindow.OutsideGraph.RawPropertyNodes.RemoveNode( node ); } }

		// Property Node
		public static void RegisterPropertyNode( PropertyNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.PropertyNodes.AddNode( node ); } }
		public static void UnregisterPropertyNode( PropertyNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.PropertyNodes.RemoveNode( node ); } }
		public static string[] PropertyNodeNodeArr() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.PropertyNodes.NodesArr; } return null; }
		public static PropertyNode GetPropertyNode( int idx ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.PropertyNodes.GetNode( idx ); } return null; }
		public static PropertyNode GetPropertyNodeByUniqueId( int uniqueId ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.PropertyNodes.GetNodeByUniqueId( uniqueId ); } return null; }
		public static void UpdatePropertyDataNode( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.PropertyNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static int GetPropertyNodeRegisterId( int uniqueId ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.PropertyNodes.GetNodeRegisterIdx( uniqueId ); } return -1; }
		public static List<PropertyNode> PropertyNodesList() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.PropertyNodes.NodesList; } return null; }
		public static int GetPropertyNodeAmount() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.PropertyNodes.NodesList.Count; } return -1; }

		// Function Inputs
		public static void RegisterFunctionInputNode( FunctionInput node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionInputNodes.AddNode( node ); } }
		public static void UnregisterFunctionInputNode( FunctionInput node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionInputNodes.RemoveNode( node ); } }
		public static void UpdateFunctionInputData( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionInputNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static List<FunctionInput> FunctionInputList() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FunctionInputNodes.NodesList; } return null; }

		// Function Nodes
		public static void RegisterFunctionNode( FunctionNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionNodes.AddNode( node ); } }
		public static void UnregisterFunctionNode( FunctionNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionNodes.RemoveNode( node ); } }
		public static void UpdateFunctionData( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static List<FunctionNode> FunctionList() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FunctionNodes.NodesList; } return null; }

		// Function Outputs
		public static void RegisterFunctionOutputNode( FunctionOutput node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionOutputNodes.AddNode( node ); } }
		public static void UnregisterFunctionOutputNode( FunctionOutput node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionOutputNodes.RemoveNode( node ); } }
		public static void UpdateFunctionOutputData( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionOutputNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static List<FunctionOutput> FunctionOutputList() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FunctionOutputNodes.NodesList; } return null; }

		// Function Switches Copy
		public static void RegisterFunctionSwitchCopyNode( FunctionSwitch node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionSwitchCopyNodes.AddNode( node ); } }
		public static void UnregisterFunctionSwitchCopyNode( FunctionSwitch node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionSwitchCopyNodes.RemoveNode( node ); } }
		public static void UpdateFunctionSwitchCopyData( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionSwitchCopyNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static List<FunctionSwitch> FunctionSwitchCopyList() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FunctionSwitchCopyNodes.NodesList; } return null; }

		// Function Switches
		public static void RegisterFunctionSwitchNode( FunctionSwitch node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionSwitchNodes.AddNode( node ); } }
		public static void UnregisterFunctionSwitchNode( FunctionSwitch node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionSwitchNodes.RemoveNode( node ); } }
		public static void UpdateFunctionSwitchData( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionSwitchNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static List<FunctionSwitch> FunctionSwitchList() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FunctionSwitchNodes.NodesList; } return null; }
		public static void UpdateFunctionSwitchArr() { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.FunctionSwitchNodes.UpdateNodeArr(); } }
		public static string[] FunctionSwitchesNodeArr() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FunctionSwitchNodes.NodesArr; } return null; }
		public static FunctionSwitch GetFunctionSwitchNode( int idx ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FunctionSwitchNodes.GetNode( idx ); } return null; }
		public static int GetFunctionSwitchNodeIndex( int uniqueId ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.FunctionSwitchNodes.GetNodeRegisterIdx( uniqueId ); } return -1; }

		// Screen Color Node
		public static void RegisterScreenColorNode( ScreenColorNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.ScreenColorNodes.AddNode( node ); } }
		public static void UnregisterScreenColorNode( ScreenColorNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.ScreenColorNodes.RemoveNode( node ); } }
		public static string[] ScreenColorNodeArr() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.ScreenColorNodes.NodesArr; } return null; }
		public static ScreenColorNode GetScreenColorNode( int idx ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.ScreenColorNodes.GetNode( idx ); } return null; }
		public static int GetScreenColorNodeRegisterId( int uniqueId ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.ScreenColorNodes.GetNodeRegisterIdx( uniqueId ); } return -1; }
		public static void UpdateScreenColorDataNode( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.ScreenColorNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static int GetScreenColorNodeAmount() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.ScreenColorNodes.NodesList.Count; } return -1; }

		// Local Var Node
		public static int RegisterLocalVarNode( RegisterLocalVarNode node ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.LocalVarNodes.AddNode( node ); } return -1; }
		public static void UnregisterLocalVarNode( RegisterLocalVarNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.LocalVarNodes.RemoveNode( node ); } }
		public static string[] LocalVarNodeArr() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.LocalVarNodes.NodesArr; } return null; }
		public static int LocalVarNodeAmount() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.LocalVarNodes.NodesList.Count; } return 0; }
		public static int GetLocalVarNodeRegisterId( int uniqueId ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.LocalVarNodes.GetNodeRegisterIdx( uniqueId ); } return -1; }
		public static RegisterLocalVarNode GetLocalVarNode( int idx ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.LocalVarNodes.GetNode( idx ); } return null; }
		public static void UpdateLocalVarDataNode( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.LocalVarNodes.UpdateDataOnNode( uniqueId, data ); } }

		//Global Array
		public static void RegisterGlobalArrayNode( GlobalArrayNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.GlobalArrayNodes.AddNode( node ); } }
		public static void UnregisterGlobalArrayNode( GlobalArrayNode node ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.GlobalArrayNodes.RemoveNode( node ); } }
		public static string[] GlobalArrayNodeArr() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.GlobalArrayNodes.NodesArr; } return null; }
		public static GlobalArrayNode GetGlobalArrayNode( int idx ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.GlobalArrayNodes.GetNode( idx ); } return null; }
		public static int GetGlobalArrayNodeRegisterId( int uniqueId ) { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.GlobalArrayNodes.GetNodeRegisterIdx( uniqueId ); } return -1; }
		public static void UpdateGlobalArrayDataNode( int uniqueId, string data ) { if( CurrentWindow != null ) { CurrentWindow.CurrentGraph.GlobalArrayNodes.UpdateDataOnNode( uniqueId, data ); } }
		public static int GetGlobalArrayNodeAmount() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.GlobalArrayNodes.NodesList.Count; } return -1; }


		public static void FocusOnNode( ParentNode node, float zoom, bool selectNode ) { if( CurrentWindow != null ) { CurrentWindow.FocusOnNode( node, zoom, selectNode ); } }
		public static PrecisionType CurrentPrecision() { if( CurrentWindow != null ) { return CurrentWindow.CurrentGraph.CurrentPrecision; } return PrecisionType.Float; }
		public static string CurrentPrecisionCg() { if( CurrentWindow != null ) { return m_precisionTypeToCg[ CurrentWindow.CurrentGraph.CurrentPrecision ]; } return m_precisionTypeToCg[ PrecisionType.Float ]; }

		public static PrecisionType GetFinalPrecision( PrecisionType precision )
		{
			if( CurrentWindow != null && CurrentWindow.CurrentGraph != null )
			{
				PrecisionType mainPrecision = CurrentWindow.CurrentGraph.CurrentPrecision;
				if( (int)mainPrecision > (int)precision )
					return mainPrecision;
			}
			return precision;
		}

		public static bool GetNodeAvailabilityInBitArray( int bitArray, NodeAvailability availability ) { return ( bitArray & (int)availability ) != 0; }
		public static bool GetCategoryInBitArray( int bitArray, MasterNodePortCategory category ) { return ( bitArray & (int)category ) != 0; }
		public static void SetCategoryInBitArray( ref int bitArray, MasterNodePortCategory category ) { bitArray = bitArray | (int)category; }

		public static int GetPriority( WirePortDataType type ) { return m_portPriority[ type ]; }

		public static void ShowIncompatiblePortMessage( bool fromInput, ParentNode inNode, WirePort inPort, ParentNode outNode, WirePort outPort )
		{
			string inPortName = inPort.Name.Equals( Constants.EmptyPortValue ) ? inPort.PortId.ToString() : inPort.Name;
			string outPortName = outPort.Name.Equals( Constants.EmptyPortValue ) ? outPort.PortId.ToString() : outPort.Name;
			ShowMessage( string.Format( ( fromInput ? IncorrectInputConnectionErrorMsg : IncorrectOutputConnectionErrorMsg ), inPortName, inNode.Attributes.Name, inPort.DataType, outPort.DataType, outPortName, outNode.Attributes.Name ) );
		}

		public static void ShowNoVertexModeNodeMessage( ParentNode node )
		{
			ShowMessage( string.Format( NoVertexModeNodeWarning, node.Attributes.Name ), MessageSeverity.Warning );
		}

		public static int TotalExampleMaterials { get { return m_exampleMaterialIDs.Count; } }

		public static int ShaderIndentLevel
		{
			get { return m_shaderIndentLevel; }
			set
			{
				m_shaderIndentLevel = Mathf.Max( value, 0 );
				m_shaderIndentTabs = string.Empty;
				for( int i = 0; i < m_shaderIndentLevel; i++ ) { m_shaderIndentTabs += "\t"; }
			}
		}

		public static string ShaderIndentTabs { get { return m_shaderIndentTabs; } }
		public static void AddLineToShaderBody( ref string ShaderBody, string line ) { ShaderBody += m_shaderIndentTabs + line; }
		public static void AddMultiLineToShaderBody( ref string ShaderBody, string[] lines )
		{
			for( int i = 0; i < lines.Length; i++ )
			{
				ShaderBody += m_shaderIndentTabs + lines[ i ];
			}
		}

		public static void ClearUndoHelper()
		{
			m_undoHelper.Clear();
		}

		public static bool CheckUndoNode( ParentNode node )
		{
			if( node == null )
				return false;
			if( m_undoHelper.ContainsKey( node.UniqueId ) )
			{
				return false;
			}

			m_undoHelper.Add( node.UniqueId, node );
			EditorUtility.SetDirty( node );
			return true;
		}

		public static void MarkUndoAction()
		{
			SerializeHelperCounter = 2;
		}

		public static bool SerializeFromUndo()
		{
			if( SerializeHelperCounter > 0 )
			{
				SerializeHelperCounter--;
				return true;
			}
			return false;
		}

		public static int GetKeywordId( string keyword )
		{
			if( AvailableKeywordsDict.Count != AvailableKeywords.Length )
			{
				AvailableKeywordsDict.Clear();
				for( int i = 1; i < AvailableKeywords.Length; i++ )
				{
					AvailableKeywordsDict.Add( AvailableKeywords[ i ], i );
				}
			}

			if( AvailableKeywordsDict.ContainsKey( keyword ) )
			{
				return AvailableKeywordsDict[ keyword ];
			}

			return 0;
		}
	}
}
