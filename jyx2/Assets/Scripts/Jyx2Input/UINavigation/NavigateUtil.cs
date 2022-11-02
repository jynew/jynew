using System;
using System.Collections.Generic;
using UnityEngine;

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


        public static void SetUpNavigation<T>(this IList<T> Items, int row, int col) where T : INavigable
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                var curItem = Items[i];
                var neibor = GetNeighbors(i, row, col);
                curItem.Connect(Items.SafeGet(neibor.up), Items.SafeGet(neibor.down), Items.SafeGet(neibor.left), Items.SafeGet(neibor.right));
            }
        }
    }
}
