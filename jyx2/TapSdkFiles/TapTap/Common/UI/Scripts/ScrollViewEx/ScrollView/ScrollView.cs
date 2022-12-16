// -----------------------------------------------------------------------
// <copyright file="ScrollView.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace TapSDK.UI.AillieoTech
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class ScrollView : ScrollRect
    {
        [Tooltip("默认item尺寸")]
        public Vector2 defaultItemSize;

        [Tooltip("item的模板")]
        public RectTransform itemTemplate;

        // 0001
        protected const int flagScrollDirection = 1;

        [SerializeField]
        [FormerlySerializedAs("m_layoutType")]
        protected ItemLayoutType layoutType = ItemLayoutType.Vertical;

        // 只保存4个临界index
        protected int[] criticalItemIndex = new int[4];

        // callbacks for items
        protected Action<int, RectTransform> updateFunc;
        protected Func<int, Vector2> itemSizeFunc;
        protected Func<int> itemCountFunc;
        protected Func<int, RectTransform> itemGetFunc;
        protected Action<RectTransform> itemRecycleFunc;

        private readonly List<ScrollItemWithRect> managedItems = new List<ScrollItemWithRect>();

        private Rect refRect;

        // resource management
        private SimpleObjPool<RectTransform> itemPool = null;

        private int dataCount = 0;

        [Tooltip("初始化时池内item数量")]
        [SerializeField]
        private int poolSize;

        // status
        private bool initialized = false;
        private int willUpdateData = 0;

        private Vector3[] viewWorldConers = new Vector3[4];
        private Vector3[] rectCorners = new Vector3[2];

        // for hide and show
        public enum ItemLayoutType
        {
            // 最后一位表示滚动方向
            Vertical = 0b0001,                   // 0001
            Horizontal = 0b0010,                 // 0010
            VerticalThenHorizontal = 0b0100,     // 0100
            HorizontalThenVertical = 0b0101,     // 0101
        }

        public virtual void SetUpdateFunc(Action<int, RectTransform> func)
        {
            this.updateFunc = func;
        }

        public virtual void SetItemSizeFunc(Func<int, Vector2> func)
        {
            this.itemSizeFunc = func;
        }

        public virtual void SetItemCountFunc(Func<int> func)
        {
            this.itemCountFunc = func;
        }

        public void SetItemGetAndRecycleFunc(Func<int, RectTransform> getFunc, Action<RectTransform> recycleFunc)
        {
            if (getFunc != null && recycleFunc != null)
            {
                this.itemGetFunc = getFunc;
                this.itemRecycleFunc = recycleFunc;
            }
            else
            {
                this.itemGetFunc = null;
                this.itemRecycleFunc = null;
            }
        }

        public void ResetAllDelegates()
        {
            this.SetUpdateFunc(null);
            this.SetItemSizeFunc(null);
            this.SetItemCountFunc(null);
            this.SetItemGetAndRecycleFunc(null, null);
        }

        public void UpdateData(bool immediately = true)
        {
            if (immediately)
            {
                this.willUpdateData |= 3; // 0011
                this.InternalUpdateData();
            }
            else
            {
                if (this.willUpdateData == 0 && this.IsActive())
                {
                    this.StartCoroutine(this.DelayUpdateData());
                }

                this.willUpdateData |= 3;
            }
        }

        public void UpdateDataIncrementally(bool immediately = true)
        {
            if (immediately)
            {
                this.willUpdateData |= 1; // 0001
                this.InternalUpdateData();
            }
            else
            {
                if (this.willUpdateData == 0)
                {
                    this.StartCoroutine(this.DelayUpdateData());
                }

                this.willUpdateData |= 1;
            }
        }

        public void ScrollTo(int index)
        {
            this.InternalScrollTo(index);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.willUpdateData != 0)
            {
                this.StartCoroutine(this.DelayUpdateData());
            }
        }

        protected override void OnDisable()
        {
            this.initialized = false;
            base.OnDisable();
        }

        protected virtual void InternalScrollTo(int index)
        {
            index = Mathf.Clamp(index, 0, this.dataCount - 1);
            this.EnsureItemRect(index);
            Rect r = this.managedItems[index].rect;
            
            var dir = (int)this.layoutType & flagScrollDirection;
            if (dir == 1)
            {
                // vertical
                var value = 1 - (-r.yMax / (this.content.sizeDelta.y - this.refRect.height));
                this.SetNormalizedPosition(value, 1);
            }
            else
            {
                // horizontal
                var value = r.xMin / (this.content.sizeDelta.x - this.refRect.width);
                this.SetNormalizedPosition(value, 0);
            }
        }

        protected override void SetContentAnchoredPosition(Vector2 position)
        {
            base.SetContentAnchoredPosition(position);
            this.UpdateCriticalItems();
        }

        protected override void SetNormalizedPosition(float value, int axis)
        {
            base.SetNormalizedPosition(value, axis);
            this.ResetCriticalItems();
        }

        protected void EnsureItemRect(int index)
        {
            if (!this.managedItems[index].rectDirty)
            {
                // 已经是干净的了
                return;
            }

            ScrollItemWithRect firstItem = this.managedItems[0];
            if (firstItem.rectDirty)
            {
                Vector2 firstSize = this.GetItemSize(0);
                firstItem.rect = CreateWithLeftTopAndSize(Vector2.zero, firstSize);
                firstItem.rectDirty = false;
            }

            // 当前item之前的最近的已更新的rect
            var nearestClean = 0;
            for (var i = index; i >= 0; --i)
            {
                if (!this.managedItems[i].rectDirty)
                {
                    nearestClean = i;
                    break;
                }
            }

            // 需要更新 从 nearestClean 到 index 的尺寸
            Rect nearestCleanRect = this.managedItems[nearestClean].rect;
            Vector2 curPos = GetLeftTop(nearestCleanRect);
            Vector2 size = nearestCleanRect.size;
            this.MovePos(ref curPos, size);

            for (var i = nearestClean + 1; i <= index; i++)
            {
                size = this.GetItemSize(i);
                this.managedItems[i].rect = CreateWithLeftTopAndSize(curPos, size);
                this.managedItems[i].rectDirty = false;
                this.MovePos(ref curPos, size);
            }

            var range = new Vector2(Mathf.Abs(curPos.x), Mathf.Abs(curPos.y));
            switch (this.layoutType)
            {
                case ItemLayoutType.VerticalThenHorizontal:
                    range.x += size.x;
                    range.y = this.refRect.height;
                    break;
                case ItemLayoutType.HorizontalThenVertical:
                    range.x = this.refRect.width;
                    if (curPos.x != 0)
                    {
                        range.y += size.y;
                    }

                    break;
                default:
                    break;
            }

            this.content.sizeDelta = range;
        }

        protected override void OnDestroy()
        {
            if (this.itemPool != null)
            {
                this.itemPool.Purge();
            }
        }

        protected Rect GetItemLocalRect(int index)
        {
            if (index >= 0 && index < this.dataCount)
            {
                this.EnsureItemRect(index);
                return this.managedItems[index].rect;
            }

            return (Rect)default;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            var dir = (int)this.layoutType & flagScrollDirection;
            if (dir == 1)
            {
                // vertical
                if (this.horizontalScrollbar != null)
                {
                    this.horizontalScrollbar.gameObject.SetActive(false);
                    this.horizontalScrollbar = null;
                }
            }
            else
            {
                // horizontal
                if (this.verticalScrollbar != null)
                {
                    this.verticalScrollbar.gameObject.SetActive(false);
                    this.verticalScrollbar = null;
                }
            }

            base.OnValidate();
        }
