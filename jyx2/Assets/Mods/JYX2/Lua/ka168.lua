Talk(83, "见性峰乃恒山派禁地，施主勿近。", "talkname83", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "什么恒山派禁地，土地权状拿出来我瞧瞧。像你们这样据地称王的人，我最痛恨了。", "talkname0", 1);
    Talk(83, "这位施主，再不离开，可别怪我们不客气！", "talkname83", 0);
    Talk(0, "我从地理课本上知道，恒山风光明媚，鸟语花香，而见性峰更是如人间仙境。因此我特地上来观光的，你们这些臭道姑太不讲道理了。老子我就偏要看看这见性峰到底长什么样！你们能奈我何！", "talkname0", 1);
    Talk(83, "小子说什么！我看你是嵩山派派来的奸细吧！快滚回去告诉你们掌门，恒山派绝不会答应并派的！", "talkname83", 0);
    Talk(0, "说什么五四三的，听呒啦。小侠我都甘愿冒着“一见尼姑，逢赌必输”的大险来到你们这尼姑庵中，怎可败兴而归。", "talkname0", 1);
    Talk(83, "胡说八道的小子，先拿下再说。", "talkname83", 0);
    if TryBattle(23) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        ModifyEvent(-2, 2, -2, -2, 169, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本169 场景31-2
        ModifyEvent(-2, 3, -2, -2, 169, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本169 场景31-3
        ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移出角色 场景31-4
        ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移出角色 场景31-5
        jyx2_ReplaceSceneObject("","NPC/恒山弟子4","");
        jyx2_ReplaceSceneObject("","NPC/恒山弟子5","");
        ModifyEvent(-2, 6, -2, -2, 169, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本169 场景31-6
        ModifyEvent(-2, 7, -2, -2, 169, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本169 场景31-7
        LightScence();
        Talk(0, "哼！愈是不让我来，我就愈想探个究竟。", "talkname0", 1);
        AddRepute(1);
do return end;
