/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2;
using HSFrameWork.Common;
using HSFrameWork.ConfigTable;
using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//AI计算相关
public class AIManager
{
    private static AIManager _instance;
    public static AIManager Instance 
    {
        get 
        {
            if (_instance == null)
            {
                _instance = new AIManager();
                _instance.Init();
            }
            return _instance;
        }
    }

    RangeLogic rangeLogic 
    {
        get 
        {
            return BattleManager.Instance.GetRangeLogic();
        }
    }
    BattleFieldModel BattleModel 
    {
        get 
        {
            return BattleManager.Instance.GetModel();
        }
    }

    private void Init()
    {
    }
    public void GetAIResult(RoleInstance role, Action<AIResult> callback)
    {
        GetAIResultClassic(role, callback);
    }

    public void GetAIResultClassic(RoleInstance role, Action<AIResult> callback)
    {
        //初始化范围逻辑
        //rangeLogic = new RangeLogic(BattleboxHelper.Instance.IsBlockExists, BattleModel.BlockHasRole);

        //获得角色移动能力
        int moveAbility = role.GetMoveAbility();

        //行动范围
        var range = rangeLogic.GetMoveRange(role.Pos.X, role.Pos.Y, moveAbility);

        //可使用招式
        var zhaoshis = role.GetZhaoshis(false);

        //TODO:攻击计算缓存 by Cherubinxxx
        //AttackResultCache cache = new AttackResultCache(currentSprite, Field);

        //AI算法：穷举每个点，使用招式，取最大收益
        AIResult result = null;
        double maxscore = 0;

        foreach (var zhaoshi in zhaoshis)
        {
            if (zhaoshi.GetStatus() != BattleZhaoshiInstance.ZhaoshiStatus.OK)
                continue;

            GetMoveAndCastPos(role, zhaoshi, range);

            BattleBlockVector[] tmp = m_GetMoveAndCastPosResult;
            if (tmp != null && tmp.Length == 2 && tmp[0] != null)
            {
                BattleBlockVector movePos = tmp[0];
                BattleBlockVector castPos = tmp[1];
                double score = GetSkillCastResultScore(role, zhaoshi, movePos.X, movePos.Y, castPos.X, castPos.Y, true);
                // yield return 0; //分帧
                //if (score <= 0 && rst.HitEnemyCount > 0)
                //{
                //    score = ToolsShared.GetRandom(0, 1);
                //}
                if (score > maxscore)
                {
                    maxscore = score;
                    result = new AIResult
                    {
                        AttackX = castPos.X,
                        AttackY = castPos.Y,
                        MoveX = movePos.X,
                        MoveY = movePos.Y,
                        Zhaoshi = zhaoshi,
                        IsRest = false
                    };
                }
            }
        }

        //考虑吃药
        if (role.Items.Count > 0 && (role.Hp < 0.2 * role.MaxHp || role.Mp < 0.2 * role.MaxMp || role.Tili < 0.2 * GameConst.MaxTili))
        {
            List<Jyx2Item> items = GetAvailableItems(role, 3); //只使用药物
            foreach(var item in items)
            {
                double score = 0;
                //尽量吃刚刚好的药
                if(item.AddHp > 0)
                {
                    score += Mathf.Min(item.AddHp, role.MaxHp - role.Hp) - item.AddHp / 10;
                }
                if(item.AddMp > 0)
                {
                    score += Mathf.Min(item.AddMp, role.MaxMp - role.Mp) / 2 - item.AddMp / 10;
                }
                if(item.AddTili > 0)
                {
                    score += Mathf.Min(item.AddTili, GameConst.MaxTili - role.Tili) - item.AddTili / 10;
                }

                if(score > 0)
                {
                    score *= 1.5;//自保系数大
                }

                if (score > maxscore)
                {
                    maxscore = score;
                    var tmp = GetFarestEnemyBlock(role, range);
                    result = new AIResult
                    {
                        MoveX = tmp.X,
                        MoveY = tmp.Y,
                        IsRest = false,
                        Item = item
                    };
                }
            }
        }

        List<Jyx2Item> anqis = GetAvailableItems(role, 4); //获取暗器
        //使用暗器
        if(anqis.Count > 0)
        {
            foreach(var anqi in anqis)
            {
                BattleZhaoshiInstance anqizhaoshi = new AnqiZhaoshiInstance(role.Anqi, anqi);

                if (anqizhaoshi.GetStatus() != BattleZhaoshiInstance.ZhaoshiStatus.OK)
                    continue;

                GetMoveAndCastPos(role, anqizhaoshi, range);

                BattleBlockVector[] tmp = m_GetMoveAndCastPosResult;
                if (tmp != null && tmp.Length == 2 && tmp[0] != null)
                {
                    BattleBlockVector movePos = tmp[0];
                    BattleBlockVector castPos = tmp[1];
                    double score = GetSkillCastResultScore(role, anqizhaoshi, movePos.X, movePos.Y, castPos.X, castPos.Y, true);

                    if (score > maxscore)
                    {
                        maxscore = score;
                        result = new AIResult
                        {
                            AttackX = castPos.X,
                            AttackY = castPos.Y,
                            MoveX = movePos.X,
                            MoveY = movePos.Y,
                            Zhaoshi = anqizhaoshi,
                            IsRest = false
                        };
                    }
                }
            }
        }

        //Debug.Log(Time.realtimeSinceStartup);

        if (result != null)
        {
            callback(result);
            return;
        }

        //否则靠近自己最近的敌人
        result = MoveToNearestEnemy(role, range);
        if (result != null)
        {
            callback(result);
            return;
        }

        //否则原地休息
        callback(Rest(role));
        return;
    }

