// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;
using UnityEditor.VersionControl;

namespace AmplifyShaderEditor
{
	public enum ShaderLoadResult
	{
		LOADED,
		TEMPLATE_LOADED,
		FILE_NOT_FOUND,
		ASE_INFO_NOT_FOUND,
		UNITY_NATIVE_PATHS
	}

	public class Worker
	{
		public static readonly object locker = new object();
		public void DoWork()
		{
			while ( IOUtils.ActiveThread )
			{
				if ( IOUtils.SaveInThreadFlag )
				{
					IOUtils.SaveInThreadFlag = false;
					lock ( locker )
					{
						IOUtils.SaveInThreadShaderBody = IOUtils.ShaderCopywriteMessage + IOUtils.SaveInThreadShaderBody;
						// Add checksum 
						string checksum = IOUtils.CreateChecksum( IOUtils.SaveInThreadShaderBody );
						IOUtils.SaveInThreadShaderBody += IOUtils.CHECKSUM + IOUtils.VALUE_SEPARATOR + checksum;

						// Write to disk
						StreamWriter fileWriter = new StreamWriter( IOUtils.SaveInThreadPathName );
						try
						{
							fileWriter.Write( IOUtils.SaveInThreadShaderBody );
							Debug.Log( "Saving complete" );
						}
						catch ( Exception e )
						{
							Debug.LogException( e );
						}
						finally
						{
							fileWriter.Close();
						}
					}
				}
			}
			Debug.Log( "Thread closed" );
		}
	}

	public static class IOUtils
	{

		public static readonly string ShaderCopywriteMessage = "// Made with Amplify Shader Editor\n// Available at the Unity Asset Store - http://u3d.as/y3X \n";
		public static readonly string GrabPassEmpty = "\t\tGrabPass{ }\n";
		public static readonly string GrabPassBegin = "\t\tGrabPass{ \"";
		public static readonly string GrabPassEnd = "\" }\n";
		public static readonly string PropertiesBegin = "\tProperties\n\t{\n";
		public static readonly string PropertiesEnd = "\t}\n";
		public static readonly string PropertiesElement = "\t\t{0}\n";
		public static readonly string PropertiesElementsRaw = "{0}\n";

		public static readonly string PragmaTargetHeader = "\t\t#pragma target {0}\n";
		public static readonly string InstancedPropertiesHeader = "multi_compile_instancing";
		public static readonly string VirtualTexturePragmaHeader = "multi_compile _ _VT_SINGLE_MODE";

		public static readonly string InstancedPropertiesBegin = "UNITY_INSTANCING_CBUFFER_START({0})";
		public static readonly string InstancedPropertiesEnd = "UNITY_INSTANCING_CBUFFER_END";
		public static readonly string InstancedPropertiesElement = "UNITY_DEFINE_INSTANCED_PROP({0}, {1})";
		public static readonly string InstancedPropertiesData = "UNITY_ACCESS_INSTANCED_PROP({0})";


		public static readonly string LWSRPInstancedPropertiesBegin = "UNITY_INSTANCING_BUFFER_START({0})";
		public static readonly string LWSRPInstancedPropertiesEnd = "UNITY_INSTANCING_BUFFER_END({0})";
		public static readonly string LWSRPInstancedPropertiesElement = "UNITY_DEFINE_INSTANCED_PROP({0}, {1})";
		public static readonly string LWSRPInstancedPropertiesData = "UNITY_ACCESS_INSTANCED_PROP({0},{1})";


		public static readonly string InstancedPropertiesBeginTabs		= "\t\t"+ InstancedPropertiesBegin + "\n";
		public static readonly string InstancedPropertiesEndTabs		= "\t\t"+ InstancedPropertiesEnd + "\n";
		public static readonly string InstancedPropertiesElementTabs	= "\t\t\t"+ InstancedPropertiesElement + "\n";
		
