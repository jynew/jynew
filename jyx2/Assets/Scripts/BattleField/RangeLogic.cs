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
using UnityEngine;

namespace Jyx2
{
    public class RangeLogic
    {
        #region 附近格子的计算
        const int MAX_COMPUTE_BLOCKS_SIZE = 10;
        //BY CG:
        //只需要计算奇数行和偶数行的格子，其他的根据数学平移计算即可获取结果

        public static void InitNearBlocksCache()
        {
            if (IsInited)
                return;

            for(int i = 0; i < MAX_COMPUTE_BLOCKS_SIZE; ++i)
            {
                cached_nearBlocks_0_0[i] = GenerateNearBlocks(0, 0, i);
                cached_nearBlocks_0_1[i] = GenerateNearBlocks(0, 1, i);
            }
            IsInited = true;
        }

        static bool IsInited = false;
        static Dictionary<int, List<BattleBlockVector>> cached_nearBlocks_0_0 = new Dictionary<int, List<BattleBlockVector>>();
        static Dictionary<int, List<BattleBlockVector>> cached_nearBlocks_0_1 = new Dictionary<int, List<BattleBlockVector>>();

        static List<BattleBlockVector> GetNearBlocksHexMatrics(int x, int y)
        {
            List<BattleBlockVector> rst = new List<BattleBlockVector>();
            for (int i = 0; i < 4; ++i)
            {
                int xx = x + dx[i];
                int yy = y + dy[i];

                rst.Add(new BattleBlockVector(xx, yy));
            }
            return rst;
        }

        static List<BattleBlockVector> GenerateNearBlocks(int x, int y,int maxDist)
        {
            var rst = new List<BattleBlockVector>();
            var visited = new HashSet<int>();
            var searchQueue = new Queue<MoveSearchHelper>();
            searchQueue.Enqueue(new MoveSearchHelper() { X = x, Y = y, Cost = 0 });

            while (searchQueue.Count > 0)
            {
                MoveSearchHelper currentNode = searchQueue.Dequeue();
                int xx = currentNode.X;
                int yy = currentNode.Y;
                int cost = currentNode.Cost;

                int blockHash = GetBlockHash(xx, yy);

                if (visited.Contains(blockHash))
                {
                    continue;
                }

                rst.Add(new BattleBlockVector() { X = xx, Y = yy });
                visited.Add(blockHash);

                foreach (var b in GetNearBlocksHexMatrics(xx, yy))
                {
                    int x2 = b.X;
                    int y2 = b.Y;
                    int dcost = 1;

                    if (cost + dcost <= maxDist && !visited.Contains(GetBlockHash(x2, y2)))
                    {
                        searchQueue.Enqueue(new MoveSearchHelper() { X = x2, Y = y2, Cost = cost + dcost });
                    }
                }
            }
            return rst;
        }
        #endregion

        public const int MOVEBLOCK_MAX_X = 500;
        public const int MOVEBLOCK_MAX_Y = 500;

        /// <summary>
        /// 获取攻击方向，注意，UP和DOWN两个方向，距离是2的倍数//旧逻辑?，目前看不存在2倍关系
        /// </summary>
        /// <param name="tx">target x</param>
        /// <param name="ty">target y</param>
        /// <param name="sx">source x</param>
        /// <param name="sy">source y</param>
        /// <returns></returns>
        public static MoveDirection GetDirection(int tx, int ty, int sx, int sy)
        {
            if (ty == sy)
            {
                //same point, error
                if (tx == sx) return MoveDirection.ERROR;
                if (tx > sx) return MoveDirection.RIGHT;
                if (tx < sx) return MoveDirection.LEFT;
            }

            if (tx == sx )
            {
                if (ty > sy) return MoveDirection.DOWN;
                if (ty < sy) return MoveDirection.UP;
            }

            //if (sy % 2 == 0)
            //{
            //    if (tx >= sx)
            //    {
            //        if (ty > sy) return MoveDirection.UP_RIGHT;
            //        if (ty < sy) return MoveDirection.DOWN_RIGHT;
            //    }
			//
            //    if (tx < sx)
            //    {
            //        if (ty > sy) return MoveDirection.UP_LEFT;
            //        if (ty < sy) return MoveDirection.DOWN_LEFT;
            //    }
            //}
            //else
            //{
            //    if (tx > sx)
            //    {
            //        if (ty > sy) return MoveDirection.UP_RIGHT;
            //        if (ty < sy) return MoveDirection.DOWN_RIGHT;
            //    }
			//
            //    if (tx <= sx)
            //    {
            //        if (ty > sy) return MoveDirection.UP_LEFT;
            //        if (ty < sy) return MoveDirection.DOWN_LEFT;
            //    }
            //}

            return MoveDirection.ERROR;
        }

