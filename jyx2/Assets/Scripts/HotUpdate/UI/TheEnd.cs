using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Jyx2;
using UnityEngine.SceneManagement;

public class TheEnd : Jyx2_UIBase
{
    private Image image;
    private Text finalT;
    private Text scrollT;
    private Text firstT;
    private int phase = 0;

    private Vector3 t;
    private Vector3 i;

    // Update is called once per frame
    public override void Update()
    {
        if (phase == 1)
        {
            Vector3 p = scrollT.transform.position;
            p[1]+=1;
            if (p[1] < UnityEngine.Screen.height-t[1]) scrollT.transform.position = p;
            else phase = 2;
        }else if (phase == 2)
        {
            Vector3 p = image.transform.position;
            p[1]+=1.5f;
            if (p[1] < UnityEngine.Screen.height+i[1]) image.transform.position = p;
            else phase = 3;
        }else if (phase == 3)
        {
            if (Input.GetButton("Fire1") || Input.GetKeyDown(KeyCode.Space) ||
                (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                Jyx2_UIManager.Instance.HideUI(nameof(TheEnd));
                SceneManager.LoadScene(GameConst.DefaultMainMenuScene);
            }
        }
    }

    protected override void OnCreate()
    {
        IsBlockControl = true;
        image=transform.Find("ScrollImage/Image").GetComponent<Image>();
        finalT=transform.Find("FinalText/Text").GetComponent<Text>();
        scrollT=transform.Find("ScrollText/Text").GetComponent<Text>();
        firstT=transform.Find("MainText/Text").GetComponent<Text>();
        t = scrollT.transform.position;
        i = image.transform.position;
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);
        StartCoroutine(DoAnimation());
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
        phase = 0;
        (firstT.gameObject).SetActive(true);
        (scrollT.gameObject).SetActive(false);
        (image.gameObject).SetActive(false);
        (finalT.gameObject).SetActive(false);
        scrollT.transform.position = t;
        image.transform.position = i;
    }

    IEnumerator DoAnimation()
    {
        yield return new WaitForSeconds(2);
        phase = 1;
        (firstT.gameObject).SetActive(false);
        yield return new WaitForSeconds(2);
        (scrollT.gameObject).SetActive(true);
        while (phase == 1)
        {
            yield return new WaitForSeconds(1);
        }
        (scrollT.gameObject).SetActive(false);
        yield return new WaitForSeconds(2);
        (image.gameObject).SetActive(true);
        while (phase == 2)
        {
            yield return new WaitForSeconds(1);
        }
        (finalT.gameObject).SetActive(true);
        yield return new WaitForSeconds(1);
        phase = 3;
    }
}
