// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public enum TemplateDataType
	{
		LegacySinglePass,
		MultiPass
	}

	[Serializable]
	public class TemplateIncludePragmaContainter
	{
		[SerializeField]
		private List<string> m_nativeDirectivesList = new List<string>();

		[SerializeField]
		private List<string> m_includesList = new List<string>();
		private Dictionary<string,string> m_includesDict = new Dictionary<string,string>();

		[SerializeField]
		private List<string> m_pragmasList = new List<string>();
		private Dictionary<string, string> m_pragmasDict = new Dictionary<string, string>();

		[SerializeField]
		private List<string> m_definesList = new List<string>();
		private Dictionary<string, string> m_definesDict = new Dictionary<string, string>();

		public void RefreshIncludesList()
		{
			if ( m_includesDict.Count != m_includesList.Count )
			{
				m_includesDict.Clear();
				int count = m_includesList.Count;
				for ( int i = 0; i < count; i++ )
				{
					m_includesDict.Add( m_includesList[ i ], m_includesList[ i ] );
				}
			}
		}

		public void RefreshPragmasList()
		{
			if ( m_pragmasDict.Count != m_pragmasList.Count )
			{
				m_pragmasDict.Clear();
				int count = m_pragmasList.Count;
				for ( int i = 0; i < count; i++ )
				{
					m_pragmasDict.Add( m_pragmasList[ i ], m_pragmasList[ i ] );
				}
			}
		}


		public void RefreshDefinesList()
		{
			if ( m_definesDict.Count != m_definesList.Count )
			{
				m_definesDict.Clear();
				int count = m_definesList.Count;
				for ( int i = 0; i < count; i++ )
				{
					m_definesDict.Add( m_definesList[ i ], m_definesList[ i ] );
				}
			}
		}
		
		public bool HasInclude( string include )
		{
			RefreshIncludesList();
			return m_includesDict.ContainsKey( include );
		}

		public bool HasPragma( string pragma )
		{
			RefreshPragmasList();
			return m_pragmasDict.ContainsKey( pragma );
		}

		public bool HasDefine( string pragma )
		{
			RefreshDefinesList();
			return m_definesDict.ContainsKey( pragma );
		}

		public void AddInclude( string include )
		{
			RefreshIncludesList();
			if ( !m_includesDict.ContainsKey( include ) )
			{
				m_includesList.Add( include );
				m_includesDict.Add( include, include );
			}
		}

		public void AddPragma( string pragma )
		{
			RefreshPragmasList();
			if ( !m_pragmasDict.ContainsKey( pragma ) )
			{
				m_pragmasList.Add( pragma );
				m_pragmasDict.Add( pragma, pragma );
			}
		}

		public void AddDefine( string define )
		{
			RefreshDefinesList();
			if ( !m_definesDict.ContainsKey( define ) )
			{
				m_definesList.Add( define );
				m_definesDict.Add( define, define );
			}
		}

		public void AddNativeDirective( string native )
		{
			m_nativeDirectivesList.Add( native );
		}

		public void Destroy()
		{
			m_nativeDirectivesList.Clear();
			m_nativeDirectivesList = null;


			m_includesList.Clear();
			m_includesDict.Clear();
			m_includesList = null;
			m_includesDict = null;

			m_pragmasList.Clear();
			m_pragmasDict.Clear();
			m_pragmasList = null;
			m_pragmasDict = null;

			m_definesList.Clear();
			m_definesDict.Clear();
			m_definesList = null;
			m_definesDict = null;
		}

		public List<string> IncludesList { get { return m_includesList; } }
		public List<string> PragmasList { get { return m_pragmasList; } }
		public List<string> DefinesList { get { return m_definesList; } }
		public List<string> NativeDirectivesList { get { return m_nativeDirectivesList; } }

	}

	[Serializable]
	public class TemplateInfoContainer
	{
		public string Id = string.Empty;
		public string Data = string.Empty;
		public int Index = -1;
		public bool IsValid { get { return Index > -1; } }
		public void Reset()
		{
			Id = string.Empty;
			Data = string.Empty;
			Index = -1;
		}
	}

	[Serializable]
	public class TemplateDataParent : ScriptableObject
	{
		[SerializeField]
		protected TemplateDataType m_templateType;

		[SerializeField]
		protected string m_name;

		[SerializeField]
		protected string m_guid;

		[SerializeField]
		protected int m_orderId;

		[SerializeField]
		protected string m_defaultShaderName = string.Empty;

		[SerializeField]
		protected bool m_isValid = true;

		[SerializeField]
		protected bool m_communityTemplate = false;

		public virtual void Destroy() { }
		public virtual bool Reload() { return true; }
		public string Name
		{
			get { return m_name; }
			set
			{
				m_name = value.StartsWith( "Hidden/" ) ? value.Replace( "Hidden/", string.Empty ) : value;
			}
		}
		public string GUID { get { return m_guid; } set { m_guid = value; } }
		public int OrderId { get { return m_orderId; } set { m_orderId = value; } }
		public string DefaultShaderName { get { return m_defaultShaderName; } set { m_defaultShaderName = value; } }
		public bool IsValid { get { return m_isValid; } }
		public TemplateDataType TemplateType { get { return m_templateType; } }
		public virtual void Init( string name, string guid, bool isCommunity ) { m_communityTemplate = isCommunity; }
	}
}
