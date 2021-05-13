Talk(61, "怎樣，想通了嗎．", "talkname61", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "不行，我還是想當個大俠，不肖與你這個人為伍．", "talkname0", 1);
    Talk(61, "真是可惜，本來還想跟你研究研究”床上功夫”呢．", "talkname61", 0);
    do return end;
::label0::
    Talk(0, "好吧，那我們就一起稱霸武林吧．反正有句名言不是說”好人早死，壞人較長命”", "talkname0", 1);
    if JudgeFemaleInTeam() == true then goto label1 end;
        Talk(61, "不行，不行，我們同伴中沒有女的我會受不了，等你找到女的再說．", "talkname61", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(61, "你的隊伍已滿，我無法加入．", "talkname61", 0);
            do return end;
::label2::
            Talk(61, "走吧，再去找一些邪惡的人來加入．", "talkname61", 0);
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/ouyangke", "");--欧阳克加入队伍
            ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 2, -2, -2, 472, -1, -1, -2, -2, -2, -2, -2, -2);
            ModifyEvent(-2, 3, -2, -2, 472, -1, -1, -2, -2, -2, -2, -2, -2);
            ModifyEvent(-2, 4, -2, -2, 472, -1, -1, -2, -2, -2, -2, -2, -2);
            ModifyEvent(-2, 5, -2, -2, 472, -1, -1, -2, -2, -2, -2, -2, -2);
            ModifyEvent(-2, 6, -2, -2, 472, -1, -1, -2, -2, -2, -2, -2, -2);
            ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(61);
            AddEthics(-6);
do return end;