		public static readonly string MetaBegin = "defaultTextures:";
		public static readonly string MetaEnd = "userData:";
		public static readonly string ShaderBodyBegin = "/*ASEBEGIN";
		public static readonly string ShaderBodyEnd = "ASEEND*/";
		//public static readonly float CurrentVersionFlt = 0.4f;
		//public static readonly string CurrentVersionStr = "Version=" + CurrentVersionFlt;

		public static readonly string CHECKSUM = "//CHKSM";
		public static readonly string LAST_OPENED_OBJ_ID = "ASELASTOPENOBJID";

		public static readonly string MAT_CLIPBOARD_ID = "ASEMATCLIPBRDID";
		public static readonly char FIELD_SEPARATOR = ';';
		public static readonly char VALUE_SEPARATOR = '=';
		public static readonly char LINE_TERMINATOR = '\n';
		public static readonly char VECTOR_SEPARATOR = ',';
		public static readonly char FLOAT_SEPARATOR = '.';
		public static readonly char CLIPBOARD_DATA_SEPARATOR = '|';
		public static readonly char MATRIX_DATA_SEPARATOR = '|';
		public readonly static string NO_TEXTURES = "<None>";
		public static readonly string SaveShaderStr = "Please enter shader name to save";
		public static readonly string FloatifyStr = ".0";

		// Node parameter names
		public const string NodeParam = "Node";
		public const string NodePosition = "Position";
		public const string NodeId = "Id";
		public const string NodeType = "Type";
		public const string WireConnectionParam = "WireConnection";

		public static readonly uint NodeTypeId = 1;

		public static readonly int InNodeId = 1;
		public static readonly int InPortId = 2;
		public static readonly int OutNodeId = 3;
		public static readonly int OutPortId = 4;

		public readonly static string DefaultASEDirtyCheckName = "__dirty";
		public readonly static string DefaultASEDirtyCheckProperty = "[HideInInspector] " + DefaultASEDirtyCheckName + "( \"\", Int ) = 1";
		public readonly static string DefaultASEDirtyCheckUniform = "uniform int " + DefaultASEDirtyCheckName + " = 1;";

		public readonly static string MaskClipValueName = "_Cutoff";
		public readonly static string MaskClipValueProperty = MaskClipValueName + "( \"{0}\", Float ) = {1}";
		public readonly static string MaskClipValueUniform = "uniform float " + MaskClipValueName + " = {0};";

		//public static readonly string ASEFolderGUID = "daca988099666ec40aaa2cde22bb4935";
		//public static string ASEResourcesPath = "/Plugins/EditorResources/";
		//public static string ASEFolderPath;

		//public static bool IsShaderFunctionWindow = false;
		

		public static int DefaultASEDirtyCheckId;

		// this is to be used in combination with AssetDatabase.GetAssetPath, both of these include the Assets/ path so we need to remove from one of them 
		public static string dataPath;


		public static string EditorResourcesGUID = "0932db7ec1402c2489679c4b72eab5eb";
		public static string GraphBgTextureGUID = "881c304491028ea48b5027ac6c62cf73";
		public static string GraphFgTextureGUID = "8c4a7fca2884fab419769ccc0355c0c1";
		public static string WireTextureGUID = "06e687f68dd96f0448c6d8217bbcf608";
		public static string MasterNodeOnTextureGUID = "26c64fcee91024a49980ea2ee9d1a2fb";
		public static string MasterNodeOffTextureGUID = "712aee08d999c16438e2d694f42428e8";
		public static string GPUInstancedOnTextureGUID = "4b0c2926cc71c5846ae2a29652d54fb6";
		public static string GPUInstancedOffTextureGUID = "486c7766baaf21b46afb63c1121ef03e";
		public static string MainSkinGUID = "57482289c346f104a8162a3a79aaff9d";

