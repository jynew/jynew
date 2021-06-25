/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ch.sycoforge.Decal;

namespace Jyx2.Battle
{
    public class BattleGridsCell : MonoBehaviour
    {
        /// <summary>
        /// 格子坐标
        /// </summary>
        public int posX, posY;
        /// <summary>
        /// 绑定的战场棋盘
        /// </summary>
        public BattleGridsManager bindGrids;

        private BoxCollider m_bindCollider;
        private EasyDecal m_bindEasyDecal;

        public void Awake()
        {
            m_bindCollider = gameObject.GetComponent<BoxCollider>();
            m_bindEasyDecal = gameObject.GetComponent<EasyDecal>();
        }
        // 初始化格子
        public void CellInit(float width,float height, int posX, int posY ,float transformX, float transformY)
        {
            this.posX = posX;
            this.posY = posY;
            gameObject.transform.position = new Vector3(transformX, 0, transformY);
            m_bindCollider.size = new Vector3(width, 0.1f, height);
        }

        public void SetColor(Color color)
        {
            //m_bindEasyDecal.DecalMaterial.SetColor("_TintColor", color);
            m_bindEasyDecal.DecalMaterial.color = color;
        } 

        private void OnMouseDown()
        {
            bindGrids.SetAllCellColor(Color.green);
            bindGrids.ShowRange(posX, posY, 2);
        }

        
    }
}

