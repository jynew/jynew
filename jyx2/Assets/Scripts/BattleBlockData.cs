

using UnityEngine;

namespace Jyx2
{
    public class BattleBlockData
    {
        //战场逻辑位置
        public BattleBlockVector BattlePos; 

        //实际对应的世界坐标系的点
        public Vector3 WorldPos; 

        //对应绘制的对象
        public GameObject gameObject;

        //包含有效信息，世界坐标，法线坐标等
        public BattleboxBlock BoxBlock;

        public bool IsActive
        {
            get { return _isActive; }
        }
        private bool _isActive = false;

        public void Show()
        {
            gameObject.layer = 0;
            foreach(Transform go in gameObject.transform)
            {
                go.gameObject.layer = 0;
            }
            _isActive = true;
        }

        public void Hide()
        {
            gameObject.layer = 17;
            foreach (Transform go in gameObject.transform)
            {
                go.gameObject.layer = 17;
            }
            _isActive = false;
        }
    }
}
