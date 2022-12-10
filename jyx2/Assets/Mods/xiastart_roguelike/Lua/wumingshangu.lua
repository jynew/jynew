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
        Talk(roleId, "当前是第 <color=red>"..level.."</color> 层，你准备好了吗？")
        
        local ret = ShowYesOrNoSelectPanel("开始下一场挑战？")
        if(ret) then
            NextBattle()
        end
    elseif(nanXianFlag == 2) then
        Talk(roleId, "先去找怪人领奖励吧!");  
    end
end

--生成对战敌人
function GenerateEnemies(level)
    if(level == 0) then
        return "171"
    elseif(level == 1) then
        return "110,121"
    end

    local ret = {}
    local totalHp = 0
    while(totalHp < level * 100) do
        local roleId = math.random(1,319)
        local role = CS.Jyx2.GameRuntimeData.Instance:GetRole(roleId)
        
        --判断是否可以加入
        if(totalHp + role.MaxHp < level * 100) then
            table.insert(ret, roleId)
            totalHp = totalHp + role.MaxHp
        elseif(#ret > 0) then
            break --至少需要有一个对手
        end
    end
    
    
    return table.concat(ret, ",")
end


--下一场战斗
function NextBattle()
    print("next battle called..")

    local level = scene_api.GetInt("Level")
    
    --构造敌人队伍
    local battleConfig = CS.Jyx2Configs.Jyx2ConfigBattle()
    
    battleConfig.Id = 9999 --随机战斗ID
    battleConfig.MapScene = "Jyx2Battle_" .. math.random(0,25) --随机挑战战斗场景
    battleConfig.Exp = 500 * (level+1)
    battleConfig.Music = 5
    battleConfig.TeamMates = 0
    battleConfig.AutoTeamMates = -1
    battleConfig.Enemies = GenerateEnemies(level)

    if(TryBattleWithConfig(battleConfig) == false) then
        Dead()
    else
        --增加层数
        scene_api.SetInt("Level", level + 1)

        Talk(74, "噫唏嘘，来找吾领取汝之奖励！")
        RestTeam()
        scene_api.SetInt("Beichou", 2)
        scene_api.SetInt("Nanxian", 2)
    end
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
        scene_api.SetInt("Beichou", 1)
    elseif(flag == 1) then
        Talk(roleId, "噫唏嘘，危乎高哉！")
        Talk(0, "还是先不要理这个怪人了……")
    elseif(flag == 2) then
        print("发奖励..")
        local ret = ShowSelectPanel(roleId, "汝欲？", {"神兵", "秘籍", "队友"})
        if(ret == 0) then
            local rndId = math.random(96,123) --对应物品ID中的装备
            AddItem(rndId,1)
        elseif(ret == 1) then
            local rndId = math.random(39,95) --对应物品ID中的秘籍
            AddItem(rndId,1)
        elseif(ret == 2) then
            local rndId = math.random(1,300) --对应角色ID
            Join(rndId)
        end
        
        scene_api.SetInt("Beichou", 3)
        scene_api.SetInt("Nanxian", 1)
    elseif(flag == 3) then
        Talk(roleId, "噫唏嘘，危乎高哉！下一关才有下一关的奖励。")
        Talk(0, "……")
    end
end


