
--必须，进入场景调用
function Start()
    --快速绑定事件到物体
    scene_api.BindEvent("NPC/Nanxian", "wumingshangu.TalkNanXian")
    scene_api.BindEvent("NPC/Beichou", "wumingshangu.TalkBeichou")

    --快速绑定flag到物体控制是否显示
    --scene_api.Register("NPC/Nanxian")
    --scene_api.Register("NPC/Beichou")

    --整个场景只调用一次
    scene_api.CallOnce(FirstTimeAccessScene)
end

--必须，退出场景
function Exit()

end

--只调用一次
function FirstTimeAccessScene()
    print("第一次进入无名山谷..")
    
    Talk(0, "唔。。。好困。。。。咦，这里是什么地方？")
    Talk(0, "前面好像有人的样子，去问问看吧。。。")

    AddHpWithoutHint(0, 300) --默认加300血
    
    --首先隐藏北丑
    scene_api.SetActive("NPC/Beichou" , false)
end


--与南贤对话
function TalkNanXian()
    
    local nanXianFlag = scene_api.GetInt("Nanxian")
    local roleId = 73

    if(nanXianFlag == 0) then
        Talk(roleId, "年轻人，你来了。");
        Talk(0, "这里是什么情况？")
        
        Talk(roleId, "这里是无名谷，这里的人都是被困在这里的，我们都是被困在这里的。");
        
        Talk(0, "那我们要怎么办？")
        Talk(roleId, "只有战斗胜利，才能获得出去的机会。");

        Talk(0, "额，为什么要战斗，我们的敌人是谁？")
        Talk(roleId, "一言难尽，你准备好了就来找我吧。旁边那个怪人也是跟我一起被困在这里的，不过一直嘟嘟囔囔不知道在说什么。。。");
        Talk(roleId, "或许他跟这个困境有什么关系也说不定…… ");
        
        scene_api.Dark()
        scene_api.SetActive("NPC/Beichou" , true) --把北丑显示出来
        scene_api.Light()

        scene_api.SetInt("Nanxian", 1)
    elseif(nanXianFlag == 1) then
        
        local level = scene_api.GetInt("Level") + 1
        Talk(roleId, "当前是第 <color=red>"..level.."</color> 层，你准备好了吗？\n记住：每次挑战胜利将<color=red>自动存档</color>，不容反悔。")
        
        local ret = ShowYesOrNoSelectPanel("开始下一场挑战？")
        if(ret) then
            NextBattle()
        end
    elseif(nanXianFlag == 2) then
        Talk(roleId, "先去找怪人领奖励吧!");  
    end
end

--生成对战敌人
function GenerateEnemies(level, battleConfig)
    print(battleConfig.DynamicEnemies)
    battleConfig:InitForDynamicData()

    --最多8个敌人
    for i=0,math.min(level/2,8) do
        local role = GenerateRole(level)
        battleConfig.DynamicEnemies:Add(role)
    end
end


--根据等级，生成一个随机敌人
function GenerateRole(level)
    local roleId = math.random(1,319)
    local selectRole = CS.Jyx2.GameRuntimeData.Instance:GetRole(roleId)
    
    --等级太高了，再重新随
    while(selectRole.Level > level + 3) do
        roleId = math.random(1,319)
        selectRole = CS.Jyx2.GameRuntimeData.Instance:GetRole(roleId)
    end

    local role = selectRole:Clone()

    --把角色提升到现在关卡的等级
    while(role.Level < math.min(level, CS.GameConst.MAX_ROLE_LEVEL)) do
        role:LevelUp() 
    end

    --每10关，随机给角色增加一个技能
    for i = 0, math.max(level/10-1,0) do
        if(role.Wugongs.Count < 10) then
            local skillId = math.random(0,92)
            role:LearnMagic(skillId)    
        end
    end
    
    --角色技能提升
    for i = 0, role.Wugongs.Count - 1 do
        local skill = role.Wugongs[i]
        
        --每4关升1级技能等级（一级对应是100level）
        if(skill ~= nil and skill.Level < level * 25) then

            --随机提升技能等级，不超过上限
            skill.Level = math.min(math.random(skill.Level, level * 25), 900) 
        end
    end
    
    role:Recover()
    return role
end


