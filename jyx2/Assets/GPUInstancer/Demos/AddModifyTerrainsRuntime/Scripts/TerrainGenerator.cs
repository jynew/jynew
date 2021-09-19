using UnityEngine;
using UnityEngine.UI;

namespace GPUInstancer
{
    public class TerrainGenerator : MonoBehaviour
    {
        public Texture2D groundTexture;
        public Texture2D detailTexture;

        public GameObject FpsController;
        public GameObject FixedCamera;

        private int terrainSize = 32;
        private int terrainCounter;
        private Vector3 terrainShiftX;
        private Vector3 terrainShiftZ;
        private Terrain[] terrainArray;
        private bool isCurrentCameraFixed = true;
        private float[,,] alphaMap;

        #region Prototype settings

        // color settings
        private Color _healthyColor = new Color(0.263f, 0.976f, 0.165f, 1f); // default unity terrain prototype healthy color
        private Color _dryColor = new Color(0.804f, 0.737f, 0.102f, 1f); // default unity terrain prototype dry color
        private float _noiseSpread = 0.2f;
        private float _ambientOcclusion = 0.5f;
        private float _gradientPower = 0.5f;

        // wind settings
        private float _windIdleSway = 0.6f;
        private bool _windWavesOn = false;
        private float _windWaveTint = 0.5f;
        private float _windWaveSize = 0.5f;
        private float _windWaveSway = 0.5f;
        private Color _windWaveTintColor = new Color(160f / 255f, 82f / 255f, 45f / 255f, 1f); // sienna
        
        // mesh settings
        private bool _isBillboard = true;
        private bool _useCrossQuads = false;
        private int _crossQuadCount = 2;
        private float _crossQuadBillboardDistance = 50f;
        private Vector4 _scale = new Vector4(0.5f, 3.0f, 0.5f, 3.0f);
        
        // GPU Instancer settings
        private bool _isShadowCasting = false;
        private bool _isFrustumCulling = true;
        private float _frustumOffset = 0.2f;
        private float _maxDistance = 250f;

        // GPU Instancer terrain Settings
        private Vector2 _windVector = new Vector2(0.4f, 0.8f);

        #endregion Prototype settings

        #region UI properties

        private Image _healthyColorImage;
        private Image _dryColorImage;
        private Slider _noiseSpreadSlider;
        private Slider _ambientOcclusionSlider;
        private Slider _gradientPowerSlider;

        private Slider _windIdleSwaySlider;
        private Toggle _windWavesOnToggle;
        private Slider _windWavesTintSlider;
        private Slider _windWavesSizeSlider;
        private Slider _windWavesSwaySlider;
        private Image _windWavesTintColorImage;

        private Toggle _billboardToggle;
        private Toggle _crossQuadsToggle;
        private Slider _crossQuadsCountSlider;
        private Slider _crossQuadsBillboardDistanceSlider;
        private InputField _scaleMinWidthInput;
        private InputField _scaleMaxWidthInput;
        private InputField _scaleMinHeightInput;
        private InputField _scaleMaxHeightInput;

        private Toggle _isShadowCastingToggle;
        private Toggle _isFrustumCullingToggle;
        private Slider _frustumOffsetSlider;
        private Slider _maxDistanceSlider;

        private InputField _windVectorXInput;
        private InputField _windVectorZInput;

        private Text _helpDescriptionText;
        private Text _helpDescriptionTitleText;
        
        private Selectable _addTerrainButton;
        private Selectable _removeTerrainButton;

        private Canvas _uiCanvas;

        #endregion UI properties

        #region Help Description Constants

