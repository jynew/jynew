using UnityEngine;
using System.Collections;

namespace ProGrids
{
	/**
	 * A substitute for GUIContent that offers some additional functionality.
	 */
	[System.Serializable]
	public class pg_ToggleContent
	{
		public string text_on, text_off;
		public Texture2D image_on, image_off;
		public string tooltip;

		GUIContent gc = new GUIContent();

		public pg_ToggleContent(string t_on, string t_off, string tooltip)
		{
			this.text_on = t_on;
			this.text_off = t_off;
			this.image_on = (Texture2D)null;
			this.image_off = (Texture2D)null;
			this.tooltip = tooltip;

			gc.tooltip = tooltip;
		}

		public pg_ToggleContent(string t_on, string t_off, Texture2D i_on, Texture2D i_off, string tooltip)
		{
			this.text_on = t_on;
			this.text_off = t_off;
			this.image_on = i_on;
			this.image_off = i_off;
			this.tooltip = tooltip;

			gc.tooltip = tooltip;
		}

		public static bool ToggleButton(Rect r, pg_ToggleContent content, bool enabled, GUIStyle imageStyle, GUIStyle altStyle)
		{
			content.gc.image = enabled ? content.image_on : content.image_off;
			content.gc.text = content.gc.image == null ? (enabled ? content.text_on : content.text_off) : "";

			return GUI.Button(r, content.gc, content.gc.image != null ? imageStyle : altStyle);
		}
	}
}
