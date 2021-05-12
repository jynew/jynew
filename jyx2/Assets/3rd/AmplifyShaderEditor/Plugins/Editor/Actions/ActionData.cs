// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>


using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class ActionData
	{
		public virtual void ExecuteForward() { }
		public virtual void ExecuteReverse() { }
	}

	// NODES
	// Create node
	public class CreateNodeActionData : ActionData
	{
		private int m_nodeId;
		private System.Type m_nodeType;
		private Vector2 m_nodePos;

		public CreateNodeActionData( ParentNode node )
		{
			m_nodeId = node.UniqueId;
			m_nodePos = node.Vec2Position;
			m_nodeType = node.GetType();
		}


		public CreateNodeActionData( int nodeId, System.Type nodeType, Vector2 nodePos )
		{
			m_nodeId = nodeId;
			m_nodePos = nodePos;
			m_nodeType = nodeType;
		}

		public override void ExecuteForward()
		{
			UIUtils.CreateNode( m_nodeType, false, m_nodePos, m_nodeId );
		}

		public override void ExecuteReverse()
		{
			UIUtils.DestroyNode( m_nodeId );
		}

		public override string ToString()
		{
			return "Create Node - Type: " + m_nodeType + " Node: " + m_nodeId + " Position: " + m_nodePos;
		}
	}

	// Destroy node
	public class DestroyNodeActionData : ActionData
	{
		private int m_nodeId;
		private System.Type m_nodeType;
		private Vector2 m_nodePos;

		public DestroyNodeActionData( ParentNode node )
		{
			m_nodeId = node.UniqueId;
			m_nodePos = node.Vec2Position;
			m_nodeType = node.GetType();
		}

		public DestroyNodeActionData( int nodeId, System.Type nodeType, Vector2 nodePos )
		{
			m_nodeId = nodeId;
			m_nodePos = nodePos;
			m_nodeType = nodeType;
		}

		public override void ExecuteForward()
		{
			UIUtils.DestroyNode( m_nodeId );
		}

		public override void ExecuteReverse()
		{
			UIUtils.CreateNode( m_nodeType, false, m_nodePos, m_nodeId );
		}

		public override string ToString()
		{
			return "Destroy Node - Type: " + m_nodeType + " Node: " + m_nodeId + " Position: " + m_nodePos;
		}
	}

	// Move node
	public class MoveNodeActionData : ActionData
	{
		private int m_nodeId;
		private Vector2 m_nodeInitalPos;
		private Vector2 m_nodeFinalPos;

		public MoveNodeActionData( int nodeId, Vector2 nodeInitialPos, Vector2 nodeFinalPos )
		{
			m_nodeId = nodeId;
			m_nodeInitalPos = nodeInitialPos;
			m_nodeFinalPos = nodeFinalPos;
		}

		public override void ExecuteForward()
		{
			ParentNode node = UIUtils.GetNode( m_nodeId );
			if ( node != null )
				node.Vec2Position = m_nodeFinalPos;
		}

		public override void ExecuteReverse()
		{
			ParentNode node = UIUtils.GetNode( m_nodeId );
			if ( node != null )
				node.Vec2Position = m_nodeInitalPos;
		}

		public override string ToString()
		{
			return "Move Node - Node: " + m_nodeId + " Initial Position: " + m_nodeInitalPos + " Final Position: " + m_nodeFinalPos;
		}
	}

	// CONNECTIONS
	// Create connection
	public class CreateConnectionActionData : ActionData
	{
		private int m_inputNodeId;
		private int m_inputPortId;

		private int m_outputNodeId;
		private int m_outputPortId;

		public CreateConnectionActionData( int inputNodeId, int inputPortId, int outputNodeId, int outputPortId )
		{
			m_inputNodeId = inputNodeId;
			m_inputPortId = inputPortId;
			m_outputNodeId = outputNodeId;
			m_outputPortId = outputPortId;
		}

		public override void ExecuteForward()
		{
			UIUtils.ConnectInputToOutput( m_inputNodeId, m_inputPortId, m_outputNodeId, m_outputPortId );
		}

		public override void ExecuteReverse()
		{
			UIUtils.DeleteConnection( true, m_inputNodeId, m_inputPortId, false, true );
		}

		public override string ToString()
		{
			return "Create Connection Node - Input Node: " + m_inputNodeId + " Input Port: " + m_inputPortId + " Output Node: " + m_outputNodeId + " Output Port: " + m_outputPortId;
		}
	}

	// Destroy connection
	public class DestroyConnectionActionData : ActionData
	{
		private int m_inputNodeId;
		private int m_inputPortId;

		private int m_outputNodeId;
		private int m_outputPortId;

		public DestroyConnectionActionData( int inputNodeId, int inputPortId, int outputNodeId, int outputPortId )
		{
			m_inputNodeId = inputNodeId;
			m_inputPortId = inputPortId;
			m_outputNodeId = outputNodeId;
			m_outputPortId = outputPortId;
		}

		public override void ExecuteForward()
		{
			UIUtils.DeleteConnection( true, m_inputNodeId, m_inputPortId, false, true );
		}

		public override void ExecuteReverse()
		{
			UIUtils.ConnectInputToOutput( m_inputNodeId, m_inputPortId, m_outputNodeId, m_outputPortId );
		}

		public override string ToString()
		{
			return "Destroy Connection Node - Input Node: " + m_inputNodeId + " Input Port: " + m_inputPortId + " Output Node: " + m_outputNodeId + " Output Port: " + m_outputPortId;
		}
	}

	// Move connection
	public class MoveInputConnectionActionData : ActionData
	{
		private int m_oldInputNodeId;
		private int m_oldInputNodePortId;

		private int m_newInputNodeId;
		private int m_newInputNodePortId;

		private int m_outputNodeId;
		private int m_outputPortId;

		public MoveInputConnectionActionData( int oldInputNodeId, int oldInputPortId, int newInputNodeId, int newInputPortId, int outputNodeId, int outputPortId )
		{
			m_oldInputNodeId = oldInputNodeId;
			m_oldInputNodePortId = oldInputPortId;

			m_newInputNodeId = newInputNodeId;
			m_newInputNodePortId = newInputPortId;

			m_outputNodeId = outputNodeId;
			m_outputPortId = outputPortId;
		}

		public override void ExecuteForward()
		{
			UIUtils.DeleteConnection( true, m_oldInputNodeId, m_oldInputNodePortId, false, true );
			UIUtils.ConnectInputToOutput( m_newInputNodeId, m_newInputNodePortId, m_outputNodeId, m_outputPortId );
		}

		public override void ExecuteReverse()
		{
			base.ExecuteReverse();
			UIUtils.DeleteConnection( true, m_newInputNodeId, m_newInputNodePortId, false, true );
			UIUtils.ConnectInputToOutput( m_oldInputNodeId, m_oldInputNodePortId, m_outputNodeId, m_outputPortId );
		}

		public override string ToString()
		{
			return "Move Input Connection Node - Old Input Node: " + m_oldInputNodeId + " Old Input Port: " + m_oldInputNodePortId + " New Input Node: " + m_newInputNodeId + " New Input Port: " + m_newInputNodePortId + " Output Node: " + m_outputNodeId + " Output Port: " + m_outputPortId;
		}
	}

	public class MoveOutputConnectionActionData : ActionData
	{
		private int m_inputNodeId;
		private int m_inputPortId;

		private int m_newOutputNodeId;
		private int m_newOutputPortId;

		private int m_oldOutputNodeId;
		private int m_oldOutputPortId;

		public MoveOutputConnectionActionData( int inputNodeId, int inputPortId, int newOutputNodeId, int newOutputPortId, int oldOutputNodeId, int oldOutputPortId )
		{
			m_inputNodeId = inputNodeId;
			m_inputPortId = inputPortId;

			m_newOutputNodeId = newOutputNodeId;
			m_newOutputPortId = newOutputPortId;

			m_oldOutputNodeId = oldOutputNodeId;
			m_oldOutputPortId = oldOutputPortId;
		}

		public override void ExecuteForward()
		{
			UIUtils.DeleteConnection( false, m_oldOutputNodeId, m_oldOutputNodeId, false, true );
			UIUtils.ConnectInputToOutput( m_inputNodeId, m_inputPortId, m_newOutputNodeId, m_newOutputPortId );
		}

		public override void ExecuteReverse()
		{
			base.ExecuteReverse();
			UIUtils.DeleteConnection( false, m_newOutputNodeId, m_newOutputPortId, false, true );
			UIUtils.ConnectInputToOutput( m_inputNodeId, m_inputPortId, m_oldOutputNodeId, m_oldOutputPortId );
		}

		public override string ToString()
		{
			return "Move Input Connection Node - Input Node: " + m_inputNodeId + " Input Port: " + m_inputPortId + " Old Output Node: " + m_oldOutputNodeId + " Old Output Port: " + m_oldOutputPortId + " New Output Node: " + m_newOutputNodeId + " New Output Port: " + m_newOutputPortId;
		}
	}

	public class CreateNewGraphActionData : ActionData
	{
		private string m_name;

		public CreateNewGraphActionData( string name )
		{
			m_name = name;
		}

		public override void ExecuteForward()
		{
			UIUtils.CreateNewGraph( m_name );
		}
	}

	public class ChangeNodePropertiesActionData : ActionData
	{
		private string m_originalProperties;
		private string m_newProperties;
		private int m_nodeId;

		public ChangeNodePropertiesActionData( ParentNode node, string originalProperties )
		{
			m_nodeId = node.UniqueId;
			m_originalProperties = originalProperties;

			m_newProperties = string.Empty;
			string trash = string.Empty;
			node.WriteToString( ref m_newProperties, ref trash );
		}

		public ChangeNodePropertiesActionData( int nodeId, string originalProperties )
		{
			m_nodeId = nodeId;
			m_originalProperties = originalProperties;

			m_newProperties = string.Empty;
			string trash = string.Empty;
			UIUtils.GetNode( nodeId ).WriteToString( ref m_newProperties, ref trash );
			Debug.Log( m_originalProperties + '\n' + m_newProperties );
		}

		public override void ExecuteForward()
		{
			string[] properties = m_newProperties.Split( IOUtils.FIELD_SEPARATOR );
			UIUtils.GetNode( m_nodeId ).ReadFromString( ref properties );
		}

		public override void ExecuteReverse()
		{
			string[] properties = m_originalProperties.Split( IOUtils.FIELD_SEPARATOR );
			UIUtils.GetNode( m_nodeId ).ReadFromString( ref properties );
		}

		public override string ToString()
		{
			return "Change Node Propertie - Node: " + m_nodeId + "\nOriginal Properties:\n" + m_originalProperties + "\nNew Properties:\n" + m_newProperties;
		}
	}
}