        private readonly Func<int, int, bool> Exists;
        private readonly Func<int, int, bool> HasRole;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="exists">BattleBlockHelper的IsBlockExists</param>
        /// <param name="hasRole">DemoBattleFieldModel的BlockHasRole（后期改成BlockRoleTeam）</param>
        public RangeLogic(Func<int, int, bool> exists, Func<int, int, bool> hasRole)
        {
            Exists = exists;
            HasRole = hasRole;
            InitNearBlocksCache();
        }

        //返回附近四个格子之一
        public BattleBlockVector GetNearBlock(int x, int y, MoveDirection dir)
        {
            int newx = x;
            int newy = y;
            switch (dir)
            {
                case MoveDirection.DOWN:
                    newy += 1;
                    break;
                case MoveDirection.UP:
                    newy -= 1;
                    break;
                case MoveDirection.LEFT:
                    newx -= 1;
                    break;
                case MoveDirection.RIGHT:
                    newx += 1;
                    break;
                default: //jyx2 不允许其他的方向，只有四向
                    return null;
            }

            if (Exists != null && !Exists(newx, newy)) return null;

            var rst = new BattleBlockVector(newx, newy);
            return rst;
        }

        public BattleBlockVector GetNearBlock(int x, int y, MoveDirection d1,
            MoveDirection d2)
        {
            var tmp = GetNearBlock(x, y, d1);
            if (tmp == null) return null;

            return GetNearBlock(tmp.X, tmp.Y, d2);
        }


        static readonly int[] dx = new int[] { 1, 0, -1, 0 };
        static readonly int[] dy = new int[] { 0, 1, 0, -1 };

        /// <summary>
        /// 返回附近距离为1的格子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public IEnumerable<BattleBlockVector> GetNearBlocks(int x, int y)
        {
            for (int i = 0; i < 4; ++i)
            {
                int xx = x + dx[i];
                int yy = y + dy[i];
                if (xx < 0 || xx >= MOVEBLOCK_MAX_X) continue;
                if (yy < 0 || yy >= MOVEBLOCK_MAX_Y) continue;

                if (Exists != null && !Exists(xx, yy)) continue;

                var rst = new BattleBlockVector(xx, yy);
                yield return rst;
            }
        }


        public Dictionary<string, List<BattleBlockVector>> nearblocksCache
            = new Dictionary<string, List<BattleBlockVector>>();

        string GetBlockCacheKey(int x, int y, int distance)
        {
            return $"{x}_{y}_{distance}";
        }

        /// <summary>
        /// 获取距离某格子距离为maxdistance的所有格子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="maxdistance"></param>
        /// <returns></returns>
        public IEnumerable<BattleBlockVector> GetNearBlocks(int x, int y, int maxdistance)
        {
            if (maxdistance <= 1)
            {
                return GetNearBlocks(x, y);
            }

            if (maxdistance > MAX_COMPUTE_BLOCKS_SIZE)
                maxdistance = MAX_COMPUTE_BLOCKS_SIZE;

            List<BattleBlockVector> blocks = null;
            int deltaX = 0;
            int deltaY = 0;
            if (y % 2 == 0)
            {
                blocks = cached_nearBlocks_0_0[maxdistance];
                deltaX = x;
                deltaY = y;
            }
            else
            {
                blocks = cached_nearBlocks_0_1[maxdistance];
                deltaX = x;
                deltaY = y - 1;
            }

            List<BattleBlockVector> rst = new List<BattleBlockVector>();
            foreach(var b in blocks)
            {
                int newX = b.X + deltaX;
                int newY = b.Y + deltaY;

                if (Exists != null && !Exists(newX, newY)) continue;

                rst.Add(new BattleBlockVector(b.X + deltaX, b.Y + deltaY));
            }
            return rst;
        }

        static int GetBlockHash(int x,int y)
        {
            return x * 10000 + y;
        }

