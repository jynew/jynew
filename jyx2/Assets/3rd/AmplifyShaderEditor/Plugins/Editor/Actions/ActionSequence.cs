// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System.Collections.Generic;
namespace AmplifyShaderEditor
{
	public class ActionSequence
	{
		private string m_name;
		private List<ActionData> m_sequence;

		public ActionSequence( string name )
		{
			m_name = name;
			m_sequence = new List<ActionData>();
		}

		public void AddToSequence( ActionData actionData )
		{
			m_sequence.Add( actionData );
		}

		public void Execute()
		{
			for ( int i = 0; i < m_sequence.Count; i++ )
			{
				m_sequence[ i ].ExecuteForward();
			}
		}

		public void Destroy()
		{
			m_sequence.Clear();
			m_sequence = null;
		}

		public string Name { get { return m_name; } }
	}
}
