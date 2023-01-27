--[[
 * 金庸群侠传3D重制版
 * https:--github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 ]]--
-- 本脚本为Lua侧游戏战斗AI模块
local ai = {}
local profiler = require 'perf.profiler'

local SkillCoverType 
local dc 

local AIStrategy = {
    NORMAL = 0,
    SHORTDIST = 1
}

-- 存储当前存活角色
local aliveRoles
local aliveRolesPos

local function RefreshRolePos()

    aliveRolesPos = {}

    for _,r in pairs(aliveRoles) do
        local posint = r.Pos.X * 10000 + r.Pos.Y
        aliveRolesPos[posint] = {Key = r.Key, team = r.team}
    end
end

local inited = false

function ai.Init()

    if inited == true then
        return
    end
    math.randomseed(os.time())

    SkillCoverType = Jyx2.Battle.SkillCoverType
    dc = Jyx2.Battle.DamageCaculator

    ai.rangeLogic = CS.Jyx2.BattleManager.Instance:GetRangeLogic()
    Jyx2.Battle.RangeLogic.Init()

    ai.BattleModel = CS.Jyx2.BattleManager.Instance:GetModel()

    aliveRoles = ai.BattleModel.AliveRoles
    RefreshRolePos()

    inited = true
end

function ai.DeInit()
    SkillCoverType = nil
    dc = nil

    ai.rangeLogic = nil
    Jyx2.Battle.RangeLogic.DeInit()

    ai.BattleModel = nil

    aliveRoles = nil
    aliveRolePos = nil

    inited = false
end

