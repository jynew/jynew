using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TapSDK.UI
{
    /// <summary>
    /// base panel of TapSDK UI module
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BasePanelController : MonoBehaviour
    {
        /// <summary>
        /// the canvas related to this panel
        /// </summary>
        [HideInInspector]
        public Canvas canvas;

        /// <summary>
        /// the canvas group related to this panel
        /// </summary>
        [HideInInspector]
        public CanvasGroup canvasGroup;

        /// <summary>
        /// fade in/out time
        /// </summary>
        private const float FADE_TIME = 0.1f;

        /// <summary>
        /// animation elapse time
        /// </summary>
        private float _animationElapse;
        
        private Vector2 _screenSize;
        private Vector2 _cachedAnchorPos;

        private RectTransform _rectTransform;

        private Coroutine _animationCoroutine;
        
        /// <summary>
        /// open parameter
        /// </summary>
        protected IOpenPanelParameter openParam;

        /// <summary>
        /// sorting order of this panel
        /// </summary>
        public int openOrder;

        /// <summary>
        /// settings about this panel
        /// </summary>
        public BasePanelConfig panelConfig;

        /// <summary>
        /// the transform parent when created it would be attached to
        /// </summary>
        /// <value></value>
        public virtual Transform AttachedParent => UIManager.Instance.GetUIRootTransform();

        #region Load
        protected virtual void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = transform as RectTransform;
            
            _screenSize = new Vector2(Screen.width, Screen.height);
            
            #if UNITY_EDITOR
            if (canvas == null)
            {
                Debug.LogErrorFormat("[TapSDK UI] BasePanel Must Be Related To Canvas Component!");
            }
            #endif
        }

        /// <summary>
        /// bind ugui components for every panel
        /// </summary>
        protected virtual void BindComponents() {}

        /// <summary>
        /// create the prefab instance
        /// </summary>
        /// <param name="param"></param>
        public void OnLoaded(IOpenPanelParameter param = null)
        {
            openParam = param;
            // 寻找组件
            BindComponents();

            // 添加到控制层
            UIManager.Instance.AddUI(this);

            // 更新层级信息
            InitCanvasSetting();

            // 开始动画效果
            OnShowEffectStart();

            // 调用加载成功方法
            OnLoadSuccess();
        }

        private void InitCanvasSetting()
        {
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                var camera = UIManager.Instance.GetUICamera();
                if (camera != null)
                {
                    canvas.worldCamera = camera;
                }
            }

            canvas.pixelPerfect = true;
            canvas.overrideSorting = true;
        }

        /// <summary>
        /// init panel logic here
        /// </summary>
        protected virtual void OnLoadSuccess()
        {

        }

        #endregion

        #region Animation

        protected virtual void OnShowEffectStart()
        {
            if (panelConfig.animationType == EAnimationMode.None)
            {
                return;
            }

            OnEffectStart();

            if ((panelConfig.animationType & EAnimationMode.Alpha) == EAnimationMode.Alpha)
            {
                canvasGroup.alpha = 0;
            }

            if ((panelConfig.animationType & EAnimationMode.Scale) == EAnimationMode.Scale)
            {
                transform.localScale = Vector3.zero;
            }
            if ((panelConfig.animationType & EAnimationMode.RightSlideIn) == EAnimationMode.RightSlideIn)
            {
                _cachedAnchorPos = _rectTransform.anchoredPosition;
                _rectTransform.anchoredPosition += new Vector2(_screenSize.x, 0);
            }
            _animationCoroutine = StartCoroutine(FadeInCoroutine(FADE_TIME));
        }

        protected virtual void OnShowEffectEnd()
        {
            OnEffectEnd();
        }

        protected virtual void OnCloseEffectStart()
        {
            OnEffectStart();

            if ((panelConfig.animationType & EAnimationMode.Alpha) == EAnimationMode.Alpha)
            {
                canvasGroup.alpha = 1;
            }
            if ((panelConfig.animationType & EAnimationMode.Scale) == EAnimationMode.Scale)
            {
                transform.localScale = Vector3.one;
            }
            if ((panelConfig.animationType & EAnimationMode.RightSlideIn) == EAnimationMode.RightSlideIn)
            {
                _rectTransform.anchoredPosition = _cachedAnchorPos;
            }
            _animationCoroutine = StartCoroutine(FadeOutCoroutine(FADE_TIME));
        }

        protected virtual void OnCloseEffectEnd()
        {
            OnEffectEnd();
            GameObject.Destroy(gameObject);
        }

        private void OnEffectStart()
        {
            _animationElapse = 0;
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }
            canvasGroup.interactable = false;
        }

        private void OnEffectEnd()
        {
            canvasGroup.interactable = true;
            _animationElapse = 0;
            _animationCoroutine = null;
        }

        private IEnumerator FadeInCoroutine(float time)
        {
            while (_animationElapse < time)
            {
                yield return null;
                _animationElapse += Time.deltaTime;
                float value = Mathf.Clamp01(_animationElapse / time);

                if ((panelConfig.animationType & EAnimationMode.Alpha) == EAnimationMode.Alpha)
                {
                    canvasGroup.alpha = value;
                }
                if ((panelConfig.animationType & EAnimationMode.Scale) == EAnimationMode.Scale)
                {
                    transform.localScale = new Vector3(value, value, value);
                }
                if ((panelConfig.animationType & EAnimationMode.RightSlideIn) == EAnimationMode.RightSlideIn)
                {
                    var temp = (1 - value) * _screenSize.x;
                    _rectTransform.anchoredPosition = new Vector2(_cachedAnchorPos.x + temp, _cachedAnchorPos.y);
                }
            }

            if ((panelConfig.animationType & EAnimationMode.Alpha) == EAnimationMode.Alpha)
            {
                canvasGroup.alpha = 1;
            }
            if ((panelConfig.animationType & EAnimationMode.Scale) == EAnimationMode.Scale)
            {
                transform.localScale = Vector3.one;
            }
            if ((panelConfig.animationType & EAnimationMode.RightSlideIn) == EAnimationMode.RightSlideIn)
            {
                _rectTransform.anchoredPosition = _cachedAnchorPos;
            }
            
            OnShowEffectEnd();
        }

        private IEnumerator FadeOutCoroutine(float time)
        {
            while (_animationElapse < time)
            {
                yield return null;
                _animationElapse += Time.deltaTime;
                float value = 1 - Mathf.Clamp01(_animationElapse / time);

                if ((panelConfig.animationType & EAnimationMode.Alpha) == EAnimationMode.Alpha)
                {
                    canvasGroup.alpha = value;
                }
                if ((panelConfig.animationType & EAnimationMode.Scale) == EAnimationMode.Scale)
                {
                    transform.localScale = new Vector3(value, value, value);
                }
                if ((panelConfig.animationType & EAnimationMode.RightSlideIn) == EAnimationMode.RightSlideIn)
                {
                    var temp = (1 - value) * _screenSize.x;
                    _rectTransform.anchoredPosition = new Vector2(_cachedAnchorPos.x + temp, _cachedAnchorPos.y);
                }
            }

            if ((panelConfig.animationType & EAnimationMode.Alpha) == EAnimationMode.Alpha)
            {
                canvasGroup.alpha = 0;
            }
            if ((panelConfig.animationType & EAnimationMode.Scale) == EAnimationMode.Scale)
            {
                transform.localScale = Vector3.zero;
            }
            if ((panelConfig.animationType & EAnimationMode.RightSlideIn) == EAnimationMode.RightSlideIn)
            {
                _rectTransform.anchoredPosition = new Vector2(_cachedAnchorPos.x + _screenSize.x, _cachedAnchorPos.y);
            }

            OnCloseEffectEnd();
        }

        #endregion
        

        /// <summary>
        /// on receive resolution change event
        /// </summary>
        /// <param name="res"></param>
        protected virtual void UIAdapt(Vector2Int res) {}

        /// <summary>
        /// common close api
        /// </summary>
        public virtual void Close()
        {
            UIManager.Instance.RemoveUI(this);
        }

        /// <summary>
        /// set canvas sorting order
        /// </summary>
        /// <param name="openOrder"></param>
        public void SetOpenOrder(int openOrder)
        {
            this.openOrder = openOrder;
            if (canvas != null)
            {
                canvas.sortingOrder = openOrder;
            }
        }

        /// <summary>
        /// also would destroy panel gameObject
        /// </summary>
        public virtual void Dispose()
        {
            if (panelConfig.animationType == EAnimationMode.None)
            {
                GameObject.Destroy(gameObject);
                return;
            }
            
            OnCloseEffectStart();
        }
    }
}
