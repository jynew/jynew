#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class WindowHelper
{
	private class R_EditorWindow
	{
		private EditorWindow m_instance;
		private System.Type m_type;

		public R_EditorWindow( EditorWindow instance )
		{
			m_instance = instance;
			m_type = instance.GetType();
		}

		public object Parent
		{
			get
			{
				var field = m_type.GetField( "m_Parent", BindingFlags.Instance | BindingFlags.NonPublic );
				return field.GetValue( m_instance );
			}
		}
	}

	private class R_DockArea
	{
		private object m_instance;
		private System.Type m_type;

		public R_DockArea( object instance )
		{
			m_instance = instance;
			m_type = instance.GetType();
		}

		public object Window
		{
			get
			{
				var property = m_type.GetProperty( "window", BindingFlags.Instance | BindingFlags.Public );
				return property.GetValue( m_instance, null );
			}
		}

		public object OriginalDragSource
		{
			set
			{
				var field = m_type.GetField( "s_OriginalDragSource", BindingFlags.Static | BindingFlags.NonPublic );
				field.SetValue( null, value );
			}
		}

		public void AddTab( EditorWindow pane )
		{
#if UNITY_2018_3_OR_NEWER
			var method = m_type.GetMethod( "AddTab", BindingFlags.Instance | BindingFlags.Public, null, new System.Type[] { typeof( EditorWindow ), typeof( bool ) }, null );
			method.Invoke( m_instance, new object[] { pane, true } );
#else
			var method = m_type.GetMethod( "AddTab", BindingFlags.Instance | BindingFlags.Public, null, new System.Type[] { typeof( EditorWindow ) }, null );
			method.Invoke( m_instance, new object[] { pane } );
#endif
		}
	}

	private class R_ContainerWindow
	{
		private object m_instance;
		private System.Type m_type;

		public R_ContainerWindow( object instance )
		{
			m_instance = instance;
			m_type = instance.GetType();
		}


		public object RootSplitView
		{
			get
			{
				var property = m_type.GetProperty( "rootSplitView", BindingFlags.Instance | BindingFlags.Public );
				return property.GetValue( m_instance, null );
			}
		}
	}

	private class R_SplitView
	{
		private object m_instance;
		private System.Type m_type;

		public R_SplitView( object instance )
		{
			m_instance = instance;
			m_type = instance.GetType();
		}

		public object DragOver( EditorWindow child, Vector2 screenPoint )
		{
			var method = m_type.GetMethod( "DragOver", BindingFlags.Instance | BindingFlags.Public );
			return method.Invoke( m_instance, new object[] { child, screenPoint } );
		}

		public void PerformDrop( EditorWindow child, object dropInfo, Vector2 screenPoint )
		{
			var method = m_type.GetMethod( "PerformDrop", BindingFlags.Instance | BindingFlags.Public );
			method.Invoke( m_instance, new object[] { child, dropInfo, screenPoint } );
		}
	}

	public enum DockPosition
	{
		Left,
		Top,
		Right,
		Bottom
	}

	/// <summary>
	/// Docks the second window to the first window at the given position
	/// </summary>
	public static void Dock( this EditorWindow wnd, EditorWindow other, DockPosition position )
	{
		var mousePosition = GetFakeMousePosition( wnd, position );

		var parent = new R_EditorWindow( wnd );
		var child = new R_EditorWindow( other );
		var dockArea = new R_DockArea( parent.Parent );
		var containerWindow = new R_ContainerWindow( dockArea.Window );
		var splitView = new R_SplitView( containerWindow.RootSplitView );
		var dropInfo = splitView.DragOver( other, mousePosition );
		dockArea.OriginalDragSource = child.Parent;
		splitView.PerformDrop( other, dropInfo, mousePosition );
	}


	/// <summary>
	/// Adds the the second window as a tab at the end of the first window tab list
	/// </summary>
	/// <param name="existingWindow"></param>
	/// <param name="newWindow"></param>
	public static void AddTab( this EditorWindow existingWindow, EditorWindow newWindow )
	{
		var parent = new R_EditorWindow( existingWindow );
		var child = new R_EditorWindow( newWindow );
		var dockArea = new R_DockArea( parent.Parent );
		dockArea.OriginalDragSource = child.Parent;
		dockArea.AddTab( newWindow );
	}

	private static Vector2 GetFakeMousePosition( EditorWindow wnd, DockPosition position )
	{
		Vector2 mousePosition = Vector2.zero;

		// The 20 is required to make the docking work.
		// Smaller values might not work when faking the mouse position.
		switch ( position )
		{
			case DockPosition.Left:
			mousePosition = new Vector2( 20, wnd.position.size.y / 2 );
			break;
			case DockPosition.Top:
			mousePosition = new Vector2( wnd.position.size.x / 2, 20 );
			break;
			case DockPosition.Right:
			mousePosition = new Vector2( wnd.position.size.x - 20, wnd.position.size.y / 2 );
			break;
			case DockPosition.Bottom:
			mousePosition = new Vector2( wnd.position.size.x / 2, wnd.position.size.y - 20 );
			break;
		}

		return GUIUtility.GUIToScreenPoint( mousePosition );
	}
}
#endif
