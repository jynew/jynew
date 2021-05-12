// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{

	public struct Constants
	{
		public readonly static string AppDataFullName = "appdata_full";
		public readonly static string CustomAppDataFullName = "appdata_full_custom";
		public readonly static string CustomAppDataFullBody =
		"\n\t\tstruct appdata_full_custom\n" +
		"\t\t{\n" +
		"\t\t\tfloat4 vertex : POSITION;\n" +
		"\t\t\tfloat4 tangent : TANGENT;\n" +
		"\t\t\tfloat3 normal : NORMAL;\n" +
		"\t\t\tfloat4 texcoord : TEXCOORD0;\n" +
		"\t\t\tfloat4 texcoord1 : TEXCOORD1;\n" +
		"\t\t\tfloat4 texcoord2 : TEXCOORD2;\n" +
		"\t\t\tfloat4 texcoord3 : TEXCOORD3;\n" +
		"\t\t\tfixed4 color : COLOR;\n" +
		"\t\t\tUNITY_VERTEX_INPUT_INSTANCE_ID\n";
		
		public readonly static string IncludeFormat = "#include \"{0}\"";
		public readonly static string PragmaFormat = "#pragma {0}";
		public readonly static string DefineFormat = "#define {0}";

		public readonly static string RenderTypeHelperStr = "RenderType";
		public readonly static string RenderQueueHelperStr = "Queue";

		public readonly static string DefaultShaderName = "New Amplify Shader";

		public readonly static string UndoReplaceMasterNodeId = "Replacing Master Node";
		public readonly static string UnityLightingLib = "Lighting.cginc";
		public readonly static string UnityAutoLightLib = "AutoLight.cginc";
		public readonly static string UnityBRDFLib = "UnityStandardBRDF.cginc";
		public readonly static string LocalValueDecWithoutIdent = "{0} {1} = {2};";
		public readonly static string LocalValueDefWithoutIdent = "{0} {1} {2};";
		public readonly static string TilingOffsetFormat = "{0} * {1} + {2}";
		public static string InvalidPostProcessDatapath = "__DELETED_GUID_Trash";
		//TEMPLATES

		public static float PlusMinusButtonLayoutWidth = 15;

		public static float NodeButtonSizeX = 16;
		public static float NodeButtonSizeY = 16;
		public static float NodeButtonDeltaX = 5;
		public static float NodeButtonDeltaY = 11;

		public readonly static string ReservedPropertyNameStr = "Property name '{0}' is reserved and cannot be used";
		public readonly static string NumericPropertyNameStr = "Property name '{0}' is numeric thus cannot be used";
		public readonly static string DeprecatedMessageStr = "Node '{0}' is deprecated. Use node '{1}' instead.";
		public readonly static string DeprecatedNoAlternativeMessageStr = "Node '{0}' is deprecated and should be removed.";
		public readonly static string UndoChangePropertyTypeNodesId = "Changing Property Types";
		public readonly static string UndoChangeTypeNodesId = "Changing Nodes Types";
		public readonly static string UndoMoveNodesId = "Moving Nodes";
		public readonly static string UndoRegisterFullGrapId = "Register Graph";
		public readonly static string UndoAddNodeToCommentaryId = "Add node to Commentary";
		public readonly static string UndoRemoveNodeFromCommentaryId = "Remove node from Commentary";
		public readonly static string UndoCreateDynamicPortId = "Create Dynamic Port";
		public readonly static string UndoDeleteDynamicPortId = "Destroy Dynamic Port";
		public readonly static string UndoRegisterNodeId = "Register Object";
		public readonly static string UndoUnregisterNodeId = "Unregister Object";
		public readonly static string UndoCreateNodeId = "Create Object";
		public readonly static string UndoPasteNodeId = "Paste Object";
		public readonly static string UndoDeleteNodeId = "Destroy Object";
		public readonly static string UndoDeleteConnectionId = "Destroy Connection";
		public readonly static string UndoCreateConnectionId = "Create Connection";

		public readonly static float MenuDragSpeed = -0.5f;
		public readonly static string DefaultCustomInspector = "ASEMaterialInspector";
		public readonly static string ReferenceTypeStr = "Mode";
		public readonly static string AvailableReferenceStr = "Reference";
		public readonly static string InstancePostfixStr = " (Reference) ";

		public readonly static string ASEMenuName = "Amplify Shader";

		public readonly static string LodCrossFadeOption2017 = "dithercrossfade";

		public readonly static string UnityShaderVariables = "UnityShaderVariables.cginc";
		public readonly static string UnityCgLibFuncs = "UnityCG.cginc";
		public readonly static string UnityStandardUtilsLibFuncs = "UnityStandardUtils.cginc";
		public readonly static string UnityPBSLightingLib = "UnityPBSLighting.cginc";
		public readonly static string UnityDeferredLightLib = "UnityDeferredLibrary.cginc";
		public readonly static string ATSharedLibGUID = "ba242738c4be3324aa88d126f7cc19f9";

		public readonly static string HelpURL = "http://wiki.amplify.pt/index.php?title=Unity_Products:Amplify_Shader_Editor";
		//public readonly static string NodeCommonUrl = "http://wiki.amplify.pt/index.php?title=Unity_Products:Amplify_Shader_Editor/Nodes#";
		//public readonly static string CommunityNodeCommonUrl = "http://wiki.amplify.pt/index.php?title=Unity_Products:Amplify_Shader_Editor/Community_Nodes#";
		public readonly static string NodeCommonUrl = "http://wiki.amplify.pt/index.php?title=Unity_Products:Amplify_Shader_Editor/";
		public readonly static string CommunityNodeCommonUrl = "http://wiki.amplify.pt/index.php?title=Unity_Products:Amplify_Shader_Editor/";
		public readonly static Color InfiniteLoopColor = Color.red;

		public readonly static Color DefaultCategoryColor = new Color( 0.26f, 0.35f, 0.44f, 1.0f );
		public readonly static Color NodeBodyColor = new Color( 1f, 1f, 1f, 1.0f );

		public readonly static Color ModeTextColor = new Color( 1f, 1f, 1f, 0.25f );
		public readonly static Color ModeIconColor = new Color( 1f, 1f, 1f, 0.75f );

		public readonly static Color PortTextColor = new Color( 1f, 1f, 1f, 0.5f );
		public readonly static Color PortLockedTextColor = new Color( 1f, 1f, 1f, 0.35f );
		public readonly static Color BoxSelectionColor = new Color( 1f, 1f, 1f, 0.5f );
		public readonly static Color SpecialRegisterLocalVarSelectionColor = new Color( 0.27f, 0.52f, 1.0f, 1f );
		public readonly static Color SpecialGetLocalVarSelectionColor = new Color( 0.2f, 0.8f, 0.4f, 1f );
		public readonly static Color NodeSelectedColor = new Color( 0.85f, 0.56f, 0f, 1f );
		public readonly static Color NodeDefaultColor = new Color( 1f, 1f, 1f, 1f );
		public readonly static Color NodeConnectedColor = new Color( 1.0f, 1f, 0.0f, 1f );
		public readonly static Color NodeErrorColor = new Color( 1f, 0.5f, 0.5f, 1f );
		public readonly static string NoSpecifiedCategoryStr = "<None>";

		public readonly static int MINIMIZE_WINDOW_LOCK_SIZE = 630;

		public readonly static int FoldoutMouseId = 0; // Left Mouse Button

		public readonly static float SNAP_SQR_DIST = 200f;
		public readonly static int INVALID_NODE_ID = -1;
		public readonly static float WIRE_WIDTH = 7f;
		public readonly static float WIRE_CONTROL_POINT_DIST = 0.7f;
		public readonly static float WIRE_CONTROL_POINT_DIST_INV = 1.7f;

		public readonly static float IconsLeftRightMargin = 5f;
		public readonly static float PropertyPickerWidth = 16f;
		public readonly static float PropertyPickerHeight = 16f;
		public readonly static float PreviewExpanderWidth = 16f;
		public readonly static float PreviewExpanderHeight = 16f;
		public readonly static float TextFieldFontSize = 11f;
		public readonly static float DefaultFontSize = 15f;
		public readonly static float DefaultTitleFontSize = 13f;
		public readonly static float PropertiesTitleFontSize = 11f;
		public readonly static float MessageFontSize = 40f;
		public readonly static float SelectedObjectFontSize = 30f;

		public readonly static float PORT_X_ADJUST = 10;
		public readonly static float PORT_INITIAL_X = 10;

		public readonly static float PORT_INITIAL_Y = 40;
		public readonly static float INPUT_PORT_DELTA_Y = 5;
		public readonly static float PORT_TO_LABEL_SPACE_X = 5;

		public readonly static float NODE_HEADER_HEIGHT = 32;
		public readonly static float NODE_HEADER_EXTRA_HEIGHT = 5;
		public readonly static float NODE_HEADER_LEFTRIGHT_MARGIN = 10;

		public readonly static float MULTIPLE_SELECION_BOX_ALPHA = 0.5f;
		public readonly static float RMB_CLICK_DELTA_TIME = 0.1f;
		public readonly static float RMB_SCREEN_DIST = 10f;

		public readonly static float CAMERA_MAX_ZOOM = 2f;
		public readonly static float CAMERA_MIN_ZOOM = 1f;
		public readonly static float CAMERA_ZOOM_SPEED = 0.1f;
		public readonly static float ALT_CAMERA_ZOOM_SPEED = -0.05f;

		public readonly static object INVALID_VALUE = null;

		public readonly static float HORIZONTAL_TANGENT_SIZE = 100f;
		public readonly static float OUTSIDE_WIRE_MARGIN = 5f;

		public readonly static string SubTitleNameFormatStr = "Name( {0} )";
		public readonly static string SubTitleSpaceFormatStr = "Space( {0} )";
		public readonly static string SubTitleTypeFormatStr = "Type( {0} )";
		public readonly static string SubTitleValueFormatStr = "Value( {0} )";
		public readonly static string SubTitleConstFormatStr = "Const( {0} )";
		public readonly static string SubTitleVarNameFormatStr = "Var( {0} )";

		public readonly static string CodeWrapper = "( {0} )";

		public readonly static string NodesDumpFormat = "{0}:,{1},{2}\n";
		public readonly static string TagFormat = " \"{0}\" = \"{1}\"";

		public readonly static string LocalVarIdentation = "\t\t\t";
		public readonly static string SimpleLocalValueDec = LocalVarIdentation + "{0} {1};\n";

		public readonly static string LocalValueDec = LocalVarIdentation + LocalValueDecWithoutIdent + '\n';
		public readonly static string LocalValueDef = LocalVarIdentation + "{0} = {1};\n";
		public readonly static string CastHelper = "({0}).{1}";
		public readonly static string PropertyLocalVarDec = "{0} {1} = {0}({2});";
		public readonly static string UniformDec = "uniform {0} {1};";

		public readonly static string PropertyValueLabel = "Value( {0} )";
		public readonly static string ConstantsValueLabel = "Const( {0} )";

		public readonly static string PropertyFloatFormatLabel = "0.###";
		public readonly static string PropertyBigFloatFormatLabel = "0.###e+0";

		public readonly static string PropertyIntFormatLabel = "0";
		public readonly static string PropertyBigIntFormatLabel = "0e+0";


		public readonly static string PropertyVectorFormatLabel = "0.##";
		public readonly static string PropertyBigVectorFormatLabel = "0.##e+0";


		public readonly static string PropertyMatrixFormatLabel = "0.#";
		public readonly static string PropertyBigMatrixFormatLabel = "0.#e+0";

		public readonly static string NoPropertiesLabel = "No assigned properties";

		public readonly static string ValueLabel = "Value";
		public readonly static string DefaultValueLabel = "Default Value";
		public readonly static string MaterialValueLabel = "Material Value";
		public readonly static GUIContent DefaultValueLabelContent = new GUIContent( "Default Value" );
		public readonly static GUIContent MaterialValueLabelContent = new GUIContent( "Material Value" );

		public readonly static string InputVarStr = "i";//"input";
		public readonly static string OutputVarStr = "o";//"output";

		public readonly static string CustomLightOutputVarStr = "s";
		public readonly static string CustomLightStructStr = "Custom";

		public readonly static string VertexShaderOutputStr = "o";
		public readonly static string VertexShaderInputStr = "v";//"vertexData";
		public readonly static string VertexDataFunc = "vertexDataFunc";

		public readonly static string VirtualCoordNameStr = "vcoord";

		public readonly static string VertexVecNameStr = "vertexVec";
		public readonly static string VertexVecDecStr = "float3 " + VertexVecNameStr;
		public readonly static string VertexVecVertStr = VertexShaderOutputStr + "." + VertexVecNameStr;

		public readonly static string NormalVecNameStr = "normalVec";
		public readonly static string NormalVecDecStr = "float3 " + NormalVecNameStr;
		public readonly static string NormalVecFragStr = InputVarStr + "." + NormalVecNameStr;
		public readonly static string NormalVecVertStr = VertexShaderOutputStr + "." + NormalVecNameStr;


		public readonly static string IncidentVecNameStr = "incidentVec";
		public readonly static string IncidentVecDecStr = "float3 " + IncidentVecNameStr;
		public readonly static string IncidentVecDefStr = VertexShaderOutputStr + "." + IncidentVecNameStr + " = normalize( " + VertexVecNameStr + " - _WorldSpaceCameraPos.xyz)";
		public readonly static string IncidentVecFragStr = InputVarStr + "." + IncidentVecNameStr;
		public readonly static string IncidentVecVertStr = VertexShaderOutputStr + "." + IncidentVecNameStr;
		public readonly static string WorldNormalLocalDecStr = "WorldNormalVector( " + Constants.InputVarStr + " , {0}( 0,0,1 ))";


		public readonly static string VFaceVariable = "ASEVFace";
		public readonly static string VFaceInput = "half ASEVFace : VFACE";

		public readonly static string ColorVariable = "vertexColor";
		public readonly static string ColorInput = "float4 vertexColor : COLOR";

		public readonly static string NoStringValue = "None";
		public readonly static string EmptyPortValue = "  ";

		public readonly static string[] OverallInvalidChars = { "\r", "\n", "\\", " ", ".", ">", ",", "<", "\'", "\"", ";", ":", "[", "{", "]", "}", "=", "+", "`", "~", "/", "?", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-" };
		public readonly static string[] ShaderInvalidChars = { "\r", "\n", "\\", "\'", "\"", };
		public readonly static string[] EnumInvalidChars = { "\r", "\n", "\\", ".", ">", ",", "<", "\'", "\"", ";", ":", "[", "{", "]", "}", "=", "+", "`", "~", "/", "?", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-" };
		public readonly static string[] AttrInvalidChars = { "\r", "\n", "\\", ".", ">", "<", "\'", "\"", ";", ":", "[", "{", "]", "}", "=", "+", "`", "~", "/", "?", "!", "@", "#", "$", "%", "^", "&", "*" };

		public readonly static string[] WikiInvalidChars = { "#", "<", ">", "[", "]", "|", "{", "}", "%", "+", "?", "\\", "/", ",", ";", "." };

		public readonly static Dictionary<string, string> UrlReplacementStringValues = new Dictionary<string, string>()
		{
			{ " = ", "Equals" },
			{ " == ", "Equals" },
			{ " != ", "NotEqual" },
			{ " \u2260 ", "NotEqual" },
			{ " > ", "Greater" },
			{ " \u2265 " , "GreaterOrEqual" },
			{ " >= ", "GreaterOrEqual" },
			{ " < ", "Less" },
			{ " \u2264 ", "LessOrEqual" },
			{ " <= ", "LessOrEqual" },
			{ " ", "_" },
			{ "[", "" },
			{ "]", "" }
		};

		public readonly static Dictionary<string, string> ReplacementStringValues = new Dictionary<string, string>()
		{
			{ " = ", "Equals" },
			{ " == ", "Equals" },
			{ " != ", "NotEqual" },
			{ " \u2260 ", "NotEqual" },
			{ " > ", "Greater" },
			{ " \u2265 ", "GreaterOrEqual" },
			{ " >= ", "GreaterOrEqual" },
			{ " < ", "Less" },
			{ " \u2264 ", "LessOrEqual" },
			{ " <= ", "LessOrEqual" }
		};

		public readonly static string InternalData = "INTERNAL_DATA";



		public readonly static string NoMaterialStr = "None";

		public readonly static string OptionalParametersSep = " ";

		public readonly static string NodeUndoId = "NODE_UNDO_ID";
		public readonly static string NodeCreateUndoId = "NODE_CREATE_UNDO_ID";
		public readonly static string NodeDestroyUndoId = "NODE_DESTROY_UNDO_ID";

		// Custom node tags
		//[InPortBegin:Id:Type:Name:InPortEnd]
		public readonly static string CNIP = "#IP";

		public readonly static float FLOAT_DRAW_HEIGHT_FIELD_SIZE = 16f;
		public readonly static float FLOAT_DRAW_WIDTH_FIELD_SIZE = 45f;
		public readonly static float FLOAT_WIDTH_SPACING = 3f;

		public readonly static Color LockedPortColor = new Color( 0.3f, 0.3f, 0.3f, 0.5f );

#if UNITY_2018_2_OR_NEWER
		public readonly static int[] AvailableUVChannels = { 0, 1, 2, 3, 4, 5, 6, 7 };
		public readonly static string[] AvailableUVChannelsStr = { "0", "1", "2", "3", "4", "5", "6", "7"};
		public readonly static string AvailableUVChannelLabel = "UV Channel";

		public readonly static int[] AvailableUVSets = { 0, 1, 2, 3, 4, 5, 6, 7 };
		public readonly static string[] AvailableUVSetsStr = { "1", "2", "3", "4","5", "6", "7", "8" };
		public readonly static string AvailableUVSetsLabel = "UV Set";
#else
		public readonly static int[] AvailableUVChannels = { 0, 1, 2, 3 };
		public readonly static string[] AvailableUVChannelsStr = { "0", "1", "2", "3" };
		public readonly static string AvailableUVChannelLabel = "UV Channel";

		public readonly static int[] AvailableUVSets = { 0, 1, 2, 3 };
		public readonly static string[] AvailableUVSetsStr = { "1", "2", "3", "4" };
		public readonly static string AvailableUVSetsLabel = "UV Set";
#endif

		public readonly static int[] AvailableUVSizes = { 2, 3, 4 };
		public readonly static string[] AvailableUVSizesStr = { "Float 2", "Float 3", "Float 4" };
		public readonly static string AvailableUVSizesLabel = "Coord Size";


		public readonly static string LineSeparator = "________________________________";

		public readonly static Vector2 CopyPasteDeltaPos = new Vector2( 40, 40 );

		public readonly static string[] VectorSuffixes = { ".x", ".y", ".z", ".w" };
		public readonly static string[] ColorSuffixes = { ".r", ".g", ".b", ".a" };


		public const string InternalDataLabelStr = "Internal Data";
		public const string AttributesLaberStr = "Attributes";
		public const string ParameterLabelStr = "Parameters";

		public static readonly string[] ReferenceArrayLabels = { "Object", "Reference" };

		public static readonly string[] ChannelNamesVector = { "X", "Y", "Z", "W" };
		public static readonly string[] ChannelNamesColor = { "R", "G", "B", "A" };
	}
}