        public static readonly string HELPTEXT_detailHealthyColor = "\"Healthy\" color of the the Healthy / Dry noise variation for the prototypes that use the \"GPUInstancer\\Foliage\" shader. This corresponds to the \"Healhy Color\" property of the detail prototypes in the Unity terrain. The Healthy / Dry noise texture can be changed globally (terrain specific) for all detail prototypes that use the \"GPUInstancer\\Foliage\" shader from the \"Healthy/Dry Noise Texture\" property of the detail manager.";
        public static readonly string HELPTEXT_detailDryColor = "\"Dry\" color of the the Healthy / Dry noise variation for the prototypes that use the \"GPUInstancer\\Foliage\" shader. This corresponds to the \"Dry Color\" property of the detail prototypes in the Unity terrain. The Healthy / Dry noise texture can be changed globally (terrain specific) for all detail prototypes that use the \"GPUInstancer\\Foliage\" shader from the \"Healthy/Dry Noise Texture\" property of the detail manager.";
        public static readonly string HELPTEXT_noiseSpread = "The \"Noise Spread\" property specifies the size of the \"Healthy\" and \"Dry\" patches for the Detail Prototypes that use the \"GPUInstancer\\Foliage\" shader. This corresponds to the \"Noise Spread\" propety of the detail prototypes in the Unity terrain. A higher number results in smaller patches. The Healthy / Dry noise texture can be changed globally (terrain specific) for all detail prototypes that use the \"GPUInstancer\\Foliage\" shader from the \"Healthy/Dry Noise Texture\" property of the detail manager.";
        public static readonly string HELPTEXT_ambientOcclusion = "The amount of \"Ambient Occlusion\" to apply to the objects that use the \"GPUInstancer\\Foliage\" shader.";
        public static readonly string HELPTEXT_gradientPower = "\"GPUInstancer\\Foliage\" shader provides an option to darken the lower regions of the mesh that uses it. This results in a more realistic look for foliage. You can set the amount of this darkening effect with the \"Gradient Power\" property, or turn it off completly by setting the slider to zero.";
        
        public static readonly string HELPTEXT_windIdleSway = "\"Wind Idle Sway\" specifies the amount of idle wind animation for the detail prototypes that use the \"GPUInstancer\\Foliage\" shader. This is the wind animation that occurs where wind waves are not present. Turning the slider down to zero disables this idle wind animation.";
        public static readonly string HELPTEXT_windWavesOn = "The \"Wind Waves\" toggle specifies whether there will be wind waves for the detail prototypes that use the \"GPUInstancer\\Foliage\" shader. The normals texture that is used to calculate winds can be changed globally (terrain specific) for all detail prototypes that use the \"GPUInstancer\\Foliage\" shader from the \"Wind Wave Normal Texture\" property of the detail manager. ";
        public static readonly string HELPTEXT_windWaveTintColor = "\"Wind Wave Tint Color\" is a shader property that acts similar to the \"Grass Tint\" property of the Unity terrain, except it applies on a per-prototype basis. This color applies to the \"Wind Wave Tint\" properties of all the objects that use the \"GPUInstancer\\Foliage\" shader.";
        public static readonly string HELPTEXT_windWaveSize = "\"Wind Wave Size\" specifies the size of the wind waves for the detail prototypes that use the \"GPUInstancer\\Foliage\" shader.";
        public static readonly string HELPTEXT_windWaveSway = "\"Wind Wave Sway\" specifies the amount of wind animation that is applied by the wind waves for the detail prototypes that use the \"GPUInstancer\\Foliage\" shader. This is the wind animation that occurs in addition to the idle wind animation. Turning the slider down to zero disables this extra wave animation.";
        public static readonly string HELPTEXT_windWaveTint = "\"Wind Wave Tint\" specifies how much the \"Wind Wave Tint\" color applies to the wind wave effect for the detail prototypes that use the \"GPUInstancer\\Foliage\" shader. Turning the slider down to zero disables wind wave coloring.";

