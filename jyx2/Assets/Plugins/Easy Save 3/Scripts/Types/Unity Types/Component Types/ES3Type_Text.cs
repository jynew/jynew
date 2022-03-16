using System;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("font", "text", "supportRichText", "resizeTextForBestFit", "resizeTextMinSize", "resizeTextMaxSize", "alignment", "alignByGeometry", "fontSize", "horizontalOverflow", "verticalOverflow", "lineSpacing", "fontStyle", "onCullStateChanged", "maskable", "color", "raycastTarget", "material", "useGUILayout", "enabled", "tag", "name", "hideFlags")]
	public class ES3Type_Text : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_Text() : base(typeof(UnityEngine.UI.Text))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.UI.Text)obj;
			
			//writer.WriteProperty("font", instance.font);
			writer.WriteProperty("text", instance.text);
			writer.WriteProperty("supportRichText", instance.supportRichText);
			writer.WriteProperty("resizeTextForBestFit", instance.resizeTextForBestFit);
			writer.WriteProperty("resizeTextMinSize", instance.resizeTextMinSize);
			writer.WriteProperty("resizeTextMaxSize", instance.resizeTextMaxSize);
			writer.WriteProperty("alignment", instance.alignment);
			writer.WriteProperty("alignByGeometry", instance.alignByGeometry);
			writer.WriteProperty("fontSize", instance.fontSize);
			writer.WriteProperty("horizontalOverflow", instance.horizontalOverflow);
			writer.WriteProperty("verticalOverflow", instance.verticalOverflow);
			writer.WriteProperty("lineSpacing", instance.lineSpacing);
			writer.WriteProperty("fontStyle", instance.fontStyle);
			writer.WriteProperty("onCullStateChanged", instance.onCullStateChanged);
			writer.WriteProperty("maskable", instance.maskable);
			writer.WriteProperty("color", instance.color);
			writer.WriteProperty("raycastTarget", instance.raycastTarget);
            // Unity automatically sets the default material if it's set to null.
            // This prevents missing reference warnings.
            if (instance.material.name.Contains("Default"))
                writer.WriteProperty("material", null);
            else
                writer.WriteProperty("material", instance.material);
            writer.WriteProperty("useGUILayout", instance.useGUILayout);
			writer.WriteProperty("enabled", instance.enabled);
			writer.WriteProperty("hideFlags", instance.hideFlags);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.UI.Text)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_FontData":
						reader.SetPrivateField("m_FontData", reader.Read<UnityEngine.UI.FontData>(), instance);
						break;
					case "m_LastTrackedFont":
						reader.SetPrivateField("m_LastTrackedFont", reader.Read<UnityEngine.Font>(), instance);
						break;
					case "m_Text":
						reader.SetPrivateField("m_Text", reader.Read<System.String>(), instance);
						break;
					case "m_TextCache":
						reader.SetPrivateField("m_TextCache", reader.Read<UnityEngine.TextGenerator>(), instance);
						break;
					case "m_TextCacheForLayout":
						reader.SetPrivateField("m_TextCacheForLayout", reader.Read<UnityEngine.TextGenerator>(), instance);
						break;
					case "m_Material":
						reader.SetPrivateField("m_Material", reader.Read<UnityEngine.Material>(), instance);
						break;
					case "font":
						instance.font = reader.Read<UnityEngine.Font>();
						break;
					case "text":
						instance.text = reader.Read<System.String>();
						break;
					case "supportRichText":
						instance.supportRichText = reader.Read<System.Boolean>();
						break;
					case "resizeTextForBestFit":
						instance.resizeTextForBestFit = reader.Read<System.Boolean>();
						break;
					case "resizeTextMinSize":
						instance.resizeTextMinSize = reader.Read<System.Int32>();
						break;
					case "resizeTextMaxSize":
						instance.resizeTextMaxSize = reader.Read<System.Int32>();
						break;
					case "alignment":
						instance.alignment = reader.Read<UnityEngine.TextAnchor>();
						break;
					case "alignByGeometry":
						instance.alignByGeometry = reader.Read<System.Boolean>();
						break;
					case "fontSize":
						instance.fontSize = reader.Read<System.Int32>();
						break;
					case "horizontalOverflow":
						instance.horizontalOverflow = reader.Read<UnityEngine.HorizontalWrapMode>();
						break;
					case "verticalOverflow":
						instance.verticalOverflow = reader.Read<UnityEngine.VerticalWrapMode>();
						break;
					case "lineSpacing":
						instance.lineSpacing = reader.Read<System.Single>();
						break;
					case "fontStyle":
						instance.fontStyle = reader.Read<UnityEngine.FontStyle>();
						break;
					case "onCullStateChanged":
						instance.onCullStateChanged = reader.Read<UnityEngine.UI.MaskableGraphic.CullStateChangedEvent>();
						break;
					case "maskable":
						instance.maskable = reader.Read<System.Boolean>();
						break;
					case "color":
						instance.color = reader.Read<UnityEngine.Color>();
						break;
					case "raycastTarget":
						instance.raycastTarget = reader.Read<System.Boolean>();
						break;
					case "material":
						instance.material = reader.Read<UnityEngine.Material>();
						break;
					case "useGUILayout":
						instance.useGUILayout = reader.Read<System.Boolean>();
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>();
						break;
					case "hideFlags":
						instance.hideFlags = reader.Read<UnityEngine.HideFlags>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}
}