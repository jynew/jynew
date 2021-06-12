using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
        if (PlayerPrefs.HasKey(m_fogkey))
        {
            m_bFog = PlayerPrefs.GetInt(m_fogkey) == 1;
        }
        else
        {
            m_bFog = RenderSettings.fog;
            PlayerPrefs.SetInt(m_fogkey, m_bFog ? 1 : 0);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey(m_postkey))
        {
            m_bPost = PlayerPrefs.GetInt(m_postkey) == 1;
        }
        else
        {
            var post = m_MainCamera.GetComponent<PostProcessLayer>();
            m_bPost = post.enabled;
            PlayerPrefs.SetInt(m_postkey, m_bPost ? 1 : 0);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey(m_qualitykey))
        {
            m_iQuality = PlayerPrefs.GetInt(m_qualitykey);
        }
        else
        {
            m_iQuality = QualitySettings.GetQualityLevel();
            PlayerPrefs.SetInt(m_qualitykey, m_iQuality);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey(m_shaderLODKey))
        {
            m_iShaderLOD = PlayerPrefs.GetInt(m_shaderLODKey);
        }
        else
        {
            m_iShaderLOD = Shader.globalMaximumLOD;
            PlayerPrefs.SetInt(m_shaderLODKey, m_iShaderLOD);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey(m_maxFrameRateKey))
        {
            m_iMaxFrameRate = PlayerPrefs.GetInt(m_maxFrameRateKey);
        }
        else
        {
            m_iMaxFrameRate = Application.targetFrameRate;
            PlayerPrefs.SetInt(m_maxFrameRateKey, m_iMaxFrameRate);
            PlayerPrefs.Save();
        }

        DoSettings();
    }

    public void DoSettings()
    {
        RenderSettings.fog = m_bFog;
        var post = m_MainCamera.GetComponent<PostProcessLayer>();
        if(post != null) post.enabled = m_bPost;
        QualitySettings.SetQualityLevel(m_iQuality, true);
        Shader.globalMaximumLOD = m_iShaderLOD;
        Application.targetFrameRate = m_iMaxFrameRate;
    }
}
