Talk(0, "老婆婆，這島很美，您一個人住著嗎？", "talkname0", 1);
Talk(15, "小子，來我島上尋晦氣的嗎？", "talkname15", 0);
Talk(0, "沒的事，我只是四處旅遊，無意間來到這島上的．", "talkname0", 1);
Talk(15, "說實話，你是那一派的弟子？到這島上來做什麼？", "talkname15", 0);
Talk(0, "我無門無派，無師自通，自己四處”練練功”罷了．", "talkname0", 1);
Talk(15, "自已四處練練？那好，我老太婆就來陪你玩玩．", "talkname15", 0);
if AskBattle() == true then goto label0 end;
    ModifyEvent(-2, -2, -2, -2, 99, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    Talk(0, "好啊！閒來無事，就跟您老人家練練功增加些經驗點數吧．", "talkname0", 1);
    if TryBattle(14) == false then goto label1 end;
        ModifyEvent(-2, -2, -2, -2, 100, -1, -1, -2, -2, -2, -2, -2, -2);
        SetScenceMap(-2, 1, 21, 17, 0); --打开门
        jyx2_ReplaceSceneObject("", "Dynamic/Door_01", "");  
        jyx2_ReplaceSceneObject("", "Dynamic/Door_02", "");  

        LightScence();
        Talk(15, "好小子，有你的．真是長江後浪推前浪．你是來救王難姑的吧，既然打輸了你，老婆婆我就改天再尋他們的晦氣．", "talkname15", 0);
        Talk(0, "＜什麼救不救人的？我都搞糊塗了 ＞", "talkname0", 1);
        AddRepute(3);
        do return end;
::label1::
        ModifyEvent(-2, -2, -2, -2, 99, -1, -1, -2, -2, -2, -2, -2, -2);
        LightScence();
        Talk(15, "看你資質挺好的，老婆婆我不想殺你，你走吧．", "talkname15", 0);
do return end;
 