		public static string UpdateOutdatedGUID = "cce638be049286c41bcbd0a26c356b18";
		public static string UpdateOFFGUID = "99d70ac09b4db9742b404c3f92d8564b";
		public static string UpdateUpToDatedGUID = "ce30b12fbb3223746bcfef9ea82effe3";
		public static string LiveOffGUID = "bb16faf366bcc6c4fbf0d7666b105354";
		public static string LiveOnGUID = "6a0ae1d7892333142aeb09585572202c";
		public static string LivePendingGUID = "e3182200efb67114eb5050f8955e1746";
		public static string CleanupOFFGUID = "f62c0c3a5ddcd844e905fb2632fdcb15";
		public static string CleanUpOnGUID = "615d853995cf2344d8641fd19cb09b5d";
		public static string OpenSourceCodeOFFGUID = "f7e8834b42791124095a8b7f2d4daac2";
		public static string OpenSourceCodeONGUID = "8b114792ff84f6546880c031eda42bc0";
		public static string FocusNodeGUID = "da673e6179c67d346abb220a6935e359";
		public static string FitViewGUID = "1def740f2314c6b4691529cadeee2e9c";
		public static string ShowInfoWindowGUID = "77af20044e9766840a6be568806dc22e";
		public static string ShowTipsWindowGUID = "066674048bbb1e64e8cdcc6c3b4abbeb";
		public static string ShowConsoleWindowGUID = "9a81d7df8e62c044a9d1cada0c8a2131";


		public static Dictionary<string, string> NodeTypeReplacer = new Dictionary<string, string>()
		{
			{"AmplifyShaderEditor.RotateAboutAxis", "AmplifyShaderEditor.RotateAboutAxisNode"},
			{"GlobalArrayNode", "AmplifyShaderEditor.GlobalArrayNode"},
			{"AmplifyShaderEditor.SimpleMaxOp", "AmplifyShaderEditor.SimpleMaxOpNode"},
			{"AmplifyShaderEditor.SimpleMinNode", "AmplifyShaderEditor.SimpleMinOpNode"},
			{"AmplifyShaderEditor.TFHCRemap", "AmplifyShaderEditor.TFHCRemapNode"},
			{"AmplifyShaderEditor.TFHCPixelateUV", "AmplifyShaderEditor.TFHCPixelate"},
			{"AmplifyShaderEditor.VirtualTexturePropertyNode", "AmplifyShaderEditor.VirtualTextureObject"}
		};

		private static readonly string AmplifyShaderEditorDefineSymbol = "AMPLIFY_SHADER_EDITOR";

		/////////////////////////////////////////////////////////////////////////////
		// THREAD IO UTILS
		public static bool SaveInThreadFlag = false;
		public static string SaveInThreadShaderBody;
		public static string SaveInThreadPathName;
		public static Thread SaveInThreadMainThread;
		public static bool ActiveThread = true;
		private static bool UseSaveThread = false;

		private static bool Initialized = false;

		public static bool FunctionNodeChanged = false;

		public static List<AmplifyShaderEditorWindow> AllOpenedWindows = new List<AmplifyShaderEditorWindow>();

		public static void StartSaveThread( string shaderBody, string pathName )
		{
			if( Provider.enabled && Provider.isActive )
			{
				Asset loadedAsset = Provider.GetAssetByPath( FileUtil.GetProjectRelativePath( pathName ) );
				if( loadedAsset != null )
				{
					//Task statusTask = Provider.Status( loadedAsset );
					//statusTask.Wait();
					//if( Provider.CheckoutIsValid( statusTask.assetList[ 0 ] ) )
					{
						Task checkoutTask = Provider.Checkout( loadedAsset, CheckoutMode.Both );
						checkoutTask.Wait();
					}
				}
			}

			if( UseSaveThread )
			{
				if ( !SaveInThreadFlag )
				{
					if ( SaveInThreadMainThread == null )
					{
						Worker worker = new Worker();
						SaveInThreadMainThread = new Thread( worker.DoWork );
						SaveInThreadMainThread.Start();
						Debug.Log( "Thread created" );
					}

					SaveInThreadShaderBody = shaderBody;
					SaveInThreadPathName = pathName;
					SaveInThreadFlag = true;
				}
			}
			else
			{
				SaveTextfileToDisk( shaderBody, pathName );
			}
		}

