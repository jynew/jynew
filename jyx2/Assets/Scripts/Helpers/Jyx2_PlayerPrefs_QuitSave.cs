using System.Collections;
using UnityEngine;

namespace Jyx2
{
    public class Jyx2_PlayerPrefs_QuitSave : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            Debug.Log("游戏即将退出,保存PlayerPrefs...");
            Jyx2_PlayerPrefs.Save();
        }
    }
}