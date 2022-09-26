jyx2_ReplaceSceneObject("", "Triggers/Leave", "");--by citydream 屏蔽出口（强制战斗推进剧情）
Talk(12, "还是你这只蝙蝠飞的快，比我这老鹰先一步到达。", "talkname12", 0);
Talk(14, "哪里，哪里，鹰王承让了。六大派似乎已经攻进去了，这小子大概是后援的人手，我们先将他拿下再说吧。", "talkname14", 0);
Talk(12, "好啊！先暖暖我这把老骨头也好。", "talkname12", 0);
Talk(0, "不是，不是，我是来帮…………", "talkname0", 1);
Talk(12, "帮六大派的！我明教才不怕你们这些自居名门的家伙。", "talkname12", 0);
if TryBattle(11) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(14, "可恶，爪子真硬，鹰王，我们先进去再说。", "talkname14", 0);
    jyx2_ReplaceSceneObject("", "NPC/weiyixiao_1", "");--韦一笑进门
    jyx2_ReplaceSceneObject("", "NPC/yintianzheng_1", ""); --殷天正进门
    DarkScence();
    ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 4, -2, -2, -2, -2, -2, 5454, 5454, 5454, -2, -2, -2);--by fanyu|改变贴图，出现人物。场景11-编号4
    ModifyEvent(-2, 5, -2, -2, -2, -2, -2, 5456, 5456, 5456, -2, -2, -2);--by fanyu|改变贴图，出现人物。场景11-编号5
	jyx2_ReplaceSceneObject("", "NPC/weiyixiao", "1");--韦一笑出现    
	jyx2_ReplaceSceneObject("", "NPC/yintianzheng", "1"); --殷天正出现
    ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "Triggers/Leave", "1");--by citydream 显示出口可以离开	
    LightScence();
    ScenceFromTo(29, 48, 29, 35);
	jyx2_CameraFollow("Level/NPC/fanyao");
    Talk(8, "魔教已然一败涂地，再不投降，还待怎的？玄慈大师，咱们这便去毁了魔教三十三代教主的牌位吧！", "talkname8", 0);
    Talk(7, "什么投不投降？魔教之众，今日不能留下任何活口。除恶务尽，否则他日死灰复燃，又将为害江湖。魔崽子们！识时务的快快自我了断，省得大爷们动手。", "talkname7", 0);
    Talk(70, "华山派和崆峒派各位，请将顶上的魔教余孽一概诛灭了。武当派从西往东搜索，峨嵋派从东往西搜索，别让魔教有一人漏网。昆仑派预备火种，焚烧魔教巢穴。少林弟子各取法器，诵念往生经文，替六派殉难英雄，魔教教众超渡，化除冤孽。", "talkname70", 0);
    ScenceFromTo(29, 35, 29, 48);
	jyx2_CameraFollowPlayer();
    AddRepute(4);
do return end;
