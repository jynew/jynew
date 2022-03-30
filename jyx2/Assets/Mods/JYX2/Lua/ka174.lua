Talk(85, "这位少侠，上泰山来不知有何见教？", "talkname85", 0);
Talk(0, "原来这儿就是泰山啊，古人曾说过“登泰山而小天下”，今日一见果然名不虚传。", "talkname0", 1);
Talk(85, "好说，好说。我泰山派立派以来，每年上山朝拜者数以万计，多少人都想到我泰山派拜师学艺。我看，你也是想来这拜师的吧？", "talkname85", 0);
Talk(0, "哈！哈！我还需要拜师？有没有搞错啊！我看你拜我为师还差不多。不然这样好了，今儿个我心情正好，就收你为徒。咱们也不必行什么拜师礼了……", "talkname0", 1);
Talk(85, "小子！好大的口气！我倒要看看你有多少斤两，想叫我拜你为师！", "talkname85", 0);
Talk(0, "那你就试试看！", "talkname0", 1);
if TryBattle(25) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除角色 场景29-2
    jyx2_ReplaceSceneObject("","NPC/泰山弟子2","");
    ModifyEvent(-2, 3, -2, -2, 175, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动175脚本 场景29-3
    ModifyEvent(-2, 4, -2, -2, 175, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动175脚本 场景29-4
    ModifyEvent(-2, 5, -2, -2, 175, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动175脚本 场景29-5
    ModifyEvent(-2, 6, -2, -2, 175, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动175脚本 场景29-6
    LightScence();
    AddRepute(1);
do return end;