    public double GetSkillCastResultScore(RoleInstance caster, BattleZhaoshiInstance skill,
            int movex, int movey, int castx, int casty, bool isAIComputing)
    {
        double score = 0;
        var coverSize = skill.GetCoverSize();
        var coverType = skill.GetCoverType();
        var coverBlocks = rangeLogic.GetSkillCoverBlocks(coverType, castx, casty, movex, movey, coverSize);

        foreach (var blockVector in coverBlocks)
        {
            var targetRole = BattleModel.GetAliveRole(blockVector);
            //还活着
            if (targetRole == null || targetRole.IsDead()) continue;
            //打敌人的招式
            if (skill.IsCastToEnemy() && caster.team == targetRole.team) continue;
            //“打”自己人的招式
            if (!skill.IsCastToEnemy() && caster.team != targetRole.team) continue;

            var result = GetSkillResult(caster, targetRole, skill, blockVector);
            score += result.GetTotalScore();

            //暗器算分
            if (skill is AnqiZhaoshiInstance)
            {
                if (score > targetRole.Hp)
                {
                    score = targetRole.Hp * 1.25;
                }
                score *= 0.5;//暗器分值略低
            }
        }

        return score;
    }


    /// <summary>
    /// 靠近自己最近的敌人
    /// </summary>
    /// <returns>The to nearest enemy.</returns>
    /// <param name="sprite">Sprite.</param>
    /// <param name="moverange">Moverange.</param>
    public AIResult MoveToNearestEnemy(RoleInstance sprite, List<BattleBlockVector> range)
    {
        var tmp = GetNearestEnemyBlock(sprite, range);
        if (tmp == null) return null;

        AIResult rst = new AIResult
        {
            Zhaoshi = null,
            MoveX = tmp.X,
            MoveY = tmp.Y,
            IsRest = true //靠近对手
        };
        return rst;
    }

    /// <summary>
    /// 原地休息
    /// </summary>
    /// <param name="sprite">Sprite.</param>
    public AIResult Rest(RoleInstance sprite)
    {
        AIResult rst = new AIResult
        {
            MoveX = sprite.Pos.X,
            MoveY = sprite.Pos.Y,
            IsRest = true
        };
        return rst;
    }

    private BattleBlockVector[] m_GetMoveAndCastPosResult = new BattleBlockVector[2];

