using System;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("type", "color", "intensity", "bounceIntensity", "shadows", "shadowStrength", "shadowResolution", "shadowCustomResolution", "shadowBias", "shadowNormalBias", "shadowNearPlane", "range", "spotAngle", "cookieSize", "cookie", "flare", "renderMode", "cullingMask", "areaSize", "lightmappingMode", "enabled", "hideFlags")]
	public class ES3Type_Light : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_Light() : base(typeof(UnityEngine.Light))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Light)obj;
			
			writer.WriteProperty("type", instance.type);
			writer.WriteProperty("color", instance.color, ES3Type_Color.Instance);
			writer.WriteProperty("intensity", instance.intensity, ES3Type_float.Instance);
			writer.WriteProperty("bounceIntensity", instance.bounceIntensity, ES3Type_float.Instance);
			writer.WriteProperty("shadows", instance.shadows);
			writer.WriteProperty("shadowStrength", instance.shadowStrength, ES3Type_float.Instance);
			writer.WriteProperty("shadowResolution", instance.shadowResolution);
			writer.WriteProperty("shadowCustomResolution", instance.shadowCustomResolution, ES3Type_int.Instance);
			writer.WriteProperty("shadowBias", instance.shadowBias, ES3Type_float.Instance);
			writer.WriteProperty("shadowNormalBias", instance.shadowNormalBias, ES3Type_float.Instance);
			writer.WriteProperty("shadowNearPlane", instance.shadowNearPlane, ES3Type_float.Instance);
			writer.WriteProperty("range", instance.range, ES3Type_float.Instance);
			writer.WriteProperty("spotAngle", instance.spotAngle, ES3Type_float.Instance);
			writer.WriteProperty("cookieSize", instance.cookieSize, ES3Type_float.Instance);
			writer.WriteProperty("cookie", instance.cookie, ES3Type_Texture2D.Instance);
			writer.WriteProperty("flare", instance.flare, ES3Type_Texture2D.Instance);
			writer.WriteProperty("renderMode", instance.renderMode);
			writer.WriteProperty("cullingMask", instance.cullingMask, ES3Type_int.Instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("hideFlags", instance.hideFlags);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.Light)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "type":
						instance.type = reader.Read<UnityEngine.LightType>();
						break;
					case "color":
						instance.color = reader.Read<UnityEngine.Color>(ES3Type_Color.Instance);
						break;
					case "intensity":
						instance.intensity = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "bounceIntensity":
						instance.bounceIntensity = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "shadows":
						instance.shadows = reader.Read<UnityEngine.LightShadows>();
						break;
					case "shadowStrength":
						instance.shadowStrength = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "shadowResolution":
						instance.shadowResolution = reader.Read<UnityEngine.Rendering.LightShadowResolution>();
						break;
					case "shadowCustomResolution":
						instance.shadowCustomResolution = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "shadowBias":
						instance.shadowBias = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "shadowNormalBias":
						instance.shadowNormalBias = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "shadowNearPlane":
						instance.shadowNearPlane = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "range":
						instance.range = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "spotAngle":
						instance.spotAngle = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "cookieSize":
						instance.cookieSize = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "cookie":
						instance.cookie = reader.Read<UnityEngine.Texture>();
						break;
					case "flare":
						instance.flare = reader.Read<UnityEngine.Flare>();
						break;
					case "renderMode":
						instance.renderMode = reader.Read<UnityEngine.LightRenderMode>();
						break;
					case "cullingMask":
						instance.cullingMask = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
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