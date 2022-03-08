using UnityEngine;
using UnityEngine.UI;

public partial class ModPanel
{
    Button CloseBtn_Button;
    RectTransform ModParent_RectTransform;

    public void InitTrans()
    {
        ModParent_RectTransform = transform.Find("ModScroll/Viewport/ModParent").GetComponent<RectTransform>();
        CloseBtn_Button = transform.Find("CloseButton").GetComponent<Button>();
    }
}