        //重复调用，将覆盖之前的内容
        void GenNearBlocksFirstTime(int x, int y, int maxdistance)
        {
            var pk = GetBlockCacheKey(x, y, maxdistance);
            if (!nearblocksCache.ContainsKey(pk))
            {
                nearblocksCache.Add(pk, new List<BattleBlockVector>());
            }

            var list = nearblocksCache[pk];
            list.Clear();
            var visited = new HashSet<int>();
            var searchQueue = new Queue<MoveSearchHelper>();
            searchQueue.Enqueue(new MoveSearchHelper() {X = x, Y = y, Cost = 0});

            while (searchQueue.Count > 0)
            {
                MoveSearchHelper currentNode = searchQueue.Dequeue();
                int xx = currentNode.X;
                int yy = currentNode.Y;
                int cost = currentNode.Cost;

                int blockHash = GetBlockHash(xx, yy);

                if (visited.Contains(blockHash))
                {
                    continue;
                }

                list.Add(new BattleBlockVector() {X = xx, Y = yy});
                visited.Add(blockHash);

                foreach (var b in GetNearBlocks(xx, yy))
                {
                    int x2 = b.X;
                    int y2 = b.Y;
                    int dcost = 1;

                    if (Exists != null && !Exists(x2, y2)) continue;

                    if (cost + dcost <= maxdistance && !visited.Contains(GetBlockHash(x2,y2)))
                    {
                        searchQueue.Enqueue(new MoveSearchHelper() {X = x2, Y = y2, Cost = cost + dcost});
                    }
                }
            }
        }

        /// <summary>
        /// 寻路函数，从sx，sy到tx，ty的路径（返回最近的任意一条）
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="ignoreRole">是否可以穿人，默认关闭</param>
        /// <returns></returns>
        public List<MoveSearchHelper> GetWay(int sx, int sy, int tx, int ty,
            bool ignoreRole = false)
        {
            var rst = new List<MoveSearchHelper>();
            if (sx == tx && sy == ty) return rst;

            var visited = new HashSet<string>();
            Queue<MoveSearchHelper> queue = new Queue<MoveSearchHelper>();
            queue.Enqueue(new MoveSearchHelper() {X = sx, Y = sy});
            visited.Add($"{sx}_{sy}");

            while (queue.Count > 0)
            {
                MoveSearchHelper node = queue.Dequeue();
                int xx = node.X;
                int yy = node.Y;
                if (xx == tx && yy == ty)
                {
                    do
                    {
                        rst.Add(node);
                        node = node.front;
                    } while (node != null);

                    rst.Reverse();
                    return rst;
                }

                var nearBlocks = GetNearBlocks(xx, yy);
                foreach (var b in nearBlocks)
                {
                    int x2 = b.X;
                    int y2 = b.Y;

                    if (Exists != null && !Exists(x2, y2)) continue;
                    if (!ignoreRole && HasRole != null && HasRole(x2, y2) && (x2 != tx || y2 != ty)) continue;

                    if (visited.Contains($"{x2}_{y2}")) continue;

                    queue.Enqueue(new MoveSearchHelper() {X = x2, Y = y2, front = node});
                    visited.Add($"{x2}_{y2}");
                }
            }

            return rst;
        }

        /// <summary>
        /// 根据角色位置和行动力，找到行动范围
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mobility"></param>
        /// <param name="ignoreRole">是否可以穿人，默认关闭</param>
        /// <returns></returns>
        public List<BattleBlockVector> GetMoveRange(int x, int y, int mobility,
            bool ignoreRole = false)
        {
            var rst = new List<BattleBlockVector>();
            var visited = new HashSet<int>();

            Queue<MoveSearchHelper> searchQueue = new Queue<MoveSearchHelper>();
            searchQueue.Enqueue(new MoveSearchHelper() {X = x, Y = y, Cost = 0});

            visited.Add(GetBlockHash(x, y));

            while (searchQueue.Count > 0)
            {
                MoveSearchHelper currentNode = searchQueue.Dequeue();
                int xx = currentNode.X;
                int yy = currentNode.Y;
                int cost = currentNode.Cost;
                rst.Add(new BattleBlockVector() {X = xx, Y = yy});

                var nearBlocks = GetNearBlocks(xx, yy);
                foreach (var b in nearBlocks)
                {
                    int x2 = b.X;
                    int y2 = b.Y;
                    int dcost = 1;

                    if (Exists != null && !Exists(x2, y2)) continue;
                    if (!ignoreRole && HasRole != null && HasRole(x2, y2)) continue;

                    if (cost + dcost <= mobility && !visited.Contains(GetBlockHash(x2, y2)))
                    {
                        searchQueue.Enqueue(new MoveSearchHelper() {X = x2, Y = y2, Cost = cost + dcost});
                        visited.Add(GetBlockHash(x2, y2));
                    }
                }
            }

            return rst;
        }