    public void GetMoveAndCastPos(RoleInstance role, BattleZhaoshiInstance zhaoshi, List<BattleBlockVector> moveRange)
    {
        //clear
        m_GetMoveAndCastPosResult[0] = null;
        m_GetMoveAndCastPosResult[1] = null;

        //丢给自己的，随便乱跑一个地方丢
        if (zhaoshi.GetCoverType() == SkillCoverType.POINT && zhaoshi.GetCastSize() == 0 && zhaoshi.GetCoverSize() == 0)
        {
            BattleBlockVector targetBlock = null;
            if ((float)role.Hp / role.MaxHp > 0.5)
            {
                targetBlock = GetNearestEnemyBlock(role, moveRange); //生命大于50%前进
            }
            else
            {
                targetBlock = GetFarestEnemyBlock(role, moveRange); //生命小于50%后退
            }
            m_GetMoveAndCastPosResult[0] = targetBlock;
            m_GetMoveAndCastPosResult[1] = targetBlock;
        }

        bool isAttack = zhaoshi.IsCastToEnemy();
        double maxScore = 0;

        //带攻击范围的，找最多人丢
        foreach (var moveBlock in moveRange)
        {
            int castSize = zhaoshi.GetCastSize();
            var coverType = zhaoshi.GetCoverType();
            var sx = moveBlock.X;
            var sy = moveBlock.Y;
            var castBlocks = rangeLogic.GetSkillCastBlocks(sx, sy, zhaoshi, role);

            int splitFrame = 0;//分帧
            foreach (var castBlock in castBlocks)
            {
                double score = 0;
                var coverSize = zhaoshi.GetCoverSize();
                var tx = castBlock.X;
                var ty = castBlock.Y;
                var coverBlocks = rangeLogic.GetSkillCoverBlocks(coverType, tx, ty, sx, sy, coverSize);

                foreach (var coverBlock in coverBlocks)
                {
                    var targetSprite = BattleModel.GetAliveRole(coverBlock);
                    //位置没人
                    if (targetSprite == null) continue;

                    //如果判断是施展给原来的自己，但自己已经不在原位置了,相当于没打中
                    if (targetSprite == role && !(targetSprite.Pos.X == moveBlock.X && targetSprite.Pos.Y == moveBlock.Y)) continue;
                    //如果是自己的新位置，则相当于施展给自己
                    if (targetSprite.Pos.X == moveBlock.X && targetSprite.Pos.Y == moveBlock.Y)
                    {
                        continue;
                        //targetSprite = sprite;
                    }
                    else if (targetSprite.team != role.team && targetSprite.Hp > 0)
                    {
                        score += 0.1;
                    }
                }

                if (score > maxScore)
                {
                    maxScore = score;

                    m_GetMoveAndCastPosResult[0] = new BattleBlockVector(moveBlock.X, moveBlock.Y);
                    m_GetMoveAndCastPosResult[1] = new BattleBlockVector(castBlock.X, castBlock.Y);
                }
            }
            if (splitFrame++ > 5)//分帧
            {
                // yield return 0;
                splitFrame = 0;
            }
        }
        if (maxScore > 0)
        {

        }
        else
        {
            m_GetMoveAndCastPosResult[0] = null;
            m_GetMoveAndCastPosResult[1] = null;
        }
    }

    public RoleInstance GetNearestEnemy(RoleInstance role)
    {
        int minDistance = int.MaxValue;
        RoleInstance targetRole = null;
        //寻找离自己最近的敌人
        foreach (var sp in BattleModel.AliveRoles)
        {
            if (sp == role) continue;

            if (sp.team == role.team) continue;

            int distance = BattleBlockVector.GetDistance(sp.Pos.X, sp.Pos.Y, role.Pos.X, role.Pos.Y);

            if (distance < minDistance)
            {
                minDistance = distance;
                targetRole = sp;
            }
        }
        return targetRole;
    }

    public BattleBlockVector GetNearestEnemyBlock(RoleInstance sprite, List<BattleBlockVector> moverange = null)
    {
        var targetRole = GetNearestEnemy(sprite);
        if (targetRole == null)
            return null;

        int minDis2 = int.MaxValue;
        int movex = sprite.Pos.X, movey = sprite.Pos.Y;
        //寻找离对手最近的一点
        foreach (var mr in moverange)
        {
            int distance = BattleBlockVector.GetDistance(mr.X, mr.Y, targetRole.Pos.X, targetRole.Pos.Y);

            if (distance <= minDis2)
            {
                minDis2 = distance;
                movex = mr.X;
                movey = mr.Y;
            }
        }
        BattleBlockVector rst = new BattleBlockVector
        {
            X = movex,
            Y = movey
        };
        return rst;
    }

    public BattleBlockVector GetFarestEnemyBlock(RoleInstance sprite, List<BattleBlockVector> range)
    {
        int max = 0;
        BattleBlockVector rst = new BattleBlockVector();
        //寻找一个点离敌人最远
        foreach (var r in range)
        {
            int min = int.MaxValue;
            foreach (RoleInstance sp in BattleModel.AliveRoles)
            {
                int distance = BattleBlockVector.GetDistance(sp.Pos.X, sp.Pos.Y, r.X, r.Y);
                if (sp.team != sprite.team && distance < min)
                {
                    min = distance;
                }
            }
            if (min > max)
            {
                max = min;
                rst = r;
            }
        }
        return rst;
    }
    

