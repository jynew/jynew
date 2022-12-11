using System;
using UnityEditor;
using UnityEngine;
namespace WH.Editor
{
	internal class ListViewGUI
	{
		private static int[] dummyWidths = new int[1];
		internal static ListViewShared.InternalListViewState ilvState = new ListViewShared.InternalListViewState();
		private static int listViewHash = "ListView".GetHashCode();
		public static ListViewShared.ListViewElementsEnumerator ListView(Rect pos, ListViewState state)
		{
			return ListViewGUI.DoListView(pos, state, null, string.Empty);
		}
		public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, GUIStyle style, params GUILayoutOption[] options)
		{
			return ListViewGUI.ListView(state, (ListViewOptions)0, null, string.Empty, style, options);
		}
		public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, int[] colWidths, GUIStyle style, params GUILayoutOption[] options)
		{
			return ListViewGUI.ListView(state, (ListViewOptions)0, colWidths, string.Empty, style, options);
		}
		public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, ListViewOptions lvOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			return ListViewGUI.ListView(state, lvOptions, null, string.Empty, style, options);
		}
		public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, ListViewOptions lvOptions, string dragTitle, GUIStyle style, params GUILayoutOption[] options)
		{
			return ListViewGUI.ListView(state, lvOptions, null, dragTitle, style, options);
		}
		public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, ListViewOptions lvOptions, int[] colWidths, string dragTitle, GUIStyle style, params GUILayoutOption[] options)
		{
			GUILayout.BeginHorizontal(style, new GUILayoutOption[0]);
			state.scrollPos = EditorGUILayout.BeginScrollView(state.scrollPos, options);
			ListViewGUI.ilvState.beganHorizontal = true;
			state.draggedFrom = -1;
			state.draggedTo = -1;
			state.fileNames = null;
			if ((lvOptions & ListViewOptions.wantsReordering) != (ListViewOptions)0)
			{
				ListViewGUI.ilvState.wantsReordering = true;
			}
			if ((lvOptions & ListViewOptions.wantsExternalFiles) != (ListViewOptions)0)
			{
				ListViewGUI.ilvState.wantsExternalFiles = true;
			}
			if ((lvOptions & ListViewOptions.wantsToStartCustomDrag) != (ListViewOptions)0)
			{
				ListViewGUI.ilvState.wantsToStartCustomDrag = true;
			}
			if ((lvOptions & ListViewOptions.wantsToAcceptCustomDrag) != (ListViewOptions)0)
			{
				ListViewGUI.ilvState.wantsToAcceptCustomDrag = true;
			}
			return ListViewGUI.DoListView(GUILayoutUtility.GetRect(1f, (float)(state.totalRows * state.rowHeight + 3)), state, colWidths, string.Empty);
		}
		public static ListViewShared.ListViewElementsEnumerator DoListView(Rect pos, ListViewState state, int[] colWidths, string dragTitle)
		{
			int controlID = GUIUtility.GetControlID(ListViewGUI.listViewHash, FocusType.Passive);
			state.ID = controlID;
			state.selectionChanged = false;
			Rect rect;
			if (GUIClipHelper.visibleRect.x < 0f || GUIClipHelper.visibleRect.y < 0f)
			{
				rect = pos;
			}
			else
			{
				rect = ((pos.y >= 0f) ? new Rect(0f, state.scrollPos.y, GUIClipHelper.visibleRect.width, GUIClipHelper.visibleRect.height) : new Rect(0f, 0f, GUIClipHelper.visibleRect.width, GUIClipHelper.visibleRect.height));
			}
			if (rect.width <= 0f)
			{
				rect.width = 1f;
			}
			if (rect.height <= 0f)
			{
				rect.height = 1f;
			}
			ListViewGUI.ilvState.rect = rect;
			int num = (int)((-pos.y + rect.yMin) / (float)state.rowHeight);
			int num2 = num + (int)Math.Ceiling((double)(((rect.yMin - pos.y) % (float)state.rowHeight + rect.height) / (float)state.rowHeight)) - 1;
			if (colWidths == null)
			{
				ListViewGUI.dummyWidths[0] = (int)rect.width;
				colWidths = ListViewGUI.dummyWidths;
			}
			ListViewGUI.ilvState.invisibleRows = num;
			ListViewGUI.ilvState.endRow = num2;
			ListViewGUI.ilvState.rectHeight = (int)rect.height;
			ListViewGUI.ilvState.state = state;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 >= state.totalRows)
			{
				num2 = state.totalRows - 1;
			}
			return new ListViewShared.ListViewElementsEnumerator(ListViewGUI.ilvState, colWidths, num, num2, dragTitle, new Rect(0f, (float)(num * state.rowHeight), pos.width, (float)state.rowHeight));
		}
		public static bool MultiSelection(int prevSelected, int currSelected, ref int initialSelected, ref bool[] selectedItems)
		{
			return ListViewShared.MultiSelection(ListViewGUI.ilvState, prevSelected, currSelected, ref initialSelected, ref selectedItems);
		}
		public static bool HasMouseUp(Rect r)
		{
			return ListViewShared.HasMouseUp(ListViewGUI.ilvState, r, 0);
		}
		public static bool HasMouseDown(Rect r)
		{
			return ListViewShared.HasMouseDown(ListViewGUI.ilvState, r, 0);
		}
		public static bool HasMouseDown(Rect r, int button)
		{
			return ListViewShared.HasMouseDown(ListViewGUI.ilvState, r, button);
		}
	}
}
