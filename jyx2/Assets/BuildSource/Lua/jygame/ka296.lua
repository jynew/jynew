if InTeam(36) == true then goto label0 end;
    Talk(24, "哼！废话少说，要杀要剐随你。", "talkname24", 0);
    do return end;
::label0::
    Talk(36, "余沧海，来受死吧，今天我要你青城派在武林中消失。", "talkname36", 1);
    if TryBattle(51) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        jyx2_ReplaceSceneObject("", "NPC", "");--战斗结束，移除人物
        LightScence();
        Talk(36, "痛快！！！今天真是杀的大快人心。", "talkname36", 1);
        AddRepute(3);
do return end;
