using UnityEngine;

namespace AmplifyShaderEditor
{
	public class NodeWireReferencesUtils
	{
		public WireReference InputPortReference = new WireReference();
		public WireReference SwitchPortReference = new WireReference();
		public WireReference OutputPortReference = new WireReference();

		public Vector2 SnapPosition = Vector2.zero;
		public bool SnapEnabled = false;
		public WireReference SnapPort = new WireReference();

		public bool ValidReferences()
		{
			return ( InputPortReference.IsValid || OutputPortReference.IsValid );
		}

		public void InvalidateReferences()
		{
			InputPortReference.Invalidate();
			OutputPortReference.Invalidate();
			SnapPort.Invalidate();
			SnapEnabled = false;
		}


		public void SetOutputReference( int nodeId, int portId, WirePortDataType dataType, bool typeLocked )
		{
			if( InputPortReference.IsValid )
				InputPortReference.Invalidate();
			OutputPortReference.SetReference( nodeId, portId, dataType, typeLocked );
		}

		public void SetInputReference( int nodeId, int portId, WirePortDataType dataType, bool typeLocked )
		{
			if( OutputPortReference.IsValid )
				OutputPortReference.Invalidate();
			InputPortReference.SetReference( nodeId, portId, dataType, typeLocked );
		}

		public void ActivateSnap( Vector2 position, WirePort port )
		{
			SnapPort.SetReference( port );
			SnapEnabled = true;
			SnapPosition = position;
		}

		public void DeactivateSnap()
		{
			SnapEnabled = false;
			SnapPort.Invalidate();
		}
	}
}
