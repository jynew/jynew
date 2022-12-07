if UseItem(176) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(176, -1);
    Talk(0, "老伯，你尝尝看这是不是你说的那道菜。", "talkname0", 1);
    Talk(69, "我看看……嗯……一条是羊羔坐臀，一条是小猪耳朵，一条是小牛腰子，还有一条是獐腿肉加兔肉。肉只五种，但猪羊混咬是一般滋味，獐牛同嚼又是一般滋味，总共二十五种变化。嗯，没错，就是这种美味。", "talkname69", 0);
    Talk(0, "老伯果然了不起。", "talkname0", 1);
    Talk(69, "我就是这个馋嘴的臭脾气，一想到吃就什么也都忘了。古人说：“食指大动”，真是一点也不错。我只要见到或是闻到奇珍异味，右手的食指就会跳个不住。有一次为了贪吃误了一件大事，我一发狠，一刀砍了指头！", "talkname69", 0);
    Talk(0, "啊！", "talkname0", 1);
    Talk(69, "指头是砍了，馋嘴的性儿却砍不了。当初我就是贪吃，才让蓉儿抓住我的个性，让我传了那郭靖降龙十八掌。今日又忍不住，吃了你那“玉笛谁家听落梅”，说不得只好也传你这天下至刚的“降龙十八掌”了。", "talkname69", 0);
    Talk(0, "谢谢前辈。", "talkname0", 1);
    DarkScence();
    SetScencePosition2(30, 33);
	jyx2_MovePlayer("30-33", "Level/Dynamic");
    SetRoleFace(2);
    LightScence();
    Talk(69, "看好了，我只使一遍。", "talkname69", 0);
    PlayAnimation(0, 6228, 6254);
    jyx2_PlayTimelineSimple("[Timeline]ka462_洪七公居_洪七公演练", false);
    jyx2_Wait(2);
    jyx2_PlayTimelineSimple("[Timeline]ka462_洪七公居_降龙十八掌", false);
    jyx2_Wait(2);
    DarkScence();
    SetScencePosition2(26, 33);
	jyx2_MovePlayer("26-33", "Level/Dynamic");
    ModifyEvent(-2, -2, -2, -2, 463, -1, -1, 6122, 6122, 6122, -2, -2, -2);--by fanyu 改变贴图，启动脚本463 场景23-编号0
    ModifyEvent(-2, 1, -2, -2, -1, -1, 464, -1, -1, -1, -2, -2, -2);--by fanyu 启动脚本464 场景23-编号1
    LightScence();
    Talk(69, "小子，学了这掌法，望你用于正途。否则，老叫化我第一个将你除去。", "talkname69", 0);
    Talk(0, "谨遵师父教诲。", "talkname0", 1);
    Talk(69, "什么“师父”，我不是你师父，你烧好菜给我吃，我教你一套掌法，各不相欠。知道吗。没事就走吧，老叫化我不会再教你了。", "talkname69", 0);
    AddItem(62, 1);
do return end;
