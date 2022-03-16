using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
    [ES3PropertiesAttribute("enabled", "numTilesX", "numTilesY", "animation", "useRandomRow", "frameOverTime", "frameOverTimeMultiplier", "startFrame", "startFrameMultiplier", "cycleCount", "rowIndex", "uvChannelMask", "flipU", "flipV")]
    public class ES3Type_TextureSheetAnimationModule : ES3Type
    {
        public static ES3Type Instance = null;

        public ES3Type_TextureSheetAnimationModule() : base(typeof(UnityEngine.ParticleSystem.TextureSheetAnimationModule))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            var instance = (UnityEngine.ParticleSystem.TextureSheetAnimationModule)obj;

            writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
            writer.WriteProperty("numTilesX", instance.numTilesX, ES3Type_int.Instance);
            writer.WriteProperty("numTilesY", instance.numTilesY, ES3Type_int.Instance);
            writer.WriteProperty("animation", instance.animation);
#if UNITY_2019_1_OR_NEWER
            writer.WriteProperty("useRandomRow", instance.rowMode);
#else
            writer.WriteProperty("useRandomRow", instance.useRandomRow, ES3Type_bool.Instance);
#endif
            writer.WriteProperty("frameOverTime", instance.frameOverTime, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("frameOverTimeMultiplier", instance.frameOverTimeMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("startFrame", instance.startFrame, ES3Type_MinMaxCurve.Instance);
            writer.WriteProperty("startFrameMultiplier", instance.startFrameMultiplier, ES3Type_float.Instance);
            writer.WriteProperty("cycleCount", instance.cycleCount, ES3Type_int.Instance);
            writer.WriteProperty("rowIndex", instance.rowIndex, ES3Type_int.Instance);
            writer.WriteProperty("uvChannelMask", instance.uvChannelMask);
            //writer.WriteProperty("flipU", instance.flipU, ES3Type_float.Instance);
            //writer.WriteProperty("flipV", instance.flipV, ES3Type_float.Instance);
        }

        public override object Read<T>(ES3Reader reader)
        {
            var instance = new UnityEngine.ParticleSystem.TextureSheetAnimationModule();
            ReadInto<T>(reader, instance);
            return instance;
        }

        public override void ReadInto<T>(ES3Reader reader, object obj)
        {
            var instance = (UnityEngine.ParticleSystem.TextureSheetAnimationModule)obj;
            string propertyName;
            while ((propertyName = reader.ReadPropertyName()) != null)
            {
                switch (propertyName)
                {

                    case "enabled":
                        instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
                        break;
                    case "numTilesX":
                        instance.numTilesX = reader.Read<System.Int32>(ES3Type_int.Instance);
                        break;
                    case "numTilesY":
                        instance.numTilesY = reader.Read<System.Int32>(ES3Type_int.Instance);
                        break;
                    case "animation":
                        instance.animation = reader.Read<UnityEngine.ParticleSystemAnimationType>();
                        break;
#if UNITY_2019_1_OR_NEWER
                    case "rowMode":
                        instance.rowMode = reader.Read<ParticleSystemAnimationRowMode>();
                        break;
#else
                    case "useRandomRow":
                        instance.useRandomRow = reader.Read<System.Boolean>(ES3Type_bool.Instance);
                        break;
#endif
                    case "frameOverTime":
                        instance.frameOverTime = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "frameOverTimeMultiplier":
                        instance.frameOverTimeMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
                        break;
                    case "startFrame":
                        instance.startFrame = reader.Read<UnityEngine.ParticleSystem.MinMaxCurve>(ES3Type_MinMaxCurve.Instance);
                        break;
                    case "startFrameMultiplier":
                        instance.startFrameMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
                        break;
                    case "cycleCount":
                        instance.cycleCount = reader.Read<System.Int32>(ES3Type_int.Instance);
                        break;
                    case "rowIndex":
                        instance.rowIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
                        break;
                    case "uvChannelMask":
                        instance.uvChannelMask = reader.Read<UnityEngine.Rendering.UVChannelFlags>();
                        break;
                    /*case "flipU":
                        instance.flipU = reader.Read<System.Single>(ES3Type_float.Instance);
                        break;
                    case "flipV":
                        instance.flipV = reader.Read<System.Single>(ES3Type_float.Instance);
                        break;*/
                    default:
                        reader.Skip();
                        break;
                }
            }
        }
    }
}
