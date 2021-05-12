// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class FallbackInt : IFallbackVars
	{
		[SerializeField]
		private int m_current;
		[SerializeField]
		private int m_previous;

		public FallbackInt()
		{
			m_current = 0;
			m_previous = 0;
		}

		public FallbackInt( int data )
		{
			m_current = data;
			m_previous = data;
		}

		public void Revert()
		{
			int aux = m_current;
			m_current = m_previous;
			m_previous = aux;
		}


		public int Current
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
			return m_current.ToString();
		}
	}
}