ai.GetAIResult = function(callback, role)
    --print(role.Name)
    aliveRoles = ai.BattleModel.AliveRoles
    RefreshRolePos()

    --获得角色移动能力
    local moveAbility = role:GetMoveAbility()

    --行动范围
    local range = ai.rangeLogic:GetMoveRange(role.Pos.X, role.Pos.Y, moveAbility - role.movedStep, false);

    --可使用招式
    local skills = role:GetSkillsList(false);

    --AI算法：穷举每个点，使用招式，取最大收益
    local result
    local maxscore = 0;

                --profiler.start()
    --考虑吃药
    local items = ai.GetAvailableItems(role, 3); --只使用药物
    local MAX_ROLE_TILI
    local MAX_ANTIPOISON
    local role_Tili
    local role_Hp
    local role_MaxHp
    local role_Hurt
    local role_Mp
    local role_MaxMp
    local role_Poison

    -- 如果身上有物品，进行物品相关初始化
    if (#items > 0) then
        MAX_ROLE_TILI = CS.GameConst.MAX_ROLE_TILI
        MAX_ANTIPOISON = CS.GameConst.MAX_ANTIPOISON

        role_Tili = role.Tili
        role_Hp = role.Hp
        role_MaxHp = role.MaxHp
        role_Hurt = role.Hurt
        role_Mp = role.Mp
        role_MaxMp = role.MaxMp
        role_Poison = role.Poison

        local tmpblk = ai.GetFarestEnemyBlock(role, range);
        result = CS.Jyx2.AIResult()

        result.MoveX = tmpblk.X
        result.MoveY = tmpblk.Y
        result.IsRest = false
    end

    for _,item in pairs(items) do

        local score = 0;
        --使用体力药
        if (role_Tili < 0.1 * MAX_ROLE_TILI) then
            if (item.AddTili > 0) then

                score = score + math.min(item.AddTili, MAX_ROLE_TILI - role_Tili) - item.AddTili / 10;
            end
        end

        --使用生命药
        if (role_Hp < 20 or role_Hurt > 50) then
            if (item.AddHp > 0) then

                score = score + math.min(item.AddHp, role_MaxHp - role_Hp) - item.AddHp / 100;
            end
        end

        local r = -1;
        if (role_Hp < 0.2 * role_MaxHp) then
            r = 90;
        elseif (role_Hp < 0.25 * role_MaxHp) then
            r = 70;
        elseif (role_Hp < 0.33 * role_MaxHp) then
            r = 50;
        elseif (role_Hp < 0.5 * role_MaxHp) then
            r = 25;
        end

        if (CS.UnityEngine.Random.Range(0, 100) < r) then

            if (item.AddHp > 0) then

                score = score + math.min(item.AddHp, role_MaxHp - role_Hp) - item.AddHp / 100;
            end
        end

        -- 如果血量太少，吃药优先级将特别高，防止npc沉迷打怪
        if (role_Hp < 0.1 * role_MaxHp) then
            score = score * 1000
        end

        --使用内力药
        local s = -1;
        if (role_Mp < 0.2 * role_MaxMp) then
            s = 75;
        elseif (role_Mp < 0.25 * role_MaxMp) then
            s = 50;
        end

        if (CS.UnityEngine.Random.Range(0, 100) < s) then

            if (item.AddMp > 0) then

                score = score + math.min(item.AddMp, role_MaxMp - role_Mp) / 2 - item.AddMp / 100;
            end
        end

        --使用解毒药
        local m = -1;
        if (role_Poison > 0.75 * MAX_ANTIPOISON) then
            m = 60;
        elseif (role_Poison > 0.5 * MAX_ANTIPOISON) then
            m = 30;
        end
        if (CS.UnityEngine.Random.Range(0, 100) < m) then

            if (item.ChangePoisonLevel > 0) then

                score = score + math.min(item.ChangePoisonLevel, MAX_ANTIPOISON - role_Poison) - item.ChangePoisonLevel / 10;
            end
        end

        if (score > 0) then
            score = score * 1.5;--自保系数大
        end

        if (score > maxscore) then
            maxscore = score;
            result.Item = item
        end
    end

    local anqis = ai.GetAvailableItems(role, 4); --获取暗器
    --使用暗器
    for _,anqi in pairs(anqis) do

        local anqiSkillCast = CS.Jyx2.AnqiSkillCastInstance(role.Anqi, anqi);

        if (anqiSkillCast:GetStatus() == CS.Jyx2.SkillCastInstance.SkillCastStatus.OK) then

            local tmp = ai.GetMoveAndCastPos(role, anqiSkillCast, range, AIStrategy.NORMAL)

            if (tmp ~= nil and #tmp == 2 and tmp[1] ~= nil) then

                local movePos = tmp[1];
                local castPos = tmp[2];
                local score = ai.GetSkillCastResultScore(role, anqiSkillCast, movePos.X, movePos.Y, castPos.X, castPos.Y, true);
                --print("aqName"..anqiSkillCast.Anqi.Name.."score"..score.."mpos"..tmp[1]:ToInt().."cpos"..tmp[2]:ToInt())
                if (score > maxscore) then
                    maxscore = score;
                    result = CS.Jyx2.AIResult()

                    result.AttackX = castPos.X
                    result.AttackY = castPos.Y
                    result.MoveX = movePos.X
                    result.MoveY = movePos.Y
                    result.SkillCast = anqiSkillCast
                    result.IsRest = false
                end
            end
        end
    end
    if maxscore > 0 then
        if result.Item ~= nil then
        elseif result.SkillCast ~= nil then
        end
    else
        --print("No Item Use")
    end

    --使用武学
    for _,skill in pairs(skills) do

        if (skill:GetStatus() == CS.Jyx2.SkillCastInstance.SkillCastStatus.OK) then
            local tmp = ai.GetMoveAndCastPos(role, skill, range);
            if (tmp ~= nil and #tmp == 2 and tmp[1] ~= nil) then
                local movePos = tmp[1];
                local castPos = tmp[2];
                local score = ai.GetSkillCastResultScore(role, skill, movePos.X, movePos.Y, castPos.X, castPos.Y, true);
                --print("skname:"..skill.Data.Name.." score"..score.."mpos"..tmp[1]:ToInt().."cpos"..tmp[2]:ToInt())
                if (score > maxscore) then
                    maxscore = score;
                    result = CS.Jyx2.AIResult()

                    result.AttackX = castPos.X
                    result.AttackY = castPos.Y
                    result.MoveX = movePos.X
                    result.MoveY = movePos.Y
                    result.SkillCast = skill
                    result.IsRest = false
                end

                coroutine.yield(CS.UnityEngine.WaitForEndOfFrame())
            end
        end
    end

    -- 如果前面可以得出结果，就反馈
    if (result ~= nil and (result.Item ~= nil or result.SkillCast ~= nil)) then

        callback(true, result)
    end

    --否则靠近自己最近的敌人
    result = ai.MoveToNearestEnemy(role, range);
    if (result ~= nil) then

        callback(true, result)
    end

    --否则原地休息
    callback(true, ai.Rest(role))
end

ai.GetAvailableItems = function(role, itemType)

    local items = {}
    local idx = 0
    -- 如果角色是玩家战友且是玩家队伍里面的
    if (role.team == 0 and CS.Jyx2.GameRuntimeData.Instance:IsRoleInTeam(role:GetJyx2RoleId())) then

        for Key,Value in pairs(CS.Jyx2.GameRuntimeData.Instance.Items) do

            local id = Key;
            local count = Value.Item1;

            local item = Jyx2.ConfigMgr.Item[tonumber(id)];
            if (item.ItemType == itemType) then
                idx = idx + 1
                items[idx] = item
            end
        end
    else

        for _,item in pairs(role.Items) do

            local tmp = Jyx2.ConfigMgr.Item[item.Id];
            if (tmp.ItemType == itemType) then
                idx = idx + 1
                items[idx] = tmp
            end
        end
    end

    return items;
end

ai.GetNearestEnemy = function(role, currentRoles)

    local minDistance = 10000
    local targetRole
    -- 如果没有给被寻找的角色表，那就获取一份
    if currentRoles == nil then
        currentRoles = ai.BattleModel.AliveRoles
    end
    --寻找离自己最近的敌人
    for _,sp in pairs(currentRoles) do

        if (sp.team ~= role.team) then

            local distance = CS.Jyx2.BattleBlockVector.GetDistance(sp.Pos.X, sp.Pos.Y, role.Pos.X, role.Pos.Y);

            if (distance < minDistance) then

                minDistance = distance;
                targetRole = sp;
            end
        end
    end
    return targetRole;
end

ai.GetNearestEnemyBlock = function(sprite, moverange)

    local targetRole = ai.GetNearestEnemy(sprite, aliveRoles);
    if (targetRole == nil) then
        return
    end

    local minDis2 = 10000
    local movex = sprite.Pos.X
    local movey = sprite.Pos.Y
    local targetx = targetRole.Pos.X
    local targety = targetRole.Pos.Y
    --寻找离对手最近的一点
    for _,mr in pairs(moverange) do

        local distance = CS.Jyx2.BattleBlockVector.GetDistance(mr.X, mr.Y, targetx, targety);

        if (distance <= minDis2) then

            minDis2 = distance;
            movex = mr.X;
            movey = mr.Y;
        end
    end
    local rst = CS.Jyx2.BattleBlockVector()

    rst.X = movex
    rst.Y = movey
    return rst;
end

ai.GetFarestEnemyBlock = function(sprite, range)

    local sprite_team = sprite.team
    local max = 0;
    local rst
    --寻找一个点离敌人最远
    for _,r in pairs(range) do

        local min = 10000;
        -- 遍历时过滤不可达的位置，以免引发异常
        if (CS.BattleboxHelper.Instance:GetBlockData(r.X, r.Y) ~= nil) then

            for rpos,sp in pairs(aliveRolesPos) do

                local distance = math.abs(r.X - rpos//10000) + math.abs(r.Y - rpos%10000)
                if (sp.team ~= sprite_team and distance < min) then

                    min = distance;
                end
            end
        end

        if (min > max) then

            max = min;
            rst = r;
        end
    end

    return rst;
end

-- 检查格子上是否有角色
-- 返回0表示没有，返回1表示队友，返回2表示敌人
local function CheckPosRole(role, targetPosX, targetPosY, sourcePosX, sourcePosY, isAttack)

    local tpos = targetPosX * 10000 + targetPosY
    local prole = aliveRolesPos[tpos]
    -- 技能是攻击技能时
    if isAttack then
        -- 如果目标位置没有人或者是队友，则跳过
        if (prole == nil or prole.team == role.team) then
            return 0
        else
            return 2
        end
    else -- 非攻击技能时,则需要包含自己
        -- 目标格子和移动位置相同，自己包含在施放范围中
        local isCastToSelf = (targetPosX == sourcePosX and targetPosY == sourcePosY)
        if isCastToSelf then
            return 1
            -- 如果目标位置有人且是队友
        elseif (prole ~= nil and prole.team == role.team) then
            return 1
        end
    end

    return 0
end

-- 获取技能释放评分
local function GetCastScore(role, tx, ty, sx, sy, coverType, coverSize, isAttack)
    local cscore = 0
    local totaldist = 0

    if coverType == SkillCoverType.POINT then
        return 0.1 * CheckPosRole(role, tx, ty, sx, sy, isAttack), 0

    elseif coverType == SkillCoverType.RECT then
        local subscore
        -- 获取场上角色
        for rpos,r in pairs(aliveRolesPos) do
            -- 角色位置
            local rX = rpos//10000
            local rY = rpos%10000
            local dx = math.abs(rX - tx)
            local dy = math.abs(rY - ty)

            if (dx <= coverSize and dy <= coverSize) then
                subscore = CheckPosRole(role, rX, rY, sx, sy, isAttack)
                cscore = cscore + 0.1 * subscore
                totaldist = totaldist + (dx + dy) * subscore
            end
        end
        return cscore, totaldist

    elseif coverType == SkillCoverType.RHOMBUS then
        -- 获取场上角色
        for rpos,r in pairs(aliveRolesPos) do
            -- 角色位置
            local rX = rpos//10000
            local rY = rpos%10000
            local dist = math.abs(rX - tx) + math.abs(rY - ty)

            if (dist <= coverSize) then
                local subscore = CheckPosRole(role, rX, rY, sx, sy, isAttack)
                cscore = cscore + 0.1 * subscore
                totaldist = totaldist + (dist) * subscore
            end
        end
        return cscore, totaldist

    elseif coverType == SkillCoverType.LINE then
        if coverSize < 1 then
            return 0,10000
        end

        local dx = 0
        local dy = 0
        local coverx = tx
        local covery = ty
        -- 获取攻击方向
        if (tx == sx) then
            if (ty == sy) then
                return 0,10000
            elseif (ty > sy) then
                dy = 1
            elseif (ty < sy) then
                dy = -1
            end
        elseif (ty == sy) then
            if (tx > sx) then
                dx = 1
            elseif (tx < sx) then
                dx = -1
            end
        else
            return 0,10000
        end

        local subscore
        for d = 1,coverSize do
            -- 技能覆盖的坐标
            coverx = dx * d + sx
            covery = dy * d + sy
            subscore = CheckPosRole(role, coverx, covery, sx, sy, isAttack)
            cscore = cscore + 0.1 * subscore
            totaldist = totaldist + d * subscore
        end
        return cscore, totaldist

    elseif coverType == SkillCoverType.CROSS then
        if coverSize < 1 then
            return 0,10000
        end

        local subscore
        for d = 1,coverSize do
            -- 四个方向循环
            subscore = CheckPosRole(role, sx + d, sy, sx, sy, isAttack)
            cscore = cscore + 0.1 * subscore
            totaldist = totaldist + d * subscore

            subscore = CheckPosRole(role, sx, sy + d, sx, sy, isAttack)
            cscore = cscore + 0.1 * subscore
            totaldist = totaldist + d * subscore

            subscore = CheckPosRole(role, sx - d, sy, sx, sy, isAttack)
            cscore = cscore + 0.1 * subscore
            totaldist = totaldist + d * subscore

            subscore = CheckPosRole(role, sx, sy - d, sx, sy, isAttack)
            cscore = cscore + 0.1 * subscore
            totaldist = totaldist + d * subscore
        end
        return cscore, totaldist
    end

    return cscore, totaldist
end

local function CheckRoleTeam(aimRole, sourceRole, isAttack)
    if isAttack and aimRole.team ~= sourceRole.team then
        return true
    end
    if not isAttack and aimRole.team == sourceRole.team then
        return true
    end
    return false
end

ai.GetMoveAndCastPos = function(role, skillCast, moveRange, strategy)

    local rst = {nil,nil}

    local damageType = skillCast:GetDamageType()
    local coverType = skillCast:GetCoverTypeInt()
    local coverSize = skillCast:GetCoverSize()
    local castSize = skillCast:GetCastSize()

    if strategy == nil then
        -- 普通攻击伤害与距离成反比，需要寻找最近点放
        if damageType == 0 then
            strategy = AIStrategy.SHORTDIST
            -- 用毒的时候根据角色血量选择策略
        elseif damageType == 2 then
            if role.Hp > 0.5 * role.MaxHp then
                strategy = AIStrategy.SHORTDIST
            else
                strategy = AIStrategy.NORMAL
            end
        else
            strategy = AIStrategy.NORMAL
        end
    end
    -- 丢给自己的，随便乱跑一个地方丢
    if (coverType == SkillCoverType.POINT and castSize == 0 and coverSize == 0) then

        local targetBlock;
        if (role.Hp / role.MaxHp > 0.5) then

            targetBlock = ai.GetNearestEnemyBlock(role, moveRange); --生命大于50%前进
        else

            targetBlock = ai.GetFarestEnemyBlock(role, moveRange); --生命小于50%后退
        end


        rst[1] = targetBlock;
        rst[2] = targetBlock;
        return rst;
    end

    -- 缓存常用数据
    local cachedScore = {}
    local cachedDist = {}
    local isAttack = skillCast:IsCastToEnemy();
    local maxScore = 0
    local minDist = 100000
    local maxDist = 0

    -- 如果是点攻击，使用简化方案
    -- 尽量离非目标敌人远
    if coverType == SkillCoverType.POINT then
        for _,moveBlock in pairs(moveRange) do

            local sx = moveBlock.X;
            local sy = moveBlock.Y;

            local aimdist = castSize + 1
            local aimpos = -1
            -- 与全部敌人的距离
            local totaldist = 0

            for rpos,r in pairs(aliveRolesPos) do
                if CheckRoleTeam(r, role, isAttack) then
                    -- 技能施放位置与移动后位置之距离
                    local tsdist = math.abs(rpos//10000 - sx) + math.abs(rpos%10000 - sy)
                    totaldist = totaldist + tsdist
                    if tsdist < aimdist then
                        aimdist = tsdist
                        aimpos = rpos
                    end
                end
            end
            -- 如果aimpos == -1，说明这个位置打不到人
            if (aimpos ~= -1) then
                if (strategy == AIStrategy.NORMAL) then
                    -- 如果是攻击型，则远离敌人
                    if (isAttack and totaldist > maxDist) then
                        maxDist = totaldist

                        rst[1] = CS.Jyx2.BattleBlockVector(sx, sy)
                        rst[2] = CS.Jyx2.BattleBlockVector(aimpos//10000, aimpos%10000)
                        -- 非攻击型，靠近队友
                    elseif(not isAttack and totaldist < minDist) then
                        minDist = totaldist
                        rst[1] = CS.Jyx2.BattleBlockVector(sx, sy)
                        rst[2] = CS.Jyx2.BattleBlockVector(aimpos//10000, aimpos%10000)
                    end
                elseif (strategy == AIStrategy.SHORTDIST) then
                    -- 如果是攻击型，则优先靠近目标，再次远离其他敌人
                    if (isAttack and (aimdist < minDist or (aimdist == minDist and totaldist - aimdist > maxDist) )) then
                        minDist = aimdist
                        maxDist = totaldist

                        rst[1] = CS.Jyx2.BattleBlockVector(sx, sy)
                        rst[2] = CS.Jyx2.BattleBlockVector(aimpos//10000, aimpos%10000)
                        -- 非攻击型，靠近队友
                    elseif(not isAttack and totaldist < minDist) then
                        minDist = totaldist
                        rst[1] = CS.Jyx2.BattleBlockVector(sx, sy)
                        rst[2] = CS.Jyx2.BattleBlockVector(aimpos//10000, aimpos%10000)
                    end
                end
            end
        end

        return rst
    end

    --带攻击范围的，找最多人丢
    for _,moveBlock in pairs(moveRange) do

        local sx = moveBlock.X;
        local sy = moveBlock.Y;

        local castBlocks = Jyx2.Battle.RangeLogic.GetSkillCastBlocks(sx, sy, coverType, castSize, role);

        local splitFrame = 0;--分帧
        for _,castBlock in pairs(castBlocks) do

            local score = 0
            local aimdist = 1000

            local tx = castBlock.X;
            local ty = castBlock.Y;
            local castBlockInt = tx * 10000 + ty
            if (cachedScore[castBlockInt] ~= nil) then

                score = cachedScore[castBlockInt]
                aimdist = cachedDist[castBlockInt]
            else

                score, aimdist = GetCastScore(role, tx, ty, sx, sy, coverType, coverSize, isAttack)

                cachedScore[castBlockInt] = score
                cachedDist[castBlockInt] = aimdist
            end

            local csdist = math.abs(tx - sx) + math.abs(ty - sy)
            if (score > maxScore or (score == maxScore and aimdist + csdist < minDist)) then

                maxScore = score
                -- 最短距离乘以策略，只有策略不是0时，才考虑最短距离问题
                minDist = (aimdist + csdist) * strategy

                rst[1] = CS.Jyx2.BattleBlockVector(moveBlock.X, moveBlock.Y);
                rst[2] = CS.Jyx2.BattleBlockVector(castBlock.X, castBlock.Y);
            end
        end

        splitFrame = splitFrame + 1;
        if (splitFrame > 5) then--分帧

            splitFrame = 0;
            coroutine.yield(CS.UnityEngine.WaitForEndOfFrame())
        end
    end

    if (maxScore == 0) then

        rst[1] = nil;
        rst[2] = nil;
    end

    return rst;
end

local function checkTeam(isCastToEnemy, casterTeam, targetTeam)
    -- 打敌人时不打到队友
    if (isCastToEnemy and casterTeam == targetTeam) then
        return false
    end
    -- 给队友加血不能加到敌人
    if ((not isCastToEnemy) and casterTeam ~= targetTeam) then
        return false
    end
    return true
end

ai.GetSkillCastResultScore = function(caster, skill,
    movex, movey, castx, casty, isAIComputing)

    local score = 0;
    local coverSize = skill:GetCoverSize();
    local coverType = skill:GetCoverType();
    local damageType = skill:GetDamageType()
    local skillType = skill:GetType()
    local coverBlocks = ai.rangeLogic:GetSkillCoverBlocks(coverType, castx, casty, movex, movey, coverSize);

    for _,blockVector in pairs(coverBlocks) do

        local targetRole = ai.BattleModel:GetAliveRole(blockVector);
        --还活着
        if (targetRole ~= nil and not targetRole:IsDead()) then
            if checkTeam(skill:IsCastToEnemy(), caster.team, targetRole.team) then

                --local result = CS.AIManager.Instance:GetSkillResult(caster, targetRole, skill, blockVector);
                local result = dc.GetSkillResult(caster, targetRole, skill, blockVector);
                score = score + result:GetTotalScore();

                --解毒算分
                if (damageType == 3) then

                    if (targetRole.Poison > 50) then

                        score = result.depoison / 5 -- 适当降低解毒优先级
                    end
                end

                --医疗算分
                if (damageType == 4) then

                    if (skillType == typeof(CS.Jyx2.HealSkillCastInstance)) then

                        if (targetRole.Hp < 0.2 * targetRole.MaxHp) then

                            score = result.heal;
                        end
                    else

                        if (targetRole.Hp < 0.5 * targetRole.MaxHp) then

                            score = result.heal;
                        end
                    end
                end

                --用毒算分
                if (damageType == 2) then

                    score = math.min(CS.GameConst.MAX_POISON - targetRole.Poison, caster.UsePoison) * 0.1;
                    if (targetRole.Hp < 10) then

                        score = 1;
                    end
                end

                --暗器算分
                if (damageType == 5) then

                    if (score > targetRole.Hp) then

                        score = targetRole.Hp * 1.25;
                    end
                    score = score * 0.1;--暗器分值略低
                end
            end
        end
    end

    return score;
end

ai.MoveToNearestEnemy = function(sprite, range)

    local tmp = ai.GetNearestEnemyBlock(sprite, range);
    if (tmp == nil) then return end

    local rst = CS.Jyx2.AIResult()

    rst.SkillCast = nil
    rst.MoveX = tmp.X
    rst.MoveY = tmp.Y
    rst.IsRest = true --靠近对手
    return rst;
end

ai.Rest = function(sprite)

    local rst = CS.Jyx2.AIResult()

    rst.MoveX = sprite.Pos.X
    rst.MoveY = sprite.Pos.Y
    rst.IsRest = true
    return rst;
end

return ai
