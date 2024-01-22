using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Jyx2.UINavigation
{
    public enum NavigationDirection
    {
        Left = 1,
        Right = 2,
        Up = 3,
        Down = 4,
    }

    public interface INavigable
    {
        void Connect(INavigable up = null, INavigable down = null, INavigable left = null, INavigable right = null);

        Selectable GetSelectable();

        void Select(bool notifyEvent);
    }
}
