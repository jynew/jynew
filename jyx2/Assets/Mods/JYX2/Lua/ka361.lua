Talk(21, "我觉得这句“银鞍照白马”和“飒沓如流星”连在一起方为正解……", "talkname21", 0);
if InTeam(38) == true then goto label0 end;
    do return end;
::label0::
    -- DarkScence();
    -- jyx2_ReplaceSceneObject("", "NPC/shipotian1", "1");--石破天出现
    -- LightScence();
    Talk(38, "大哥，这马下的云气，好像一团团云雾在不断的向前推涌……", "talkname38", 1);
    Add3EventNum(-2, 4, 0, 0, 1);
    Add3EventNum(-2, 5, 0, 0, 1);
    Add3EventNum(-2, 6, 0, 0, 1);
    ModifyEvent(-2, -2, -2, -2, 386, -1, -1, -2, -2, -2, -2, -2, -2);
    -- DarkScence();
    -- jyx2_ReplaceSceneObject("", "NPC/shipotian1", "");--石破天出现
    -- LightScence();
do return end;
