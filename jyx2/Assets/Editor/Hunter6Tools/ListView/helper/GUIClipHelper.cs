using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class GUIClipHelper
{
    private static Func<Rect> VisibleRect;

    public static void InitType()
    {
        if (VisibleRect != null)
        {
            return;
        }
        var tyGUIClip = Type.GetType("UnityEngine.GUIClip,UnityEngine");
        if (tyGUIClip != null)
        {
            var piVisibleRect = tyGUIClip.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (piVisibleRect != null)
            {
                var getMethod = piVisibleRect.GetGetMethod(true) ?? piVisibleRect.GetGetMethod(false);
                VisibleRect = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), getMethod);
            }
        }
    }

    public static Rect visibleRect
    {
        get
        {
            InitType();
            return VisibleRect();
        }
    }
}