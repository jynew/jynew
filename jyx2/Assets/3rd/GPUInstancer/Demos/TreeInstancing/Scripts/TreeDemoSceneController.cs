using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPUInstancer
{
    public class TreeDemoSceneController : MonoBehaviour
    {

        public GPUInstancerTreeManager manager;
        public Text GPUIStateText;
        public Text FPSCountTextText;

        private FPS _fpsCounter;

        private void Awake()
        {
            _fpsCounter = GetComponent<FPS>();
        }

        private void Start()
        {
            QualitySettings.shadowDistance = 450;
        }

        private void Update()
        {
            FPSCountTextText.text = "FPS: " + _fpsCounter.FPSCount;
        }

        public void ToggleManager()
        {
            manager.gameObject.SetActive(!manager.gameObject.activeSelf);
            GPUIStateText.text = manager.gameObject.activeSelf ? "GPU Instancer ON" : "GPU Instancer OFF";
            GPUIStateText.color = manager.gameObject.activeSelf ? Color.green : Color.red;
        }
    }
}
