--[[
 * 金庸群侠传3D重制版
 * https:--github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 ]]--
-- 本脚本为Lua侧游戏战斗伤害结算模块(缺少每回合中毒和Hurt被动扣血部分)
local damage = {}

local BattleModel

function damage.Init()
end

function damage.DeInit()
end

--用毒
--/ </summary>
--/ 中毒计算公式可以参考：https://tiexuedanxin.net/thread-365140-1-1.html
--/ 也参考War_PoisonHurt：https://github.com/ZhanruiLiang/jinyong-legend
--/ 
--/ </summary>
--/ <param name="r1"></param>
--/ <param name="r2"></param>
--/ <returns></returns>
local function usePoison(r1, r2)

    --中毒程度 ＝（用毒能力－抗毒能力）/ 4
    local poison = (r1.UsePoison - r2.AntiPoison) // 4;
    --小于0则为0
    if (poison < 0) then
        poison = 0;
    end
    return poison;
end

--医疗
--/ </summary>
--/ 医疗计算公式可以参考：https://tiexuedanxin.net/forum.php?mod=viewthread&tid=394465
--/ 也参考ExecDoctor：https://github.com/ZhanruiLiang/jinyong-legend
--/ 
--/ </summary>
--/ <param name="r1"></param>
--/ <param name="r2"></param>
--/ <returns></returns>
local function medicine(r1, r2)

    local rst = CS.Jyx2.SkillCastResult();
    if (r2.Hurt > r1.Heal + 20) then

        if (not CS.Jyx2.BattleManager.Instance.IsInBattle) then

            CS.GameUtil.DisplayPopinfo("受伤太重无法医疗");
        end
        return rst;
    end
    --增加生命 = 医疗能力 * a + random(5);
    --当受伤程度 > 75, a = 1 / 2;
    --当50 < 受伤程度 <= 75, a = 2 / 3;
    --当25 < 受伤程度 <= 50, a = 3 / 4;
    --当0 < 受伤程度 <= 25, a = 4 / 5;
    --当受伤程度 = 0，a = 4 / 5;
    local a = math.ceil(r2.Hurt / 25);
    if (a == 0) then a = 1 end
    local addHp = r1.Heal * (5 - a) // (6 - a) + math.random(0, 4);
    rst.heal = addHp;
    --减低受伤程度 = 医疗能力.
    rst.hurt = -addHp;
    return rst;
end

