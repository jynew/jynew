// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public enum MessageSeverity
	{
		Normal,
		Warning,
		Error
	}
	public class GenericMessageData
	{
		public string message;
		public MessageSeverity severity;
		public GenericMessageData( string msg, MessageSeverity svrty )
		{
			message = msg;
			severity = svrty;
		}
	}

	class GenericMessageUI
	{
		public delegate void OnMessageDisplay( string message, MessageSeverity severity );
		public event OnMessageDisplay OnMessageDisplayEvent;

		private const double MESSAGE_TIME = 2;
		private double m_currentMessageStartTime;
		private Queue<GenericMessageData> m_messageQueue;
		private bool m_displayingMessage;

		public GenericMessageUI()
		{
			m_messageQueue = new Queue<GenericMessageData>();
			m_displayingMessage = false;
			m_currentMessageStartTime = EditorApplication.timeSinceStartup;
		}
		
		public void Destroy()
		{
			m_messageQueue.Clear();
			OnMessageDisplayEvent = null;
		}

		public void AddToQueue( string message, MessageSeverity severity )
		{
			m_messageQueue.Enqueue( new GenericMessageData( message, severity ) );
		}

		public void Log( string message )
		{
			m_messageQueue.Enqueue( new GenericMessageData( message, MessageSeverity.Normal ) );
			Debug.Log( message );
		}

		public void LogError( string message )
		{
			m_messageQueue.Enqueue( new GenericMessageData( message, MessageSeverity.Error ) );
			Debug.LogError( message );
		}

		public void LogWarning( string message )
		{
			m_messageQueue.Enqueue( new GenericMessageData( message, MessageSeverity.Warning ) );
			Debug.LogWarning( message );
		}

		public void CheckForMessages()
		{
			if ( m_displayingMessage )
			{
				double timeLeft = EditorApplication.timeSinceStartup - m_currentMessageStartTime;
				if ( timeLeft > MESSAGE_TIME )
				{
					m_displayingMessage = false;
				}
			}

			if ( !m_displayingMessage )
			{
				if ( m_messageQueue.Count > 0 )
				{
					m_displayingMessage = true;
					GenericMessageData data = m_messageQueue.Dequeue();
					m_currentMessageStartTime = EditorApplication.timeSinceStartup;

					if ( OnMessageDisplayEvent != null )
						OnMessageDisplayEvent( data.message, data.severity );
				}
			}
		}

		public void CleanUpMessageStack()
		{
			m_displayingMessage = false;
			m_messageQueue.Clear();
		}

		public void StartMessageCounter()
		{
			m_displayingMessage = true;
			m_currentMessageStartTime = EditorApplication.timeSinceStartup;
		}

		public bool DisplayingMessage
		{
			get { return ( m_displayingMessage || m_messageQueue.Count > 0 ); }
		}
	}
}
