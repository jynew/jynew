using System;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("sprite", "color", "flipX", "flipY", "enabled", "shadowCastingMode", "receiveShadows", "sharedMaterials", "lightmapIndex", "realtimeLightmapIndex", "lightmapScaleOffset", "motionVectorGenerationMode", "realtimeLightmapScaleOffset", "lightProbeUsage", "lightProbeProxyVolumeOverride", "probeAnchor", "reflectionProbeUsage", "sortingLayerName", "sortingLayerID", "sortingOrder")]
	public class ES3Type_SpriteRenderer : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_SpriteRenderer() : base(typeof(UnityEngine.SpriteRenderer))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.SpriteRenderer)obj;

			writer.WriteProperty("sprite", instance.sprite);
			writer.WriteProperty("color", instance.color, ES3Type_Color.Instance);
			writer.WriteProperty("flipX", instance.flipX, ES3Type_bool.Instance);
			writer.WriteProperty("flipY", instance.flipY, ES3Type_bool.Instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("shadowCastingMode", instance.shadowCastingMode);
			writer.WriteProperty("receiveShadows", instance.receiveShadows, ES3Type_bool.Instance);
			writer.WriteProperty("sharedMaterials", instance.sharedMaterials);
			writer.WriteProperty("lightmapIndex", instance.lightmapIndex, ES3Type_int.Instance);
			writer.WriteProperty("realtimeLightmapIndex", instance.realtimeLightmapIndex, ES3Type_int.Instance);
			writer.WriteProperty("lightmapScaleOffset", instance.lightmapScaleOffset, ES3Type_Vector4.Instance);
			writer.WriteProperty("motionVectorGenerationMode", instance.motionVectorGenerationMode);
			writer.WriteProperty("realtimeLightmapScaleOffset", instance.realtimeLightmapScaleOffset, ES3Type_Vector4.Instance);
			writer.WriteProperty("lightProbeUsage", instance.lightProbeUsage);
			writer.WriteProperty("lightProbeProxyVolumeOverride", instance.lightProbeProxyVolumeOverride, ES3Type_GameObject.Instance);
			writer.WriteProperty("probeAnchor", instance.probeAnchor, ES3Type_Transform.Instance);
			writer.WriteProperty("reflectionProbeUsage", instance.reflectionProbeUsage);
			writer.WriteProperty("sortingLayerName", instance.sortingLayerName, ES3Type_string.Instance);
			writer.WriteProperty("sortingLayerID", instance.sortingLayerID, ES3Type_int.Instance);
			writer.WriteProperty("sortingOrder", instance.sortingOrder, ES3Type_int.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.SpriteRenderer)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					case "sprite":
						instance.sprite = reader.Read<UnityEngine.Sprite>(ES3Type_Sprite.Instance);
						break;
					case "color":
						instance.color = reader.Read<UnityEngine.Color>(ES3Type_Color.Instance);
						break;
					case "flipX":
						instance.flipX = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "flipY":
						instance.flipY = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "shadowCastingMode":
						instance.shadowCastingMode = reader.Read<UnityEngine.Rendering.ShadowCastingMode>();
						break;
					case "receiveShadows":
						instance.receiveShadows = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "sharedMaterials":
						instance.sharedMaterials = reader.Read<UnityEngine.Material[]>();
						break;
					case "lightmapIndex":
						instance.lightmapIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "realtimeLightmapIndex":
						instance.realtimeLightmapIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "lightmapScaleOffset":
						instance.lightmapScaleOffset = reader.Read<UnityEngine.Vector4>(ES3Type_Vector4.Instance);
						break;
					case "motionVectorGenerationMode":
						instance.motionVectorGenerationMode = reader.Read<UnityEngine.MotionVectorGenerationMode>();
						break;
					case "realtimeLightmapScaleOffset":
						instance.realtimeLightmapScaleOffset = reader.Read<UnityEngine.Vector4>(ES3Type_Vector4.Instance);
						break;
					case "lightProbeUsage":
						instance.lightProbeUsage = reader.Read<UnityEngine.Rendering.LightProbeUsage>();
						break;
					case "lightProbeProxyVolumeOverride":
						instance.lightProbeProxyVolumeOverride = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "probeAnchor":
						instance.probeAnchor = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "reflectionProbeUsage":
						instance.reflectionProbeUsage = reader.Read<UnityEngine.Rendering.ReflectionProbeUsage>();
						break;
					case "sortingLayerName":
						instance.sortingLayerName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "sortingLayerID":
						instance.sortingLayerID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "sortingOrder":
						instance.sortingOrder = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

		public class ES3Type_SpriteRendererArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_SpriteRendererArray() : base(typeof(UnityEngine.SpriteRenderer[]), ES3Type_SpriteRenderer.Instance)
		{
			Instance = this;
		}
	}
}