        public static readonly string HELPTEXT_isBillboard = "If \"Billboard\" is enabled, the generated mesh for this prototype will be billboarded. Note that billboarding will turn off automatically if cross quads are enabled.";
        public static readonly string HELPTEXT_crossQuads = "If \"Cross Quads\" is enabled, a mesh with multiple quads will be generated for this detail texture (instead of a single quad or billboard). The generated quads will be equiangular to each other. Cross quadding means more polygons for a given prototype, so turning this off will increase performance if there will be a huge number of instances of this detail prototype.";
        public static readonly string HELPTEXT_quadCount = "\"Cross Quad Count\" defines the number of generated equiangular quads for this detail texture. Using less quads will increase performance (especially where there will be a huge number of instances of this prototype).";
        public static readonly string HELPTEXT_billboardDistance = "When Cross Quads is enabled, \"Cross Quad Billboard Distance\" specifies the distance from the selected camera where the objects will be drawn as billboards to increase performance further. This is useful beacause at a certain distance, the difference between a multiple quad mesh and a billboard is barely visible. The value used here is similar to the screenRelativeTransitionHeight property of Unity LOD groups.";
        public static readonly string HELPTEXT_detailScale = "\"Detail Scale\" can be used to set a range for the instance sizes for detail prototypes. For the texture type detail prototypes, this range applies to the \"Healthy / Dry Noise Texture\" property of the detail manager. The values here correspond to the width and height values for the detail prototypes in the Unity terrain. \nX: Minimum Width - Y: Maximum Width\nZ: Minimum Height - W: Maximum Height";

        public static readonly string HELPTEXT_isShadowCasting = "The \"Shadow Casting\" toggle specifies whether the object will cast shadows or not. Shadow casting requires extra shadow passes in the shader resulting in additional rendering operations. GPU Instancer uses various techniques that boost the performance of these operations, but turning shadow casting off completely will increase performance.";
        public static readonly string HELPTEXT_isFrustumCulling = "The \"Frustum Culling\" toggle specifies whether the objects that are not in the selected camera's view frustum will be rendered or not. If enabled, GPU Instancer will not render the objects that are outside the selected camera's view frustum. This will increase performance. It is recommended to turn frustum culling on unless there are multiple cameras rendering the scene at the same time.";
        public static readonly string HELPTEXT_frustumOffset = "\"Frustum Offset\" defines the size of the area around the camera frustum planes within which objects will be rendered while frustum culling is enabled. GPU Instancer does frustum culling on the GPU which provides a performance boost. However, if there is a performance hit (usually while rendering an extreme amount of objects), and if the camera is moving very fast, rendering can lag behind camera movement. This could result in some objects not being rendered around the frustum edges. This offset expands the calculated frustum area so that the renderer can keep up.";
        public static readonly string HELPTEXT_maxDetailDistance = "\"Max Detail Distance\" defines the maximum distance from the camera within which the terrain details will be rendered. Details that are farther than the specified distance will not be visible. Note that this setting can also be limited globally (terrain specific) by the \"Max distance\" property of the detail manager itself.";

        public static readonly string HELPTEXT_windVector = "The \"Wind Vector\" specifies the [X, Z] vector (world axis) of the wind for all the prototypes (terrain specific) that use the \"GPUInstancer\\Foliage\" shader. This vector supplies both direction and magnitude information for wind.";

        #endregion Help Description Constants

        #region MonoBehaviour Methods

        private void Start()
        {
            SetupUI();

            if (FixedCamera)
                FixedCamera.SetActive(true);
            if (FpsController)
                FpsController.SetActive(false);

            terrainCounter = 0;
            terrainShiftX = new Vector3(terrainSize, 0, 0);
            terrainShiftZ = new Vector3(0, 0, -terrainSize);
            terrainArray = new Terrain[9];
            AddTerrain();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                AddTerrain();
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                RemoveTerrain();
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                SwitchCameras();
                isCurrentCameraFixed = !isCurrentCameraFixed;
            }

            if (Input.GetKeyUp(KeyCode.U))
            {
                _uiCanvas.gameObject.SetActive(!_uiCanvas.gameObject.activeSelf);
            }
        }

        #endregion MonoBehaviour Methods

        #region Main

        public void AddTerrain()
        {
            if (terrainCounter == 9)
                return;

            GenerateTerrain();
            AddInstancer(terrainArray[terrainCounter]);

            terrainCounter++;

            ManageButtons();
        }

        public void RemoveTerrain()
        {
            if (terrainCounter == 0)
                return;

            Destroy(terrainArray[terrainCounter - 1].gameObject);
            terrainCounter--;
            
            ManageButtons();
        }

