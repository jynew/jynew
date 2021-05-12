// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	//GENERICS DON'T WORK WITH HOT CODE RELOAD
	[Serializable]
	public class FallbackVariable<T>
	{
		[SerializeField]
		private T m_current;
		[SerializeField]
		private T m_last;

		public void Revert()
		{
			m_current = m_last;
		}

		public T Current
		{
			get
			{
				return m_current;
			}
			set
			{
				m_last = m_current;
				m_current = value;
			}
		}
	}
}
