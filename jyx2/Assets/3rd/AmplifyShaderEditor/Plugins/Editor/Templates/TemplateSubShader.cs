// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateSubShader
	{
		[SerializeField]
		private int m_idx = -1;

		[SerializeField]
		private List<TemplatePass> m_passes = new List<TemplatePass>();

		[SerializeField]
		private TemplateModulesData m_modules;

		[SerializeField]
		private string m_uniquePrefix;

		[SerializeField]
		private TemplatePropertyContainer m_templateProperties = new TemplatePropertyContainer();

		[SerializeField]
		private List<TemplateShaderPropertyData> m_availableShaderGlobals = new List<TemplateShaderPropertyData>();

		[SerializeField]
		private TemplateInfoContainer m_LODContainer = new TemplateInfoContainer();

		[SerializeField]
		private int m_passAmount = 0;

		[SerializeField]
		private int m_mainPass = -1;

		[SerializeField]
		private bool m_foundMainPassTag = false;

		public TemplateSubShader( int subShaderIx, TemplateIdManager idManager, string uniquePrefix, TemplateSubShaderInfo subShaderData, ref Dictionary<string, TemplateShaderPropertyData> duplicatesHelper )
		{
			m_idx = subShaderIx;

			m_uniquePrefix = uniquePrefix;

			FetchLOD( subShaderData.StartIdx, subShaderData.Modules );
			if( m_LODContainer.Index > -1 )
			{
				idManager.RegisterId( m_LODContainer.Index, uniquePrefix + "Module" + m_LODContainer.Id, m_LODContainer.Id );
			}

			m_modules = new TemplateModulesData( idManager, m_templateProperties, uniquePrefix + "Module", subShaderData.StartIdx, subShaderData.Modules, true );
			if( m_modules.SRPType == TemplateSRPType.HD )
			{
				m_modules.SRPIsPBR = subShaderData.Data.Contains( TemplateHelperFunctions.HDPBRTag );
			}

			Dictionary<string, TemplateShaderPropertyData> ownDuplicatesDict = new Dictionary<string, TemplateShaderPropertyData>( duplicatesHelper );

			TemplateHelperFunctions.CreateShaderGlobalsList( subShaderData.Modules, ref m_availableShaderGlobals, ref ownDuplicatesDict );

			m_passAmount = subShaderData.Passes.Count;
			
			//if( !m_modules.PassTag.IsValid )
			//{
			//	m_modules.PassTag.StartIdx = subShaderData.Passes[ 0 ].GlobalStartIdx;
			//	m_templateProperties.AddId( subShaderData.Data, m_modules.PassTag.Id, subShaderData.Passes[ 0 ].LocalStartIdx, m_modules.PassTag.SearchIndentation );
			//	m_modules.PassTag.StartIdx -= m_templateProperties.PropertyDict[ m_modules.PassTag.Id ].Indentation.Length;
			//	m_templateProperties.PropertyDict[ m_modules.PassTag.Id ].UseIndentationAtStart = true;
			//	idManager.RegisterId( m_modules.PassTag.StartIdx, m_modules.UniquePrefix + m_modules.PassTag.Id, string.Empty );
			//}
			
			int firstVisible = -1;
			int currAddedPassIdx = 0;
			for( int passIdx = 0; passIdx < m_passAmount; passIdx++ )
			{
				TemplatePass newPass = new TemplatePass( m_modules,subShaderIx, passIdx, idManager, uniquePrefix + "Pass" + passIdx, subShaderData.Passes[ passIdx ].GlobalStartIdx, subShaderData.Passes[ passIdx ], ref ownDuplicatesDict );
				if( newPass.AddToList )
				{
					if( newPass.IsMainPass && m_mainPass < 0  )
					{
						m_mainPass = currAddedPassIdx;
						m_foundMainPassTag = true;
					}
					else if(!newPass.IsInvisible && firstVisible < 0 )
					{
						firstVisible = currAddedPassIdx;
					}

					m_passes.Add( newPass );
					currAddedPassIdx++;
				}
				else
				{
					newPass.Destroy();
					newPass = null;
				}

			}

			if( m_mainPass < 0 )
			{
				// If no main pass was set then choose the first visible one
				m_mainPass = ( firstVisible < 0 ) ? 0 : firstVisible;
				m_passes[ m_mainPass ].IsMainPass = true;
			}

			ownDuplicatesDict.Clear();
			ownDuplicatesDict = null;
		}

		public void Destroy()
		{
			m_LODContainer = null;

			m_templateProperties.Destroy();
			m_templateProperties = null;

			m_passes.Clear();
			m_passes = null;

			m_modules.Destroy();
			m_modules = null;

			m_availableShaderGlobals.Clear();
			m_availableShaderGlobals = null;

		}

		void FetchLOD( int offsetIdx, string body )
		{
			Match match = Regex.Match( body, TemplateHelperFunctions.SubShaderLODPattern );
			if( match != null && match.Groups.Count > 1 )
			{
				m_LODContainer.Id = match.Groups[ 0 ].Value;
				m_LODContainer.Data = match.Groups[ 1 ].Value;
				m_LODContainer.Index = offsetIdx + match.Index;
			}
		}
		
		public List<TemplatePass> Passes { get { return m_passes; } }
		public TemplateModulesData Modules { get { return m_modules; } }
		public string UniquePrefix { get { return m_uniquePrefix; } }
		public TemplatePropertyContainer TemplateProperties { get { return m_templateProperties; } }
		public List<TemplateShaderPropertyData> AvailableShaderGlobals { get { return m_availableShaderGlobals; } }
		public TemplateInfoContainer LODContainer { get { return m_LODContainer; } }
		public int PassAmount { get { return m_passAmount; } }
		public bool FoundMainPass { get { return m_foundMainPassTag; } }
		public int MainPass { get { return m_mainPass; } }
		public int Idx { get { return m_idx; } }
	}
}
