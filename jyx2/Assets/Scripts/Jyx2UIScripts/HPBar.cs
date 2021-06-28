/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [HideInInspector]
    public Image fillImage;
    Image secondFillImage;

    float currentValue;

    Tween tweener;

    public void Init()
    {
        fillImage = transform.Find("TopFill").GetComponent<Image>();
        secondFillImage = transform.Find("SecondFill").GetComponent<Image>();
    }

    public void SetValue(float v)
    {
        if (tweener != null)
        {
            tweener.Kill();
        }

        if (currentValue == v)
        {
            return;
        }

        if (v < currentValue)
        {
            fillImage.fillAmount = v;
            tweener = DOTween.To(() => secondFillImage.fillAmount, x => secondFillImage.fillAmount = x, v, 2f).Play();
        }
        else if (v > currentValue)
        {
            secondFillImage.fillAmount = v;
            tweener = DOTween.To(() => fillImage.fillAmount, x => fillImage.fillAmount = x, v, 2f).Play();
        }
        else
        {
            fillImage.fillAmount = v;
            secondFillImage.fillAmount = v;
        }

        currentValue = v;
    }

#if false
    [Range(0, 1)]
    [SerializeField] float testValue;

    [ContextMenu("测试")]
    void Test()
    {
        SetValue(testValue);
    }
#endif 
}
