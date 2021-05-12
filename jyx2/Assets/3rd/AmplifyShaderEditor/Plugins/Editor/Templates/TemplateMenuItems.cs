// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using UnityEditor;

namespace AmplifyShaderEditor
{
	public class TemplateMenuItems
	{
		[ MenuItem( "Assets/Create/Amplify Shader/Unlit", false, 85 )]
		public static void ApplyTemplate0()
		{
			AmplifyShaderEditorWindow.CreateNewTemplateShader( "0770190933193b94aaa3065e307002fa" );
		}
		[ MenuItem( "Assets/Create/Amplify Shader/Legacy/Post Process", false, 85 )]
		public static void ApplyTemplate1()
		{
			AmplifyShaderEditorWindow.CreateNewTemplateShader( "c71b220b631b6344493ea3cf87110c93" );
		}
		[ MenuItem( "Assets/Create/Amplify Shader/Legacy/Default Unlit", false, 85 )]
		public static void ApplyTemplate2()
		{
			AmplifyShaderEditorWindow.CreateNewTemplateShader( "6e114a916ca3e4b4bb51972669d463bf" );
		}
		[ MenuItem( "Assets/Create/Amplify Shader/Legacy/Default UI", false, 85 )]
		public static void ApplyTemplate3()
		{
			AmplifyShaderEditorWindow.CreateNewTemplateShader( "5056123faa0c79b47ab6ad7e8bf059a4" );
		}
		[ MenuItem( "Assets/Create/Amplify Shader/Legacy/Default Sprites", false, 85 )]
		public static void ApplyTemplate4()
		{
			AmplifyShaderEditorWindow.CreateNewTemplateShader( "0f8ba0101102bb14ebf021ddadce9b49" );
		}
		[ MenuItem( "Assets/Create/Amplify Shader/Legacy/Particles Alpha Blended", false, 85 )]
		public static void ApplyTemplate5()
		{
			AmplifyShaderEditorWindow.CreateNewTemplateShader( "0b6a9f8b4f707c74ca64c0be8e590de0" );
		}
		[ MenuItem( "Assets/Create/Amplify Shader/Legacy/Multi Pass Unlit", false, 85 )]
		public static void ApplyTemplate6()
		{
			AmplifyShaderEditorWindow.CreateNewTemplateShader( "e1de45c0d41f68c41b2cc20c8b9c05ef" );
		}
	}
}
