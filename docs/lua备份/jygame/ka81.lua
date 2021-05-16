Talk(12, "還是你這隻蝙蝠飛的快，比我這老鷹先一步到達．", "talkname12", 0);
Talk(14, "那裡，那裡，鷹王承讓了．六大派似乎已經攻進去了，這小子大概是後援的人手，我們先將他拿下再說吧．", "talkname14", 0);
Talk(12, "好啊！先暖暖我這把老骨頭也好．", "talkname12", 0);
Talk(0, "不是，不是，我是來幫．．．．．．　", "talkname0", 1);
Talk(12, "幫六大派的！我明教才不怕你們這些自居名門的傢伙．", "talkname12", 0);
if TryBattle(11) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(14, "可惡，爪子真硬，鷹王，我們先進去再說．", "talkname14", 0);
    jyx2_ReplaceSceneObject("", "NPC/weiyixiao_1", "");--韦一笑进门
    jyx2_ReplaceSceneObject("", "NPC/yintianzheng_1", ""); --殷天正进门
    DarkScence();
    ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 4, -2, -2, -2, -2, -2, 5454, 5454, 5454, -2, -2, -2);--by fanyu|改变贴图，出现人物。场景11-编号4
    ModifyEvent(-2, 5, -2, -2, -2, -2, -2, 5456, 5456, 5456, -2, -2, -2);--by fanyu|改变贴图，出现人物。场景11-编号5
    ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    LightScence();
    ScenceFromTo(29, 48, 29, 35);
    Talk(8, "魔教已然一敗塗地，再不投降，還待怎的？玄慈大師，咱們這便去毀了魔教三十三代教主的牌位吧！", "talkname8", 0);
    Talk(7, "什麼投不投降？魔教之眾，今日不能留下任何活口．除惡務盡，否則他日死灰復燃，又將為害江湖．魔崽子們！識時務的快快自我了斷，省得大爺們動手．", "talkname7", 0);
    Talk(70, "華山派和崆峒派各位，請將頂上的魔教餘孽一概誅滅了．武當派從西往東搜索，峨嵋派從東往西搜索，別讓魔教有一人漏網．崑崙派預備火種，焚燒魔教巢穴．少林弟子各取法器，誦念往生經文，替六派殉難英雄，魔教教眾超渡，化除冤孽．", "talkname70", 0);
    ScenceFromTo(29, 35, 29, 48);
    AddRepute(4);
do return end;
