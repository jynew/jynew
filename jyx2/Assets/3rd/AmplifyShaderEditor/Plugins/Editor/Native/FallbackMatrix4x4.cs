// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class FallbackMatrix4x4 : IFallbackVars
	{
		[SerializeField]
		private Matrix4x4 m_current;
		[SerializeField]
		private Matrix4x4 m_previous;

		public FallbackMatrix4x4()
		{
			m_current = new Matrix4x4();
			m_previous = new Matrix4x4();
		}

		public FallbackMatrix4x4( Matrix4x4 data )
		{
			m_current = data;
			m_previous = data;
		}

		public void Revert()
		{
			Matrix4x4 aux = m_current;
			m_current = m_previous;
			m_previous = aux;
		}

		public Matrix4x4 Current
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
