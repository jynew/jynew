// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplatePassInfo
	{
		public string Modules;
		public string Data;
		public int GlobalStartIdx = -1;
		public int LocalStartIdx = -1;
	}

	[Serializable]
	public class TemplateSubShaderInfo
	{
		public string Data;
		public string Modules;
		public int StartIdx = -1;
		public List<TemplatePassInfo> Passes = new List<TemplatePassInfo>();
		public void Destroy()
		{
			Passes.Clear();
			Passes = null;
		}
	}

	[Serializable]
	public class TemplateShaderInfo
	{
		public string Body;
		public string Properties;
		public int PropertyStartIdx = -1;
		public List<TemplateSubShaderInfo> SubShaders = new List<TemplateSubShaderInfo>();
		public void Destroy()
		{
			int count = SubShaders.Count;
			for( int i = 0; i < count; i++ )
			{
				SubShaders[ i ].Destroy();
			}
			SubShaders.Clear();
			SubShaders = null;
		}
	}

	public class TemplateShaderInfoUtil
	{
		public static TemplateShaderInfo CreateShaderData( string body )
		{
			int nameBegin = body.IndexOf( TemplatesManager.TemplateShaderNameBeginTag );
			if( nameBegin < 0 )
			{
				// Not a template
				return null;
			}

			TemplateShaderInfo shaderData = null;
			//SHADER
			MatchCollection shaderMatch = Regex.Matches( body, "\\bShader\\b" );
			if( shaderMatch.Count > 0 )
			{
				//SUBSHADER
				MatchCollection subShaderMatch = Regex.Matches( body, TemplatesManager.TemplateMPSubShaderTag );
				int subShaderAmount = subShaderMatch.Count;
				if( subShaderAmount > 0 )
				{
					shaderData = new TemplateShaderInfo();
					shaderData.Body = body;
					int length = subShaderMatch[ 0 ].Index - shaderMatch[ 0 ].Groups[ 0 ].Index;
					shaderData.Properties = body.Substring( shaderMatch[ 0 ].Index, length );
					shaderData.PropertyStartIdx = body.IndexOf( TemplatesManager.TemplatePropertyTag );

					for( int subShaderIdx = 0; subShaderIdx < subShaderAmount; subShaderIdx++ )
					{
						TemplateSubShaderInfo subShaderData = new TemplateSubShaderInfo();
						int subshaderBeginIndex = subShaderMatch[ subShaderIdx ].Index;
						int subShaderEndIndex = ( subShaderIdx == ( subShaderAmount - 1 ) ) ? body.Length - 1 : subShaderMatch[ subShaderIdx + 1 ].Index;
						subShaderData.Data = body.Substring( subshaderBeginIndex, subShaderEndIndex - subshaderBeginIndex );
						subShaderData.StartIdx = subshaderBeginIndex;

						//PASS
						MatchCollection passMatch = Regex.Matches( subShaderData.Data, TemplatesManager.TemplatePassTagPattern );
						if( passMatch.Count == 0 )
						{
							passMatch = Regex.Matches( subShaderData.Data, TemplatesManager.TemplateMPPassTag );
						}

						int passCount = passMatch.Count;
						if( passCount > 0 )
						{
							subShaderData.Modules = subShaderData.Data.Substring( 0, passMatch[ 0 ].Index );
							for( int passIdx = 0; passIdx < passCount; passIdx++ )
							{
								int passBeginIndex = passMatch[ passIdx ].Index;
								int passEndIdx = ( passIdx == ( passCount - 1 ) ) ? subShaderData.Data.Length - 1 : passMatch[ passIdx + 1 ].Index;
								TemplatePassInfo passData = new TemplatePassInfo();
								passData.Data = subShaderData.Data.Substring( passBeginIndex, passEndIdx - passBeginIndex );
								passData.GlobalStartIdx = subshaderBeginIndex + passBeginIndex;
								passData.LocalStartIdx = passBeginIndex;
								subShaderData.Passes.Add( passData );
							}
							shaderData.SubShaders.Add( subShaderData );
						}
					}
				}
			}
			return shaderData;
		}
	}
}
