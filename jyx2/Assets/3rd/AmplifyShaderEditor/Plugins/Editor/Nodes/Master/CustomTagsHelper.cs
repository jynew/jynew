using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class CustomTagData
	{
		private const string TagFormat = "\"{0}\"=\"{1}\"";
		public string TagName;
		public string TagValue;
		public int TagId = -1;
		public bool TagFoldout = true;

		[SerializeField]
		private TemplateSpecialTags m_specialTag = TemplateSpecialTags.None;
		[SerializeField]
		private RenderType m_renderType = RenderType.Opaque;
		[SerializeField]
		private RenderQueue m_renderQueue = RenderQueue.Geometry;
		[SerializeField]
		private int m_renderQueueOffset = 0;

		public CustomTagData()
		{
			TagName = string.Empty;
			TagValue = string.Empty;
			m_specialTag = TemplateSpecialTags.None;
			m_renderType = RenderType.Opaque;
			m_renderQueue = RenderQueue.Geometry;
			m_renderQueueOffset = 0;
		}

		public CustomTagData( CustomTagData other )
		{
			TagName = other.TagName;
			TagValue = other.TagValue;
			TagId = other.TagId;
			TagFoldout = other.TagFoldout;

			m_specialTag = other.m_specialTag;
			m_renderType = other.m_renderType;
			m_renderQueue = other.m_renderQueue;
			m_renderQueueOffset = other.m_renderQueueOffset;
		}

		public void SetTagValue( params string[] value )
		{
			TagValue = value[ 0 ];
			switch( m_specialTag )
			{
				case TemplateSpecialTags.RenderType:
				m_renderType = TemplateHelperFunctions.StringToRenderType[ value[ 0 ] ];
				break;
				case TemplateSpecialTags.Queue:
				{
					if( value.Length == 2 )
					{
						m_renderQueue = TemplateHelperFunctions.StringToRenderQueue[ value[ 0 ] ];
						int.TryParse( value[ 1 ], out m_renderQueueOffset );
					}
					else
					{
						int indexPlus = value[ 0 ].IndexOf( '+' );
						if( indexPlus > 0 )
						{
							string[] args = value[ 0 ].Split( '+' );
							m_renderQueue = TemplateHelperFunctions.StringToRenderQueue[ args[ 0 ] ];
							int.TryParse( args[ 1 ], out m_renderQueueOffset );
						}
						else
						{
							int indexMinus = value[ 0 ].IndexOf( '-' );
							if( indexMinus > 0 )
							{
								string[] args = value[ 0 ].Split( '-' );
								m_renderQueue = TemplateHelperFunctions.StringToRenderQueue[ args[ 0 ] ];
								int.TryParse( args[ 1 ], out m_renderQueueOffset );
								m_renderQueueOffset *= -1;
							}
							else
							{
								m_renderQueue = TemplateHelperFunctions.StringToRenderQueue[ value[ 0 ] ];
								m_renderQueueOffset = 0;
							}
						}
					}
					BuildQueueTagValue();
				}
				break;

			}
		}


		void CheckSpecialTag()
		{
			if( TagName.Equals( Constants.RenderTypeHelperStr ) )
			{
				m_specialTag = TemplateSpecialTags.RenderType;
				m_renderType = TemplateHelperFunctions.StringToRenderType[ TagValue ];
			}
			else if( TagName.Equals( Constants.RenderQueueHelperStr ) )
			{
				m_specialTag = TemplateSpecialTags.Queue;
				SetTagValue( TagValue );
			}
			else
			{
				m_specialTag = TemplateSpecialTags.None;
			}
		}

		public CustomTagData( string name, string value, int id )
		{
			TagName = name;
			TagValue = value;
			TagId = id;
			CheckSpecialTag();
		}

		//Used on Template based shaders loading
		public CustomTagData( string data, int id )
		{
			TagId = id;
			string[] arr = data.Split( IOUtils.VALUE_SEPARATOR );
			if( arr.Length > 1 )
			{
				TagName = arr[ 0 ];
				TagValue = arr[ 1 ];
			}

			if( arr.Length > 2 )
			{
				m_specialTag = (TemplateSpecialTags)Enum.Parse( typeof( TemplateSpecialTags ), arr[ 2 ] );
				switch( m_specialTag )
				{
					case TemplateSpecialTags.RenderType:
					{
						m_renderType = (RenderType)Enum.Parse( typeof( RenderType ), TagValue );
					}
					break;
					case TemplateSpecialTags.Queue:
					{
						if( arr.Length == 4 )
						{
							m_renderQueue = (RenderQueue)Enum.Parse( typeof( RenderQueue ), TagValue );
							int.TryParse( arr[ 3 ], out m_renderQueueOffset );
						}
						BuildQueueTagValue();
					}
					break;
				}
			}
			else if( UIUtils.CurrentShaderVersion() < 15600 )
			{
				CheckSpecialTag();
			}
		}

		//Used on Standard Surface shaders loading
		public CustomTagData( string data )
		{
			string[] arr = data.Split( IOUtils.VALUE_SEPARATOR );
			if( arr.Length > 1 )
			{
				TagName = arr[ 0 ];
				TagValue = arr[ 1 ];
			}
		}

		public override string ToString()
		{
			switch( m_specialTag )
			{
				case TemplateSpecialTags.RenderType:
				return TagName + IOUtils.VALUE_SEPARATOR +
						TagValue + IOUtils.VALUE_SEPARATOR +
						m_specialTag;
				case TemplateSpecialTags.Queue:
				return TagName + IOUtils.VALUE_SEPARATOR +
						m_renderQueue.ToString() + IOUtils.VALUE_SEPARATOR +
						m_specialTag + IOUtils.VALUE_SEPARATOR +
						m_renderQueueOffset;
			}

			return TagName + IOUtils.VALUE_SEPARATOR + TagValue;
		}

		public string GenerateTag()
		{
			return string.Format( TagFormat, TagName, TagValue );
		}

		public void BuildQueueTagValue()
		{
			TagValue = m_renderQueue.ToString();
			if( m_renderQueueOffset > 0 )
			{
				TagValue += "+" + m_renderQueueOffset;
			}
			else if( m_renderQueueOffset < 0 )
			{
				TagValue += m_renderQueueOffset;
			}
		}

		public TemplateSpecialTags SpecialTag
		{
			get { return m_specialTag; }
			set
			{
				m_specialTag = value;
				switch( value )
				{
					case TemplateSpecialTags.RenderType:
					{
						TagValue = m_renderType.ToString();
					}
					break;
					case TemplateSpecialTags.Queue:
					{
						BuildQueueTagValue();
					}
					break;
				}
			}
		}

		public RenderType RenderType
		{
			get { return m_renderType; }
			set
			{
				m_renderType = value;
				TagValue = value.ToString();
			}
		}

		public RenderQueue RenderQueue
		{
			get { return m_renderQueue; }
			set { m_renderQueue = value; }
		}
		public int RenderQueueOffset
		{
			get { return m_renderQueueOffset; }
			set { m_renderQueueOffset = value; }
		}

		public bool IsValid { get { return ( !string.IsNullOrEmpty( TagValue ) && !string.IsNullOrEmpty( TagName ) ); } }
	}

	[Serializable]
	public class CustomTagsHelper
	{
		private const string CustomTagsStr = " Custom SubShader Tags";
		private const string TagNameStr = "Name";
		private const string TagValueStr = "Value";

		private const float ShaderKeywordButtonLayoutWidth = 15;
		private ParentNode m_currentOwner;

		[SerializeField]
		private List<CustomTagData> m_availableTags = new List<CustomTagData>();

		public void Draw( ParentNode owner )
		{
			m_currentOwner = owner;
			bool value = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedCustomTags;
			NodeUtils.DrawPropertyGroup( ref value, CustomTagsStr, DrawMainBody, DrawButtons );
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedCustomTags = value;
		}

		void DrawButtons()
		{
			EditorGUILayout.Separator();

			// Add tag
			if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				m_availableTags.Add( new CustomTagData() );
				EditorGUI.FocusTextInControl( null );
			}

			//Remove tag
			if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				if( m_availableTags.Count > 0 )
				{
					m_availableTags.RemoveAt( m_availableTags.Count - 1 );
					EditorGUI.FocusTextInControl( null );
				}
			}
		}

		void DrawMainBody()
		{
			EditorGUILayout.Separator();
			int itemCount = m_availableTags.Count;

			if( itemCount == 0 )
			{
				EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add one.", MessageType.Info );
			}

			int markedToDelete = -1;
			float originalLabelWidth = EditorGUIUtility.labelWidth;
			for( int i = 0; i < itemCount; i++ )
			{
				m_availableTags[ i ].TagFoldout = m_currentOwner.EditorGUILayoutFoldout( m_availableTags[ i ].TagFoldout, string.Format( "[{0}] - {1}", i, m_availableTags[ i ].TagName ) );
				if( m_availableTags[ i ].TagFoldout )
				{
					EditorGUI.indentLevel += 1;
					EditorGUIUtility.labelWidth = 70;
					//Tag Name
					EditorGUI.BeginChangeCheck();
					m_availableTags[ i ].TagName = EditorGUILayout.TextField( TagNameStr, m_availableTags[ i ].TagName );
					if( EditorGUI.EndChangeCheck() )
					{
						m_availableTags[ i ].TagName = UIUtils.RemoveShaderInvalidCharacters( m_availableTags[ i ].TagName );
					}

					//Tag Value
					EditorGUI.BeginChangeCheck();
					m_availableTags[ i ].TagValue = EditorGUILayout.TextField( TagValueStr, m_availableTags[ i ].TagValue );
					if( EditorGUI.EndChangeCheck() )
					{
						m_availableTags[ i ].TagValue = UIUtils.RemoveShaderInvalidCharacters( m_availableTags[ i ].TagValue );
					}

					EditorGUIUtility.labelWidth = originalLabelWidth;

					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Label( " " );
						// Add new port
						if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
						{
							m_availableTags.Insert( i + 1, new CustomTagData() );
							EditorGUI.FocusTextInControl( null );
						}

						//Remove port
						if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
						{
							markedToDelete = i;
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUI.indentLevel -= 1;
				}

			}
			if( markedToDelete > -1 )
			{
				if( m_availableTags.Count > markedToDelete )
				{
					m_availableTags.RemoveAt( markedToDelete );
					EditorGUI.FocusTextInControl( null );
				}
			}
			EditorGUILayout.Separator();
		}


		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			int count = Convert.ToInt32( nodeParams[ index++ ] );
			for( int i = 0; i < count; i++ )
			{
				m_availableTags.Add( new CustomTagData( nodeParams[ index++ ] ) );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			int tagsCount = m_availableTags.Count;
			IOUtils.AddFieldValueToString( ref nodeInfo, tagsCount );
			for( int i = 0; i < tagsCount; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_availableTags[ i ].ToString() );
			}
		}

		public string GenerateCustomTags()
		{
			int tagsCount = m_availableTags.Count;
			string result = tagsCount == 0 ? string.Empty : " ";

			for( int i = 0; i < tagsCount; i++ )
			{
				if( m_availableTags[ i ].IsValid )
				{
					result += m_availableTags[ i ].GenerateTag();
					if( i < tagsCount - 1 )
					{
						result += " ";
					}
				}
			}
			return result;
		}

		public void Destroy()
		{
			m_availableTags.Clear();
			m_availableTags = null;
			m_currentOwner = null;
		}
	}
}
