Talk(0, "诸位大师，我想进少林寺参观一下，可以吗？", "talkname0", 1);
Talk(96, "请施主先将兵刃留下，待下山前我们再归还予你。", "talkname96", 0);
Talk(0, "对了，这是少林寺千年下来的传统，以表示对少林寺的尊重。好，我就将兵刃……＜等等，武侠小说中的主角向来都不吃这一套的，若我这么听话，那戏还有什么好看的……＞等会，这兵刃向来不与我离身，我不能将兵刃放下。", "talkname0", 1);
Talk(96, "那就请施主下山。", "talkname96", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "好，好，我下回再来。", "talkname0", 1);
    ModifyEvent(-2, 3, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-3
    ModifyEvent(-2, 4, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-4
    ModifyEvent(-2, 5, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-5
    ModifyEvent(-2, 6, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-6
    do return end;
::label0::
    Talk(0, "可是我还是想进去看看，对不住了。", "talkname0", 1);
    if TryBattle(79) == true then goto label1 end;
        ModifyEvent(-2, 3, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-3
        ModifyEvent(-2, 4, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-4
        ModifyEvent(-2, 5, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-5
        ModifyEvent(-2, 6, -2, -2, 522, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-522。场景28-6

        LightScence();
        Talk(96, "请施主下山。", "talkname96", 0);
        Talk(0, "可是我还是想进去看看，对不住了。", "talkname0", 1);
        do return end;
::label1::
        ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-3
        ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-4
        ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-5
        ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-6
		jyx2_ReplaceSceneObject("", "NPC/少林弟子3", "");
		jyx2_ReplaceSceneObject("", "NPC/少林弟子4", "");
		jyx2_ReplaceSceneObject("", "NPC/少林弟子5", "");
		jyx2_ReplaceSceneObject("", "NPC/少林弟子6", "");
        LightScence();
        AddRepute(2);
do return end;