--下一场战斗
function NextBattle()
    print("next battle called..")

    local level = scene_api.GetInt("Level")
    
    --动态构建一场战斗
    local battleConfig = CS.Jyx2Configs.Jyx2ConfigBattle()
    
    battleConfig.Id = 9999 --随便拟定一个战斗ID，无所谓
    battleConfig.MapScene = "Jyx2Battle_" .. math.random(0,25) --随机挑选一个战斗场景
    battleConfig.Exp = 400 * (level+1)
    battleConfig.Music = 22
    battleConfig.TeamMates = 0
    battleConfig.AutoTeamMates = -1
    GenerateEnemies(level, battleConfig)

    if(TryBattleWithConfig(battleConfig) == false) then
        Dead()
    else
        --增加层数
        scene_api.SetInt("Level", level + 1)

        Talk(74, "噫唏嘘，来找吾领取汝之奖励！")
        RestTeam()
        scene_api.SetInt("Beichou", 2)
        scene_api.SetInt("Nanxian", 2)

        --先生成奖励，防止SL
        local itemRndId = math.random(96,123) --对应物品ID中的装备
        local bookRndId = math.random(39,95) --对应物品ID中的秘籍
        
        local role = GenerateRandomTeammate(level)

        scene_api.SetInt("rndItem", itemRndId)
        scene_api.SetInt("rndBook", bookRndId)
        scene_api.SetInt("rndTeamMate", role.Key)
        
        --生成普通药品奖励
        for i=0,math.min(level/3,5) do
            local itemId = math.random(0,36) --药品
            AddItem(itemId, 1)
        end

        AutoSave()
    end
end

--根据等级生成一个随机队友
function GenerateRandomTeammate(level)

    local role = nil
    local maxLoop = 0
    while(true)do
        --已经在队伍了，或者随出来角色等级太高了，就重新随一下
        local teamMateId = math.random(1,71) --对应角色ID
        role = CS.Jyx2.GameRuntimeData.Instance:GetRole(teamMateId)
        
        if((not InTeam(teamMateId)) and (role.Level < level + 4) and (role.Level >= level - 4)) then
            print("bingo")
            break
        end

        maxLoop = maxLoop + 1
        if(maxLoop > 100) then --防止死循环
            print("maxLoop " .. maxLoop)
            break
        end
    end
    
    
    --将角色提升到现在等级
    while(role.Level < math.min(level, CS.GameConst.MAX_ROLE_LEVEL)) do
        role:LevelUp() 
    end
    
    --给角色增加生命、内力上限
    role.MaxHp = math.min(role.MaxHp + 100, CS.GameConst.MAX_ROLE_HP)
    role.MaxMp = math.min(role.MaxMp + 100, CS.GameConst.MAX_ROLE_MP)
    role:Recover()
    return role
end

function AutoSave()
    --覆盖所有存档位
    CS.LevelMaster.Instance:OnManuelSave(0);
    ShowToast("已自动存档..")
end

--北丑对话
function TalkBeichou()
    local flag = scene_api.GetInt("Beichou")
    local roleId = 74

    if(flag == 0) then
        Talk(roleId, "噫唏嘘！")
        Talk(0, "什么鬼……")
        Talk(roleId, "汝可在吾之前，不知道吾是谁？")
        Talk(0, "这个神经病看起来有点问题，我还是尽量少惹他吧。。。")
        Talk(roleId, "此间战斗仅可<color=red>自动进行</color>，好好规划汝之队伍！")
        Talk(roleId, "每完成一次战斗和奖励选择都会<color=red>自动存档</color>！所有来此大侠须直面人生，无法反悔！")
        
        Talk(0, "行吧，我试试看.. 还有什么么？")
        Talk(roleId, "噫唏嘘，危乎高哉！此间秘密不可语...")
        Talk(0, "看来还是先不要理这个怪人了……")
        
        scene_api.SetInt("Beichou", 1)
    elseif(flag == 1) then
        Talk(roleId, "噫唏嘘，危乎高哉！此间秘密不可语...")
        Talk(0, "还是先不要理这个怪人了……")
    elseif(flag == 2) then
        print("发奖励..")

        local rndItem = scene_api.GetInt("rndItem", itemRndId)
        local rndBook = scene_api.GetInt("rndBook", bookRndId)
        local rndTeamMate = scene_api.GetInt("rndTeamMate", teamMateId)

        
        local item = GetConfigTableItem(CS.Jyx2Configs.Jyx2ConfigItem, rndItem)
        local book = GetConfigTableItem(CS.Jyx2Configs.Jyx2ConfigItem, rndBook)
        local teamMate = GetConfigTableItem(CS.Jyx2Configs.Jyx2ConfigCharacter, rndTeamMate)
        
        local ret = ShowSelectPanel(roleId, "汝欲神兵、秘笈，还是队友？\n选择后将<color=red>自动存档</color>，不容反悔。", {item.Name, book.Name, teamMate.Name, "再想想"})
        if(ret == 0) then
            AddItem(rndItem,1)
        elseif(ret == 1) then
            AddItem(rndBook,1)
        elseif(ret == 2) then
            Join(rndTeamMate)
        elseif(ret == 3) then
            goto label_end
        end
        
        scene_api.SetInt("Beichou", 3)
        scene_api.SetInt("Nanxian", 1)
        AutoSave()
        ::label_end::
        
    elseif(flag == 3) then
        Talk(roleId, "噫唏嘘，危乎高哉！此间秘密不可语...")
        Talk(0, "……")
    end
end