		////////////////////////////////////////////////////////////////////////////
		private static void SetAmplifyDefineSymbolOnBuildTargetGroup( BuildTargetGroup targetGroup )
		{
			string currData = PlayerSettings.GetScriptingDefineSymbolsForGroup( targetGroup );
			if ( !currData.Contains( AmplifyShaderEditorDefineSymbol ) )
			{
				if ( string.IsNullOrEmpty( currData ) )
				{
					PlayerSettings.SetScriptingDefineSymbolsForGroup( targetGroup, AmplifyShaderEditorDefineSymbol );
				}
				else
				{
					if ( !currData[ currData.Length - 1 ].Equals( ';' ) )
					{
						currData += ';';
					}
					currData += AmplifyShaderEditorDefineSymbol;
					PlayerSettings.SetScriptingDefineSymbolsForGroup( targetGroup, currData );
				}
			}
		}

		public static void Init()
		{
			if ( !Initialized )
			{
				Initialized = true;
				SetAmplifyDefineSymbolOnBuildTargetGroup( EditorUserBuildSettings.selectedBuildTargetGroup );
				//Array BuildTargetGroupValues = Enum.GetValues( typeof(  BuildTargetGroup ));
				//for ( int i = 0; i < BuildTargetGroupValues.Length; i++ )
				//{
				//	if( i != 0 && i != 15 && i != 16 )
				//		SetAmplifyDefineSymbolOnBuildTargetGroup( ( BuildTargetGroup ) BuildTargetGroupValues.GetValue( i ) );
				//}

				DefaultASEDirtyCheckId = Shader.PropertyToID( DefaultASEDirtyCheckName );
				dataPath = Application.dataPath.Remove( Application.dataPath.Length - 6 );


				//ASEFolderPath = AssetDatabase.GUIDToAssetPath( ASEFolderGUID );
				//ASEResourcesPath = ASEFolderPath + ASEResourcesPath;
			}
		}


		public static void DumpTemplateManagers()
		{
			for( int i = 0; i < AllOpenedWindows.Count; i++ )
			{
				if( AllOpenedWindows[ i ].TemplatesManagerInstance != null )
				{
					Debug.Log( AllOpenedWindows[ i ].titleContent.text + ": " + AllOpenedWindows[ i ].TemplatesManagerInstance.GetInstanceID() );
				}
			}
		}

		public static TemplatesManager FirstValidTemplatesManager
		{
			get
			{
				for( int i = 0; i < AllOpenedWindows.Count; i++ )
				{
					if( AllOpenedWindows[ i ].TemplatesManagerInstance != null )
					{
						return AllOpenedWindows[ i ].TemplatesManagerInstance;
					}
				}
				return null;
			}
		}

		public static void UpdateSFandRefreshWindows( AmplifyShaderFunction function )
		{
			for( int i = 0; i < AllOpenedWindows.Count; i++ )
			{
				AllOpenedWindows[ i ].LateRefreshAvailableNodes();
				if( AllOpenedWindows[ i ].IsShaderFunctionWindow )
				{
					if( AllOpenedWindows[ i ].OpenedShaderFunction == function )
					{
						AllOpenedWindows[ i ].UpdateTabTitle();
					}
				}
			}
		}

		public static void UpdateIO()
		{
			int windowCount = AllOpenedWindows.Count;
			if ( windowCount == 0 )
			{
				EditorApplication.update -= IOUtils.UpdateIO;
				return;
			}

			for ( int i = 0; i < AllOpenedWindows.Count; i++ )
			{
				if ( AllOpenedWindows[i] == EditorWindow.focusedWindow )
				{
					UIUtils.CurrentWindow = AllOpenedWindows[ i ];
				}

				if( FunctionNodeChanged )
					AllOpenedWindows[ i ].CheckFunctions = true;

				if ( AllOpenedWindows[ i ] == null )
				{
					AllOpenedWindows.RemoveAt( i );
					i--;
				}
			}

			if ( FunctionNodeChanged )
				FunctionNodeChanged = false;
		}

		public static void Destroy()
		{
			ActiveThread = false;
			if ( SaveInThreadMainThread != null )
			{
				SaveInThreadMainThread.Abort();
				SaveInThreadMainThread = null;
			}
		}

