// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class FallbackFloat : IFallbackVars
	{
		[SerializeField]
		private float m_current;
		[SerializeField]
		private float m_previous;

		public FallbackFloat()
		{
			m_current = 0;
			m_previous = 0;
		}

		public FallbackFloat( float data )
		{
			m_current = data;
			m_previous = data;
		}

		public void Revert()
		{
			float aux = m_current;
			m_current = m_previous;
			m_previous = aux;
		}


		public float Current
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
