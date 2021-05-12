using UnityEditor;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	public class OptionsWindow
	{
		private AmplifyShaderEditorWindow m_parentWindow = null;

		private bool m_coloredPorts = true;
		private bool m_multiLinePorts = true;
		private const string MultiLineId = "MultiLinePortsDefault";
		private const string ColorPortId = "ColoredPortsDefault";
		public OptionsWindow( AmplifyShaderEditorWindow parentWindow )
		{
			m_parentWindow = parentWindow;
			//Load ();
		}

		public void Init()
		{
			Load();
		}

		public void Destroy()
		{
			Save();
		}

		public void Save()
		{
			EditorPrefs.SetBool( ColorPortId, ColoredPorts );
			EditorPrefs.SetBool( MultiLineId, m_multiLinePorts );
		}

		public void Load()
		{
			ColoredPorts = EditorPrefs.GetBool( ColorPortId, true );
			m_multiLinePorts = EditorPrefs.GetBool( MultiLineId, true );
		}

		public bool ColoredPorts
		{
			get { return m_coloredPorts; }
			set
			{
				if ( m_coloredPorts != value )
					EditorPrefs.SetBool( ColorPortId, value );

				m_coloredPorts = value;
			}
		}

		public bool MultiLinePorts
		{
			get { return m_multiLinePorts; }
			set
			{
				if ( m_multiLinePorts != value )
					EditorPrefs.SetBool( MultiLineId, value );

				m_multiLinePorts = value;
			}
		}
		public AmplifyShaderEditorWindow ParentWindow { get { return m_parentWindow; } set { m_parentWindow = value; } }
	}
}
