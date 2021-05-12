using UnityEngine;
using UnityEngine.EventSystems;

namespace Lean.Common.Examples
{
	/// <summary>This component allows you to open a URL using Unity events (e.g. a button).</summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Lean/Common/Lean Link To")]
	public class LeanLinkTo : MonoBehaviour, IPointerClickHandler
	{
		public enum LinkType
		{
			Publisher,
			PreviousScene,
			NextScene
		}

		/// <summary>The URL that will be opened.</summary>
		public LinkType Link { set { link = value; } get { return link; } } [SerializeField] private LinkType link;

		protected virtual void Update()
		{
			switch (link)
			{
				case LinkType.PreviousScene:
				case LinkType.NextScene:
				{
					var group = GetComponent<CanvasGroup>();

					if (group != null)
					{
						var show = GetCurrentLevel() >= 0 && GetLevelCount() > 1;

						group.alpha          = show == true ? 1.0f : 0.0f;
						group.blocksRaycasts = show;
						group.interactable   = show;
					}
				}
				break;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			switch (link)
			{
				case LinkType.Publisher:
				{
					Application.OpenURL("http://carloswilkes.com");
				}
				break;

				case LinkType.PreviousScene:
				{
					var index = GetCurrentLevel();

					if (index >= 0)
					{
						if (--index < 0)
						{
							index = GetLevelCount() - 1;
						}

						LoadLevel(index);
					}
				}
				break;

				case LinkType.NextScene:
				{
					var index = GetCurrentLevel();

					if (index >= 0)
					{
						if (++index >= GetLevelCount())
						{
							index = 0;
						}

						LoadLevel(index);
					}
				}
				break;
			}
		}

		private static int GetCurrentLevel()
		{
			var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			var index = scene.buildIndex;

			if (index >= 0)
			{
				if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(index).path != scene.path)
				{
					return -1;
				}
			}

			return index;
		}

		private static int GetLevelCount()
		{
			return UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
		}

		private static void LoadLevel(int index)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(index);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Examples
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanLinkTo))]
	public class LeanLinkTo_Editor : LeanInspector<LeanLinkTo>
	{
		protected override void DrawInspector()
		{
			base.DrawInspector();
		}
	}
}
#endif