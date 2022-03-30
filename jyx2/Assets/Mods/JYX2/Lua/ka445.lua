Talk(61, "怎样，想通了吗。", "talkname61", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "不行，我还是想当个大侠，不肖与你这个人为伍。", "talkname0", 1);
    Talk(61, "真是可惜，本来还想跟你研究研究“床上功夫”呢。", "talkname61", 0);
    do return end;
::label0::
    Talk(0, "好吧，那我们就一起称霸武林吧。反正有句名言不是说“好人早死，坏人较长命”。", "talkname0", 1);
    if JudgeFemaleInTeam() == true then goto label1 end;
        Talk(61, "不行，不行，我们同伴中没有女的我会受不了，等你找到女的再说。", "talkname61", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(61, "你的队伍已满，我无法加入。", "talkname61", 0);
            do return end;
::label2::
            Talk(61, "走吧，再去找一些邪恶的人来加入。", "talkname61", 0);
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/欧阳克", "");--欧阳克加入队伍
            jyx2_ReplaceSceneObject("", "NPC/欧阳克婢女", "");
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
