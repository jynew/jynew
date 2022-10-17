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
using System.Linq;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2;
using Jyx2.MOD;
using Jyx2.ResourceManagement;
using UnityEngine;

public enum UILayer 
{
    MainUI = 0,//主界面层
    NormalUI = 1,//普通界面层
    PopupUI = 2,//弹出层
    Top = 3,//top层 高于弹出层
}

public class Jyx2_UIManager : MonoBehaviour
{
    static Jyx2_UIManager _instace;
    public static Jyx2_UIManager Instance 
    {
        get 
        {
            if (_instace == null) 
            {
                var prefab = Resources.Load<GameObject>("MainCanvas");
                var obj = Instantiate(prefab);
                obj.gameObject.name = "MainCanvas";
                _instace = obj.GetComponent<Jyx2_UIManager>();
                _instace.Init();
                DontDestroyOnLoad(_instace);
            }
            return _instace;
        }
    }

    private Transform m_mainParent;
    private Transform m_normalParent;
    private Transform m_popParent;
    private Transform m_topParent;

    private Dictionary<string, Jyx2_UIBase> m_uiDic = new Dictionary<string, Jyx2_UIBase>();
    private Jyx2_UIBase m_currentMainUI;
    private Stack<Jyx2_UIBase> m_normalUIStack = new Stack<Jyx2_UIBase>();
    private Stack<Jyx2_UIBase> m_PopUIStack = new Stack<Jyx2_UIBase>();

    void Init()
    {
        m_mainParent = transform.Find("MainUI");
        m_normalParent = transform.Find("NormalUI");
        m_popParent = transform.Find("PopupUI");
        m_topParent = transform.Find("Top");
    }

    public bool IsTopVisibleUI(Jyx2_UIBase ui)
	{
        if (!ui.gameObject.activeSelf)
            return false;

        if (ui.Layer == UILayer.MainUI)
		{
			//make sure no normal and popup ui on top
			return noShowingNormalUi() &&
				(noInterferingPopupUI());
		}
		else if (ui.Layer == UILayer.NormalUI)
		{
            Jyx2_UIBase currentUi = m_normalUIStack.Count > 0 ? m_normalUIStack.Peek() : null;
            if (currentUi == null)
                return true;
            
			return (ui == currentUi || ui.transform.IsChildOf(currentUi.transform)) && noInterferingPopupUI();
		}
        else if (ui.Layer == UILayer.PopupUI)
		{
            return (m_PopUIStack.Count > 0 ? m_PopUIStack.Peek() : null) == ui;
		}
        else if (ui.Layer == UILayer.Top)
		{
            return true;
		}

        return false;
	}

	private bool noShowingNormalUi()
	{
        return !m_normalUIStack
            .Any(ui => ui.gameObject.activeSelf);
	}

	private bool noInterferingPopupUI()
	{
        //common tips panel has no interaction, doesn't count towards active uis
        return !m_normalUIStack
            .Any(ui => ui.gameObject.activeSelf) || (m_PopUIStack.All(p => p is CommonTipsUIPanel));
	}

	public async void GameStart()
    {
        // await UniTask.WaitForEndOfFrame();
        await RuntimeEnvSetup.Setup();
        
        
        //TODO: 20220723 CG: 待调整Loading出现的逻辑，因为ResLoader的初始化很慢。但这里目前有前后依赖关系，必须在ResLoader初始化之后
        await ShowUIAsync(nameof(GameMainMenu));

        string info = string.Format("<b>版本：{0} 模组：{1}</b>".GetContent(nameof(Jyx2_UIManager)),
            Application.version,
            RuntimeEnvSetup.CurrentModConfig.ModName);
        
        await ShowUIAsync(nameof(GameInfoPanel), info);
        
        GraphicSetting.GlobalSetting.Execute();
    }

    public Transform GetUIParent(UILayer layer) 
    {
        switch (layer) 
        {
            case UILayer.MainUI:
                return m_mainParent;
            case UILayer.NormalUI:
                return m_normalParent;
            case UILayer.PopupUI:
                return m_popParent;
            case UILayer.Top:
                return m_topParent;
            default:
                return transform;
        }
    }

    Dictionary<string, object[]> _loadingUIParams = new Dictionary<string, object[]>();
    
    public async UniTask ShowUIAsync(string uiName, params object[] allParams)
    {
        Jyx2_UIBase uibase;
        if (m_uiDic.ContainsKey(uiName))
        {
            uibase = m_uiDic[uiName];
            if (uibase.IsOnly)//如果这个层唯一存在 那么先关闭其他
                PopAllUI(uibase.Layer);
            PushUI(uibase);
            uibase.Show(allParams);
        }
        else
        {
            if (_loadingUIParams.ContainsKey(uiName)) //如果正在加载这个UI 那么覆盖参数
            {
                _loadingUIParams[uiName] = allParams;
                return;
            }

            _loadingUIParams[uiName] = allParams;
            string uiPath = string.Format(GameConst.UI_PREFAB_PATH, uiName);

            var prefab = await ResLoader.LoadAsset<GameObject>(uiPath);
            var go = Instantiate(prefab);
            
            OnUILoaded(go);
        }
    }

