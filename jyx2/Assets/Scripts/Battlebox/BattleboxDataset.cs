/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;


using ProtoBuf;

namespace Jyx2
{
    [ProtoContract]
    public class BattleboxDataset
    {
        //相邻格子的中心点在Unity中x方向上的间隔
        public static float BlockLength = 1f;
        //让奇数行和偶数行格子沿着x轴方向错开数值为RowXDiff的距离
        public static float RowXDiff = 0f; //0.75f;
        //相邻格子的中心点在Unity中z方向上的间隔
        public static float BlockLengthY = BlockLength; //1.25f;


        [ProtoMember(1)]
        public string SceneName { get; set; }

        [ProtoMember(2)]
        public string BattleboxName { get; set; }
        
        [ProtoMember(3)]
        public float MinX { get; set; }
        [ProtoMember(4)]
        public float MinY { get; set; }

        [ProtoMember(5)]
        public float BoxLength { get; set; }
        [ProtoMember(6)]
        public float BoxWidth { get; set; }

        [ProtoMember(7)]
        public int CountX { get; set; }
        [ProtoMember(8)]
        public int CountY { get; set; }

        [ProtoMember(9)]
        public List<BattleboxBlock> Blocks { get; set; }

        private BattleboxDataset() { }

        public BattleboxDataset(string scene, string name, float length, float width, float minx, float miny)
        {
            SceneName = scene;
            BattleboxName = name;
            BoxLength = length;
            BoxWidth = width;
            MinX = minx;
            MinY = miny;
            
            CountX = (int)Math.Floor(BoxLength / BlockLength + 3);
            CountY = (int)Math.Floor(BoxWidth / BlockLengthY + 3);

            Blocks = new List<BattleboxBlock>();
        }

        //格子编号从0,0开始
        public BattleboxBlock GetBLock(int xindex, int yindex)
        {
            return Blocks.FirstOrDefault(x => x.XIndex == xindex && x.YIndex == yindex);
        }

        public List<BattleboxBlock> GetVaildBLock()
        {
            return Blocks.Where(block => block.IsValid).ToList();
        }

        public bool Exist(int xindex, int yindex)
        {
            return Blocks.Exists(x => x.XIndex == xindex && x.YIndex == yindex);
        }

        public bool IsValid(int xindex, int yindex)
        {
            if (!Exist(xindex, yindex)) return false;
            return GetBLock(xindex, yindex).IsValid;
        }

        public void SetValid(int xindex, int yindex, bool isValid)
        {
            var block = GetBLock(xindex, yindex);
            if (block != null) block.IsValid = isValid;
        }

        public int GetValidCount()
        {
            return Blocks.Sum(x => x.IsValid ? 1 : 0);
        }

        public int GetCount()
        {
            return Blocks.Count;
        }

        public int GetSizeCount()
        {
            return CountX * CountY;
        }

        //格子编号从0,0开始
        //x轴的buff是m_BlockLength+
        //y轴的buff是m_BlockLengthY
        public Vector2 CalcPos(int xindex, int yindex)
        {
            var rst = new Vector2(0, 0);

            if (xindex < 0 || yindex < 0) return rst;

            if (xindex >= CountX || yindex >= CountY) return rst;

            rst.X = BlockLength * 0.5f + xindex * BlockLength + MinX + RowXDiff;
            rst.Y = BlockLengthY * 0.5f + yindex * BlockLengthY + MinY;
            if (yindex % 2 == 0) rst.X -= RowXDiff;
            return rst;
        }

        public System.Numerics.Vector2 GetXYIndex(float x, float z)
        {
            var rst = new System.Numerics.Vector2(-1, -1);

            var iY = Math.Floor((z - MinY) / BlockLengthY);

            var iX = x - MinX - RowXDiff;
            if ((int)iY % 2 == 0) iX += RowXDiff;
            iX = (float)Math.Floor(iX / BlockLength);

            rst.X = iX;
            rst.Y = (float)iY;

            return rst;
        }
    }

    [ProtoContract]
    public class BattleboxBlock
    {
        //box collider 中的坐标序号
        [ProtoMember(1)]
        public int XIndex;
        //box collider 中的坐标序号
        [ProtoMember(2)]
        public int YIndex;

        //世界坐标，用来画格子
        [ProtoMember(3)]
        public float WorldPosX;
        [ProtoMember(4)]
        public float WorldPosY;
        [ProtoMember(5)]
        public float WorldPosZ;

        //法线，用来画格子
        [ProtoMember(6)]
        public float NormalX;
        [ProtoMember(7)]
        public float NormalY;
        [ProtoMember(8)]
        public float NormalZ;

        //是否为有效格子，给策划编辑用
        [ProtoMember(9)]
        public bool IsValid;
    }
}
