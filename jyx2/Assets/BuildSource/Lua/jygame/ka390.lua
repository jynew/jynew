Talk(0, "哇塞！雕蛇大战，精彩！……咦！雕兄似乎快不行了，看我的！", "talkname0", 1);
if TryBattle(66) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 4, 1, 1, 391, -1, -1, 6194, 6194, 6194, 0, 25, 35);--by fanyu|雕胜利，变换贴图。场景07-编号4
    ModifyEvent(-2, 5, 1, 1, 392, -1, -1, 6224, 6224, 6224, 0, 24, 36);--by fanyu|雕胜利，变换贴图。场景07-编号5
    jyx2_SwitchRoleAnimation("NPC/蟒蛇", "Assets/BuildSource/AnimationControllers/Viper_jyx2_dead.controller");--蟒蛇动作
    jyx2_SwitchRoleAnimation("NPC/大雕", "Assets/BuildSource/AnimationControllers/Eagle_idle.controller");--大雕动作
    LightScence();
    Talk(0, "这巨蟒还真难对付，总算把它搞定了。雕兄，你还好吧？", "talkname0", 1);
    Talk(104, "嘎，嘎，嘎……", "talkname104", 0);
    Talk(0, "你在谢我是吧。唉！没什么了不起的。", "talkname0", 1);
    Talk(104, "嘎，嘎，嘎……", "talkname104", 0);
    Talk(0, "这头雕看起来颇通灵性，像是被饲养过的，莫非洞中住有什么高人？", "talkname0", 1);
    AddEthics(2);
    AddRepute(4);
do return end;
