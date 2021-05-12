using Jyx2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDItem : MonoBehaviour
{
    Slider hpSlider;
    RoleInstance currentRole;
    RectTransform rectTrans;
    Image fillImg;
    public void Init() 
    {
        hpSlider = transform.Find("HpSlider").GetComponent<Slider>();
        rectTrans = transform as RectTransform;
        fillImg = hpSlider.transform.Find("Fill").GetComponent<Image>();
    }

    public void BindRole(RoleInstance role) 
    {
        currentRole = role;
    }

    int preHp = -1;
    private void Update()
    {
        if (currentRole == null)
            return;
        UpdatePosition();
        if (currentRole.Hp == preHp)
            return;
        preHp = currentRole.Hp;
        hpSlider.value = (float)currentRole.Hp / currentRole.MaxHp;
        UpdateHpColor();
        if (currentRole.IsDead())
            transform.gameObject.SetActive(false);
    }

    Vector3 hpPos;
    void UpdatePosition() 
    {
        hpPos = Jyx2_UIManager.Instance.GetUICamera().WorldToScreenPoint(currentRole.View.transform.position);
        rectTrans.position = hpPos;
    }

    void UpdateHpColor()
    {
        fillImg.sprite = null;
        if (currentRole.Poison > 0) 
        {
            fillImg.color = Color.cyan;
            return;
        }
        fillImg.color = currentRole.team == 0 ? Color.green : Color.red;
    }

    private void OnDisable()
    {
        currentRole = null;
        preHp = -1;
    }
}
