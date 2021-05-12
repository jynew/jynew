// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class VersionInfo
	{
		public const byte Major = 1;
		public const byte Minor = 6;
		public const byte Release = 1;
		public static byte Revision = 00;
		
		//private static string StageSuffix = "_dev0"+Revision;
		
		//public static string StaticToString()
		//{
		//	return string.Format( "{0}.{1}.{2}", Major, Minor, Release ) + StageSuffix;
		//}
		
		public static string StaticToString()
		{
			return string.Format( "{0}.{1}.{2}", Major, Minor, Release ) + ( Revision > 0 ? "r" + Revision.ToString() : "" );
		}

		public static int FullNumber { get { return Major * 10000 + Minor * 1000 + Release * 100 + Revision; } }
		public static string FullLabel { get { return "Version=" + FullNumber; } }
	}
}
