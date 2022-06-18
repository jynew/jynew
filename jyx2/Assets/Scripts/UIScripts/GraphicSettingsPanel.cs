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
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Object = System.Object;
using StylizedWater;

public class GraphicSettingsPanel : Jyx2_UIBase
{
    public Toggle m_FogToggle;
    public Toggle m_PostToggle;
    public Toggle m_WaterNormalToggle;
    public Toggle m_AntiAliasingToggle;
    public Toggle m_Vsynctoggle;

    public Dropdown m_maxFpsDropdown;
    public Dropdown m_QualityLevelDropdown;
    public Dropdown m_ShaderLodLevelropdown;
    public Dropdown m_ShadowQualityDropdown;
    public Dropdown m_ShadowShowLevelDropdown;

    public Button m_CloseButton;

    private GraphicSetting _graphicSetting;
    // Start is called before the first frame update
    void Start()
    {
        _graphicSetting = GraphicSetting.GlobalSetting;

        InitUI();

        m_FogToggle.onValueChanged.AddListener(SetFog);
        m_PostToggle.onValueChanged.AddListener(SetPostProcess);
        m_WaterNormalToggle.onValueChanged.AddListener(SetWaterNormal);
        m_AntiAliasingToggle.onValueChanged.AddListener(SetAntiAliasing);
        m_Vsynctoggle.onValueChanged.AddListener(SetVSync);

        m_maxFpsDropdown.onValueChanged.AddListener(DropdownMaxFps);
        m_QualityLevelDropdown.onValueChanged.AddListener(DropdownQualityLevel);
        m_ShaderLodLevelropdown.onValueChanged.AddListener(DropdownShaderLodLevel);
        m_ShadowQualityDropdown.onValueChanged.AddListener(DropdownShadowQuality);
        m_ShadowShowLevelDropdown.onValueChanged.AddListener(DropdownShadowShowLevel);


        m_CloseButton.onClick.AddListener(Close);
    }

    void Close()
    {
        _graphicSetting.Save();
        _graphicSetting.Execute();
        Jyx2_UIManager.Instance.HideUI(nameof(GameSettingsPanel));
    }
    
    public void InitUI()
    {
        m_FogToggle.isOn = _graphicSetting.HasFog == 1;
        m_PostToggle.isOn = _graphicSetting.HasPost == 1;
        m_WaterNormalToggle.isOn = _graphicSetting.HasWaterNormal == 1;
        m_AntiAliasingToggle.isOn = _graphicSetting.HasAntiAliasing == 1;
        m_Vsynctoggle.isOn = _graphicSetting.Vsync == 1;

        InitDropDown(m_maxFpsDropdown, _graphicSetting.MaxFps, "最大fps");
        InitDropDown(m_QualityLevelDropdown, _graphicSetting.QualityLevel, "图形品质");
        InitDropDown(m_ShaderLodLevelropdown, _graphicSetting.ShaderLodLevel, "Shader效果");
        InitDropDown(m_ShadowQualityDropdown, _graphicSetting.ShadowQuality, "阴影质量");
        InitDropDown(m_ShadowShowLevelDropdown, _graphicSetting.ShadowShowLevel, "阴影展现级别");
    }
    
    //根据枚举类型来初始化Dropdown
    void InitDropDown(Dropdown drop, object enumObj, string propName)
    {
        var type = enumObj.GetType();
        if (!type.IsEnum) return;
        
        var items = type.GetEnumNames().ToList();
        drop.ClearOptions();
        var first = new Dropdown.OptionData($"请选择{propName}");
        drop.options.Add(first);
        drop.AddOptions(items);
        RefreshDropDown(drop, enumObj);
    }

    void RefreshDropDown(Dropdown drop, object enumObj)
    {
        var type = enumObj.GetType();
        var items = type.GetEnumNames().ToList();
        var thisName = enumObj.ToString();
        var index = items.IndexOf(thisName);
        drop.value = index + 1;
    }

    object SetDropDown(Dropdown drop, int index, object enumObj)
    {
        if (index == 0)
        {
            RefreshDropDown(drop, enumObj);
            return null;
        }

        index--;
        var type = enumObj.GetType();
        var values = type.GetEnumValues();
        var value = values.GetValue(index);
        return value;
    }
    
    public void DropdownMaxFps(int index)
    {
        var value = SetDropDown(m_maxFpsDropdown, index, _graphicSetting.MaxFps);
        if (value == null) return;

        _graphicSetting.MaxFps = (MaxFpsEnum) value;
    }

    public void DropdownShadowShowLevel(int index)
    {
        var value = SetDropDown(m_ShadowShowLevelDropdown, index, _graphicSetting.ShadowShowLevel);
        if (value == null) return;

        _graphicSetting.ShadowShowLevel = (ShadowShowLevelEnum)value;
    }

    public void DropdownShadowQuality(int index)
    {
        var value = SetDropDown(m_ShadowQualityDropdown, index, _graphicSetting.ShadowQuality);
        if (value == null) return;

        _graphicSetting.ShadowQuality = (ShadowQuality)value;
    }

    public void DropdownQualityLevel(int index)
    {
        var value = SetDropDown(m_QualityLevelDropdown, index, _graphicSetting.QualityLevel);
        if (value == null) return;

        _graphicSetting.QualityLevel = (QualityLevelEnum)value;
    }

    public void DropdownShaderLodLevel(int index)
    {
        var value = SetDropDown(m_ShaderLodLevelropdown, index, _graphicSetting.ShaderLodLevel);
        if (value == null) return;

        _graphicSetting.ShaderLodLevel = (ShaderLodLevelEnum)value;
    }

    public void SetFog(bool value)
    {
        _graphicSetting.HasFog = value ? 1 : 0;
    }

    public void SetPostProcess(bool value)
    {
        _graphicSetting.HasPost = value ? 1 : 0;
    }
    
    public void SetWaterNormal(bool value)
    {
        _graphicSetting.HasWaterNormal = value ? 1 : 0;
    }

    public void SetAntiAliasing(bool value) {
        _graphicSetting.HasAntiAliasing = value ? 1 : 0;
    }

    public void SetVSync(bool value)
    {
        _graphicSetting.Vsync = value ? 1 : 0;
    }

    protected override void OnCreate()
    {
        IsBlockControl = true;
    }

	protected override void handleGamepadButtons()
	{
		//only allow close setting for now, so at least this UI can be closed via gamepad
        if (gameObject.activeSelf)
		{
            if(GamepadHelper.IsConfirm() 
                ||GamepadHelper.IsCancel())
			{
                Close();
			}
		}
	}
}
