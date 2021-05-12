// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;

using System.Collections.Generic;
namespace AmplifyShaderEditor
{
	public class PreMadeShaders
	{
		public static readonly string FlatColorSequenceId = "Flat Color";
		private Dictionary<string, ActionSequence> m_actionLib;
		public PreMadeShaders()
		{
			m_actionLib = new Dictionary<string, ActionSequence>();
			ActionSequence sequence = new ActionSequence( FlatColorSequenceId );
			sequence.AddToSequence( new CreateNodeActionData( 1, typeof( ColorNode ), new Vector2( -250, 125 ) ) );
			sequence.AddToSequence( new CreateConnectionActionData( 0, 4, 1, 0 ) );
			m_actionLib.Add( sequence.Name, sequence );
		}

		public ActionSequence GetSequence( string name )
		{
			if ( m_actionLib.ContainsKey( name ) )
			{
				return m_actionLib[ name ];
			}
			return null;
		}

		public void Destroy()
		{
			var items = m_actionLib.GetEnumerator();
			while ( items.MoveNext() )
			{
				items.Current.Value.Destroy();
			}
			m_actionLib.Clear();
			m_actionLib = null;
		}

		public ActionSequence FlatColorSequence
		{
			get { return m_actionLib[ FlatColorSequenceId ]; }
		}
	}
}
