using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateLocalVarData
	{
		[SerializeField]
		private WirePortDataType m_dataType = WirePortDataType.OBJECT;

		[SerializeField]
		private string m_localVarName = string.Empty;

		[SerializeField]
		private int m_position = -1;
		
		[SerializeField]
		private bool m_isSpecialVar = false;

		[SerializeField]
		private TemplateInfoOnSematics m_specialVarType;

		[SerializeField]
		private MasterNodePortCategory m_category;

		[SerializeField]
		private string m_id;

		public TemplateLocalVarData( WirePortDataType dataType, MasterNodePortCategory category, string localVarName, int position )
		{
			m_dataType = dataType;
			m_localVarName = localVarName;
			m_position = position;
			m_category = category;
			//Debug.Log( m_localVarName + " " + m_inputData.PortCategory + " " + m_inputData.PortName );
		}

		public TemplateLocalVarData( TemplateInfoOnSematics specialVarType,string id, WirePortDataType dataType, MasterNodePortCategory category, string localVarName, int position )
		{
			m_id = id;
			m_dataType = dataType;
			m_localVarName = localVarName;
			m_position = position;
			m_specialVarType = specialVarType;
			m_isSpecialVar = true;
			m_category = category;
			//Debug.Log( m_localVarName + " " + m_inputData.PortCategory + " " + m_inputData.PortName );
		}
		
		public WirePortDataType DataType { get { return m_dataType; } }
		public string LocalVarName { get { return m_localVarName; } }
		public int Position { get { return m_position; } }
		public bool IsSpecialVar { get { return m_isSpecialVar; } }
		public TemplateInfoOnSematics SpecialVarType{ get { return m_specialVarType; } }
		public MasterNodePortCategory Category { get { return m_category; } }
		public string Id { get { return m_id; } }
	}
}
