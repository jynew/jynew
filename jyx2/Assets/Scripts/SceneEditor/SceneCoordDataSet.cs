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
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using ProtoBuf;


namespace Jyx2
{
    [ProtoContract]
    public class SceneCoordDataSet
    {
        public static float BlockLength = 1.5f;
        //相邻两行x轴的整体偏移
        public static float RowXDiff = 0.75f;
        //相邻两行y轴的偏移
        public static float RowYDiff = 1.25f;
        //排除拥挤点半径
        public static float DetechRadius = 0.8f;

        [ProtoMember(1)]
        public string SceneName { get; set; }

        [ProtoMember(2)]
        public int CountX { get; set; }
        [ProtoMember(3)]
        public int CountY { get; set; }

        [ProtoMember(4)]
        public int[] CoordCode { get; set; }

        [ProtoMember(5)]
        public float MinX { get; set; }
        [ProtoMember(6)]
        public float MinY { get; set; }

        [ProtoMember(7)]
        public float TerrainLength { get; set; }
        [ProtoMember(8)]
        public float TerrainWidth { get; set; }


        private SceneCoordDataSet() { }

        public SceneCoordDataSet(string name, float tlength, float twidth, float minx, float miny)
        {
            SceneName = name;
            TerrainLength = tlength;
            TerrainWidth = twidth;

            CountX = (int)Math.Floor(TerrainLength / BlockLength - 3.0);
            CountY = (int)Math.Floor(TerrainWidth / RowYDiff - 3.0);

            CoordCode = new int[CountX * CountY];

            MinX = minx;
            MinY = miny;
        }

        public void SaveToFile()
        {
            var filePath = $"{ConStr.BattleBlockDatasetPath}{SceneName}_coord_dataset.bytes";
            
            using (var memory = new MemoryStream())
            {
                Serializer.Serialize(memory, this);
                File.WriteAllBytes(filePath, memory.ToArray());
            }
        }

        public static async UniTask<SceneCoordDataSet> CreateBySceneName(string name)
        {
            return await Jyx2ResourceHelper.GetSceneCoordDataSet(name);
        }
        
        public int GetCoordValue(int xindex, int yindex)
        {
            return CoordCode[yindex * CountX + xindex];
        }
        
        //格子编号从0,0开始
        //x轴的buff是m_BlockLength+
        //y轴的buff是m_RowYDiff
        public System.Numerics.Vector2 CalcPos(int xindex, int yindex)
        {
            var rst = new System.Numerics.Vector2(0, 0);

            if (xindex < 0 || yindex < 0) return rst;
            
            if (xindex >= CountX || yindex >= CountY) return rst;
            
            rst.X = BlockLength * 1.5f + xindex * BlockLength + MinX + RowXDiff;
            rst.Y = RowYDiff * 1.5f + yindex * RowYDiff + MinY;
            if (yindex % 2 == 0) rst.X -= RowXDiff;
            return rst;
        }

        public System.Numerics.Vector2 GetXYIndex(float x, float z)
        {
            var rst = new System.Numerics.Vector2(-1, -1);

            var iY = Math.Floor((z - MinY - 1f * RowYDiff) / RowYDiff);

            var iX = x - MinX - 1f * BlockLength - RowXDiff;
            if ((int) iY % 2 == 0) iX += RowXDiff;
            iX = (float)Math.Floor(iX / BlockLength);

            rst.X = iX;
            rst.Y = (float)iY;

            return rst;
        }
        
        public void SetPoint(int xindex, int yindex, int value)
        {
            CoordCode[yindex * CountX + xindex] = value;
        }

        public int GetValueSum()
        {
            return CoordCode.Sum();
        }

        public int GetCount()
        {
            return CoordCode.Length;
        }
    }
}
