if InTeam(53) == true then goto label0 end;
    do return end;
::label0::
    JudgeScencePic(-2, 2, 6298, 1, 0);
        do return end;
::label1::
        jyx2_ReplaceSceneObject("", "NPC/duanyu", "1");--段誉
        Talk(53, "兄弟，我決定要留下來陪神仙姊姊，服侍她．你自己走吧．", "talkname53", 1);
        Talk(0, "段兄，這王姑娘不是你的神仙姊姊，更何況人家喜歡的是她表哥，別自作多情了．", "talkname0", 1);
        Talk(53, "兄弟，我心意已決，你自己保重吧．", "talkname53", 1);
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 3, 1, 1, 492, -1, -1, 6310, 6310, 6310, -2, -2, -2);
        Leave(53);
do return end;
