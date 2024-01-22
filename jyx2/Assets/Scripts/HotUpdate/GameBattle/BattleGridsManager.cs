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
using System.Linq;
using UnityEngine;



namespace Jyx2.Battle
{
    public class BattleGridsManager : MonoBehaviour
    {
        private float cellWidth = 1.6f, cellHeight = 1.9f;
        private float gridsCountX, gridsCountY;
        private float gridsPivotX, gridsPivotY;

        private List<BattleGridsCell> m_cellList = new List<BattleGridsCell>();

        // Start is called before the first frame update
        void Start()
        {
            CreateBattleGround(16, 12, cellWidth/2,cellHeight/2);
        }

        public void CreateBattleGround(int xCount,int yCount,float xPivot = 0.0f,float yPivot = 0.0f)
        {
            ClearBattleGround();
            
            gridsCountX = xCount;
            gridsCountY = yCount;
            gridsPivotX = xPivot;
            gridsPivotX = yPivot;
            float firstPivotX = gridsPivotX - xCount / 2 * cellWidth;
            float firstPivotY = gridsPivotY - yCount / 2 * cellHeight;
            var prefab = Jyx2ResourceHelper.GetCachedPrefab("BattleGridCeil");
            for (int i = 0; i < gridsCountX; i++)
            {
                for (int j = 0; j < gridsCountY; j++)
                {
                    BattleGridsCell cell = Instantiate(prefab).GetComponent<BattleGridsCell>();
                    cell.transform.SetParent(this.transform);
                    cell.bindGrids = this;
                    cell.CellInit(cellWidth, cellHeight, i, j, firstPivotX + i * cellWidth, firstPivotY + j * cellHeight);
                    cell.SetColor(Color.white);
                    m_cellList.Add(cell);
                }
            }
        }

        public void ClearBattleGround()
        {
            foreach(var cell in m_cellList)
            {
                Destroy(cell.gameObject);
            }
            m_cellList.Clear();
        }

        public void ShowAllCell()
        {
            foreach (var cell in m_cellList)
            {
                cell.gameObject.SetActive(true);
            }
        }

        public void HideAllCell()
        {
            foreach (var cell in m_cellList)
            {
                cell.gameObject.SetActive(false);
            }
        }

        public BattleGridsCell GetCell(int posX, int posY)
        {
            return m_cellList.FirstOrDefault(cell => cell.posX == posX && cell.posY == posY);
        }

        public void SetAllCellColor(Color color)
        {
            foreach (var cell in m_cellList)
            {
                cell.SetColor(color);
            }
        }

        public void ShowRange(int posX,int posY,int range)
        {
            foreach (var cell in m_cellList)
            {
                if (BattleUtil.CellInRange(posX, posY, cell.posX, cell.posY, range))
                {
                    cell.gameObject.SetActive(true);
                }
                else
                {
                    cell.gameObject.SetActive(false);
                }
            }
        }
    }

}
