using DG.Tweening;
using Jyx2.InputCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

namespace Jyx2.UINavigation
{
    public static class NavigateUtil
    {
        public static T SafeGet<T>(this IList<T> source, int idx, bool logError = false)
        {
            if (source == null)
            {
                if (logError)
                    Debug.LogError("Error, source list is null");
                return default(T);
            }
            if (idx < 0 || idx >= source.Count)
            {
                if (logError)
                    Debug.LogError("Error, idx is out of range");
                return default(T);
            }
            return source[idx];
        }

        public static (int up, int down, int left, int right) GetNeighbors(int idx, int row, int col)
        {
            int u, d, l, r;
            u = d = l = r = -1;
            if (row < 1 || col < 1)
            {
                Debug.LogErrorFormat("Invalid row[{0}], col[{1}]", row, col);
                return (u, d, l, r);
            }
            if (idx >= 0 && idx < row * col)
            {
                l = IsEdge(idx, row, col, NavigationDirection.Left) ? -1 : idx - 1;
                r = IsEdge(idx, row, col, NavigationDirection.Right) ? -1 : idx + 1;
                u = IsEdge(idx, row, col, NavigationDirection.Up) ? -1 : idx - col;
                d = IsEdge(idx, row, col, NavigationDirection.Down) ? -1 : idx + col;
            }
            return (u, d, l, r);
        }

        private static bool IsEdge(int idx, int row, int col, NavigationDirection direction)
        {
            if (idx < 0 || idx > row * col)
                return true;
            if (row < 1 || col < 1)
                return true;
            bool ret = false;
            var col_idx = idx % col;
            var row_idx = idx / col;
            switch (direction)
            {
                case NavigationDirection.Left:
                    {
                        ret = col_idx == 0;
                        break;
                    }
                case NavigationDirection.Right:
                    {
                        ret = col_idx == col - 1;
                        break;
                    }
                case NavigationDirection.Up:
                    {
                        ret = row_idx == 0;
                        break;
                    }
                case NavigationDirection.Down:
                    {
                        ret = row_idx == row - 1;
                        break;
                    }
            }
            return ret;
        }

        public static bool IsNavigateInputLastFrame()
        {
            var controller = Jyx2_Input.GetLastActiveController();
            if (controller == null)
                return false;
            if (!controller.isConnected)
                return false;
            if (controller.type == Rewired.ControllerType.Mouse)
                return false;
            return true;
        }

        public static void SetUpNavigation<T>(this IList<T> Items, int row, int col) where T : INavigable
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                var curItem = Items[i];
                var neibor = GetNeighbors(i, row, col);
                curItem.Connect(Items.SafeGet(neibor.up), Items.SafeGet(neibor.down), Items.SafeGet(neibor.left), Items.SafeGet(neibor.right));
            }
        }

        public static void SetUpNavigation(this IList<Selectable> Items, int row, int col)
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                var curItem = Items[i];
                var neibor = GetNeighbors(i, row, col);
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnLeft = Items.SafeGet(neibor.left);
                navigation.selectOnRight = Items.SafeGet(neibor.right);
                navigation.selectOnUp = Items.SafeGet(neibor.up);
                navigation.selectOnDown = Items.SafeGet(neibor.down);
                curItem.navigation = navigation;
            }
        }

        static Vector3[] corners = new Vector3[4];
        public static Bounds TransformBoundsTo(this RectTransform source, Transform target)
        {
            var bounds = new Bounds();
            if (source != null)
            {
                source.GetWorldCorners(corners);

                var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                var matrix = target.worldToLocalMatrix;
                for (int j = 0; j < 4; j++)
                {
                    Vector3 v = matrix.MultiplyPoint3x4(corners[j]);
                    vMin = Vector3.Min(v, vMin);
                    vMax = Vector3.Max(v, vMax);
                }
                bounds = new Bounds(vMin, Vector3.zero);
                bounds.Encapsulate(vMax);
            }
            return bounds;
        }

        private static float NormalizeScrollDistance(this ScrollRect scrollRect, int axis, float distance)
        {
            var viewport = scrollRect.viewport;
            var viewRect = viewport != null ? viewport : scrollRect.GetComponent<RectTransform>();
            var viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);

            var content = scrollRect.content;
            var contentBounds = content != null ? content.TransformBoundsTo(viewRect) : new Bounds();

            var hiddenLength = contentBounds.size[axis] - viewBounds.size[axis];
            return distance / hiddenLength;
        }

        public static void ScrollToTarget(this ScrollRect scrollRect, RectTransform target)
        {
            var view = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();

            var viewRect = view.rect;
            var elementBounds = target.TransformBoundsTo(view);
            var result = scrollRect.normalizedPosition;
            if (scrollRect.horizontal)
            {
                var offsetX = viewRect.center.x - elementBounds.center.x;
                var scrollPosX = scrollRect.horizontalNormalizedPosition - scrollRect.NormalizeScrollDistance(0, offsetX);
                result.x = Mathf.Clamp01(scrollPosX);
            }
            if (scrollRect.vertical)
            {
                var offsetY = viewRect.center.y - elementBounds.center.y;
                var scrollPosY = scrollRect.verticalNormalizedPosition - scrollRect.NormalizeScrollDistance(1, offsetY);
                result.y = Mathf.Clamp01(scrollPosY);
            }
            scrollRect.DONormalizedPos(result, 0.1f);
        }

        public static void TryFocusInScrollRect(Component comp)
        {
            var rectTranform = comp.GetComponent<RectTransform>();
            var scrollRect = comp.GetComponentInParent<ScrollRect>();
            if (rectTranform == null || scrollRect == null)
                return;
            scrollRect.ScrollToTarget(rectTranform);
        }
    }
}
