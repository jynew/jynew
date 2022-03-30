Talk(7, "你看看这一条的注解：“吴钩者，吴王阖庐之宝刀也。”为什么……", "talkname7", 0);
if InTeam(38) == true then goto label0 end;
    do return end;
::label0::
-- DarkScence();
-- jyx2_ReplaceSceneObject("", "NPC/shipotian3", "1");--石破天出现
-- LightScence();
    Talk(38, "大哥，我的“巨骨穴”好热……", "talkname38", 1);
    Add3EventNum(-2, 4, 0, 0, 1);
    Add3EventNum(-2, 5, 0, 0, 1);
    Add3EventNum(-2, 6, 0, 0, 1);
    ModifyEvent(-2, -2, -2, -2, 385, -1, -1, -2, -2, -2, -2, -2, -2);
    -- DarkScence();
    -- jyx2_ReplaceSceneObject("", "NPC/shipotian3", "");--石破天出现
    -- LightScence();
do return end;
