/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Jyx2
{
    public class DissolveByCamera : MonoBehaviour
    {
        private GameObject Player;

        [HideInInspector]
        private Material[] Mats;

        void Start()
        {
        }

        void FixedUpdate()
        {
            TrySetPlayer();
        }

        void TrySetPlayer()
        {
            if(Player == null)
            {
                var p = RoleHelper.FindPlayer();
                if (p == null) return;
                Player = p.gameObject;
            }

            Shader.SetGlobalVector("_PlayerPos", Player.transform.position);
        }

    }
}