        private void AddInstancer(Terrain terrain)
        {
            GPUInstancerDetailManager detailManager = terrain.gameObject.AddComponent<GPUInstancerDetailManager>();
            GPUInstancerAPI.SetupManagerWithTerrain(detailManager, terrain);

            detailManager.terrainSettings.windVector = _windVector;
            
            // Can change prototype properties here
            if(detailManager.prototypeList.Count > 0)
            {
                GPUInstancerDetailPrototype detailPrototype = (GPUInstancerDetailPrototype)detailManager.prototypeList[0];

                detailPrototype.detailHealthyColor = _healthyColor;
                detailPrototype.detailDryColor = _dryColor;
                detailPrototype.noiseSpread = _noiseSpread;
                detailPrototype.ambientOcclusion = _ambientOcclusion;
                detailPrototype.gradientPower = _gradientPower;

                detailPrototype.windIdleSway = _windIdleSway;
                detailPrototype.windWavesOn = _windWavesOn;
                detailPrototype.windWaveTint = _windWaveTint;
                detailPrototype.windWaveSize = _windWaveSize;
                detailPrototype.windWaveSway = _windWaveSway;
                detailPrototype.windWaveTintColor = _windWaveTintColor;
                
                detailPrototype.isBillboard = _isBillboard;
                detailPrototype.useCrossQuads = _useCrossQuads;
                detailPrototype.quadCount = _crossQuadCount;
                detailPrototype.billboardDistance = _crossQuadBillboardDistance;
                detailPrototype.detailScale = _scale;

                detailPrototype.isShadowCasting = _isShadowCasting;
                detailPrototype.isFrustumCulling = _isFrustumCulling;
                detailPrototype.frustumOffset = _frustumOffset;
                detailPrototype.maxDistance = _maxDistance;


            }

            GPUInstancerAPI.InitializeGPUInstancer(detailManager);
        }

        private void UpdateManagers()
        {
            for (int i = 0; i < terrainCounter; i++)
            {
                GPUInstancerDetailManager detailManager = terrainArray[i].GetComponent<GPUInstancerDetailManager>();

                detailManager.terrainSettings.windVector = _windVector;

                for (int j = 0; j < detailManager.prototypeList.Count; j++)
                {
                    GPUInstancerDetailPrototype detailPrototype = (GPUInstancerDetailPrototype) detailManager.prototypeList[j];

                    // noise settings:
                    detailPrototype.detailHealthyColor = _healthyColor;
                    detailPrototype.detailDryColor = _dryColor;
                    detailPrototype.noiseSpread = _noiseSpread;
                    detailPrototype.ambientOcclusion = _ambientOcclusion;
                    detailPrototype.gradientPower = _gradientPower;

                    // wind settings:
                    detailPrototype.windIdleSway = _windIdleSway;
                    detailPrototype.windWavesOn = _windWavesOn;
                    detailPrototype.windWaveTint = _windWaveTint;
                    detailPrototype.windWaveSize = _windWaveSize;
                    detailPrototype.windWaveSway = _windWaveSway;
                    detailPrototype.windWaveTintColor = _windWaveTintColor;

                    // mesh settings:
                    detailPrototype.isBillboard = _useCrossQuads ? false : _isBillboard;
                    detailPrototype.useCrossQuads = _useCrossQuads;
                    detailPrototype.quadCount = _crossQuadCount;
                    detailPrototype.billboardDistance = _crossQuadBillboardDistance;
                    detailPrototype.detailScale = _scale;

                    // GPU Instancer settings
                    detailPrototype.isShadowCasting = _isShadowCasting;
                    detailPrototype.isFrustumCulling = _isFrustumCulling;
                    detailPrototype.frustumOffset = _frustumOffset;
                    detailPrototype.maxDistance = _maxDistance;
                }

                GPUInstancerAPI.UpdateDetailInstances(detailManager, true);
            }
        }

