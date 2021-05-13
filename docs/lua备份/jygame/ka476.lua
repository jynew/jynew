Talk(0, "這位小哥，我初到此地，不知這附近有那裡好玩的．", "talkname0", 1);
Talk(53, "我聽人家說，西去有個無量山，風景清幽，在下也正準備前往一遊．", "talkname53", 0);
Talk(0, "不知兄台如何稱呼，怎麼一人在外遊蕩？", "talkname0", 1);
Talk(53, "在下姓段，單名一個譽字．其實，我是從家裡面逃出來的．", "talkname53", 0);
Talk(0, "你幹麼要從家裡逃出來？", "talkname0", 1);
Talk(53, "爹爹要教我練武功，我不大想練．後來他逼的緊了，我只得逃走．", "talkname53", 0);
Talk(0, "你爹爹教你什麼武功？", "talkname0", 1);
Talk(53, "叫什麼”六脈神劍”的．", "talkname53", 0);
Talk(0, "這武功聽起來好像很厲害的樣子，你為什麼不肯學，是怕辛苦嗎？", "talkname0", 1);
Talk(53, "辛苦我才不怕呢．我從小受了佛戒，這十多年來，我學的都是儒家的仁人之心，推己及人．佛家的戒殺戒嗔，慈悲為懷．忽然爹爹要我練武，學打人殺人的法子，我自然覺得不對頭．", "talkname53", 0);
Talk(0, "可是如果你不會武功，看見有人被欺負，而你又想救他時，怎麼辦？", "talkname0", 1);
Talk(53, "那我會大大的曉諭他一番，不許他們這樣胡亂殺人．要知冤家宜解不宜結，何況兇殺鬥狠，有違國法，若叫官府知道，大大的不妥．", "talkname53", 0);
Talk(0, "＜這小子似乎有點秀逗＞", "talkname0", 1);
ModifyEvent(-2, 0, -2, -2, 477, -2, -2, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, 8, -2, -2, 477, -2, -2, -2, -2, -2, -2, -2, -2);
if AskJoin () == true then goto label0 end;
    Talk(0, "好了，不打擾兄台了．他日有緣，再一同遊山玩水吧．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "不知兄台是否願與我同行，前往那無量山一遊？", "talkname0", 1);
    if JudgeEthics(0, 40, 100) == true then goto label1 end;
        Talk(53, "嗯．．．我還有些事要辦，恐怕無法與閣下同行．", "talkname53", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(53, "你的隊伍已滿，我無法加入．", "talkname53", 0);
            do return end;
::label2::
            Talk(53, "好啊，有個人做伴，路上也有個照應．", "talkname53", 0);
            jyx2_ReplaceSceneObject("", "NPC/duanyu", "");--段誉加入队伍
            DarkScence();
            ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(53);
do return end;
