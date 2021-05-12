// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class FallbackString : IFallbackVars
	{
		[SerializeField]
		private string m_current;
		[SerializeField]
		private string m_previous;

		public FallbackString()
		{
			m_current = string.Empty;
			m_previous = string.Empty;
		}

		public FallbackString( string data )
		{
			m_current = data;
			m_previous = data;
		}

		public void Revert()
		{
			string aux = m_current;
			m_current = m_previous;
			m_previous = aux;
		}


		public string Current
		{
			get
			{
				return m_current;
			}

			set
			{
				m_previous = m_current;
				m_current = value;
			}
		}

		public override string ToString()
		{
			return m_current;
		}
	}
}