		public static void GetShaderName( out string shaderName, out string fullPathname, string defaultName, string customDatapath )
		{
			string currDatapath = String.IsNullOrEmpty( customDatapath ) ? Application.dataPath : customDatapath;
			fullPathname = EditorUtility.SaveFilePanelInProject( "Select Shader to save", defaultName, "shader", SaveShaderStr, currDatapath );
			if ( !String.IsNullOrEmpty( fullPathname ) )
			{
				shaderName = fullPathname.Remove( fullPathname.Length - 7 ); // -7 remove .shader extension
				string[] subStr = shaderName.Split( '/' );
				if ( subStr.Length > 0 )
				{
					shaderName = subStr[ subStr.Length - 1 ]; // Remove pathname 
				}
			}
			else
			{
				shaderName = string.Empty;
			}
		}

		public static void AddTypeToString( ref string myString, string typeName )
		{
			myString += typeName;
		}

		public static void AddFieldToString( ref string myString, string fieldName, object fieldValue )
		{
			myString += FIELD_SEPARATOR + fieldName + VALUE_SEPARATOR + fieldValue;
		}

		public static void AddFieldValueToString( ref string myString, object fieldValue )
		{
			myString += FIELD_SEPARATOR + fieldValue.ToString();
		}

		public static void AddLineTerminator( ref string myString )
		{
			myString += LINE_TERMINATOR;
		}

		public static string CreateChecksum( string buffer )
		{
			SHA1 sha1 = SHA1.Create();
			byte[] buf = System.Text.Encoding.UTF8.GetBytes( buffer );
			byte[] hash = sha1.ComputeHash( buf, 0, buf.Length );
			string hashstr = BitConverter.ToString( hash ).Replace( "-", "" );
			return hashstr;
		}

		public static void SaveTextfileToDisk( string shaderBody, string pathName, bool addAdditionalInfo = true )
		{

			if ( addAdditionalInfo )
			{
				shaderBody = ShaderCopywriteMessage + shaderBody;
				// Add checksum 
				string checksum = CreateChecksum( shaderBody );
				shaderBody += CHECKSUM + VALUE_SEPARATOR + checksum;
			}

			// Write to disk
			StreamWriter fileWriter = new StreamWriter( pathName );
			try
			{
				fileWriter.Write( shaderBody );
			}
			catch ( Exception e )
			{
				Debug.LogException( e );
			}
			finally
			{
				fileWriter.Close();
			}
		}

		public static string AddAdditionalInfo( string shaderBody )
		{
			shaderBody = ShaderCopywriteMessage + shaderBody;
			string checksum = CreateChecksum( shaderBody );
			shaderBody += CHECKSUM + VALUE_SEPARATOR + checksum;
			return shaderBody;
		}

		public static string LoadTextFileFromDisk( string pathName )
		{
			string result = string.Empty;
            if ( !string.IsNullOrEmpty( pathName ) && File.Exists( pathName ) )
            {

                StreamReader fileReader = null;
                try
                {
                    fileReader = new StreamReader( pathName );
                    result = fileReader.ReadToEnd();
                }
                catch ( Exception e )
                {
                    Debug.LogException( e );
                }
                finally
                {
                    if( fileReader != null)
                        fileReader.Close();
                }
            }
			return result;
		}

		public static bool IsASEShader( Shader shader )
		{
			string datapath = AssetDatabase.GetAssetPath( shader );
			if ( UIUtils.IsUnityNativeShader( datapath ) )
			{
				return false;
			}

			string buffer = LoadTextFileFromDisk( datapath );
			if ( String.IsNullOrEmpty( buffer ) || !IOUtils.HasValidShaderBody( ref buffer ) )
			{
				return false;
			}
			return true;
		}

		public static bool IsShaderFunction( string functionInfo )
		{
			string buffer = functionInfo;
			if ( String.IsNullOrEmpty( buffer ) || !IOUtils.HasValidShaderBody( ref buffer ) )
			{
				return false;
			}
			return true;
		}