        public void ReInitializeManagers()
        {
            for (int i = 0; i < terrainCounter; i++)
            {
                GPUInstancerDetailManager detailManager = terrainArray[i].GetComponent<GPUInstancerDetailManager>();
                GPUInstancerAPI.InitializeGPUInstancer(detailManager);
            }
        }

        #endregion Main

        #region Camera and GUI

        private void SetupUI()
        {
            _uiCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

            _addTerrainButton = GameObject.Find("AddTerrainButton").GetComponent<Selectable>();
            _removeTerrainButton = GameObject.Find("RemoveTerrainButton").GetComponent<Selectable>();

            _helpDescriptionText = GameObject.Find("HelpDescriptionText").GetComponent<Text>();
            _helpDescriptionTitleText = GameObject.Find("HelpDescriptionTitleText").GetComponent<Text>();

            _healthyColorImage = GameObject.Find("HealthyColorPicker").transform.GetChild(0).GetComponent<Image>();
            GameObject.Find("HealthyColorPicker").GetComponent<ColorPicker>().Color = _healthyColor;
            GameObject.Find("HealthyColorPicker").GetComponent<ColorPicker>().SetOnValueChangeCallback(UpdateDetailSettings);
            _dryColorImage = GameObject.Find("DryColorPicker").transform.GetChild(0).GetComponent<Image>();
            GameObject.Find("DryColorPicker").GetComponent<ColorPicker>().Color = _dryColor;
            GameObject.Find("DryColorPicker").GetComponent<ColorPicker>().SetOnValueChangeCallback(UpdateDetailSettings);
            _noiseSpreadSlider = GameObject.Find("NoiseSpreadSlider").GetComponent<Slider>();
            _ambientOcclusionSlider = GameObject.Find("AmbientOcclusionSlider").GetComponent<Slider>();
            _gradientPowerSlider = GameObject.Find("GradientPowerSlider").GetComponent<Slider>();

            _windIdleSwaySlider = GameObject.Find("WindIdleSwaySlider").GetComponent<Slider>();
            _windWavesOnToggle = GameObject.Find("WindWavesToggle").GetComponent<Toggle>();
            _windWavesTintSlider = GameObject.Find("WindWavesTintSlider").GetComponent<Slider>();
            _windWavesSizeSlider = GameObject.Find("WindWavesSizeSlider").GetComponent<Slider>();
            _windWavesSwaySlider = GameObject.Find("WindWavesSwaySlider").GetComponent<Slider>();
            _windWavesTintColorImage = GameObject.Find("WindWavesTintColorPicker").transform.GetChild(0).GetComponent<Image>();
            GameObject.Find("WindWavesTintColorPicker").GetComponent<ColorPicker>().Color = _windWaveTintColor;
            GameObject.Find("WindWavesTintColorPicker").GetComponent<ColorPicker>().SetOnValueChangeCallback(UpdateDetailSettings);

            _billboardToggle = GameObject.Find("BillboardToggle").GetComponent<Toggle>();
            _crossQuadsToggle = GameObject.Find("CrossQuadsToggle").GetComponent<Toggle>();
            _crossQuadsCountSlider = GameObject.Find("CrossQuadsCountSlider").GetComponent<Slider>();
            _crossQuadsBillboardDistanceSlider = GameObject.Find("CrossQuadsBillboardDistanceSlider").GetComponent<Slider>();
            _scaleMinWidthInput = GameObject.Find("ScaleMinimumWidthInput").GetComponent<InputField>();
            _scaleMaxWidthInput = GameObject.Find("ScaleMaximumWidthInput").GetComponent<InputField>();
            _scaleMinHeightInput = GameObject.Find("ScaleMinimumHeightInput").GetComponent<InputField>();
            _scaleMaxHeightInput = GameObject.Find("ScaleMaximumHeightInput").GetComponent<InputField>();

            _isShadowCastingToggle = GameObject.Find("ShadowCastingToggle").GetComponent<Toggle>();
            _isFrustumCullingToggle = GameObject.Find("FrustumCullingToggle").GetComponent<Toggle>();
            _frustumOffsetSlider = GameObject.Find("FrustumOffsetSlider").GetComponent<Slider>();
            _maxDistanceSlider = GameObject.Find("MaxDistanceSlider").GetComponent<Slider>();

            _windVectorXInput = GameObject.Find("WindVectorXInput").GetComponent<InputField>();
            _windVectorZInput = GameObject.Find("WindVectorZInput").GetComponent<InputField>();
        }

