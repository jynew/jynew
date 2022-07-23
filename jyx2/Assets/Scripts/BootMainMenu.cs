using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Jyx2;
using UnityEngine;

public class BootMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        Jyx2_UIManager.Instance.GameStart();
    }
}
