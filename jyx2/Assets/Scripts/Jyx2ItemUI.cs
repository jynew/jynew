/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


using Jyx2;
using Jyx2.UINavigation;
using Jyx2.Util;
using Steamworks.Ugc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Jyx2ItemUI : Selectable,INavigable,IDataContainer<KeyValuePair<string,(int,int)>>,IPointerClickHandler
{
    public Image m_Image;
    public Text m_NameText;
    public Text m_CountText;
    private int _id;

    public int ItemId => _id;

    [SerializeField]
    private Graphic m_CheckMark;

    [SerializeField]
    private bool m_IsSelected = false;

    public bool IsSelected => m_IsSelected;

    public event Action<Jyx2ItemUI> OnItemSelect;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnItemSelect = null;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshMark();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        RefreshMark();
    }
#endif

    public LItemConfig GetItemConfigData()
    {
        return LuaToCsBridge.ItemTable[ItemId];
    }

    public Selectable GetSelectable()
    {
        return this;
    }

    public void Connect(INavigable up = null, INavigable down = null, INavigable left = null, INavigable right = null)
    {
        Navigation navigation = new Navigation();
        navigation.mode = Navigation.Mode.Explicit;
        navigation.selectOnUp = up?.GetSelectable();
        navigation.selectOnDown = down?.GetSelectable();
        navigation.selectOnLeft = left?.GetSelectable();
        navigation.selectOnRight = right?.GetSelectable();
        this.navigation = navigation;
    }

    public void Select(bool notifyEvent)
    {
        SetSelectState(true, notifyEvent);
    }

    public void SetSelectState(bool state, bool notifyEvent)
    {
        m_IsSelected = state;
        RefreshMark();
        if (notifyEvent && m_IsSelected)
        {
            OnItemSelect?.Invoke(this);
        }
    }

    private void RefreshItem(int id, int count)
    {
        _id = id;
        var item = GetItemConfigData();
        var htmlColorStr = GetItemNameColorStr();

        m_NameText.text = string.Format("<color={0}>{1}</color>", htmlColorStr, item.Name);
        m_CountText.text = count > 1 ? count.ToString() : "";

        m_Image.LoadAsyncForget(item.GetPic());
        
    }

    private string GetItemNameColorStr()
    {
        var result = ColorStringDefine.Default;
        var item = GetItemConfigData();
        if (item == null || !item.IsBook())
            return result;
        //0-阴性内力，1-阳性内力，2-中性内力
        if (item.NeedMPType == 0)
            result = ColorStringDefine.Mp_type0;
        else if(item.NeedMPType == 1)
            result = ColorStringDefine.Mp_type1;
        return result;
    }

    public void SetData(KeyValuePair<string, (int, int)> data)
    {
        if (!int.TryParse(data.Key, out int itemId))
            itemId = -1;
        int itemCount = data.Value.Item1;
        RefreshItem(itemId, itemCount);
    }

    private void RefreshMark(bool instant = true)
    {
        if (m_CheckMark != null)
        {
            if (!Application.isPlaying)
            {
                m_CheckMark.canvasRenderer.SetAlpha(IsSelected ? 1f : 0f);
            }
            else
            {
                m_CheckMark.CrossFadeAlpha(IsSelected ? 1f : 0f, instant ? 0f : 0.1f, true);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Select(true);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        SetSelectState(true, true);
    }
}