        public void UpdateDetailSettings()
        {
            _healthyColor = _healthyColorImage.color;
            _dryColor = _dryColorImage.color;
            _noiseSpread = _noiseSpreadSlider.value;
            _ambientOcclusion = _ambientOcclusionSlider.value;
            _gradientPower = _gradientPowerSlider.value;

            _windIdleSway = _windIdleSwaySlider.value;
            _windWavesOn = _windWavesOnToggle.isOn;
            _windWaveTint = _windWavesTintSlider.value;
            _windWaveSize = _windWavesSizeSlider.value;
            _windWaveSway = _windWavesSwaySlider.value;
            _windWaveTintColor = _windWavesTintColorImage.color;

            _isBillboard = _crossQuadsToggle.isOn ? false : _billboardToggle.isOn;
            _billboardToggle.isOn = _isBillboard;

            _useCrossQuads = _crossQuadsToggle.isOn;
            _crossQuadCount = (int)_crossQuadsCountSlider.value;
            _crossQuadBillboardDistance = _crossQuadsBillboardDistanceSlider.value;

            if (!float.TryParse(_scaleMinWidthInput.text, out _scale.x))
            {
                _scale.x = 1.0f;
                _scaleMinWidthInput.text = "1.0";
            }
            if (!float.TryParse(_scaleMaxWidthInput.text, out _scale.y))
            {
                _scale.y = 1.0f;
                _scaleMaxWidthInput.text = "1.0";
            }
            if (!float.TryParse(_scaleMinHeightInput.text, out _scale.z))
            {
                _scale.z = 1.0f;
                _scaleMinHeightInput.text = "1.0";
            }
            if (!float.TryParse(_scaleMaxHeightInput.text, out _scale.w))
            {
                _scale.w = 1.0f;
                _scaleMaxHeightInput.text = "1.0";
            }

            _isShadowCasting = _isShadowCastingToggle.isOn;
            _isFrustumCulling = _isFrustumCullingToggle.isOn;
            _frustumOffset = _frustumOffsetSlider.value;
            _maxDistance = _maxDistanceSlider.value;

            if (!float.TryParse(_windVectorXInput.text, out _windVector.x))
            {
                _windVector.x = 0.4f;
                _windVectorXInput.text = "0.4";
            }
            if (!float.TryParse(_windVectorZInput.text, out _windVector.y))
            {
                _windVector.y = 0.8f;
                _windVectorZInput.text = "0.8";
            }

            UpdateManagers();
        }