--解毒
--/ </summary>
--/ 解毒计算公式可以参考ExecDecPoison：https://github.com/ZhanruiLiang/jinyong-legend
--/ 
--/ </summary>
--/ <param name="r1"></param>
--/ <param name="r2"></param>
--/ <returns></returns>
local function detoxification(r1, r2)

    if (r2.Poison > r1.DePoison + 20) then

        if (not CS.Jyx2.BattleManager.Instance.IsInBattle) then

            CS.GameUtil.DisplayPopinfo("中毒太重无法解毒"); 
        end
        return 0;
    end
    local add = (r1.DePoison // 3) + math.random(0, 9) - math.random(0, 9)
    local depoison = CS.Jyx2.Middleware.Tools.Limit(add, 0, r2.Poison);
    return depoison;
end

--暗器
--返回值为一正数
--/ </summary>
--/ 暗器计算公式可以参考War_AnqiHurt：https://tiexuedanxin.net/forum.php?mod=viewthread&tid=394465
--/ 
--/ </summary>
--/ <param name="r1"></param>
--/ <param name="r2"></param>
--/ <param name="anqi"></param>
--/ <returns></returns>
local function hiddenWeapon(r1, r2, anqi)

    local rst = CS.Jyx2.SkillCastResult();
    --增加生命 = (暗器增加生命/a-random(5)-暗器能力*2)/3;
    --式中暗器增加生命为负值.
    --当受伤程度 = 100，a = 1;
    --当66 < 受伤程度 <= 99, a = 1;
    --当33 < 受伤程度 <= 66, a = 2;
    --当0 < 受伤程度 <= 33, a = 3;
    --当受伤程度 = 0, a = 4;
    local a = math.ceil(r2.Hurt / 33);
    if (a == 4) then a = 3 end
    local v = (anqi.AddHp // (4 - a) - math.random(0, 4) - r1.Anqi * 2) // 3;
    rst.damage = -v;
    --敌人受伤程度
    rst.hurt = -v // 4; --此处v为负值
    --当暗器带毒 > 0,
    --增加中毒程度 = [(暗器带毒 + 暗器技巧) / 2 - 抗毒能力] / 2;
    --当抗毒 = 100, 增加中毒程度 = 0.
    if (anqi.ChangePoisonLevel > 0) then

        local add = ((anqi.ChangePoisonLevel + r1.Anqi) // 2 - r2.AntiPoison) // 2;
        if (r2.AntiPoison == 100) then
            add = 0;
        end
        local poison = CS.Jyx2.Middleware.Tools.Limit(add, 0, CS.GameConst.MAX_USE_POISON);
        rst.poison = poison;
    end
    return rst;
end

--/ </summary>
--/ 战斗计算公式可以参考：https://tiexuedanxin.net/thread-365140-1-1.html
--/ 公式中的RND(n)和random(n)等都是指从0到n-1的随机整数
--/ </summary>
--/ <param name="r1">RoleInstance</param>
--/ <param name="r2">RoleInstance</param>
--/ <param name="skill">SkillCastInstance</param>
--/ <param name="blockVector">BattleBlockVector</param>
--/ <returns>SkillCastResult</returns>
function damage.GetSkillResult(r1, r2, skill, blockVector)
    local ai = Jyx2.Battle.AIManager

    local rst = CS.Jyx2.SkillCastResult(r1, r2, skill, blockVector.X, blockVector.Y);
    local magic = skill.Data:GetSkill();
    local level_index = skill.Data:GetLevel()-1;--此方法返回的是显示的武功等级，1-10。用于calMaxLevelIndexByMP时需要先-1变为数组index再使用
    level_index = skill:calMaxLevelIndexByMP(r1.Mp, level_index)+1;--此处计算是基于武功等级数据index，0-9.用于GetSkillLevelInfo时需要+1，因为用于GetSkillLevelInfo时需要里是基于GetLevel计算的，也就是1-10.
    --普通攻击
    if (magic.DamageType == 0) then

        --队伍1武学常识
        local totalWuxue = ai.BattleModel:GetTotalWuXueChangShi(r1.team);

        --队伍2武学常识
        local totalWuxue2 = ai.BattleModel:GetTotalWuXueChangShi(r2.team);

        if (r1.Mp <= magic.MpCost) then --已经不够内力释放了

            rst.damage = 1 + math.random(0, 9)
            return rst;
        end
        --总攻击力＝(人物攻击力×3 ＋ 武功当前等级攻击力)/2 ＋武器加攻击力 ＋ 防具加攻击力 ＋ 武器武功配合加攻击力 ＋我方武学常识之和
        local attack = ((r1.Attack - r1:GetWeaponProperty("Attack") - r1:GetArmorProperty("Attack")) * 3 + skill.Data:GetSkillLevelInfo(level_index).Attack) // 2 + r1:GetWeaponProperty("Attack") + r1:GetArmorProperty("Attack") + r1:GetExtraAttack(magic.Id) + totalWuxue;

        --总防御力 ＝ 人物防御力 ＋武器加防御力 ＋ 防具加防御力 ＋ 敌方武学常识之和
        local defence = r2.Defence + totalWuxue2;

        --伤害 ＝ （总攻击力－总防御×3）×2 / 3 + RND(20) – RND(20)                  （公式1）
        local v = (attack - defence * 3) * 2 // 3 + math.random(0, 19) - math.random(0, 19)

        --如果上面的伤害 < 0 则
        --伤害 ＝  总攻击力 / 10 + RND(4) – RND(4)                                            （公式2）
        if (v <= 0) then

            v = attack // 10 + math.random(0, 3) - math.random(0, 3)
        end

        --7、如果伤害仍然 < 0 则    伤害 ＝ 0
        if (v <= 0) then

            v = 0;
        else

            --8、if  伤害 > 0 then
            --    伤害＝ 伤害 ＋ 我方体力/15  ＋ 敌人受伤点数/ 20
            v = v + r1.Tili // 15 + r2.Hurt // 20;
        end

        --点、线、十字的伤害，距离就是两人相差的格子数，最小为1。
        --面攻击时，距离是两人相差的格子数＋敌人到攻击点的距离。
        local dist = r1.Pos:GetDistance(r2.Pos);
        if (skill:GetCoverType() == CS.Jyx2.SkillCoverType.RECT or skill:GetCoverType() == CS.Jyx2.SkillCoverType.RHOMBUS) then

            dist = dist + blockVector:GetDistance(r2.Pos);
        end

        --9、if 双方距离 <= 10 then
        --    伤害 ＝ 伤害×（100 -  ( 距离–1 ) * 3 ）/ 100
        --else
        --    伤害 ＝ 伤害*2 /3
        if (dist <= 10) then

            v = v * (100 - (dist - 1) * 3) // 100;
        else

            v = v * 2 // 3;
        end

        --10、if 伤害 < 1  伤害 ＝ 1
        if (v < 1) then
            v = 1;
        end

        rst.damage = v;

        --敌人受伤程度
        rst.hurt = v // 10;

        --攻击带毒
        --中毒程度＝武功等级×武功中毒点数＋人物攻击带毒
        local add = level_index * skill.Data:GetSkill().Poison + r1.AttackPoison;
        --if 敌人抗毒> 中毒程度 或 敌人抗毒> 90 则不中毒
        --敌人中毒=敌人已中毒＋ 中毒程度/15
        --if 敌人中毒> 100 then 敌人中毒 ＝100
        --if 敌人中毒<0 then 敌人中毒=0
        if (r2.AntiPoison <= add and r2.AntiPoison <= 90) then

            local poison = CS.Jyx2.Middleware.Tools.Limit(add // 15, 0, CS.GameConst.MAX_ROLE_ATK_POISON);
            rst.poison = poison;
        end

        return rst;
    elseif (magic.DamageType == 1) then --吸内

        local levelInfo = skill.Data:GetSkillLevelInfo();

        --杀伤内力逻辑
        local v = levelInfo.KillMp;
        v = v + math.random(0, 2) - math.random(0, 2)
        rst.damageMp = v;

        --吸取内力逻辑
        local addMp = levelInfo.AddMp;
        if (addMp > 0) then

            rst.addMaxMp = math.random(0, addMp // 2 - 1)-- 如果addMp=1会报错
            addMp = addMp + math.random(0, 2) - math.random(0, 2)
            rst.addMp = addMp;    
        end

        return rst;
    elseif (magic.DamageType == 2) then --用毒

        rst.poison = usePoison(r1, r2);
        return rst;
    elseif (magic.DamageType == 3) then --解毒

        rst.depoison = detoxification(r1, r2);
        return rst;
    elseif (magic.DamageType == 4) then --治疗

        local _rst = medicine(r1, r2);
        rst.heal = _rst.heal;
        rst.hurt = _rst.hurt;
        return rst;
    elseif (magic.DamageType == 5) then --暗器

        local anqi = skill.Anqi;
        local _rst = hiddenWeapon(r1, r2, anqi);
        rst.damage = _rst.damage;
        rst.hurt = _rst.hurt;
        rst.poison = _rst.poison;
        return rst;
    end
    return null;
end

return damage
