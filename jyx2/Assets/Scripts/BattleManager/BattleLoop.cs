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
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Jyx2.Middleware;
using Jyx2Configs;
using UnityEngine;

namespace Jyx2.Battle
{
    /// <summary>
    /// 战斗主循环逻辑
    /// </summary>
    public class BattleLoop
    {
        public BattleLoop(BattleManager manager)
        {
            _manager = manager;
        }

        private BattleManager _manager;
        private GameObject m_roleFocusRing;

        /// <summary>
        /// 开始战斗主循环
        /// </summary>
        /// <returns></returns>
        public async UniTask StartLoop()
        {
            var model = _manager.GetModel();

            //生成当前角色高亮环
            m_roleFocusRing = Jyx2ResourceHelper.CreatePrefabInstance("Assets/Prefabs/CurrentBattleRoleTag.prefab");

            //战斗逻辑的主循环
            while (true)
            {
                //判断战斗是否结束
                var rst = model.GetBattleResult();
                if (rst != BattleResult.InProgress)
                {
                    _manager.OnBattleEnd(rst);
                    break;
                }

                //寻找下一个行动的人
                var role = model.GetNextActiveRole();

                if (role == null)
                {
                    GameUtil.LogError("错误：BattleModel.GetNextActiveRole() 返回了空角色");
                    continue;
                }

                //当前行动角色 UI相关展示
                await SetCurrentRole(role);

                //中毒受伤生效
                await RunPosionHurtLogic(role);

                //判断是AI还是人工
                if (role.isAI)
                {
                    await RoleAIAction(role); //AI行动
                }
                else
                {
                    await RoleManualAction(role); //人工操作
                }

                //标记角色已经行动过
                model.OnActioned(role);
            }

            //销毁当前角色高亮环
            Jyx2ResourceHelper.ReleasePrefabInstance(m_roleFocusRing);
        }

        //标记当前激活角色
        async UniTask SetCurrentRole(RoleInstance role)
        {
            //切换摄像机跟随角色
            CameraHelper.Instance.ChangeFollow(role.View.transform);

            //展示UI头像
            Jyx2_UIManager.Instance.ShowUI(nameof(BattleMainUIPanel), BattleMainUIState.ShowRole, role);

            //选中框 跟随目标
            m_roleFocusRing.transform.SetParent(role.View.transform, false);

            //略微比地面高一点
            m_roleFocusRing.transform.localPosition = new Vector3(0, 0.15f, 0);
        }

        //中毒受伤
        /// </summary>
        /// 中毒掉血计算公式可以参考：https://github.com/ZhanruiLiang/jinyong-legend
        ///
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        async UniTask RunPosionHurtLogic(RoleInstance role)
        {
            int hurtEffect = role.Hurt / 20;
            int poisonEffect = role.Poison / 10;

            int hurtEffectRst = Tools.Limit(hurtEffect, 0, role.Hp);
            int poisonEffectRst = Tools.Limit(poisonEffect, 0, role.Hp);
            
            if (hurtEffect == 0 && poisonEffect == 0) return;

            if (hurtEffectRst > 0)
            {
                role.View?.ShowAttackInfo($"<color=white>-{hurtEffectRst}</color>");
                role.Hp -= hurtEffectRst;
            }
            
            if (poisonEffectRst > 0)
            {
                role.View?.ShowAttackInfo($"<color=green>-{poisonEffectRst}</color>");
                role.Hp -= poisonEffectRst;
            }
            if (role.Hp < 1)
                role.Hp = 1;

            //只有实际中毒和受伤才等待
            role.View?.MarkHpBarIsDirty();
            await UniTask.Delay(TimeSpan.FromSeconds(0.8));
        }

        //AI角色行动
        async UniTask RoleAIAction(RoleInstance role)
        {
            //获取AI计算结果
            var aiResult = await AIManager.Instance.GetAIResult(role);
            
            //先移动
            await RoleMove(role, new BattleBlockVector(aiResult.MoveX, aiResult.MoveY));

            //再执行具体逻辑
            await ExecuteAIResult(role, aiResult);
        }

        //执行具体逻辑
        async UniTask ExecuteAIResult(RoleInstance role, AIResult aiResult)
        {
            if (aiResult.Item != null)
            {
                //使用道具
                await RoleUseItem(role, aiResult.Item);
            }
            else if (aiResult.Zhaoshi != null)
            {
                //使用技能
                await RoleCastSkill(role, aiResult.Zhaoshi, new BattleBlockVector(aiResult.AttackX, aiResult.AttackY));
            }
            else
            {
                //休息
                role.OnRest();
            }
        }

