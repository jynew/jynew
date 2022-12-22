// -----------------------------------------------------------------------
// <copyright file="ScrollViewEx.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace TapSDK.UI.AillieoTech
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class ScrollViewEx : ScrollView
    {
        [SerializeField]
        [FormerlySerializedAs("m_pageSize")]
        private int pageSize = 50;

        private int startOffset = 0;

        private Func<int> realItemCountFunc;

        private Vector2 lastPosition;

        private bool reloadFlag = false;

        public override void SetUpdateFunc(Action<int, RectTransform> func)
        {
            if (func != null)
            {
                var f = func;
                func = (index, rect) =>
                {
                    f(index + this.startOffset, rect);
                };
            }

            base.SetUpdateFunc(func);
        }

        public override void SetItemSizeFunc(Func<int, Vector2> func)
        {
            if (func != null)
            {
                var f = func;
                func = (index) =>
                {
                    return f(index + this.startOffset);
                };
            }

            base.SetItemSizeFunc(func);
        }

        public override void SetItemCountFunc(Func<int> func)
        {
            this.realItemCountFunc = func;
            if (func != null)
            {
                var f = func;
                func = () => Mathf.Min(f(), this.pageSize);
            }

            base.SetItemCountFunc(func);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (this.reloadFlag)
            {
                this.reloadFlag = false;
                this.OnEndDrag(eventData);
                this.OnBeginDrag(eventData);

                return;
            }

            base.OnDrag(eventData);
        }

        protected override void Awake()
        {
            base.Awake();

            this.lastPosition = Vector2.up;
            this.onValueChanged.AddListener(this.OnValueChanged);
        }

        protected override void InternalScrollTo(int index)
        {
            var count = 0;
            if (this.realItemCountFunc != null)
            {
                count = this.realItemCountFunc();
            }

            index = Mathf.Clamp(index, 0, count - 1);
            this.startOffset = Mathf.Clamp(index - (this.pageSize / 2), 0, count - this.itemCountFunc());
            this.UpdateData(true);
            base.InternalScrollTo(index - this.startOffset);
        }

        private void OnValueChanged(Vector2 position)
        {
            int toShow;
            int critical;
            bool downward;
            int pin;

            Vector2 delta = position - this.lastPosition;
            this.lastPosition = position;

            this.reloadFlag = false;

            if (((int)this.layoutType & flagScrollDirection) == 1)
            {
                // 垂直滚动 只计算y向
                if (delta.y < 0)
                {
                    // 向上
                    toShow = this.criticalItemIndex[CriticalItemType.DownToShow];
                    critical = this.pageSize - 1;
                    if (toShow < critical)
                    {
                        return;
                    }

                    pin = critical - 1;
                    downward = false;
                }
                else if (delta.y > 0)
                {
                    // 向下
                    toShow = this.criticalItemIndex[CriticalItemType.UpToShow];
                    critical = 0;
                    if (toShow > critical)
                    {
                        return;
                    }

                    pin = critical + 1;
                    downward = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                // = 0
                // 水平滚动 只计算x向
                if (delta.x > 0)
                {
                    // 向右
                    toShow = this.criticalItemIndex[CriticalItemType.UpToShow];
                    critical = 0;
                    if (toShow > critical)
                    {
                        return;
                    }

                    pin = critical + 1;
                    downward = true;
                }
                else if (delta.x < 0)
                {
                    // 向左
                    toShow = this.criticalItemIndex[CriticalItemType.DownToShow];
                    critical = this.pageSize - 1;
                    if (toShow < critical)
                    {
                        return;
                    }

                    pin = critical - 1;
                    downward = false;
                }
                else
                {
                    return;
                }
            }

            // 该翻页了 翻半页吧
            var old = this.startOffset;
            if (downward)
            {
                this.startOffset -= this.pageSize / 2;
            }
            else
            {
                this.startOffset += this.pageSize / 2;
            }

            var realDataCount = 0;
            if (this.realItemCountFunc != null)
            {
                realDataCount = this.realItemCountFunc();
            }

            this.startOffset = Mathf.Clamp(this.startOffset, 0, Mathf.Max(realDataCount - this.pageSize, 0));

            if (old != this.startOffset)
            {
                this.reloadFlag = true;

                // 记录 原先的速度
                Vector2 oldVelocity = this.velocity;

                // 计算 pin元素的世界坐标
                Rect rect = this.GetItemLocalRect(pin);

                Vector2 oldWorld = this.content.TransformPoint(rect.position);
                var dataCount = 0;
                if (this.itemCountFunc != null)
                {
                    dataCount = this.itemCountFunc();
                }

                if (dataCount > 0)
                {
                    this.EnsureItemRect(0);
                    if (dataCount > 1)
                    {
                        this.EnsureItemRect(dataCount - 1);
                    }
                }

                // 根据 pin元素的世界坐标 计算出content的position
                var pin2 = pin + old - this.startOffset;
                Rect rect2 = this.GetItemLocalRect(pin2);
                Vector2 newWorld = this.content.TransformPoint(rect2.position);
                Vector2 deltaWorld = newWorld - oldWorld;
                Vector2 deltaLocal = this.content.InverseTransformVector(deltaWorld);
                this.SetContentAnchoredPosition(this.content.anchoredPosition - deltaLocal);
                this.UpdateData(true);
                this.velocity = oldVelocity;
            }
        }
    }
}
