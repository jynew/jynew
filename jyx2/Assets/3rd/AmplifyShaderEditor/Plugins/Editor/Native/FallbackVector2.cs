// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class FallbackVector2 : IFallbackVars
	{
		[SerializeField]
		private Vector2 m_current;
		[SerializeField]
		private Vector2 m_previous;

		public FallbackVector2()
		{
			m_current = new Vector2( 0, 0 );
			m_previous = new Vector2( 0, 0 );
		}

		public FallbackVector2( Vector2 data )
		{
			m_current = data;
			m_previous = data;
		}

		public void Revert()
		{
			Vector2 aux = m_current;
			m_current = m_previous;
			m_previous = aux;
		}


		public Vector2 Current
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
