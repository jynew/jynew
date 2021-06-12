using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullSuggestPanel : MonoBehaviour {

    public Text suggestText;

    System.Action _callback;

    public void Show(string msg, System.Action callback)
    {
        Time.timeScale = 0;
        suggestText.text = msg;
        _callback = callback;
        gameObject.SetActive(true);
    }

    public void OnClicked()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        if (_callback != null)
            _callback();
    }
    
}
