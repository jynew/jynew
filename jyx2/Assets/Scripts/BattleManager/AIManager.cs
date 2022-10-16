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


using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Jyx2.Middleware;
using Jyx2Configs;
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

    public async UniTask<AIResult> GetAIResult(RoleInstance role)
    {
        //初始化范围逻辑
        //rangeLogic = new RangeLogic(BattleboxHelper.Instance.IsBlockExists, BattleModel.BlockHasRole);

        //获得角色移动能力
        int moveAbility = role.GetMoveAbility();

        //行动范围
        var range = rangeLogic.GetMoveRange(role.Pos.X, role.Pos.Y, moveAbility - role.movedStep, false);

        //可使用招式
        var skills = role.GetSkills(false);
        
        //AI算法：穷举每个点，使用招式，取最大收益
        AIResult result = null;
        double maxscore = 0;

        //考虑吃药
        List<Jyx2ConfigItem> items = GetAvailableItems(role, 3); //只使用药物
        foreach (var item in items)
        {
            double score = 0;
            //使用体力药
            if (role.Tili < 0.1 * GameConst.MAX_ROLE_TILI)
            {
                if (item.AddTili > 0)
                {
                    score += Mathf.Min(item.AddTili, GameConst.MAX_ROLE_TILI - role.Tili) - item.AddTili / 10;
                }
            }
            //使用生命药
            if (role.Hp < 20 || role.Hurt > 50)
            {
                if (item.AddHp > 0)
                {
                    score += Mathf.Min(item.AddHp, role.MaxHp - role.Hp) - item.AddHp / 10;
                }
            }
            int r = -1;
            if (role.Hp < 0.2 * role.MaxHp)
            {
                r = 90;
            }
            else if (role.Hp < 0.25 * role.MaxHp)
            {
                r = 70;
            }
            else if (role.Hp < 0.33 * role.MaxHp)
            {
                r = 50;
            }
            else if (role.Hp < 0.5 * role.MaxHp)
            {
                r = 25;
            }
            if (UnityEngine.Random.Range(0, 100) < r)
            {
                if (item.AddHp > 0)
                {
                    score += Mathf.Min(item.AddHp, role.MaxHp - role.Hp) - item.AddHp / 10;
                }
            }
            //使用内力药
            int s = -1;
            if (role.Mp < 0.2 * role.MaxMp)
            {
                s = 75;
            }
            else if (role.Mp < 0.25 * role.MaxMp)
            {
                s = 50;
            }
            if (UnityEngine.Random.Range(0, 100) < s)
            {
                if (item.AddMp > 0)
                {
                    score += Mathf.Min(item.AddMp, role.MaxMp - role.Mp) / 2 - item.AddMp / 10;
                }
            }
            //使用解毒药
            int m = -1;
            if (role.Poison > 0.75 * GameConst.MAX_ANTIPOISON)
            {
                m = 60;
            }
            else if (role.Poison > 0.5 * GameConst.MAX_ANTIPOISON)
            {
                m = 30;
            }
            if (UnityEngine.Random.Range(0, 100) < m)
            {
                if (item.ChangePoisonLevel > 0)
                {
                    score += Mathf.Min(item.ChangePoisonLevel, GameConst.MAX_ANTIPOISON - role.Poison) - item.ChangePoisonLevel / 10;
                }
            }
            
            if (score > 0)
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

        List<Jyx2ConfigItem> anqis = GetAvailableItems(role, 4); //获取暗器
        //使用暗器
        foreach (var anqi in anqis)
        {
            SkillCastInstance anqiSkillCast = new AnqiSkillCastInstance(role.Anqi, anqi);

            if (anqiSkillCast.GetStatus() != SkillCastInstance.SkillCastStatus.OK)
                continue;

            BattleBlockVector[] tmp = await GetMoveAndCastPos(role, anqiSkillCast, range);

            if (tmp != null && tmp.Length == 2 && tmp[0] != null)
            {
                BattleBlockVector movePos = tmp[0];
                BattleBlockVector castPos = tmp[1];
                double score = GetSkillCastResultScore(role, anqiSkillCast, movePos.X, movePos.Y, castPos.X, castPos.Y, true);

                if (score > maxscore)
                {
                    maxscore = score;
                    result = new AIResult
                    {
                        AttackX = castPos.X,
                        AttackY = castPos.Y,
                        MoveX = movePos.X,
                        MoveY = movePos.Y,
                        SkillCast = anqiSkillCast,
                        IsRest = false
                    };
                }
            }
        }
        
        //使用武学
        foreach (var skill in skills)
        {
            if (skill.GetStatus() != SkillCastInstance.SkillCastStatus.OK)
                continue;

            BattleBlockVector[] tmp = await GetMoveAndCastPos(role, skill, range);
            if (tmp != null && tmp.Length == 2 && tmp[0] != null)
            {
                BattleBlockVector movePos = tmp[0];
                BattleBlockVector castPos = tmp[1];
                double score = GetSkillCastResultScore(role, skill, movePos.X, movePos.Y, castPos.X, castPos.Y, true);
                if (score > maxscore)
                {
                    maxscore = score;
                    result = new AIResult
                    {
                        AttackX = castPos.X,
                        AttackY = castPos.Y,
                        MoveX = movePos.X,
                        MoveY = movePos.Y,
                        SkillCast = skill,
                        IsRest = false
                    };
                }

                await UniTask.WaitForEndOfFrame();
            }
        }

        //Debug.Log(Time.realtimeSinceStartup);

        if (result != null)
        {
            return result;
        }

        //否则靠近自己最近的敌人
        result = MoveToNearestEnemy(role, range);
        if (result != null)
        {
            return result;
        }

        //否则原地休息
        return Rest(role);
    }

    public double GetSkillCastResultScore(RoleInstance caster, SkillCastInstance skill,
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
            
            //解毒算分
            if (skill is DePoisonSkillCastInstance)
            {
                if (targetRole.Poison > 50)
                {
                    score = result.poison;
                }
            }

            //医疗算分
            if (skill.GetDamageType() == 4)
            {
                if (skill is HealSkillCastInstance)
                {
                    if (targetRole.Hp < 0.2 * targetRole.MaxHp)
                    {
                        score = result.heal;
                    }
                }
                else
                {
                    if (targetRole.Hp < 0.5 * targetRole.MaxHp)
                    {
                        score = result.heal;
                    }
                }
            }

            //用毒算分
            if (skill is PoisonSkillCastInstance)
            {
                score = Mathf.Min(GameConst.MAX_POISON - targetRole.Poison, caster.UsePoison) * 0.1;
                if (targetRole.Hp < 10)
                {
                    score = 1;
                }
            }
            
            //暗器算分
            if (skill is AnqiSkillCastInstance)
            {
                if (score > targetRole.Hp)
                {
                    score = targetRole.Hp * 1.25;
                }
                score *= 0.1;//暗器分值略低
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
            SkillCast = null,
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

    public async UniTask<BattleBlockVector[]> GetMoveAndCastPos(RoleInstance role, SkillCastInstance skillCast, List<BattleBlockVector> moveRange)
    {
        BattleBlockVector[] rst = new BattleBlockVector[2];
        
        //丢给自己的，随便乱跑一个地方丢
        if (skillCast.GetCoverType() == SkillCoverType.POINT && skillCast.GetCastSize() == 0 && skillCast.GetCoverSize() == 0)
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
            
            
            rst[0] = targetBlock;
            rst[1] = targetBlock;
            return rst;
        }

        bool isAttack = skillCast.IsCastToEnemy();
        double maxScore = 0;

        Dictionary<int,float > cachedScore = new Dictionary<int, float>();
        //带攻击范围的，找最多人丢
        foreach (var moveBlock in moveRange)
        {
            var coverType = skillCast.GetCoverType();
            var sx = moveBlock.X;
            var sy = moveBlock.Y;
            var castBlocks = rangeLogic.GetSkillCastBlocks(sx, sy, skillCast, role);

            int splitFrame = 0;//分帧
            foreach (var castBlock in castBlocks)
            {
                float score = 0;
                if (cachedScore.ContainsKey(castBlock.ToInt()))
                {
                    score = cachedScore[castBlock.ToInt()];
                }
                else
                {
                    var coverSize = skillCast.GetCoverSize();
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
                        //打敌人的招式优先“打”自己人的招式
                        if (isAttack)
                        {
                            //如果是自己的新位置，则相当于施展给自己
                            if (targetSprite.Pos.X == moveBlock.X && targetSprite.Pos.Y == moveBlock.Y)
                            {
                                continue;
                            }
                            else if (targetSprite.team != role.team && targetSprite.Hp > 0)
                            {
                                score += 0.2f;
                            }
                        }
                        else
                        {
                            score += 0.1f;
                        }
                    }

                    cachedScore[castBlock.ToInt()] = score;
                }

                if (score > maxScore)
                {
                    maxScore = score;

                    rst[0] = new BattleBlockVector(moveBlock.X, moveBlock.Y);
                    rst[1] = new BattleBlockVector(castBlock.X, castBlock.Y);
                }
            }

            splitFrame++;
            if (splitFrame > 5)//分帧
            {
                splitFrame = 0;
                await UniTask.WaitForEndOfFrame();
            }
        }
        
        if (maxScore == 0)
        {
            rst[0] = null;
            rst[1] = null;
        }

        return rst;
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
            // 遍历时过滤不可达的位置，以免引发异常
            if (BattleboxHelper.Instance.GetBlockData(r.X, r.Y) == null)
                continue;
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

    /// </summary>
    /// 战斗计算公式可以参考：https://tiexuedanxin.net/thread-365140-1-1.html
    ///
    /// 
    /// </summary>
    /// <param name="r1"></param>
    /// <param name="r2"></param>
    /// <param name="skill"></param>
    /// <param name="blockVector"></param>
    /// <returns></returns>
    public SkillCastResult GetSkillResult(RoleInstance r1, RoleInstance r2, SkillCastInstance skill, BattleBlockVector blockVector)
    {        
        SkillCastResult rst = new SkillCastResult(r1, r2, skill, blockVector.X, blockVector.Y);
        var magic = skill.Data.GetSkill();
        int level_index = skill.Data.GetLevel()-1;//此方法返回的是显示的武功等级，1-10。用于calMaxLevelIndexByMP时需要先-1变为数组index再使用
        level_index = skill.calMaxLevelIndexByMP(r1.Mp, level_index)+1;//此处计算是基于武功等级数据index，0-9.用于GetSkillLevelInfo时需要+1，因为用于GetSkillLevelInfo时需要里是基于GetLevel计算的，也就是1-10.
        //普通攻击
        if (magic.DamageType == 0)
        {
            //队伍1武学常识
            int totalWuxue = BattleModel.GetTotalWuXueChangShi(r1.team);

            //队伍2武学常识
            int totalWuxue2 = BattleModel.GetTotalWuXueChangShi(r2.team);

            if (r1.Mp <= magic.MpCost) //已经不够内力释放了
            {
                rst.damage = 1 + UnityEngine.Random.Range(0, 10);
                return rst;
            }
            //总攻击力＝(人物攻击力×3 ＋ 武功当前等级攻击力)/2 ＋武器加攻击力 ＋ 防具加攻击力 ＋ 武器武功配合加攻击力 ＋我方武学常识之和
            int attack = ((r1.Attack - r1.GetWeaponProperty("Attack") - r1.GetArmorProperty("Attack")) * 3 + skill.Data.GetSkillLevelInfo(level_index).Attack) / 2 + r1.GetWeaponProperty("Attack") + r1.GetArmorProperty("Attack") + r1.GetExtraAttack(magic) + totalWuxue;
            
            //总防御力 ＝ 人物防御力 ＋武器加防御力 ＋ 防具加防御力 ＋ 敌方武学常识之和
            int defence = r2.Defence + totalWuxue2;

            //伤害 ＝ （总攻击力－总防御×3）×2 / 3 + RND(20) – RND(20)                  （公式1）
            int v = (attack - defence * 3) * 2 / 3 + UnityEngine.Random.Range(0, 20) - UnityEngine.Random.Range(0, 20);
            
            //如果上面的伤害 < 0 则
            //伤害 ＝  总攻击力 / 10 + RND(4) – RND(4)                                            （公式2）
            if (v <= 0)
            {
                v = attack / 10 + UnityEngine.Random.Range(0, 4) - UnityEngine.Random.Range(0, 4);
            }

            //7、如果伤害仍然 < 0 则    伤害 ＝ 0
            if (v <= 0)
            {
                v = 0;
            }
            else
            {
                //8、if  伤害 > 0 then
                //    伤害＝ 伤害 ＋ 我方体力/15  ＋ 敌人受伤点数/ 20
                v = v + r1.Tili / 15 + r2.Hurt / 20;
            }
            
            //点、线、十字的伤害，距离就是两人相差的格子数，最小为1。
            //面攻击时，距离是两人相差的格子数＋敌人到攻击点的距离。
            int dist = r1.Pos.GetDistance(r2.Pos);
            if (skill.GetCoverType() == SkillCoverType.RECT || skill.GetCoverType() == SkillCoverType.RHOMBUS)
            {
                dist += blockVector.GetDistance(r2.Pos);
            }

            //9、if 双方距离 <= 10 then
            //    伤害 ＝ 伤害×（100 -  ( 距离–1 ) * 3 ）/ 100
            //else
            //    伤害 ＝ 伤害*2 /3
            if (dist <= 10)
            {
                v = v * (100 - (dist - 1) * 3) / 100;
            }
            else
            {
                v = v * 2 / 3;
            }

            //10、if 伤害 < 1  伤害 ＝ 1
            if (v < 1)
                v = 1;

            rst.damage = v;

            //敌人受伤程度
            rst.hurt = v / 10;

            //攻击带毒
            //中毒程度＝武功等级×武功中毒点数＋人物攻击带毒
            int add = level_index * skill.Data.GetSkill().Poison + r1.AttackPoison;
            //if 敌人抗毒> 中毒程度 或 敌人抗毒> 90 则不中毒
            //敌人中毒=敌人已中毒＋ 中毒程度/15
            //if 敌人中毒> 100 then 敌人中毒 ＝100
            //if 敌人中毒<0 then 敌人中毒=0
            if (r2.AntiPoison <= add && r2.AntiPoison <= 90)
            {
                int poison = Tools.Limit(add / 15, 0, GameConst.MAX_ROLE_ATK_POISON);
                rst.poison = poison;
            }
  
            return rst;
        }
        else if ((int)magic.DamageType == 1) //吸内
        {
            var levelInfo = skill.Data.GetSkillLevelInfo();
            
            //杀伤内力逻辑
            int v = levelInfo.KillMp;
            v += UnityEngine.Random.Range(0, 3) - UnityEngine.Random.Range(0, 3);
            rst.damageMp = v;

            //吸取内力逻辑
            int addMp = levelInfo.AddMp;
            if (addMp > 0)
            {
                rst.addMaxMp = UnityEngine.Random.Range(0, addMp / 2);
                addMp += UnityEngine.Random.Range(0, 3) - UnityEngine.Random.Range(0, 3);
                rst.addMp = addMp;    
            }
            
            return rst;
        }
        else if ((int)magic.DamageType == 2) //用毒 -GameUtil::usePoison
        {
            rst.poison = usePoison(r1, r2);
            return rst;
        }
        else if ((int)magic.DamageType == 3) //解毒
        {
            rst.depoison = detoxification(r1, r2);
            return rst;
        }
        else if ((int)magic.DamageType == 4) //治疗
        {
            var _rst = medicine(r1, r2);
            rst.heal = _rst.heal;
            rst.hurt = _rst.hurt;
            return rst;
        }
        else if ((int)magic.DamageType == 5) //暗器
        {
            var anqi = skill.Anqi;
            var _rst = hiddenWeapon(r1, r2, anqi);
            rst.damage = _rst.damage;
            rst.hurt = _rst.hurt;
            rst.poison = _rst.poison;
            return rst;
        }
        return null;
    }

    List<Jyx2ConfigItem> GetAvailableItems(RoleInstance role, int itemType)
    {
        List<Jyx2ConfigItem> items = new List<Jyx2ConfigItem>();
        // 如果角色是玩家这方的，应该使用玩家的物品栏 by Tomato
        if (role.team == 0)
        {
            foreach (var kv in GameRuntimeData.Instance.Items)
            {
                string id = kv.Key;
                int count = kv.Value.Item1;

                var item = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(id);
                if ((int)item.ItemType == itemType)
                    items.Add(item);
            }
        }
        else
        {
            foreach (var item in role.Items)
            {
                var tmp = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(item.Id);
                if ((int)tmp.ItemType == itemType)
                    items.Add(tmp);
            }
        }

        return items;
    }


    //用毒
    /// </summary>
    /// 中毒计算公式可以参考：https://tiexuedanxin.net/thread-365140-1-1.html
    /// 也参考War_PoisonHurt：https://github.com/ZhanruiLiang/jinyong-legend
    /// 
    /// </summary>
    /// <param name="r1"></param>
    /// <param name="r2"></param>
    /// <returns></returns>
    int usePoison(RoleInstance r1, RoleInstance r2)
    {
        //中毒程度 ＝（用毒能力－抗毒能力）/ 4
        int poison = (r1.UsePoison - r2.AntiPoison) / 4;
        //小于0则为0
        if (poison < 0)
            poison = 0;
        return poison;
    }

    //医疗
    /// </summary>
    /// 医疗计算公式可以参考：https://tiexuedanxin.net/forum.php?mod=viewthread&tid=394465
    /// 也参考ExecDoctor：https://github.com/ZhanruiLiang/jinyong-legend
    /// 
    /// </summary>
    /// <param name="r1"></param>
    /// <param name="r2"></param>
    /// <returns></returns>
    SkillCastResult medicine(RoleInstance r1, RoleInstance r2)
    {
        SkillCastResult rst = new SkillCastResult();
        if (r2.Hurt > r1.Heal + 20)
        {
            if (!BattleManager.Instance.IsInBattle)
            {
                GameUtil.DisplayPopinfo("受伤太重无法医疗");
            }
            return rst;
        }
        //增加生命 = 医疗能力 * a + random(5);
        //当受伤程度 > 75, a = 1 / 2;
        //当50 < 受伤程度 <= 75, a = 2 / 3;
        //当25 < 受伤程度 <= 50, a = 3 / 4;
        //当0 < 受伤程度 <= 25, a = 4 / 5;
        //当受伤程度 = 0，a = 4 / 5;
        int a = (int)Math.Ceiling((double)r2.Hurt / 25);
        if (a == 0) a = 1;
        int addHp = r1.Heal * (5 - a) / (6 - a) + UnityEngine.Random.Range(0, 5);
        rst.heal = addHp;
        //减低受伤程度 = 医疗能力.
        rst.hurt = -addHp;
        return rst;
    }

    //解毒
    /// </summary>
    /// 解毒计算公式可以参考ExecDecPoison：https://github.com/ZhanruiLiang/jinyong-legend
    ///
    /// 
    /// </summary>
    /// <param name="r1"></param>
    /// <param name="r2"></param>
    /// <returns></returns>
    int detoxification(RoleInstance r1, RoleInstance r2)
    {
        if (r2.Poison > r1.DePoison + 20)
        {
            if (!BattleManager.Instance.IsInBattle)
            {
                GameUtil.DisplayPopinfo("中毒太重无法解毒"); 
            }
            return 0;
        }
        int add = (r1.DePoison / 3) + UnityEngine.Random.Range(0, 10) - UnityEngine.Random.Range(0, 10);
        int depoison = Tools.Limit(add, 0, r2.Poison);
        return depoison;
    }

    //暗器
    //返回值为一正数
    /// </summary>
    /// 暗器计算公式可以参考War_AnqiHurt：https://tiexuedanxin.net/forum.php?mod=viewthread&tid=394465
    ///
    /// 
    /// </summary>
    /// <param name="r1"></param>
    /// <param name="r2"></param>
    /// <param name="anqi"></param>
    /// <returns></returns>
    SkillCastResult hiddenWeapon(RoleInstance r1, RoleInstance r2, Jyx2ConfigItem anqi)
    {
        SkillCastResult rst = new SkillCastResult();
        //增加生命 = (暗器增加生命/a-random(5)-暗器能力*2)/3;
        //式中暗器增加生命为负值.
        //当受伤程度 = 100，a = 1;
        //当66 < 受伤程度 <= 99, a = 1;
        //当33 < 受伤程度 <= 66, a = 2;
        //当0 < 受伤程度 <= 33, a = 3;
        //当受伤程度 = 0, a = 4;
        int a = (int)Math.Ceiling((double)r2.Hurt / 33);
        if (a == 4) a = 3;
        int v = (anqi.AddHp / (4 - a) - UnityEngine.Random.Range(0, 5) - r1.Anqi * 2) / 3;
        rst.damage = -v;
        //敌人受伤程度
        rst.hurt = -v / 4; //此处v为负值
        //当暗器带毒 > 0,
        //增加中毒程度 = [(暗器带毒 + 暗器技巧) / 2 - 抗毒能力] / 2;
        //当抗毒 = 100, 增加中毒程度 = 0.
        if (anqi.ChangePoisonLevel > 0)
        {
            int add = ((anqi.ChangePoisonLevel + r1.Anqi) / 2 - r2.AntiPoison) / 2;
            if (r2.AntiPoison == 100)
                add = 0;
            int poison = Tools.Limit(add, 0, GameConst.MAX_USE_POISON);
            rst.poison = poison;
        }
        return rst;
    }
}