		public static bool HasValidShaderBody( ref string shaderBody )
		{
			int shaderBodyBeginId = shaderBody.IndexOf( ShaderBodyBegin );
			if ( shaderBodyBeginId > -1 )
			{
				int shaderBodyEndId = shaderBody.IndexOf( ShaderBodyEnd );
				return ( shaderBodyEndId > -1 && shaderBodyEndId > shaderBodyBeginId );
			}
			return false;
		}

		public static int[] AllIndexesOf( this string str, string substr, bool ignoreCase = false )
		{
			if ( string.IsNullOrEmpty( str ) || string.IsNullOrEmpty( substr ) )
			{
				throw new ArgumentException( "String or substring is not specified." );
			}

			List<int> indexes = new List<int>();
			int index = 0;

			while ( ( index = str.IndexOf( substr, index, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal ) ) != -1 )
			{
				indexes.Add( index++ );
			}

			return indexes.ToArray();
		}

		public static void AddFunctionHeader( ref string function, string header )
		{
			function += "\t\t" + header + "\n\t\t{\n";
		}

		public static void AddSingleLineFunction( ref string function, string header )
		{
			function += "\t\t" + header;
		}

		public static void AddFunctionLine( ref string function, string line )
		{
			function += "\t\t\t" + line + "\n";
		}

		public static void CloseFunctionBody( ref string function )
		{
			function += "\t\t}\n";
		}

		public static string CreateFullFunction( string header, params string[] functionLines )
		{
			string result = string.Empty;
			AddFunctionHeader( ref result, header );
			for ( int i = 0; i > functionLines.Length; i++ )
			{
				AddFunctionLine( ref result, functionLines[ i ] );
			}
			CloseFunctionBody( ref result );
			return result;
		}

		public static string CreateCodeComments( bool forceForwardSlash, params string[] comments )
		{
			string finalComment = string.Empty;
			if ( comments.Length == 1 )
			{
				finalComment = "//" + comments[ 0 ];
			}
			else
			{
				if ( forceForwardSlash )
				{
					for ( int i = 0; i < comments.Length; i++ )
					{
						finalComment += "//" + comments[ i ];
						if ( i < comments.Length - 1 )
						{
							finalComment += "\n\t\t\t";
						}
					}
				}
				else
				{
					finalComment = "/*";
					for ( int i = 0; i < comments.Length; i++ )
					{
						if ( i != 0 )
							finalComment += "\t\t\t";
						finalComment += comments[ i ];
						if ( i < comments.Length - 1 )
							finalComment += "\n";
					}
					finalComment += "*/";
				}
			}
			return finalComment;
		}

		public static string GetUVChannelDeclaration( string uvName, int channelId, int set )
		{
			string uvSetStr = ( set == 0 ) ? "uv" : "uv" + Constants.AvailableUVSetsStr[ set ];
			return "float2 " + uvSetStr + uvName /*+ " : TEXCOORD" + channelId*/;
		}

		public static string GetUVChannelName( string uvName, int set )
		{
			string uvSetStr = ( set == 0 ) ? "uv" : "uv" + Constants.AvailableUVSetsStr[ set ];
			return uvSetStr + uvName;
		}

		public static string GetVertexUVChannelName( int set )
		{
			string uvSetStr = ( set == 0 ) ? "texcoord" : ( "texcoord" + set.ToString() );
			return uvSetStr;
		}

		public static string Floatify( float value )
		{
			return ( value % 1 ) != 0 ? value.ToString() : ( value.ToString() + FloatifyStr );
		}

		public static string Vector2ToString( Vector2 data )
		{
			return data.x.ToString() + VECTOR_SEPARATOR + data.y.ToString();
		}

		public static string Vector3ToString( Vector3 data )
		{
			return data.x.ToString() + VECTOR_SEPARATOR + data.y.ToString() + VECTOR_SEPARATOR + data.z.ToString();
		}

