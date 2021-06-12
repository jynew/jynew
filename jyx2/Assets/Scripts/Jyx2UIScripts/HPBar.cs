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
