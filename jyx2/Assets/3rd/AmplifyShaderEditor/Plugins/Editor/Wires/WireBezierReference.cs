using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	public class WireBezierReference
	{
		private Rect m_boundingBox;
		private int m_inNodeId;
		private int m_inPortId;
		private int m_outNodeId;
		private int m_outPortId;

		public WireBezierReference()
		{
			m_boundingBox = new Rect();
			m_inNodeId = -1;
			m_inPortId = -1;
			m_outNodeId = -1;
			m_outPortId = -1;
		}

		public WireBezierReference( ref Rect area, int inNodeId, int inPortId, int outNodeId, int outPortId )
		{
			UpdateInfo( ref area, inNodeId, inPortId, outNodeId, outPortId );
		}

		public void UpdateInfo( ref Rect area, int inNodeId, int inPortId, int outNodeId, int outPortId )
		{
			m_boundingBox = area;
			m_inNodeId = inNodeId;
			m_inPortId = inPortId;
			m_outNodeId = outNodeId;
			m_outPortId = outPortId;
		}

		public bool Contains( Vector2 position )
		{
			return m_boundingBox.Contains( position );
		}

		public void DebugDraw()
		{
			GUI.Label( m_boundingBox, string.Empty, UIUtils.GetCustomStyle( CustomStyle.MainCanvasTitle ));
		}

		public override string ToString()
		{
			return string.Format( "In node: {0} port: {1} -> Out node: {2} port: {3}", m_inNodeId, m_inPortId, m_outNodeId, m_outPortId );
		}

		public Rect BoundingBox { get { return m_boundingBox; } }
		public int InNodeId { get { return m_inNodeId; } }
		public int InPortId { get { return m_inPortId; } }
		public int OutNodeId { get { return m_outNodeId; } }
		public int OutPortId { get { return m_outPortId; } }
	}
}
