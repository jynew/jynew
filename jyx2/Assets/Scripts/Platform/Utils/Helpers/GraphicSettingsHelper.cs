/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using Jyx2;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// 处理图像设置相关功能
/// </summary>
public class GraphicSettingsHelper
{
    public Camera m_MainCamera;

    private bool m_bFog = true;
    private string m_fogkey = "config-fog";
    private bool m_bPost = true;
    private string m_postkey = "config-post";
    private int m_iQuality = 0;
    private string m_qualitykey = "config-quality";
    private int m_iShaderLOD = int.MaxValue;
    private string m_shaderLODKey = "config-shaderlod";
    private int m_iMaxFrameRate = 30;
    private string m_maxFrameRateKey = "config-maxframerate";

    public void InitSettings(Camera mainCamera)
    {
        if (mainCamera == null) return;

        m_MainCamera = mainCamera;
        if (Jyx2_PlayerPrefs.HasKey(m_fogkey))
        {
            m_bFog = Jyx2_PlayerPrefs.GetInt(m_fogkey) == 1;
        }
        else
        {
            m_bFog = RenderSettings.fog;
            Jyx2_PlayerPrefs.SetInt(m_fogkey, m_bFog ? 1 : 0);
            Jyx2_PlayerPrefs.Save();
        }

        if (Jyx2_PlayerPrefs.HasKey(m_postkey))
        {
            m_bPost = Jyx2_PlayerPrefs.GetInt(m_postkey) == 1;
        }
        else
        {
            var post = m_MainCamera.GetComponent<PostProcessLayer>();
            m_bPost = post.enabled;
            Jyx2_PlayerPrefs.SetInt(m_postkey, m_bPost ? 1 : 0);
            Jyx2_PlayerPrefs.Save();
        }

        if (Jyx2_PlayerPrefs.HasKey(m_qualitykey))
        {
            m_iQuality = Jyx2_PlayerPrefs.GetInt(m_qualitykey);
        }
        else
        {
            m_iQuality = QualitySettings.GetQualityLevel();
            Jyx2_PlayerPrefs.SetInt(m_qualitykey, m_iQuality);
            Jyx2_PlayerPrefs.Save();
        }

        if (Jyx2_PlayerPrefs.HasKey(m_shaderLODKey))
        {
            m_iShaderLOD = Jyx2_PlayerPrefs.GetInt(m_shaderLODKey);
        }
        else
        {
            m_iShaderLOD = Shader.globalMaximumLOD;
            Jyx2_PlayerPrefs.SetInt(m_shaderLODKey, m_iShaderLOD);
            Jyx2_PlayerPrefs.Save();
        }

        if (Jyx2_PlayerPrefs.HasKey(m_maxFrameRateKey))
        {
            m_iMaxFrameRate = Jyx2_PlayerPrefs.GetInt(m_maxFrameRateKey);
        }
        else
        {
            m_iMaxFrameRate = Application.targetFrameRate;
            Jyx2_PlayerPrefs.SetInt(m_maxFrameRateKey, m_iMaxFrameRate);
            Jyx2_PlayerPrefs.Save();
        }

        DoSettings();
    }

    public void DoSettings()
    {
        RenderSettings.fog = m_bFog;
        var post = m_MainCamera.GetComponent<PostProcessLayer>();
        if (post != null) post.enabled = m_bPost;
        QualitySettings.SetQualityLevel(m_iQuality, true);
        Shader.globalMaximumLOD = m_iShaderLOD;
        Application.targetFrameRate = m_iMaxFrameRate;
    }
}