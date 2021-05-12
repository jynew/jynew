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
    [CustomEditor(typeof(Beautify))]
    public class BeautifyEffectInspector : Editor
    {
        const string PRAGMA_COMMENT_MARK = "// Edited by Shader Control: ";
        const string PRAGMA_DISABLED_MARK = "// Disabled by Shader Control: ";
        const string PRAGMA_MULTICOMPILE = "#pragma multi_compile ";
        const string PRAGMA_UNDERSCORE = "__ ";
        Beautify _effect;
        Texture2D _headerTexture;
        static GUIStyle titleLabelStyle, labelBoldStyle, labelNormalStyle, sectionHeaderBoldStyle, sectionHeaderNormalStyle, sectionHeaderIndentedStyle;
        static GUIStyle buttonNormalStyle, buttonPressedStyle, blackBack;
        static Color titleColor;
        bool expandSharpenSection, expandBloomSection, expandAFSection;
        bool expandSFSection, expandSFCoronaRays1Section, expandSFCoronaRays2Section, expandSFGhosts1Section, expandSFGhosts2Section, expandSFGhosts3Section, expandSFGhosts4Section, expandSFHaloSection;
        bool expandDirtSection, expandDoFSection;
        bool expandEASection, expandPurkinjeSection, expandVignettingSection, expandDitherSection, expandFrameSection;
        bool expandOutlineSection, expandLUTSection, expandBlurSection;
        bool toggleOptimizeBuild;
        BeautifySAdvancedOptionsInfo shaderAdvancedOptionsInfo;

        GUIStyle foldoutBold, foldoutNormal, foldOutIndented;
        List<BeautifySInfo> shaders;
        bool layer1, layer2, layer3, layer4, layer5, layer6;

        SerializedProperty _quality, _preset, _compareMode, _compareLineAngle, _compareLineWidth, _profile;
        SerializedProperty _sharpen, _sharpenMinDepth, _sharpenMaxDepth, _sharpenDepthThreshold, _sharpenRelaxation, _sharpenClamp, _sharpenMotionSensibility;
        SerializedProperty _dither, _ditherDepth;
        SerializedProperty _tonemap, _brightness, _saturate, _daltonize, _tintColor, _contrast;
        SerializedProperty _bloom, _bloomDebug, _bloomCullingMask, _bloomLayerMaskDownsampling, _bloomLayerZBias, _bloomIntensity, _bloomThreshold, _bloomDepthAtten, _bloomMaxBrightness;
        SerializedProperty _bloomAntiflicker, _bloomUltra, _bloomCustomize, _bloomWeight0, _bloomWeight1, _bloomWeight2, _bloomWeight3, _bloomWeight4, _bloomWeight5;
        SerializedProperty _bloomBoost0, _bloomBoost1, _bloomBoost2, _bloomBoost3, _bloomBoost4, _bloomBoost5, _bloomBlur;
        SerializedProperty _anamorphicFlares, _anamorphicFlaresIntensity, _anamorphicFlaresThreshold, _anamorphicFlaresSpread, _anamorphicFlaresVertical, _anamorphicFlaresTint;
        SerializedProperty _anamorphicFlaresAntiflicker, _anamorphicFlaresUltra, _anamorphicFlaresBlur;
        SerializedProperty _sunFlares, _sunFlaresLayerMask, _sun, _sunFlaresIntensity, _sunFlaresTint, _sunFlaresDownsampling, _sunFlaresSunIntensity, _sunFlaresSunDiskSize, _sunFlaresSunRayDiffractionIntensity;
        SerializedProperty _sunFlaresSunRayDiffractionThreshold, _sunFlaresSolarWindSpeed, _sunFlaresRotationDeadZone;
        SerializedProperty _sunFlaresCoronaRays1Length, _sunFlaresCoronaRays1Streaks, _sunFlaresCoronaRays1Spread, _sunFlaresCoronaRays1AngleOffset;
        SerializedProperty _sunFlaresCoronaRays2Length, _sunFlaresCoronaRays2Streaks, _sunFlaresCoronaRays2Spread, _sunFlaresCoronaRays2AngleOffset;
        SerializedProperty _sunFlaresGhosts1Size, _sunFlaresGhosts1Offset, _sunFlaresGhosts1Brightness;
        SerializedProperty _sunFlaresGhosts2Size, _sunFlaresGhosts2Offset, _sunFlaresGhosts2Brightness;
        SerializedProperty _sunFlaresGhosts3Size, _sunFlaresGhosts3Offset, _sunFlaresGhosts3Brightness;
        SerializedProperty _sunFlaresGhosts4Size, _sunFlaresGhosts4Offset, _sunFlaresGhosts4Brightness;
        SerializedProperty _sunFlaresHaloOffset, _sunFlaresHaloAmplitude, _sunFlaresHaloIntensity;
        SerializedProperty _lensDirt, _lensDirtTexture, _lensDirtThreshold, _lensDirtIntensity;
        SerializedProperty _depthOfField, _depthOfFieldTransparencySupport, _depthOfFieldExclusionLayerMask, _depthOfFieldDebug, _depthOfFieldAutofocus, _depthOfFieldAutofocusMinDistance, _depthOfFieldAutofocusMaxDistance, _depthOfFieldAutofocusLayerMask;
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
        bool profileChanges;


        void OnEnable()
        {
            titleColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);
            _headerTexture = Resources.Load<Texture2D>("beautifyHeader");
            _effect = (Beautify)target;
            blackBack = new GUIStyle();
            blackBack.normal.background = MakeTex(4, 4, Color.black);
            expandSharpenSection = EditorPrefs.GetBool("BeautifySharpenSection", false);
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
            expandLUTSection = EditorPrefs.GetBool("BeautifyLUTSection", false);
            expandDitherSection = EditorPrefs.GetBool("BeautifyDitherSection", false);
            expandBlurSection = EditorPrefs.GetBool("BeautifyBlurSection", false);
            ScanKeywords();
            ScanAdvancedOptions();

            // Setup property fields
            _quality = serializedObject.FindProperty("_quality");
            _preset = serializedObject.FindProperty("_preset");
            _compareMode = serializedObject.FindProperty("_compareMode");
            _compareLineAngle = serializedObject.FindProperty("_compareLineAngle");
            _compareLineWidth = serializedObject.FindProperty("_compareLineWidth");
            _profile = serializedObject.FindProperty("_profile");

            // Image enhancements
            _sharpen = serializedObject.FindProperty("_sharpen");
            _sharpenMinDepth = serializedObject.FindProperty("_sharpenMinDepth");
            _sharpenMaxDepth = serializedObject.FindProperty("_sharpenMaxDepth");
            _sharpenDepthThreshold = serializedObject.FindProperty("_sharpenDepthThreshold");
            _sharpenRelaxation = serializedObject.FindProperty("_sharpenRelaxation");
            _sharpenClamp = serializedObject.FindProperty("_sharpenClamp");
            _sharpenMotionSensibility = serializedObject.FindProperty("_sharpenMotionSensibility");
            _dither = serializedObject.FindProperty("_dither");
            _ditherDepth = serializedObject.FindProperty("_ditherDepth");

            // Tonemapping & color grading
            _tonemap = serializedObject.FindProperty("_tonemap");
            _brightness = serializedObject.FindProperty("_brightness");
            _saturate = serializedObject.FindProperty("_saturate");
            _daltonize = serializedObject.FindProperty("_daltonize");
            _tintColor = serializedObject.FindProperty("_tintColor");
            _contrast = serializedObject.FindProperty("_contrast");

            // Bloom
            _bloom = serializedObject.FindProperty("_bloom");
            _bloomDebug = serializedObject.FindProperty("_bloomDebug");
            _bloomCullingMask = serializedObject.FindProperty("_bloomCullingMask");
            _bloomLayerMaskDownsampling = serializedObject.FindProperty("_bloomLayerMaskDownsampling");
            _bloomLayerZBias = serializedObject.FindProperty("_bloomLayerZBias");
            _bloomIntensity = serializedObject.FindProperty("_bloomIntensity");
            _bloomThreshold = serializedObject.FindProperty("_bloomThreshold");
            _bloomDepthAtten = serializedObject.FindProperty("_bloomDepthAtten");
            _bloomMaxBrightness = serializedObject.FindProperty("_bloomMaxBrightness");
            _bloomAntiflicker = serializedObject.FindProperty("_bloomAntiflicker");
            _bloomUltra = serializedObject.FindProperty("_bloomUltra");
            _bloomCustomize = serializedObject.FindProperty("_bloomCustomize");
            _bloomWeight0 = serializedObject.FindProperty("_bloomWeight0");
            _bloomWeight1 = serializedObject.FindProperty("_bloomWeight1");
            _bloomWeight2 = serializedObject.FindProperty("_bloomWeight2");
            _bloomWeight3 = serializedObject.FindProperty("_bloomWeight3");
            _bloomWeight4 = serializedObject.FindProperty("_bloomWeight4");
            _bloomWeight5 = serializedObject.FindProperty("_bloomWeight5");
            _bloomBoost0 = serializedObject.FindProperty("_bloomBoost0");
            _bloomBoost1 = serializedObject.FindProperty("_bloomBoost1");
            _bloomBoost2 = serializedObject.FindProperty("_bloomBoost2");
            _bloomBoost3 = serializedObject.FindProperty("_bloomBoost3");
            _bloomBoost4 = serializedObject.FindProperty("_bloomBoost4");
            _bloomBoost5 = serializedObject.FindProperty("_bloomBoost5");
            _bloomBlur = serializedObject.FindProperty("_bloomBlur");

            // Anamorphic flares
            _anamorphicFlares = serializedObject.FindProperty("_anamorphicFlares");
            _anamorphicFlaresIntensity = serializedObject.FindProperty("_anamorphicFlaresIntensity");
            _anamorphicFlaresThreshold = serializedObject.FindProperty("_anamorphicFlaresThreshold");
            _anamorphicFlaresSpread = serializedObject.FindProperty("_anamorphicFlaresSpread");
            _anamorphicFlaresVertical = serializedObject.FindProperty("_anamorphicFlaresVertical");
            _anamorphicFlaresTint = serializedObject.FindProperty("_anamorphicFlaresTint");
            _anamorphicFlaresAntiflicker = serializedObject.FindProperty("_anamorphicFlaresAntiflicker");
            _anamorphicFlaresUltra = serializedObject.FindProperty("_anamorphicFlaresUltra");
            _anamorphicFlaresBlur = serializedObject.FindProperty("_anamorphicFlaresBlur");

            // Sun flares
            _sunFlares = serializedObject.FindProperty("_sunFlares");
            _sun = serializedObject.FindProperty("_sun");
            _sunFlaresLayerMask = serializedObject.FindProperty("_sunFlaresLayerMask");
            _sunFlaresIntensity = serializedObject.FindProperty("_sunFlaresIntensity");
            _sunFlaresTint = serializedObject.FindProperty("_sunFlaresTint");
            _sunFlaresDownsampling = serializedObject.FindProperty("_sunFlaresDownsampling");
            _sunFlaresSunIntensity = serializedObject.FindProperty("_sunFlaresSunIntensity");
            _sunFlaresSunDiskSize = serializedObject.FindProperty("_sunFlaresSunDiskSize");
            _sunFlaresSunRayDiffractionIntensity = serializedObject.FindProperty("_sunFlaresSunRayDiffractionIntensity");
            _sunFlaresSunRayDiffractionThreshold = serializedObject.FindProperty("_sunFlaresSunRayDiffractionThreshold");
            _sunFlaresSolarWindSpeed = serializedObject.FindProperty("_sunFlaresSolarWindSpeed");
            _sunFlaresRotationDeadZone = serializedObject.FindProperty("_sunFlaresRotationDeadZone");
            _sunFlaresCoronaRays1Length = serializedObject.FindProperty("_sunFlaresCoronaRays1Length");
            _sunFlaresCoronaRays1Streaks = serializedObject.FindProperty("_sunFlaresCoronaRays1Streaks");
            _sunFlaresCoronaRays1Spread = serializedObject.FindProperty("_sunFlaresCoronaRays1Spread");
            _sunFlaresCoronaRays1AngleOffset = serializedObject.FindProperty("_sunFlaresCoronaRays1AngleOffset");
            _sunFlaresCoronaRays2Length = serializedObject.FindProperty("_sunFlaresCoronaRays2Length");
            _sunFlaresCoronaRays2Streaks = serializedObject.FindProperty("_sunFlaresCoronaRays2Streaks");
            _sunFlaresCoronaRays2Spread = serializedObject.FindProperty("_sunFlaresCoronaRays2Spread");
            _sunFlaresCoronaRays2AngleOffset = serializedObject.FindProperty("_sunFlaresCoronaRays2AngleOffset");
            _sunFlaresGhosts1Size = serializedObject.FindProperty("_sunFlaresGhosts1Size");
            _sunFlaresGhosts1Offset = serializedObject.FindProperty("_sunFlaresGhosts1Offset");
            _sunFlaresGhosts1Brightness = serializedObject.FindProperty("_sunFlaresGhosts1Brightness");
            _sunFlaresGhosts2Size = serializedObject.FindProperty("_sunFlaresGhosts2Size");
            _sunFlaresGhosts2Offset = serializedObject.FindProperty("_sunFlaresGhosts2Offset");
            _sunFlaresGhosts2Brightness = serializedObject.FindProperty("_sunFlaresGhosts2Brightness");
            _sunFlaresGhosts3Size = serializedObject.FindProperty("_sunFlaresGhosts3Size");
            _sunFlaresGhosts3Offset = serializedObject.FindProperty("_sunFlaresGhosts3Offset");
            _sunFlaresGhosts3Brightness = serializedObject.FindProperty("_sunFlaresGhosts3Brightness");
            _sunFlaresGhosts4Size = serializedObject.FindProperty("_sunFlaresGhosts4Size");
            _sunFlaresGhosts4Offset = serializedObject.FindProperty("_sunFlaresGhosts4Offset");
            _sunFlaresGhosts4Brightness = serializedObject.FindProperty("_sunFlaresGhosts4Brightness");
            _sunFlaresHaloOffset = serializedObject.FindProperty("_sunFlaresHaloOffset");
            _sunFlaresHaloAmplitude = serializedObject.FindProperty("_sunFlaresHaloAmplitude");
            _sunFlaresHaloIntensity = serializedObject.FindProperty("_sunFlaresHaloIntensity");

            // Lens Dirt
            _lensDirt = serializedObject.FindProperty("_lensDirt");
            _lensDirtTexture = serializedObject.FindProperty("_lensDirtTexture");
            _lensDirtThreshold = serializedObject.FindProperty("_lensDirtThreshold");
            _lensDirtIntensity = serializedObject.FindProperty("_lensDirtIntensity");

            // Depth of Field
            _depthOfField = serializedObject.FindProperty("_depthOfField");
            _depthOfFieldTransparencySupport = serializedObject.FindProperty("_depthOfFieldTransparencySupport");
            _depthOfFieldExclusionLayerMask = serializedObject.FindProperty("_depthOfFieldExclusionLayerMask");
            _depthOfFieldDebug = serializedObject.FindProperty("_depthOfFieldDebug");
            _depthOfFieldAutofocus = serializedObject.FindProperty("_depthOfFieldAutofocus");
            _depthOfFieldAutofocusMinDistance = serializedObject.FindProperty("_depthOfFieldAutofocusMinDistance");
            _depthOfFieldAutofocusMaxDistance = serializedObject.FindProperty("_depthOfFieldAutofocusMaxDistance");
            _depthOfFieldAutofocusLayerMask = serializedObject.FindProperty("_depthOfFieldAutofocusLayerMask");
            _depthOfFieldTargetFocus = serializedObject.FindProperty("_depthOfFieldTargetFocus");
            _depthOfFieldDistance = serializedObject.FindProperty("_depthOfFieldDistance");
            _depthOfFieldFocusSpeed = serializedObject.FindProperty("_depthOfFieldFocusSpeed");
            _depthOfFieldFocalLength = serializedObject.FindProperty("_depthOfFieldFocalLength");
            _depthOfFieldAperture = serializedObject.FindProperty("_depthOfFieldAperture");
            _depthOfFieldForegroundBlur = serializedObject.FindProperty("_depthOfFieldForegroundBlur");
            _depthOfFieldForegroundBlurHQ = serializedObject.FindProperty("_depthOfFieldForegroundBlurHQ");
            _depthOfFieldForegroundDistance = serializedObject.FindProperty("_depthOfFieldForegroundDistance");
            _depthOfFieldMaxBrightness = serializedObject.FindProperty("_depthOfFieldMaxBrightness");
            _depthOfFieldBokeh = serializedObject.FindProperty("_depthOfFieldBokeh");
            _depthOfFieldBokehThreshold = serializedObject.FindProperty("_depthOfFieldBokehThreshold");
            _depthOfFieldBokehIntensity = serializedObject.FindProperty("_depthOfFieldBokehIntensity");
            _depthOfFieldDownsampling = serializedObject.FindProperty("_depthOfFieldDownsampling");
            _depthOfFieldMaxSamples = serializedObject.FindProperty("_depthOfFieldMaxSamples");
            _depthOfFieldExclusionBias = serializedObject.FindProperty("_depthOfFieldExclusionBias");
            _depthOfFieldExclusionLayerMaskDownsampling = serializedObject.FindProperty("_depthOfFieldExclusionLayerMaskDownsampling");
            _depthOfFieldTransparencySupportDownsampling = serializedObject.FindProperty("_depthOfFieldTransparencySupportDownsampling");
            _depthOfFieldFilterMode = serializedObject.FindProperty("_depthOfFieldFilterMode");

            // Eye adaptation
            _eyeAdaptation = serializedObject.FindProperty("_eyeAdaptation");
            _eyeAdaptationMinExposure = serializedObject.FindProperty("_eyeAdaptationMinExposure");
            _eyeAdaptationMaxExposure = serializedObject.FindProperty("_eyeAdaptationMaxExposure");
            _eyeAdaptationSpeedToDark = serializedObject.FindProperty("_eyeAdaptationSpeedToDark");
            _eyeAdaptationSpeedToLight = serializedObject.FindProperty("_eyeAdaptationSpeedToLight");

            // Purkinje shift
            _purkinje = serializedObject.FindProperty("_purkinje");
            _purkinjeAmount = serializedObject.FindProperty("_purkinjeAmount");
            _purkinjeLuminanceThreshold = serializedObject.FindProperty("_purkinjeLuminanceThreshold");

            // Vignetting
            _vignetting = serializedObject.FindProperty("_vignetting");
            _vignettingColor = serializedObject.FindProperty("_vignettingColor");
            _vignettingFade = serializedObject.FindProperty("_vignettingFade");
            _vignettingCircularShape = serializedObject.FindProperty("_vignettingCircularShape");
            _vignettingAspectRatio = serializedObject.FindProperty("_vignettingAspectRatio");
            _vignettingMask = serializedObject.FindProperty("_vignettingMask");
            _vignettingBlink = serializedObject.FindProperty("_vignettingBlink");

            // Frame
            _frame = serializedObject.FindProperty("_frame");
            _frameColor = serializedObject.FindProperty("_frameColor");
            _frameMask = serializedObject.FindProperty("_frameMask");

            // Outline
            _outline = serializedObject.FindProperty("_outline");
            _outlineColor = serializedObject.FindProperty("_outlineColor");

            // LUT
            _lut = serializedObject.FindProperty("_lut");
            _lutTexture = serializedObject.FindProperty("_lutTexture");
            _lutIntensity = serializedObject.FindProperty("_lutIntensity");

            // Night vision
            _nightVision = serializedObject.FindProperty("_nightVision");
            _nightVisionColor = serializedObject.FindProperty("_nightVisionColor");

            // Thermal vision
            _thermalVision = serializedObject.FindProperty("_thermalVision");

            // Blur behind UI
            _blur = serializedObject.FindProperty("_blur");
            _blurIntensity = serializedObject.FindProperty("_blurIntensity");

            // Pixelate
            _pixelateAmount = serializedObject.FindProperty("_pixelateAmount");
            _pixelateDownscale = serializedObject.FindProperty("_pixelateDownscale");
        }

        void OnDestroy()
        {
            // Restore folding sections state
            EditorPrefs.SetBool("BeautifySharpenSection", expandSharpenSection);
            EditorPrefs.SetBool("BeautifyBloomSection", expandBloomSection);
            EditorPrefs.SetBool("BeautifyAFSection", expandAFSection);
            EditorPrefs.SetBool("BeautifySFSection", expandSFSection);
            EditorPrefs.SetBool("BeautifySFCoronaRays1Section", expandSFCoronaRays1Section);
            EditorPrefs.SetBool("BeautifySFCoronaRays2Section", expandSFCoronaRays2Section);
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
            if (_effect == null)
                return;

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

            bool qualityChanged = false;
            bool profileChanged = false;

            // draw interface
            EditorGUIUtility.labelWidth = 125;
            EditorGUILayout.Separator();
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.BeginHorizontal(blackBack);
            GUILayout.Label(_headerTexture, GUILayout.ExpandWidth(true));
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.EndHorizontal();
            if (!_effect.enabled)
            {
                EditorGUILayout.HelpBox("Beautify disabled.", MessageType.Info);
            }
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            DrawLabel("General Settings");
            EditorGUILayout.EndHorizontal();

            //												#if UNITY_5_6_OR_NEWER
            //												serializedObject.UpdateIfRequiredOrScript ();
            //												#else
            //												serializedObject.UpdateIfDirtyOrScript ();
            //												#endif
            serializedObject.Update();

            bool generalSettingsChanged = false;
            EditorGUI.BeginChangeCheck();

            int prevQuality = _quality.intValue;
            EditorGUILayout.PropertyField(_quality, new GUIContent("Quality", "The best performance variant is simply less accurate but slightly faster. Can be changed at runtime."));
            qualityChanged = (_quality.intValue != prevQuality);

            EditorGUILayout.PropertyField(_preset, new GUIContent("Preset", "Quick configurations."));

            EditorGUILayout.BeginHorizontal();
            BeautifyProfile prevProfile = (BeautifyProfile)_profile.objectReferenceValue;
            EditorGUILayout.PropertyField(_profile, new GUIContent("Profile", "Create or load stored presets."));
            profileChanged = (_profile.objectReferenceValue != prevProfile);

            if (_effect.profile != null)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(130));
                if (GUILayout.Button(new GUIContent("Create", "Creates a new profile which is a copy of the current settings."), GUILayout.Width(60)))
                {
                    CreateProfile();
                    profileChanges = false;
                    GUIUtility.ExitGUI();
                    return;
                }
                if (GUILayout.Button(new GUIContent("Revert", "Reload profile settings."), GUILayout.Width(60)))
                {
                    _effect.profile.Load(_effect);
                    profileChanges = false;
                }
                if (!profileChanges)
                    GUI.enabled = false;
                if (GUILayout.Button(new GUIContent("Apply", "Updates profile configuration with changes in this inspector."), GUILayout.Width(60)))
                {
                    profileChanges = false;
                    _effect.profile.Save(_effect);
                    EditorUtility.SetDirty(_effect.profile);
                    RefreshProfiles();
                    GUIUtility.ExitGUI();
                    return;
                }
                GUI.enabled = true;
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Create", "Creates a new profile which is a copy of the current settings."), GUILayout.Width(60)))
                {
                    CreateProfile();
                    GUIUtility.ExitGUI();
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_compareMode, new GUIContent("Compare Mode", "Shows a side by side comparison."));
            if (GUILayout.Button(toggleOptimizeBuild ? "Hide Shader Options" : "Shader Options", toggleOptimizeBuild ? buttonPressedStyle : buttonNormalStyle, GUILayout.Width(toggleOptimizeBuild ? 140 : 110)))
            {
                toggleOptimizeBuild = !toggleOptimizeBuild;
                EditorGUIUtility.ExitGUI();
                return;
            }
            if (GUILayout.Button("Help", GUILayout.Width(50)))
            {
                EditorUtility.DisplayDialog("Help", "Beautify is a full-screen image processing effect that makes your scenes crisp, vivid and intense.\n\nMove the mouse over a setting for a short description or read the provided documentation (PDF) for details and tips.\n\nVisit kronnect.com's forum for support and questions.\n\nPlease rate Beautify on the Asset Store! Thanks.", "Ok");
            }
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                generalSettingsChanged = true;
            }

            BeautifySInfo shaderInfo = null;
            if (shaders.Count > 0)
            {
                shaderInfo = shaders[0];
            }

            if (toggleOptimizeBuild)
            {
                EditorGUILayout.Separator();
                DrawLabel("Shader Features");
                EditorGUILayout.HelpBox("SELECTED features can be toggled on/off at runtime.\nUNSELECTED features will NOT be included in the build, reducing keywords usage, compilation time and build size.", MessageType.Info);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Refresh", GUILayout.Width(60)))
                {
                    ScanKeywords();
                    EditorGUIUtility.ExitGUI();
                    return;
                }
                bool shaderChanged = false;
                for (int k = 0; k < shaders.Count; k++)
                {
                    if (shaders[k].pendingChanges)
                        shaderChanged = true;
                }
                if (!shaderChanged)
                    GUI.enabled = false;
                if (GUILayout.Button("Save Changes", GUILayout.Width(110)))
                {
                    UpdateShaders();
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                if (shaderInfo != null)
                {
                    bool firstColumn = true;
                    EditorGUILayout.BeginHorizontal();
                    int keywordCount = shaderInfo.keywords.Count;
                    for (int k = 0; k < keywordCount; k++)
                    {
                        SCKeyword keyword = shaderInfo.keywords[k];
                        if (keyword.isUnderscoreKeyword)
                            continue;
                        if (firstColumn)
                        {
                            EditorGUILayout.LabelField("", GUILayout.Width(10));
                        }
                        bool prevState = keyword.enabled;
                        keyword.enabled = EditorGUILayout.Toggle(prevState, GUILayout.Width(18));
                        if (prevState != keyword.enabled)
                        {
                            shaderInfo.pendingChanges = true;
                            EditorGUIUtility.ExitGUI();
                            return;
                        }
                        string keywordName = SCKeywordChecker.Translate(keyword.name);
                        EditorGUILayout.LabelField(keywordName, GUILayout.Width(140));
                        firstColumn = !firstColumn;
                        if (firstColumn)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Separator();

                if (shaderAdvancedOptionsInfo != null)
                {

                    DrawLabel("Advanced Options");
                    EditorGUILayout.HelpBox("Additional options that help fine-tune the shader performance or behaviour.", MessageType.Info);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Refresh", GUILayout.Width(60)))
                    {
                        ScanAdvancedOptions();
                        EditorGUIUtility.ExitGUI();
                        return;
                    }
                    if (!shaderAdvancedOptionsInfo.pendingChanges)
                        GUI.enabled = false;
                    if (GUILayout.Button("Save Changes", GUILayout.Width(110)))
                    {
                        shaderAdvancedOptionsInfo.UpdateAdvancedOptionsFile();
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                    int optionsCount = shaderAdvancedOptionsInfo.options.Length;
                    for (int k = 0; k < optionsCount; k++)
                    {
                        ShaderAdvancedOption option = shaderAdvancedOptionsInfo.options[k];
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(10));
                        bool prevState = option.enabled;
                        bool newState = EditorGUILayout.Toggle(prevState, GUILayout.Width(18));
                        if (prevState != newState)
                        {
                            shaderAdvancedOptionsInfo.options[k].enabled = newState;
                            shaderAdvancedOptionsInfo.pendingChanges = true;
                            EditorGUIUtility.ExitGUI();
                            return;
                        }
                        EditorGUILayout.LabelField(option.name);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(10));
                        EditorGUILayout.LabelField("", GUILayout.Width(18));
                        GUIStyle wrapStyle = new GUIStyle(GUI.skin.label);
                        wrapStyle.wordWrap = true;
                        EditorGUILayout.LabelField(option.description, wrapStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.Separator();

                return;
            }

            if (_compareMode.boolValue)
            {
                EditorGUILayout.PropertyField(_compareLineAngle, new GUIContent("   Angle", "Angle of the separator line."));
                EditorGUILayout.PropertyField(_compareLineWidth, new GUIContent("   Width", "Width of the separator line."));
            }

            if (shaderInfo != null && shaderInfo.enabledKeywordCount >= 17)
            {
                EditorGUILayout.HelpBox("Please remember to disable unwanted effects in Build Options before building your game!", MessageType.Warning);
            }

            EditorGUILayout.Separator();
            DrawLabel("Image Enhancement");

#if UNITY_5_6_OR_NEWER
            if (_effect.cameraEffect != null && !_effect.cameraEffect.allowHDR)
            {
#else
												if (_effect.cameraEffect != null && !_effect.cameraEffect.hdr) {
#endif
                EditorGUILayout.HelpBox("Some effects, like dither and bloom, works better with HDR enabled. Check your camera setting.", MessageType.Warning);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(121));
            expandSharpenSection = EditorGUILayout.Foldout(expandSharpenSection, new GUIContent("Sharpen", "Sharpen intensity."), sectionHeaderNormalStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_sharpen, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            if (expandSharpenSection)
            {
                if (_effect.cameraEffect != null && !_effect.cameraEffect.orthographic && _quality.intValue != (int)BEAUTIFY_QUALITY.Basic)
                {

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent("   Min/Max Depth", "Any pixel outside this depth range won't be affected by sharpen. Reduce range to create a depth-of-field-like effect."), GUILayout.Width(121));
                    float minDepth = _sharpenMinDepth.floatValue;
                    float maxDepth = _sharpenMaxDepth.floatValue;
                    EditorGUILayout.MinMaxSlider(ref minDepth, ref maxDepth, 0f, 1.1f);
                    _sharpenMinDepth.floatValue = minDepth;
                    _sharpenMaxDepth.floatValue = maxDepth;
                    EditorGUILayout.EndHorizontal();

                    if (_quality.intValue != (int)BEAUTIFY_QUALITY.BestPerformance)
                    {
                        EditorGUILayout.PropertyField(_sharpenDepthThreshold, new GUIContent("   Depth Threshold", "Reduces sharpen if depth difference around a pixel exceeds this value. Useful to prevent artifacts around wires or thin objects."));
                    }
                }
                EditorGUILayout.PropertyField(_sharpenRelaxation, new GUIContent("   Luminance Relax.", "Soften sharpen around a pixel with high contrast. Reduce this value to remove ghosting and protect fine drawings or wires over a flat surface."));
                EditorGUILayout.PropertyField(_sharpenClamp, new GUIContent("   Clamp", "Maximum pixel adjustment."));
                EditorGUILayout.PropertyField(_sharpenMotionSensibility, new GUIContent("   Motion Sensibility", "Increase to reduce sharpen to simulate a cheap motion blur and to reduce flickering when camera rotates or moves. This slider controls the amount of camera movement/rotation that contributes to sharpen reduction. Set this to 0 to disable this feature."));
            }

            if (_quality.intValue != (int)BEAUTIFY_QUALITY.Basic)
            {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(121));
                expandDitherSection = EditorGUILayout.Foldout(expandDitherSection, new GUIContent("Dither", "Simulates more colors than RGB quantization can produce. Removes banding artifacts in gradients, like skybox. This setting controls the dithering strength."), sectionHeaderNormalStyle);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(_dither, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                if (expandDitherSection)
                {
                    if (_effect.cameraEffect != null && !_effect.cameraEffect.orthographic)
                    {
                        EditorGUILayout.PropertyField(_ditherDepth, new GUIContent("   Min Depth", "Will only remove bands on pixels beyond this depth. Useful if you only want to remove sky banding (set this to 0.99)"));
                    }
                }
            }

            bool isLUTEnabled = isShaderFeatureEnabled(Beautify.SKW_LUT, false);
            if (!isLUTEnabled)
            {
                if (_lut.boolValue)
                {
                    _lut.boolValue = false;
                }
            }

            EditorGUILayout.Separator();
            DrawLabel("Tonemapping & Color Grading");

            if (_quality.intValue != (int)BEAUTIFY_QUALITY.BestQuality)
            {
                GUI.enabled = false;
            }

            int prevTonemap = _tonemap.intValue;
            EditorGUILayout.BeginHorizontal();
            GUIStyle labelStyle = _tonemap.intValue != (int)BEAUTIFY_TMO.Linear ? labelBoldStyle : labelNormalStyle;
            GUILayout.Label(new GUIContent("Tonemapping", "Converts high dynamic range colors into low dynamic range space according to a chosen tone mapping operator."), labelStyle, GUILayout.Width(121));
            if (isShaderFeatureEnabled(Beautify.SKW_TONEMAP_ACES))
            {
                EditorGUILayout.PropertyField(_tonemap, GUIContent.none);
            }
            else
            {
                _tonemap.intValue = (int)BEAUTIFY_TMO.Linear;
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            if (prevTonemap != _tonemap.intValue && _tonemap.intValue == (int)BEAUTIFY_TMO.ACES)
            {
                _saturate.floatValue = 0;
                _contrast.floatValue = 1f;
            }

            if (_tonemap.intValue != (int)BEAUTIFY_TMO.Linear)
            {
                EditorGUILayout.PropertyField(_brightness, new GUIContent("Exposure", "Exposure applied before tonemapping. Increase to make the image brighter."));
            }

            EditorGUILayout.PropertyField(_saturate, new GUIContent("Vibrance", "Improves pixels color depending on their saturation."));

            if (_quality.intValue != (int)BEAUTIFY_QUALITY.Basic)
            {
                EditorGUILayout.BeginHorizontal();
                labelStyle = _daltonize.floatValue > 0 ? labelBoldStyle : labelNormalStyle;
                GUILayout.Label(new GUIContent("Daltonize", "Similar to vibrance but mostly accentuate primary red, green and blue colors to compensate protanomaly (red deficiency), deuteranomaly (green deficiency) and tritanomaly (blue deficiency). This effect does not shift color hue hence it won't help completely red, green or blue color blindness. The effect will vary depending on each subject so this effect should be enabled on user demand."), labelStyle, GUILayout.Width(121));
                if (isShaderFeatureEnabled(Beautify.SKW_DALTONIZE))
                {
                    EditorGUILayout.PropertyField(_daltonize, GUIContent.none);
                }
                else
                {
                    _daltonize.floatValue = 0f;
                }
                EditorGUILayout.EndHorizontal();
                if (_daltonize.floatValue > 0 && _lut.boolValue)
                    EditorGUILayout.HelpBox("Daltonize disabled by LUT.", MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                labelStyle = _tintColor.colorValue.a > 0 ? labelBoldStyle : labelNormalStyle;
                GUILayout.Label(new GUIContent("Tint", "Blends image with an optional color. Alpha specifies intensity."), labelStyle, GUILayout.Width(121));
                EditorGUILayout.PropertyField(_tintColor, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                bool sepiaMode = shaderAdvancedOptionsInfo != null && shaderAdvancedOptionsInfo.GetAdvancedOptionState("BEAUTIFY_USE_PROCEDURAL_SEPIA");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                labelStyle = _lut.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandLUTSection = EditorGUILayout.Foldout(expandLUTSection, new GUIContent(sepiaMode ? "Sepia" : "LUT", "Enables Sepia or LUT based transformation effects."), labelStyle);
                EditorGUILayout.EndHorizontal();
                if (sepiaMode)
                {
                    EditorGUILayout.EndHorizontal();
                    if (expandLUTSection)
                    {
                        EditorGUILayout.PropertyField(_lutIntensity, new GUIContent("   Intensity"));
                    }
                }
                else
                {
                    if (testFeatureEnabled(isLUTEnabled))
                    {
                        EditorGUILayout.PropertyField(_lut, GUIContent.none);
                        EditorGUILayout.EndHorizontal();
                        if (expandLUTSection)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(_lutTexture, new GUIContent("   Texture"));
                            if (GUILayout.Button("Help", GUILayout.Width(50)))
                            {
                                EditorUtility.DisplayDialog("LUT Requirements", "LUT textures must be of 1024x32 dimensions.\nA sample Sepia LUT texture can be found in Beautify/Resources/Textures folder.\n\nEnsure the following import settings are set in your LUT textures:\n- Uncheck sRGM Texture (no gamma conversion)\n- No compression\n- Disable mip mapping\n- Aniso set to 0\n- Filtering set to Bilinear\n- Wrapping set to Clamp", "Ok");
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.PropertyField(_lutIntensity, new GUIContent("   Intensity"));

                            if (_quality.intValue == (int)BEAUTIFY_QUALITY.BestPerformance) {
                                bool isLutHQ = shaderAdvancedOptionsInfo != null && shaderAdvancedOptionsInfo.GetAdvancedOptionState("BEAUTIFY_BETTER_FASTER_LUT");
                                if (!isLutHQ) {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("", GUILayout.Width(5));
                                    EditorGUILayout.HelpBox("To improve LUT quality, enable Better Fast LUT in Shader Options.", MessageType.Info);
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            EditorGUILayout.PropertyField(_contrast, new GUIContent("Contrast", "Final image contrast adjustment. Allows you to create more vivid images."));

            if (_tonemap.intValue == (int)BEAUTIFY_TMO.Linear)
            {
                EditorGUILayout.PropertyField(_brightness, new GUIContent("Brightness", "Final image brightness adjustment."));
            }

            if (_quality.intValue != (int)BEAUTIFY_QUALITY.Basic)
            {

                bool isBloomEnabled = isShaderFeatureEnabled(Beautify.SKW_BLOOM, false);
                if (!isBloomEnabled)
                {
                    if (_bloom.boolValue)
                    {
                        _bloom.boolValue = false;
                    }
                    if (_anamorphicFlares.boolValue)
                    {
                        _anamorphicFlares.boolValue = false;
                    }
                    if (_sunFlares.boolValue)
                    {
                        _sunFlares.boolValue = false;
                    }
                }
                bool isLensDirtEnabled = isShaderFeatureEnabled(Beautify.SKW_DIRT, false);
                if (!isLensDirtEnabled)
                {
                    if (_lensDirt.boolValue)
                    {
                        _lensDirt.boolValue = false;
                    }
                }


                EditorGUILayout.Separator();
                DrawLabel("Lens & Lighting Effects");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(121));
                labelStyle = _bloom.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandBloomSection = EditorGUILayout.Foldout(expandBloomSection, new GUIContent("Bloom", "Produces fringes of light extending from the borders of bright areas, contributing to the illusion of an extremely bright light overwhelming the camera or eye capturing the scene."), labelStyle);
                EditorGUILayout.EndHorizontal();
                if (testFeatureEnabled(isBloomEnabled))
                {
                    EditorGUILayout.PropertyField(_bloom, GUIContent.none);
                    if (expandBloomSection)
                    {
                        if (_bloom.boolValue)
                        {
                            GUILayout.Label(new GUIContent("Debug", "Enable to see flares buffer."));
                            _bloomDebug.boolValue = EditorGUILayout.Toggle(_bloomDebug.boolValue, GUILayout.Width(40));
                        }
                        EditorGUILayout.EndHorizontal();
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
                            if (_quality.intValue == (int)BEAUTIFY_QUALITY.BestQuality)
                            {
                                EditorGUILayout.PropertyField(_bloomLayerMaskDownsampling, new GUIContent("   Mask Downsampling", "Bloom/anamorphic flares layer mask downsampling factor. Increase to improve performance."));
                            }
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
                        if (_quality.intValue == (int)BEAUTIFY_QUALITY.BestQuality)
                        {
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
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(_bloomBlur, new GUIContent("   Blur", "Adds an additional blur pass to smooth bloom."));
                        }
                    }
                    else
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                labelStyle = _anamorphicFlares.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandAFSection = EditorGUILayout.Foldout(expandAFSection, new GUIContent("Anamorphic F.", "Also known as JJ Abrams flares, adds spectacular light streaks to your scene."), labelStyle);
                EditorGUILayout.EndHorizontal();
                if (testFeatureEnabled(isBloomEnabled))
                {
                    EditorGUILayout.PropertyField(_anamorphicFlares, GUIContent.none);
                    if (expandAFSection)
                    {
                        if (_anamorphicFlares.boolValue)
                        {
                            GUILayout.Label(new GUIContent("Debug", "Enable to see flares buffer."));
                            _bloomDebug.boolValue = EditorGUILayout.Toggle(_bloomDebug.boolValue, GUILayout.Width(40));
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(_bloomCullingMask, new GUIContent("   Layer Mask", "Select which layers can be used for bloom."));
                        if (_anamorphicFlares.boolValue)
                        {
                            if ((_bloomCullingMask.intValue & 1) != 0)
                            {
                                EditorGUILayout.HelpBox("Set Layer Mask either to Nothing (default value) or to specific layers. Including Default layer is not recommended. If you want flares to be applied to all objects, set Layer Mask to Nothing.", MessageType.Warning);
                            }
                        }
                        if (_bloomCullingMask.intValue != 0 && _quality.intValue == (int)BEAUTIFY_QUALITY.BestQuality)
                        {
                            EditorGUILayout.PropertyField(_bloomLayerMaskDownsampling, new GUIContent("   Mask Downsampling", "Bloom/anamorphic flares layer mask downsampling factor. Increase to improve performance."));
                        }
                        EditorGUILayout.PropertyField(_anamorphicFlaresIntensity, new GUIContent("   Intensity", "Flares light multiplier."));
                        EditorGUILayout.PropertyField(_anamorphicFlaresThreshold, new GUIContent("   Threshold", "Brightness sensibility."));
                        EditorGUILayout.PropertyField(_anamorphicFlaresSpread, new GUIContent("   Spread", "Amplitude of the flares."));
                        EditorGUILayout.PropertyField(_anamorphicFlaresVertical, new GUIContent("   Vertical"));
                        EditorGUILayout.PropertyField(_anamorphicFlaresTint, new GUIContent("   Tint", "Optional tint color for the anamorphic flares. Use color alpha component to blend between original color and the tint."));

                        if (_quality.intValue == (int)BEAUTIFY_QUALITY.BestQuality)
                        {
                            EditorGUILayout.PropertyField(_anamorphicFlaresAntiflicker, new GUIContent("   Reduce Flicker", "Enables an additional filter to reduce excess of flicker."));
                            EditorGUILayout.PropertyField(_anamorphicFlaresUltra, new GUIContent("   Ultra", "Increases anamorphic flares fidelity."));
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(_anamorphicFlaresBlur, new GUIContent("   Blur", "Adds an additional blur pass to smooth anamorphic flare effect."));
                        }
                    }
                    else
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                labelStyle = _sunFlares.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandSFSection = EditorGUILayout.Foldout(expandSFSection, new GUIContent("Sun Flares", "Adds lens flares caused by bright Sun light."), labelStyle);
                EditorGUILayout.EndHorizontal();
                if (testFeatureEnabled(isBloomEnabled))
                {
                    _sunFlares.boolValue = EditorGUILayout.Toggle(_sunFlares.boolValue);
                    if (expandSFSection)
                    {
                        if (_sunFlares.boolValue)
                        {
                            GUILayout.Label(new GUIContent("Debug", "Enable to see flares buffer."));
                            _bloomDebug.boolValue = EditorGUILayout.Toggle(_bloomDebug.boolValue, GUILayout.Width(40));
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(_sun, new GUIContent("   Sun", "Assign a Game Object (usually a Directional Light that acts as the Sun) to automatically synchronize the light direction."));
                        EditorGUILayout.PropertyField(_sunFlaresIntensity, new GUIContent("   Global Intensity", "Global intensity for the sun flares buffer."));
                        EditorGUILayout.PropertyField(_sunFlaresTint, new GUIContent("   Tint", "Global flares tint color."));
                        EditorGUILayout.PropertyField(_sunFlaresDownsampling, new GUIContent("   Downsampling", "Reduces sun flares buffer dimensions to improve performance."));
                        EditorGUILayout.PropertyField(_sunFlaresSunIntensity, new GUIContent("   Sun Intensity", "Intensity for the Sun's disk and corona rays."));

                        if (_quality.intValue == (int)BEAUTIFY_QUALITY.BestQuality)
                        {
                            EditorGUILayout.PropertyField(_sunFlaresSunDiskSize, new GUIContent("   Sun Disk Size", "Size of Sun disk."));
                            EditorGUILayout.PropertyField(_sunFlaresSunRayDiffractionIntensity, new GUIContent("   Diffraction Intensity", "Intensity for the Sun's rays diffraction."));
                            EditorGUILayout.PropertyField(_sunFlaresSunRayDiffractionThreshold, new GUIContent("   Diffraction Threshold", "Theshold of the Sun's rays diffraction."));
                            EditorGUILayout.PropertyField(_sunFlaresSolarWindSpeed, new GUIContent("   Solar Wind Speed", "Animation speed for the diffracted rays."));
                            EditorGUILayout.PropertyField(_sunFlaresRotationDeadZone, new GUIContent("   Rotation DeadZone", "Prevents ray rotation when looking directly to the Sun."));
                        }
                        EditorGUILayout.PropertyField(_sunFlaresLayerMask, new GUIContent("   Occlusion Mask", "Specifies which objects can occlude Sun thus deactivate the Sun flares effect."));

                        expandSFCoronaRays1Section = EditorGUILayout.Foldout(expandSFCoronaRays1Section, new GUIContent("Corona Rays Group 1", "Customize appearance of solar corona rays group 1."), sectionHeaderIndentedStyle);
                        if (expandSFCoronaRays1Section)
                        {
                            EditorGUILayout.PropertyField(_sunFlaresCoronaRays1Length, new GUIContent("   Length", "Length of solar corona rays group 1."));
                            EditorGUILayout.PropertyField(_sunFlaresCoronaRays1Streaks, new GUIContent("   Streaks", "Number of streaks for group 1."));
                            EditorGUILayout.PropertyField(_sunFlaresCoronaRays1Spread, new GUIContent("   Spread", "Light spread factor for group 1."));
                            EditorGUILayout.PropertyField(_sunFlaresCoronaRays1AngleOffset, new GUIContent("   Angle Offset", "Rotation offset for group 1."));
                        }

                        if (_quality.intValue == (int)BEAUTIFY_QUALITY.BestQuality)
                        {
                            expandSFCoronaRays2Section = EditorGUILayout.Foldout(expandSFCoronaRays2Section, new GUIContent("Corona Rays Group 2", "Customize appearance of solar corona rays group 2."), sectionHeaderIndentedStyle);
                            if (expandSFCoronaRays2Section)
                            {
                                EditorGUILayout.PropertyField(_sunFlaresCoronaRays2Length, new GUIContent("   Length", "Length of solar corona rays group 2."));
                                EditorGUILayout.PropertyField(_sunFlaresCoronaRays2Streaks, new GUIContent("   Streaks", "Number of streaks for group 2."));
                                EditorGUILayout.PropertyField(_sunFlaresCoronaRays2Spread, new GUIContent("   Spread", "Light spread factor for group 2."));
                                EditorGUILayout.PropertyField(_sunFlaresCoronaRays2AngleOffset, new GUIContent("   Angle Offset", "Rotation offset for group 2."));
                            }
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

                        if (_quality.intValue == (int)BEAUTIFY_QUALITY.BestQuality)
                        {
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
                        }

                        expandSFHaloSection = EditorGUILayout.Foldout(expandSFHaloSection, new GUIContent("Halo", "Customize appearance of halo."), sectionHeaderIndentedStyle);
                        if (expandSFHaloSection)
                        {
                            EditorGUILayout.PropertyField(_sunFlaresHaloOffset, new GUIContent("   Offset"));
                            EditorGUILayout.PropertyField(_sunFlaresHaloAmplitude, new GUIContent("   Amplitude"));
                            EditorGUILayout.PropertyField(_sunFlaresHaloIntensity, new GUIContent("   Intensity"));
                        }

                    }
                    else
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                labelStyle = _lensDirt.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandDirtSection = EditorGUILayout.Foldout(expandDirtSection, new GUIContent("Lens Dirt", "Enables lens dirt effect which intensifies when looking to a light (uses the nearest light to camera). You can assign other dirt textures directly to the shader material with name 'Beautify'."), labelStyle);
                EditorGUILayout.EndHorizontal();
                if (testFeatureEnabled(isLensDirtEnabled))
                {
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
                else
                {
                    EditorGUILayout.EndHorizontal();
                }

                bool isDoFEnabled = isShaderFeatureEnabled(Beautify.SKW_DEPTH_OF_FIELD, false);
                if (!isDoFEnabled)
                {
                    if (_depthOfField.boolValue)
                    {
                        _depthOfField.boolValue = false;
                    }
                }
                bool isDoFTransp = isShaderFeatureEnabled(Beautify.SKW_DEPTH_OF_FIELD_TRANSPARENT, false);
                if (!isDoFTransp)
                {
                    _depthOfFieldTransparencySupport.boolValue = false;
                    _depthOfFieldExclusionLayerMask.intValue = 0;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                labelStyle = _depthOfField.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandDoFSection = EditorGUILayout.Foldout(expandDoFSection, new GUIContent("Depth of Field", "Blurs the image based on distance to focus point."), labelStyle);
                EditorGUILayout.EndHorizontal();
                if (testFeatureEnabled(isDoFEnabled))
                {
                    _depthOfField.boolValue = EditorGUILayout.Toggle(_depthOfField.boolValue);
                    if (expandDoFSection)
                    {
                        if (_depthOfField.boolValue)
                        {
                            GUILayout.Label(new GUIContent("Debug", "Enable to see depth of field focus area."));
                            _depthOfFieldDebug.boolValue = EditorGUILayout.Toggle(_depthOfFieldDebug.boolValue, GUILayout.Width(40));
                        }
                        EditorGUILayout.EndHorizontal();
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
                            EditorGUILayout.PropertyField(_depthOfFieldForegroundDistance, new GUIContent("      Near Offset", "Distance from focus plane for foreground blur."));
                            if (_quality.intValue == (int)BEAUTIFY_QUALITY.BestQuality)
                            {
                                EditorGUILayout.PropertyField(_depthOfFieldForegroundBlurHQ, new GUIContent("      High Quality", "Improves depth of field foreground blur."));
                            }
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
                        if (testFeatureEnabled(isDoFTransp))
                        {
                            EditorGUILayout.PropertyField(_depthOfFieldExclusionLayerMask, GUIContent.none);
                            if (_depthOfFieldExclusionLayerMask.intValue != 0)
                            {
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.PropertyField(_depthOfFieldExclusionBias, new GUIContent("       Depth Bias", "Depth offset for the exclusion mask computation."));
                                EditorGUILayout.PropertyField(_depthOfFieldExclusionLayerMaskDownsampling, new GUIContent("       Downsampling", "This value is added to the DoF downsampling factor for the exclusion mask creation. Increase to improve performance. "));
                                EditorGUILayout.PropertyField(_depthOfFieldFilterMode, new GUIContent("       Filter Mode", "Texture filter mode."));
                            }
                            else
                            {
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(new GUIContent("   Transparency", "Enable transparency support."), GUILayout.Width(120));
                        if (testFeatureEnabled(isDoFTransp))
                        {
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
                        }
                        else
                        {
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }

                bool isEyeAdaptationEnabled = isShaderFeatureEnabled(Beautify.SKW_EYE_ADAPTATION, false);
                if (!isEyeAdaptationEnabled)
                {
                    if (_eyeAdaptation.boolValue)
                    {
                        _eyeAdaptation.boolValue = false;
                    }
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                labelStyle = _eyeAdaptation.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandEASection = EditorGUILayout.Foldout(expandEASection, new GUIContent("Eye Adaptation", "Enables eye adaptation effect. Simulates retina response to quick luminance changes in the scene."), labelStyle);
                EditorGUILayout.EndHorizontal();
                if (testFeatureEnabled(isEyeAdaptationEnabled))
                {
                    EditorGUILayout.PropertyField(_eyeAdaptation, GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                    if (expandEASection)
                    {
                        EditorGUILayout.PropertyField(_eyeAdaptationMinExposure, new GUIContent("   Min Exposure"));
                        EditorGUILayout.PropertyField(_eyeAdaptationMaxExposure, new GUIContent("   Max Exposure"));
                        EditorGUILayout.PropertyField(_eyeAdaptationSpeedToDark, new GUIContent("   Dark Adapt Speed"));
                        EditorGUILayout.PropertyField(_eyeAdaptationSpeedToLight, new GUIContent("   Light Adapt Speed"));
                    }
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }

                bool isPurkinjeEnabled = isShaderFeatureEnabled(Beautify.SKW_PURKINJE, false);
                if (!isPurkinjeEnabled)
                {
                    if (_purkinje.boolValue)
                    {
                        _purkinje.boolValue = false;
                    }
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                labelStyle = _purkinje.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandPurkinjeSection = EditorGUILayout.Foldout(expandPurkinjeSection, new GUIContent("Purkinje", "Simulates achromatic vision plus spectrum shift to blue in the dark."), labelStyle);
                EditorGUILayout.EndHorizontal();
                if (testFeatureEnabled(isPurkinjeEnabled))
                {
                    EditorGUILayout.PropertyField(_purkinje, GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                    if (expandPurkinjeSection)
                    {
                        EditorGUILayout.PropertyField(_purkinjeAmount, new GUIContent("   Shift Amount", "Spectrum shift to blue. A value of zero will not shift colors and stay in grayscale."));
                        EditorGUILayout.PropertyField(_purkinjeLuminanceThreshold, new GUIContent("   Threshold", "Increase this value to augment the purkinje effect (applies to higher luminance levels)."));
                    }
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Separator();
                DrawLabel("Artistic Choices");

                bool isVignettingEnabled = isShaderFeatureEnabled(Beautify.SKW_VIGNETTING, false);
                if (!isVignettingEnabled)
                {
                    if (_vignetting.boolValue)
                    {
                        _vignetting.boolValue = false;
                    }
                }
                bool isFrameEnabled = isShaderFeatureEnabled(Beautify.SKW_FRAME, false);
                if (!isFrameEnabled)
                {
                    if (_frame.boolValue)
                    {
                        _frame.boolValue = false;
                    }
                }
                bool isOutlineEnabled = isShaderFeatureEnabled(Beautify.SKW_OUTLINE, false);
                if (!isOutlineEnabled)
                {
                    if (_outline.boolValue)
                    {
                        _outline.boolValue = false;
                    }
                }
                bool isNightVisionEnabled = isShaderFeatureEnabled(Beautify.SKW_NIGHT_VISION, false);
                if (!isNightVisionEnabled)
                {
                    if (_nightVision.boolValue)
                    {
                        _nightVision.boolValue = false;
                    }
                }
                bool isThermalVisionEnabled = isShaderFeatureEnabled(Beautify.SKW_THERMAL_VISION, false);
                if (!isThermalVisionEnabled)
                {
                    if (_thermalVision.boolValue)
                    {
                        _thermalVision.boolValue = false;
                    }
                }

                if (_vignetting.boolValue || _frame.boolValue || _outline.boolValue || _nightVision.boolValue || _thermalVision.boolValue)
                {
                    EditorGUILayout.HelpBox("Customize the effects below using color picker. Alpha has special meaning depending on effect. Read the tooltip moving the mouse over the effect name.", MessageType.Info);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                labelStyle = _vignetting.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandVignettingSection = EditorGUILayout.Foldout(expandVignettingSection, new GUIContent("Vignetting", "Enables colored vignetting effect. Color alpha specifies intensity of effect."), labelStyle);
                EditorGUILayout.EndHorizontal();
                if (testFeatureEnabled(isVignettingEnabled))
                {
                    EditorGUILayout.PropertyField(_vignetting, GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                    if (expandVignettingSection)
                    {
                        EditorGUILayout.PropertyField(_vignettingColor, new GUIContent("   Color/Size", "The color for the vignetting effect. Alpha specifies intensity of effect."));
                        EditorGUILayout.PropertyField(_vignettingFade, new GUIContent("   Fade Out", "Fade out effect to the vignette color."));
                        EditorGUILayout.PropertyField(_vignettingBlink, new GUIContent("   Blink", "Manages blink effect for testing purposes. Use Beautify.instance.Blink to quickly produce a blink effect."));
                        EditorGUILayout.PropertyField(_vignettingCircularShape, new GUIContent("   Circular Shape", "Ignores screen aspect ratio showing a circular shape."));
                        if (!_vignettingCircularShape.boolValue)
                        {
                            EditorGUILayout.PropertyField(_vignettingAspectRatio, new GUIContent("   Aspect Ratio", "Custom aspect ratio."));
                        }

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(new GUIContent("   Mask Texture", "Texture used for masking vignette effect. Alpha channel will be used to determine which areas remain untouched (1=color untouched, less than 1=vignette effect)"), GUILayout.Width(120));
                        if (isShaderFeatureEnabled(Beautify.SKW_VIGNETTING_MASK))
                        {
                            EditorGUILayout.PropertyField(_vignettingMask, GUIContent.none);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                labelStyle = _frame.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandFrameSection = EditorGUILayout.Foldout(expandFrameSection, new GUIContent("Frame", "Enables colored frame effect. Color alpha specifies intensity of effect."), labelStyle);
                EditorGUILayout.EndHorizontal();
                if (testFeatureEnabled(isFrameEnabled))
                {
                    EditorGUILayout.PropertyField(_frame, GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                    if (expandFrameSection)
                    {
                        EditorGUILayout.PropertyField(_frameColor, new GUIContent("   Color", "The color for the frame effect. Alpha specifies intensity of effect."));

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(new GUIContent("   Mask Texture", "Texture used for frame effect."), GUILayout.Width(120));
                        if (isShaderFeatureEnabled(Beautify.SKW_FRAME_MASK))
                        {
                            EditorGUILayout.PropertyField(_frameMask, GUIContent.none);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                labelStyle = _outline.boolValue ? sectionHeaderBoldStyle : sectionHeaderNormalStyle;
                expandOutlineSection = EditorGUILayout.Foldout(expandOutlineSection, new GUIContent("Outline", "Enables outline (edge detection) effect. Color alpha specifies edge detection threshold (reference values: 0.8 for depth, 0.35 for Sobel)."), labelStyle);
                EditorGUILayout.EndHorizontal();
                if (testFeatureEnabled(isOutlineEnabled))
                {
                    EditorGUILayout.PropertyField(_outline, GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                    if (expandOutlineSection)
                    {
                        EditorGUILayout.PropertyField(_outlineColor, new GUIContent("   Color", "The color for the outline. Alpha specifies edge detection threshold."));
                        if (_outline.boolValue)
                        {
                            EditorGUILayout.HelpBox("Choose between depth-based or color-based edge detection algorithm in Shader Options section.", MessageType.Info);
                        }
                    }
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                labelStyle = _nightVision.boolValue ? labelBoldStyle : labelNormalStyle;
                GUILayout.Label(new GUIContent("Night Vision", "Enables night vision effect. Color alpha controls intensity. For a better result, enable Vignetting and set its color to (0,0,0,32)."), labelStyle, GUILayout.Width(120));
                if (testFeatureEnabled(isNightVisionEnabled))
                {
                    EditorGUILayout.PropertyField(_nightVision, GUIContent.none);
                    if (_nightVision.boolValue)
                    {
                        GUILayout.Label(new GUIContent("Color", "The color for the night vision effect. Alpha controls intensity."), GUILayout.Width(40));
                        EditorGUILayout.PropertyField(_nightVisionColor, GUIContent.none);
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (_lut.boolValue && _nightVision.boolValue)
                    EditorGUILayout.HelpBox("Night Vision disabled by LUT.", MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                labelStyle = _thermalVision.boolValue ? labelBoldStyle : labelNormalStyle;
                GUILayout.Label(new GUIContent("Thermal Vision", "Enables thermal vision effect."), labelStyle, GUILayout.Width(120));
                if (testFeatureEnabled(isThermalVisionEnabled))
                {
                    EditorGUILayout.PropertyField(_thermalVision, GUIContent.none);
                }
                EditorGUILayout.EndHorizontal();
                if (_lut.boolValue && _thermalVision.boolValue)
                    EditorGUILayout.HelpBox("Thermal Vision disabled by LUT.", MessageType.Info);

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
            }

            if (serializedObject.ApplyModifiedProperties() || (Event.current.type == EventType.ValidateCommand &&
                            Event.current.commandName == "UndoRedoPerformed" || GUI.changed || _effect.isDirty))
            {
                if (qualityChanged)
                {
                    _effect.UpdateQualitySettings();
                }
                if (!generalSettingsChanged)
                {
                    _effect.preset = BEAUTIFY_PRESET.Custom;
                }
                if (_effect.profile != null)
                {
                    if (profileChanged)
                    {
                        _effect.profile.Load(_effect);
                        profileChanges = false;
                    }
                    else
                    {
                        profileChanges = true;
                    }
                }
                _effect.UpdateMaterialPropertiesNow();
                _effect.isDirty = false;
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


        #region Shader handling

        void ScanKeywords()
        {
            if (shaders == null)
                shaders = new List<BeautifySInfo>();
            else
                shaders.Clear();
            string[] guids = AssetDatabase.FindAssets("Beautify t:Shader");
            for (int k = 0; k < guids.Length; k++)
            {
                string guid = guids[k];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                BeautifySInfo shader = new BeautifySInfo();
                shader.path = path;
                shader.name = Path.GetFileNameWithoutExtension(path);
                ScanShader(shader);
                if (shader.keywords.Count > 0)
                {
                    shaders.Add(shader);
                }
            }
        }

        void ScanShader(BeautifySInfo shader)
        {
            // Inits shader
            shader.passes.Clear();
            shader.keywords.Clear();
            shader.pendingChanges = false;
            shader.editedByShaderControl = false;

            // Reads shader
            string[] shaderLines = File.ReadAllLines(shader.path);
            string[] separator = new string[] { " " };
            SCShaderPass currentPass = new SCShaderPass();
            SCShaderPass basePass = null;
            int pragmaControl = 0;
            int pass = -1;
            SCKeywordLine keywordLine = new SCKeywordLine();
            for (int k = 0; k < shaderLines.Length; k++)
            {
                string line = shaderLines[k].Trim();
                if (line.Length == 0)
                    continue;
                string lineUPPER = line.ToUpper();
                if (lineUPPER.Equals("PASS") || lineUPPER.StartsWith("PASS "))
                {
                    if (pass >= 0)
                    {
                        currentPass.pass = pass;
                        if (basePass != null)
                            currentPass.Add(basePass.keywordLines);
                        shader.Add(currentPass);
                    }
                    else if (currentPass.keywordCount > 0)
                    {
                        basePass = currentPass;
                    }
                    currentPass = new SCShaderPass();
                    pass++;
                    continue;
                }
                int j = line.IndexOf(PRAGMA_COMMENT_MARK);
                if (j >= 0)
                {
                    pragmaControl = 1;
                }
                else
                {
                    j = line.IndexOf(PRAGMA_DISABLED_MARK);
                    if (j >= 0)
                        pragmaControl = 3;
                }
                j = line.IndexOf(PRAGMA_MULTICOMPILE);
                if (j >= 0)
                {
                    if (pragmaControl != 2)
                        keywordLine = new SCKeywordLine();
                    string[] kk = line.Substring(j + 22).Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    // Sanitize keywords
                    for (int i = 0; i < kk.Length; i++)
                        kk[i] = kk[i].Trim();
                    // Act on keywords
                    switch (pragmaControl)
                    {
                        case 1: // Edited by Shader Control line
                            shader.editedByShaderControl = true;
                            // Add original keywords to current line
                            for (int s = 0; s < kk.Length; s++)
                            {
                                keywordLine.Add(shader.GetKeyword(kk[s]));
                            }
                            pragmaControl = 2;
                            break;
                        case 2:
                            // check enabled keywords
                            keywordLine.DisableKeywords();
                            for (int s = 0; s < kk.Length; s++)
                            {
                                SCKeyword keyword = keywordLine.GetKeyword(kk[s]);
                                if (keyword != null)
                                    keyword.enabled = true;
                            }
                            currentPass.Add(keywordLine);
                            pragmaControl = 0;
                            break;
                        case 3: // disabled by Shader Control line
                            shader.editedByShaderControl = true;
                            // Add original keywords to current line
                            for (int s = 0; s < kk.Length; s++)
                            {
                                SCKeyword keyword = shader.GetKeyword(kk[s]);
                                keyword.enabled = false;
                                keywordLine.Add(keyword);
                            }
                            currentPass.Add(keywordLine);
                            pragmaControl = 0;
                            break;
                        case 0:
                            // Add keywords to current line
                            for (int s = 0; s < kk.Length; s++)
                            {
                                keywordLine.Add(shader.GetKeyword(kk[s]));
                            }
                            currentPass.Add(keywordLine);
                            break;
                    }
                }
            }
            currentPass.pass = Mathf.Max(pass, 0);
            if (basePass != null)
                currentPass.Add(basePass.keywordLines);
            shader.Add(currentPass);
        }

        void UpdateShaders()
        {
            // normalize keywords
            if (shaders.Count > 0)
            {
                BeautifySInfo mainShader = shaders[0];
                BeautifySInfo shader = shaders[1];
                for (int k = 0; k < mainShader.keywords.Count; k++)
                {
                    SCKeyword mainKeyword = mainShader.keywords[k];
                    SCKeyword keyword = shader.GetKeyword(mainKeyword.name);
                    keyword.enabled = mainKeyword.enabled;
                    if (!keyword.enabled)
                    {
                        // disable OnRenderImage processing for disabled effects
                        switch (keyword.name)
                        {
                            case Beautify.SKW_DEPTH_OF_FIELD:
                                _depthOfField.boolValue = false;
                                break;
                            case Beautify.SKW_DEPTH_OF_FIELD_TRANSPARENT:
                                _depthOfFieldTransparencySupport.boolValue = false;
                                _depthOfFieldExclusionLayerMask.intValue = 0;
                                break;
                            case Beautify.SKW_BLOOM:
                                _bloom.boolValue = false;
                                break;
                            case Beautify.SKW_EYE_ADAPTATION:
                                _eyeAdaptation.boolValue = false;
                                break;
                            case Beautify.SKW_DIRT:
                                _lensDirt.boolValue = false;
                                break;
                            case Beautify.SKW_PURKINJE:
                                _purkinje.boolValue = false;
                                break;
                        }
                    }

                }
            }

            // Update shader files
            for (int k = 0; k < shaders.Count; k++)
            {
                BeautifySInfo shader = shaders[k];
                UpdateShader(shader);
            }
        }

        void UpdateShader(BeautifySInfo shader)
        {

            // Reads and updates shader from disk
            string[] shaderLines = File.ReadAllLines(shader.path);
            string[] separator = new string[] { " " };
            StringBuilder sb = new StringBuilder();
            int pragmaControl = 0;
            shader.editedByShaderControl = false;
            SCKeywordLine keywordLine = new SCKeywordLine();
            for (int k = 0; k < shaderLines.Length; k++)
            {
                int j = shaderLines[k].IndexOf(PRAGMA_COMMENT_MARK);
                if (j >= 0)
                    pragmaControl = 1;
                j = shaderLines[k].IndexOf(PRAGMA_MULTICOMPILE);
                if (j >= 0)
                {
                    if (pragmaControl != 2)
                        keywordLine.Clear();
                    string[] kk = shaderLines[k].Substring(j + 22).Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    // Sanitize keywords
                    for (int i = 0; i < kk.Length; i++)
                        kk[i] = kk[i].Trim();
                    // Act on keywords
                    switch (pragmaControl)
                    {
                        case 1:
                            // Read original keywords
                            for (int s = 0; s < kk.Length; s++)
                            {
                                SCKeyword keyword = shader.GetKeyword(kk[s]);
                                keywordLine.Add(keyword);
                            }
                            pragmaControl = 2;
                            break;
                        case 0:
                        case 2:
                            if (pragmaControl == 0)
                            {
                                for (int s = 0; s < kk.Length; s++)
                                {
                                    SCKeyword keyword = shader.GetKeyword(kk[s]);
                                    keywordLine.Add(keyword);
                                }
                            }
                            int kCount = keywordLine.keywordCount;
                            int kEnabledCount = keywordLine.keywordsEnabledCount;
                            if (kEnabledCount < kCount)
                            {
                                // write original keywords
                                if (kEnabledCount == 0)
                                {
                                    sb.Append(PRAGMA_DISABLED_MARK);
                                }
                                else
                                {
                                    sb.Append(PRAGMA_COMMENT_MARK);
                                }
                                shader.editedByShaderControl = true;
                                sb.Append(PRAGMA_MULTICOMPILE);
                                if (keywordLine.hasUnderscoreVariant)
                                    sb.Append(PRAGMA_UNDERSCORE);
                                for (int s = 0; s < kCount; s++)
                                {
                                    SCKeyword keyword = keywordLine.keywords[s];
                                    sb.Append(keyword.name);
                                    if (s < kCount - 1)
                                        sb.Append(" ");
                                }
                                sb.AppendLine();
                            }

                            if (kEnabledCount > 0)
                            {
                                // Write actual keywords
                                sb.Append(PRAGMA_MULTICOMPILE);
                                if (keywordLine.hasUnderscoreVariant)
                                    sb.Append(PRAGMA_UNDERSCORE);
                                for (int s = 0; s < kCount; s++)
                                {
                                    SCKeyword keyword = keywordLine.keywords[s];
                                    if (keyword.enabled)
                                    {
                                        sb.Append(keyword.name);
                                        if (s < kCount - 1)
                                            sb.Append(" ");
                                    }
                                }
                                sb.AppendLine();
                            }
                            pragmaControl = 0;
                            break;
                    }
                }
                else
                {
                    sb.AppendLine(shaderLines[k]);
                }
            }

            // Writes modified shader
            File.WriteAllText(shader.path, sb.ToString());

            AssetDatabase.Refresh();

            ScanShader(shader); // Rescan shader
        }

        bool isShaderFeatureEnabled(string name, bool drawLabel = true)
        {
            if (shaders.Count == 0)
                return false;
            SCKeyword keyword = shaders[0].GetKeyword(name);
            if (drawLabel)
            {
                return testFeatureEnabled(keyword.enabled);
            }
            return keyword.enabled;
        }

        bool testFeatureEnabled(bool value)
        {
            if (!value)
            {
                GUILayout.Label("(feature disabled in build options)");
                return false;
            }
            return true;
        }


        #endregion

        #region Profile handling

        void CreateProfile()
        {

            BeautifyProfile newProfile = ScriptableObject.CreateInstance<BeautifyProfile>();
            newProfile.Save(_effect);

            AssetDatabase.CreateAsset(newProfile, "Assets/BeautifyProfile.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newProfile;

            _effect.profile = newProfile;
        }

        void RefreshProfiles()
        {
            Beautify[] bb = FindObjectsOfType<Beautify>();
            for (int k = 0; k < bb.Length; k++)
            {
                Beautify b = bb[k];
                if (b.gameObject.activeInHierarchy && b.profile == _profile.objectReferenceValue)
                {
                    b.profile.Load(b);
                    EditorUtility.SetDirty(b);
                }
            }
        }

        #endregion

        #region Advanced Options handling

        void ScanAdvancedOptions()
        {
            if (shaderAdvancedOptionsInfo == null)
            {
                shaderAdvancedOptionsInfo = new BeautifySAdvancedOptionsInfo();
            }
            shaderAdvancedOptionsInfo.ReadOptions();
        }


        #endregion
    }

}
