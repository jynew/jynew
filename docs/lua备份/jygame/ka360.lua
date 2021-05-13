Talk(7, "你看看這一條的注解：”吳鉤者，吳王闔廬之寶刀也．”為什麼．． ．", "talkname7", 0);
if InTeam(38) == true then goto label0 end;
    do return end;
::label0::
jyx2_ReplaceSceneObject("", "NPC/shipotian3", "1");--石破天出现
    Talk(38, "大哥，我的”巨骨穴”好熱．．．", "talkname38", 1);
    --Add3EventNum(-2, 4, 0, 0, 26)
    --Add3EventNum(-2, 5, 0, 0, 26)
    --Add3EventNum(-2, 6, 0, 0, 3)
    ModifyEvent(-2, -2, -2, -2, 385, -1, -1, -2, -2, -2, -2, -2, -2);
    jyx2_ReplaceSceneObject("", "NPC/shipotian3", "");--石破天出现
do return end;
