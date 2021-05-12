/// <summary>
/// Beautify effect inspector. Copyright 2016-2018 Kronnect
/// </summary>
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace BeautifyEffect
{
    [CustomEditor(typeof(BeautifyProfile))]
    public class BeautifyProfilenspector : Editor
    {
        static GUIStyle titleLabelStyle, labelBoldStyle, labelNormalStyle, sectionHeaderBoldStyle, sectionHeaderNormalStyle, sectionHeaderIndentedStyle;
        static GUIStyle buttonNormalStyle, buttonPressedStyle, blackBack;
        static Color titleColor;
        bool expandSharpenSection, expandLUTSection, expandBloomSection, expandAFSection;
        bool expandSFSection, expandSFCoronaRays1Section, expandSFCoronaRays2Section, expandSFGhosts1Section, expandSFGhosts2Section, expandSFGhosts3Section, expandSFGhosts4Section, expandSFHaloSection;
        bool expandDirtSection, expandDoFSection;
        bool expandEASection, expandPurkinjeSection, expandVignettingSection, expandDitherSection, expandFrameSection;
        bool expandOutlineSection, expandBlurSection;
        GUIStyle foldoutBold, foldoutNormal, foldOutIndented;
        bool layer1, layer2, layer3, layer4, layer5, layer6;

        SerializedProperty _sharpen, _sharpenMinDepth, _sharpenMaxDepth, _sharpenDepthThreshold, _sharpenRelaxation, _sharpenClamp, _sharpenMotionSensibility;
        SerializedProperty _dither, _ditherDepth;
        SerializedProperty _tonemap, _brightness, _saturate, _daltonize, _tintColor, _contrast;
        SerializedProperty _bloom, _bloomCullingMask, _bloomLayerMaskDownsampling, _bloomLayerZBias, _bloomIntensity, _bloomThreshold, _bloomDepthAtten, _bloomMaxBrightness;
        SerializedProperty _bloomAntiflicker, _bloomUltra, _bloomCustomize, _bloomWeight0, _bloomWeight1, _bloomWeight2, _bloomWeight3, _bloomWeight4, _bloomWeight5;
        SerializedProperty _bloomBoost0, _bloomBoost1, _bloomBoost2, _bloomBoost3, _bloomBoost4, _bloomBoost5, _bloomBlur;
        SerializedProperty _anamorphicFlares, _anamorphicFlaresIntensity, _anamorphicFlaresThreshold, _anamorphicFlaresSpread, _anamorphicFlaresVertical, _anamorphicFlaresTint;
        SerializedProperty _anamorphicFlaresAntiflicker, _anamorphicFlaresUltra, _anamorphicFlaresBlur;
        SerializedProperty _sunFlares, _sunFlaresIntensity, _sunFlaresTint, _sunFlaresDownsampling, _sunFlaresSunIntensity, _sunFlaresSunDiskSize, _sunFlaresSunRayDiffractionIntensity;
        SerializedProperty _sunFlaresSunRayDiffractionThreshold, _sunFlaresSolarWindSpeed, _sunFlaresRotationDeadZone;
        SerializedProperty _sunFlaresCoronaRays1Length, _sunFlaresCoronaRays1Streaks, _sunFlaresCoronaRays1Spread, _sunFlaresCoronaRays1AngleOffset;
        SerializedProperty _sunFlaresCoronaRays2Length, _sunFlaresCoronaRays2Streaks, _sunFlaresCoronaRays2Spread, _sunFlaresCoronaRays2AngleOffset;
        SerializedProperty _sunFlaresGhosts1Size, _sunFlaresGhosts1Offset, _sunFlaresGhosts1Brightness;
        SerializedProperty _sunFlaresGhosts2Size, _sunFlaresGhosts2Offset, _sunFlaresGhosts2Brightness;
        SerializedProperty _sunFlaresGhosts3Size, _sunFlaresGhosts3Offset, _sunFlaresGhosts3Brightness;
        SerializedProperty _sunFlaresGhosts4Size, _sunFlaresGhosts4Offset, _sunFlaresGhosts4Brightness;
        SerializedProperty _sunFlaresHaloOffset, _sunFlaresHaloAmplitude, _sunFlaresHaloIntensity;
        SerializedProperty _lensDirt, _lensDirtTexture, _lensDirtThreshold, _lensDirtIntensity;
        SerializedProperty _depthOfField, _depthOfFieldTransparencySupport, _depthOfFieldExclusionLayerMask, _depthOfFieldAutofocus, _depthOfFieldAutofocusLayerMask, _depthOfFieldAutofocusMinDistance, _depthOfFieldAutofocusMaxDistance;
        SerializedProperty _depthOfFieldTargetFocus, _depthOfFieldDistance, _depthOfFieldFocusSpeed, _depthOfFieldFocalLength, _depthOfFieldAperture, _depthOfFieldForegroundBlur, _depthOfFieldForegroundBlurHQ;
        SerializedProperty _depthOfFieldForegroundDistance, _depthOfFieldMaxBrightness, _depthOfFieldBokeh, _depthOfFieldBokehThreshold, _depthOfFieldBokehIntensity;
        SerializedProperty _depthOfFieldDownsampling, _depthOfFieldMaxSamples, _depthOfFieldExclusionBias, _depthOfFieldExclusionLayerMaskDownsampling;
        SerializedProperty _depthOfFieldTransparencySupportDownsampling, _depthOfFieldFilterMode;
        SerializedProperty _eyeAdaptation, _eyeAdaptationMinExposure, _eyeAdaptationMaxExposure, _eyeAdaptationSpeedToDark, _eyeAdaptationSpeedToLight;
        SerializedProperty _purkinje, _purkinjeAmount, _purkinjeLuminanceThreshold;
        SerializedProperty _vignetting, _vignettingColor, _vignettingFade, _vignettingCircularShape, _vignettingMask, _vignettingAspectRatio, _vignettingBlink;
        SerializedProperty _frame, _frameColor, _frameMask;
        SerializedProperty _outline, _outlineColor;
        SerializedProperty _lut, _lutTexture, _lutIntensity;
        SerializedProperty _nightVision, _nightVisionColor;
        SerializedProperty _thermalVision;
        SerializedProperty _blur, _blurIntensity;
        SerializedProperty _pixelateAmount, _pixelateDownscale;

        void OnEnable()
        {
            titleColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);
            blackBack = new GUIStyle();
            blackBack.normal.background = MakeTex(4, 4, Color.black);
            expandSharpenSection = EditorPrefs.GetBool("BeautifySharpenSection", false);
            expandLUTSection = EditorPrefs.GetBool("BeautifyLUTSection", false);
            expandBloomSection = EditorPrefs.GetBool("BeautifyBloomSection", false);
            expandAFSection = EditorPrefs.GetBool("BeautifyAFSection", false);
            expandSFSection = EditorPrefs.GetBool("BeautifySFSection", false);
            expandSFCoronaRays1Section = EditorPrefs.GetBool("BeautifySFCoronaRays1Section", false);
            expandSFCoronaRays2Section = EditorPrefs.GetBool("BeautifySFCoronaRays2Section", false);
            expandSFGhosts1Section = EditorPrefs.GetBool("BeautifySFGhosts1Section", false);
            expandSFGhosts2Section = EditorPrefs.GetBool("BeautifySFGhosts2Section", false);
            expandSFGhosts3Section = EditorPrefs.GetBool("BeautifySFGhosts3Section", false);
            expandSFGhosts4Section = EditorPrefs.GetBool("BeautifySFGhosts4Section", false);
            expandSFHaloSection = EditorPrefs.GetBool("BeautifySFHaloSection", false);
            expandDirtSection = EditorPrefs.GetBool("BeautifyDirtSection", false);
            expandDoFSection = EditorPrefs.GetBool("BeautifyDoFSection", false);
            expandEASection = EditorPrefs.GetBool("BeautifyEASection", false);
            expandPurkinjeSection = EditorPrefs.GetBool("BeautifyPurkinjeSection", false);
            expandVignettingSection = EditorPrefs.GetBool("BeautifyVignettingSection", false);
            expandFrameSection = EditorPrefs.GetBool("BeautifyFrameSection", false);
            expandOutlineSection = EditorPrefs.GetBool("BeautifyOutlineSection", false);
            expandDitherSection = EditorPrefs.GetBool("BeautifyDitherSection", false);
            expandBlurSection = EditorPrefs.GetBool("BeautifyBlurSection", false);

            // Setup property fields

            // Image enhancements
            _sharpen = serializedObject.FindProperty("sharpen");
            _sharpenMinDepth = serializedObject.FindProperty("sharpenMinDepth");
            _sharpenMaxDepth = serializedObject.FindProperty("sharpenMaxDepth");
            _sharpenDepthThreshold = serializedObject.FindProperty("sharpenDepthThreshold");
            _sharpenRelaxation = serializedObject.FindProperty("sharpenRelaxation");
            _sharpenClamp = serializedObject.FindProperty("sharpenClamp");
            _sharpenMotionSensibility = serializedObject.FindProperty("sharpenMotionSensibility");
            _dither = serializedObject.FindProperty("dither");
            _ditherDepth = serializedObject.FindProperty("ditherDepth");

            // Tonemapping & color grading
            _tonemap = serializedObject.FindProperty("tonemap");
            _brightness = serializedObject.FindProperty("brightness");
            _saturate = serializedObject.FindProperty("saturate");
            _daltonize = serializedObject.FindProperty("daltonize");
            _tintColor = serializedObject.FindProperty("tintColor");
            _contrast = serializedObject.FindProperty("contrast");

            // Bloom
            _bloom = serializedObject.FindProperty("bloom");
            _bloomCullingMask = serializedObject.FindProperty("bloomCullingMask");
            _bloomLayerMaskDownsampling = serializedObject.FindProperty("bloomLayerMaskDownsampling");
            _bloomLayerZBias = serializedObject.FindProperty("bloomLayerZBias");
            _bloomIntensity = serializedObject.FindProperty("bloomIntensity");
            _bloomThreshold = serializedObject.FindProperty("bloomThreshold");
            _bloomDepthAtten = serializedObject.FindProperty("bloomDepthAtten");
            _bloomMaxBrightness = serializedObject.FindProperty("bloomMaxBrightness");
            _bloomAntiflicker = serializedObject.FindProperty("bloomAntiflicker");
            _bloomUltra = serializedObject.FindProperty("bloomUltra");
            _bloomCustomize = serializedObject.FindProperty("bloomCustomize");
            _bloomWeight0 = serializedObject.FindProperty("bloomWeight0");
            _bloomWeight1 = serializedObject.FindProperty("bloomWeight1");
            _bloomWeight2 = serializedObject.FindProperty("bloomWeight2");
            _bloomWeight3 = serializedObject.FindProperty("bloomWeight3");
            _bloomWeight4 = serializedObject.FindProperty("bloomWeight4");
            _bloomWeight5 = serializedObject.FindProperty("bloomWeight5");
            _bloomBoost0 = serializedObject.FindProperty("bloomBoost0");
            _bloomBoost1 = serializedObject.FindProperty("bloomBoost1");
            _bloomBoost2 = serializedObject.FindProperty("bloomBoost2");
            _bloomBoost3 = serializedObject.FindProperty("bloomBoost3");
            _bloomBoost4 = serializedObject.FindProperty("bloomBoost4");
            _bloomBoost5 = serializedObject.FindProperty("bloomBoost5");
            _bloomBlur = serializedObject.FindProperty("bloomBlur");

            // Anamorphic flares
            _anamorphicFlares = serializedObject.FindProperty("anamorphicFlares");
            _anamorphicFlaresIntensity = serializedObject.FindProperty("anamorphicFlaresIntensity");
            _anamorphicFlaresThreshold = serializedObject.FindProperty("anamorphicFlaresThreshold");
            _anamorphicFlaresSpread = serializedObject.FindProperty("anamorphicFlaresSpread");
            _anamorphicFlaresVertical = serializedObject.FindProperty("anamorphicFlaresVertical");
            _anamorphicFlaresTint = serializedObject.FindProperty("anamorphicFlaresTint");
            _anamorphicFlaresAntiflicker = serializedObject.FindProperty("anamorphicFlaresAntiflicker");
            _anamorphicFlaresUltra = serializedObject.FindProperty("anamorphicFlaresUltra");
            _anamorphicFlaresBlur = serializedObject.FindProperty("anamorphicFlaresBlur");

            // Sun flares
            _sunFlares = serializedObject.FindProperty("sunFlares");
            _sunFlaresIntensity = serializedObject.FindProperty("sunFlaresIntensity");
            _sunFlaresTint = serializedObject.FindProperty("sunFlaresTint");
            _sunFlaresDownsampling = serializedObject.FindProperty("sunFlaresDownsampling");
            _sunFlaresSunIntensity = serializedObject.FindProperty("sunFlaresSunIntensity");
            _sunFlaresSunDiskSize = serializedObject.FindProperty("sunFlaresSunDiskSize");
            _sunFlaresSunRayDiffractionIntensity = serializedObject.FindProperty("sunFlaresSunRayDiffractionIntensity");
            _sunFlaresSunRayDiffractionThreshold = serializedObject.FindProperty("sunFlaresSunRayDiffractionThreshold");
            _sunFlaresSolarWindSpeed = serializedObject.FindProperty("sunFlaresSolarWindSpeed");
            _sunFlaresRotationDeadZone = serializedObject.FindProperty("sunFlaresRotationDeadZone");
            _sunFlaresCoronaRays1Length = serializedObject.FindProperty("sunFlaresCoronaRays1Length");
            _sunFlaresCoronaRays1Streaks = serializedObject.FindProperty("sunFlaresCoronaRays1Streaks");
            _sunFlaresCoronaRays1Spread = serializedObject.FindProperty("sunFlaresCoronaRays1Spread");
            _sunFlaresCoronaRays1AngleOffset = serializedObject.FindProperty("sunFlaresCoronaRays1AngleOffset");
            _sunFlaresCoronaRays2Length = serializedObject.FindProperty("sunFlaresCoronaRays2Length");
            _sunFlaresCoronaRays2Streaks = serializedObject.FindProperty("sunFlaresCoronaRays2Streaks");
            _sunFlaresCoronaRays2Spread = serializedObject.FindProperty("sunFlaresCoronaRays2Spread");
            _sunFlaresCoronaRays2AngleOffset = serializedObject.FindProperty("sunFlaresCoronaRays2AngleOffset");
            _sunFlaresGhosts1Size = serializedObject.FindProperty("sunFlaresGhosts1Size");
            _sunFlaresGhosts1Offset = serializedObject.FindProperty("sunFlaresGhosts1Offset");
            _sunFlaresGhosts1Brightness = serializedObject.FindProperty("sunFlaresGhosts1Brightness");
            _sunFlaresGhosts2Size = serializedObject.FindProperty("sunFlaresGhosts2Size");
            _sunFlaresGhosts2Offset = serializedObject.FindProperty("sunFlaresGhosts2Offset");
            _sunFlaresGhosts2Brightness = serializedObject.FindProperty("sunFlaresGhosts2Brightness");
            _sunFlaresGhosts3Size = serializedObject.FindProperty("sunFlaresGhosts3Size");
            _sunFlaresGhosts3Offset = serializedObject.FindProperty("sunFlaresGhosts3Offset");
            _sunFlaresGhosts3Brightness = serializedObject.FindProperty("sunFlaresGhosts3Brightness");
            _sunFlaresGhosts4Size = serializedObject.FindProperty("sunFlaresGhosts4Size");
            _sunFlaresGhosts4Offset = serializedObject.FindProperty("sunFlaresGhosts4Offset");
            _sunFlaresGhosts4Brightness = serializedObject.FindProperty("sunFlaresGhosts4Brightness");
            _sunFlaresHaloOffset = serializedObject.FindProperty("sunFlaresHaloOffset");
            _sunFlaresHaloAmplitude = serializedObject.FindProperty("sunFlaresHaloAmplitude");
            _sunFlaresHaloIntensity = serializedObject.FindProperty("sunFlaresHaloIntensity");

            // Lens Dirt
            _lensDirt = serializedObject.FindProperty("lensDirt");
            _lensDirtTexture = serializedObject.FindProperty("lensDirtTexture");
            _lensDirtThreshold = serializedObject.FindProperty("lensDirtThreshold");
            _lensDirtIntensity = serializedObject.FindProperty("lensDirtIntensity");

            // Depth of Field
            _depthOfField = serializedObject.FindProperty("depthOfField");
            _depthOfFieldTransparencySupport = serializedObject.FindProperty("depthOfFieldTransparencySupport");
            _depthOfFieldExclusionLayerMask = serializedObject.FindProperty("depthOfFieldExclusionLayerMask");
            _depthOfFieldAutofocus = serializedObject.FindProperty("depthOfFieldAutofocus");
            _depthOfFieldAutofocusLayerMask = serializedObject.FindProperty("depthOfFieldAutofocusLayerMask");
            _depthOfFieldAutofocusMinDistance = serializedObject.FindProperty("depthOfFieldAutofocusMinDistance");
            _depthOfFieldAutofocusMaxDistance = serializedObject.FindProperty("depthOfFieldAutofocusMaxDistance");
            _depthOfFieldTargetFocus = serializedObject.FindProperty("depthOfFieldTargetFocus");
            _depthOfFieldDistance = serializedObject.FindProperty("depthOfFieldDistance");
            _depthOfFieldFocusSpeed = serializedObject.FindProperty("depthOfFieldFocusSpeed");
            _depthOfFieldFocalLength = serializedObject.FindProperty("depthOfFieldFocalLength");
            _depthOfFieldAperture = serializedObject.FindProperty("depthOfFieldAperture");
            _depthOfFieldForegroundBlur = serializedObject.FindProperty("depthOfFieldForegroundBlur");
            _depthOfFieldForegroundBlurHQ = serializedObject.FindProperty("depthOfFieldForegroundBlurHQ");
            _depthOfFieldForegroundDistance = serializedObject.FindProperty("depthOfFieldForegroundDistance");
            _depthOfFieldMaxBrightness = serializedObject.FindProperty("depthOfFieldMaxBrightness");
            _depthOfFieldBokeh = serializedObject.FindProperty("depthOfFieldBokeh");
            _depthOfFieldBokehThreshold = serializedObject.FindProperty("depthOfFieldBokehThreshold");
            _depthOfFieldBokehIntensity = serializedObject.FindProperty("depthOfFieldBokehIntensity");
            _depthOfFieldDownsampling = serializedObject.FindProperty("depthOfFieldDownsampling");
            _depthOfFieldMaxSamples = serializedObject.FindProperty("depthOfFieldMaxSamples");
            _depthOfFieldExclusionBias = serializedObject.FindProperty("depthOfFieldExclusionBias");
            _depthOfFieldExclusionLayerMaskDownsampling = serializedObject.FindProperty("depthOfFieldExclusionLayerMaskDownsampling");
            _depthOfFieldTransparencySupportDownsampling = serializedObject.FindProperty("depthOfFieldTransparencySupportDownsampling");
            _depthOfFieldFilterMode = serializedObject.FindProperty("depthOfFieldFilterMode");

            // Eye adaptation
            _eyeAdaptation = serializedObject.FindProperty("eyeAdaptation");
            _eyeAdaptationMinExposure = serializedObject.FindProperty("eyeAdaptationMinExposure");
            _eyeAdaptationMaxExposure = serializedObject.FindProperty("eyeAdaptationMaxExposure");
            _eyeAdaptationSpeedToDark = serializedObject.FindProperty("eyeAdaptationSpeedToDark");
            _eyeAdaptationSpeedToLight = serializedObject.FindProperty("eyeAdaptationSpeedToLight");

            // Purkinje shift
            _purkinje = serializedObject.FindProperty("purkinje");
            _purkinjeAmount = serializedObject.FindProperty("purkinjeAmount");
            _purkinjeLuminanceThreshold = serializedObject.FindProperty("purkinjeLuminanceThreshold");

            // Vignetting
            _vignetting = serializedObject.FindProperty("vignetting");
            _vignettingColor = serializedObject.FindProperty("vignettingColor");
            _vignettingFade = serializedObject.FindProperty("vignettingFade");
            _vignettingCircularShape = serializedObject.FindProperty("vignettingCircularShape");
            _vignettingAspectRatio = serializedObject.FindProperty("vignettingAspectRatio");
            _vignettingMask = serializedObject.FindProperty("vignettingMask");
            _vignettingBlink = serializedObject.FindProperty("vignettingBlink");

            // Frame
            _frame = serializedObject.FindProperty("frame");
            _frameColor = serializedObject.FindProperty("frameColor");
            _frameMask = serializedObject.FindProperty("frameMask");

            // Outline
            _outline = serializedObject.FindProperty("outline");
            _outlineColor = serializedObject.FindProperty("outlineColor");

            // Lut
            _lut = serializedObject.FindProperty("lut");
            _lutTexture = serializedObject.FindProperty("lutTexture");
            _lutIntensity = serializedObject.FindProperty("lutIntensity");

            // Night vision
            _nightVision = serializedObject.FindProperty("nightVision");
            _nightVisionColor = serializedObject.FindProperty("nightVisionColor");

            // Thermal vision
            _thermalVision = serializedObject.FindProperty("thermalVision");

            // Blur
            _blur = serializedObject.FindProperty("blur");
            _blurIntensity = serializedObject.FindProperty("blurIntensity");

            // Pixelate
            _pixelateAmount = serializedObject.FindProperty("pixelateAmount");
            _pixelateDownscale = serializedObject.FindProperty("pixelateDownscale");
        }

        void OnDestroy()
        {
            // Restore folding sections state
            EditorPrefs.SetBool("BeautifySharpenSection", expandSharpenSection);
            EditorPrefs.SetBool("BeautifyBloomSection", expandBloomSection);
            EditorPrefs.SetBool("BeautifyAFSection", expandAFSection);
            EditorPrefs.SetBool("BeautifySFSection", expandSFSection);
            EditorPrefs.SetBool("BeautifySFCoronaRays1Section", expandSFCoronaRays1Section);
            EditorPrefs.SetBool("BeautifyFCoronaRays2Section", expandSFCoronaRays2Section);
            EditorPrefs.SetBool("BeautifySFGhosts1Section", expandSFGhosts1Section);
            EditorPrefs.SetBool("BeautifySFGhosts2Section", expandSFGhosts2Section);
            EditorPrefs.SetBool("BeautifySFGhosts3Section", expandSFGhosts3Section);
            EditorPrefs.SetBool("BeautifySFGhosts4Section", expandSFGhosts4Section);
            EditorPrefs.SetBool("BeautifySFHaloSection", expandSFHaloSection);
            EditorPrefs.SetBool("BeautifyDirtSection", expandDirtSection);
            EditorPrefs.SetBool("BeautifyDoFSection", expandDoFSection);
            EditorPrefs.SetBool("BeautifyEASection", expandEASection);
            EditorPrefs.SetBool("BeautifyPurkinjeSection", expandPurkinjeSection);
            EditorPrefs.SetBool("BeautifyVignettingSection", expandVignettingSection);
            EditorPrefs.SetBool("BeautifyFrameSection", expandFrameSection);
            EditorPrefs.SetBool("BeautifyOutlineSection", expandOutlineSection);
            EditorPrefs.SetBool("BeautifyLUTSection", expandLUTSection);
            EditorPrefs.SetBool("BeautifyDitherSection", expandDitherSection);
            EditorPrefs.SetBool("BeautifyBlurSection", expandBlurSection);
        }

        public override void OnInspectorGUI()
        {

            // setup styles
            if (labelBoldStyle == null)
            {
                labelBoldStyle = new GUIStyle(EditorStyles.label); // GUI.skin.label);
                labelBoldStyle.fontStyle = FontStyle.Bold;
            }
            if (labelNormalStyle == null)
            {
                labelNormalStyle = new GUIStyle(EditorStyles.label); // GUI.skin.label);
            }
            if (sectionHeaderNormalStyle == null)
            {
                sectionHeaderNormalStyle = new GUIStyle(EditorStyles.foldout);
            }
            sectionHeaderNormalStyle.margin = new RectOffset(12, 0, 0, 0);
            if (sectionHeaderBoldStyle == null)
            {
                sectionHeaderBoldStyle = new GUIStyle(sectionHeaderNormalStyle);
            }
            sectionHeaderBoldStyle.fontStyle = FontStyle.Bold;
            if (sectionHeaderIndentedStyle == null)
            {
                sectionHeaderIndentedStyle = new GUIStyle(EditorStyles.foldout);
            }
            sectionHeaderIndentedStyle.margin = new RectOffset(24, 0, 0, 0);
            if (buttonNormalStyle == null)
            {
                buttonNormalStyle = new GUIStyle(GUI.skin.button); // EditorStyles.miniButtonMid);
            }
            if (buttonPressedStyle == null)
            {
                buttonPressedStyle = new GUIStyle(buttonNormalStyle);
                buttonPressedStyle.fontStyle = FontStyle.Bold;
            }
            if (foldoutBold == null)
            {
                foldoutBold = new GUIStyle(EditorStyles.foldout);
                foldoutBold.fontStyle = FontStyle.Bold;
            }
            if (foldoutNormal == null)
            {
                foldoutNormal = new GUIStyle(EditorStyles.foldout);
            }
            if (foldOutIndented == null)
            {
                foldOutIndented = new GUIStyle(EditorStyles.foldout);
            }
            foldOutIndented.margin = new RectOffset(26, 0, 0, 0);

            serializedObject.Update();

            // draw interface
            EditorGUIUtility.labelWidth = 125;
            EditorGUILayout.Separator();
            DrawLabel("Image Enhancement");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(121));
            expandSharpenSection = EditorGUILayout.Foldout(expandSharpenSection, new GUIContent("Sharpen", "Sharpen intensity."), sectionHeaderNormalStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_sharpen, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            if (expandSharpenSection)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("   Min/Max Depth", "Any pixel outside this depth range won't be affected by sharpen. Reduce range to create a depth-of-field-like effect."), GUILayout.Width(121));
                float minDepth = _sharpenMinDepth.floatValue;
                float maxDepth = _sharpenMaxDepth.floatValue;
                EditorGUILayout.MinMaxSlider(ref minDepth, ref maxDepth, 0f, 1.1f);
                _sharpenMinDepth.floatValue = minDepth;
                _sharpenMaxDepth.floatValue = maxDepth;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(_sharpenDepthThreshold, new GUIContent("   Depth Threshold", "Reduces sharpen if depth difference around a pixel exceeds this value. Useful to prevent artifacts around wires or thin objects."));
                EditorGUILayout.PropertyField(_sharpenRelaxation, new GUIContent("   Luminance Relax.", "Soften sharpen around a pixel with high contrast. Reduce this value to remove ghosting and protect fine drawings or wires over a flat surface."));
                EditorGUILayout.PropertyField(_sharpenClamp, new GUIContent("   Clamp", "Maximum pixel adjustment."));
                EditorGUILayout.PropertyField(_sharpenMotionSensibility, new GUIContent("   Motion Sensibility", "Increase to reduce sharpen to simulate a cheap motion blur and to reduce flickering when camera rotates or moves. This slider controls the amount of camera movement/rotation that contributes to sharpen reduction. Set this to 0 to disable this feature."));
            }


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(121));
            expandDitherSection = EditorGUILayout.Foldout(expandDitherSection, new GUIContent("Dither", "Simulates more colors than RGB quantization can produce. Removes banding artifacts in gradients, like skybox. This setting controls the dithering strength."), sectionHeaderNormalStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_dither, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            if (expandDitherSection)
            {
                EditorGUILayout.PropertyField(_ditherDepth, new GUIContent("   Min Depth", "Will only remove bands on pixels beyond this depth. Useful if you only want to remove sky banding (set this to 0.99)"));
            }

            EditorGUILayout.Separator();
            DrawLabel("Tonemapping & Color Grading");

            EditorGUILayout.BeginHorizontal();
            GUIStyle labelStyle = _tonemap.intValue != (int)BEAUTIFY_TMO.Linear ? labelBoldStyle : labelNormalStyle;
            GUILayout.Label(new GUIContent("Tonemapping", "Converts high dynamic range colors into low dynamic range space according to a chosen tone mapping operator."), labelStyle, GUILayout.Width(121));
            EditorGUILayout.PropertyField(_tonemap, GUIContent.none);

            EditorGUILayout.EndHorizontal();

            if (_tonemap.intValue != (int)BEAUTIFY_TMO.Linear)
            {
                EditorGUILayout.PropertyField(_brightness, new GUIContent("Exposure", "Exposure applied before tonemapping. Increase to make the image brighter."));
            }

            EditorGUILayout.PropertyField(_saturate, new GUIContent("Vibrance", "Improves pixels color depending on their saturation."));

            EditorGUILayout.BeginHorizontal();
            labelStyle = _daltonize.floatValue > 0 ? labelBoldStyle : labelNormalStyle;
            GUILayout.Label(new GUIContent("Daltonize", "Similar to vibrance but mostly accentuate primary red, green and blue colors to compensate protanomaly (red deficiency), deuteranomaly (green deficiency) and tritanomaly (blue deficiency). This effect does not shift color hue hence it won't help completely red, green or blue color blindness. The effect will vary depending on each subject so this effect should be enabled on user demand."), labelStyle, GUILayout.Width(121));
            EditorGUILayout.PropertyField(_daltonize, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            labelStyle = _tintColor.colorValue.a > 0 ? labelBoldStyle : labelNormalStyle;
            GUILayout.Label(new GUIContent("Tint", "Blends image with an optional color. Alpha specifies intensity."), labelStyle, GUILayout.Width(121));
            EditorGUILayout.PropertyField(_tintColor, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
            labelStyle = _lut.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
            expandLUTSection = EditorGUILayout.Foldout(expandLUTSection, new GUIContent("LUT", "Enables LUT based transformation."), labelStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_lut, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            if (expandLUTSection)
            {
                EditorGUILayout.PropertyField(_lutTexture, new GUIContent("   Texture"));
                EditorGUILayout.PropertyField(_lutIntensity, new GUIContent("   Intensity"));
            }

            EditorGUILayout.PropertyField(_contrast, new GUIContent("Contrast", "Final image contrast adjustment. Allows you to create more vivid images."));

            if (_tonemap.intValue == (int)BEAUTIFY_TMO.Linear)
            {
                EditorGUILayout.PropertyField(_brightness, new GUIContent("Brightness", "Final image brightness adjustment."));
            }


            EditorGUILayout.Separator();
            DrawLabel("Lens & Lighting Effects");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(121));
            labelStyle = _bloom.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
            expandBloomSection = EditorGUILayout.Foldout(expandBloomSection, new GUIContent("Bloom", "Produces fringes of light extending from the borders of bright areas, contributing to the illusion of an extremely bright light overwhelming the camera or eye capturing the scene."), labelStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_bloom, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            if (expandBloomSection)
            {
                EditorGUILayout.BeginHorizontal();
                labelStyle = _bloomCullingMask.intValue != 0 ? labelBoldStyle : labelNormalStyle;
                GUILayout.Label(new GUIContent("   Layer Mask", "Select which layers can be used for bloom."), labelStyle, GUILayout.Width(121));
                EditorGUILayout.PropertyField(_bloomCullingMask, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                if (_bloom.boolValue)
                {
                    if ((_bloomCullingMask.intValue & 1) != 0)
                    {
                        EditorGUILayout.HelpBox("Set Layer Mask either to Nothing (default value) or to specific layers. Including Default layer is not recommended. If you want bloom to be applied to all objects, set Layer Mask to Nothing.", MessageType.Warning);
                    }
                }
                if (_bloomCullingMask.intValue != 0)
                {
                    EditorGUILayout.PropertyField(_bloomLayerMaskDownsampling, new GUIContent("   Mask Downsampling", "Bloom/anamorphic flares layer mask downsampling factor. Increase to improve performance."));
                    EditorGUILayout.PropertyField(_bloomLayerZBias, new GUIContent("   Mask Z Bias", "Optionally applies a small offset (ie. 0.01) to the depth comparisson between objects in the bloom layer and others, enabling bloom effect behind opaque objects (similar to translucency effect)."));
                }
                EditorGUILayout.PropertyField(_bloomIntensity, new GUIContent("   Intensity", "Bloom multiplier."));
                EditorGUILayout.PropertyField(_bloomThreshold, new GUIContent("   Threshold", "Brightness sensibility."));

                EditorGUILayout.BeginHorizontal();
                labelStyle = _bloomDepthAtten.floatValue > 0 ? labelBoldStyle : labelNormalStyle;
                GUILayout.Label(new GUIContent("   Depth Atten", "Reduces bloom effect on distance."), labelStyle, GUILayout.Width(121));
                EditorGUILayout.PropertyField(_bloomDepthAtten, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(_bloomMaxBrightness, new GUIContent("   Clamp Brightness", "Clamps maximum pixel brightness to prevent out of range bright spots."));
                EditorGUILayout.PropertyField(_bloomAntiflicker, new GUIContent("   Reduce Flicker", "Enables an additional filter to reduce excess of flicker."));
                EditorGUILayout.PropertyField(_bloomUltra, new GUIContent("   Ultra", "Increase bloom fidelity."));

                bool prevCustomize = _bloomCustomize.boolValue;
                EditorGUILayout.PropertyField(_bloomCustomize, new GUIContent("   Customize", "Edit bloom style parameters."));
                if (_bloomCustomize.boolValue)
                {
                    if (!prevCustomize)
                    {
                        layer1 = layer2 = layer3 = layer4 = layer5 = layer6 = true;
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("   Presets", GUILayout.Width(120));
                    if (GUILayout.Button("Focused"))
                    {
                        _bloomWeight0.floatValue = 1f;
                        _bloomWeight1.floatValue = 0.9f;
                        _bloomWeight2.floatValue = 0.75f;
                        _bloomWeight3.floatValue = 0.6f;
                        _bloomWeight4.floatValue = 0.35f;
                        _bloomWeight5.floatValue = 0.1f;
                        _bloomBoost0.floatValue = _bloomBoost1.floatValue = _bloomBoost2.floatValue = _bloomBoost3.floatValue = _bloomBoost4.floatValue = _bloomBoost5.floatValue = 0;
                    }
                    if (GUILayout.Button("Regular"))
                    {
                        _bloomWeight0.floatValue = 0.25f;
                        _bloomWeight1.floatValue = 0.33f;
                        _bloomWeight2.floatValue = 0.8f;
                        _bloomWeight3.floatValue = 1f;
                        _bloomWeight4.floatValue = 1f;
                        _bloomWeight5.floatValue = 1f;
                        _bloomBoost0.floatValue = _bloomBoost1.floatValue = _bloomBoost2.floatValue = _bloomBoost3.floatValue = _bloomBoost4.floatValue = _bloomBoost5.floatValue = 0;
                    }
                    if (GUILayout.Button("Blurred"))
                    {
                        _bloomWeight0.floatValue = 0.05f;
                        _bloomWeight1.floatValue = 0.075f;
                        _bloomWeight2.floatValue = 0.1f;
                        _bloomWeight3.floatValue = 0.2f;
                        _bloomWeight4.floatValue = 0.4f;
                        _bloomWeight5.floatValue = 1f;
                        _bloomBoost0.floatValue = _bloomBoost1.floatValue = _bloomBoost2.floatValue = _bloomBoost3.floatValue = 0;
                        _bloomBoost4.floatValue = 0.5f;
                        _bloomBoost5.floatValue = 1f;
                    }
                    EditorGUILayout.EndHorizontal();

                    layer1 = EditorGUILayout.Foldout(layer1, "Layer 1", foldOutIndented);
                    if (layer1)
                    {
                        EditorGUILayout.PropertyField(_bloomWeight0, new GUIContent("      Weight", "First layer bloom weight."));
                        EditorGUILayout.PropertyField(_bloomBoost0, new GUIContent("      Boost", "Intensity bonus for first layer."));
                    }
                    layer2 = EditorGUILayout.Foldout(layer2, "Layer 2", foldOutIndented);
                    if (layer2)
                    {
                        EditorGUILayout.PropertyField(_bloomWeight1, new GUIContent("      Weight", "Second layer bloom weight."));
                        EditorGUILayout.PropertyField(_bloomBoost1, new GUIContent("      Boost", "Intensity bonus for second layer."));
                    }
                    layer3 = EditorGUILayout.Foldout(layer3, "Layer 3", foldOutIndented);
                    if (layer3)
                    {
                        EditorGUILayout.PropertyField(_bloomWeight2, new GUIContent("      Weight", "Third layer bloom weight."));
                        EditorGUILayout.PropertyField(_bloomBoost2, new GUIContent("      Boost", "Intensity bonus for third layer."));
                    }
                    layer4 = EditorGUILayout.Foldout(layer4, "Layer 4", foldOutIndented);
                    if (layer4)
                    {
                        EditorGUILayout.PropertyField(_bloomWeight3, new GUIContent("      Weight", "Fourth layer bloom weight."));
                        EditorGUILayout.PropertyField(_bloomBoost3, new GUIContent("      Boost", "Intensity bonus for fourth layer."));
                    }
                    layer5 = EditorGUILayout.Foldout(layer5, "Layer 5", foldOutIndented);
                    if (layer5)
                    {
                        EditorGUILayout.PropertyField(_bloomWeight4, new GUIContent("      Weight", "Fifth layer bloom weight."));
                        EditorGUILayout.PropertyField(_bloomBoost4, new GUIContent("      Boost", "Intensity bonus for fifth layer."));
                    }
                    layer6 = EditorGUILayout.Foldout(layer6, "Layer 6", foldOutIndented);
                    if (layer6)
                    {
                        EditorGUILayout.PropertyField(_bloomWeight5, new GUIContent("      Weight", "Sixth layer bloom weight."));
                        EditorGUILayout.PropertyField(_bloomBoost5, new GUIContent("      Boost", "Intensity bonus for sixth layer."));
                    }
                }
                EditorGUILayout.PropertyField(_bloomBlur, new GUIContent("   Blur", "Adds an additional blur pass to smooth bloom."));
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
            labelStyle = _anamorphicFlares.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
            expandAFSection = EditorGUILayout.Foldout(expandAFSection, new GUIContent("Anamorphic F.", "Also known as JJ Abrams flares, adds spectacular light streaks to your scene."), labelStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_anamorphicFlares, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            if (expandAFSection)
            {
                EditorGUILayout.PropertyField(_bloomCullingMask, new GUIContent("   Layer Mask", "Select which layers can be used for bloom."));
                if (_anamorphicFlares.boolValue)
                {
                    if ((_bloomCullingMask.intValue & 1) != 0)
                    {
                        EditorGUILayout.HelpBox("Set Layer Mask either to Nothing (default value) or to specific layers. Including Default layer is not recommended. If you want flares to be applied to all objects, set Layer Mask to Nothing.", MessageType.Warning);
                    }
                }
                if (_bloomCullingMask.intValue != 0)
                {
                    EditorGUILayout.PropertyField(_bloomLayerMaskDownsampling, new GUIContent("   Mask Downsampling", "Bloom/anamorphic flares layer mask downsampling factor. Increase to improve performance."));
                }
                EditorGUILayout.PropertyField(_anamorphicFlaresIntensity, new GUIContent("   Intensity", "Flares light multiplier."));
                EditorGUILayout.PropertyField(_anamorphicFlaresThreshold, new GUIContent("   Threshold", "Brightness sensibility."));
                EditorGUILayout.PropertyField(_anamorphicFlaresSpread, new GUIContent("   Spread", "Amplitude of the flares."));
                EditorGUILayout.PropertyField(_anamorphicFlaresVertical, new GUIContent("   Vertical"));
                EditorGUILayout.PropertyField(_anamorphicFlaresTint, new GUIContent("   Tint", "Optional tint color for the anamorphic flares. Use color alpha component to blend between original color and the tint."));

                EditorGUILayout.PropertyField(_anamorphicFlaresAntiflicker, new GUIContent("   Reduce Flicker", "Enables an additional filter to reduce excess of flicker."));
                EditorGUILayout.PropertyField(_anamorphicFlaresUltra, new GUIContent("   Ultra", "Increases anamorphic flares fidelity."));
                EditorGUILayout.PropertyField(_anamorphicFlaresBlur, new GUIContent("   Blur", "Adds an additional blur pass to smooth anamorphic flares effect."));
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
            labelStyle = _sunFlares.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
            expandSFSection = EditorGUILayout.Foldout(expandSFSection, new GUIContent("Sun Flares", "Adds lens flares caused by bright Sun light."), labelStyle);
            EditorGUILayout.EndHorizontal();
            _sunFlares.boolValue = EditorGUILayout.Toggle(_sunFlares.boolValue);
            EditorGUILayout.EndHorizontal();
            if (expandSFSection)
            {
                EditorGUILayout.PropertyField(_sunFlaresIntensity, new GUIContent("   Global Intensity", "Global intensity for the sun flares buffer."));
                EditorGUILayout.PropertyField(_sunFlaresTint, new GUIContent("   Tint", "Global flares tint color."));
                EditorGUILayout.PropertyField(_sunFlaresDownsampling, new GUIContent("   Downsampling", "Reduces sun flares buffer dimensions to improve performance."));
                EditorGUILayout.PropertyField(_sunFlaresSunIntensity, new GUIContent("   Sun Intensity", "Intensity for the Sun's disk and corona rays."));
                EditorGUILayout.PropertyField(_sunFlaresSunDiskSize, new GUIContent("   Sun Disk Size", "Size of Sun disk."));
                EditorGUILayout.PropertyField(_sunFlaresSunRayDiffractionIntensity, new GUIContent("   Diffraction Intensity", "Intensity for the Sun's rays diffraction."));
                EditorGUILayout.PropertyField(_sunFlaresSunRayDiffractionThreshold, new GUIContent("   Diffraction Threshold", "Theshold of the Sun's rays diffraction."));
                EditorGUILayout.PropertyField(_sunFlaresSolarWindSpeed, new GUIContent("   Solar Wind Speed", "Animation speed for the diffracted rays."));
                EditorGUILayout.PropertyField(_sunFlaresRotationDeadZone, new GUIContent("   Rotation DeadZone", "Prevents ray rotation when looking directly to the Sun."));

                expandSFCoronaRays1Section = EditorGUILayout.Foldout(expandSFCoronaRays1Section, new GUIContent("Corona Rays Group 1", "Customize appearance of solar corona rays group 1."), sectionHeaderIndentedStyle);
                if (expandSFCoronaRays1Section)
                {
                    EditorGUILayout.PropertyField(_sunFlaresCoronaRays1Length, new GUIContent("   Length", "Length of solar corona rays group 1."));
                    EditorGUILayout.PropertyField(_sunFlaresCoronaRays1Streaks, new GUIContent("   Streaks", "Number of streaks for group 1."));
                    EditorGUILayout.PropertyField(_sunFlaresCoronaRays1Spread, new GUIContent("   Spread", "Light spread factor for group 1."));
                    EditorGUILayout.PropertyField(_sunFlaresCoronaRays1AngleOffset, new GUIContent("   Angle Offset", "Rotation offset for group 1."));
                }

                expandSFCoronaRays2Section = EditorGUILayout.Foldout(expandSFCoronaRays2Section, new GUIContent("Corona Rays Group 2", "Customize appearance of solar corona rays group 2."), sectionHeaderIndentedStyle);
                if (expandSFCoronaRays2Section)
                {
                    EditorGUILayout.PropertyField(_sunFlaresCoronaRays2Length, new GUIContent("   Length", "Length of solar corona rays group 2."));
                    EditorGUILayout.PropertyField(_sunFlaresCoronaRays2Streaks, new GUIContent("   Streaks", "Number of streaks for group 2."));
                    EditorGUILayout.PropertyField(_sunFlaresCoronaRays2Spread, new GUIContent("   Spread", "Light spread factor for group 2."));
                    EditorGUILayout.PropertyField(_sunFlaresCoronaRays2AngleOffset, new GUIContent("   Angle Offset", "Rotation offset for group 2."));
                }

                expandSFGhosts1Section = EditorGUILayout.Foldout(expandSFGhosts1Section, new GUIContent("Ghost 1", "Customize appearance of lens ghost #1"), sectionHeaderIndentedStyle);
                if (expandSFGhosts1Section)
                {
                    EditorGUILayout.PropertyField(_sunFlaresGhosts1Size, new GUIContent("   Size"));
                    EditorGUILayout.PropertyField(_sunFlaresGhosts1Offset, new GUIContent("   Offset"));
                    EditorGUILayout.PropertyField(_sunFlaresGhosts1Brightness, new GUIContent("   Brightness"));
                }

                expandSFGhosts2Section = EditorGUILayout.Foldout(expandSFGhosts2Section, new GUIContent("Ghost 2", "Customize appearance of lens ghost #2"), sectionHeaderIndentedStyle);
                if (expandSFGhosts2Section)
                {
                    EditorGUILayout.PropertyField(_sunFlaresGhosts2Size, new GUIContent("   Size"));
                    EditorGUILayout.PropertyField(_sunFlaresGhosts2Offset, new GUIContent("   Offset"));
                    EditorGUILayout.PropertyField(_sunFlaresGhosts2Brightness, new GUIContent("   Brightness"));
                }

                expandSFGhosts3Section = EditorGUILayout.Foldout(expandSFGhosts3Section, new GUIContent("Ghost 3", "Customize appearance of lens ghost #3"), sectionHeaderIndentedStyle);
                if (expandSFGhosts3Section)
                {
                    EditorGUILayout.PropertyField(_sunFlaresGhosts3Size, new GUIContent("   Size"));
                    EditorGUILayout.PropertyField(_sunFlaresGhosts3Offset, new GUIContent("   Offset"));
                    EditorGUILayout.PropertyField(_sunFlaresGhosts3Brightness, new GUIContent("   Brightness"));
                }

                expandSFGhosts4Section = EditorGUILayout.Foldout(expandSFGhosts4Section, new GUIContent("Ghost 4", "Customize appearance of lens ghost #4"), sectionHeaderIndentedStyle);
                if (expandSFGhosts4Section)
                {
                    EditorGUILayout.PropertyField(_sunFlaresGhosts4Size, new GUIContent("   Size"));
                    EditorGUILayout.PropertyField(_sunFlaresGhosts4Offset, new GUIContent("   Offset"));
                    EditorGUILayout.PropertyField(_sunFlaresGhosts4Brightness, new GUIContent("   Brightness"));
                }

                expandSFHaloSection = EditorGUILayout.Foldout(expandSFHaloSection, new GUIContent("Halo", "Customize appearance of halo."), sectionHeaderIndentedStyle);
                if (expandSFHaloSection)
                {
                    EditorGUILayout.PropertyField(_sunFlaresHaloOffset, new GUIContent("   Offset"));
                    EditorGUILayout.PropertyField(_sunFlaresHaloAmplitude, new GUIContent("   Amplitude"));
                    EditorGUILayout.PropertyField(_sunFlaresHaloIntensity, new GUIContent("   Intensity"));
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                labelStyle = _lensDirt.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandDirtSection = EditorGUILayout.Foldout(expandDirtSection, new GUIContent("Lens Dirt", "Enables lens dirt effect which intensifies when looking to a light (uses the nearest light to camera). You can assign other dirt textures directly to the shader material with name 'Beautify'."), labelStyle);
                EditorGUILayout.EndHorizontal();
                _lensDirt.boolValue = EditorGUILayout.Toggle(_lensDirt.boolValue);
                if (expandDirtSection)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(_lensDirtTexture, new GUIContent("   Dirt Texture", "Texture used for the lens dirt effect."));
                    if (GUILayout.Button("?", GUILayout.Width(20)))
                    {
                        EditorUtility.DisplayDialog("Lens Dirt Texture", "You can find additional lens dirt textures inside \nBeautify/Resources/Textures folder.", "Ok");
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(_lensDirtThreshold, new GUIContent("   Threshold", "This slider controls the visibility of lens dirt. A high value will make lens dirt only visible when looking directly towards a light source. A lower value will make lens dirt visible all time."));
                    EditorGUILayout.PropertyField(_lensDirtIntensity, new GUIContent("   Intensity", "This slider controls the maximum brightness of lens dirt effect."));

                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
            labelStyle = _depthOfField.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
            expandDoFSection = EditorGUILayout.Foldout(expandDoFSection, new GUIContent("Depth of Field", "Blurs the image based on distance to focus point."), labelStyle);
            EditorGUILayout.EndHorizontal();
            _depthOfField.boolValue = EditorGUILayout.Toggle(_depthOfField.boolValue);
            EditorGUILayout.EndHorizontal();
            if (expandDoFSection)
            {
                EditorGUILayout.PropertyField(_depthOfFieldAutofocus, new GUIContent("   Autofocus", "Automatically focus the object in front of camera."));

                if (_depthOfFieldAutofocus.boolValue)
                {
                    EditorGUILayout.PropertyField(_depthOfFieldAutofocusLayerMask, new GUIContent("      Layer Mask", "Select which layers can be used for autofocus option."));
                    EditorGUILayout.PropertyField(_depthOfFieldAutofocusMinDistance, new GUIContent("      Min Distance", "Minimum distance accepted for any focused object."));
                    EditorGUILayout.PropertyField(_depthOfFieldAutofocusMaxDistance, new GUIContent("      Max Distance", "Maximum distance accepted for any focused object."));
                }
                else
                {
                    EditorGUILayout.PropertyField(_depthOfFieldTargetFocus, new GUIContent("   Focus Target", "Dynamically focus target."));

                    if (_depthOfFieldTargetFocus.objectReferenceValue == null)
                    {
                        EditorGUILayout.PropertyField(_depthOfFieldDistance, new GUIContent("   Focus Distance", "Distance to focus point."));
                    }
                }
                EditorGUILayout.PropertyField(_depthOfFieldFocusSpeed, new GUIContent("   Focus Speed", "1=immediate focus on distance or target."));
                EditorGUILayout.PropertyField(_depthOfFieldFocalLength, new GUIContent("   Focal Length", "Focal length of the virtual lens."));
                EditorGUILayout.PropertyField(_depthOfFieldAperture, new GUIContent("   Aperture", "Diameter of the aperture (mm)."));
                EditorGUILayout.PropertyField(_depthOfFieldForegroundBlur, new GUIContent("   Foreground Blur", "Enables blur in front of focus object."));
                if (_depthOfFieldForegroundBlur.boolValue)
                {
                    EditorGUILayout.PropertyField(_depthOfFieldForegroundDistance, new GUIContent("      Offset", "Distance from focus plane for foreground blur."));
                    EditorGUILayout.PropertyField(_depthOfFieldForegroundBlurHQ, new GUIContent("      High Quality", "Improves depth of field foreground blur."));
                }
                EditorGUILayout.PropertyField(_depthOfFieldMaxBrightness, new GUIContent("   Clamp Brightness", "Clamps maximum pixel brightness to prevent out of range bright spots."));
                EditorGUILayout.PropertyField(_depthOfFieldBokeh, new GUIContent("   Bokeh", "Bright spots will be augmented in defocused areas."));
                if (_depthOfFieldBokeh.boolValue)
                {
                    EditorGUILayout.PropertyField(_depthOfFieldBokehThreshold, new GUIContent("      Threshold", "Brightness threshold to be considered as 'bright' spot."));
                    EditorGUILayout.PropertyField(_depthOfFieldBokehIntensity, new GUIContent("      Intensity", "Intensity multiplier for bright spots in defocused areas."));
                }
                EditorGUILayout.PropertyField(_depthOfFieldDownsampling, new GUIContent("   Downsampling", "Reduces screen buffer size to improve performance."));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_depthOfFieldMaxSamples, new GUIContent("   Sample Count", "Determines the maximum number of samples to be gathered in the effect."));
                GUILayout.Label("(" + ((_depthOfFieldMaxSamples.intValue - 1) * 2 + 1) + " samples)", GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("   Exclusion Mask", "Select which layers will always remain in focus."), GUILayout.Width(120));
                EditorGUILayout.PropertyField(_depthOfFieldExclusionLayerMask, GUIContent.none);
                if (_depthOfFieldExclusionLayerMask.intValue != 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(_depthOfFieldExclusionBias, new GUIContent("       Depth Bias", "Depth offset for the exclusion mask computation."));
                    EditorGUILayout.PropertyField(_depthOfFieldExclusionLayerMaskDownsampling, new GUIContent("       Downsampling", "This value is added to the DoF downsampling factor for the exclusion mask creation. Increase to improve performance."));
                    EditorGUILayout.PropertyField(_depthOfFieldFilterMode, new GUIContent("       Filter Mode", "Texture filter mode."));
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("   Transparency", "Enable transparency support."), GUILayout.Width(120));
            if (_depthOfFieldExclusionLayerMask.intValue != 0)
            {
                GUI.enabled = false;
                EditorGUILayout.Toggle(true);
                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.PropertyField(_depthOfFieldTransparencySupport, GUIContent.none);
            }
            EditorGUILayout.EndHorizontal();

            if (_depthOfFieldTransparencySupport.boolValue || _depthOfFieldExclusionLayerMask.intValue != 0)
            {
                EditorGUILayout.PropertyField(_depthOfFieldTransparencySupportDownsampling, new GUIContent("       Downsampling", "This value is added to the DoF downsampling factor for the transparency mask creation. Increase to improve performance."));
            }


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
            labelStyle = _eyeAdaptation.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
            expandEASection = EditorGUILayout.Foldout(expandEASection, new GUIContent("Eye Adaptation", "Enables eye adaptation effect. Simulates retina response to quick luminance changes in the scene."), labelStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_eyeAdaptation, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            if (expandEASection)
            {
                EditorGUILayout.PropertyField(_eyeAdaptationMinExposure, new GUIContent("   Min Exposure"));
                EditorGUILayout.PropertyField(_eyeAdaptationMaxExposure, new GUIContent("   Max Exposure"));
                EditorGUILayout.PropertyField(_eyeAdaptationSpeedToDark, new GUIContent("   Dark Adapt Speed"));
                EditorGUILayout.PropertyField(_eyeAdaptationSpeedToLight, new GUIContent("   Light Adapt Speed"));
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
            labelStyle = _purkinje.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
            expandPurkinjeSection = EditorGUILayout.Foldout(expandPurkinjeSection, new GUIContent("Purkinje", "Simulates achromatic vision plus spectrum shift to blue in the dark."), labelStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_purkinje, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            if (expandPurkinjeSection)
            {
                EditorGUILayout.PropertyField(_purkinjeAmount, new GUIContent("   Shift Amount", "Spectrum shift to blue. A value of zero will not shift colors and stay in grayscale."));
                EditorGUILayout.PropertyField(_purkinjeLuminanceThreshold, new GUIContent("   Threshold", "Increase this value to augment the purkinje effect (applies to higher luminance levels)."));
            }

            EditorGUILayout.Separator();
            DrawLabel("Artistic Choices");


            if (_vignetting.boolValue || _frame.boolValue || _outline.boolValue || _nightVision.boolValue || _thermalVision.boolValue)
            {
                EditorGUILayout.HelpBox("Customize the effects below using color picker. Alpha has special meaning depending on effect. Read the tooltip moving the mouse over the effect name.", MessageType.Info);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
            labelStyle = _vignetting.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
            expandVignettingSection = EditorGUILayout.Foldout(expandVignettingSection, new GUIContent("Vignetting", "Enables colored vignetting effect. Color alpha specifies intensity of effect."), labelStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_vignetting, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            if (expandVignettingSection)
            {
                EditorGUILayout.PropertyField(_vignettingColor, new GUIContent("   Color", "The color for the vignetting effect. Alpha specifies intensity of effect."));
                EditorGUILayout.PropertyField(_vignettingFade, new GUIContent("   Fade Out", "Fade out effect to the vignette color."));
                EditorGUILayout.PropertyField(_vignettingBlink, new GUIContent("   Blink", "Manages blink effect for testing purposes. At runtime use Beautify.instance.Blink to produce a blink effect."));
                EditorGUILayout.PropertyField(_vignettingCircularShape, new GUIContent("   Circular Shape", "Ignores screen aspect ratio showing a circular shape."));
                if (!_vignettingCircularShape.boolValue)
                {
                    EditorGUILayout.PropertyField(_vignettingAspectRatio, new GUIContent("   Aspect Ratio", "Custom aspect ratio."));
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("   Mask Texture", "Texture used for masking vignette effect. Alpha channel will be used to determine which areas remain untouched (1=color untouched, less than 1=vignette effect)"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(_vignettingMask, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
            labelStyle = _frame.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
            expandFrameSection = EditorGUILayout.Foldout(expandFrameSection, new GUIContent("Frame", "Enables colored frame effect. Color alpha specifies intensity of effect."), labelStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_frame, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            if (expandFrameSection)
            {
                EditorGUILayout.PropertyField(_frameColor, new GUIContent("   Color", "The color for the frame effect. Alpha specifies intensity of effect."));

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("   Mask Texture", "Texture used for frame effect."), GUILayout.Width(120));
                EditorGUILayout.PropertyField(_frameMask, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
            labelStyle = _outline.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
            expandOutlineSection = EditorGUILayout.Foldout(expandOutlineSection, new GUIContent("Outline", "Enables outline (edge detection) effect. Color alpha specifies edge detection threshold (reference values: 0.8 for depth, 0.35 for Sobel)."), labelStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_outline, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            if (expandOutlineSection)
            {
                EditorGUILayout.PropertyField(_outlineColor, new GUIContent("   Color", "The color for the outline. Alpha specifies edge detection threshold."));
            }

            EditorGUILayout.BeginHorizontal();
            labelStyle = _nightVision.boolValue ? labelBoldStyle : labelNormalStyle;
            GUILayout.Label(new GUIContent("Night Vision", "Enables night vision effect. Color alpha controls intensity. For a better result, enable Vignetting and set its color to (0,0,0,32)."), labelStyle, GUILayout.Width(120));
            EditorGUILayout.PropertyField(_nightVision, GUIContent.none);
            if (_nightVision.boolValue)
            {
                GUILayout.Label(new GUIContent("Color", "The color for the night vision effect. Alpha controls intensity."), GUILayout.Width(40));
                EditorGUILayout.PropertyField(_nightVisionColor, GUIContent.none);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            labelStyle = _thermalVision.boolValue ? labelBoldStyle : labelNormalStyle;
            GUILayout.Label(new GUIContent("Thermal Vision", "Enables thermal vision effect."), labelStyle, GUILayout.Width(120));
            EditorGUILayout.PropertyField(_thermalVision, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
            labelStyle = _blur.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
            expandBlurSection = EditorGUILayout.Foldout(expandBlurSection, new GUIContent("Blur", "Enables final blur effect."), labelStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_blur, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            if (expandBlurSection)
            {
                EditorGUILayout.PropertyField(_blurIntensity, new GUIContent("   Intensity"));
            }
            EditorGUILayout.PropertyField(_pixelateAmount, new GUIContent("Pixelate", "Creates a retro/pixelated effect."));
            EditorGUILayout.PropertyField(_pixelateDownscale, new GUIContent("   Downscale", "If enabled, makes the camera render to a smaller render target. This option increases performance while producing the pixelation effect. If disabled, the pixelation effect is applied to the normal framebuffer."));

            if (serializedObject.ApplyModifiedProperties() || (Event.current.type == EventType.ValidateCommand &&
                Event.current.commandName == "UndoRedoPerformed"))
            {
                // Triggers profile reload on all Beautify scripts
                Beautify[] bb = FindObjectsOfType<Beautify>();
                for (int t = 0; t < targets.Length; t++)
                {
                    BeautifyProfile profile = (BeautifyProfile)targets[t];
                    for (int k = 0; k < bb.Length; k++)
                    {
                        if (bb[k] != null && bb[k].profile == profile)
                        {
                            profile.Load(bb[k]);
                        }
                    }
                }
                EditorUtility.SetDirty(target);
            }
        }

        void DrawLabel(string s)
        {
            if (titleLabelStyle == null)
            {
                GUIStyle skurikenModuleTitleStyle = "ShurikenModuleTitle";
                titleLabelStyle = new GUIStyle(skurikenModuleTitleStyle);
                titleLabelStyle.contentOffset = new Vector2(5f, -2f);
                titleLabelStyle.normal.textColor = titleColor;
                titleLabelStyle.fixedHeight = 22;
                titleLabelStyle.fontStyle = FontStyle.Bold;
            }

            GUILayout.Label(s, titleLabelStyle);
        }

        Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            TextureFormat tf = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat) ? TextureFormat.RGBAFloat : TextureFormat.RGBA32;
            Texture2D result = new Texture2D(width, height, tf, false);
            result.hideFlags = HideFlags.DontSave;
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

    }

}
