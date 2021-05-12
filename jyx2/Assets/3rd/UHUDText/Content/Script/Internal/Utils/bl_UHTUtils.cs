using UnityEngine;

public static class bl_UHTUtils  {

    private static  bl_HUDText m_HudText = null;
    public static bl_HUDText GetHUDText
    {
        get
        {
            if(m_HudText == null)
            {
                m_HudText = GameObject.FindObjectOfType<bl_HUDText>();
            }
            return m_HudText;
        }
        set
        {
            m_HudText = value;
        }
    }
}