    //UI加载完后的回调
    void OnUILoaded(GameObject go) 
    {
        string uiName = go.name.Replace("(Clone)", "");
        object[] allParams = _loadingUIParams[uiName];
        Component com = GameUtil.GetOrAddComponent(go.transform, uiName);
        Jyx2_UIBase uibase = com as Jyx2_UIBase;

        Transform parent = GetUIParent(uibase.Layer);
        go.transform.SetParent(parent);

		//听取ui的 OnVisibilityToggle event
		uibase.VisibilityToggled += Uibase_OnVisibilityToggle;

        uibase.Init();
        m_uiDic[uiName] = uibase;
        if (uibase.IsOnly)//如果这个层唯一存在 那么先关闭其他
            PopAllUI(uibase.Layer);
        PushUI(uibase);

        uibase.Show(allParams);
        _loadingUIParams.Remove(uiName);
    }

	private void Uibase_OnVisibilityToggle(Jyx2_UIBase ui, bool obj)
	{
		UIVisibilityToggled?.Invoke(ui, obj);
	}

    public event Action<Jyx2_UIBase, bool> UIVisibilityToggled;

	//显示主界面 LoadingPanel中加载完场景调用 移到这里来 方便修改
	public async UniTask ShowMainUI()
    {
        var map = LevelMaster.GetCurrentGameMap();
        /*if (map == null)
        {
            //this.HideUI("MainUIPanel");
            this.ShowUI(nameof(BattleMainUIPanel),BattleMainUIState.None);
            Debug.Log("当前地图没有地图数据");
            return;
        }*/
        if (map != null && map.Tags.Contains("BATTLE"))
        {
            await ShowUIAsync(nameof(BattleMainUIPanel), BattleMainUIState.None);
            return;
        }
        else
            await ShowUIAsync(nameof(MainUIPanel));
    }

    void PushUI(Jyx2_UIBase uibase) 
    {
        switch (uibase.Layer)
        {
            case UILayer.MainUI:
                if (m_currentMainUI && m_currentMainUI != uibase)
                {
                    m_currentMainUI.Hide();
                }
                m_currentMainUI = uibase;
                break;
            case UILayer.NormalUI:
                m_normalUIStack.Push(uibase);
                break;
            case UILayer.PopupUI:
                m_PopUIStack.Push(uibase);
                break;
        }
    }

    void PopAllUI(UILayer layer) 
    {
        if (layer == UILayer.NormalUI)
        {
            PopUI(null, m_normalUIStack);
        }
        else if (layer == UILayer.PopupUI) 
        {
            PopUI(null, m_PopUIStack);
        }
    }

    void PopUI(Jyx2_UIBase ui, Stack<Jyx2_UIBase> uiStack) 
    {
        if (!uiStack.Contains(ui))
            return;
        Jyx2_UIBase node = uiStack.Pop();
        while (node) 
        {
            if (node == ui) 
            {
                node.Hide();
                return;
            }
            if (uiStack.Count <= 0)
                return;
            node.Hide();
            node = uiStack.Pop();
        }
    }

    public void HideUI(string uiName) 
    {
        if (!m_uiDic.ContainsKey(uiName))
            return;
        Jyx2_UIBase uibase = m_uiDic[uiName];
        if (m_normalUIStack.Contains(uibase))
        {
            PopUI(uibase, m_normalUIStack);
        }
        else if (m_PopUIStack.Contains(uibase))
        {
            PopUI(uibase, m_PopUIStack);
        }
        else if (uibase.Layer == UILayer.MainUI)
            uibase.Hide();
        else
            uibase.Hide();
    }

    public void HideAllUI()
    {
        foreach (var item in m_uiDic)
        {
            HideUI(item.Key);
        }
    }


    public void CloseAllUI()
    {
        //Keys仍然会引用Dictionary内部元素迭代 用一个新List暂存来遍历
        var uiNames = m_uiDic.Keys.ToList(); 
        foreach (var uiName in uiNames)
        {
            if(m_uiDic.ContainsKey(uiName))
            {
                HideUI(uiName);
                Destroy(m_uiDic[uiName].gameObject);
                m_uiDic.Remove(uiName);
            }
        }
    }




    public void SetMainUIActive(bool active) 
    {
        if (m_currentMainUI == null)
            return;
        if (active)
            m_currentMainUI.Show();
        else
            m_currentMainUI.Hide();
    }

    public Camera GetUICamera() 
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas.worldCamera)
            return canvas.worldCamera;
        return Camera.main;
    }


    public bool IsUIOpen(string uiName)
    {
        if (!m_uiDic.ContainsKey(uiName))
            return false;
        if (m_uiDic[uiName] == null)
            return false;
        return m_uiDic[uiName].gameObject.activeSelf;
    }
}
