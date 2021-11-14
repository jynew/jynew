using HSFrameWork.Common;
using Jyx2.Setup;
using System.Collections;
using System.Collections.Generic;
using Jyx2.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityToolbarExtender.Examples
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;
		public static readonly GUIStyle normalButtonStyle;

		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold,
				
			};
		}
	}

	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		static SceneSwitchLeftButton()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();

			if(GUILayout.Button(new GUIContent("P", "Start Play"), ToolbarStyles.commandButtonStyle))
			{
#if UNITY_EDITOR
				GPDC.OnProjectLoadedInEditor();
                //Container.TryResolve<IXLsReloader>()?.Do(); //自动重载配置表
#endif
                SceneHelper.StartScene("Assets/Jyx2Scenes/0_GameStart.unity");
            }
        }
	}
}
