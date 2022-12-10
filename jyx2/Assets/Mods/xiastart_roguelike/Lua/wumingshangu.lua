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
    Talk(0, "不管那么多，先探索下吧。。。")

    --首先隐藏北丑
    scene_api.SetActive("NPC/Beichou" , false)
end


--与南贤对话
function TalkNanXian()
    
    local nanXianFlag = scene_api.GetInt("Nanxian")
    local roleId = 73

    if(nanXianFlag == 0) then
        Talk(roleId, "年轻人，你来了");
        Talk(0, "什么情况？为什么我会来到这里，这是在做梦么？")

        Talk(roleId, "不，恭喜你来到了这个与世隔绝的地方，这里是梦境与现实之间。");
        Talk(roleId, "你必须要不断的战斗，才可以离开这里。。");

        Talk(0, "可是我为什么要战斗？")
        Talk(roleId, "因为游戏策划让你这么干..");

        Talk(0, "额，又是该死的策划，好吧，你告诉我这个游戏该怎么玩吧？")

        --告诉玩家去找北丑
        Talk(roleId, "你可以去找北丑，他会告诉你怎么玩的。");
        
        --告诉玩家北丑知道宝藏在哪
        Talk(roleId, "北丑知道宝藏在哪，你可以去找他。");
        
        --玩家说算了，别去了
        Talk(0, "算了，我不想玩了，我要去找北丑。");

        scene_api.Dark()
        scene_api.SetActive("NPC/Beichou" , true) --把北丑显示出来
        scene_api.Light()

        scene_api.SetInt("Nanxian", 1)
    elseif(nanXianFlag == 1) then
        Talk(roleId, "你找旁边这个神经病聊聊吧...")
    else
        Talk(roleId, "....")
    end
end

--北丑对话
function TalkBeichou()
    local flag = scene_api.GetInt("Beichou")
    local roleId = 74

    if(flag == 0) then
        Talk(roleId, "我什么都不知道..不要问我。")
        scene_api.SetInt("Nanxian", 2)
        scene_api.SetInt("Beichou", 1)
    elseif(flag == 1) then
        Talk(roleId, "...")
    end
end


