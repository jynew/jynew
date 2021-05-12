// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateTag
	{
		public string Tag = string.Empty;
		public string Replacement = string.Empty;
		public TemplateTag( string tag, string replacement = null )
		{
			Tag = tag;
			if( replacement != null )
				Replacement = replacement;
		}
	}

	[Serializable]
	public class TemplateId
	{
		public int StartIdx = -1;
		public string UniqueID;
		public string Tag;
		public string ReplacementText;
		public bool IsReplaced = false;
		public bool EmptyReplacer = false;
		public TemplateId( int bodyIdx, string uniqueID, string tag, bool emptyReplacer = false )
		{
			StartIdx = bodyIdx;
			UniqueID = uniqueID;
			Tag = tag;
			EmptyReplacer = emptyReplacer;
			ReplacementText = emptyReplacer ? string.Empty : tag;
		}

		public void SetReplacementText( string replacementText )
		{
			ReplacementText = replacementText;
			IsReplaced = true;
		}

		public void Reset()
		{
			ReplacementText = EmptyReplacer?string.Empty:Tag;
			IsReplaced = false;
		}
	}

	[Serializable]
	public class TemplateIdManager
	{
		[SerializeField]
		private bool m_isSorted = false;
		[SerializeField]
		private string m_shaderBody;
		[SerializeField]
		private List<TemplateId> m_registeredIds = new List<TemplateId>();

		[SerializeField]
		private List<TemplateTag> m_registeredTags = new List<TemplateTag>();

		private Dictionary<string, TemplateId> m_registeredIdsDict = new Dictionary<string, TemplateId>();

		public TemplateIdManager( string shaderBody )
		{
			m_shaderBody = shaderBody;
		}

		public void Destroy()
		{
			m_registeredTags.Clear();
			m_registeredTags = null;

			m_registeredIds.Clear();
			m_registeredIds = null;
			if( m_registeredIdsDict != null )
			{
				m_registeredIdsDict.Clear();
				m_registeredIdsDict = null;
			}
		}

		void RefreshIds()
		{
			if( m_registeredIdsDict == null )
			{
				m_registeredIdsDict = new Dictionary<string, TemplateId>();
			}

			if( m_registeredIdsDict.Count != m_registeredIds.Count )
			{
				m_registeredIdsDict.Clear();
				int count = m_registeredIds.Count;
				for( int i = 0; i < count; i++ )
				{
					m_registeredIdsDict.Add( m_registeredIds[ i ].UniqueID, m_registeredIds[ i ] );
				}
			}
		}

		public void RegisterId( int bodyIdx, string uniqueID, string tag, bool emptyReplacer = false )
		{
			if( bodyIdx < 0 )
				return;

			RefreshIds();

			TemplateId templateId = new TemplateId( bodyIdx, uniqueID, tag, emptyReplacer );
			m_registeredIds.Add( templateId );
			m_registeredIdsDict.Add( uniqueID, templateId );
		}

		public void RegisterTag( string tag, string replacement = null )
		{
			m_registeredTags.Add( new TemplateTag( tag, replacement ) );
		}

		public void SetReplacementText( string uniqueId, string replacementText )
		{
			RefreshIds();

			if( m_registeredIdsDict.ContainsKey( uniqueId ) && m_registeredIdsDict[ uniqueId ].StartIdx >= 0 )
				m_registeredIdsDict[ uniqueId ].SetReplacementText( replacementText );
		}


		public string BuildShader()
		{
			if( !m_isSorted )
			{
				m_registeredIds.Sort( ( x, y ) => { return x.StartIdx.CompareTo( y.StartIdx ); } );
			}

			int idCount = m_registeredIds.Count;
			int offset = 0;
			string finalShaderBody = m_shaderBody;
			for( int i = 0; i < idCount; i++ )
			{
				if( m_registeredIds[ i ].StartIdx >= 0 && m_registeredIds[ i ].IsReplaced )
				{
					finalShaderBody = finalShaderBody.ReplaceAt( m_registeredIds[ i ].Tag, m_registeredIds[ i ].ReplacementText, offset + m_registeredIds[ i ].StartIdx );
					offset += ( m_registeredIds[ i ].ReplacementText.Length - m_registeredIds[ i ].Tag.Length );
				}
			}

			for( int i = 0; i < idCount; i++ )
			{
				if( !m_registeredIds[ i ].IsReplaced && !m_registeredIds[ i ].Tag.Equals( m_registeredIds[ i ].ReplacementText ) )
				{
					finalShaderBody = finalShaderBody.Replace( m_registeredIds[ i ].Tag, m_registeredIds[ i ].ReplacementText );
				}
			}

			int tagCount = m_registeredTags.Count;
			for( int i = 0; i < tagCount; i++ )
			{
				finalShaderBody = finalShaderBody.Replace( m_registeredTags[ i ].Tag, m_registeredTags[ i ].Replacement );
			}

			//finalShaderBody = finalShaderBody.Replace( TemplatesManager.TemplateExcludeFromGraphTag, string.Empty );
			finalShaderBody = finalShaderBody.Replace( TemplatesManager.TemplateMainPassTag, string.Empty );

			return finalShaderBody;
		}

		public void ResetRegistersState()
		{
			int count = m_registeredIds.Count;
			for( int i = 0; i < count; i++ )
			{
				m_registeredIds[ i ].Reset();
			}
		}

		public void Reset()
		{
			m_registeredIds.Clear();
			if( m_registeredIdsDict == null )
			{
				m_registeredIdsDict = new Dictionary<string, TemplateId>();
			}
			else
			{
				m_registeredIdsDict.Clear();
			}
		}

		public string ShaderBody
		{
			get { return m_shaderBody; }
			set { m_shaderBody = value; }
		}
	}
}
