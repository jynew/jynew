Talk(13, "少侠准备好要破我明教之“光明圣火阵”了吗？", "talkname13", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "我还没准备好。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "我准备好了，还请各位前辈高抬贵手。", "talkname0", 1);
    SetOneMagic(10, 0, 10, 900);
    SetOneMagic(11, 0, 50, 900);
    SetOneMagic(12, 0, 9, 900);
    SetOneMagic(13, 0, 6, 900);
    SetOneMagic(13, 1, 92, 900);
    SetOneMagic(14, 0, 8, 900);
    SetOneMagic(15, 0, 85, 900);
    AddHp(10, 200);
    AddHp(11, 200);
    AddHp(12, 200);
    AddHp(13, 200);
    AddHp(14, 200);
    AddHp(15, 200);
    if TryBattle(15) == true then goto label1 end;
        LightScence();
        Talk(13, "小兄弟似乎需要再磨练。", "talkname13", 0);
        do return end;
::label1::
        ModifyEvent(-2, 102, 1, 1, 118, -1, -1, 5318, 5318, 5318, -2, -2, -2);
        ModifyEvent(-2, 103, 1, 1, 119, -1, -1, 5324, 5324, 5324, -2, -2, -2);
        ModifyEvent(-2, 104, 1, 1, 120, -1, -1, 5342, 5342, 5342, -2, -2, -2);
        ModifyEvent(-2, 101, 1, 1, 122, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 90, 1, 1, 117, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 94, 1, 1, 121, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 91, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 92, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 93, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        jyx2_ReplaceSceneObject("", "NPC/殷天正91", "");--殷天正2离开
        jyx2_ReplaceSceneObject("", "NPC/韦一笑92", "");--韦一笑2离开
        jyx2_ReplaceSceneObject("", "NPC/杨逍93", "");--杨逍房间离开
        jyx2_ReplaceSceneObject("", "NPC/殷天正102", "1");--殷天正1圣火阵
        jyx2_ReplaceSceneObject("", "NPC/杨逍104", "1");--杨逍在圣火阵出现
        jyx2_ReplaceSceneObject("", "NPC/韦一笑103", "1");--韦一笑圣火阵
        LightScence();
        Talk(12, "果然是英雄出少年，我们这些老骨头都不行了。", "talkname12", 0);
        Talk(14, "今后武林中，就是你们这些年轻人的天下了。", "talkname14", 0);
        Talk(0, "是各位前辈承让了。", "talkname0", 1);
        Talk(13, "就遵照我们的约定，《倚天屠龙记》一书该为少侠所有。", "talkname13", 0);
        Talk(0, "谢谢各位前辈。在经过了这么多波折才拿到此书，虽说辛苦，但也从中学到不少东西。世上的好坏人真的很难去界定，名门正派的人，外表有羊皮披挂着，反倒是更容易去为恶。", "talkname0", 1);
        Talk(10, "经过了这些，你的江湖历练又增长了不少。希望你在其它的旅途上也更能顺利。", "talkname10", 0);
        Talk(0, "好了，我还要去忙别的了。有空我会再回来的。", "talkname0", 1);
        AddItem(155, 1);
        AddRepute(10);
do return end;
