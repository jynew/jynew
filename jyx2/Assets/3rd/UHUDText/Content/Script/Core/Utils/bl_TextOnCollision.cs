using UnityEngine;
using System.Collections;

public class bl_TextOnCollision : MonoBehaviour {

    public bl_HUDText m_HudText;

	void OnCollisionEnter(Collision c)
    {
        if (c.transform == null)
            return;
        if(m_HudText == null)
        {
            Debug.LogError("Please assign the HudText in inspector.");
            return;
        }
        m_HudText.NewText("Message to Show", c.transform);
        Destroy(gameObject);
    }
}