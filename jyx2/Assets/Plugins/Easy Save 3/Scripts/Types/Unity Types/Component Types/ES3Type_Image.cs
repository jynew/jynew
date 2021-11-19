using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("sprite", "overrideSprite", "type", "preserveAspect", "fillCenter", "fillMethod", "fillAmount", "fillClockwise", "fillOrigin", "alphaHitTestMinimumThreshold", "useSpriteMesh", "pixelsPerUnitMultiplier", "material", "onCullStateChanged", "maskable", "color", "raycastTarget", "useLegacyMeshGeneration", "useGUILayout", "enabled", "hideFlags")]
	public class ES3Type_Image : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_Image() : base(typeof(UnityEngine.UI.Image)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.UI.Image)obj;
			
			writer.WritePropertyByRef("sprite", instance.sprite);
			writer.WritePropertyByRef("overrideSprite", instance.overrideSprite);
			writer.WriteProperty("type", instance.type);
			writer.WriteProperty("preserveAspect", instance.preserveAspect, ES3Type_bool.Instance);
			writer.WriteProperty("fillCenter", instance.fillCenter, ES3Type_bool.Instance);
			writer.WriteProperty("fillMethod", instance.fillMethod);
			writer.WriteProperty("fillAmount", instance.fillAmount, ES3Type_float.Instance);
			writer.WriteProperty("fillClockwise", instance.fillClockwise, ES3Type_bool.Instance);
			writer.WriteProperty("fillOrigin", instance.fillOrigin, ES3Type_int.Instance);
			writer.WriteProperty("alphaHitTestMinimumThreshold", instance.alphaHitTestMinimumThreshold, ES3Type_float.Instance);
#if UNITY_2019_1_OR_NEWER
            writer.WriteProperty("useSpriteMesh", instance.useSpriteMesh, ES3Type_bool.Instance);
#endif
            // Unity automatically sets the default material if it's set to null.
            // This prevents missing reference warnings.
            if (instance.material.name.Contains("Default"))
                writer.WriteProperty("material", null);
            else
                writer.WriteProperty("material", instance.material);
            writer.WriteProperty("onCullStateChanged", instance.onCullStateChanged);
			writer.WriteProperty("maskable", instance.maskable, ES3Type_bool.Instance);
			writer.WriteProperty("color", instance.color, ES3Type_Color.Instance);
			writer.WriteProperty("raycastTarget", instance.raycastTarget, ES3Type_bool.Instance);
			writer.WritePrivateProperty("useLegacyMeshGeneration", instance);
			writer.WriteProperty("useGUILayout", instance.useGUILayout, ES3Type_bool.Instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("hideFlags", instance.hideFlags, ES3Type_enum.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.UI.Image)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "sprite":
						instance.sprite = reader.Read<UnityEngine.Sprite>(ES3Type_Sprite.Instance);
						break;
					case "overrideSprite":
						instance.overrideSprite = reader.Read<UnityEngine.Sprite>(ES3Type_Sprite.Instance);
						break;
					case "type":
						instance.type = reader.Read<UnityEngine.UI.Image.Type>();
						break;
					case "preserveAspect":
						instance.preserveAspect = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "fillCenter":
						instance.fillCenter = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "fillMethod":
						instance.fillMethod = reader.Read<UnityEngine.UI.Image.FillMethod>();
						break;
					case "fillAmount":
						instance.fillAmount = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "fillClockwise":
						instance.fillClockwise = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "fillOrigin":
						instance.fillOrigin = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "alphaHitTestMinimumThreshold":
						instance.alphaHitTestMinimumThreshold = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
#if UNITY_2019_1_OR_NEWER
                    case "useSpriteMesh":
						instance.useSpriteMesh = reader.Read<System.Boolean>(ES3Type_bool.Instance);
                        break;
#endif
                    case "material":
						instance.material = reader.Read<UnityEngine.Material>(ES3Type_Material.Instance);
						break;
					case "onCullStateChanged":
						instance.onCullStateChanged = reader.Read<UnityEngine.UI.MaskableGraphic.CullStateChangedEvent>();
						break;
					case "maskable":
						instance.maskable = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "color":
						instance.color = reader.Read<UnityEngine.Color>(ES3Type_Color.Instance);
						break;
					case "raycastTarget":
						instance.raycastTarget = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "useLegacyMeshGeneration":
					reader.SetPrivateProperty("useLegacyMeshGeneration", reader.Read<System.Boolean>(), instance);
					break;
					case "useGUILayout":
						instance.useGUILayout = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "hideFlags":
						instance.hideFlags = reader.Read<UnityEngine.HideFlags>(ES3Type_enum.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3Type_ImageArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_ImageArray() : base(typeof(UnityEngine.UI.Image[]), ES3Type_Image.Instance)
		{
			Instance = this;
		}
	}
}