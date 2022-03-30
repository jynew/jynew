Talk(0, "老婆婆，这岛很美，您一个人住着吗？", "talkname0", 1);
Talk(15, "小子，来我岛上寻晦气的吗？", "talkname15", 0);
Talk(0, "没的事，我只是四处旅游，无意间来到这岛上的。", "talkname0", 1);
Talk(15, "说实话，你是哪一派的弟子？到这岛上来做什么？", "talkname15", 0);
Talk(0, "我无门无派，无师自通，自己四处“练练功”罢了。", "talkname0", 1);
Talk(15, "自已四处练练？那好，我老太婆就来陪你玩玩。", "talkname15", 0);
if AskBattle() == true then goto label0 end;
    ModifyEvent(-2, -2, -2, -2, 99, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    Talk(0, "好啊！闲来无事，就跟您老人家练练功增加些经验点数吧。", "talkname0", 1);
    if TryBattle(14) == false then goto label1 end;
        ModifyEvent(-2, -2, -2, -2, 100, -1, -1, -2, -2, -2, -2, -2, -2);
        SetScenceMap(-2, 1, 21, 17, 0); --打开门
        jyx2_FixMapObject("灵蛇岛开门",1);

        LightScence();
        Talk(15, "好小子，有你的。真是长江后浪推前浪。你是来救王难姑的吧，既然打输了你，老婆婆我就改天再寻他们的晦气。", "talkname15", 0);
        Talk(0, "＜什么救不救人的？我都搞糊涂了。＞", "talkname0", 1);
        AddRepute(3);
        do return end;
::label1::
        ModifyEvent(-2, -2, -2, -2, 99, -1, -1, -2, -2, -2, -2, -2, -2);
        LightScence();
        Talk(15, "看你资质挺好的，老婆婆我不想杀你，你走吧。", "talkname15", 0);
do return end;
 