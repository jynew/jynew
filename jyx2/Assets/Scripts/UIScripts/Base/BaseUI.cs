using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Jyx2.Middleware;

public enum BUTTON_TYPE
{
    CLICK,
    SWITCH,
    CUSTOM
}

public enum TOGGLE_TYPE
{
    SWITCH,
    CUSTOM
}
namespace HSUI
{
    public abstract class BaseUI : MonoBehaviour
    {
        /// <summary>
        /// 隐藏本元素
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Close()
        {
            HSUnityTools.SafeDestroyGameObject(this.gameObject);
        }

        #region 控件绑定

        // 绑定T
        protected void Bind<T>(ref T go, string path = null)
        {
            if (path == null)
            {
                if (go == null)
                    go = GetComponent<T>();
            }
            else
            {
                Transform tf = transform.Find(path);
                if (go == null && tf != null)
                    go = tf.GetComponent<T>();
            }
        }

        /// <summary>
        /// Button绑定Listener
        /// 
        /// 绑定一个新的方法会删除其他在此Button上的方法
        /// </summary>
        /// <param name="button">Button实例</param>
        /// <param name="callback">绑定的方法</param>
        /// <param name="type">Button的点击类型</param>
        public void BindListener(Button button, Action callback, BUTTON_TYPE type = BUTTON_TYPE.CLICK)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate
                {
                    switch (type)
                    {
                        case BUTTON_TYPE.CLICK:
                            //AudioManager.PlayEffect("音效.点击");
                            break;
                        case BUTTON_TYPE.SWITCH:
                            //AudioManager.PlayEffect("音效.切换界面");
                            break;
                        default:
                            break;
                    }
                    callback();
                });
                var nav = Navigation.defaultNavigation;
                nav.mode = Navigation.Mode.None;
                button.navigation = nav;
            }
        }

        /// <summary>
        /// Toggle绑定Listener
        /// </summary>
        /// <param name="toggle">Toggle实例</param>
        /// <param name="callback">Toggle打开时的回调</param>
        /// <param name="type">Toggle的类型</param>
        /// <param name="offcallback">Toggle关闭时的回调</param>
        public void BindListener(Toggle toggle, Action<bool> callback, TOGGLE_TYPE type = TOGGLE_TYPE.SWITCH)
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(delegate (bool isOn)
                {
                    callback(isOn);
                });
                var nav = Navigation.defaultNavigation;
                nav.mode = Navigation.Mode.None;
                toggle.navigation = nav;
            }
        }

        /// <summary>
        /// Slider绑定Listener
        /// </summary>
        /// <param name="slider">Slider实例</param>
        /// <param name="callback">Slider值改变时的回调</param>
        public void BindListener(Slider slider, Action<float> callback)
        {
            if (slider != null)
            {
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener(delegate (float val)
                {
                    callback(val);
                });
                var nav = Navigation.defaultNavigation;
                nav.mode = Navigation.Mode.None;
                slider.navigation = nav;
            }
        }

        /// <summary>
        /// 设置UI控件的Text
        /// 此组件在物体的子物体需要含有Text
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">MonoBehaviourUI</param>
        /// <param name="text">设置的Text</param>
        public void SetText<T>(T obj, string text) where T : MonoBehaviour
        {
            if (obj == null) return;
            var textChild = obj.GetComponentInChildren<Text>();
            if (textChild == null) return;
            textChild.text = text;
        }

        /// <summary>
        /// 设置UI控件的Text
        /// 此组件在物体的子物体需要含有Text
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">MonoBehaviourUI</param>
        /// <param name="text">设置的Text</param>
        /// <param name="color">Text的颜色</param>
        public void SetText<T>(T obj, string text, Color color) where T : MonoBehaviour
        {
            if (obj == null) return;
            var textChild = obj.GetComponentInChildren<Text>();
            if (textChild == null) return;
            textChild.text = text;
            textChild.color = color;
        }

        /// <summary>
        /// GameObjec子物体设置Text
        /// </summary>
        /// <param name="go"></param>
        /// <param name="text"></param>
        public void SetText(GameObject go, string text)
        {
            if (go != null && go.transform.Find("Text") != null)
            {
                go.transform.Find("Text").GetComponent<Text>().text = text;
            }
        }

        /// <summary>
        /// GameObjec子物体设置Text
        /// </summary>
        /// <param name="go"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public void SetText(GameObject go, string text, Color color)
        {
            if (go != null && go.transform.Find("Text") != null)
            {
                go.transform.Find("Text").GetComponent<Text>().text = text;
                go.transform.Find("Text").GetComponent<Text>().color = color;
            }
        }

        /// <summary>
        /// 清除Transform下的所有子项
        /// </summary>
        /// <param name="transform"></param>
        public void ClearChildren(Transform transform)
        {
            HSUnityTools.DestroyChildren(transform);
        }
        #endregion

        #region UI组件位置、动态效果相关方法

        private Dictionary<Transform, Vector3> _moveInSourceLocations = null;

        //移动位置
        //从一个相对偏移位置移到当前设置的位置
        //protected void MoveIn(Transform go, float dx, float dy, float delay, float duration)
        //{
        //    // 暂停游戏时禁止播放动画
        //    if (Time.timeScale == 0)
        //        return;

        //    //初始化位置
        //    if (_moveInSourceLocations == null)
        //        _moveInSourceLocations = new Dictionary<Transform, Vector3>();

        //    //第一次进来，记住原始位置
        //    if (!_moveInSourceLocations.ContainsKey(go))
        //    {
        //        _moveInSourceLocations.Add(go, go.localPosition);
        //    }

        //    go.DOKill();//先移除所有当前在跑的tween
        //    go.localPosition = _moveInSourceLocations[go]; //还原位置

        //    float cx = go.localPosition.x;
        //    float cy = go.localPosition.y;

        //    go.localPosition = new Vector3(cx + dx, cy + dy, go.localPosition.z);

        //    Container.Resolve<IUITools>().CallWithDelay(() =>
        //    {
        //        if (go != null)
        //        {
        //            go.DOLocalMove(new Vector3(cx, cy, go.localPosition.z), duration);
        //        }
        //    }, delay);
        //}

        //protected void FadeInActiveChildren(bool enableAnimation = true)
        //{
        //    foreach (Transform tf in this.transform)
        //    {
        //        var go = tf.gameObject;
        //        if (go.activeSelf)
        //        {
        //            if (enableAnimation)
        //            {
        //                MoveIn(go.transform, 80, 0, 0.05f, 0.2f);
        //                Container.Resolve<IUITools>().FadeIn(go, 0.05f, 0.05f);
        //            }
        //        }
        //    }
        //}

        #endregion

        #region 全面屏适配

        public void ExpandObjectScale(Transform tf)
        {
            //if (Container.Resolve<IUITools>().IsUltraWideScreen())
            //{
            //    if (tf != null)
            //    {
            //        tf.localScale = new Vector3(1.112f, 1.112f, 1);
            //    }
            //}
        }

        public void ReduceObjectScale(Transform tf)
        {
            //    if (Container.Resolve<IUITools>().IsUltraWideScreen())
            //    {
            //        if (tf != null)
            //        {
            //            tf.localScale = new Vector3(0.9f, 0.9f, 1);
            //        }
            //    }
        }

        public void ExpandBackgroundScale(string parentPath = "")
        {
            //if (Container.Resolve<IUITools>().IsUltraWideScreen())
            //{
            //    Transform Background = null;
            //    if (string.IsNullOrEmpty(parentPath))
            //        Background = transform.Find("Background");
            //    else
            //        Background = transform.Find(parentPath + "/Background");
            //    ExpandObjectScale(Background);
            //}
        }

        protected bool _isAdjustScaled = false;

        public void ExpandPanelHeight(Transform tf = null)
        {
            //if (Container.Resolve<IUITools>().IsUltraWideScreen() && !_isAdjustScaled)
            //{
            //    _isAdjustScaled = true;
            //    if (tf == null)
            //        tf = this.transform;
            //    RectTransform rtf = tf.GetComponent<RectTransform>();
            //    if (rtf != null)
            //    {
            //        rtf.offsetMax = new Vector2(rtf.offsetMax.x, rtf.offsetMax.y + 32f);
            //        rtf.offsetMin = new Vector2(rtf.offsetMin.x, rtf.offsetMin.y - 32f);
            //    }
            //}
        }

        public void AdjustLeftRegionPosition(Transform tf)
        {
            //if (Container.Resolve<IUITools>().IsUltraWideScreen())
            //    tf.localPosition = new Vector3(tf.localPosition.x - 50, tf.localPosition.y, tf.localPosition.z);
        }

        public void AdjustRightRegionPosition(Transform tf)
        {
            //if (Container.Resolve<IUITools>().IsUltraWideScreen())
            //    tf.localPosition = new Vector3(tf.localPosition.x + 50, tf.localPosition.y, tf.localPosition.z);
        }

        public void UltraWideScreenAdjust()
        {
            ExpandBackgroundScale();
            ExpandPanelHeight();
        }


        #endregion
    }
}