        //角色移动
        async UniTask RoleMove(RoleInstance role, BattleBlockVector moveTo)
        {
            if (role == null || moveTo == null)
            {
                GameUtil.LogError("enter move state failed");
                return;
            }

            //寻找移动路径
            var path = _manager.FindMovePath(role, moveTo);
            if (path == null || path.Count == 0)
                return;

            //播放奔跑动画
            role.View.Run();

            //播放移动
            await role.View.transform.DOPath(path.ToArray(), path.Count * 0.2f).SetLookAt(0).SetEase(Ease.Linear);

            //idle动画
            role.View.Idle();

            //设置逻辑位置
            role.Pos = moveTo;
            var enemy = AIManager.Instance.GetNearestEnemy(role);
            if (enemy != null)
            {
                //面向最近的敌人
                role.View.LookAtWorldPosInBattle(enemy.View.transform.position);
            }
        }

        //角色施展技能总逻辑
        async UniTask RoleCastSkill(RoleInstance role, BattleZhaoshiInstance skill, BattleBlockVector skillTo)
        {
            if (role == null || skill == null || skillTo == null)
            {
                GameUtil.LogError("RoleCastSkill失败");
                return;
            }

            var block = BattleboxHelper.Instance.GetBlockData(skillTo.X, skillTo.Y); //获取攻击目标点
            role.View.LookAtBattleBlock(block); //先面向目标
            role.SwitchAnimationToSkill(skill.Data); //切换姿势
            skill.CastCD(); //技能CD
            skill.CastCost(role); //技能消耗（左右互搏体力消耗一次，内力消耗两次）
            skill.CastMP(role);

            await CastOnce(role, skill, skillTo); //攻击一次
            if (Zuoyouhubo(role, skill))
            {
                skill.CastMP(role);
                await CastOnce(role, skill, skillTo); //再攻击一次
            }
        }

        //一次施展技能
        async UniTask CastOnce(RoleInstance role, BattleZhaoshiInstance skill, BattleBlockVector skillTo)
        {
            List<RoleInstance> beHitAnimationList = new List<RoleInstance>();
            //获取攻击范围
            var coverBlocks = BattleManager.Instance.GetSkillCoverBlocks(skill, skillTo, role.Pos);
            //处理掉血
            foreach (var blockVector in coverBlocks)
            {
                var rolei = BattleManager.Instance.GetModel().GetAliveRole(blockVector);
                //还活着
                if (rolei == null || rolei.IsDead()) continue;
                //打敌人的招式
                if (skill.IsCastToEnemy() && rolei.team == role.team) continue;
                //“打”自己人的招式
                if (!skill.IsCastToEnemy() && rolei.team != role.team) continue;

                var result = AIManager.Instance.GetSkillResult(role, rolei, skill, blockVector);

                result.Run();

                //当需要播放受攻击动画时，不直接刷新血条，延后到播放受攻击动画时再刷新。其他情况直接刷新血条。
                if (result.IsDamage())
                {
                    //加入到受击动作List
                    beHitAnimationList.Add(rolei);
                }
                else
                {
                    rolei.View.MarkHpBarIsDirty();
                }
            }

            SkillCastHelper castHelper = new SkillCastHelper
            {
                Source = role.View,
                CoverBlocks = coverBlocks.ToTransforms(),
                Zhaoshi = skill,
                Targets = beHitAnimationList.ToMapRoles(),
            };

            await castHelper.Play();
        }

        //判断是否可以左右互搏
        bool Zuoyouhubo(RoleInstance role, BattleZhaoshiInstance skill)
        {
            return (role.Zuoyouhubo > 0 && (skill.Data.GetSkill().DamageType == 0 || (int)skill.Data.GetSkill().DamageType == 1));
        }

