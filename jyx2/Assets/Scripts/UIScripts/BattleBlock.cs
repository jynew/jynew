/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using System;

public enum BattleBlockStatus
{
    Hide,
    AttackRange,
    AttackDistance,
    AttackTarget,
    CurrentAttackRange,
    MoveTarget,
    MoveRange,
    MoveRangeEnemy
}

public enum BlockActionType
{
    Enter,
    Exit,
    Action,
    LongPress
}

public class BattleBlock : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IDragHandler
{
    //public const float BASIC_SCALE = 1.25f;
    public int X;
    public int Y;

    public static int CurrentX;
    public static int CurrentY;
    public static bool ShowAll;

    public static BattleBlock currentBlock;

    public Image Image;
    public GameObject Animator;
    public GameObject Enemy;
    public Sprite attackRangeSp;
    public Sprite attackDistanceSp;
    public Sprite attackTargetSp;
    public Sprite currentAttackRangeSp;
    public Sprite moveTargetSp;
    public Sprite moveRangeSp;
    public Sprite moveRangeEnemySp;
    public Sprite noneSp;

    [HideInInspector]
    public Action<int, int> callback;

    public void OnPointerDown(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Left)
        {
            if (IsActive)
            {
                currentBlock = this;
            }
        }
        LongPress();
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (currentBlock != null && IsActive)
        {

            currentBlock = null;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (currentBlock != null && IsActive)
        {
            currentBlock = null;
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {

    }

    private void LongPress()
    {

    }

    public void OnPointerExit(PointerEventData data)
    {
        if (currentBlock != null && IsActive)
        {
            currentBlock = null;
        }
    }

    public void Reset()
    {
        IsActive = false;
        Animator.SetActive(false);
        Enemy.SetActive(false);
        Status = BattleBlockStatus.Hide;
        transform.localScale = Vector3.one;
        callback = null;
    }

    private void ChangeSprite(Image img, Sprite sprite)
    {
        img.sprite = sprite;
        img.SetNativeSize();
    }

    public void MarkEnemy()
    {
        Enemy.SetActive(true);
    }

    public void Hightlight()
    {
        Animator.SetActive(true);
    }

    public BattleBlockStatus Status
    {
        set
        {
            _status = value;

            switch (_status)
            {
                case BattleBlockStatus.Hide:
                    ChangeSprite(Image, noneSp);
                    this.gameObject.SetActive(ShowAll);
                    break;
                case BattleBlockStatus.AttackRange:
                    ChangeSprite(Image, attackRangeSp);
                    this.gameObject.SetActive(true);
                    break;
                case BattleBlockStatus.AttackDistance:
                    ChangeSprite(Image, attackDistanceSp);
                    this.gameObject.SetActive(true);
                    break;
                case BattleBlockStatus.AttackTarget:
                    ChangeSprite(Image, attackTargetSp);
                    this.gameObject.SetActive(true);
                    break;
                case BattleBlockStatus.CurrentAttackRange:
                    ChangeSprite(Image, currentAttackRangeSp);
                    this.gameObject.SetActive(true);
                    break;
                case BattleBlockStatus.MoveTarget:
                    ChangeSprite(Image, moveTargetSp);
                    this.gameObject.SetActive(true);
                    break;
                case BattleBlockStatus.MoveRange:
                    ChangeSprite(Image, moveRangeSp);
                    this.gameObject.SetActive(true);
                    break;
                case BattleBlockStatus.MoveRangeEnemy:
                    ChangeSprite(Image, moveRangeEnemySp);
                    this.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
        get
        {
            return _status;
        }
    }
    BattleBlockStatus _status;

    public bool IsActive = true;
}