if UseItem(134) == true then goto label0 end;
    do return end;
::label0::
    AddItem(134, -1);
    AddEthics(4);
    Talk(0, "楊兄，你快將這服下．", "talkname0", 1);
    Talk(58, "這是什麼？", "talkname58", 0);
    Talk(0, "這是生長在情花叢旁的斷腸草．我曾聽人說過，凡毒蛇出沒之處，七步之內必有解毒之藥，其他毒物，無不如此．這是天地間萬物相生相剋的至理．這斷腸草正好生長在情花樹旁，雖說此草具有劇毒，但我反覆思量，此草以毒攻毒正是情花的對頭剋星．服這毒草自是冒極大險，但反正已無藥可救，咱們就死馬當活馬醫，試它一試．", "talkname0", 1);
    Talk(58, "好，我便服這斷腸草試試，倘若無效，十六年後，請少俠告知我那苦命的妻子罷！", "talkname58", 0);
    Talk(58, "．．．啊．．．", "talkname58", 0);
    Talk(0, "楊兄怎麼了？", "talkname0", 1);
    Talk(58, "．．．．．", "talkname58", 0);
    DarkScence();
    ModifyEvent(-2, -2, -2, -2, 397, -1, -1, 6186, 6186, 6186, -2, -2, -2);--by fanyu|杨过贴图替换。场景07-编号06
    LightScence();
    Talk(58, "我楊某這條命是少俠你救回來的．", "talkname58", 0);
    Talk(0, "你身上的毒質當真都解了？還好還好，我剛真捏了把冷汗．", "talkname0", 1);
    Talk(58, "這次真謝謝少俠的幫忙，讓楊某從鬼門關回來．", "talkname58", 0);
    Talk(0, "不知楊兄今後有何打算？", "talkname0", 1);
    Talk(58, "我也不知道，但總是要保持著健康，待十六年後與我妻子相見．對了，我這裡有瓶玉蜂漿，就送給兄台好了．", "talkname58", 0);
    GetItem(124, 1);
    if AskJoin () == true then goto label1 end;
        Talk(0, "那楊兄就好好休養吧，過些時候我再來看你．", "talkname0", 1);
        do return end;
::label1::
        Talk(0, "不知楊兄是否有意與我為伴雲遊各地，一覽這五岳三川的風貌．", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(58, "你的隊伍已滿，我無法加入．", "talkname58", 0);
            do return end;
::label2::
            Talk(58, "好啊！或許旅途中會有龍兒的下落也說不定．", "talkname58", 0);
            DarkScence();
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|杨过加入队伍。场景07-编号06
            jyx2_ReplaceSceneObject("", "NPC/杨过", ""); 
            LightScence();
            Join(58);
            AddEthics(3);
do return end;