        public void ShowHelpDescription(Text itemTitle)
        {
            _helpDescriptionTitleText.text = itemTitle.text;

            switch (itemTitle.text)
            {
                case "Healthy Color":
                    _helpDescriptionText.text = HELPTEXT_detailHealthyColor;
                    break;
                case "Dry Color":
                    _helpDescriptionText.text = HELPTEXT_detailDryColor;
                    break;
                case "Noise Spread":
                    _helpDescriptionText.text = HELPTEXT_noiseSpread;
                    break;
                case "Ambient Occlusion":
                    _helpDescriptionText.text = HELPTEXT_ambientOcclusion;
                    break;
                case "Gradient Power":
                    _helpDescriptionText.text = HELPTEXT_gradientPower;
                    break;
                case "Wind Idle Sway":
                    _helpDescriptionText.text = HELPTEXT_windIdleSway;
                    break;
                case "Wind Waves":
                    _helpDescriptionText.text = HELPTEXT_windWavesOn;
                    break;
                case "Wind Waves Tint":
                    _helpDescriptionText.text = HELPTEXT_windWaveTint;
                    break;
                case "Wind Waves Size":
                    _helpDescriptionText.text = HELPTEXT_windWaveSize;
                    break;
                case "Wind Waves Sway":
                    _helpDescriptionText.text = HELPTEXT_windWaveSway;
                    break;
                case "Wind Waves Tint Color":
                    _helpDescriptionText.text = HELPTEXT_windWaveTintColor;
                    break;
                case "Billboard":
                    _helpDescriptionText.text = HELPTEXT_isBillboard;
                    break;
                case "Cross Quads":
                    _helpDescriptionText.text = HELPTEXT_crossQuads;
                    break;
                case "Cross Quad Count":
                    _helpDescriptionText.text = HELPTEXT_quadCount;
                    break;
                case "CQ Billboard Dist.":
                    _helpDescriptionTitleText.text = "Cross Quad Billboard Distance";
                    _helpDescriptionText.text = HELPTEXT_billboardDistance;
                    break;
                case "Scale":
                    _helpDescriptionText.text = HELPTEXT_detailScale;
                    break;
                case "Shadow Casting":
                    _helpDescriptionText.text = HELPTEXT_isShadowCasting;
                    break;
                case "(GPU) Furstum Culling":
                    _helpDescriptionText.text = HELPTEXT_isFrustumCulling;
                    break;
                case "Frustum Offset":
                    _helpDescriptionText.text = HELPTEXT_frustumOffset;
                    break;
                case "Max Render Distance":
                    _helpDescriptionText.text = HELPTEXT_maxDetailDistance;
                    break;
                case "Wind Vector":
                    _helpDescriptionText.text = HELPTEXT_windVector;
                    break;
            }
        }

        public void HideHelpDescription()
        {
            _helpDescriptionTitleText.text = "Description";
            _helpDescriptionText.text = "This scene is intended to act as a tutorial and to showcase how to use GPU Instancer's terrain detail system to add, remove and modify instanced detail prototypes at runtime.\n\nCheck the \"TerrainGenerator.cs\" in this scene for details.\n\nHover over a setting's title to see it's description here.";
        }

        private void ManageButtons()
        {
            _addTerrainButton.interactable = terrainCounter < 9;
            _removeTerrainButton.interactable = terrainCounter > 0;
        }

