using Jyx2.ResourceManagement;
using MOD.UI;
using System;
using UnityEngine;

namespace Jyx2
{
    public class ModPanelLauncher : MonoBehaviour
    { 
        private async void Start()
        {
            await ResLoader.Init(); //UI资源在基础包里要初始化下
            await Jyx2_UIManager.Instance.ShowUIAsync<ModPanelNew>();
        }
    }
}