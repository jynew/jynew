Talk(15, "你又想做什麼？", "talkname15", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "晚輩斗膽向前輩討教討教．", "talkname0", 1);
    Talk(15, "好，我們來玩玩．", "talkname15", 0);
    if TryBattle(14) == false then goto label1 end;
        ModifyEvent(-2, -2, -2, -2, 100, -1, -1, -2, -2, -2, -2, -2, -2);
        SetScenceMap(-2, 1, 21, 17, 0); --开门
        jyx2_ReplaceSceneObject("", "Dynamic/Door_01", "");  
        jyx2_ReplaceSceneObject("", "Dynamic/Door_02", "");  

        LightScence();
        Talk(15, "好小子，有你的．真是長江後浪推前浪．你是來救王難姑的吧，既然打輸了你，老婆婆我就改天再尋他們的晦氣．", "talkname15", 0);
        Talk(0, "＜什麼救不救人的？我都搞糊塗了 ＞", "talkname0", 1);
        AddRepute(3);
        do return end;
::label1::
        LightScence();
        Talk(15, "看你資質挺好的，老婆婆我不想殺你，你走吧．", "talkname15", 0);
do return end;
