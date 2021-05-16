Talk(85, "這位少俠，上泰山來不知有何見教？", "talkname85", 0);
Talk(0, "原來這兒就是泰山啊，古人曾說過”登泰山而小天下”今日一見果然名不虛傳．", "talkname0", 1);
Talk(85, "好說，好說．我泰山派立派以來，每年上山朝拜者數以萬計，多少人都想到我泰山派拜師學藝．我看，你也是想來這拜師的吧？", "talkname85", 0);
Talk(0, "哈！哈！我還需要拜師？有沒有搞錯啊！我看你拜我為師還差不多．不然這樣好了，今兒個我心情正好，就收你為徒．咱們也不必行什麼拜師禮了．．", "talkname0", 1);
Talk(85, "小子！好大的口氣！我倒要看看你有多少斤兩，想叫我拜你為師！", "talkname85", 0);
Talk(0, "那你就試試看！", "talkname0", 1);
if TryBattle(25) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除角色 场景29-2
    jyx2_ReplaceSceneObject("","NPC/NPC 2","");
    ModifyEvent(-2, 3, -2, -2, 175, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动175脚本 场景29-3
    ModifyEvent(-2, 4, -2, -2, 175, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动175脚本 场景29-4
    ModifyEvent(-2, 5, -2, -2, 175, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动175脚本 场景29-5
    ModifyEvent(-2, 6, -2, -2, 175, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动175脚本 场景29-6
    jyx2_ReplaceSceneObject("","GasWalls/Wall1","");
    LightScence();
    AddRepute(1);
do return end;
