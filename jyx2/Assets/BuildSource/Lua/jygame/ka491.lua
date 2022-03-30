if InTeam(53) == true then goto label0 end;
    do return end;
::label0::
    if JudgeScenePic(-2, 2, 6298, 1, 0) then goto label1 end;
        do return end;
::label1::
        Talk(53, "兄弟，我决定要留下来陪神仙姊姊，服侍她。你自己走吧。", "talkname53", 1);
        Talk(0, "段兄，这王姑娘不是你的神仙姊姊，更何况人家喜欢的是她表哥，别自作多情了。", "talkname0", 1);
        Talk(53, "兄弟，我心意已决，你自己保重吧。", "talkname53", 1);
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 3, 1, 1, 492, -1, -1, 6310, 6310, 6310, -2, -2, -2);
        jyx2_ReplaceSceneObject("", "NPC/段誉", "1");--段誉
        Leave(53);
do return end;
