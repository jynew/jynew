// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class FallbackTexture : IFallbackVars
	{
		[SerializeField]
		private Texture m_current;
		[SerializeField]
		private Texture m_previous;

		public FallbackTexture()
		{
			m_current = null;
			m_previous = null;
		}

		public FallbackTexture( Texture data )
		{
			m_current = data;
			m_previous = data;
		}

		public void Revert()
		{
			Texture aux = m_current;
			m_current = m_previous;
			m_previous = aux;
		}

		public Texture Current
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
