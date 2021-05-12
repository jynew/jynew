// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class FallbackVector3 : IFallbackVars
	{
		[SerializeField]
		private Vector3 m_current;
		[SerializeField]
		private Vector3 m_previous;

		public FallbackVector3()
		{
			m_current = new Vector3( 0, 0, 0 );
			m_previous = new Vector3( 0, 0, 0 );
		}

		public FallbackVector3( Vector3 data )
		{
			m_current = data;
			m_previous = data;
		}

		public void Revert()
		{
			Vector3 aux = m_current;
			m_current = m_previous;
			m_previous = aux;
		}


		public Vector3 Current
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
