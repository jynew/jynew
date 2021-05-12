// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
    public class DragAndDropTool
    {
        public delegate void OnValidDropObject(params UnityEngine.Object[] draggedObjs );
        public event OnValidDropObject OnValidDropObjectEvt;

        public void Destroy()
        {
            OnValidDropObjectEvt = null;
        }

        public void TestDragAndDrop( Rect dropArea )
        {
            Event currentEvent = Event.current;
            EventType currentEventType = currentEvent.type;
            
            switch (currentEventType)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                {
                    
                    if (!dropArea.Contains(currentEvent.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        if (OnValidDropObjectEvt != null)
                        {
                            OnValidDropObjectEvt(DragAndDrop.objectReferences);
                        }
                    }
                }break;
                case EventType.DragExited:DragAndDrop.PrepareStartDrag();break;
            }
        }
    }
}
