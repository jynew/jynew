Talk(0, "諸位大師，我想進少林寺參觀一下，可以嗎？", "talkname0", 1);
Talk(96, "請施主先將兵刃留下，待下山前我們再歸還予你．", "talkname96", 0);
Talk(0, "對了，這是少林寺千年下來的傳統，以表示對少林寺的尊重．好，我就將兵刃．．＜等等，武俠小說中的主角向來都不吃這一套的，若我這麼聽話，那戲還有什麼好看的．．．＞等會，這兵刃向來不與我離身，我不能將兵刃放下．", "talkname0", 1);
Talk(96, "那就請施主下山．", "talkname96", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "好，好，我下回再來．", "talkname0", 1);
    ModifyEvent(-2, 3, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-3
    ModifyEvent(-2, 4, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-4
    ModifyEvent(-2, 5, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-5
    ModifyEvent(-2, 6, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-6
    do return end;
::label0::
    Talk(0, "可是我還是想進去看看，對不住了．", "talkname0", 1);
    if TryBattle(79) == true then goto label1 end;
        ModifyEvent(-2, 3, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-3
        ModifyEvent(-2, 4, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-4
        ModifyEvent(-2, 5, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-5
        ModifyEvent(-2, 6, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-6

        LightScence();
        Talk(96, "請施主下山．", "talkname96", 0);
        Talk(0, "可是我還是想進去看看，對不住了．", "talkname0", 1);
        do return end;
::label1::
        ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-3
        ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-4
        ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-5
        ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-6
        jyx2_ReplaceSceneObject("","GasWalls/Wall1","");
        LightScence();
        AddRepute(2);
do return end;