#endif

        private static Vector2 GetLeftTop(Rect rect)
        {
            Vector2 ret = rect.position;
            ret.y += rect.size.y;
            return ret;
        }

        private static Rect CreateWithLeftTopAndSize(Vector2 leftTop, Vector2 size)
        {
            Vector2 leftBottom = leftTop - new Vector2(0, size.y);
            return new Rect(leftBottom, size);
        }

        private IEnumerator DelayUpdateData()
        {
            yield return new WaitForEndOfFrame();
            this.InternalUpdateData();
        }

        private void InternalUpdateData()
        {
            if (!this.IsActive())
            {
                this.willUpdateData |= 3;
                return;
            }

            if (!this.initialized)
            {
                this.InitScrollView();
            }

            var newDataCount = 0;
            var keepOldItems = (this.willUpdateData & 2) == 0;

            if (this.itemCountFunc != null)
            {
                newDataCount = this.itemCountFunc();
            }

            if (newDataCount != this.managedItems.Count)
            {
                if (this.managedItems.Count < newDataCount)
                {
                    // 增加
                    if (!keepOldItems)
                    {
                        foreach (var itemWithRect in this.managedItems)
                        {
                            // 重置所有rect
                            itemWithRect.rectDirty = true;
                        }
                    }

                    while (this.managedItems.Count < newDataCount)
                    {
                        this.managedItems.Add(new ScrollItemWithRect());
                    }
                }
                else
                {
                    // 减少 保留空位 避免GC
                    for (int i = 0, count = this.managedItems.Count; i < count; ++i)
                    {
                        if (i < newDataCount)
                        {
                            // 重置所有rect
                            if (!keepOldItems)
                            {
                                this.managedItems[i].rectDirty = true;
                            }

                            if (i == newDataCount - 1)
                            {
                                this.managedItems[i].rectDirty = true;
                            }
                        }

                        // 超出部分 清理回收item
                        if (i >= newDataCount)
                        {
                            this.managedItems[i].rectDirty = true;
                            if (this.managedItems[i].item != null)
                            {
                                this.RecycleOldItem(this.managedItems[i].item);
                                this.managedItems[i].item = null;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!keepOldItems)
                {
                    for (int i = 0, count = this.managedItems.Count; i < count; ++i)
                    {
                        // 重置所有rect
                        this.managedItems[i].rectDirty = true;
                    }
                }
            }

            this.dataCount = newDataCount;

            this.ResetCriticalItems();

            this.willUpdateData = 0;
        }

        private void ResetCriticalItems()
        {
            bool hasItem, shouldShow;
            int firstIndex = -1, lastIndex = -1;

            for (var i = 0; i < this.dataCount; i++)
            {
                hasItem = this.managedItems[i].item != null;
                shouldShow = this.ShouldItemSeenAtIndex(i);

                if (shouldShow)
                {
                    if (firstIndex == -1)
                    {
                        firstIndex = i;
                    }

                    lastIndex = i;
                }

                if (hasItem && shouldShow)
                {
                    // 应显示且已显示
                    this.SetDataForItemAtIndex(this.managedItems[i].item, i);
                    continue;
                }

                if (hasItem == shouldShow)
                {
                    // 不应显示且未显示
                    // if (firstIndex != -1)
                    // {
                    //     // 已经遍历完所有要显示的了 后边的先跳过
                    //     break;
                    // }
                    continue;
                }

                if (hasItem && !shouldShow)
                {
                    // 不该显示 但是有
                    this.RecycleOldItem(this.managedItems[i].item);
                    this.managedItems[i].item = null;
                    continue;
                }

                if (shouldShow && !hasItem)
                {
                    // 需要显示 但是没有
                    RectTransform item = this.GetNewItem(i);
                    this.OnGetItemForDataIndex(item, i);
                    this.managedItems[i].item = item;
                    continue;
                }
            }

            // content.localPosition = Vector2.zero;
            this.criticalItemIndex[CriticalItemType.UpToHide] = firstIndex;
            this.criticalItemIndex[CriticalItemType.DownToHide] = lastIndex;
            this.criticalItemIndex[CriticalItemType.UpToShow] = Mathf.Max(firstIndex - 1, 0);
            this.criticalItemIndex[CriticalItemType.DownToShow] = Mathf.Min(lastIndex + 1, this.dataCount - 1);
        }

        private RectTransform GetCriticalItem(int type)
        {
            var index = this.criticalItemIndex[type];
            if (index >= 0 && index < this.dataCount)
            {
                return this.managedItems[index].item;
            }

            return null;
        }

        private void UpdateCriticalItems()
        {
            var dirty = true;

            while (dirty)
            {
                dirty = false;

                for (int i = CriticalItemType.UpToHide; i <= CriticalItemType.DownToShow; i++)
                {
                    if (i <= CriticalItemType.DownToHide)
                    {
                        // 隐藏离开可见区域的item
                        dirty = dirty || this.CheckAndHideItem(i);
                    }
                    else
                    {
                        // 显示进入可见区域的item
                        dirty = dirty || this.CheckAndShowItem(i);
                    }
                }
            }
        }

        private bool CheckAndHideItem(int criticalItemType)
        {
            RectTransform item = this.GetCriticalItem(criticalItemType);
            var criticalIndex = this.criticalItemIndex[criticalItemType];
            if (item != null && !this.ShouldItemSeenAtIndex(criticalIndex))
            {
                this.RecycleOldItem(item);
                this.managedItems[criticalIndex].item = null;

                if (criticalItemType == CriticalItemType.UpToHide)
                {
                    // 最上隐藏了一个
                    this.criticalItemIndex[criticalItemType + 2] = Mathf.Max(criticalIndex, this.criticalItemIndex[criticalItemType + 2]);
                    this.criticalItemIndex[criticalItemType]++;
                }
                else
                {
                    // 最下隐藏了一个
                    this.criticalItemIndex[criticalItemType + 2] = Mathf.Min(criticalIndex, this.criticalItemIndex[criticalItemType + 2]);
                    this.criticalItemIndex[criticalItemType]--;
                }

                this.criticalItemIndex[criticalItemType] = Mathf.Clamp(this.criticalItemIndex[criticalItemType], 0, this.dataCount - 1);

                if (this.criticalItemIndex[CriticalItemType.UpToHide] > this.criticalItemIndex[CriticalItemType.DownToHide])
                {
                    // 偶然的情况 拖拽超出一屏
                    this.ResetCriticalItems();
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool CheckAndShowItem(int criticalItemType)
        {
            RectTransform item = this.GetCriticalItem(criticalItemType);
            var criticalIndex = this.criticalItemIndex[criticalItemType];

            if (item == null && this.ShouldItemSeenAtIndex(criticalIndex))
            {
                RectTransform newItem = this.GetNewItem(criticalIndex);
                this.OnGetItemForDataIndex(newItem, criticalIndex);
                this.managedItems[criticalIndex].item = newItem;

                if (criticalItemType == CriticalItemType.UpToShow)
                {
                    // 最上显示了一个
                    this.criticalItemIndex[criticalItemType - 2] = Mathf.Min(criticalIndex, this.criticalItemIndex[criticalItemType - 2]);
                    this.criticalItemIndex[criticalItemType]--;
                }
                else
                {
                    // 最下显示了一个
                    this.criticalItemIndex[criticalItemType - 2] = Mathf.Max(criticalIndex, this.criticalItemIndex[criticalItemType - 2]);
                    this.criticalItemIndex[criticalItemType]++;
                }

                this.criticalItemIndex[criticalItemType] = Mathf.Clamp(this.criticalItemIndex[criticalItemType], 0, this.dataCount - 1);

                if (this.criticalItemIndex[CriticalItemType.UpToShow] >= this.criticalItemIndex[CriticalItemType.DownToShow])
                {
                    // 偶然的情况 拖拽超出一屏
                    this.ResetCriticalItems();
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool ShouldItemSeenAtIndex(int index)
        {
            if (index < 0 || index >= this.dataCount)
            {
                return false;
            }

            this.EnsureItemRect(index);
            return new Rect(this.refRect.position - this.content.anchoredPosition, this.refRect.size).Overlaps(this.managedItems[index].rect);
        }

        private bool ShouldItemFullySeenAtIndex(int index)
        {
            if (index < 0 || index >= this.dataCount)
            {
                return false;
            }

            this.EnsureItemRect(index);
            return this.IsRectContains(new Rect(this.refRect.position - this.content.anchoredPosition, this.refRect.size), this.managedItems[index].rect);
        }

        private bool IsRectContains(Rect outRect, Rect inRect, bool bothDimensions = false)
        {
            if (bothDimensions)
            {
                var xContains = (outRect.xMax >= inRect.xMax) && (outRect.xMin <= inRect.xMin);
                var yContains = (outRect.yMax >= inRect.yMax) && (outRect.yMin <= inRect.yMin);
                return xContains && yContains;
            }
            else
            {
                var dir = (int)this.layoutType & flagScrollDirection;
                if (dir == 1)
                {
                    // 垂直滚动 只计算y向
                    return (outRect.yMax >= inRect.yMax) && (outRect.yMin <= inRect.yMin);
                }
                else
                {
                    // = 0
                    // 水平滚动 只计算x向
                    return (outRect.xMax >= inRect.xMax) && (outRect.xMin <= inRect.xMin);
                }
            }
        }

        private void InitPool()
        {
            var poolNode = new GameObject("POOL");
            poolNode.SetActive(false);
            poolNode.transform.SetParent(this.transform, false);
            this.itemPool = new SimpleObjPool<RectTransform>(
                this.poolSize,
                (RectTransform item) =>
                {
                    item.transform.SetParent(poolNode.transform, false);
                },
                () =>
                {
                    GameObject itemObj = Instantiate(this.itemTemplate.gameObject);
                    RectTransform item = itemObj.GetComponent<RectTransform>();
                    itemObj.transform.SetParent(poolNode.transform, false);

                    item.anchorMin = Vector2.up;
                    item.anchorMax = Vector2.up;
                    item.pivot = Vector2.zero;

                    itemObj.SetActive(true);
                    return item;
                });
        }

        private void OnGetItemForDataIndex(RectTransform item, int index)
        {
            this.SetDataForItemAtIndex(item, index);
            item.transform.SetParent(this.content, false);
        }

        private void SetDataForItemAtIndex(RectTransform item, int index)
        {
            if (this.updateFunc != null)
            {
                this.updateFunc(index, item);
            }

            this.SetPosForItemAtIndex(item, index);
        }

        private void SetPosForItemAtIndex(RectTransform item, int index)
        {
            this.EnsureItemRect(index);
            Rect r = this.managedItems[index].rect;
            item.localPosition = r.position;
            item.sizeDelta = r.size;
        }

        private Vector2 GetItemSize(int index)
        {
            if (index >= 0 && index <= this.dataCount)
            {
                if (this.itemSizeFunc != null)
                {
                    return this.itemSizeFunc(index);
                }
            }

            return this.defaultItemSize;
        }

        private RectTransform GetNewItem(int index)
        {
            RectTransform item;
            if (this.itemGetFunc != null)
            {
                item = this.itemGetFunc(index);
            }
            else
            {
                item = this.itemPool.Get();
            }

            return item;
        }

        private void RecycleOldItem(RectTransform item)
        {
            if (this.itemRecycleFunc != null)
            {
                this.itemRecycleFunc(item);
            }
            else
            {
                this.itemPool.Recycle(item);
            }
        }

        private void InitScrollView()
        {
            this.initialized = true;

            // 根据设置来控制原ScrollRect的滚动方向
            var dir = (int)this.layoutType & flagScrollDirection;
            this.vertical = dir == 1;
            this.horizontal = dir == 0;

            this.content.pivot = Vector2.up;
            this.content.anchorMin = Vector2.up;
            this.content.anchorMax = Vector2.up;
            this.content.anchoredPosition = Vector2.zero;

            this.InitPool();
            this.UpdateRefRect();
        }

        // refRect是在Content节点下的 viewport的 rect
        private void UpdateRefRect()
        {
            /*
             *  WorldCorners
             *
             *    1 ------- 2
             *    |         |
             *    |         |
             *    0 ------- 3
             *
             */

            if (!CanvasUpdateRegistry.IsRebuildingLayout())
            {
                Canvas.ForceUpdateCanvases();
            }

            this.viewRect.GetWorldCorners(this.viewWorldConers);
            this.rectCorners[0] = this.content.transform.InverseTransformPoint(this.viewWorldConers[0]);
            this.rectCorners[1] = this.content.transform.InverseTransformPoint(this.viewWorldConers[2]);
            this.refRect = new Rect((Vector2)this.rectCorners[0] - this.content.anchoredPosition, this.rectCorners[1] - this.rectCorners[0]);
        }

        private void MovePos(ref Vector2 pos, Vector2 size)
        {
            // 注意 所有的rect都是左下角为基准
            switch (this.layoutType)
            {
                case ItemLayoutType.Vertical:
                    // 垂直方向 向下移动
                    pos.y -= size.y;
                    break;
                case ItemLayoutType.Horizontal:
                    // 水平方向 向右移动
                    pos.x += size.x;
                    break;
                case ItemLayoutType.VerticalThenHorizontal:
                    pos.y -= size.y;
                    if (pos.y <= -this.refRect.height)
                    {
                        pos.y = 0;
                        pos.x += size.x;
                    }

                    break;
                case ItemLayoutType.HorizontalThenVertical:
                    pos.x += size.x;
                    if (pos.x >= this.refRect.width)
                    {
                        pos.x = 0;
                        pos.y -= size.y;
                    }

                    break;
                default:
                    break;
            }
        }

        // const int 代替 enum 减少 (int)和(CriticalItemType)转换
        protected static class CriticalItemType
        {
            public static byte UpToHide = 0;
            public static byte DownToHide = 1;
            public static byte UpToShow = 2;
            public static byte DownToShow = 3;
        }

        private class ScrollItemWithRect
        {
            // scroll item 身上的 RectTransform组件
            public RectTransform item;

            // scroll item 在scrollview中的位置
            public Rect rect;

            // rect 是否需要更新
            public bool rectDirty = true;
        }
    }
}
