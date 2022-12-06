using MOD.UI;
using System;
using UnityEngine;

namespace Jyx2
{
    public class ModPanelLauncher : MonoBehaviour
    { 
        private async void Start()
        {
            await Jyx2_UIManager.Instance.ShowUIAsync<ModPanelNew>();
        }
    }
}