Talk(170, "四二，你们去调查莫掌门的死因，一路上可能会遭遇强敌。我传你们一手自创的暗器手法，这套手法叫<color=orange>摩柯观音泪</color>，非天资聪颖不能学会。");
Talk(170, "你们静下心来默记口诀：<color=orange>一滴真珠万斛愁，不知何处是瀛洲。若教化作莲花瓣，犹在人间更可留……</color>");
Talk(170, "你们记住了吗？接下来我演示一遍，你们跟着模仿。");
jyx2_PlayTimelineSimple("[Timeline]7_粟兰传功", false, "NPC/粟兰");
jyx2_Wait(7.8);
Talk(80, "娘，我学会了。");
LearnMagic2(80, 72, 1);
Talk(170, "好，孺子可教也。");
if JudgeIQ(0, 80, 100) == true then goto label0 end;
    Talk(0, "我资质愚钝，看来是无缘学会了。");
    ModifyEvent(-2, 3, -2, -2, 721, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    Talk(0, "多谢粟谷主。");
    LearnMagic2(0, 72, 1);
    AddItem(73, 1);
    ModifyEvent(-2, 3, -2, -2, 721, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
