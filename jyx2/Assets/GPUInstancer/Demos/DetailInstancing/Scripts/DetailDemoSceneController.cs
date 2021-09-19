using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPUInstancer
{
    public class DetailDemoSceneController : MonoBehaviour
    {
        public GameObject fpController;
        public GameObject spaceshipCamera;
        public GameObject grassMowerCamera;
        public GPUInstancerDetailManager detailManager;
        public bool persistRemoval;

        private GameObject _uiCanvas;
        private GameObject _spaceShipControlsText;
        private GameObject _grassMowerControlsText;
        private GameObject _loadingTerrainDetailsText;
        private Text _currentQualityModeText;

        private Transform _spaceShip;
        private Transform _grassMower;
        private GameObject _activeCameraGO;
        private enum CameraModes { FPMode, SpaceshipMode, MowerMode };
        private CameraModes _currentCameraMode;
        private ParticleSystem _spaceShipThrusterGlow;
        private enum QualityMode { Low, Mid, High }
        private QualityMode _currentQualityMode = QualityMode.High;

        private List<int[,]> detailMapData;

        private void Awake()
        {
            // Setup UI
            _uiCanvas = GameObject.Find("Canvas");
            _spaceShipControlsText = GameObject.Find("SpaceShipControlsPanel");
            _grassMowerControlsText = GameObject.Find("GrassMowerControlsPanel");
            _loadingTerrainDetailsText = GameObject.Find("LoadingInfoPanel");
            _currentQualityModeText = GameObject.Find("CurrentQualityModeInfoText").GetComponent<Text>();
            _currentQualityModeText.text = "Current Quality Mode: " + _currentQualityMode + " Quality";

            // Setup space ship
            _spaceShip = FindObjectOfType<SpaceshipController>().transform;
            _spaceShipThrusterGlow = _spaceShip.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();

            // Setup grass mower
            _grassMower = FindObjectOfType<GrassMowerController>().transform;

            // Setup camera
            SetCameraMode(CameraModes.FPMode);

            SetPrototypesByQuality(_currentQualityMode);

            // Setup loading bar
            GPUInstancerAPI.StartListeningGPUIEvent(GPUInstancerEventType.DetailInitializationFinished, DisableLoadingTerrainDetailsText);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.C))
            {
                SwitchCameraMode();
            }

            if (Input.GetKeyUp(KeyCode.U))
            {
                _uiCanvas.gameObject.SetActive(!_uiCanvas.gameObject.activeSelf);
            }

            if (Input.GetKeyUp(KeyCode.F1))
            {
                if (detailManager.gameObject.activeSelf && persistRemoval)
                    detailMapData = GPUInstancerAPI.GetDetailMapData(detailManager);
                detailManager.gameObject.SetActive(!detailManager.gameObject.activeSelf);
                if (detailManager.gameObject.activeSelf)
                {
                    if (persistRemoval && detailMapData != null)
                        GPUInstancerAPI.SetDetailMapData(detailManager, detailMapData);
                    _loadingTerrainDetailsText.SetActive(true);
                    GPUInstancerAPI.SetCamera(_activeCameraGO.GetComponentInChildren<Camera>());
                    GPUInstancerAPI.StartListeningGPUIEvent(GPUInstancerEventType.DetailInitializationFinished, DisableLoadingTerrainDetailsText);
                }
                SetQualityMode(_currentQualityMode);
            }

            if (Input.GetKeyUp(KeyCode.F2))
            {
                SetQualityMode(QualityMode.Low);
            }

            if (Input.GetKeyUp(KeyCode.F3))
            {
                SetQualityMode(QualityMode.Mid);
            }

            if (Input.GetKeyUp(KeyCode.F4))
            {
                SetQualityMode(QualityMode.High);
            }
        }

        private void SwitchCameraMode()
        {
            SetCameraMode((CameraModes)(((int)_currentCameraMode + 1) % 3));
        }

        private void SetCameraMode(CameraModes cameraMode)
        {
            if (!fpController || !spaceshipCamera || !grassMowerCamera)
            {
                Debug.Log("Not all cameras are set. Please assign the relevant cameras from the inspector");
                return;
            }

            fpController.SetActive(false);
            spaceshipCamera.SetActive(false);
            grassMowerCamera.SetActive(false);

            _spaceShip.GetComponent<SpaceshipController>().enabled = false;
            _spaceShipThrusterGlow.gameObject.SetActive(false);
            _spaceShipControlsText.gameObject.SetActive(false);

            _grassMower.GetComponent<GrassMowerController>().enabled = false;
            _grassMower.GetComponent<GPUInstancerInstanceRemover>().enabled = false;
            _grassMowerControlsText.gameObject.SetActive(false);

            switch (cameraMode)
            {
                case CameraModes.FPMode:
                    fpController.SetActive(true);
                    _activeCameraGO = fpController;
                    break;
                case CameraModes.SpaceshipMode:
                    spaceshipCamera.SetActive(true);
                    _spaceShip.GetComponent<SpaceshipController>().enabled = true;
                    _spaceShipThrusterGlow.gameObject.SetActive(true);
                    _spaceShipControlsText.gameObject.SetActive(true);
                    _activeCameraGO = spaceshipCamera;
                    break;
                case CameraModes.MowerMode:
                    grassMowerCamera.SetActive(true);
                    _grassMower.GetComponent<GrassMowerController>().enabled = true;
                    _grassMower.GetComponent<GPUInstancerInstanceRemover>().enabled = true;
                    _grassMowerControlsText.gameObject.SetActive(true);
                    _activeCameraGO = grassMowerCamera;
                    break;
            }

            _currentCameraMode = cameraMode;
            
            // Notify GPU Instancer of the camera change:
            GPUInstancerAPI.SetCamera(_activeCameraGO.GetComponentInChildren<Camera>());
        }

        private void DisableLoadingTerrainDetailsText()
        {
            _loadingTerrainDetailsText.SetActive(false);
            GPUInstancerAPI.StopListeningGPUIEvent(GPUInstancerEventType.DetailInitializationFinished, DisableLoadingTerrainDetailsText);
        }

        private void SetQualityMode(QualityMode qualityMode)
        {
            if (!detailManager.gameObject.activeSelf)
            {
                _currentQualityModeText.text = "Current Quality Mode: GPU Instancer disabled (Unity terrain)";
            }
            else
            {
                _currentQualityModeText.text = "Current Quality Mode: " + qualityMode + " Quality";

                if (_currentQualityMode == qualityMode)
                    return;

                _currentQualityMode = qualityMode;

                SetPrototypesByQuality(qualityMode);
                

                GPUInstancerAPI.UpdateDetailInstances(detailManager, true);
            }
        }

        private void SetPrototypesByQuality(QualityMode qualityMode)
        {
            for (int i = 0; i < detailManager.prototypeList.Count; i++)
            {
                GPUInstancerDetailPrototype detailPrototype = (GPUInstancerDetailPrototype)detailManager.prototypeList[i];

                switch (qualityMode)
                {
                    case QualityMode.Low:
                        detailPrototype.isBillboard = !detailPrototype.usePrototypeMesh;
                        detailPrototype.useCrossQuads = false;
                        //detailPrototype.quadCount = 2;
                        //detailPrototype.billboardDistance = 50f;
                        detailPrototype.isShadowCasting = false;
                        detailPrototype.maxDistance = 150f;
                        break;
                    case QualityMode.Mid:
                        detailPrototype.isBillboard = false;
                        detailPrototype.useCrossQuads = !detailPrototype.usePrototypeMesh;
                        detailPrototype.quadCount = 2;
                        //detailPrototype.billboardDistance = 50f;
                        detailPrototype.isShadowCasting = detailPrototype.usePrototypeMesh;
                        detailPrototype.maxDistance = 250;
                        break;
                    case QualityMode.High:
                        detailPrototype.isBillboard = false;
                        detailPrototype.useCrossQuads = !detailPrototype.usePrototypeMesh;
                        detailPrototype.quadCount = 4;
                        //detailPrototype.billboardDistance = 50f;
                        detailPrototype.isShadowCasting = true;
                        detailPrototype.maxDistance = 500;
                        break;
                }
            }
        }
    }
}

