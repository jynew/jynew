if InTeam(29) == false then goto label0 end;
    Talk(28, "田伯光！你这恶贼，我跟你拼了！", "talkname28", 0);
    if TryBattle(52) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除平一指 场景30-0
		jyx2_ReplaceSceneObject("", "NPC/平一指", "");
        LightScence();
        AddEthics(-5);
        do return end;
::label0::
        Talk(0, "看先生家中摆设，似乎是位大夫。", "talkname0", 1);
        Talk(28, "小子，你到我平一指家中做什么？", "talkname28", 0);
        Talk(0, "平一指？难道你就是江湖中传说的“杀人名医”平一指。是怎么说来着，对了，“医一人，杀一人。杀一人，医一人。医人杀人一样多，蚀本生意绝不做。”", "talkname0", 1);
        Talk(28, "你是来找我求医的是吗？", "talkname28", 0);
        Talk(0, "目前小爷我身体无恙。", "talkname0", 1);
        Talk(28, "那就快滚。", "talkname28", 0);
        if AskJoin () == true then goto label2 end;
            Talk(0, "滚就滚，凶什么凶，又不是死了女儿。", "talkname0", 1);
            ModifyEvent(-2, -2, -2, -2, 300, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本300 场景30-0
            do return end;
::label2::
            Talk(0, "虽然我目前没什么病痛，但难保以后路程上会出什么差错，你就跟着我一起走吧。", "talkname0", 1);
            Talk(28, "你知道我的外号？", "talkname28", 0);
            Talk(0, "不是“杀人名医”吗？", "talkname0", 1);
            Talk(28, "知道就好。你如果要我加入你，你得先帮我去杀一个人。", "talkname28", 0);
            Talk(0, "谁？", "talkname0", 1);
            Talk(28, "万里独行田伯光那个淫虫。", "talkname28", 0);
            ModifyEvent(-2, -2, -2, -2, 302, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本302 场景30-0
            ModifyEvent(59, 0, -2, -2, 307, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本307 场景59-0
do return end;