        private void SwitchCameras()
        {
            if (!FpsController || !FixedCamera)
                return;
            FpsController.SetActive(isCurrentCameraFixed);
            FixedCamera.SetActive(!isCurrentCameraFixed);

            var managers = FindObjectsOfType<GPUInstancerDetailManager>();
            foreach (var manager in managers)
            {
                GPUInstancerAPI.SetCamera(isCurrentCameraFixed
                    ? FpsController.GetComponentInChildren<Camera>()
                    : FixedCamera.GetComponentInChildren<Camera>());
            }

            if (!isCurrentCameraFixed)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        #endregion Camera and GUI

        #region Unity Terrain Generation

        private void GenerateTerrain()
        {
            SplatPrototype[] splatPrototypes = new SplatPrototype[1];
            splatPrototypes[0] = new SplatPrototype();
            splatPrototypes[0].texture = groundTexture;

            DetailPrototype[] detailPrototypes = new DetailPrototype[1];
            detailPrototypes[0] = new DetailPrototype();
            detailPrototypes[0].prototypeTexture = detailTexture;
            detailPrototypes[0].renderMode = DetailRenderMode.GrassBillboard;

            Vector3 terrainPosition = Vector3.zero + (terrainShiftX * (terrainCounter % 3)) + (terrainShiftZ * (Mathf.FloorToInt(terrainCounter / 3f)));

            terrainArray[terrainCounter] = InitializeTerrainObject(terrainPosition, terrainSize, terrainSize / 2f, 16, 16, splatPrototypes, detailPrototypes);

            terrainArray[terrainCounter].transform.SetParent(transform);

            SetDetailMap(terrainArray[terrainCounter]);
        }

        private Terrain InitializeTerrainObject(Vector3 position, int terrainSize, float terrainHeight, int baseTextureResolution = 16, int detailResolutionPerPatch = 16, SplatPrototype[] splatPrototypes = null, DetailPrototype[] detailPrototypes = null)
        {
            GameObject terrainGameObject = new GameObject("GenratedTerrain");
            terrainGameObject.transform.position = position;

            Terrain terrain = terrainGameObject.AddComponent<Terrain>();
            TerrainCollider terrainCollider = terrainGameObject.AddComponent<TerrainCollider>();

#if UNITY_2019_2_OR_NEWER || UNITY_2018_4
            if (GPUInstancerConstants.gpuiSettings.isURP)
                terrain.materialTemplate = new Material(Shader.Find("Universal Render Pipeline/Terrain/Lit"));
            else if (GPUInstancerConstants.gpuiSettings.isHDRP)
                terrain.materialTemplate = new Material(Shader.Find("HDRP/TerrainLit"));
            else
                terrain.materialTemplate = new Material(Shader.Find("Nature/Terrain/Standard"));
#endif
            TerrainData terrainData = CreateTerrainData(terrainSize, terrainHeight, baseTextureResolution, detailResolutionPerPatch, splatPrototypes, detailPrototypes);

            terrainCollider.terrainData = terrainData;
            terrain.terrainData = terrainData;

#if UNITY_2019_2_OR_NEWER || UNITY_2018_4
            if (alphaMap == null)
            {
                alphaMap = new float[terrainSize, terrainSize, 1];
                for (int i = 0; i < terrainSize; i++)
                {
                    for (int j = 0; j < terrainSize; j++)
                    {
                        alphaMap[i, j, 0] = 1;
                    }
                }
            }
            terrain.terrainData.SetAlphamaps(0, 0, alphaMap);
#endif

            return terrain;
        }

        private TerrainData CreateTerrainData(int terrainSize, float terrainHeight, int baseTextureResolution = 16, int detailResolutionPerPatch = 16, SplatPrototype[] splatPrototypes = null, DetailPrototype[] detailPrototypes = null)
        {
            TerrainData terrainData = new TerrainData();

            terrainData.heightmapResolution = terrainSize + 1;
            terrainData.baseMapResolution = baseTextureResolution; //16 is enough.
            terrainData.alphamapResolution = terrainSize;
            terrainData.SetDetailResolution(terrainSize, detailResolutionPerPatch);
#if UNITY_2018_3_OR_NEWER
            terrainData.terrainLayers = SplatPrototypesToTerrainLayers(splatPrototypes);
#else
            terrainData.splatPrototypes = splatPrototypes;
#endif
            terrainData.detailPrototypes = detailPrototypes;

            //terrain size must be set after setting terrain resolution.
            terrainData.size = new Vector3(terrainSize, terrainHeight, terrainSize);

            return terrainData;
        }

#if UNITY_2018_3_OR_NEWER
        private TerrainLayer[] SplatPrototypesToTerrainLayers(SplatPrototype[] splatPrototypes)
        {
            if (splatPrototypes == null)
                return null;
            TerrainLayer[] terrainLayers = new TerrainLayer[splatPrototypes.Length];
            for (int i = 0; i < splatPrototypes.Length; i++)
            {
                terrainLayers[i] = new TerrainLayer() { diffuseTexture = splatPrototypes[i].texture, normalMapTexture = splatPrototypes[i].normalMap };
            }
            return terrainLayers;
        }
#endif

        private void SetDetailMap(Terrain terrain)
        {
            int[,] detailMap = new int[terrain.terrainData.detailResolution, terrain.terrainData.detailResolution];
            for (int i = 0; i < terrain.terrainData.detailPrototypes.Length; i++)
            {
                for (int x = 0; x < terrain.terrainData.detailResolution; x++)
                {
                    for (int y = 0; y < terrain.terrainData.detailResolution; y++)
                    {
                        detailMap[x, y] = Random.Range(4, 8);
                    }
                }
                terrain.terrainData.SetDetailLayer(0, 0, i, detailMap);
            }
            terrain.detailObjectDistance = 250;
        }

        #endregion Unity Terrain Generation
    }
}
