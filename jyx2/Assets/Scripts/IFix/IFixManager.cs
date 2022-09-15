using System;
using System.IO;
using Cysharp.Threading.Tasks;
using IFix.Core;
using Jyx2.ResourceManagement;
using UnityEngine;
using UnityEngine.Networking;

namespace Jyx2
{
    public class IFixManager
    {
        public static async UniTask LoadPatch()
        {
            var patch = await ResLoader.LoadAsset<TextAsset>("Assets/Patch/Assembly-CSharp.patch.bytes");
            if (patch != null)
            {
                Debug.Log($"loading patch...");
                PatchManager.Load(new MemoryStream(patch.bytes));
                Debug.Log($"load patch finished.");
            }
        }
    }
}
