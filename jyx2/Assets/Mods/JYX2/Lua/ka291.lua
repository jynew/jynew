Talk(0, "林兄剑法不知练的怎样了。", "talkname0", 1);
Talk(36, "哈！哈！真的“辟邪剑谱”被我找到了。我终于领悟到辟邪剑法的精妙之处了。我打算上四川，诛灭他青城派。你要不要一起去？", "talkname36", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "以暴制暴，不是上策，林兄要多三思啊。", "talkname0", 1);
    Talk(36, "哼！", "talkname36", 0);
    do return end;
::label0::
    Talk(0, "好，我们就上青城杀光他全派。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(36, "你的队伍已满，我无法加入。", "talkname36", 0);
        do return end;
::label1::
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        jyx2_ReplaceSceneObject("", "NPC/林平之", "");--加入队伍
        ModifyEvent(36, 3, -2, -2, 315, -1, -1, -2, -2, -2, -2, -2, -2);
        LightScence();
        Join(36);
        AddEthics(-4);
do return end;
