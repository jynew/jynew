namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Test Canvas mode.
	/// </summary>
	public class TestCanvasMode : MonoBehaviour
	{
		/// <summary>
		/// Canvas.
		/// </summary>
		[SerializeField]
		public Canvas Canvas;

		/// <summary>
		/// Set ScreenSpaceOverlay.
		/// </summary>
		public void SetOverlay()
		{
			Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		}

		/// <summary>
		/// Set ScreenSpaceCamera.
		/// </summary>
		public void SetCamera()
		{
			Canvas.renderMode = RenderMode.ScreenSpaceCamera;
		}

		/// <summary>
		/// Set WorldSpace.
		/// </summary>
		public void SetWorldSpace()
		{
			Canvas.renderMode = RenderMode.WorldSpace;

			var rt = Canvas.transform as RectTransform;
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1280f);
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 720f);
			rt.localScale = new Vector3(0.16f, 0.16f, 0.16f);
			rt.localRotation = Quaternion.Euler(45f, -5f, -5f);
			rt.localPosition = new Vector3(10f, -70f, 60f);
		}
	}
}