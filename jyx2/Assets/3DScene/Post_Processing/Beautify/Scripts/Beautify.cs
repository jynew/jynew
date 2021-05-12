/// <summary>
/// Copyright 2016-2018 Ramiro Oliva (Kronnect) - All rights reserved
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
//#define DEBUG_BEAUTIFY

namespace BeautifyEffect
{
    public enum BEAUTIFY_QUALITY
    {
        BestQuality,
        BestPerformance,
        Basic
    }

    public enum BEAUTIFY_PRESET
    {
        Soft = 10,
        Medium = 20,
        Strong = 30,
        Exaggerated = 40,
        Custom = 999
    }

    public enum BEAUTIFY_TMO
    {
        Linear = 0,
        ACES = 10
    }


    [ExecuteInEditMode, RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Rendering/Beautify")]
    [HelpURL("http://kronnect.com/taptapgo")]
    [ImageEffectAllowedInSceneView]
    public class Beautify : MonoBehaviour
    {


        #region General settings

        [SerializeField]
        BEAUTIFY_PRESET
                        _preset = BEAUTIFY_PRESET.Medium;

        public BEAUTIFY_PRESET preset
        {
            get { return _preset; }
            set
            {
                if (_preset != value)
                {
                    _preset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        BEAUTIFY_QUALITY
                        _quality = BEAUTIFY_QUALITY.BestQuality;

        public BEAUTIFY_QUALITY quality
        {
            get { return _quality; }
            set
            {
                if (_quality != value)
                {
                    _quality = value;
                    UpdateQualitySettings();
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        BeautifyProfile _profile;

        public BeautifyProfile profile
        {
            get { return _profile; }
            set
            {
                if (_profile != value)
                {
                    _profile = value;
                    if (_profile != null)
                    {
                        _profile.Load(this);
                        _preset = BEAUTIFY_PRESET.Custom;
                    }
                }
            }
        }

        [SerializeField]
        bool
                        _compareMode = false;

        public bool compareMode
        {
            get { return _compareMode; }
            set
            {
                if (_compareMode != value)
                {
                    _compareMode = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(-Mathf.PI, Mathf.PI)]
        float
                        _compareLineAngle = 1.4f;

        public float compareLineAngle
        {
            get { return _compareLineAngle; }
            set
            {
                if (_compareLineAngle != value)
                {
                    _compareLineAngle = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.0001f, 0.05f)]
        float
                        _compareLineWidth = 0.002f;

        public float compareLineWidth
        {
            get { return _compareLineWidth; }
            set
            {
                if (_compareLineWidth != value)
                {
                    _compareLineWidth = value;
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region RGB Dither

        [SerializeField]
        [Range(0, 0.2f)]
        float
                        _dither = 0.02f;

        public float dither
        {
            get { return _dither; }
            set
            {
                if (_dither != value)
                {
                    _preset = BEAUTIFY_PRESET.Custom;
                    _dither = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1f)]
        float
                        _ditherDepth = 0f;

        public float ditherDepth
        {
            get { return _ditherDepth; }
            set
            {
                if (_ditherDepth != value)
                {
                    _preset = BEAUTIFY_PRESET.Custom;
                    _ditherDepth = value;
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region Sharpen Settings


        [SerializeField]
        [Range(0, 1f)]
        float
                        _sharpenMinDepth = 0f;

        public float sharpenMinDepth
        {
            get { return _sharpenMinDepth; }
            set
            {
                if (_sharpenMinDepth != value)
                {
                    _sharpenMinDepth = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1.1f)]
        float
                        _sharpenMaxDepth = 0.999f;

        public float sharpenMaxDepth
        {
            get { return _sharpenMaxDepth; }
            set
            {
                if (_sharpenMaxDepth != value)
                {
                    _sharpenMaxDepth = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 15f)]
        float
                        _sharpen = 2f;

        public float sharpen
        {
            get { return _sharpen; }
            set
            {
                if (_sharpen != value)
                {
                    _preset = BEAUTIFY_PRESET.Custom;
                    _sharpen = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 0.05f)]
        float
                        _sharpenDepthThreshold = 0.035f;

        public float sharpenDepthThreshold
        {
            get { return _sharpenDepthThreshold; }
            set
            {
                if (_sharpenDepthThreshold != value)
                {
                    _preset = BEAUTIFY_PRESET.Custom;
                    _sharpenDepthThreshold = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Color
                        _tintColor = new Color(1, 1, 1, 0);

        public Color tintColor
        {
            get { return _tintColor; }
            set
            {
                if (_tintColor != value)
                {
                    _tintColor = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 0.2f)]
        float
                        _sharpenRelaxation = 0.08f;

        public float sharpenRelaxation
        {
            get { return _sharpenRelaxation; }
            set
            {
                if (_sharpenRelaxation != value)
                {
                    _preset = BEAUTIFY_PRESET.Custom;
                    _sharpenRelaxation = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1f)]
        float
                        _sharpenClamp = 0.45f;

        public float sharpenClamp
        {
            get { return _sharpenClamp; }
            set
            {
                if (_sharpenClamp != value)
                {
                    _preset = BEAUTIFY_PRESET.Custom;
                    _sharpenClamp = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1f)]
        float
                        _sharpenMotionSensibility = 0.5f;

        public float sharpenMotionSensibility
        {
            get { return _sharpenMotionSensibility; }
            set
            {
                if (_sharpenMotionSensibility != value)
                {
                    _sharpenMotionSensibility = value;
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region Color grading

        [SerializeField]
        [Range(-2f, 3f)]
        float
                        _saturate = 1f;

        public float saturate
        {
            get { return _saturate; }
            set
            {
                if (_saturate != value)
                {
                    _preset = BEAUTIFY_PRESET.Custom;
                    _saturate = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.5f, 1.5f)]
        float
                        _contrast = 1.02f;

        public float contrast
        {
            get { return _contrast; }
            set
            {
                if (_contrast != value)
                {
                    _preset = BEAUTIFY_PRESET.Custom;
                    _contrast = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 2f)]
        float
                        _brightness = 1.05f;

        public float brightness
        {
            get { return _brightness; }
            set
            {
                if (_brightness != value)
                {
                    _preset = BEAUTIFY_PRESET.Custom;
                    _brightness = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 2f)]
        float
                        _daltonize = 0f;

        public float daltonize
        {
            get { return _daltonize; }
            set
            {
                if (_daltonize != value)
                {
                    _preset = BEAUTIFY_PRESET.Custom;
                    _daltonize = value;
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region Vignetting

        [SerializeField]
        bool
                        _vignetting = false;

        public bool vignetting
        {
            get { return _vignetting; }
            set
            {
                if (_vignetting != value)
                {
                    _vignetting = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Color
                        _vignettingColor = new Color(0.3f, 0.3f, 0.3f, 0.05f);

        public Color vignettingColor
        {
            get { return _vignettingColor; }
            set
            {
                if (_vignettingColor != value)
                {
                    _vignettingColor = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float
        _vignettingFade = 0;

        public float vignettingFade
        {
            get { return _vignettingFade; }
            set
            {
                if (_vignettingFade != value)
                {
                    _vignettingFade = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _vignettingCircularShape = false;

        public bool vignettingCircularShape
        {
            get { return _vignettingCircularShape; }
            set
            {
                if (_vignettingCircularShape != value)
                {
                    _vignettingCircularShape = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        float
                        _vignettingAspectRatio = 1.0f;

        public float vignettingAspectRatio
        {
            get { return _vignettingAspectRatio; }
            set
            {
                if (_vignettingAspectRatio != value)
                {
                    _vignettingAspectRatio = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField, Range(0, 1f)]
        float
                        _vignettingBlink = 0f;

        public float vignettingBlink
        {
            get { return _vignettingBlink; }
            set
            {
                if (_vignettingBlink != value)
                {
                    _vignettingBlink = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        Texture2D
                        _vignettingMask;

        public Texture2D vignettingMask
        {
            get { return _vignettingMask; }
            set
            {
                if (_vignettingMask != value)
                {
                    _vignettingMask = value;
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region Frame

        [SerializeField]
        bool
                        _frame = false;

        public bool frame
        {
            get { return _frame; }
            set
            {
                if (_frame != value)
                {
                    _frame = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Color
                        _frameColor = new Color(1, 1, 1, 0.047f);

        public Color frameColor
        {
            get { return _frameColor; }
            set
            {
                if (_frameColor != value)
                {
                    _frameColor = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Texture2D
                        _frameMask;

        public Texture2D frameMask
        {
            get { return _frameMask; }
            set
            {
                if (_frameMask != value)
                {
                    _frameMask = value;
                    UpdateMaterialProperties();
                }
            }
        }


        #endregion

        #region LUT

        [SerializeField]
        bool
                        _lut = false;

        public bool lut
        {
            get { return _lut; }
            set
            {
                if (_lut != value)
                {
                    _lut = value;
                    if (_lut)
                    {
                        _nightVision = false;
                        _thermalVision = false;
                    }
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _lutIntensity = 1f;

        public float lutIntensity
        {
            get { return _lutIntensity; }
            set
            {
                if (_lutIntensity != value)
                {
                    _lutIntensity = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Texture2D _lutTexture;

        public Texture2D lutTexture
        {
            get { return _lutTexture; }
            set
            {
                if (_lutTexture != value)
                {
                    _lutTexture = value;
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region Night Vision

        [SerializeField]
        bool
                        _nightVision = false;

        public bool nightVision
        {
            get { return _nightVision; }
            set
            {
                if (_nightVision != value)
                {
                    _nightVision = value;
                    if (_nightVision)
                    {
                        _thermalVision = false;
                        _lut = false;
                        _vignetting = true;
                        _vignettingFade = 0;
                        _vignettingColor = new Color(0, 0, 0, 32f / 255f);
                        _vignettingCircularShape = true;
                    }
                    else
                    {
                        _vignetting = false;
                    }
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Color
                        _nightVisionColor = new Color(0.5f, 1f, 0.5f, 0.5f);

        public Color nightVisionColor
        {
            get { return _nightVisionColor; }
            set
            {
                if (_nightVisionColor != value)
                {
                    _nightVisionColor = value;
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region Outline

        [SerializeField]
        bool
                        _outline = false;

        public bool outline
        {
            get { return _outline; }
            set
            {
                if (_outline != value)
                {
                    _outline = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Color
                        _outlineColor = new Color(0, 0, 0, 0.8f);

        public Color outlineColor
        {
            get { return _outlineColor; }
            set
            {
                if (_outlineColor != value)
                {
                    _outlineColor = value;
                    UpdateMaterialProperties();
                }
            }
        }


        #endregion

        #region Thermal Vision

        [SerializeField]
        bool
                        _thermalVision = false;

        public bool thermalVision
        {
            get { return _thermalVision; }
            set
            {
                if (_thermalVision != value)
                {
                    _thermalVision = value;
                    if (_thermalVision)
                    {
                        _nightVision = false;
                        _lut = false;
                        _vignetting = true;
                        _vignettingFade = 0;
                        _vignettingColor = new Color(1f, 16f / 255f, 16f / 255f, 18f / 255f);
                        _vignettingCircularShape = true;
                    }
                    else
                    {
                        _vignetting = false;
                    }
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region Lens Dirt

        [SerializeField]
        bool
                        _lensDirt = false;

        public bool lensDirt
        {
            get { return _lensDirt; }
            set
            {
                if (_lensDirt != value)
                {
                    _lensDirt = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _lensDirtThreshold = 0.5f;

        public float lensDirtThreshold
        {
            get { return _lensDirtThreshold; }
            set
            {
                if (_lensDirtThreshold != value)
                {
                    _lensDirtThreshold = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _lensDirtIntensity = 0.9f;

        public float lensDirtIntensity
        {
            get { return _lensDirtIntensity; }
            set
            {
                if (_lensDirtIntensity != value)
                {
                    _lensDirtIntensity = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Texture2D
                        _lensDirtTexture;

        public Texture2D lensDirtTexture
        {
            get { return _lensDirtTexture; }
            set
            {
                if (_lensDirtTexture != value)
                {
                    _lensDirtTexture = value;
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region Bloom

        [SerializeField]
        bool
                        _bloom = false;

        public bool bloom
        {
            get { return _bloom; }
            set
            {
                if (_bloom != value)
                {
                    _bloom = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        LayerMask
                        _bloomCullingMask = 0;

        public LayerMask bloomCullingMask
        {
            get { return _bloomCullingMask; }
            set
            {
                if (_bloomCullingMask != value)
                {
                    _bloomCullingMask = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(1f, 4f)]
        float
                        _bloomLayerMaskDownsampling = 1f;

        public float bloomLayerMaskDownsampling
        {
            get { return _bloomLayerMaskDownsampling; }
            set
            {
                if (_bloomLayerMaskDownsampling != value)
                {
                    _bloomLayerMaskDownsampling = Mathf.Max(value, 1f);
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(0, 10f)]
        float
                        _bloomIntensity = 1f;

        public float bloomIntensity
        {
            get { return _bloomIntensity; }
            set
            {
                if (_bloomIntensity != value)
                {
                    _bloomIntensity = Mathf.Abs(value);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        float
                        _bloomMaxBrightness = 1000f;

        public float bloomMaxBrightness
        {
            get { return _bloomMaxBrightness; }
            set
            {
                if (_bloomMaxBrightness != value)
                {
                    _bloomMaxBrightness = Mathf.Abs(value);
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(0f, 3f)]
        float
                        _bloomBoost0 = 0f;

        public float bloomBoost0
        {
            get { return _bloomBoost0; }
            set
            {
                if (_bloomBoost0 != value)
                {
                    _bloomBoost0 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 3f)]
        float
                        _bloomBoost1 = 0f;

        public float bloomBoost1
        {
            get { return _bloomBoost1; }
            set
            {
                if (_bloomBoost1 != value)
                {
                    _bloomBoost1 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 3f)]
        float
                        _bloomBoost2 = 0f;

        public float bloomBoost2
        {
            get { return _bloomBoost2; }
            set
            {
                if (_bloomBoost2 != value)
                {
                    _bloomBoost2 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 3f)]
        float
                        _bloomBoost3 = 0f;

        public float bloomBoost3
        {
            get { return _bloomBoost3; }
            set
            {
                if (_bloomBoost3 != value)
                {
                    _bloomBoost3 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 3f)]
        float
                        _bloomBoost4 = 0f;

        public float bloomBoost4
        {
            get { return _bloomBoost4; }
            set
            {
                if (_bloomBoost4 != value)
                {
                    _bloomBoost4 = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(0f, 3f)]
        float
                        _bloomBoost5 = 0f;

        public float bloomBoost5
        {
            get { return _bloomBoost5; }
            set
            {
                if (_bloomBoost5 != value)
                {
                    _bloomBoost5 = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        bool
                        _bloomAntiflicker = false;

        public bool bloomAntiflicker
        {
            get { return _bloomAntiflicker; }
            set
            {
                if (_bloomAntiflicker != value)
                {
                    _bloomAntiflicker = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _bloomUltra = false;

        public bool bloomUltra
        {
            get { return _bloomUltra; }
            set
            {
                if (_bloomUltra != value)
                {
                    _bloomUltra = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 5f)]
        float
                        _bloomThreshold = 0.75f;

        public float bloomThreshold
        {
            get { return _bloomThreshold; }
            set
            {
                if (_bloomThreshold != value)
                {
                    _bloomThreshold = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _bloomCustomize = false;

        public bool bloomCustomize
        {
            get { return _bloomCustomize; }
            set
            {
                if (_bloomCustomize != value)
                {
                    _bloomCustomize = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _bloomDebug = false;

        public bool bloomDebug
        {
            get { return _bloomDebug; }
            set
            {
                if (_bloomDebug != value)
                {
                    _bloomDebug = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _bloomWeight0 = 0.5f;

        public float bloomWeight0
        {
            get { return _bloomWeight0; }
            set
            {
                if (_bloomWeight0 != value)
                {
                    _bloomWeight0 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _bloomWeight1 = 0.5f;

        public float bloomWeight1
        {
            get { return _bloomWeight1; }
            set
            {
                if (_bloomWeight1 != value)
                {
                    _bloomWeight1 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _bloomWeight2 = 0.5f;

        public float bloomWeight2
        {
            get { return _bloomWeight2; }
            set
            {
                if (_bloomWeight2 != value)
                {
                    _bloomWeight2 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _bloomWeight3 = 0.5f;

        public float bloomWeight3
        {
            get { return _bloomWeight3; }
            set
            {
                if (_bloomWeight3 != value)
                {
                    _bloomWeight3 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _bloomWeight4 = 0.5f;

        public float bloomWeight4
        {
            get { return _bloomWeight4; }
            set
            {
                if (_bloomWeight4 != value)
                {
                    _bloomWeight4 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _bloomWeight5 = 0.5f;

        public float bloomWeight5
        {
            get { return _bloomWeight5; }
            set
            {
                if (_bloomWeight5 != value)
                {
                    _bloomWeight5 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _bloomBlur = true;

        public bool bloomBlur
        {
            get { return _bloomBlur; }
            set
            {
                if (_bloomBlur != value)
                {
                    _bloomBlur = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _bloomDepthAtten = 0;

        public float bloomDepthAtten
        {
            get { return _bloomDepthAtten; }
            set
            {
                if (_bloomDepthAtten != value)
                {
                    _bloomDepthAtten = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(-1f, 1f)]
        float
                        _bloomLayerZBias = 0;

        public float bloomLayerZBias
        {
            get { return _bloomLayerZBias; }
            set
            {
                if (_bloomLayerZBias != value)
                {
                    _bloomLayerZBias = Mathf.Clamp(value, -1f, 1f);
                    UpdateMaterialProperties();
                }
            }
        }



        #endregion

        #region Anamorphic Flares


        [SerializeField]
        bool
                        _anamorphicFlares = false;

        public bool anamorphicFlares
        {
            get { return _anamorphicFlares; }
            set
            {
                if (_anamorphicFlares != value)
                {
                    _anamorphicFlares = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 10f)]
        float
                        _anamorphicFlaresIntensity = 1f;

        public float anamorphicFlaresIntensity
        {
            get { return _anamorphicFlaresIntensity; }
            set
            {
                if (_anamorphicFlaresIntensity != value)
                {
                    _anamorphicFlaresIntensity = Mathf.Abs(value);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _anamorphicFlaresAntiflicker = false;

        public bool anamorphicFlaresAntiflicker
        {
            get { return _anamorphicFlaresAntiflicker; }
            set
            {
                if (_anamorphicFlaresAntiflicker != value)
                {
                    _anamorphicFlaresAntiflicker = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        bool
                        _anamorphicFlaresUltra = false;

        public bool anamorphicFlaresUltra
        {
            get { return _anamorphicFlaresUltra; }
            set
            {
                if (_anamorphicFlaresUltra != value)
                {
                    _anamorphicFlaresUltra = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 5f)]
        float
                        _anamorphicFlaresThreshold = 0.75f;

        public float anamorphicFlaresThreshold
        {
            get { return _anamorphicFlaresThreshold; }
            set
            {
                if (_anamorphicFlaresThreshold != value)
                {
                    _anamorphicFlaresThreshold = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.1f, 2f)]
        float
                        _anamorphicFlaresSpread = 1f;

        public float anamorphicFlaresSpread
        {
            get { return _anamorphicFlaresSpread; }
            set
            {
                if (_anamorphicFlaresSpread != value)
                {
                    _anamorphicFlaresSpread = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _anamorphicFlaresVertical = false;

        public bool anamorphicFlaresVertical
        {
            get { return _anamorphicFlaresVertical; }
            set
            {
                if (_anamorphicFlaresVertical != value)
                {
                    _anamorphicFlaresVertical = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Color
                        _anamorphicFlaresTint = new Color(0.5f, 0.5f, 1f, 0f);

        public Color anamorphicFlaresTint
        {
            get { return _anamorphicFlaresTint; }
            set
            {
                if (_anamorphicFlaresTint != value)
                {
                    _anamorphicFlaresTint = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        bool
        _anamorphicFlaresBlur = true;

        public bool anamorphicFlaresBlur
        {
            get { return _anamorphicFlaresBlur; }
            set
            {
                if (_anamorphicFlaresBlur != value)
                {
                    _anamorphicFlaresBlur = value;
                    UpdateMaterialProperties();
                }
            }
        }


        #endregion

        #region Depth of Field


        [SerializeField]
        bool
                        _depthOfField = false;

        public bool depthOfField
        {
            get { return _depthOfField; }
            set
            {
                if (_depthOfField != value)
                {
                    _depthOfField = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _depthOfFieldTransparencySupport = false;

        public bool depthOfFieldTransparencySupport
        {
            get { return _depthOfFieldTransparencySupport; }
            set
            {
                if (_depthOfFieldTransparencySupport != value)
                {
                    _depthOfFieldTransparencySupport = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Transform
                        _depthOfFieldTargetFocus;

        public Transform depthOfFieldTargetFocus
        {
            get { return _depthOfFieldTargetFocus; }
            set
            {
                if (_depthOfFieldTargetFocus != value)
                {
                    _depthOfFieldTargetFocus = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _depthOfFieldDebug = false;

        public bool depthOfFieldDebug
        {
            get { return _depthOfFieldDebug; }
            set
            {
                if (_depthOfFieldDebug != value)
                {
                    _depthOfFieldDebug = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _depthOfFieldAutofocus = false;

        public bool depthOfFieldAutofocus
        {
            get { return _depthOfFieldAutofocus; }
            set
            {
                if (_depthOfFieldAutofocus != value)
                {
                    _depthOfFieldAutofocus = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        float
                        _depthOfFieldAutofocusMinDistance = 0;

        public float depthOfFieldAutofocusMinDistance
        {
            get { return _depthOfFieldAutofocusMinDistance; }
            set
            {
                if (_depthOfFieldAutofocusMinDistance != value)
                {
                    _depthOfFieldAutofocusMinDistance = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        float
                        _depthOfFieldAutofocusMaxDistance = 10000;

        public float depthOfFieldAutofocusMaxDistance
        {
            get { return _depthOfFieldAutofocusMaxDistance; }
            set
            {
                if (_depthOfFieldAutofocusMaxDistance != value)
                {
                    _depthOfFieldAutofocusMaxDistance = value;
                    UpdateMaterialProperties();
                }
            }
        }



        [SerializeField]
        LayerMask
                        _depthOfFieldAutofocusLayerMask = -1;

        public LayerMask depthOfFieldAutofocusLayerMask
        {
            get { return _depthOfFieldAutofocusLayerMask; }
            set
            {
                if (_depthOfFieldAutofocusLayerMask != value)
                {
                    _depthOfFieldAutofocusLayerMask = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        LayerMask
                        _depthOfFieldExclusionLayerMask = 0;

        public LayerMask depthOfFieldExclusionLayerMask
        {
            get { return _depthOfFieldExclusionLayerMask; }
            set
            {
                if (_depthOfFieldExclusionLayerMask != value)
                {
                    _depthOfFieldExclusionLayerMask = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(1, 4)]
        float
                        _depthOfFieldExclusionLayerMaskDownsampling = 1f;

        public float depthOfFieldExclusionLayerMaskDownsampling
        {
            get { return _depthOfFieldExclusionLayerMaskDownsampling; }
            set
            {
                if (_depthOfFieldExclusionLayerMaskDownsampling != value)
                {
                    _depthOfFieldExclusionLayerMaskDownsampling = Mathf.Max(value, 1f);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(1, 4)]
        float
                        _depthOfFieldTransparencySupportDownsampling = 1f;

        public float depthOfFieldTransparencySupportDownsampling
        {
            get { return _depthOfFieldTransparencySupportDownsampling; }
            set
            {
                if (_depthOfFieldTransparencySupportDownsampling != value)
                {
                    _depthOfFieldTransparencySupportDownsampling = Mathf.Max(value, 1f);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.9f, 1f)]
        float
                        _depthOfFieldExclusionBias = 0.99f;

        public float depthOfFieldExclusionBias
        {
            get { return _depthOfFieldExclusionBias; }
            set
            {
                if (_depthOfFieldExclusionBias != value)
                {
                    _depthOfFieldExclusionBias = Mathf.Clamp01(value);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(1f, 100f)]
        float
                        _depthOfFieldDistance = 1f;

        public float depthOfFieldDistance
        {
            get { return _depthOfFieldDistance; }
            set
            {
                if (_depthOfFieldDistance != value)
                {
                    _depthOfFieldDistance = Mathf.Max(value, 1f);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.001f, 5f)]
        float
                        _depthOfFieldFocusSpeed = 1f;

        public float depthOfFieldFocusSpeed
        {
            get { return _depthOfFieldFocusSpeed; }
            set
            {
                if (_depthOfFieldFocusSpeed != value)
                {
                    _depthOfFieldFocusSpeed = Mathf.Clamp(value, 0.001f, 1f);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(1, 5)]
        int
                        _depthOfFieldDownsampling = 2;

        public int depthOfFieldDownsampling
        {
            get { return _depthOfFieldDownsampling; }
            set
            {
                if (_depthOfFieldDownsampling != value)
                {
                    _depthOfFieldDownsampling = Mathf.Max(value, 1);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(2, 16)]
        int
                        _depthOfFieldMaxSamples = 4;

        public int depthOfFieldMaxSamples
        {
            get { return _depthOfFieldMaxSamples; }
            set
            {
                if (_depthOfFieldMaxSamples != value)
                {
                    _depthOfFieldMaxSamples = Mathf.Max(value, 2);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.005f, 0.5f)]
        float
                        _depthOfFieldFocalLength = 0.050f;

        public float depthOfFieldFocalLength
        {
            get { return _depthOfFieldFocalLength; }
            set
            {
                if (_depthOfFieldFocalLength != value)
                {
                    _depthOfFieldFocalLength = Mathf.Abs(value);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        float
                        _depthOfFieldAperture = 2.8f;

        public float depthOfFieldAperture
        {
            get { return _depthOfFieldAperture; }
            set
            {
                if (_depthOfFieldAperture != value)
                {
                    _depthOfFieldAperture = Mathf.Abs(value);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _depthOfFieldForegroundBlur = true;

        public bool depthOfFieldForegroundBlur
        {
            get { return _depthOfFieldForegroundBlur; }
            set
            {
                if (_depthOfFieldForegroundBlur != value)
                {
                    _depthOfFieldForegroundBlur = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _depthOfFieldForegroundBlurHQ;

        public bool depthOfFieldForegroundBlurHQ
        {
            get { return _depthOfFieldForegroundBlurHQ; }
            set
            {
                if (_depthOfFieldForegroundBlurHQ != value)
                {
                    _depthOfFieldForegroundBlurHQ = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        float
                        _depthOfFieldForegroundDistance = 0.25f;

        public float depthOfFieldForegroundDistance
        {
            get { return _depthOfFieldForegroundDistance; }
            set
            {
                if (_depthOfFieldForegroundDistance != value)
                {
                    _depthOfFieldForegroundDistance = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool
                        _depthOfFieldBokeh = true;

        public bool depthOfFieldBokeh
        {
            get { return _depthOfFieldBokeh; }
            set
            {
                if (_depthOfFieldBokeh != value)
                {
                    _depthOfFieldBokeh = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.5f, 3f)]
        float
                        _depthOfFieldBokehThreshold = 1f;

        public float depthOfFieldBokehThreshold
        {
            get { return _depthOfFieldBokehThreshold; }
            set
            {
                if (_depthOfFieldBokehThreshold != value)
                {
                    _depthOfFieldBokehThreshold = Mathf.Max(value, 0f);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 8f)]
        float
                        _depthOfFieldBokehIntensity = 2f;

        public float depthOfFieldBokehIntensity
        {
            get { return _depthOfFieldBokehIntensity; }
            set
            {
                if (_depthOfFieldBokehIntensity != value)
                {
                    _depthOfFieldBokehIntensity = Mathf.Max(value, 0);
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        float
                        _depthOfFieldMaxBrightness = 1000f;

        public float depthOfFieldMaxBrightness
        {
            get { return _depthOfFieldMaxBrightness; }
            set
            {
                if (_depthOfFieldMaxBrightness != value)
                {
                    _depthOfFieldMaxBrightness = Mathf.Abs(value);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        FilterMode _depthOfFieldFilterMode = FilterMode.Bilinear;

        public FilterMode depthOfFieldFilterMode
        {
            get { return _depthOfFieldFilterMode; }
            set
            {
                if (_depthOfFieldFilterMode != value)
                {
                    _depthOfFieldFilterMode = value;
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region Eye Adaptation


        [SerializeField]
        bool
                        _eyeAdaptation = false;

        public bool eyeAdaptation
        {
            get { return _eyeAdaptation; }
            set
            {
                if (_eyeAdaptation != value)
                {
                    _eyeAdaptation = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _eyeAdaptationMinExposure = 0.2f;

        public float eyeAdaptationMinExposure
        {
            get { return _eyeAdaptationMinExposure; }
            set
            {
                if (_eyeAdaptationMinExposure != value)
                {
                    _eyeAdaptationMinExposure = Mathf.Clamp01(value);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(1f, 100f)]
        float
                        _eyeAdaptationMaxExposure = 5f;

        public float eyeAdaptationMaxExposure
        {
            get { return _eyeAdaptationMaxExposure; }
            set
            {
                if (_eyeAdaptationMaxExposure != value)
                {
                    _eyeAdaptationMaxExposure = Mathf.Clamp(value, 1f, 100f);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _eyeAdaptationSpeedToLight = 0.4f;

        public float eyeAdaptationSpeedToLight
        {
            get { return _eyeAdaptationSpeedToLight; }
            set
            {
                if (_eyeAdaptationSpeedToLight != value)
                {
                    _eyeAdaptationSpeedToLight = Mathf.Clamp01(value);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _eyeAdaptationSpeedToDark = 0.2f;

        public float eyeAdaptationSpeedToDark
        {
            get { return _eyeAdaptationSpeedToDark; }
            set
            {
                if (_eyeAdaptationSpeedToDark != value)
                {
                    _eyeAdaptationSpeedToDark = Mathf.Clamp01(value);
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region Purkinje effect

        [SerializeField]
        bool
                        _purkinje = false;

        public bool purkinje
        {
            get { return _purkinje; }
            set
            {
                if (_purkinje != value)
                {
                    _purkinje = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 5f)]
        float
                        _purkinjeAmount = 1f;

        public float purkinjeAmount
        {
            get { return _purkinjeAmount; }
            set
            {
                if (_purkinjeAmount != value)
                {
                    _purkinjeAmount = Mathf.Clamp(value, 0f, 5f);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _purkinjeLuminanceThreshold = 0.15f;

        public float purkinjeLuminanceThreshold
        {
            get { return _purkinjeLuminanceThreshold; }
            set
            {
                if (purkinjeLuminanceThreshold != value)
                {
                    _purkinjeLuminanceThreshold = Mathf.Clamp(value, 0f, 1f);
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion

        #region Tonemapping


        [SerializeField]
        BEAUTIFY_TMO
                        _tonemap = BEAUTIFY_TMO.Linear;

        public BEAUTIFY_TMO tonemap
        {
            get { return _tonemap; }
            set
            {
                if (_tonemap != value)
                {
                    _tonemap = value;
                    if (_tonemap == BEAUTIFY_TMO.ACES)
                    {
                        _saturate = 0;
                        _contrast = 1f;
                    }
                    UpdateMaterialProperties();
                }
            }
        }

        #endregion


        #region Sun Flares

        [SerializeField]
        bool
                        _sunFlares = false;

        public bool sunFlares
        {
            get { return _sunFlares; }
            set
            {
                if (_sunFlares != value)
                {
                    _sunFlares = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        Transform
                        _sun;

        public Transform sun
        {
            get { return _sun; }
            set
            {
                if (_sun != value)
                {
                    _sun = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        LayerMask
        _sunFlaresLayerMask = -1;

        public LayerMask sunFlaresLayerMask
        {
            get { return _sunFlaresLayerMask; }
            set
            {
                if (_sunFlaresLayerMask != value)
                {
                    _sunFlaresLayerMask = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresIntensity = 1.0f;

        public float sunFlaresIntensity
        {
            get { return _sunFlaresIntensity; }
            set
            {
                if (_sunFlaresIntensity != value)
                {
                    _sunFlaresIntensity = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresSolarWindSpeed = 0.01f;

        public float sunFlaresSolarWindSpeed
        {
            get { return _sunFlaresSolarWindSpeed; }
            set
            {
                if (_sunFlaresSolarWindSpeed != value)
                {
                    _sunFlaresSolarWindSpeed = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Color
                        _sunFlaresTint = new Color(1, 1, 1);

        public Color sunFlaresTint
        {
            get { return _sunFlaresTint; }
            set
            {
                if (_sunFlaresTint != value)
                {
                    _sunFlaresTint = value;
                    UpdateMaterialProperties();
                }
            }
        }




        [SerializeField]
        [Range(1, 5)]
        int
                        _sunFlaresDownsampling = 1;

        public int sunFlaresDownsampling
        {
            get { return _sunFlaresDownsampling; }
            set
            {
                if (_sunFlaresDownsampling != value)
                {
                    _sunFlaresDownsampling = Mathf.Max(value, 1);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresSunIntensity = 0.1f;

        public float sunFlaresSunIntensity
        {
            get { return _sunFlaresSunIntensity; }
            set
            {
                if (_sunFlaresSunIntensity != value)
                {
                    _sunFlaresSunIntensity = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresSunDiskSize = 0.05f;

        public float sunFlaresSunDiskSize
        {
            get { return _sunFlaresSunDiskSize; }
            set
            {
                if (_sunFlaresSunDiskSize != value)
                {
                    _sunFlaresSunDiskSize = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 10f)]
        float
                        _sunFlaresSunRayDiffractionIntensity = 3.5f;

        public float sunFlaresSunRayDiffractionIntensity
        {
            get { return _sunFlaresSunRayDiffractionIntensity; }
            set
            {
                if (_sunFlaresSunRayDiffractionIntensity != value)
                {
                    _sunFlaresSunRayDiffractionIntensity = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresSunRayDiffractionThreshold = 0.13f;

        public float sunFlaresSunRayDiffractionThreshold
        {
            get { return _sunFlaresSunRayDiffractionThreshold; }
            set
            {
                if (_sunFlaresSunRayDiffractionThreshold != value)
                {
                    _sunFlaresSunRayDiffractionThreshold = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 0.2f)]
        float
                        _sunFlaresCoronaRays1Length = 0.02f;

        public float sunFlaresCoronaRays1Length
        {
            get { return _sunFlaresCoronaRays1Length; }
            set
            {
                if (_sunFlaresCoronaRays1Length != value)
                {
                    _sunFlaresCoronaRays1Length = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(2, 30)]
        int
                        _sunFlaresCoronaRays1Streaks = 12;

        public int sunFlaresCoronaRays1Streaks
        {
            get { return _sunFlaresCoronaRays1Streaks; }
            set
            {
                if (_sunFlaresCoronaRays1Streaks != value)
                {
                    _sunFlaresCoronaRays1Streaks = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 0.1f)]
        float
                        _sunFlaresCoronaRays1Spread = 0.001f;

        public float sunFlaresCoronaRays1Spread
        {
            get { return _sunFlaresCoronaRays1Spread; }
            set
            {
                if (_sunFlaresCoronaRays1Spread != value)
                {
                    _sunFlaresCoronaRays1Spread = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 2f * Mathf.PI)]
        float
                        _sunFlaresCoronaRays1AngleOffset = 0f;

        public float sunFlaresCoronaRays1AngleOffset
        {
            get { return _sunFlaresCoronaRays1AngleOffset; }
            set
            {
                if (_sunFlaresCoronaRays1AngleOffset != value)
                {
                    _sunFlaresCoronaRays1AngleOffset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 0.2f)]
        float
                        _sunFlaresCoronaRays2Length = 0.05f;

        public float sunFlaresCoronaRays2Length
        {
            get { return _sunFlaresCoronaRays2Length; }
            set
            {
                if (_sunFlaresCoronaRays2Length != value)
                {
                    _sunFlaresCoronaRays2Length = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(2, 30)]
        int
                        _sunFlaresCoronaRays2Streaks = 12;

        public int sunFlaresCoronaRays2Streaks
        {
            get { return _sunFlaresCoronaRays2Streaks; }
            set
            {
                if (_sunFlaresCoronaRays2Streaks != value)
                {
                    _sunFlaresCoronaRays2Streaks = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 0.1f)]
        float
                        _sunFlaresCoronaRays2Spread = 0.1f;

        public float sunFlaresCoronaRays2Spread
        {
            get { return _sunFlaresCoronaRays2Spread; }
            set
            {
                if (_sunFlaresCoronaRays2Spread != value)
                {
                    _sunFlaresCoronaRays2Spread = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 2f * Mathf.PI)]
        float
                        _sunFlaresCoronaRays2AngleOffset = 0f;

        public float sunFlaresCoronaRays2AngleOffset
        {
            get { return _sunFlaresCoronaRays2AngleOffset; }
            set
            {
                if (_sunFlaresCoronaRays2AngleOffset != value)
                {
                    _sunFlaresCoronaRays2AngleOffset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresGhosts1Size = 0.03f;

        public float sunFlaresGhosts1Size
        {
            get { return _sunFlaresGhosts1Size; }
            set
            {
                if (_sunFlaresGhosts1Size != value)
                {
                    _sunFlaresGhosts1Size = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(-3f, 3f)]
        float
                        _sunFlaresGhosts1Offset = 1.04f;

        public float sunFlaresGhosts1Offset
        {
            get { return _sunFlaresGhosts1Offset; }
            set
            {
                if (_sunFlaresGhosts1Offset != value)
                {
                    _sunFlaresGhosts1Offset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresGhosts1Brightness = 0.037f;

        public float sunFlaresGhosts1Brightness
        {
            get { return _sunFlaresGhosts1Brightness; }
            set
            {
                if (_sunFlaresGhosts1Brightness != value)
                {
                    _sunFlaresGhosts1Brightness = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresGhosts2Size = 0.1f;

        public float sunFlaresGhosts2Size
        {
            get { return _sunFlaresGhosts2Size; }
            set
            {
                if (_sunFlaresGhosts2Size != value)
                {
                    _sunFlaresGhosts2Size = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(-3f, 3f)]
        float
                        _sunFlaresGhosts2Offset = 0.71f;

        public float sunFlaresGhosts2Offset
        {
            get { return _sunFlaresGhosts2Offset; }
            set
            {
                if (_sunFlaresGhosts2Offset != value)
                {
                    _sunFlaresGhosts2Offset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresGhosts2Brightness = 0.03f;

        public float sunFlaresGhosts2Brightness
        {
            get { return _sunFlaresGhosts2Brightness; }
            set
            {
                if (_sunFlaresGhosts2Brightness != value)
                {
                    _sunFlaresGhosts2Brightness = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresGhosts3Size = 0.24f;

        public float sunFlaresGhosts3Size
        {
            get { return _sunFlaresGhosts3Size; }
            set
            {
                if (_sunFlaresGhosts3Size != value)
                {
                    _sunFlaresGhosts3Size = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(-3f, 3f)]
        float
                        _sunFlaresGhosts3Brightness = 0.025f;

        public float sunFlaresGhosts3Brightness
        {
            get { return _sunFlaresGhosts3Brightness; }
            set
            {
                if (_sunFlaresGhosts3Brightness != value)
                {
                    _sunFlaresGhosts3Brightness = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresGhosts3Offset = 0.31f;

        public float sunFlaresGhosts3Offset
        {
            get { return _sunFlaresGhosts3Offset; }
            set
            {
                if (_sunFlaresGhosts3Offset != value)
                {
                    _sunFlaresGhosts3Offset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresGhosts4Size = 0.016f;

        public float sunFlaresGhosts4Size
        {
            get { return _sunFlaresGhosts4Size; }
            set
            {
                if (_sunFlaresGhosts4Size != value)
                {
                    _sunFlaresGhosts4Size = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(-3f, 3f)]
        float
                        _sunFlaresGhosts4Offset = 0f;

        public float sunFlaresGhosts4Offset
        {
            get { return _sunFlaresGhosts4Offset; }
            set
            {
                if (_sunFlaresGhosts4Offset != value)
                {
                    _sunFlaresGhosts4Offset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresGhosts4Brightness = 0.017f;

        public float sunFlaresGhosts4Brightness
        {
            get { return _sunFlaresGhosts4Brightness; }
            set
            {
                if (_sunFlaresGhosts4Brightness != value)
                {
                    _sunFlaresGhosts4Brightness = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresHaloOffset = 0.22f;

        public float sunFlaresHaloOffset
        {
            get { return _sunFlaresHaloOffset; }
            set
            {
                if (_sunFlaresHaloOffset != value)
                {
                    _sunFlaresHaloOffset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 50f)]
        float
                        _sunFlaresHaloAmplitude = 15.1415f;

        public float sunFlaresHaloAmplitude
        {
            get { return _sunFlaresHaloAmplitude; }
            set
            {
                if (_sunFlaresHaloAmplitude != value)
                {
                    _sunFlaresHaloAmplitude = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        float
                        _sunFlaresHaloIntensity = 0.01f;

        public float sunFlaresHaloIntensity
        {
            get { return _sunFlaresHaloIntensity; }
            set
            {
                if (_sunFlaresHaloIntensity != value)
                {
                    _sunFlaresHaloIntensity = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        bool
                        _sunFlaresRotationDeadZone = false;

        public bool sunFlaresRotationDeadZone
        {
            get { return _sunFlaresRotationDeadZone; }
            set
            {
                if (_sunFlaresRotationDeadZone != value)
                {
                    _sunFlaresRotationDeadZone = value;
                    UpdateMaterialProperties();
                }
            }
        }





        #endregion


        #region Blur


        [SerializeField]
        bool
                        _blur = false;

        public bool blur
        {
            get { return _blur; }
            set
            {
                if (_blur != value)
                {
                    _blur = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(0, 4f)]
        float
                        _blurIntensity = 1f;

        public float blurIntensity
        {
            get { return _blurIntensity; }
            set
            {
                if (_blurIntensity != value)
                {
                    _blurIntensity = value;
                    UpdateMaterialProperties();
                }
            }
        }


        #endregion


        #region Pixelate

        [SerializeField]
        [Range(1, 256)]
        int
                        _pixelateAmount = 1;

        public int pixelateAmount
        {
            get { return _pixelateAmount; }
            set
            {
                if (_pixelateAmount != value)
                {
                    _pixelateAmount = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        bool
                        _pixelateDownscale = false;

        public bool pixelateDownscale
        {
            get { return _pixelateDownscale; }
            set
            {
                if (_pixelateDownscale != value)
                {
                    _pixelateDownscale = value;
                    UpdateMaterialProperties();
                }
            }
        }



        #endregion

        public static Beautify instance
        {
            get
            {
                if (_beautify == null)
                {
                    foreach (Camera camera in Camera.allCameras)
                    {
                        _beautify = camera.GetComponent<Beautify>();
                        if (_beautify != null)
                            break;
                    }
                }
                return _beautify;
            }
        }

        public Camera cameraEffect { get { return currentCamera; } }

        // Internal stuff **************************************************************************************************************

        public bool isDirty;
        static Beautify _beautify;

        // Shader keywords
        public const string SKW_BLOOM = "BEAUTIFY_BLOOM";
        public const string SKW_LUT = "BEAUTIFY_LUT";
        public const string SKW_NIGHT_VISION = "BEAUTIFY_NIGHT_VISION";
        public const string SKW_THERMAL_VISION = "BEAUTIFY_THERMAL_VISION";
        public const string SKW_OUTLINE = "BEAUTIFY_OUTLINE";
        public const string SKW_FRAME = "BEAUTIFY_FRAME";
        public const string SKW_FRAME_MASK = "BEAUTIFY_FRAME_MASK";
        public const string SKW_DALTONIZE = "BEAUTIFY_DALTONIZE";
        public const string SKW_DIRT = "BEAUTIFY_DIRT";
        public const string SKW_VIGNETTING = "BEAUTIFY_VIGNETTING";
        public const string SKW_VIGNETTING_MASK = "BEAUTIFY_VIGNETTING_MASK";
        public const string SKW_DEPTH_OF_FIELD = "BEAUTIFY_DEPTH_OF_FIELD";
        public const string SKW_DEPTH_OF_FIELD_TRANSPARENT = "BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT";
        public const string SKW_EYE_ADAPTATION = "BEAUTIFY_EYE_ADAPTATION";
        public const string SKW_TONEMAP_ACES = "BEAUTIFY_TONEMAP_ACES";
        public const string SKW_PURKINJE = "BEAUTIFY_PURKINJE";
        public const string SKW_BLOOM_USE_DEPTH = "BEAUTIFY_BLOOM_USE_DEPTH";
        public const string SKW_BLOOM_USE_LAYER = "BEAUTIFY_BLOOM_USE_LAYER";

        Material bMatDesktop, bMatMobile, bMatBasic;

        [SerializeField]
        Material bMat;
        Camera currentCamera;
        Vector3 camPrevForward, camPrevPos;
        float currSens;
        int renderPass;
        RenderTextureFormat rtFormat;
        RenderTexture[] rt, rtAF, rtEA;
        RenderTexture rtEAacum, rtEAHist;
        float dofPrevDistance, dofLastAutofocusDistance;
        Vector4 dofLastBokehData;
        Camera sceneCamera, depthCam;
        GameObject depthCamObj;
        List<string> shaderKeywords;
        Shader depthShader, dofExclusionShader;
        bool shouldUpdateMaterialProperties;
        const string BEAUTIFY_BUILD_HINT = "BeautifyBuildHint62RC1";
        float sunFlareCurrentIntensity;
        Vector4 sunLastScrPos;
        float sunLastRot;
        Texture2D flareNoise;
        RenderTexture dofDepthTexture, dofExclusionTexture, bloomSourceTexture, bloomSourceDepthTexture, pixelateTexture;
        RenderTextureDescriptor rtDescBase;
        float sunFlareTime;
        int dofCurrentLayerMaskValue;

        #region Game loop events

        // Creates a private material used to the effect
        void OnEnable()
        {
            currentCamera = GetComponent<Camera>();
            if (_profile != null)
            {
                _profile.Load(this);
            }
            UpdateMaterialPropertiesNow();

#if UNITY_EDITOR
            if (EditorPrefs.GetInt(BEAUTIFY_BUILD_HINT) == 0)
            {
                EditorPrefs.SetInt(BEAUTIFY_BUILD_HINT, 1);
                EditorUtility.DisplayDialog("Beautify Update", "Beautify shaders have been updated. Please check the 'Shader Options' button in Beautify's inspector for new shader capabilities and disable/enable features to optimize build size and compilation time.\n\nOtherwise when you build the game it will take a long time during shader compilation!", "Ok");
            }
#endif
        }

#if UNITY_5_4
								void Start () {
												// Fix prefab glitch on Unity 5.4
												enabled = false;
												enabled = true;
								}
#endif

        void OnDestroy()
        {
            CleanUpRT();
            if (depthCamObj != null)
            {
                DestroyImmediate(depthCamObj);
                depthCamObj = null;
            }
            if (rtEAacum != null)
                rtEAacum.Release();
            if (rtEAHist != null)
                rtEAHist.Release();
            if (bMatDesktop != null)
            {
                DestroyImmediate(bMatDesktop);
                bMatDesktop = null;
            }
            if (bMatMobile != null)
            {
                DestroyImmediate(bMatMobile);
                bMatMobile = null;
            }
            if (bMatBasic != null)
            {
                DestroyImmediate(bMatBasic);
                bMatBasic = null;
            }
            bMat = null;
        }

        void Reset()
        {
            UpdateMaterialPropertiesNow();
        }

        void LateUpdate()
        {
            if (bMat == null || !Application.isPlaying || _sharpenMotionSensibility <= 0)
                return;

            // Motion sensibility 
            float angleDiff = Vector3.Angle(camPrevForward, currentCamera.transform.forward) * _sharpenMotionSensibility;
            float posDiff = (currentCamera.transform.position - camPrevPos).sqrMagnitude * 10f * _sharpenMotionSensibility;

            float diff = angleDiff + posDiff;
            if (diff > 0.1f)
            {
                camPrevForward = currentCamera.transform.forward;
                camPrevPos = currentCamera.transform.position;
                if (diff > _sharpenMotionSensibility)
                    diff = _sharpenMotionSensibility;
                currSens += diff;
                float min = _sharpen * _sharpenMotionSensibility * 0.75f;
                float max = _sharpen * (1f + _sharpenMotionSensibility) * 0.5f;
                currSens = Mathf.Clamp(currSens, min, max);
            }
            else
            {
                if (currSens <= 0.001f)
                    return;
                currSens *= 0.75f;
            }
            float tempSharpen = Mathf.Clamp(_sharpen - currSens, 0, _sharpen);
            UpdateSharpenParams(tempSharpen);
        }

        void OnPreCull()
        {   // Aquas issue with OnPreRender
            bool bloomLayerMaskUsed = (_bloom || _anamorphicFlares) && _bloomCullingMask != 0;
            if (!enabled || !gameObject.activeSelf || currentCamera == null || bMat == null || (!_depthOfField && !bloomLayerMaskUsed))
                return;

            CleanUpRT();

            if (dofCurrentLayerMaskValue != _depthOfFieldExclusionLayerMask.value)
                shouldUpdateMaterialProperties = true;

            if (depthOfField && (_depthOfFieldTransparencySupport || _depthOfFieldExclusionLayerMask != 0))
            {
                CheckDoFTransparencySupport();
                CheckDoFExclusionMask();
            }
            if (bloomLayerMaskUsed)
            {
                CheckBloomCullingLayer();
            }
        }

        void OnPreRender()
        {

            if (_pixelateDownscale && _pixelateAmount > 1 && rtDescBase.width > 1 && rtDescBase.height > 1)
            {
                RenderTextureDescriptor rtPixDesc = rtDescBase;
                rtPixDesc.width = Mathf.RoundToInt(Mathf.Max(1, currentCamera.pixelWidth / _pixelateAmount));
                float aspectRatio = (float)currentCamera.pixelHeight / currentCamera.pixelWidth;
                rtPixDesc.height = Mathf.Max(1, Mathf.RoundToInt(rtPixDesc.width * aspectRatio));
                pixelateTexture = RenderTexture.GetTemporary(rtPixDesc);
                currentCamera.targetTexture = pixelateTexture;
            }

        }

        void CleanUpRT()
        {
            if (dofDepthTexture != null)
            {
                RenderTexture.ReleaseTemporary(dofDepthTexture);
                dofDepthTexture = null;
            }
            if (dofExclusionTexture != null)
            {
                RenderTexture.ReleaseTemporary(dofExclusionTexture);
                dofExclusionTexture = null;
            }
            if (bloomSourceTexture != null)
            {
                RenderTexture.ReleaseTemporary(bloomSourceTexture);
                bloomSourceTexture = null;
            }
            if (bloomSourceDepthTexture != null)
            {
                RenderTexture.ReleaseTemporary(bloomSourceDepthTexture);
                bloomSourceDepthTexture = null;
            }
            if (pixelateTexture != null)
            {
                RenderTexture.ReleaseTemporary(pixelateTexture);
                pixelateTexture = null;
            }
        }

        void CheckDoFTransparencySupport()
        {
            if (depthCam == null)
            {
                if (depthCamObj == null)
                {
                    depthCamObj = new GameObject("DepthCamera");
                    depthCamObj.hideFlags = HideFlags.HideAndDontSave;
                    depthCam = depthCamObj.AddComponent<Camera>();
                    depthCam.enabled = false;
                }
                else
                {
                    depthCam = depthCamObj.GetComponent<Camera>();
                    if (depthCam == null)
                    {
                        DestroyImmediate(depthCamObj);
                        depthCamObj = null;
                        return;
                    }
                }
            }
            depthCam.CopyFrom(currentCamera);
            depthCam.depthTextureMode = DepthTextureMode.None;
            depthCam.renderingPath = RenderingPath.Forward;
            float downsampling = _depthOfFieldTransparencySupportDownsampling * _depthOfFieldDownsampling;
            dofDepthTexture = RenderTexture.GetTemporary((int)(currentCamera.pixelWidth / downsampling), (int)(currentCamera.pixelHeight / downsampling), 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            dofDepthTexture.filterMode = FilterMode.Point;
            depthCam.backgroundColor = new Color(0.9882353f, 0.4470558f, 0.75f, 0f); // new Color (1, 1, 1, 1);
            depthCam.clearFlags = CameraClearFlags.SolidColor;
            depthCam.targetTexture = dofDepthTexture;
            depthCam.cullingMask = ~_depthOfFieldExclusionLayerMask;
            if (depthShader == null)
            {
                depthShader = Shader.Find("Beautify/CopyDepth");
            }
            depthCam.RenderWithShader(depthShader, "RenderType");
            bMat.SetTexture("_DepthTexture", dofDepthTexture);
        }

        void CheckDoFExclusionMask()
        {
            if (depthCam == null)
            {
                if (depthCamObj == null)
                {
                    depthCamObj = new GameObject("DepthCamera");
                    depthCamObj.hideFlags = HideFlags.HideAndDontSave;
                    depthCam = depthCamObj.AddComponent<Camera>();
                    depthCam.enabled = false;
                }
                else
                {
                    depthCam = depthCamObj.GetComponent<Camera>();
                    if (depthCam == null)
                    {
                        DestroyImmediate(depthCamObj);
                        depthCamObj = null;
                        return;
                    }
                }
            }
            depthCam.CopyFrom(currentCamera);
            depthCam.depthTextureMode = DepthTextureMode.None;
            depthCam.renderingPath = RenderingPath.Forward;
            float downsampling = _depthOfFieldExclusionLayerMaskDownsampling * _depthOfFieldDownsampling;
            dofExclusionTexture = RenderTexture.GetTemporary((int)(currentCamera.pixelWidth / downsampling), (int)(currentCamera.pixelHeight / downsampling), 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            dofExclusionTexture.filterMode = FilterMode.Point;
            depthCam.backgroundColor = new Color(0.9882353f, 0.4470558f, 0.75f, 0f); // new Color (1, 1, 1, 1);
            depthCam.clearFlags = CameraClearFlags.SolidColor;
            depthCam.targetTexture = dofExclusionTexture;
            depthCam.cullingMask = _depthOfFieldExclusionLayerMask;
            if (dofExclusionShader == null)
            {
                dofExclusionShader = Shader.Find("Beautify/CopyDepthBiased");
            }
            depthCam.RenderWithShader(dofExclusionShader, null);
            bMat.SetTexture("_DofExclusionTexture", dofExclusionTexture);
        }

        void CheckBloomCullingLayer()
        {
            // Reuses depth camera
            if (depthCam == null)
            {
                if (depthCamObj == null)
                {
                    depthCamObj = new GameObject("DepthCamera");
                    depthCamObj.hideFlags = HideFlags.HideAndDontSave;
                    depthCam = depthCamObj.AddComponent<Camera>();
                    depthCam.enabled = false;
                }
                else
                {
                    depthCam = depthCamObj.GetComponent<Camera>();
                    if (depthCam == null)
                    {
                        DestroyImmediate(depthCamObj);
                        depthCamObj = null;
                        return;
                    }
                }
            }
            depthCam.CopyFrom(currentCamera);
            depthCam.depthTextureMode = DepthTextureMode.None;
            depthCam.allowMSAA = false;
            depthCam.allowHDR = false;
            int size;
            if (_quality == BEAUTIFY_QUALITY.BestPerformance)
            {
                size = 256;
            }
            else
            {
                size = _bloomUltra ? (int)(currentCamera.pixelHeight / 4) * 4 : 512;
                size = (int)(size * (1f / _bloomLayerMaskDownsampling) / 4) * 4;
            }
            float aspectRatio = (float)currentCamera.pixelHeight / currentCamera.pixelWidth;
            bloomSourceTexture = RenderTexture.GetTemporary(size, Mathf.Max(1, (int)(size * aspectRatio)), 0, rtFormat);
            bloomSourceDepthTexture = RenderTexture.GetTemporary(bloomSourceTexture.width, bloomSourceTexture.height, 24, RenderTextureFormat.Depth);
            depthCam.clearFlags = CameraClearFlags.SolidColor;
#if UNITY_5_4_OR_NEWER
            depthCam.stereoTargetEye = StereoTargetEyeMask.None;
#endif
            depthCam.renderingPath = RenderingPath.Forward; // currently this feature does not work in deferred
            depthCam.backgroundColor = Color.black;
            depthCam.SetTargetBuffers(bloomSourceTexture.colorBuffer, bloomSourceDepthTexture.depthBuffer);
            depthCam.cullingMask = _bloomCullingMask;
            depthCam.Render();
            bMat.SetTexture("_BloomSourceTex", bloomSourceTexture);
            bMat.SetTexture("_BloomSourceDepth", bloomSourceDepthTexture);
        }

        protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
        {

            if (bMat == null || !enabled)
            {
                Graphics.Blit(source, destination);
                return;
            }

            if (shouldUpdateMaterialProperties)
            {
                UpdateMaterialPropertiesNow();
            }

            bool allowExtraEffects = (_quality != BEAUTIFY_QUALITY.Basic);

            // Copy source settings; RRTT will be created using descriptor to take advantage of the vrUsage field.
            rtDescBase = source.descriptor;
            rtDescBase.msaaSamples = 1;
            rtDescBase.colorFormat = rtFormat;
            rtDescBase.depthBufferBits = 0;

            // Prepare compare & final blur buffer
            RenderTexture rtBeauty = null;
            RenderTexture rtBlurTex = null;
            RenderTexture rtCustomBloom = null;

            float aspectRatio = (float)source.height / source.width;

            bool doFinalBlur = _blur && _blurIntensity > 0 && allowExtraEffects;
            if (renderPass == 0 || doFinalBlur)
            {
                if (doFinalBlur)
                {
                    int size;
                    if (_blurIntensity < 1f)
                    {
                        size = (int)Mathf.Lerp(currentCamera.pixelWidth, 512, _blurIntensity);
                        if (_quality == BEAUTIFY_QUALITY.BestPerformance)
                            size /= 2;
                    }
                    else
                    {
                        size = _quality == BEAUTIFY_QUALITY.BestQuality ? 512 : 256;
                        size = (int)(size / _blurIntensity);
                    }
                    //																				float aspectRatio = (float)currentCamera.pixelHeight / currentCamera.pixelWidth;
                    RenderTextureDescriptor rtBlurDesc = rtDescBase;
                    rtBlurDesc.width = size;
                    rtBlurDesc.height = Mathf.Max(1, (int)(size * aspectRatio));
                    rtBlurTex = RenderTexture.GetTemporary(rtBlurDesc);
                    if (renderPass == 0)
                    {
                        rtBeauty = RenderTexture.GetTemporary(rtBlurDesc);
                    }
                }
                else
                {
                    rtBeauty = RenderTexture.GetTemporary(rtDescBase); //source.descriptor);
                }
            }

            RenderTexture rtPixelated = null;
            RenderTexture rtDoF = null;

            if (allowExtraEffects)
            {
                // Pixelate
                if (_pixelateAmount > 1)
                {
                    source.filterMode = FilterMode.Point;
                    if (!_pixelateDownscale)
                    {
                        RenderTextureDescriptor rtPixDesc = rtDescBase;
                        rtPixDesc.width = Mathf.RoundToInt(Mathf.Max(1, source.width / _pixelateAmount));
                        rtPixDesc.height = Mathf.Max(1, Mathf.RoundToInt(rtPixDesc.width * aspectRatio));
                        rtPixelated = RenderTexture.GetTemporary(rtPixDesc); //rtPixDesc.width, rtPixDesc.height, -1);
                        rtPixelated.filterMode = FilterMode.Point;
                        Graphics.Blit(source, rtPixelated, bMat, 22);
                        source = rtPixelated;
                    }
                }


                // DoF!
                if (_depthOfField)
                {
#if UNITY_EDITOR
                    if (sceneCamera == null && Camera.current != null && Camera.current.name.Equals("SceneCamera"))
                    {
                        sceneCamera = Camera.current;
                    }

                    if (Camera.current != sceneCamera)
                    {
                        if (!bMat.IsKeywordEnabled(SKW_DEPTH_OF_FIELD))
                        {
                            bMat.EnableKeyword(SKW_DEPTH_OF_FIELD);
                        }
                        if ((_depthOfFieldTransparencySupport || _depthOfFieldExclusionLayerMask != 0) && !bMat.IsKeywordEnabled(SKW_DEPTH_OF_FIELD_TRANSPARENT))
                        {
                            bMat.EnableKeyword(SKW_DEPTH_OF_FIELD_TRANSPARENT);
                        }
#endif
                        UpdateDepthOfFieldData();

                        int pass = _quality == BEAUTIFY_QUALITY.BestQuality ? 12 : 6;
                        RenderTextureDescriptor rtDofDescriptor = rtDescBase;
                        rtDofDescriptor.width = source.width / _depthOfFieldDownsampling;
                        rtDofDescriptor.height = source.height / _depthOfFieldDownsampling;
                        rtDoF = RenderTexture.GetTemporary(rtDofDescriptor);
                        rtDoF.filterMode = _depthOfFieldFilterMode;
                        Graphics.Blit(source, rtDoF, bMat, pass);

                        if (_quality == BEAUTIFY_QUALITY.BestQuality)
                        {
                            pass = _depthOfFieldBokeh ? 14 : 19;
                        }
                        else
                        {
                            pass = _depthOfFieldBokeh ? 8 : 15;
                        
                        }
                        if (_quality == BEAUTIFY_QUALITY.BestQuality && _depthOfFieldForegroundBlur && _depthOfFieldForegroundBlurHQ)
                        {
                            BlurThisAlpha(rtDoF, 16);
                        }
                        BlurThisDoF(rtDoF, pass);

                        if (_depthOfFieldDebug)
                        {
                            source.MarkRestoreExpected();
                            pass = _quality == BEAUTIFY_QUALITY.BestQuality ? 13 : 7;
                            Graphics.Blit(rtDoF, destination, bMat, pass);
                            RenderTexture.ReleaseTemporary(rtDoF);
                            return;
                        }

                        bMat.SetTexture("_DoFTex", rtDoF);
#if UNITY_EDITOR
                    }
                    else
                    {
                        // Cancels DoF
                        if (bMat.IsKeywordEnabled(SKW_DEPTH_OF_FIELD))
                        {
                            bMat.DisableKeyword(SKW_DEPTH_OF_FIELD); // .SetVector ("_BokehData", new Vector4 (10000, 0, 0, 0));
                        }
                        if (bMat.IsKeywordEnabled(SKW_DEPTH_OF_FIELD_TRANSPARENT))
                        {
                            bMat.DisableKeyword(SKW_DEPTH_OF_FIELD_TRANSPARENT); // .SetVector ("_BokehData", new Vector4 (10000, 0, 0, 0));
                        }
                    }
#endif
                }
            }

            bool sunFlareEnabled = _sunFlares && _sun != null;
            if (allowExtraEffects && (_lensDirt || _bloom || _anamorphicFlares || sunFlareEnabled))
            {
                RenderTexture rtBloom = null;

                int PYRAMID_COUNT, size;
                if (_quality == BEAUTIFY_QUALITY.BestPerformance)
                {
                    PYRAMID_COUNT = 4;
                    size = 256;
                }
                else
                {
                    PYRAMID_COUNT = 5;
                    size = _bloomUltra ? (int)(source.height / 4) * 4 : 512;
                }

                // Bloom buffers
                if (rt == null || rt.Length != PYRAMID_COUNT + 1)
                {
                    rt = new RenderTexture[PYRAMID_COUNT + 1];
                }
                // Anamorphic flare buffers
                if (rtAF == null || rtAF.Length != PYRAMID_COUNT + 1)
                {
                    rtAF = new RenderTexture[PYRAMID_COUNT + 1];
                }

                //bool useBloomSourceTexture = (_bloom || _anamorphicFlares) && _bloomCullingMask != 0 && bloomSourceTexture;
                if (_bloom || (_lensDirt && !_anamorphicFlares))
                {
                    UpdateMaterialBloomIntensityAndThreshold();
                    RenderTextureDescriptor rtBloomDescriptor = rtDescBase;
                    for (int k = 0; k <= PYRAMID_COUNT; k++)
                    {
                        rtBloomDescriptor.width = size;
                        rtBloomDescriptor.height = Mathf.Max(1, (int)(size * aspectRatio));
                        rt[k] = RenderTexture.GetTemporary(rtBloomDescriptor);
                        size /= 2;
                    }
                    rtBloom = rt[0];

                    if (_quality == BEAUTIFY_QUALITY.BestQuality && _bloomAntiflicker)
                    {
                        Graphics.Blit(source, rt[0], bMat, 9);
                    }
                    else
                    {
                        Graphics.Blit(source, rt[0], bMat, 2);
                    }

                    BlurThis(rt[0]);

                    for (int k = 0; k < PYRAMID_COUNT; k++)
                    {
                        if (_quality == BEAUTIFY_QUALITY.BestPerformance)
                        {
                            if (_bloomBlur)
                            {
                                BlurThisDownscaling(rt[k], rt[k + 1]);
                            }
                            else
                            {
                                Graphics.Blit(rt[k], rt[k + 1], bMat, 18);
                            }
                        }
                        else
                        {
                            Graphics.Blit(rt[k], rt[k + 1], bMat, 7);
                            BlurThis(rt[k + 1]);
                        }
                    }

                    if (_bloom)
                    {
                        for (int k = PYRAMID_COUNT; k > 0; k--)
                        {
                            rt[k - 1].MarkRestoreExpected();
                            Graphics.Blit(rt[k], rt[k - 1], bMat, _quality == BEAUTIFY_QUALITY.BestQuality ? 8 : 13);
                        }
                        if (quality == BEAUTIFY_QUALITY.BestQuality && _bloomCustomize)
                        {
                            bMat.SetTexture("_BloomTex4", rt[4]);
                            bMat.SetTexture("_BloomTex3", rt[3]);
                            bMat.SetTexture("_BloomTex2", rt[2]);
                            bMat.SetTexture("_BloomTex1", rt[1]);
                            bMat.SetTexture("_BloomTex", rt[0]);
                            RenderTextureDescriptor rtCustomBloomDescriptor = rt[0].descriptor;
                            rtCustomBloom = RenderTexture.GetTemporary(rtCustomBloomDescriptor); // rt[0].width, rt[0].height, 0, rtFormat);
                            rtBloom = rtCustomBloom;
                            Graphics.Blit(rt[PYRAMID_COUNT], rtBloom, bMat, 6);
                        }
                    }

                }

                // anamorphic flares
                if (_anamorphicFlares)
                {
                    UpdateMaterialAnamorphicIntensityAndThreshold();

                    int sizeAF;
                    if (_quality == BEAUTIFY_QUALITY.BestPerformance)
                    {
                        sizeAF = 256;
                    }
                    else
                    {
                        sizeAF = _anamorphicFlaresUltra ? (int)(source.height / 4) * 4 : 512;
                    }

                    RenderTextureDescriptor rtAFDescriptor = rtDescBase;
                    for (int origSize = sizeAF, k = 0; k <= PYRAMID_COUNT; k++)
                    {
                        if (_anamorphicFlaresVertical)
                        {
                            rtAFDescriptor.width = origSize;
                            rtAFDescriptor.height = Mathf.Max(1, (int)(sizeAF * aspectRatio / _anamorphicFlaresSpread));
                            rtAF[k] = RenderTexture.GetTemporary(rtAFDescriptor);
                        }
                        else
                        {
                            rtAFDescriptor.width = Mathf.Max(1, (int)(sizeAF * aspectRatio / _anamorphicFlaresSpread));
                            rtAFDescriptor.height = origSize;
                            rtAF[k] = RenderTexture.GetTemporary(rtAFDescriptor);
                        }
                        sizeAF /= 2;
                    }

                    if (_anamorphicFlaresAntiflicker && _quality == BEAUTIFY_QUALITY.BestQuality)
                    {
                        //Graphics.Blit(useBloomSourceTexture ? bloomSourceTexture : source, rtAF[0], bMat, 9);
                        Graphics.Blit(source, rtAF[0], bMat, 9);
                    }
                    else
                    {
                        //Graphics.Blit (useBloomSourceTexture ? bloomSourceTexture : source, rtAF [0], bMat, 2);
                        Graphics.Blit(source, rtAF[0], bMat, 2);
                    }

                    rtAF[0] = BlurThisOneDirection(rtAF[0], _anamorphicFlaresVertical);

                    for (int k = 0; k < PYRAMID_COUNT; k++)
                    {
                        if (_quality == BEAUTIFY_QUALITY.BestPerformance)
                        {
                            Graphics.Blit(rtAF[k], rtAF[k + 1], bMat, 18);
                            if (_anamorphicFlaresBlur)
                            {
                                rtAF[k + 1] = BlurThisOneDirection(rtAF[k + 1], _anamorphicFlaresVertical);
                            }
                        }
                        else
                        {
                            Graphics.Blit(rtAF[k], rtAF[k + 1], bMat, 7);
                            rtAF[k + 1] = BlurThisOneDirection(rtAF[k + 1], _anamorphicFlaresVertical);
                        }
                    }

                    for (int k = PYRAMID_COUNT; k > 0; k--)
                    {
                        rtAF[k - 1].MarkRestoreExpected();
                        if (k == 1)
                        {
                            Graphics.Blit(rtAF[k], rtAF[k - 1], bMat, _quality == BEAUTIFY_QUALITY.BestQuality ? 10 : 14); // applies intensity in last stage
                        }
                        else
                        {
                            Graphics.Blit(rtAF[k], rtAF[k - 1], bMat, _quality == BEAUTIFY_QUALITY.BestQuality ? 8 : 13);
                        }
                    }
                    if (_bloom)
                    {
                        if (_lensDirt)
                        {
                            rt[3].MarkRestoreExpected();
                            Graphics.Blit(rtAF[3], rt[3], bMat, _quality == BEAUTIFY_QUALITY.BestQuality ? 11 : 13);
                        }
                        rtBloom.MarkRestoreExpected();
                        Graphics.Blit(rtAF[0], rtBloom, bMat, _quality == BEAUTIFY_QUALITY.BestQuality ? 11 : 13);
                    }
                    else
                    {
                        rtBloom = rtAF[0];
                    }
                    UpdateMaterialBloomIntensityAndThreshold();
                }

                if (sunFlareEnabled)
                {
                    // check if Sun is visible
                    Vector3 sunWorldPosition = currentCamera.transform.position - _sun.transform.forward * 1000f;
                    Vector3 sunScrPos = currentCamera.WorldToViewportPoint(sunWorldPosition);
                    float flareIntensity = 0;
                    if (sunScrPos.z > 0 && sunScrPos.x >= -0.1f && sunScrPos.x < 1.1f && sunScrPos.y >= -0.1f && sunScrPos.y < 1.1f)
                    {
                        Ray ray = new Ray(currentCamera.transform.position, (sunWorldPosition - currentCamera.transform.position).normalized);
                        if (!Physics.Raycast(ray, currentCamera.farClipPlane, _sunFlaresLayerMask))
                        {
                            Vector2 dd = sunScrPos - Vector3.one * 0.5f;
                            flareIntensity = _sunFlaresIntensity * Mathf.Clamp01((0.6f - Mathf.Max(Mathf.Abs(dd.x), Mathf.Abs(dd.y))) / 0.6f);
                        }
                    }
                    sunFlareCurrentIntensity = Mathf.Lerp(sunFlareCurrentIntensity, flareIntensity, Application.isPlaying ? 0.5f : 1f);
                    if (sunFlareCurrentIntensity > 0)
                    {
                        if (flareIntensity > 0)
                        {
                            sunLastScrPos = sunScrPos;
                        }
                        bMat.SetColor("_SunTint", _sunFlaresTint * sunFlareCurrentIntensity);
                        sunLastScrPos.z = 0.5f + sunFlareTime * _sunFlaresSolarWindSpeed;
                        Vector2 sfDist = new Vector2(0.5f - sunLastScrPos.y, sunLastScrPos.x - 0.5f);
                        if (!_sunFlaresRotationDeadZone || sfDist.sqrMagnitude > 0.00025f)
                        {
                            sunLastRot = Mathf.Atan2(sfDist.x, sfDist.y);
                        }
                        sunLastScrPos.w = sunLastRot;
                        sunFlareTime += Time.deltaTime;
                        bMat.SetVector("_SunPos", sunLastScrPos);
                        RenderTextureDescriptor rtSFDescriptor = rtDescBase;
                        rtSFDescriptor.width = currentCamera.pixelWidth / _sunFlaresDownsampling;
                        rtSFDescriptor.height = currentCamera.pixelHeight / _sunFlaresDownsampling;
                        RenderTexture rtSF = RenderTexture.GetTemporary(rtSFDescriptor);
                        int sfRenderPass;
                        if (_quality == BEAUTIFY_QUALITY.BestQuality)
                        {
                            sfRenderPass = rtBloom != null ? 21 : 20;
                        }
                        else
                        {
                            sfRenderPass = rtBloom != null ? 17 : 16;
                        }
                        Graphics.Blit(rtBloom, rtSF, bMat, sfRenderPass);
                        if (_lensDirt)
                        {
                            if (_bloom)
                            {
                                rt[3].MarkRestoreExpected();
                                Graphics.Blit(rtSF, rt[3], bMat, _quality == BEAUTIFY_QUALITY.BestQuality ? 11 : 13);
                            }
                        }
                        rtBloom = rtSF;
                        RenderTexture.ReleaseTemporary(rtSF);
                        if (!_bloom && !_anamorphicFlares)
                        { // ensure _Bloom.x is 1 into the shader for sun flares to be visible if no bloom nor anamorphic flares are enabled
                            bMat.SetVector("_Bloom", Vector4.one);
                            if (!bMat.IsKeywordEnabled(SKW_BLOOM))
                            {
                                bMat.EnableKeyword(SKW_BLOOM);
                            }
                        }
                    }
                }
                if (rtBloom != null)
                {
                    bMat.SetTexture("_BloomTex", rtBloom);
                }
                else
                {
					if (bMat.IsKeywordEnabled(SKW_BLOOM)) {
						bMat.DisableKeyword(SKW_BLOOM); // required to avoid Metal issue
					}
                    bMat.SetVector("_Bloom", Vector4.zero);
                }

                if (_lensDirt)
                {
                    bMat.SetTexture("_ScreenLum", (_anamorphicFlares && !_bloom) ? rtAF[3] : rt[3]);
                }

            }

            if (_lensDirt)
            {
                Vector4 dirtData = new Vector4(1.0f, 1.0f / (1.01f - _lensDirtIntensity), _lensDirtThreshold, Mathf.Max(_bloomIntensity, 1f));
                bMat.SetVector("_Dirt", dirtData);
            }

            // tonemap + eye adaptation + purkinje
            bool requiresLuminanceComputation = Application.isPlaying && allowExtraEffects && (_eyeAdaptation || _purkinje);
            if (requiresLuminanceComputation)
            {
                int rtEACount = _quality == BEAUTIFY_QUALITY.BestQuality ? 9 : 8;
                int sizeEA = (int)Mathf.Pow(2, rtEACount);
                if (rtEA == null || rtEA.Length < rtEACount)
                    rtEA = new RenderTexture[rtEACount];
                RenderTextureDescriptor rtLumDescriptor = rtDescBase;
                for (int k = 0; k < rtEACount; k++)
                {
                    rtLumDescriptor.width = sizeEA;
                    rtLumDescriptor.height = sizeEA;
                    rtEA[k] = RenderTexture.GetTemporary(rtLumDescriptor);
                    sizeEA /= 2;
                }
                Graphics.Blit(source, rtEA[0], bMat, _quality == BEAUTIFY_QUALITY.BestQuality ? 22 : 18);
                int lumRT = rtEACount - 1;
                int basePass = _quality == BEAUTIFY_QUALITY.BestQuality ? 15 : 9;
                for (int k = 0; k < lumRT; k++)
                {
                    Graphics.Blit(rtEA[k], rtEA[k + 1], bMat, k == 0 ? basePass : basePass + 1);
                }
                bMat.SetTexture("_EALumSrc", rtEA[lumRT]);
                if (rtEAacum == null)
                {
                    int rawCopyPass = _quality == BEAUTIFY_QUALITY.BestQuality ? 22 : 18;
                    RenderTextureDescriptor rtEASmallDesc = rtDescBase;
                    rtEASmallDesc.width = 2;
                    rtEASmallDesc.height = 2;
                    rtEAacum = new RenderTexture(rtEASmallDesc);
                    Graphics.Blit(rtEA[lumRT], rtEAacum, bMat, rawCopyPass);
                    rtEAHist = new RenderTexture(rtEASmallDesc);
                    Graphics.Blit(rtEAacum, rtEAHist, bMat, rawCopyPass);
                }
                else
                {
                    rtEAacum.MarkRestoreExpected();
                    Graphics.Blit(rtEA[lumRT], rtEAacum, bMat, basePass + 2);
                    Graphics.Blit(rtEAacum, rtEAHist, bMat, basePass + 3);
                }
                bMat.SetTexture("_EAHist", rtEAHist);
                bMat.SetTexture("_EALum", rtEAacum);
            }

            // Final Pass
            if (rtBeauty != null)
            {
                Graphics.Blit(source, rtBeauty, bMat, 1);
                bMat.SetTexture("_CompareTex", rtBeauty);
            }
            if (rtBlurTex != null)
            {
                float blurScale = _blurIntensity > 1f ? 1f : _blurIntensity;
                if (rtBeauty != null)
                {
                    Graphics.Blit(rtBeauty, rtBlurTex, bMat, renderPass);
                    BlurThis(rtBlurTex, blurScale);
                }
                else
                {
                    BlurThisDownscaling(source, rtBlurTex, blurScale);
                }
                BlurThis(rtBlurTex, blurScale);
                if (_quality == BEAUTIFY_QUALITY.BestQuality)
                {
                    BlurThis(rtBlurTex, blurScale);
                }
                if (rtBeauty != null)
                {
                    bMat.SetTexture("_CompareTex", rtBlurTex);
                    Graphics.Blit(source, destination, bMat, renderPass);
                }
                else
                {
                    Graphics.Blit(rtBlurTex, destination, bMat, renderPass);
                }
                RenderTexture.ReleaseTemporary(rtBlurTex);
            }
            else
            {
                Graphics.Blit(source, destination, bMat, renderPass);
            }

            // Release RTs used in final pass
            if (rtEA != null)
            {
                for (int k = 0; k < rtEA.Length; k++)
                {
                    if (rtEA[k] != null)
                    {
                        RenderTexture.ReleaseTemporary(rtEA[k]);
                        rtEA[k] = null;
                    }
                }
            }
            if (rt != null)
            {
                for (int k = 0; k < rt.Length; k++)
                {
                    if (rt[k] != null)
                    {
                        RenderTexture.ReleaseTemporary(rt[k]);
                        rt[k] = null;
                    }
                }
            }
            if (rtAF != null)
            {
                for (int k = 0; k < rtAF.Length; k++)
                {
                    if (rtAF[k] != null)
                    {
                        RenderTexture.ReleaseTemporary(rtAF[k]);
                        rtAF[k] = null;
                    }
                }
            }
            if (rtCustomBloom != null)
            {
                RenderTexture.ReleaseTemporary(rtCustomBloom);
            }
            if (rtDoF != null)
            {
                RenderTexture.ReleaseTemporary(rtDoF);
            }
            if (rtBeauty != null)
            {
                RenderTexture.ReleaseTemporary(rtBeauty);
            }
            if (rtPixelated != null)
            {
                RenderTexture.ReleaseTemporary(rtPixelated);
            }

        }

        void OnPostRender()
        {
            if (_pixelateDownscale && _pixelateAmount > 1 && pixelateTexture != null)
            {
                RenderTexture.active = null;
                currentCamera.targetTexture = null;
                pixelateTexture.filterMode = FilterMode.Point;
                Graphics.Blit(pixelateTexture, (RenderTexture)null);
            }
        }


        void BlurThis(RenderTexture rt, float blurScale = 1f)
        {
            RenderTextureDescriptor desc = rt.descriptor;
            RenderTexture rt2 = RenderTexture.GetTemporary(desc);
            rt2.filterMode = FilterMode.Bilinear;
            bMat.SetFloat("_BlurScale", blurScale);
            Graphics.Blit(rt, rt2, bMat, 4);
			#if !UNITY_STANDALONE && (!UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_WII || UNITY_PS4 || UNITY_XBOXONE || UNITY_TIZEN || UNITY_TVOS || UNITY_WSA || UNITY_WSA_10_0 || UNITY_WEBGL)
            rt.DiscardContents();
            #endif
            Graphics.Blit(rt2, rt, bMat, 5);
            RenderTexture.ReleaseTemporary(rt2);
        }

        void BlurThisDownscaling(RenderTexture rt, RenderTexture downscaled, float blurScale = 1f)
        {
            RenderTextureDescriptor desc = rt.descriptor;
            desc.width = downscaled.width;
            desc.height = downscaled.height;
            RenderTexture rt2 = RenderTexture.GetTemporary(desc);
            rt2.filterMode = FilterMode.Bilinear;
            float ratio = rt.width / desc.width;
            bMat.SetFloat("_BlurScale", blurScale * ratio);
            Graphics.Blit(rt, rt2, bMat, 4);
            bMat.SetFloat("_BlurScale", blurScale);
			#if !UNITY_STANDALONE && (!UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_WII || UNITY_PS4 || UNITY_XBOXONE || UNITY_TIZEN || UNITY_TVOS || UNITY_WSA || UNITY_WSA_10_0 || UNITY_WEBGL)
                 downscaled.DiscardContents();
            #endif
            Graphics.Blit(rt2, downscaled, bMat, 5);
            RenderTexture.ReleaseTemporary(rt2);
        }

        RenderTexture BlurThisOneDirection(RenderTexture rt, bool vertical, float blurScale = 1f)
        {
            RenderTextureDescriptor desc = rt.descriptor;
            RenderTexture rt2 = RenderTexture.GetTemporary(desc);
            rt2.filterMode = FilterMode.Bilinear;
            bMat.SetFloat("_BlurScale", blurScale);
            Graphics.Blit(rt, rt2, bMat, vertical ? 5 : 4);
            RenderTexture.ReleaseTemporary(rt);
            return rt2;
        }

        void BlurThisDoF(RenderTexture rt, int renderPass)
        {
            RenderTextureDescriptor desc = rt.descriptor;
            RenderTexture rt2 = RenderTexture.GetTemporary(desc);
            RenderTexture rt3 = RenderTexture.GetTemporary(desc);
            rt2.filterMode = _depthOfFieldFilterMode;
            rt3.filterMode = _depthOfFieldFilterMode;
            UpdateDepthOfFieldBlurData(new Vector2(0.44721f, -0.89443f));
            Graphics.Blit(rt, rt2, bMat, renderPass);
            UpdateDepthOfFieldBlurData(new Vector2(-1f, 0f));
            Graphics.Blit(rt2, rt3, bMat, renderPass);
            UpdateDepthOfFieldBlurData(new Vector2(0.44721f, 0.89443f));
			#if !UNITY_STANDALONE && (!UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_WII || UNITY_PS4 || UNITY_XBOXONE || UNITY_TIZEN || UNITY_TVOS || UNITY_WSA || UNITY_WSA_10_0 || UNITY_WEBGL)
            rt.DiscardContents();
            #endif
            Graphics.Blit(rt3, rt, bMat, renderPass);
            RenderTexture.ReleaseTemporary(rt3);
            RenderTexture.ReleaseTemporary(rt2);
        }


    void BlurThisAlpha(RenderTexture rt, float blurScale = 1f)
        {
            RenderTextureDescriptor desc = rt.descriptor;
            RenderTexture rt2 = RenderTexture.GetTemporary(desc);
            rt2.filterMode = FilterMode.Bilinear;
            bMat.SetFloat("_BlurScale", blurScale);
            Graphics.Blit(rt, rt2, bMat, 23);
			#if !UNITY_STANDALONE && (!UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_WII || UNITY_PS4 || UNITY_XBOXONE || UNITY_TIZEN || UNITY_TVOS || UNITY_WSA || UNITY_WSA_10_0 || UNITY_WEBGL)
            rt.DiscardContents();
			#endif
            Graphics.Blit(rt2, rt, bMat, 24);
            RenderTexture.ReleaseTemporary(rt2);
        }
		
        #endregion

        #region Settings stuff


        void OnDidApplyAnimationProperties()
        {   // support for animating property based fields
            shouldUpdateMaterialProperties = true;
        }

        public void UpdateQualitySettings()
        {
            switch (_quality)
            {
                case BEAUTIFY_QUALITY.BestPerformance:
                    _depthOfFieldDownsampling = 2;
                    _depthOfFieldMaxSamples = 4;
                    _sunFlaresDownsampling = 2;
                    break;
                case BEAUTIFY_QUALITY.BestQuality:
                    _depthOfFieldDownsampling = 1;
                    _depthOfFieldMaxSamples = 8;
                    _sunFlaresDownsampling = 1;
                    break;
                case BEAUTIFY_QUALITY.Basic:
                    //																_bloom = false;
                    //																_depthOfField = false;
                    //																_lensDirt = false;
                    //																_anamorphicFlares = false;
                    //																_purkinje = false;
                    //																_eyeAdaptation = false;
                    //																_outline = false;
                    //																_nightVision = false;
                    //																_thermalVision = false;
                    //																_lut = false;
                    //																_frame = false;
                    //																_tonemap = BEAUTIFY_TMO.Linear;
                    //																_vignetting = false;
                    //																_sunFlares = false;
                    break;
            }
            isDirty = true;
        }

        public void UpdateMaterialProperties()
        {
            if (Application.isPlaying)
            {
                shouldUpdateMaterialProperties = true;
            }
            else
            {
                UpdateMaterialPropertiesNow();
            }
        }

        public void UpdateMaterialPropertiesNow()
        {
            shouldUpdateMaterialProperties = false;

            // Checks camera depth texture mode
            if (currentCamera != null && currentCamera.depthTextureMode == DepthTextureMode.None && _quality != BEAUTIFY_QUALITY.Basic)
            {
                currentCamera.depthTextureMode = DepthTextureMode.Depth;
            }

            string gpu = SystemInfo.graphicsDeviceName;
            if (gpu != null && gpu.ToUpper().Contains("MALI-T720"))
            {
                rtFormat = RenderTextureFormat.Default;
                _bloomBlur = false; // avoid artifacting due to low precision textures
                _anamorphicFlaresBlur = false;
            }
            else
            {
                rtFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
            }

            switch (_quality)
            {
                case BEAUTIFY_QUALITY.BestQuality:
                    if (bMatDesktop == null)
                    {
                        bMatDesktop = new Material(Shader.Find("Beautify/Beautify"));
                        bMatDesktop.hideFlags = HideFlags.DontSave;
                    }
                    bMat = bMatDesktop;
                    break;
                case BEAUTIFY_QUALITY.BestPerformance:
                    if (bMatMobile == null)
                    {
                        bMatMobile = new Material(Shader.Find("Beautify/BeautifyMobile"));
                        bMatMobile.hideFlags = HideFlags.DontSave;
                    }
                    bMat = bMatMobile;
                    break;
                case BEAUTIFY_QUALITY.Basic:
                    if (bMatBasic == null)
                    {
                        bMatBasic = new Material(Shader.Find("Beautify/BeautifyBasic"));
                        bMatBasic.hideFlags = HideFlags.DontSave;
                    }
                    bMat = bMatBasic;
                    break;
            }

            bool linearColorSpace = (QualitySettings.activeColorSpace == ColorSpace.Linear);

            switch (_preset)
            {
                case BEAUTIFY_PRESET.Soft:
                    _sharpen = 2.0f;
                    if (linearColorSpace)
                        _sharpen *= 2f;
                    _sharpenDepthThreshold = 0.035f;
                    _sharpenRelaxation = 0.065f;
                    _sharpenClamp = 0.4f;
                    _saturate = 0.5f;
                    _contrast = 1.005f;
                    _brightness = 1.05f;
                    _dither = 0.02f;
                    _ditherDepth = 0;
                    _daltonize = 0;
                    break;
                case BEAUTIFY_PRESET.Medium:
                    _sharpen = 3f;
                    if (linearColorSpace)
                        _sharpen *= 2f;
                    _sharpenDepthThreshold = 0.035f;
                    _sharpenRelaxation = 0.07f;
                    _sharpenClamp = 0.45f;
                    _saturate = 1.0f;
                    _contrast = 1.02f;
                    _brightness = 1.05f;
                    _dither = 0.02f;
                    _ditherDepth = 0;
                    _daltonize = 0;
                    break;
                case BEAUTIFY_PRESET.Strong:
                    _sharpen = 4.75f;
                    if (linearColorSpace)
                        _sharpen *= 2f;
                    _sharpenDepthThreshold = 0.035f;
                    _sharpenRelaxation = 0.075f;
                    _sharpenClamp = 0.5f;
                    _saturate = 1.5f;
                    _contrast = 1.03f;
                    _brightness = 1.05f;
                    _dither = 0.022f;
                    _ditherDepth = 0;
                    _daltonize = 0;
                    break;
                case BEAUTIFY_PRESET.Exaggerated:
                    _sharpen = 6f;
                    if (linearColorSpace)
                        _sharpen *= 2f;
                    _sharpenDepthThreshold = 0.035f;
                    _sharpenRelaxation = 0.08f;
                    _sharpenClamp = 0.55f;
                    _saturate = 2.25f;
                    _contrast = 1.035f;
                    _brightness = 1.05f;
                    _dither = 0.025f;
                    _ditherDepth = 0;
                    _daltonize = 0;
                    break;
            }
            isDirty = true;

            if (bMat == null)
                return;
            renderPass = 1;
            if (_pixelateAmount > 1)
            {
                if (QualitySettings.antiAliasing > 1)
                {
                    QualitySettings.antiAliasing = 1;
                }
                if (_pixelateDownscale)
                {
                    _dither = 0;
                }
            }

            // sharpen settings
            UpdateSharpenParams(_sharpen);

            // dither settings
            bool isOrtho = (currentCamera != null && currentCamera.orthographic);
            bMat.SetVector("_Dither", new Vector4(_dither, isOrtho ? 0 : _ditherDepth, (_sharpenMaxDepth + _sharpenMinDepth) * 0.5f, Mathf.Abs(_sharpenMaxDepth - _sharpenMinDepth) * 0.5f + (isOrtho ? 1000.0f : 0f)));
            float cont = linearColorSpace ? 1.0f + (_contrast - 1.0f) / 2.2f : _contrast;

            // color grading settings
            bMat.SetVector("_ColorBoost", new Vector4(_brightness, cont, _saturate, _daltonize * 10f));

            // vignetting FX
            Color vignettingColorAdjusted = _vignettingColor;
            vignettingColorAdjusted.a *= _vignetting ? 32f : 0f;
            float vb = 1f - _vignettingBlink * 2f;
            if (vb < 0) vb = 0;
            vignettingColorAdjusted.r *= vb;
            vignettingColorAdjusted.g *= vb;
            vignettingColorAdjusted.b *= vb;
            bMat.SetColor("_Vignetting", vignettingColorAdjusted);
            if (currentCamera != null)
            {
                bMat.SetFloat("_VignettingAspectRatio", (_vignettingCircularShape && _vignettingBlink <= 0) ? 1.0f / currentCamera.aspect : _vignettingAspectRatio + 1.001f / (1.001f - _vignettingBlink) - 1f);
            }

            // frame FX
            if (_frame)
            {
                Vector4 frameColorAdjusted = new Vector4(_frameColor.r, _frameColor.g, _frameColor.b, (1.00001f - _frameColor.a) * 0.5f);
                bMat.SetVector("_Frame", frameColorAdjusted);
            }

            // outline FX
            bMat.SetColor("_Outline", _outlineColor);

            // bloom
            float bloomWeightsSum = 0.00001f + _bloomWeight0 + _bloomWeight1 + _bloomWeight2 + _bloomWeight3 + _bloomWeight4 + _bloomWeight5;
            bMat.SetVector("_BloomWeights", new Vector4(_bloomWeight0 / bloomWeightsSum + _bloomBoost0, _bloomWeight1 / bloomWeightsSum + _bloomBoost1, _bloomWeight2 / bloomWeightsSum + _bloomBoost2, _bloomWeight3 / bloomWeightsSum + _bloomBoost3));
            bMat.SetVector("_BloomWeights2", new Vector4(_bloomWeight4 / bloomWeightsSum + _bloomBoost4, _bloomWeight5 / bloomWeightsSum + _bloomBoost5, _bloomMaxBrightness, bloomWeightsSum));
            if (_bloomDebug && (_bloom || _anamorphicFlares || _sunFlares))
                renderPass = 3;

            // sun flares
            if (_sunFlares)
            {
                bMat.SetVector("_SunData", new Vector4(_sunFlaresSunIntensity, _sunFlaresSunDiskSize, _sunFlaresSunRayDiffractionIntensity, _sunFlaresSunRayDiffractionThreshold));
                bMat.SetVector("_SunCoronaRays1", new Vector4(_sunFlaresCoronaRays1Length, Mathf.Max(_sunFlaresCoronaRays1Streaks / 2f, 1), Mathf.Max(_sunFlaresCoronaRays1Spread, 0.0001f), _sunFlaresCoronaRays1AngleOffset));
                bMat.SetVector("_SunCoronaRays2", new Vector4(_sunFlaresCoronaRays2Length, Mathf.Max(_sunFlaresCoronaRays2Streaks / 2f, 1), Mathf.Max(_sunFlaresCoronaRays2Spread + 0.0001f), _sunFlaresCoronaRays2AngleOffset));
                bMat.SetVector("_SunGhosts1", new Vector4(0, _sunFlaresGhosts1Size, _sunFlaresGhosts1Offset, _sunFlaresGhosts1Brightness));
                bMat.SetVector("_SunGhosts2", new Vector4(0, _sunFlaresGhosts2Size, _sunFlaresGhosts2Offset, _sunFlaresGhosts2Brightness));
                bMat.SetVector("_SunGhosts3", new Vector4(0, _sunFlaresGhosts3Size, _sunFlaresGhosts3Offset, _sunFlaresGhosts3Brightness));
                bMat.SetVector("_SunGhosts4", new Vector4(0, _sunFlaresGhosts4Size, _sunFlaresGhosts4Offset, _sunFlaresGhosts4Brightness));
                bMat.SetVector("_SunHalo", new Vector3(_sunFlaresHaloOffset, _sunFlaresHaloAmplitude, _sunFlaresHaloIntensity * 100f));
            }

            // lens dirt
            if (_lensDirtTexture == null)
            {
                _lensDirtTexture = Resources.Load<Texture2D>("Textures/dirt2") as Texture2D;
            }
            bMat.SetTexture("_OverlayTex", _lensDirtTexture);

            // anamorphic flares
            bMat.SetColor("_AFTint", _anamorphicFlaresTint);

            // dof
            if (_depthOfField && _depthOfFieldAutofocusLayerMask != 0)
            {
                Shader.SetGlobalFloat("_BeautifyDepthBias", _depthOfFieldExclusionBias);
            }
            dofCurrentLayerMaskValue = _depthOfFieldExclusionLayerMask.value;

            // final config
            if (_compareMode)
            {
                renderPass = 0;
                bMat.SetVector("_CompareParams", new Vector4(Mathf.Cos(_compareLineAngle), Mathf.Sin(_compareLineAngle), -Mathf.Cos(_compareLineAngle), _compareLineWidth));
            }
            if (shaderKeywords == null)
                shaderKeywords = new List<string>();
            else
                shaderKeywords.Clear();

            if (_quality != BEAUTIFY_QUALITY.Basic)
            {
                if (_lut && _lutTexture != null)
                {
                    shaderKeywords.Add(SKW_LUT);
                    bMat.SetTexture("_LUTTex", _lutTexture);
                    bMat.SetColor("_FXColor", new Color(0, 0, 0, _lutIntensity));
                }
                else if (_nightVision)
                {
                    shaderKeywords.Add(SKW_NIGHT_VISION);
                    Color nightVisionAdjusted = _nightVisionColor;
                    if (linearColorSpace)
                    {
                        nightVisionAdjusted.a *= 5.0f * nightVisionAdjusted.a;
                    }
                    else
                    {
                        nightVisionAdjusted.a *= 3.0f * nightVisionAdjusted.a;
                    }
                    nightVisionAdjusted.r = nightVisionAdjusted.r * nightVisionAdjusted.a;
                    nightVisionAdjusted.g = nightVisionAdjusted.g * nightVisionAdjusted.a;
                    nightVisionAdjusted.b = nightVisionAdjusted.b * nightVisionAdjusted.a;
                    bMat.SetColor("_FXColor", nightVisionAdjusted);
                }
                else if (_thermalVision)
                {
                    shaderKeywords.Add(SKW_THERMAL_VISION);
                }
                else if (_daltonize > 0)
                {
                    shaderKeywords.Add(SKW_DALTONIZE);
                }
                else
                { // set _FXColor for procedural sepia
                    bMat.SetColor("_FXColor", new Color(0, 0, 0, _lutIntensity));
                }
                bMat.SetColor("_TintColor", _tintColor);
                if (_sunFlares)
                {
                    if (flareNoise == null)
                    {
                        flareNoise = Resources.Load<Texture2D>("Textures/flareNoise");
                    }
                    flareNoise.wrapMode = TextureWrapMode.Repeat;
                    bMat.SetTexture("_FlareTex", flareNoise);
                    if (_sun == null)
                    {
                        Light[] lights = GameObject.FindObjectsOfType<Light>();
                        for (int k = 0; k < lights.Length; k++)
                        {
                            Light light = lights[k];
                            if (light.type == LightType.Directional && light.enabled && light.gameObject.activeSelf)
                            {
                                _sun = light.transform;
                                break;
                            }
                        }
                    }
                }

                if (_vignetting)
                {
                    if (_vignettingMask != null)
                    {
                        bMat.SetTexture("_VignettingMask", _vignettingMask);
                        shaderKeywords.Add(SKW_VIGNETTING_MASK);
                    }
                    else
                    {
                        shaderKeywords.Add(SKW_VIGNETTING);
                    }
                }
                if (_frame)
                {
                    if (_frameMask != null)
                    {
                        bMat.SetTexture("_FrameMask", _frameMask);
                        shaderKeywords.Add(SKW_FRAME_MASK);
                    }
                    else
                    {
                        shaderKeywords.Add(SKW_FRAME);
                    }
                }
                if (_outline)
                    shaderKeywords.Add(SKW_OUTLINE);
                if (_lensDirt)
                    shaderKeywords.Add(SKW_DIRT);
                if (_bloom || _anamorphicFlares || _sunFlares)
                {
                    shaderKeywords.Add(SKW_BLOOM);
                    if (_bloomDepthAtten > 0)
                    {
                        bMat.SetFloat("_BloomDepthTreshold", _bloomDepthAtten);
                        shaderKeywords.Add(SKW_BLOOM_USE_DEPTH);
                    }
                    if ((_bloom || _anamorphicFlares) && _bloomCullingMask != 0)
                    {
                        bMat.SetFloat("_BloomLayerZBias", _bloomLayerZBias);
                        shaderKeywords.Add(SKW_BLOOM_USE_LAYER);
                    }
                }
                if (_depthOfField)
                {
                    if (_depthOfFieldTransparencySupport || _depthOfFieldExclusionLayerMask != 0)
                    {
                        shaderKeywords.Add(SKW_DEPTH_OF_FIELD_TRANSPARENT);
                    }
                    else
                    {
                        shaderKeywords.Add(SKW_DEPTH_OF_FIELD);
                    }
                }
                if (_eyeAdaptation)
                {
                    Vector4 eaData = new Vector4(_eyeAdaptationMinExposure, _eyeAdaptationMaxExposure, _eyeAdaptationSpeedToDark, _eyeAdaptationSpeedToLight);
                    bMat.SetVector("_EyeAdaptation", eaData);
                    shaderKeywords.Add(SKW_EYE_ADAPTATION);
                }
                if (_quality == BEAUTIFY_QUALITY.BestQuality)
                {
                    if (_tonemap == BEAUTIFY_TMO.ACES)
                        shaderKeywords.Add(SKW_TONEMAP_ACES);
                }
                if (_purkinje || _vignetting)
                {
                    float vd = _vignettingFade + _vignettingBlink * 0.5f;
                    if (_vignettingBlink > 0.99f) vd = 1f;
                    Vector3 purkinjeData = new Vector3(_purkinjeAmount, _purkinjeLuminanceThreshold, vd);
                    bMat.SetVector("_Purkinje", purkinjeData);
                    shaderKeywords.Add(SKW_PURKINJE);
                }
            }
            bMat.shaderKeywords = shaderKeywords.ToArray();

#if DEBUG_BEAUTIFY
												Debug.Log("*** DEBUG: Updating material properties...");
												Debug.Log("Linear color space: " + linearColorSpace.ToString());
												Debug.Log("Preset: " + _preset.ToString());
												Debug.Log("Sharpen: " + _sharpen.ToString());
												Debug.Log("Dither: " + _dither.ToString());
												Debug.Log("Contrast: " + cont.ToString());
												Debug.Log("Bloom: " + _bloom.ToString());
												Debug.Log("Bloom Intensity: " + _bloomIntensity.ToString());
												Debug.Log("Bloom Threshold: " + bloomThreshold.ToString());
												Debug.Log("Bloom Weight: " + bloomWeightsSum.ToString());
#endif
        }

        void UpdateMaterialBloomIntensityAndThreshold()
        {
            float threshold = _bloomThreshold;
            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
                threshold *= threshold;
            }
            bMat.SetVector("_Bloom", new Vector4(_bloomIntensity + (_anamorphicFlares ? 0.0001f : 0f), 0, 0, threshold));
        }

        void UpdateMaterialAnamorphicIntensityAndThreshold()
        {
            float threshold = _anamorphicFlaresThreshold;
            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
                threshold *= threshold;
            }
            float intensity = _anamorphicFlaresIntensity / (_bloomIntensity + 0.0001f);
            bMat.SetVector("_Bloom", new Vector4(intensity, 0, 0, threshold));
        }

        void UpdateSharpenParams(float sharpen)
        {
            bMat.SetVector("_Sharpen", new Vector4(sharpen, _sharpenDepthThreshold, _sharpenClamp, _sharpenRelaxation));
        }

        void UpdateDepthOfFieldData()
        {
            // TODO: get focal length from camera FOV: FOV = 2 arctan (x/2f) x = diagonal of film (0.024mm)
            float d;
            if (_depthOfFieldAutofocus)
            {
                UpdateDoFAutofocusDistance();
                d = dofLastAutofocusDistance > 0 ? dofLastAutofocusDistance : currentCamera.farClipPlane;
            }
            else if (_depthOfFieldTargetFocus != null)
            {
                Vector3 spos = currentCamera.WorldToScreenPoint(_depthOfFieldTargetFocus.position);
                if (spos.z < 0)
                {
                    d = currentCamera.farClipPlane;
                }
                else
                {
                    d = Vector3.Distance(currentCamera.transform.position, _depthOfFieldTargetFocus.position);
                }
            }
            else
            {
                d = _depthOfFieldDistance;
            }
            dofPrevDistance = Mathf.Lerp(dofPrevDistance, d, Application.isPlaying ? _depthOfFieldFocusSpeed * Time.deltaTime * 30f : 1f);
            float dofCoc = _depthOfFieldAperture * (_depthOfFieldFocalLength / Mathf.Max(dofPrevDistance - _depthOfFieldFocalLength, 0.001f)) * (1f / 0.024f);
            dofLastBokehData = new Vector4(dofPrevDistance, dofCoc, 0, 0);
            bMat.SetVector("_BokehData", dofLastBokehData);
            bMat.SetVector("_BokehData2", new Vector4(_depthOfFieldForegroundBlur ? _depthOfFieldForegroundDistance : currentCamera.farClipPlane, _depthOfFieldMaxSamples, _depthOfFieldBokehThreshold, _depthOfFieldBokehIntensity * _depthOfFieldBokehIntensity));
            bMat.SetFloat("_BokehData3", _depthOfFieldMaxBrightness);
        }

        void UpdateDepthOfFieldBlurData(Vector2 blurDir)
        {
            float downsamplingRatio = 1f / (float)_depthOfFieldDownsampling;
            blurDir *= downsamplingRatio;
            dofLastBokehData.z = blurDir.x;
            dofLastBokehData.w = blurDir.y;
            bMat.SetVector("_BokehData", dofLastBokehData);
        }

        void UpdateDoFAutofocusDistance()
        {
            Ray r = new Ray(currentCamera.transform.position, currentCamera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, currentCamera.farClipPlane, _depthOfFieldAutofocusLayerMask))
            {
                dofLastAutofocusDistance = Mathf.Clamp(hit.distance, _depthOfFieldAutofocusMinDistance, _depthOfFieldAutofocusMaxDistance);
            }
            else
            {
                dofLastAutofocusDistance = currentCamera.farClipPlane;
            }
        }

        public Texture2D GenerateSepiaLUT()
        {
            Texture2D tex = new Texture2D(1024, 32, TextureFormat.ARGB32, false, true);
            Color[] colors = new Color[1024 * 32];
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 1024; x++)
                {
                    Vector3 rgb;
                    rgb.z = ((x / 32) * 8) / 255f;
                    rgb.y = (y * 8) / 255f;
                    rgb.x = ((x % 32) * 8) / 255f;
                    float sepiaR = Vector3.Dot(rgb, new Vector3(0.393f, 0.769f, 0.189f));
                    float sepiaG = Vector3.Dot(rgb, new Vector3(0.349f, 0.686f, 0.168f));
                    float sepiaB = Vector3.Dot(rgb, new Vector3(0.272f, 0.534f, 0.131f));
                    colors[y * 1024 + x] = new Color(sepiaR, sepiaG, sepiaB);
                }
            }
            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }

        #endregion

        #region API

        /// <summary>
        /// Animates blink parameter
        /// </summary>
        /// <returns>The blink.</returns>
        /// <param name="duration">Duration.</param>
        public void Blink(float duration, float maxValue = 1) {
            if (duration <= 0) return;
            StartCoroutine(DoBlink(duration, maxValue));
        }

        IEnumerator DoBlink(float duration, float maxValue) {

            float start = Time.time;
            float t = 0;
            WaitForEndOfFrame w = new WaitForEndOfFrame();

            // Close
            do
            {
                t = (Time.time - start) / duration;
                if (t > 1f) t = 1f;
                float easeOut = t * (2f - t);
                vignettingBlink = easeOut * maxValue;
                yield return w;
            } while (t < 1f);

            // Open
            start = Time.time;
            do
            {
                t = (Time.time - start) / duration;
                if (t > 1f) t = 1f;
                float easeIn = t * t;
                vignettingBlink = (1f - easeIn) * maxValue;
                yield return w;
            } while (t < 1f);
        }

        #endregion



    }

}