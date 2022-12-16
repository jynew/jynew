using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TapSDK.UI
{
    public class UIManager : MonoSingleton<UIManager>
    {
        public const int TOPPEST_SORTING_ORDER = 32767;

        private const int LOADING_PANEL_SORTING_ORDER = TOPPEST_SORTING_ORDER;

        private const int TOAST_PANEL_SORTING_ORDER = TOPPEST_SORTING_ORDER - 10;
        
        private readonly Dictionary<Type, BasePanelController> _registerPanels = new Dictionary<Type, BasePanelController>();

        // uicamera
        private Camera _uiCamera;

        private GameObject _uiRoot;

        public int uiOpenOrder;// UI打开时的Order值 用来标识界面层级顺序

        public Camera UiCamera => _uiCamera;

        protected override void Init()
        {
            if (_uiRoot == null)
            {
                CreateUIRoot();
            }
        }

        private void CreateUIRoot()
        {
            _uiRoot = Instantiate(Resources.Load<GameObject>("TapSDKUIRoot"));
            DontDestroyOnLoad(_uiRoot);
            var canvas = _uiRoot.GetComponent<Canvas>();
            _uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? Camera.main : canvas.worldCamera;
        }
        
        /// <summary>
        /// Get Or Create UI asynchronously
        /// </summary>
        /// <param name="path">the prefab path</param>
        /// <param name="param">opening param</param>
        /// <param name="onAsyncGet">if u want to get ui do something,here is for u, which is invoked after BasePanelController.OnLoadSuccess</param>
        /// <typeparam name="TPanelController"></typeparam>
        /// <returns>get panel instance if sync mode load</returns>
        public async Task<TPanelController> OpenUIAsync<TPanelController>(string path, IOpenPanelParameter param = null, Action<BasePanelController> onAsyncGet = null) where TPanelController : BasePanelController
        {
            var basePanelController = GetUI<TPanelController>();

            // 如果已有界面(之前缓存过的),则不执行任何操作
            if (basePanelController != null)
            {
                if (!basePanelController.canvas.enabled)
                {
                    basePanelController.canvas.enabled = true;
                }

                onAsyncGet?.Invoke(basePanelController);

                return basePanelController;
            }
            
            ResourceRequest request = Resources.LoadAsync<GameObject>(path);
            while (request.isDone == false)
            {
                await Task.Yield();
            }
            
            GameObject go = request.asset as GameObject;
            var basePanel = Instantiate(go).GetComponent<TPanelController>();
            if (basePanel != null)
            {
                InternalOnPanelLoaded(typeof(TPanelController), basePanel, param);    
                onAsyncGet?.Invoke(basePanel);
                return basePanel;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// Get Or Create UI asynchronously
        /// </summary>
        /// <param name="panelType">the panel Type</param>
        /// <param name="path">the prefab path</param>
        /// <param name="param">opening param</param>
        /// <param name="onAsyncGet">if u want to get ui do something,here is for u, which is invoked after BasePanelController.OnLoadSuccess</param>
        /// <returns>get panel instance if sync mode load</returns>
        public async Task<BasePanelController> OpenUIAsync(Type panelType, string path, IOpenPanelParameter param = null, Action<BasePanelController> onAsyncGet = null)
        {
            if (!typeof(BasePanelController).IsAssignableFrom(panelType))
            {
                return null;
            }
            var basePanelController = GetUI(panelType);

            // 如果已有界面(之前缓存过的),则不执行任何操作
            if (basePanelController != null)
            {
                if (!basePanelController.canvas.enabled)
                {
                    basePanelController.canvas.enabled = true;
                }

                onAsyncGet?.Invoke(basePanelController);

                return basePanelController;
            }
            
            ResourceRequest request = Resources.LoadAsync<GameObject>(path);
            while (request.isDone == false)
            {
                await Task.Yield();
            }
            
            GameObject go = request.asset as GameObject;
            var basePanel = Instantiate(go).GetComponent<BasePanelController>();
            if (basePanel != null)
            {
                InternalOnPanelLoaded(panelType, basePanel, param);    
                onAsyncGet?.Invoke(basePanel);
                return basePanel;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Or Create UI
        /// </summary>
        /// <param name="path">the prefab path</param>
        /// <param name="param">opening param</param>
        /// <typeparam name="TPanelController"></typeparam>
        /// <returns>get panel instance if sync mode load</returns>
        public TPanelController OpenUI<TPanelController>(string path, IOpenPanelParameter param = null) where TPanelController : BasePanelController
        {
            TPanelController basePanelController = GetUI<TPanelController>();

            // 如果已有界面(之前缓存过的),则不执行任何操作
            if (basePanelController != null)
            {
                if (!basePanelController.canvas.enabled)
                {
                    basePanelController.canvas.enabled = true;
                }

                return basePanelController;
            }

            var go = Resources.Load(path) as GameObject;
            if (go != null)
            {
                var basePanel = GameObject.Instantiate(go).GetComponent<TPanelController>();

                uiOpenOrder++;

                _registerPanels.Add(typeof(TPanelController), basePanel);

                basePanel.OnLoaded(param);
                    
                basePanel.SetOpenOrder(uiOpenOrder);
                return basePanel;
            }
            return null;
        }

        /// <summary>
        /// Get Or Create UI
        /// </summary>
        /// <param name="panelType">panel type MUST based on BasePanelController</param>
        /// <param name="path">the prefab path</param>
        /// <param name="param">opening param</param>
        /// <returns>get panel instance if sync mode load</returns>
        public BasePanelController OpenUI(Type panelType, string path, IOpenPanelParameter param = null)
        {
            if (!typeof(BasePanelController).IsAssignableFrom(panelType))
            {
                return null;
            }
            var basePanelController = GetUI(panelType);

            // 如果已有界面(之前缓存过的),则不执行任何操作
            if (basePanelController != null)
            {
                if (basePanelController != null && !basePanelController.canvas.enabled)
                {
                    basePanelController.canvas.enabled = true;
                }
                
                return basePanelController;
            }

            var panelGo = Resources.Load(path) as GameObject;
            if (panelGo != null)
            {
                var basePanel = GameObject.Instantiate(panelGo).GetComponent<BasePanelController>();

                uiOpenOrder++;

                _registerPanels.Add(panelType, basePanel);

                basePanel.OnLoaded(param);
                    
                basePanel.SetOpenOrder(uiOpenOrder);
                return basePanel;
            }
            return null;
        }

        public BasePanelController GetUI(Type panelType)
        {
            if (!typeof(BasePanelController).IsAssignableFrom(panelType))
            {
                return null;
            }

            if (_registerPanels.TryGetValue(panelType, out BasePanelController basePanelController))
            {
                return basePanelController;
            }

            return null;
        }

        public TPanelController GetUI<TPanelController>() where TPanelController : BasePanelController
        {
            Type panelType = typeof(TPanelController);

            if (_registerPanels.TryGetValue(panelType, out BasePanelController panel))
            {
                return (TPanelController)panel;
            }
                
            return null;
        }

        public bool CloseUI(Type panelType)
        {
            if (!typeof(BasePanelController).IsAssignableFrom(panelType))
            {
                return false;
            }
            BasePanelController baseController = GetUI(panelType);
            if (baseController != null)
            {
                if (panelType == typeof(BasePanelController))     // 标尺界面是测试界面 不用关闭
                    return false;
                else
                    baseController.Close();
                return true;
            }
            return false;
        }

        public bool CloseUI<TPanelController>() where TPanelController : BasePanelController
        {
            TPanelController panel = GetUI<TPanelController>();
            if (panel != null)
            {
                panel.Close();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// add ui would invoked after create ui automatically, don't need mannually call it in most case
        /// </summary>
        public void AddUI(BasePanelController panel)
        {
            if (panel == null)
            {
                return;
            }
            panel.transform.SetParent(panel.AttachedParent);

            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = Vector3.one;
            panel.transform.localRotation = Quaternion.identity;
            panel.gameObject.name = panel.gameObject.name.Replace("(Clone)", "");
        }

        /// <summary>
        /// remove ui would invoked automatically, don't need mannually call it in most case
        /// </summary>
        public void RemoveUI(BasePanelController panel)
        {
            if (panel == null)
            {
                return;
            }
            Type panelType = panel.GetType();
            if (_registerPanels.ContainsKey(panelType))
            {
                _registerPanels.Remove(panelType);
            }
            panel.Dispose();
        }

        /// <summary>
        /// take some ui to the most top layer
        /// </summary>
        /// <param name="panelType"></param>
        public void ToppedUI(Type panelType)
        {
            if (!typeof(BasePanelController).IsAssignableFrom(panelType))
            {
                return;
            }
            ToppedUI(GetUI(panelType));
        }

        /// <summary>
        /// take some ui to the most top layer
        /// </summary>
        /// <param name="panel"></param>
        public void ToppedUI(BasePanelController panel)
        {
            if (panel == null)
            {
                return;
            }

            uiOpenOrder++;

            panel.SetOpenOrder(uiOpenOrder);
        }
        
        /// <summary>
        /// open toast panel for tip info
        /// </summary>
        public void OpenToast(string text, float popupTime = 2.0f)
        {
            if (!string.IsNullOrEmpty(text) && popupTime > 0)
            {
                var toast = OpenUI<ToastPanelController>("Toast", new ToastPanelOpenParam(text, popupTime));
                if (toast != null)
                {
                    var rect = toast.transform as RectTransform;
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.anchoredPosition = Vector2.zero;
                    rect.sizeDelta = Vector2.zero;
                    toast.SetOpenOrder(TOAST_PANEL_SORTING_ORDER);
                }
            }
        }
        
        /// <summary>
        /// open toast panel for tip info
        /// </summary>
        public async Task OpenToastAsync(string text, float popupTime = 2.0f)
        {
            if (!string.IsNullOrEmpty(text) && popupTime > 0)
            {
                var toast = await OpenUIAsync<ToastPanelController>("Toast", new ToastPanelOpenParam(text, popupTime));
                if (toast != null)
                {
                    var rect = toast.transform as RectTransform;
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.anchoredPosition = Vector2.zero;
                    rect.sizeDelta = Vector2.zero;
                    toast.SetOpenOrder(TOAST_PANEL_SORTING_ORDER);
                }
            }
        }

        /// <summary>
        /// open loading panel that would at the toppest layer and block interaction
        /// </summary>
        public void OpenLoading()
        {
            var loadingPanel = OpenUI<LoadingPanelController>("Loading");
            if (loadingPanel != null)
            {
                var rect = loadingPanel.transform as RectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
                // https://www.reddit.com/r/Unity3D/comments/2b1g1l/order_in_layer_maximum_value/
                loadingPanel.SetOpenOrder(LOADING_PANEL_SORTING_ORDER);
            }
        }
        
        /// <summary>
        /// open loading panel that would at the toppest layer and block interaction
        /// </summary>
        public async Task OpenLoadingAsync()
        {
            var loadingPanel = await OpenUIAsync<LoadingPanelController>("Loading");
            if (loadingPanel != null)
            {
                var rect = loadingPanel.transform as RectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
                // https://www.reddit.com/r/Unity3D/comments/2b1g1l/order_in_layer_maximum_value/
                loadingPanel.SetOpenOrder(LOADING_PANEL_SORTING_ORDER);
            }
        }

        public void CloseLoading()
        {
            var loadingPanel = GetUI<LoadingPanelController>();
            if (loadingPanel != null)
            {
                loadingPanel.Close();
            }
        }

        private void InternalOnPanelLoaded(Type tPanelController, BasePanelController basePanel, IOpenPanelParameter param = null)
        {
            uiOpenOrder++;

            _registerPanels.Add(tPanelController, basePanel);

            basePanel.OnLoaded(param);
            
            basePanel.SetOpenOrder(uiOpenOrder);
        }
        
        #region external api
        public Transform GetUIRootTransform()
        {
            return _uiRoot.transform;
        }

        public Camera GetUICamera()
        {
            return _uiCamera;
        }
        #endregion
    }
}