        /// <summary>
        /// 返回角色技能的施展距离
        /// </summary>
        /// <param name="zhaoshi"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public int GetCastSize(BattleZhaoshiInstance zhaoshi, RoleInstance source)
        {
            if (zhaoshi.GetCastSize() == 0)
                return 0;
            var castsize = PreCastSizeAdjust(zhaoshi, source);
            castsize = PostRoleCastSizeAdjust(castsize, source);
            return castsize;
        }

        /// <summary>
        /// 角色施展技能前调整距离
        /// </summary>
        /// <param name="zhaoshi"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int PreCastSizeAdjust(BattleZhaoshiInstance zhaoshi, RoleInstance source)
        {
            var castsize = zhaoshi.GetCastSize();
            return castsize;
        }

        /// <summary>
        /// 角色施展技能距离确定前调整距离
        /// </summary>
        /// <param name="castsize"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int PostRoleCastSizeAdjust(int castsize, RoleInstance source)
        {
            return castsize;
        }

        /// <summary>
        /// 获取技能施展范围
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zhaoshi"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<BattleBlockVector> GetSkillCastBlocks(int x, int y, BattleZhaoshiInstance zhaoshi, RoleInstance source)
        {
            var castSize = GetCastSize(zhaoshi, source);

            var covertype = zhaoshi.GetCoverType();
            if (covertype == SkillCoverType.LINE )
            {
                foreach (var loc in GetNearBlocks(x, y))
                {
                    yield return new BattleBlockVector(loc.X, loc.Y);
                }

                yield break;
            }

            yield return new BattleBlockVector(x, y);

            if (castSize == 0)
            {
                yield break;
            }

            foreach (var loc in GetNearBlocks(x, y, castSize))
            {
                yield return new BattleBlockVector(loc.X, loc.Y);
            }
        }

        /// <summary>
        /// 获取技能覆盖范围
        /// </summary>
        /// <param name="covertype"></param>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="coversize"></param>
        /// <returns></returns>
        public IEnumerable<BattleBlockVector> GetSkillCoverBlocks(SkillCoverType covertype, int tx, int ty,
            int sx, int sy, int coversize)
        {
            var rst = new List<BattleBlockVector>();
            switch (covertype)
            {
                //点攻击，只有一个格子
                case SkillCoverType.POINT:
                    rst.Add(new BattleBlockVector() { X = tx, Y = ty });
                    break;
                case SkillCoverType.LINE:
                {
                    if (coversize == 0) coversize = 1; //修正直线攻击没有攻击覆盖的BUG
                    int tmpx = sx;
                    int tmpy = sy;
                    MoveDirection direction = GetDirection(tx, ty, sx, sy);
                    if (direction == MoveDirection.ERROR) break;
                    for (int i = 0; i < coversize; ++i)
                    {
                        BattleBlockVector b = GetNearBlock(tmpx, tmpy, direction);
                        if (b == null) continue;
                        rst.Add(b);
                        tmpx = b.X;
                        tmpy = b.Y;
                    }
                    break;
                }
                case SkillCoverType.CROSS:
                {
                    //rst.Add(new BattleBlockVector() { X = tx, Y = ty });
					var directionList=new MoveDirection[4]{MoveDirection.LEFT,MoveDirection.RIGHT,MoveDirection.UP,MoveDirection.DOWN};
                    for (int i = 0; i < directionList.Count(); i++)
                    {
                        int tmpx = tx;
                        int tmpy = ty;
                        MoveDirection direction = directionList[i];
						for (int j = 0; j < coversize; ++j)
                        {
                            BattleBlockVector b = GetNearBlock(tmpx, tmpy, direction);
							if (b == null) continue;
                            rst.Add(b);
                            tmpx = b.X;
                            tmpy = b.Y;
                        }
                    }
                    break;
                }
                /*case SkillCoverType.RECT:
                    rst.Add(new BattleBlockVector() { X = tx, Y = ty });
                    

                    if (coversize > 0)
                    {
                        int actualSize = (int)Math.Ceiling(coversize / Math.Sqrt(2)); //尝试修复距离不对？
                        rst.AddRange(GetNearBlocks(tx, ty, actualSize));
                    }
                    break;*/
                case SkillCoverType.RECT:
                    for (int i = tx - coversize; i <= tx + coversize; ++i)
                    {
                        for (int j = ty - coversize; j <= ty + coversize; ++j)
                        {
                            if (i < 0 || j < 0) continue;
                            rst.Add(new BattleBlockVector(i, j));
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(covertype), covertype, null);
            }

            return rst.Distinct();
        }
    }

    public class MoveSearchHelper
    {
        public int X;
        public int Y;
        public int Cost;
        public MoveSearchHelper front;
    }
}