		public static string Vector4ToString( Vector4 data )
		{
			return data.x.ToString() + VECTOR_SEPARATOR + data.y.ToString() + VECTOR_SEPARATOR + data.z.ToString() + VECTOR_SEPARATOR + data.w.ToString();
		}

		public static string ColorToString( Color data )
		{
			return data.r.ToString() + VECTOR_SEPARATOR + data.g.ToString() + VECTOR_SEPARATOR + data.b.ToString() + VECTOR_SEPARATOR + data.a.ToString();
		}

		public static string Matrix3x3ToString( Matrix4x4 matrix )
		{
			return matrix[ 0, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 0, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 0, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR +
					matrix[ 1, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 1, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 1, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR +
					matrix[ 2, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 2, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 2, 2 ].ToString();
		}

		public static string Matrix4x4ToString( Matrix4x4 matrix )
		{
			return	matrix[ 0, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 0, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 0, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 0, 3 ].ToString() + IOUtils.VECTOR_SEPARATOR +
					matrix[ 1, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 1, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 1, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 1, 3 ].ToString() + IOUtils.VECTOR_SEPARATOR +
					matrix[ 2, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 2, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 2, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 2, 3 ].ToString() + IOUtils.VECTOR_SEPARATOR +
					matrix[ 3, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 3, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 3, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR + matrix[ 3, 3 ].ToString();
		}

		public static Vector2 StringToVector2( string data )
		{
			string[] parsedData = data.Split( VECTOR_SEPARATOR );
			if ( parsedData.Length >= 2 )
			{
				return new Vector2( Convert.ToSingle( parsedData[ 0 ] ),
									Convert.ToSingle( parsedData[ 1 ] ) );
			}
			return Vector2.zero;
		}

		public static Vector3 StringToVector3( string data )
		{
			string[] parsedData = data.Split( VECTOR_SEPARATOR );
			if ( parsedData.Length >= 3 )
			{
				return new Vector3( Convert.ToSingle( parsedData[ 0 ] ),
									Convert.ToSingle( parsedData[ 1 ] ),
									Convert.ToSingle( parsedData[ 2 ] ) );
			}
			return Vector3.zero;
		}

		public static Vector4 StringToVector4( string data )
		{
			string[] parsedData = data.Split( VECTOR_SEPARATOR );
			if ( parsedData.Length >= 4 )
			{
				return new Vector4( Convert.ToSingle( parsedData[ 0 ] ),
									Convert.ToSingle( parsedData[ 1 ] ),
									Convert.ToSingle( parsedData[ 2 ] ),
									Convert.ToSingle( parsedData[ 3 ] ) );
			}
			return Vector4.zero;
		}

		public static Color StringToColor( string data )
		{
			string[] parsedData = data.Split( VECTOR_SEPARATOR );
			if ( parsedData.Length >= 4 )
			{
				return new Color( Convert.ToSingle( parsedData[ 0 ] ),
									Convert.ToSingle( parsedData[ 1 ] ),
									Convert.ToSingle( parsedData[ 2 ] ),
									Convert.ToSingle( parsedData[ 3 ] ) );
			}
			return Color.white;
		}

		public static Matrix4x4 StringToMatrix3x3( string data )
		{
			string[] parsedData = data.Split( VECTOR_SEPARATOR );
			if ( parsedData.Length == 9 )
			{
				Matrix4x4 matrix = new Matrix4x4();
				matrix[ 0, 0 ] = Convert.ToSingle( parsedData[ 0 ] );
				matrix[ 0, 1 ] = Convert.ToSingle( parsedData[ 1 ] );
				matrix[ 0, 2 ] = Convert.ToSingle( parsedData[ 2 ] );

				matrix[ 1, 0 ] = Convert.ToSingle( parsedData[ 3 ] );
				matrix[ 1, 1 ] = Convert.ToSingle( parsedData[ 4 ] );
				matrix[ 1, 2 ] = Convert.ToSingle( parsedData[ 5 ] );

				matrix[ 2, 0 ] = Convert.ToSingle( parsedData[ 6 ] );
				matrix[ 2, 1 ] = Convert.ToSingle( parsedData[ 7 ] );
				matrix[ 2, 2 ] = Convert.ToSingle( parsedData[ 8 ] );
				return matrix;
			}
			return Matrix4x4.identity;
		}

		public static Matrix4x4 StringToMatrix4x4( string data )
		{
			string[] parsedData = data.Split( VECTOR_SEPARATOR );
			if ( parsedData.Length == 16 )
			{
				Matrix4x4 matrix = new Matrix4x4();
				matrix[ 0, 0 ] = Convert.ToSingle( parsedData[ 0 ] );
				matrix[ 0, 1 ] = Convert.ToSingle( parsedData[ 1 ] );
				matrix[ 0, 2 ] = Convert.ToSingle( parsedData[ 2 ] );
				matrix[ 0, 3 ] = Convert.ToSingle( parsedData[ 3 ] );

				matrix[ 1, 0 ] = Convert.ToSingle( parsedData[ 4 ] );
				matrix[ 1, 1 ] = Convert.ToSingle( parsedData[ 5 ] );
				matrix[ 1, 2 ] = Convert.ToSingle( parsedData[ 6 ] );
				matrix[ 1, 3 ] = Convert.ToSingle( parsedData[ 7 ] );

				matrix[ 2, 0 ] = Convert.ToSingle( parsedData[ 8 ] );
				matrix[ 2, 1 ] = Convert.ToSingle( parsedData[ 9 ] );
				matrix[ 2, 2 ] = Convert.ToSingle( parsedData[ 10 ] );
				matrix[ 2, 3 ] = Convert.ToSingle( parsedData[ 11 ] );

				matrix[ 3, 0 ] = Convert.ToSingle( parsedData[ 12 ] );
				matrix[ 3, 1 ] = Convert.ToSingle( parsedData[ 13 ] );
				matrix[ 3, 2 ] = Convert.ToSingle( parsedData[ 14 ] );
				matrix[ 3, 3 ] = Convert.ToSingle( parsedData[ 15 ] );
				return matrix;
			}
			return Matrix4x4.identity;
		}

		public static void SaveTextureToDisk( Texture2D tex, string pathname )
		{
			byte[] rawData = tex.GetRawTextureData();
			Texture2D newTex = new Texture2D( tex.width, tex.height, tex.format,  tex.mipmapCount > 1, false );
			newTex.LoadRawTextureData( rawData );
			newTex.Apply();
			byte[] pngData = newTex.EncodeToPNG();
			File.WriteAllBytes( pathname, pngData );
		}

		//public static void SaveObjToList( string newObj )
		//{
		//	Debug.Log( UIUtils.CurrentWindow.Lastpath );
		//	UIUtils.CurrentWindow.Lastpath = newObj;
		//	string lastOpenedObj = EditorPrefs.GetString( IOUtils.LAST_OPENED_OBJ_ID );
		//	string[] allLocations = lastOpenedObj.Split( ':' );

		//	string lastLocation = allLocations[ allLocations.Length - 1 ];

		//	string resave = string.Empty;
		//	for ( int i = 0; i < allLocations.Length; i++ )
		//	{
		//		if ( string.IsNullOrEmpty( allLocations[ i ] ) )
		//			continue;

		//		resave += allLocations[ i ];
		//		resave += ":";
		//	}

		//	resave += newObj;
		//	EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, resave );
		//}

		//public static void DeleteObjFromList( string newObj )
		//{
		//	string lastOpenedObj = EditorPrefs.GetString( IOUtils.LAST_OPENED_OBJ_ID );
		//	string[] allLocations = lastOpenedObj.Split( ':' );

		//	string resave = string.Empty;
		//	for ( int i = 0; i < allLocations.Length; i++ )
		//	{
		//		if ( string.IsNullOrEmpty( allLocations[ i ] ) || newObj.Equals( allLocations[ i ] ) )
		//			continue;

		//		resave += allLocations[ i ];
		//		if ( i < allLocations.Length - 1 )
		//			resave += ":";
		//	}

		//	EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, resave );
		//}
	}
}
