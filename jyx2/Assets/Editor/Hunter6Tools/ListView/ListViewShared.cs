using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace WH.Editor
{
	internal class ListViewShared
	{
		internal class InternalListViewState
		{
			public int id = -1;
			public int invisibleRows;
			public int endRow;
			public int rectHeight;
			public ListViewState state;
			public bool beganHorizontal;
			public Rect rect;
			public bool wantsReordering;
			public bool wantsExternalFiles;
			public bool wantsToStartCustomDrag;
			public bool wantsToAcceptCustomDrag;
			public int dragItem;
		}
		internal class Constants
		{
			public static string insertion = "PR Insertion";
		}
		internal class ListViewElementsEnumerator : IDisposable, IEnumerator, IEnumerator<ListViewElement>
		{
			private int[] colWidths;
			private int xTo;
			private int yFrom;
			private int yTo;
			private Rect firstRect;
			private Rect rect;
			private int xPos = -1;
			private int yPos = -1;
			private ListViewElement element;
			private ListViewShared.InternalListViewState ilvState;
			private bool quiting;
			private bool isLayouted;
			private string dragTitle;
			ListViewElement IEnumerator<ListViewElement>.Current
			{
				get
				{
					return this.element;
				}
			}
			object IEnumerator.Current
			{
				get
				{
					return this.element;
				}
			}
			internal ListViewElementsEnumerator(ListViewShared.InternalListViewState ilvState, int[] colWidths, int yFrom, int yTo, string dragTitle, Rect firstRect)
			{
				this.colWidths = colWidths;
				this.xTo = colWidths.Length - 1;
				this.yFrom = yFrom;
				this.yTo = yTo;
				this.firstRect = firstRect;
				this.rect = firstRect;
				this.quiting = (ilvState.state.totalRows == 0);
				this.ilvState = ilvState;
				this.dragTitle = dragTitle;
				ilvState.state.customDraggedFromID = 0;
				this.Reset();
			}
			public bool MoveNext()
			{
				if (this.xPos > -1)
				{
					if (ListViewShared.HasMouseDown(this.ilvState, this.rect))
					{
						this.ilvState.state.selectionChanged = true;
						this.ilvState.state.row = this.yPos;
						this.ilvState.state.column = this.xPos;
						this.ilvState.state.scrollPos = ListViewShared.ListViewScrollToRow(this.ilvState, this.yPos);
						if ((this.ilvState.wantsReordering || this.ilvState.wantsToStartCustomDrag) && GUIUtility.hotControl == this.ilvState.state.ID)
						{
							DragAndDropDelay dragAndDropDelay = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), this.ilvState.state.ID);
							dragAndDropDelay.mouseDownPosition = Event.current.mousePosition;
							this.ilvState.dragItem = this.yPos;
							ListViewShared.dragControlID = this.ilvState.state.ID;
						}
					}
					if ((this.ilvState.wantsReordering || this.ilvState.wantsToStartCustomDrag) && GUIUtility.hotControl == this.ilvState.state.ID && Event.current.type == EventType.MouseDrag && GUIClipHelper.visibleRect.Contains(Event.current.mousePosition))
					{
						DragAndDropDelay dragAndDropDelay2 = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), this.ilvState.state.ID);
						if (dragAndDropDelay2.CanStartDrag())
						{
							DragAndDrop.PrepareStartDrag();
							DragAndDrop.objectReferences = new UnityEngine.Object[0];
							DragAndDrop.paths = null;
							if (this.ilvState.wantsReordering)
							{
								this.ilvState.state.dropHereRect = new Rect(this.ilvState.rect.x, 0f, this.ilvState.rect.width, (float)(this.ilvState.state.rowHeight * 2));
								DragAndDrop.StartDrag(this.dragTitle);
							}
							else
							{
								if (this.ilvState.wantsToStartCustomDrag)
								{
									DragAndDrop.SetGenericData("CustomDragID", this.ilvState.state.ID);
									DragAndDrop.StartDrag(this.dragTitle);
								}
							}
						}
						Event.current.Use();
					}
				}
				this.xPos++;
				if (this.xPos > this.xTo)
				{
					this.xPos = 0;
					this.yPos++;
					this.rect.x = this.firstRect.x;
					this.rect.width = (float)this.colWidths[0];
					if (this.yPos > this.yTo)
					{
						this.quiting = true;
					}
					else
					{
						this.rect.y = this.rect.y + this.rect.height;
					}
				}
				else
				{
					if (this.xPos >= 1)
					{
						this.rect.x = this.rect.x + (float)this.colWidths[this.xPos - 1];
					}
					this.rect.width = (float)this.colWidths[this.xPos];
				}
				this.element.row = this.yPos;
				this.element.column = this.xPos;
				this.element.position = this.rect;
				if (this.element.row >= this.ilvState.state.totalRows)
				{
					this.quiting = true;
				}
				if (this.isLayouted && Event.current.type == EventType.Layout && this.yFrom + 1 == this.yPos)
				{
					this.quiting = true;
				}
				if (this.isLayouted && this.yPos != this.yFrom)
				{
					GUILayout.EndHorizontal();
				}
				if (this.quiting)
				{
					if (this.ilvState.state.drawDropHere && Event.current.GetTypeForControl(this.ilvState.state.ID) == EventType.Repaint)
					{
						GUIStyle gUIStyle = ListViewShared.Constants.insertion;
						gUIStyle.Draw(gUIStyle.margin.Remove(this.ilvState.state.dropHereRect), false, false, false, false);
					}
					if (ListViewShared.ListViewKeyboard(this.ilvState, this.colWidths.Length))
					{
						this.ilvState.state.selectionChanged = true;
					}
					if (Event.current.GetTypeForControl(this.ilvState.state.ID) == EventType.MouseUp)
					{
						GUIUtility.hotControl = 0;
					}
					if (this.ilvState.wantsReordering && GUIUtility.hotControl == this.ilvState.state.ID)
					{
						ListViewState state = this.ilvState.state;
						EventType type = Event.current.type;
						if (type != EventType.DragUpdated)
						{
							if (type != EventType.DragPerform)
							{
								if (type == EventType.DragExited)
								{
									this.ilvState.wantsReordering = false;
									this.ilvState.state.drawDropHere = false;
									GUIUtility.hotControl = 0;
								}
							}
							else
							{
								if (GUIClipHelper.visibleRect.Contains(Event.current.mousePosition))
								{
									this.ilvState.state.draggedFrom = this.ilvState.dragItem;
									this.ilvState.state.draggedTo = Mathf.RoundToInt(Event.current.mousePosition.y / (float)state.rowHeight);
									if (this.ilvState.state.draggedTo > this.ilvState.state.totalRows)
									{
										this.ilvState.state.draggedTo = this.ilvState.state.totalRows;
									}
									if (this.ilvState.state.draggedTo > this.ilvState.state.draggedFrom)
									{
										this.ilvState.state.row = this.ilvState.state.draggedTo - 1;
									}
									else
									{
										this.ilvState.state.row = this.ilvState.state.draggedTo;
									}
									this.ilvState.state.selectionChanged = true;
									DragAndDrop.AcceptDrag();
									Event.current.Use();
									this.ilvState.wantsReordering = false;
									this.ilvState.state.drawDropHere = false;
								}
								GUIUtility.hotControl = 0;
							}
						}
						else
						{
							DragAndDrop.visualMode = ((!this.ilvState.rect.Contains(Event.current.mousePosition)) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move);
							Event.current.Use();
							if (DragAndDrop.visualMode != DragAndDropVisualMode.None)
							{
								state.dropHereRect.y = (float)((Mathf.RoundToInt(Event.current.mousePosition.y / (float)state.rowHeight) - 1) * state.rowHeight);
								if (state.dropHereRect.y >= (float)(state.rowHeight * state.totalRows))
								{
									state.dropHereRect.y = (float)(state.rowHeight * (state.totalRows - 1));
								}
								state.drawDropHere = true;
							}
						}
					}
					else
					{
						if (this.ilvState.wantsExternalFiles)
						{
							EventType type = Event.current.type;
							if (type != EventType.DragUpdated)
							{
								if (type != EventType.DragPerform)
								{
									if (type == EventType.DragExited)
									{
										this.ilvState.wantsExternalFiles = false;
										this.ilvState.state.drawDropHere = false;
										GUIUtility.hotControl = 0;
									}
								}
								else
								{
									if (GUIClipHelper.visibleRect.Contains(Event.current.mousePosition))
									{
										this.ilvState.state.fileNames = DragAndDrop.paths;
										DragAndDrop.AcceptDrag();
										Event.current.Use();
										this.ilvState.wantsExternalFiles = false;
										this.ilvState.state.drawDropHere = false;
										this.ilvState.state.draggedTo = Mathf.RoundToInt(Event.current.mousePosition.y / (float)this.ilvState.state.rowHeight);
										if (this.ilvState.state.draggedTo > this.ilvState.state.totalRows)
										{
											this.ilvState.state.draggedTo = this.ilvState.state.totalRows;
										}
										this.ilvState.state.row = this.ilvState.state.draggedTo;
									}
									GUIUtility.hotControl = 0;
								}
							}
							else
							{
								if (GUIClipHelper.visibleRect.Contains(Event.current.mousePosition) && DragAndDrop.paths != null && DragAndDrop.paths.Length != 0)
								{
									DragAndDrop.visualMode = ((!this.ilvState.rect.Contains(Event.current.mousePosition)) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Copy);
									Event.current.Use();
									if (DragAndDrop.visualMode != DragAndDropVisualMode.None)
									{
										this.ilvState.state.dropHereRect = new Rect(this.ilvState.rect.x, (float)((Mathf.RoundToInt(Event.current.mousePosition.y / (float)this.ilvState.state.rowHeight) - 1) * this.ilvState.state.rowHeight), this.ilvState.rect.width, (float)this.ilvState.state.rowHeight);
										if (this.ilvState.state.dropHereRect.y >= (float)(this.ilvState.state.rowHeight * this.ilvState.state.totalRows))
										{
											this.ilvState.state.dropHereRect.y = (float)(this.ilvState.state.rowHeight * (this.ilvState.state.totalRows - 1));
										}
										this.ilvState.state.drawDropHere = true;
									}
								}
							}
						}
						else
						{
							if (this.ilvState.wantsToAcceptCustomDrag && ListViewShared.dragControlID != this.ilvState.state.ID)
							{
								EventType type = Event.current.type;
								if (type != EventType.DragUpdated)
								{
									if (type != EventType.DragPerform)
									{
										if (type == EventType.DragExited)
										{
											GUIUtility.hotControl = 0;
										}
									}
									else
									{
										object genericData = DragAndDrop.GetGenericData("CustomDragID");
										if (GUIClipHelper.visibleRect.Contains(Event.current.mousePosition) && genericData != null)
										{
											this.ilvState.state.customDraggedFromID = (int)genericData;
											DragAndDrop.AcceptDrag();
											Event.current.Use();
										}
										GUIUtility.hotControl = 0;
									}
								}
								else
								{
									object genericData2 = DragAndDrop.GetGenericData("CustomDragID");
									if (GUIClipHelper.visibleRect.Contains(Event.current.mousePosition) && genericData2 != null)
									{
										DragAndDrop.visualMode = ((!this.ilvState.rect.Contains(Event.current.mousePosition)) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move);
										Event.current.Use();
									}
								}
							}
						}
					}
					if (this.ilvState.beganHorizontal)
					{
						EditorGUILayout.EndScrollView();
						GUILayout.EndHorizontal();
						this.ilvState.beganHorizontal = false;
					}
					if (this.isLayouted)
					{
					}
					this.ilvState.wantsReordering = false;
					this.ilvState.wantsExternalFiles = false;
				}
				else
				{
					if (this.isLayouted)
					{
					}
				}
				if (this.isLayouted)
				{
					if (!this.quiting)
					{
						GUILayout.BeginHorizontal(GUIStyle.none, new GUILayoutOption[0]);
					}
					else
					{
						GUILayout.EndHorizontal();
					}
				}
				return !this.quiting;
			}
			public void Reset()
			{
				this.xPos = -1;
				this.yPos = this.yFrom;
			}
			public IEnumerator GetEnumerator()
			{
				return this;
			}
			public void Dispose()
			{
			}
		}
		public static bool OSX = Application.platform == RuntimePlatform.OSXEditor;
		internal static int dragControlID = -1;
		private static bool DoLVPageUpDown(ListViewShared.InternalListViewState ilvState, ref int selectedRow, ref Vector2 scrollPos, bool up)
		{
			int num = ilvState.endRow - ilvState.invisibleRows;
			if (up)
			{
				if (!ListViewShared.OSX)
				{
					selectedRow -= num;
					if (selectedRow < 0)
					{
						selectedRow = 0;
					}
					return true;
				}
				scrollPos.y -= (float)(ilvState.state.rowHeight * num);
				if (scrollPos.y < 0f)
				{
					scrollPos.y = 0f;
				}
			}
			else
			{
				if (!ListViewShared.OSX)
				{
					selectedRow += num;
					if (selectedRow >= ilvState.state.totalRows)
					{
						selectedRow = ilvState.state.totalRows - 1;
					}
					return true;
				}
				scrollPos.y += (float)(ilvState.state.rowHeight * num);
			}
			return false;
		}
		internal static bool ListViewKeyboard(ListViewShared.InternalListViewState ilvState, int totalCols)
		{
			int totalRows = ilvState.state.totalRows;
			return Event.current.type == EventType.KeyDown && totalRows != 0 && GUIUtility.keyboardControl == ilvState.state.ID && Event.current.GetTypeForControl(ilvState.state.ID) == EventType.KeyDown && ListViewShared.SendKey(ilvState, Event.current.keyCode, totalCols);
		}
		internal static bool SendKey(ListViewShared.InternalListViewState ilvState, KeyCode keyCode, int totalCols)
		{
			ListViewState state = ilvState.state;
			switch (keyCode)
			{
			case KeyCode.UpArrow:
				if (state.row > 0)
				{
					state.row--;
				}
				goto IL_136;
			case KeyCode.DownArrow:
				if (state.row < state.totalRows - 1)
				{
					state.row++;
				}
				goto IL_136;
			case KeyCode.RightArrow:
				if (state.column < totalCols - 1)
				{
					state.column++;
				}
				goto IL_136;
			case KeyCode.LeftArrow:
				if (state.column > 0)
				{
					state.column--;
				}
				goto IL_136;
			case KeyCode.Home:
				state.row = 0;
				goto IL_136;
			case KeyCode.End:
				state.row = state.totalRows - 1;
				goto IL_136;
			case KeyCode.PageUp:
				if (!ListViewShared.DoLVPageUpDown(ilvState, ref state.row, ref state.scrollPos, true))
				{
					Event.current.Use();
					return false;
				}
				goto IL_136;
			case KeyCode.PageDown:
				if (!ListViewShared.DoLVPageUpDown(ilvState, ref state.row, ref state.scrollPos, false))
				{
					Event.current.Use();
					return false;
				}
				goto IL_136;
			}
			return false;
			IL_136:
			state.scrollPos = ListViewShared.ListViewScrollToRow(ilvState, state.scrollPos, state.row);
			Event.current.Use();
			return true;
		}
		internal static bool HasMouseDown(ListViewShared.InternalListViewState ilvState, Rect r)
		{
			return ListViewShared.HasMouseDown(ilvState, r, 0);
		}
		internal static bool HasMouseDown(ListViewShared.InternalListViewState ilvState, Rect r, int button)
		{
			if (Event.current.type == EventType.MouseDown && Event.current.button == button && r.Contains(Event.current.mousePosition))
			{
				GUIUtility.hotControl = ilvState.state.ID;
				GUIUtility.keyboardControl = ilvState.state.ID;
				Event.current.Use();
				return true;
			}
			return false;
		}
		internal static bool HasMouseUp(ListViewShared.InternalListViewState ilvState, Rect r)
		{
			return ListViewShared.HasMouseUp(ilvState, r, 0);
		}
		internal static bool HasMouseUp(ListViewShared.InternalListViewState ilvState, Rect r, int button)
		{
			if (Event.current.type == EventType.MouseUp && Event.current.button == button && r.Contains(Event.current.mousePosition))
			{
				GUIUtility.hotControl = 0;
				Event.current.Use();
				return true;
			}
			return false;
		}
		internal static bool MultiSelection(ListViewShared.InternalListViewState ilvState, int prevSelected, int currSelected, ref int initialSelected, ref bool[] selectedItems)
		{
			bool shift = Event.current.shift;
			bool actionKey = EditorGUI.actionKey;
			bool result = false;
			if ((shift || actionKey) && initialSelected == -1)
			{
				initialSelected = prevSelected;
			}
			if (shift)
			{
				int num = Math.Min(initialSelected, currSelected);
				int num2 = Math.Max(initialSelected, currSelected);
				if (!actionKey)
				{
					for (int i = 0; i < num; i++)
					{
						if (selectedItems[i])
						{
							result = true;
						}
						selectedItems[i] = false;
					}
					for (int j = num2 + 1; j < selectedItems.Length; j++)
					{
						if (selectedItems[j])
						{
							result = true;
						}
						selectedItems[j] = false;
					}
				}
				if (num < 0)
				{
					num = num2;
				}
				for (int k = num; k <= num2; k++)
				{
					if (!selectedItems[k])
					{
						result = true;
					}
					selectedItems[k] = true;
				}
			}
			else
			{
				if (actionKey)
				{
					selectedItems[currSelected] = !selectedItems[currSelected];
					initialSelected = currSelected;
					result = true;
				}
				else
				{
					if (!selectedItems[currSelected])
					{
						result = true;
					}
					for (int l = 0; l < selectedItems.Length; l++)
					{
						if (selectedItems[l] && currSelected != l)
						{
							result = true;
						}
						selectedItems[l] = false;
					}
					initialSelected = -1;
					selectedItems[currSelected] = true;
				}
			}
			if (ilvState != null)
			{
				ilvState.state.scrollPos = ListViewShared.ListViewScrollToRow(ilvState, currSelected);
			}
			return result;
		}
		internal static Vector2 ListViewScrollToRow(ListViewShared.InternalListViewState ilvState, int row)
		{
			return ListViewShared.ListViewScrollToRow(ilvState, ilvState.state.scrollPos, row);
		}
		internal static int ListViewScrollToRow(ListViewShared.InternalListViewState ilvState, int currPosY, int row)
		{
			return (int)ListViewShared.ListViewScrollToRow(ilvState, new Vector2(0f, (float)currPosY), row).y;
		}
		internal static Vector2 ListViewScrollToRow(ListViewShared.InternalListViewState ilvState, Vector2 currPos, int row)
		{
			if (ilvState.invisibleRows < row && ilvState.endRow > row)
			{
				return currPos;
			}
			if (row <= ilvState.invisibleRows)
			{
				currPos.y = (float)(ilvState.state.rowHeight * row);
			}
			else
			{
				currPos.y = (float)(ilvState.state.rowHeight * (row + 1) - ilvState.rectHeight);
			}
			if (currPos.y < 0f)
			{
				currPos.y = 0f;
			}
			else
			{
				if (currPos.y > (float)(ilvState.state.totalRows * ilvState.state.rowHeight - ilvState.rectHeight))
				{
					currPos.y = (float)(ilvState.state.totalRows * ilvState.state.rowHeight - ilvState.rectHeight);
				}
			}
			return currPos;
		}
	}
}