    public SkillCastResult GetSkillResult(RoleInstance r1, RoleInstance r2, BattleZhaoshiInstance skill, BattleBlockVector blockVector)
    {
        SkillCastResult rst = new SkillCastResult(r1, r2, skill, blockVector.X, blockVector.Y);
        var magic = skill.Data.GetSkill();
        int level_index = skill.Data.GetLevel()-1;//此方法返回的是显示的武功等级，1-10。用于calMaxLevelIndexByMP时需要先-1变为数组index再使用
        level_index = skill.calMaxLevelIndexByMP(r1.Mp, level_index)+1;//此处计算是基于武功等级数据index，0-9.用于GetSkillLevelInfo时需要+1，因为用于GetSkillLevelInfo时需要里是基于GetLevel计算的，也就是1-10.
        //普通攻击
        if (magic.DamageType == 0)
        {
            if (r1.Mp <= 10)
            {
                rst.damage = 1 + UnityEngine.Random.Range(0, 10);
                return rst;
            }
            int attack = r1.Attack + skill.Data.GetSkillLevelInfo(level_index).Attack / 3;
            int defence = r2.Defence;
            if (r1.Weapon >= 0)
            {
                var i = ConfigTable.Get<Jyx2Item>(r1.Weapon);
                attack += i.Attack;
            }
            if (r1.Armor >= 0)
            {
                var i = ConfigTable.Get<Jyx2Item>(r1.Armor);
                attack += i.Attack;
            }
            if (r2.Weapon >= 0)
            {
                var i = ConfigTable.Get<Jyx2Item>(r2.Weapon);
                defence += i.Defence;
            }
            if (r2.Armor >= 0)
            {
                var i = ConfigTable.Get<Jyx2Item>(r2.Armor);
                defence += i.Defence;
            }

            int v = attack - defence;
            int dis = r1.Pos.GetDistance(r2.Pos);
            v = (int)(v / Math.Exp((dis - 1) / 10));
            v += UnityEngine.Random.Range(0, 10) - UnityEngine.Random.Range(0, 10);
            if (v < 10)
            {
                v = 1 + UnityEngine.Random.Range(0, 10);
            }
            rst.damage = v;
            return rst;
        }
        else if (magic.DamageType == 1) //吸内
        {
            var levelInfo = skill.Data.GetSkillLevelInfo();
            
            //杀伤内力逻辑
            int v = levelInfo.KillMp;
            v += UnityEngine.Random.Range(0, 10) - UnityEngine.Random.Range(0, 10);
            if (v < 10)
            {
                v = 1 + UnityEngine.Random.Range(0, 10);
            }
            rst.damageMp = v;

            //吸取内力逻辑
            int addMp = levelInfo.AddMp;
            if (addMp > 0)
            {
                addMp += UnityEngine.Random.Range(0, 10) - UnityEngine.Random.Range(0, 10);
                if (addMp < 10)
                {
                    addMp = 1 + UnityEngine.Random.Range(0, 10);
                }
                rst.addMp = addMp;    
            }
            
            return rst;
        }
        else if (magic.DamageType == 2) //用毒 -GameUtil::usePoison
        {
            rst.poison = usePoison(r1, r2);
            return rst;
        }
        else if (magic.DamageType == 3) //解毒
        {
            rst.depoison = detoxification(r1, r2);
            return rst;
        }
        else if (magic.DamageType == 4) //治疗
        {
            rst.heal = medicine(r1, r2);
            return rst;
        }
        return null;
    }

    List<Jyx2Item> GetAvailableItems(RoleInstance role, int itemType)
    {
        List<Jyx2Item> items = new List<Jyx2Item>();
        foreach (var item in role.Items)
        {
            var tmp = ConfigTable.Get<Jyx2Item>(item.Id);
            if (tmp.ItemType == itemType)
                items.Add(tmp);
        }
        return items;
    }

    //对应kys-cpp:BattleScene::calMagicHurt

    //用毒
    int usePoison(RoleInstance r1, RoleInstance r2)
    {
        int add = r1.UsePoison / 3;
        int rst = Tools.Limit(r2.Poison + add, 0, 100);
        return rst - r2.Poison;
    }

    //医疗的效果
    int medicine(RoleInstance r1, RoleInstance r2)
    {
        int add = r1.Heal;
        int rst = Tools.Limit(r2.Hp + add, 0, r2.MaxHp);
        return rst - r2.Hp;
    }

    //解毒
    //注意这个返回值通常应为负
    int detoxification(RoleInstance r1, RoleInstance r2)
    {
        int mius = r1.DePoison / 3;
        int rst = Tools.Limit(r2.Poison - mius, 0, 100);
        return r2.Poison - rst;
    }
}