        //使用道具
        async UniTask RoleUseItem(RoleInstance role, Jyx2ConfigItem item)
        {
            if (role == null || item == null)
            {
                GameUtil.LogError("使用物品状态错误");
                return;
            }

            AnimationClip clip = null;
            var itemType = item.GetItemType();
            if (itemType == Jyx2ItemType.Costa)
                clip = GlobalAssetConfig.Instance.useItemClip; //选择吃药的动作
            else if (itemType == Jyx2ItemType.Anqi)
                clip = GlobalAssetConfig.Instance.anqiClip; //选择使用暗器的动作

            //如果配置了动作，则先播放动作
            if (clip != null)
            {
                await role.View.PlayAnimationAsync(clip, 0.25f);
            }

            role.UseItem(item);

            if (GameRuntimeData.Instance.IsRoleInTeam(role.GetJyx2RoleId())) //如果是玩家角色，则从背包里扣。
            {
                GameRuntimeData.Instance.AddItem(item.Id, -1);
            }
            else //否则从角色身上扣
            {
                role.AddItem(item.Id, -1);
            }

            Dictionary<int, int> effects = UIHelper.GetItemEffect(item);
            foreach (var effect in effects)
            {
                if (!GameConst.ProItemDic.ContainsKey(effect.Key.ToString()))
                    continue;
                PropertyItem pro = GameConst.ProItemDic[effect.Key.ToString()];
                if (effect.Key == 15 || effect.Key == 17)
                {
                    role.View.ShowBattleText($"{pro.Name}+{effect.Value}", Color.blue);
                }
                else if (effect.Key == 6 || effect.Key == 8 || effect.Key == 26)
                {
                    string valueText = effect.Value > 0 ? $"+{effect.Value}" : effect.Value.ToString();
                    role.View.ShowBattleText($"{pro.Name}{valueText}", Color.green);
                }
                else if (effect.Key == 13 || effect.Key == 16)
                {
                    role.View.ShowBattleText($"{pro.Name}+{effect.Value}", Color.white);
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }


        /// <summary>
        /// 手动控制战斗
        ///
        /// 手动控制的状态为：
        /// 1）没有移动的时候可以选择移动或者行动
        /// 2）移动了之后只能选择行动
        /// 3）有的行动（如用毒、医疗、攻击等）需要选择目标格，有的不需要（如休息）
        ///
        /// 可能存在的状态分别为：
        /// 
        /// 1）待移动，显示指令面板。
        /// 2）已经移动过，待选择攻击目标，显示指令面板
        /// 3）尚未移动过，待选择攻击目标，显示指令面板
        /// 4）正在移动，不显示指令面板
        /// 5）选择待使用的道具（浮于所有面板之上）
        /// 
        /// 任何时候点击取消，都回到初始状态1
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        async UniTask RoleManualAction(RoleInstance role)
        {
            bool isSelectMove = true;
            var originalPos = role.Pos;
            //原始的移动范围
            var moveRange = BattleManager.Instance.GetMoveRange(role, role.movedStep);

            while (true)
            {
                var ret = await WaitForPlayerInput(role, moveRange, isSelectMove);

                if (ret.isRevert) //点击取消
                {
                    isSelectMove = true;
                    role.movedStep = 0;
                    role.Pos = originalPos;
                }
                else if (ret.movePos != null && isSelectMove) //移动
                {
                    isSelectMove = false;
                    role.movedStep += originalPos.GetDistance(ret.movePos);
                    await RoleMove(role, ret.movePos);
                }else if (ret.isWait) //等待
                {
                    _manager.GetModel().ActWait(role);
                    break;
                }else if (ret.isAuto) //托管给AI
                {
                    role.Pos = originalPos;
                    await RoleAIAction(role);
                    break;
                }
                else if (ret.aiResult != null) //具体执行行动逻辑（攻击、道具、用毒、医疗等）
                {
                    role.movedStep = 0;
                    await ExecuteAIResult(role, ret.aiResult);
                    break;
                }
            }
        }
        
        //手动控制操作结果
        public class ManualResult
        {
            public BattleBlockVector movePos = null;
            public bool isRevert = false;
            public AIResult aiResult = null;
            public bool isWait = false;
            public bool isAuto = false;
        }
        
        //等待玩家输入
        async UniTask<ManualResult> WaitForPlayerInput(RoleInstance role, List<BattleBlockVector> moveRange, bool isSelectMove)
        {
            UniTaskCompletionSource<ManualResult> t = new UniTaskCompletionSource<ManualResult>();
            Action<ManualResult> callback = delegate(ManualResult result) { t.TrySetResult(result); };
            
            //显示技能动作面板，同时接受格子输入
            Jyx2_UIManager.Instance.ShowUI(nameof(BattleActionUIPanel),role, moveRange, isSelectMove, callback);
            
            //等待完成
            await t.Task;
            
            //关闭面板
            Jyx2_UIManager.Instance.HideUI(nameof(BattleActionUIPanel));
            
            //返回
            return t.GetResult(0);
        }
    }
}