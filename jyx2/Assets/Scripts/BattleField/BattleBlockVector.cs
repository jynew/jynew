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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using UnityEngine;

namespace Jyx2
{
    [ProtoContract]
    public class BattleBlockVector
    {
        public BattleBlockVector() { }
        public BattleBlockVector(int x,int y)
        {
            X = x;
            Y = y;
        }
        [ProtoMember(1)]
        public int X;
        [ProtoMember(2)]
        public int Y;

        public bool Inaccessible { get; internal set; }

        //求两点的距离
        public int GetDistance(BattleBlockVector pos)
        {
            return GetDistance(X, Y, pos.X, pos.Y);
        }

        public static int GetDistance(int x1, int y1, int x2, int y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            var lucky = dx > 0 ^ y1 % 2 == 0;
            var xOffset = lucky ? (int)Math.Ceiling(Math.Abs(dy * 0.5f)) : (int)Math.Floor(Math.Abs(dy * 0.5f));
            var step = xOffset >= Math.Abs(dx) ? Math.Abs(dy) : (Math.Abs(dy) + Math.Abs(dx) - xOffset);
            //Debug.Log($"the dis from {x1}:{y1} to {x2}:{y2} is {step}");
            return step;
        }

        public static int ToInt(int x, int y)
        {
            return x * 10000 + y;
        }

        public int ToInt()
        {
            return ToInt(this.X, this.Y);
        }

        public static BattleBlockVector FromInt(int hash)
        {
            var lb = new BattleBlockVector();
            lb.X = hash / 10000;
            lb.Y = hash % 10000;
            return lb;
        }

        public static List<BattleBlockVector> FromInt(HashSet<int> hashs)
        {
            List<BattleBlockVector> list = new List<BattleBlockVector>();
            foreach (int hash in hashs)
            {
                list.Add(BattleBlockVector.FromInt(hash));
            }
            return list;
        }

        public override bool Equals(object obj)
        {
            var temp = obj as BattleBlockVector;
            if (temp == null) return false;

            return X == temp.X && Y == temp.Y;
        }

        public override int GetHashCode()
        {
            return X * 10000 + Y;
        }
    }

    static public class BattleBlockVectorTools
    {
        public static IEnumerable<Transform> ToTransforms(this IEnumerable<BattleBlockVector> blocks)
        {
            foreach(var block in blocks)
            {
                var tempBlock = BattleboxHelper.Instance.GetBlockData(block.X, block.Y);
                if (tempBlock != null && tempBlock.gameObject != null)
                {
                    yield return tempBlock.gameObject.transform;
                }
            }
        }
    }
}
