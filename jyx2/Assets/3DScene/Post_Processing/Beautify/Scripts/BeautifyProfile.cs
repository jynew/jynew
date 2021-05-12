/// <summary>
/// Copyright 2016-2018 Ramiro Oliva (Kronnect) - All rights reserved
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BeautifyEffect
{

    [CreateAssetMenu(fileName = "BeautifyProfile", menuName = "Beautify Profile", order = 101)]
    public class BeautifyProfile : ScriptableObject
    {

        #region RGB Dither
        [Range(0, 0.2f)] public float dither = 0.02f;
        [Range(0, 1f)] public float ditherDepth = 0f;
        #endregion

        #region Sharpen Settings

        [Range(0, 1f)] public float sharpenMinDepth = 0f;
        [Range(0, 1.1f)] public float sharpenMaxDepth = 0.999f;
        [Range(0f, 15f)] public float sharpen = 2f;
        [Range(0f, 0.05f)] public float sharpenDepthThreshold = 0.035f;
        public Color tintColor = new Color(1, 1, 1, 0);
        [Range(0f, 0.2f)] public float sharpenRelaxation = 0.08f;
        [Range(0, 1f)] public float sharpenClamp = 0.45f;
        [Range(0, 1f)] public float sharpenMotionSensibility = 0.5f;
        #endregion

        #region Color grading
        [Range(-2f, 3f)] public float saturate = 1f;
        [Range(0.5f, 1.5f)] public float contrast = 1.02f;
        [Range(0f, 2f)] public float brightness = 1.05f;
        [Range(0f, 2f)] public float daltonize = 0f;
        #endregion

        #region Vignetting
        public bool vignetting = false;
        public Color vignettingColor = new Color(0.3f, 0.3f, 0.3f, 0.05f);
        public float vignettingFade = 0;
        public bool vignettingCircularShape = false;
        public float vignettingAspectRatio = 1f;
        [Range(0, 1f)] public float vignettingBlink = 0f;
        public Texture2D vignettingMask;
        #endregion

        #region Frame
        public bool frame = false;
        public Color frameColor = new Color(1, 1, 1, 0.047f);
        public Texture2D frameMask;
        #endregion

        #region LUT
        public bool lut = false;
        [Range(0f, 1f)] public float lutIntensity = 1f;
        public Texture2D lutTexture;
        #endregion

        #region Night Vision
        public bool nightVision = false;
        public Color nightVisionColor = new Color(0.5f, 1f, 0.5f, 0.5f);
        #endregion

        #region Outline
        public bool outline = false;
        public Color outlineColor = new Color(0, 0, 0, 0.8f);
        #endregion

        #region Thermal Vision
        public bool thermalVision = false;
        #endregion

        #region Lens Dirt
        public bool lensDirt = false;
        [Range(0f, 1f)] public float lensDirtThreshold = 0.5f;
        [Range(0f, 1f)] public float lensDirtIntensity = 0.9f;
        public Texture2D lensDirtTexture;
        #endregion

        #region Bloom
        public bool bloom = false;
        public LayerMask bloomCullingMask = 0;
        [Range(1f, 4f)] public float bloomLayerMaskDownsampling = 1f;
        [Range(0, 10f)] public float bloomIntensity = 1f;
        public float bloomMaxBrightness = 1000f;
        [Range(0f, 3f)] public float bloomBoost0 = 0f;
        [Range(0f, 3f)] public float bloomBoost1 = 0f;
        [Range(0f, 3f)] public float bloomBoost2 = 0f;
        [Range(0f, 3f)] public float bloomBoost3 = 0f;
        [Range(0f, 3f)] public float bloomBoost4 = 0f;
        [Range(0f, 3f)] public float bloomBoost5 = 0f;
        public bool bloomAntiflicker = false;
        public bool bloomUltra = false;
        [Range(0f, 5f)] public float bloomThreshold = 0.75f;
        public bool bloomCustomize = false;
        [Range(0f, 1f)] public float bloomWeight0 = 0.5f;
        [Range(0f, 1f)] public float bloomWeight1 = 0.5f;
        [Range(0f, 1f)] public float bloomWeight2 = 0.5f;
        [Range(0f, 1f)] public float bloomWeight3 = 0.5f;
        [Range(0f, 1f)] public float bloomWeight4 = 0.5f;
        [Range(0f, 1f)] public float bloomWeight5 = 0.5f;
        public bool bloomBlur = true;
        [Range(0f, 1f)] public float bloomDepthAtten = 0;
        [Range(-1f, 1f)] public float bloomLayerZBias = 0;
        #endregion

        #region Anamorphic Flares
        public bool anamorphicFlares = false;
        [Range(0f, 10f)] public float anamorphicFlaresIntensity = 1f;
        public bool anamorphicFlaresAntiflicker = false;
        public bool anamorphicFlaresUltra = false;
        [Range(0f, 5f)] public float anamorphicFlaresThreshold = 0.75f;
        [Range(0.1f, 2f)] public float anamorphicFlaresSpread = 1f;
        public bool anamorphicFlaresVertical = false;
        public Color anamorphicFlaresTint = new Color(0.5f, 0.5f, 1f, 0f);
        public bool anamorphicFlaresBlur = true;
        #endregion

        #region Depth of Field
        public bool depthOfField = false;
        public bool depthOfFieldTransparencySupport = false;
        public Transform depthOfFieldTargetFocus;
        public bool depthOfFieldAutofocus = false;
        public LayerMask depthOfFieldAutofocusLayerMask = -1;
        public float depthOfFieldAutofocusMinDistance = 0;
        public float depthOfFieldAutofocusMaxDistance = 10000;
        public LayerMask depthOfFieldExclusionLayerMask = 0;
        [Range(1, 4)] public float depthOfFieldExclusionLayerMaskDownsampling = 1f;
        [Range(1, 4)] public float depthOfFieldTransparencySupportDownsampling = 1f;
        [Range(0.9f, 1f)] public float depthOfFieldExclusionBias = 0.99f;
        [Range(1f, 100f)] public float depthOfFieldDistance = 1f;
        [Range(0.001f, 1f)] public float depthOfFieldFocusSpeed = 1f;
        [Range(1, 5)] public int depthOfFieldDownsampling = 2;
        [Range(2, 16)] public int depthOfFieldMaxSamples = 4;
        [Range(0.005f, 0.5f)] public float depthOfFieldFocalLength = 0.050f;
        public float depthOfFieldAperture = 2.8f;
        public bool depthOfFieldForegroundBlur = true;
        public bool depthOfFieldForegroundBlurHQ = false;
        public float depthOfFieldForegroundDistance = 0.25f;
        public bool depthOfFieldBokeh = true;
        [Range(0.5f, 3f)] public float depthOfFieldBokehThreshold = 1f;
        [Range(0f, 8f)] public float depthOfFieldBokehIntensity = 2f;
        public float depthOfFieldMaxBrightness = 1000f;
        public FilterMode depthOfFieldFilterMode = FilterMode.Bilinear;
        #endregion

        #region Eye Adaptation
        public bool eyeAdaptation = false;
        [Range(0f, 1f)] public float eyeAdaptationMinExposure = 0.2f;
        [Range(1f, 100f)] public float eyeAdaptationMaxExposure = 5f;
        [Range(0f, 1f)] public float eyeAdaptationSpeedToLight = 1f;
        [Range(0f, 1f)] public float eyeAdaptationSpeedToDark = 0.7f;
        #endregion

        #region Purkinje effect
        public bool purkinje = false;
        [Range(0f, 5f)] public float purkinjeAmount = 1f;
        [Range(0f, 1f)] public float purkinjeLuminanceThreshold = 0.15f;
        #endregion

        #region Tonemapping
        public BEAUTIFY_TMO tonemap = BEAUTIFY_TMO.Linear;
        #endregion


        #region Sun Flares
        public bool sunFlares = false;
        [Range(0f, 1f)] public float sunFlaresIntensity = 1.0f;
        [Range(0f, 1f)] public float sunFlaresSolarWindSpeed = 0.01f;
        public Color sunFlaresTint = new Color(1, 1, 1);
        [Range(1, 5)] public int sunFlaresDownsampling = 1;
        [Range(0f, 1f)] public float sunFlaresSunIntensity = 0.1f;
        [Range(0f, 1f)] public float sunFlaresSunDiskSize = 0.05f;
        [Range(0f, 10f)] public float sunFlaresSunRayDiffractionIntensity = 3.5f;
        [Range(0f, 1f)] public float sunFlaresSunRayDiffractionThreshold = 0.13f;
        [Range(0f, 0.2f)] public float sunFlaresCoronaRays1Length = 0.02f;
        [Range(2, 30)] public int sunFlaresCoronaRays1Streaks = 12;
        [Range(0f, 0.1f)] public float sunFlaresCoronaRays1Spread = 0.001f;
        [Range(0f, 2f * Mathf.PI)] public float sunFlaresCoronaRays1AngleOffset = 0f;
        [Range(0f, 0.2f)] public float sunFlaresCoronaRays2Length = 0.05f;
        [Range(2, 30)] public int sunFlaresCoronaRays2Streaks = 12;
        [Range(0f, 0.1f)] public float sunFlaresCoronaRays2Spread = 0.1f;
        [Range(0f, 2f * Mathf.PI)] public float sunFlaresCoronaRays2AngleOffset = 0f;
        [Range(0f, 1f)] public float sunFlaresGhosts1Size = 0.03f;
        [Range(-3f, 3f)] public float sunFlaresGhosts1Offset = 1.04f;
        [Range(0f, 1f)] public float sunFlaresGhosts1Brightness = 0.037f;
        [Range(0f, 1f)] public float sunFlaresGhosts2Size = 0.1f;
        [Range(-3f, 3f)] public float sunFlaresGhosts2Offset = 0.71f;
        [Range(0f, 1f)] public float sunFlaresGhosts2Brightness = 0.03f;
        [Range(0f, 1f)] public float sunFlaresGhosts3Size = 0.24f;
        [Range(-3f, 3f)] public float sunFlaresGhosts3Brightness = 0.025f;
        [Range(0f, 1f)] public float sunFlaresGhosts3Offset = 0.31f;
        [Range(0f, 1f)] public float sunFlaresGhosts4Size = 0.016f;
        [Range(-3f, 3f)] public float sunFlaresGhosts4Offset = 0f;
        [Range(0f, 1f)] public float sunFlaresGhosts4Brightness = 0.017f;
        [Range(0f, 1f)] public float sunFlaresHaloOffset = 0.22f;
        [Range(0f, 50f)] public float sunFlaresHaloAmplitude = 15.1415f;
        [Range(0f, 1f)] public float sunFlaresHaloIntensity = 0.01f;
        public bool sunFlaresRotationDeadZone = false;
        #endregion

        #region Blur
        public bool blur = false;
        [Range(0f, 4f)] public float blurIntensity = 1f;
        #endregion

        #region Pixelate
        public int pixelateAmount = 1;
        public bool pixelateDownscale = false;
        #endregion

        /// <summary>
        /// Applies profile settings
        /// </summary>
        public void Load(Beautify b)
        {
            // dither
            b.dither = dither;
            b.ditherDepth = ditherDepth;
            // sharpen
            b.sharpenMinDepth = sharpenMinDepth;
            b.sharpenMaxDepth = sharpenMaxDepth;
            b.sharpen = sharpen;
            b.sharpenDepthThreshold = sharpenDepthThreshold;
            b.tintColor = tintColor;
            b.sharpenRelaxation = sharpenRelaxation;
            b.sharpenClamp = sharpenClamp;
            b.sharpenMotionSensibility = sharpenMotionSensibility;
            // color grading
            b.saturate = saturate;
            b.contrast = contrast;
            b.brightness = brightness;
            b.daltonize = daltonize;
            // vignetting
            b.vignetting = vignetting;
            b.vignettingColor = vignettingColor;
            b.vignettingFade = vignettingFade;
            b.vignettingCircularShape = vignettingCircularShape;
            b.vignettingAspectRatio = vignettingAspectRatio;
            b.vignettingBlink = vignettingBlink;
            b.vignettingMask = vignettingMask;
            // frame
            b.frame = frame;
            b.frameColor = frameColor;
            b.frameMask = frameMask;
            // lut
            b.lut = lut;
            b.lutTexture = lutTexture;
            b.lutIntensity = lutIntensity;
            // night vision
            b.nightVision = nightVision;
            b.nightVisionColor = nightVisionColor;
            // outline
            b.outline = outline;
            b.outlineColor = outlineColor;
            // thermal vision
            b.thermalVision = thermalVision;
            // lens dirt
            b.lensDirt = lensDirt;
            b.lensDirtThreshold = lensDirtThreshold;
            b.lensDirtIntensity = lensDirtIntensity;
            b.lensDirtTexture = lensDirtTexture;
            // bloom
            b.bloom = bloom;
            b.bloomCullingMask = bloomCullingMask;
            b.bloomLayerMaskDownsampling = bloomLayerMaskDownsampling;
            b.bloomIntensity = bloomIntensity;
            b.bloomMaxBrightness = bloomMaxBrightness;
            b.bloomBoost0 = bloomBoost0;
            b.bloomBoost1 = bloomBoost1;
            b.bloomBoost2 = bloomBoost2;
            b.bloomBoost3 = bloomBoost3;
            b.bloomBoost4 = bloomBoost4;
            b.bloomBoost5 = bloomBoost5;
            b.bloomAntiflicker = bloomAntiflicker;
            b.bloomUltra = bloomUltra;
            b.bloomThreshold = bloomThreshold;
            b.bloomCustomize = bloomCustomize;
            b.bloomWeight0 = bloomWeight0;
            b.bloomWeight1 = bloomWeight1;
            b.bloomWeight2 = bloomWeight2;
            b.bloomWeight3 = bloomWeight3;
            b.bloomWeight4 = bloomWeight4;
            b.bloomWeight5 = bloomWeight5;
            b.bloomBlur = bloomBlur;
            b.bloomDepthAtten = bloomDepthAtten;
            b.bloomLayerZBias = bloomLayerZBias;
            // anamorphic flares
            b.anamorphicFlares = anamorphicFlares;
            b.anamorphicFlaresIntensity = anamorphicFlaresIntensity;
            b.anamorphicFlaresAntiflicker = anamorphicFlaresAntiflicker;
            b.anamorphicFlaresUltra = anamorphicFlaresUltra;
            b.anamorphicFlaresThreshold = anamorphicFlaresThreshold;
            b.anamorphicFlaresSpread = anamorphicFlaresSpread;
            b.anamorphicFlaresVertical = anamorphicFlaresVertical;
            b.anamorphicFlaresTint = anamorphicFlaresTint;
            b.anamorphicFlaresBlur = anamorphicFlaresBlur;
            // dof
            b.depthOfField = depthOfField;
            b.depthOfFieldTransparencySupport = depthOfFieldTransparencySupport;
            b.depthOfFieldTargetFocus = depthOfFieldTargetFocus;
            b.depthOfFieldAutofocus = depthOfFieldAutofocus;
            b.depthOfFieldAutofocusLayerMask = depthOfFieldAutofocusLayerMask;
            b.depthOfFieldAutofocusMinDistance = depthOfFieldAutofocusMinDistance;
            b.depthOfFieldAutofocusMaxDistance = depthOfFieldAutofocusMaxDistance;
            b.depthOfFieldExclusionLayerMask = depthOfFieldExclusionLayerMask;
            b.depthOfFieldExclusionLayerMaskDownsampling = depthOfFieldExclusionLayerMaskDownsampling;
            b.depthOfFieldTransparencySupportDownsampling = depthOfFieldTransparencySupportDownsampling;
            b.depthOfFieldExclusionBias = depthOfFieldExclusionBias;
            b.depthOfFieldDistance = depthOfFieldDistance;
            b.depthOfFieldFocusSpeed = depthOfFieldFocusSpeed;
            b.depthOfFieldDownsampling = depthOfFieldDownsampling;
            b.depthOfFieldMaxSamples = depthOfFieldMaxSamples;
            b.depthOfFieldFocalLength = depthOfFieldFocalLength;
            b.depthOfFieldAperture = depthOfFieldAperture;
            b.depthOfFieldForegroundBlur = depthOfFieldForegroundBlur;
            b.depthOfFieldForegroundBlurHQ = depthOfFieldForegroundBlurHQ;
            b.depthOfFieldForegroundDistance = depthOfFieldForegroundDistance;
            b.depthOfFieldBokeh = depthOfFieldBokeh;
            b.depthOfFieldBokehThreshold = depthOfFieldBokehThreshold;
            b.depthOfFieldBokehIntensity = depthOfFieldBokehIntensity;
            b.depthOfFieldMaxBrightness = depthOfFieldMaxBrightness;
            b.depthOfFieldFilterMode = depthOfFieldFilterMode;
            // ea
            b.eyeAdaptation = eyeAdaptation;
            b.eyeAdaptationMaxExposure = eyeAdaptationMaxExposure;
            b.eyeAdaptationMinExposure = eyeAdaptationMinExposure;
            b.eyeAdaptationSpeedToDark = eyeAdaptationSpeedToDark;
            b.eyeAdaptationSpeedToLight = eyeAdaptationSpeedToLight;
            // purkinje
            b.purkinje = purkinje;
            b.purkinjeAmount = purkinjeAmount;
            b.purkinjeLuminanceThreshold = purkinjeLuminanceThreshold;
            // tonemap
            b.tonemap = tonemap;
            // flares
            b.sunFlares = sunFlares;
            b.sunFlaresIntensity = sunFlaresIntensity;
            b.sunFlaresSolarWindSpeed = sunFlaresSolarWindSpeed;
            b.sunFlaresTint = sunFlaresTint;
            b.sunFlaresDownsampling = sunFlaresDownsampling;
            b.sunFlaresSunIntensity = sunFlaresSunIntensity;
            b.sunFlaresSunDiskSize = sunFlaresSunDiskSize;
            b.sunFlaresSunRayDiffractionIntensity = sunFlaresSunRayDiffractionIntensity;
            b.sunFlaresSunRayDiffractionThreshold = sunFlaresSunRayDiffractionThreshold;
            b.sunFlaresCoronaRays1Length = sunFlaresCoronaRays1Length;
            b.sunFlaresCoronaRays1Spread = sunFlaresCoronaRays1Spread;
            b.sunFlaresCoronaRays1AngleOffset = sunFlaresCoronaRays1AngleOffset;
            b.sunFlaresCoronaRays1Streaks = sunFlaresCoronaRays1Streaks;
            b.sunFlaresCoronaRays2Length = sunFlaresCoronaRays2Length;
            b.sunFlaresCoronaRays2Spread = sunFlaresCoronaRays2Spread;
            b.sunFlaresCoronaRays2AngleOffset = sunFlaresCoronaRays2AngleOffset;
            b.sunFlaresCoronaRays2Streaks = sunFlaresCoronaRays2Streaks;
            b.sunFlaresGhosts1Size = sunFlaresGhosts1Size;
            b.sunFlaresGhosts1Offset = sunFlaresGhosts1Offset;
            b.sunFlaresGhosts1Brightness = sunFlaresGhosts1Brightness;
            b.sunFlaresGhosts2Size = sunFlaresGhosts2Size;
            b.sunFlaresGhosts2Offset = sunFlaresGhosts2Offset;
            b.sunFlaresGhosts2Brightness = sunFlaresGhosts2Brightness;
            b.sunFlaresGhosts3Size = sunFlaresGhosts3Size;
            b.sunFlaresGhosts3Offset = sunFlaresGhosts3Offset;
            b.sunFlaresGhosts3Brightness = sunFlaresGhosts3Brightness;
            b.sunFlaresGhosts4Size = sunFlaresGhosts4Size;
            b.sunFlaresGhosts4Offset = sunFlaresGhosts4Offset;
            b.sunFlaresGhosts4Brightness = sunFlaresGhosts4Brightness;
            b.sunFlaresHaloOffset = sunFlaresHaloOffset;
            b.sunFlaresHaloAmplitude = sunFlaresHaloAmplitude;
            b.sunFlaresHaloIntensity = sunFlaresHaloIntensity;
            b.sunFlaresRotationDeadZone = sunFlaresRotationDeadZone;
            // blur
            b.blur = blur;
            b.blurIntensity = blurIntensity;
            // pixelate
            b.pixelateAmount = pixelateAmount;
            b.pixelateDownscale = pixelateDownscale;
        }

        /// <summary>
        /// Replaces profile settings with current Beautify configuration
        /// </summary>
        public void Save(Beautify b)
        {
            // dither
            dither = b.dither;
            ditherDepth = b.ditherDepth;
            // sharpen
            sharpenMinDepth = b.sharpenMinDepth;
            sharpenMaxDepth = b.sharpenMaxDepth;
            sharpen = b.sharpen;
            sharpenDepthThreshold = b.sharpenDepthThreshold;
            tintColor = b.tintColor;
            sharpenRelaxation = b.sharpenRelaxation;
            sharpenClamp = b.sharpenClamp;
            sharpenMotionSensibility = b.sharpenMotionSensibility;
            // color grading
            saturate = b.saturate;
            contrast = b.contrast;
            brightness = b.brightness;
            daltonize = b.daltonize;
            // vignetting
            vignetting = b.vignetting;
            vignettingColor = b.vignettingColor;
            vignettingFade = b.vignettingFade;
            vignettingCircularShape = b.vignettingCircularShape;
            vignettingMask = b.vignettingMask;
            vignettingAspectRatio = b.vignettingAspectRatio;
            vignettingBlink = b.vignettingBlink;
            // frame
            frame = b.frame;
            frameColor = b.frameColor;
            frameMask = b.frameMask;
            // lut
            lut = b.lut;
            lutTexture = b.lutTexture;
            lutIntensity = b.lutIntensity;
            // night vision
            nightVision = b.nightVision;
            nightVisionColor = b.nightVisionColor;
            // outline
            outline = b.outline;
            outlineColor = b.outlineColor;
            // thermal vision
            thermalVision = b.thermalVision;
            // lens dirt
            lensDirt = b.lensDirt;
            lensDirtThreshold = b.lensDirtThreshold;
            lensDirtIntensity = b.lensDirtIntensity;
            lensDirtTexture = b.lensDirtTexture;
            // bloom
            bloom = b.bloom;
            bloomCullingMask = b.bloomCullingMask;
            bloomLayerMaskDownsampling = b.bloomLayerMaskDownsampling;
            bloomIntensity = b.bloomIntensity;
            bloomMaxBrightness = b.bloomMaxBrightness;
            bloomBoost0 = b.bloomBoost0;
            bloomBoost1 = b.bloomBoost1;
            bloomBoost2 = b.bloomBoost2;
            bloomBoost3 = b.bloomBoost3;
            bloomBoost4 = b.bloomBoost4;
            bloomBoost5 = b.bloomBoost5;
            bloomAntiflicker = b.bloomAntiflicker;
            bloomUltra = b.bloomUltra;
            bloomThreshold = b.bloomThreshold;
            bloomCustomize = b.bloomCustomize;
            bloomWeight0 = b.bloomWeight0;
            bloomWeight1 = b.bloomWeight1;
            bloomWeight2 = b.bloomWeight2;
            bloomWeight3 = b.bloomWeight3;
            bloomWeight4 = b.bloomWeight4;
            bloomWeight5 = b.bloomWeight5;
            bloomBlur = b.bloomBlur;
            bloomDepthAtten = b.bloomDepthAtten;
            bloomLayerZBias = b.bloomLayerZBias;
            // anamorphic flares
            anamorphicFlares = b.anamorphicFlares;
            anamorphicFlaresIntensity = b.anamorphicFlaresIntensity;
            anamorphicFlaresAntiflicker = b.anamorphicFlaresAntiflicker;
            anamorphicFlaresUltra = b.anamorphicFlaresUltra;
            anamorphicFlaresThreshold = b.anamorphicFlaresThreshold;
            anamorphicFlaresSpread = b.anamorphicFlaresSpread;
            anamorphicFlaresVertical = b.anamorphicFlaresVertical;
            anamorphicFlaresTint = b.anamorphicFlaresTint;
            anamorphicFlaresBlur = b.anamorphicFlaresBlur;
            // dof
            depthOfField = b.depthOfField;
            depthOfFieldTransparencySupport = b.depthOfFieldTransparencySupport;
            depthOfFieldTargetFocus = b.depthOfFieldTargetFocus;
            depthOfFieldAutofocus = b.depthOfFieldAutofocus;
            depthOfFieldAutofocusLayerMask = b.depthOfFieldAutofocusLayerMask;
            depthOfFieldAutofocusMinDistance = b.depthOfFieldAutofocusMinDistance;
            depthOfFieldAutofocusMaxDistance = b.depthOfFieldAutofocusMaxDistance;
            depthOfFieldExclusionLayerMask = b.depthOfFieldExclusionLayerMask;
            depthOfFieldExclusionLayerMaskDownsampling = b.depthOfFieldExclusionLayerMaskDownsampling;
            depthOfFieldTransparencySupportDownsampling = b.depthOfFieldTransparencySupportDownsampling;
            depthOfFieldExclusionBias = b.depthOfFieldExclusionBias;
            depthOfFieldDistance = b.depthOfFieldDistance;
            depthOfFieldFocusSpeed = b.depthOfFieldFocusSpeed;
            depthOfFieldDownsampling = b.depthOfFieldDownsampling;
            depthOfFieldMaxSamples = b.depthOfFieldMaxSamples;
            depthOfFieldFocalLength = b.depthOfFieldFocalLength;
            depthOfFieldAperture = b.depthOfFieldAperture;
            depthOfFieldForegroundBlur = b.depthOfFieldForegroundBlur;
            depthOfFieldForegroundBlurHQ = b.depthOfFieldForegroundBlurHQ;
            depthOfFieldForegroundDistance = b.depthOfFieldForegroundDistance;
            depthOfFieldBokeh = b.depthOfFieldBokeh;
            depthOfFieldBokehThreshold = b.depthOfFieldBokehThreshold;
            depthOfFieldBokehIntensity = b.depthOfFieldBokehIntensity;
            depthOfFieldMaxBrightness = b.depthOfFieldMaxBrightness;
            depthOfFieldFilterMode = b.depthOfFieldFilterMode;
            // ea
            eyeAdaptation = b.eyeAdaptation;
            eyeAdaptationMaxExposure = b.eyeAdaptationMaxExposure;
            eyeAdaptationMinExposure = b.eyeAdaptationMinExposure;
            eyeAdaptationSpeedToDark = b.eyeAdaptationSpeedToDark;
            eyeAdaptationSpeedToLight = b.eyeAdaptationSpeedToLight;
            // purkinje
            purkinje = b.purkinje;
            purkinjeAmount = b.purkinjeAmount;
            purkinjeLuminanceThreshold = b.purkinjeLuminanceThreshold;
            // tonemap
            tonemap = b.tonemap;
            // flares
            sunFlares = b.sunFlares;
            sunFlaresIntensity = b.sunFlaresIntensity;
            sunFlaresSolarWindSpeed = b.sunFlaresSolarWindSpeed;
            sunFlaresTint = b.sunFlaresTint;
            sunFlaresDownsampling = b.sunFlaresDownsampling;
            sunFlaresSunIntensity = b.sunFlaresSunIntensity;
            sunFlaresSunDiskSize = b.sunFlaresSunDiskSize;
            sunFlaresSunRayDiffractionIntensity = b.sunFlaresSunRayDiffractionIntensity;
            sunFlaresSunRayDiffractionThreshold = b.sunFlaresSunRayDiffractionThreshold;
            sunFlaresCoronaRays1Length = b.sunFlaresCoronaRays1Length;
            sunFlaresCoronaRays1Spread = b.sunFlaresCoronaRays1Spread;
            sunFlaresCoronaRays1AngleOffset = b.sunFlaresCoronaRays1AngleOffset;
            sunFlaresCoronaRays1Streaks = b.sunFlaresCoronaRays1Streaks;
            sunFlaresCoronaRays2Length = b.sunFlaresCoronaRays2Length;
            sunFlaresCoronaRays2Spread = b.sunFlaresCoronaRays2Spread;
            sunFlaresCoronaRays2AngleOffset = b.sunFlaresCoronaRays2AngleOffset;
            sunFlaresCoronaRays2Streaks = b.sunFlaresCoronaRays2Streaks;
            sunFlaresGhosts1Size = b.sunFlaresGhosts1Size;
            sunFlaresGhosts1Offset = b.sunFlaresGhosts1Offset;
            sunFlaresGhosts1Brightness = b.sunFlaresGhosts1Brightness;
            sunFlaresGhosts2Size = b.sunFlaresGhosts2Size;
            sunFlaresGhosts2Offset = b.sunFlaresGhosts2Offset;
            sunFlaresGhosts2Brightness = b.sunFlaresGhosts2Brightness;
            sunFlaresGhosts3Size = b.sunFlaresGhosts3Size;
            sunFlaresGhosts3Offset = b.sunFlaresGhosts3Offset;
            sunFlaresGhosts3Brightness = b.sunFlaresGhosts3Brightness;
            sunFlaresGhosts4Size = b.sunFlaresGhosts4Size;
            sunFlaresGhosts4Offset = b.sunFlaresGhosts4Offset;
            sunFlaresGhosts4Brightness = b.sunFlaresGhosts4Brightness;
            sunFlaresHaloOffset = b.sunFlaresHaloOffset;
            sunFlaresHaloAmplitude = b.sunFlaresHaloAmplitude;
            sunFlaresHaloIntensity = b.sunFlaresHaloIntensity;
            sunFlaresRotationDeadZone = b.sunFlaresRotationDeadZone;
            // blur
            blur = b.blur;
            blurIntensity = b.blurIntensity;
            // pixelate
            pixelateAmount = b.pixelateAmount;
            pixelateDownscale = b.pixelateDownscale;
        }
    }

}