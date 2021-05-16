Talk(0, "韋小寶躲在那裡，快叫他出來．", "talkname0", 1);
Talk(87, "小子你瘋了，跑到我五毒教來大吼大叫的．", "talkname87", 0);
Talk(0, "妳們教主呢？是不是在跟韋小寶相好．", "talkname0", 1);
Talk(87, "瘋小子，想見教主是吧，我們就拿你去見．", "talkname87", 0);
if TryBattle(97) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 0, -2, -2, 690, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 1, -2, -2, 690, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 2, -2, -2, 690, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 3, -2, -2, 690, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 4, -2, -2, 690, -1, -1, -2, -2, -2, -2, -2, -2);
    SetScenceMap(-2, 1, 22, 26, 0);
    SetScenceMap(-2, 1, 22, 25, 2276);
    SetScenceMap(-2, 1, 22, 27, 2272);
    jyx2_ReplaceSceneObject("", "Bake/Static/Others/Door_0211", "");--打赢开门
    jyx2_ReplaceSceneObject("", "NPC/lanfenghuang", "1");--蓝凤凰
    ModifyEvent(-2, 5, 1, 1, 616, -1, -1, 6804, 6804, 6804, -2, -2, -2);
    LightScence();
    AddRepute(1);
do return end;
