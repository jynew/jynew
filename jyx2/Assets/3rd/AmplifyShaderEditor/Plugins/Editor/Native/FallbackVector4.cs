// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class FallbackVector4 : IFallbackVars
	{
		[SerializeField]
		private Vector4 m_current;
		[SerializeField]
		private Vector4 m_previous;

		public FallbackVector4()
		{
			m_current = new Vector4( 0, 0, 0, 0 );
			m_previous = new Vector4( 0, 0, 0, 0 );
		}

		public FallbackVector4( Vector4 data )
		{
			m_current = data;
			m_previous = data;
		}

		public void Revert()
		{
			Vector4 aux = m_current;
			m_current = m_previous;
			m_previous = aux;
		}

		public Vector4 Current
		{
			get { return m_current; }
			set
			{
				m_previous = m_current;
				m_current = value;
			}
		}

		public override string ToString() { return m_current.ToString(); }
	}
}
