Talk(0, "哇塞！鵰蛇大戰，精彩！．．．．咦！鵰兄似乎快不行了，看我的！　　　　　　　　　　　", "talkname0", 1);
if TryBattle(66) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 4, 1, 1, 391, -1, -1, 6194, 6194, 6194, 0, 25, 35);--by fanyu|雕胜利，变换贴图。场景07-编号4
    ModifyEvent(-2, 5, 1, 1, 392, -1, -1, 6224, 6224, 6224, 0, 24, 36);--by fanyu|雕胜利，变换贴图。场景07-编号5

    jyx2_ReplaceSceneObject("", "NPC/大雕", "");
    jyx2_ReplaceSceneObject("", "NPC/蟒蛇", "");

    LightScence();
    Talk(0, "這巨蟒還真難對付，總算把牠搞定了．鵰兄，你還好吧？", "talkname0", 1);
    Talk(104, "嘎，嘎，嘎．．．．", "talkname104", 0);
    Talk(0, "你在謝我是吧．唉！沒什麼了不起的．", "talkname0", 1);
    Talk(104, "嘎，嘎，嘎．．．．", "talkname104", 0);
    Talk(0, "這頭鵰看起來頗通靈性，像是被飼養過的，莫非洞中住有什麼高人？", "talkname0", 1);
    AddEthics(2);
    AddRepute(4);
do return end;
