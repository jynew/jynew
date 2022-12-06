if UseItem(134) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(134, -1);
    AddEthics(4);
    Talk(0, "杨兄，你快将这服下。", "talkname0", 1);
    Talk(58, "这是什么？", "talkname58", 0);
    Talk(0, "这是生长在情花丛旁的断肠草。我曾听人说过，凡毒蛇出没之处，七步之内必有解毒之药，其他毒物，无不如此。这是天地间万物相生相克的至理。这断肠草正好生长在情花树旁，虽说此草具有剧毒，但我反覆思量，此草以毒攻毒正是情花的对头克星。服这毒草自是冒极大险，但反正已无药可救，咱们就死马当活马医，试它一试。", "talkname0", 1);
    Talk(58, "好，我便服这断肠草试试，倘若无效，十六年后，请少侠告知我那苦命的妻子罢！", "talkname58", 0);
    Talk(58, "……啊……", "talkname58", 0);
    Talk(0, "杨兄怎么了？", "talkname0", 1);
    Talk(58, "…………", "talkname58", 0);
    DarkScence();
    ModifyEvent(-2, -2, -2, -2, 397, -1, -1, 6186, 6186, 6186, -2, -2, -2);--by fanyu|杨过贴图替换。场景07-编号06
    jyx2_SwitchRoleAnimation("NPC/杨过", "Assets/BuildSource/AnimationControllers/备份/YangguoController.controller");--尽量不要增加新trigger
    LightScence();
    Talk(58, "我杨某这条命是少侠你救回来的。", "talkname58", 0);
    Talk(0, "你身上的毒质当真都解了？还好还好，我刚真捏了把冷汗。", "talkname0", 1);
    Talk(58, "这次真谢谢少侠的帮忙，让杨某从鬼门关回来。", "talkname58", 0);
    Talk(0, "不知杨兄今后有何打算？", "talkname0", 1);
    Talk(58, "我也不知道，但总是要保持着健康，待十六年后与我妻子相见。对了，我这里有瓶玉蜂浆，就送给兄台好了。", "talkname58", 0);
    AddItem(124, 1);
    if AskJoin () == true then goto label1 end;
        Talk(0, "那杨兄就好好休养吧，过些时候我再来看你。", "talkname0", 1);
        do return end;
::label1::
        Talk(0, "不知杨兄是否有意与我为伴云游各地，一览这五岳三川的风貌。", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(58, "你的队伍已满，我无法加入。", "talkname58", 0);
            do return end;
::label2::
            Talk(58, "好啊！或许旅途中会有龙儿的下落也说不定。", "talkname58", 0);
            DarkScence();
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|杨过加入队伍。场景07-编号06
            jyx2_ReplaceSceneObject("", "NPC/杨过", ""); 
            LightScence();
            Join(58);
            AddEthics(3);
do return end;
