// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public enum TemplateCodeSnippetType
	{
		Toggle
	};


	public enum TemplateCodeSnippetInfoIdx
	{
		Name = 0, 
		Type
	};

	[Serializable]
	public class TemplateCodeSnippetElement
	{
		public string Id;
		public string Snippet;
		public TemplateCodeSnippetElement( string id, string snippet )
		{
			Id = id;
			Snippet = snippet;
		}
	}

	[Serializable]
	public class TemplateCodeSnippetBase : ScriptableObject
	{
		[SerializeField]
		private string m_nameId;

		[SerializeField]
		private TemplateCodeSnippetType m_type;

		[SerializeField]
		private List<TemplateCodeSnippetElement> m_elements = new List<TemplateCodeSnippetElement>();
		
		public void Init( string nameId, TemplateCodeSnippetType type )
		{
			m_nameId = nameId;
			m_type = type;
		}

		public void AddSnippet( TemplateCodeSnippetElement element )
		{
			m_elements.Add( element );
		}

		public void Destroy()
		{
			for ( int i = 0; i < m_elements.Count; i++ )
			{
				m_elements[ i ].Snippet = null;
			}
			m_elements.Clear();
			m_elements = null;
		}

		public virtual void DrawProperties( ParentNode owner ) { }
		public virtual bool CheckSnippet() { return true; }

		public void InsertSnippet( ref string shaderBody )
		{
			bool insertSnippet = CheckSnippet();
			for ( int i = 0; i < m_elements.Count; i++ )
			{
				shaderBody = shaderBody.Replace( m_elements[ i ].Id, ( insertSnippet ? m_elements[ i ].Snippet : string.Empty ) );
			}
		}
		public string NameId { get { return m_nameId; } }
		public TemplateCodeSnippetType Type { get { return m_type; } }
		public List<TemplateCodeSnippetElement> Elements { get { return m_elements; } }
	}

	[Serializable]
	public class TemplateCodeSnippetToggle : TemplateCodeSnippetBase
	{
		private const string Label = "Activate";
		[SerializeField]
		private bool m_value = false;


		public override bool CheckSnippet()
		{
			return m_value;
		}

		public override void DrawProperties( ParentNode owner )
		{
			m_value = owner.EditorGUILayoutToggle( Label, m_value );
		}
	}
	
}
