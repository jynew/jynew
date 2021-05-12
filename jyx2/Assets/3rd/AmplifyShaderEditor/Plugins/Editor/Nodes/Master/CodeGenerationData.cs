// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
namespace AmplifyShaderEditor
{
	[System.Serializable]
	public class CodeGenerationData
	{
		[SerializeField]
		public bool IsActive;
		[SerializeField]
		public string Name;
		[SerializeField]
		public string Value;

		public CodeGenerationData( string name, string value )
		{
			IsActive = false;
			Name = name;
			Value = value;
		}
	}
}
