Talk(21, "我覺得這句”銀鞍照白馬”和”颯沓如流星”連在一起方為正解．．．", "talkname21", 0);
if InTeam(38) == true then goto label0 end;
    do return end;
::label0::
    jyx2_ReplaceSceneObject("", "NPC/shipotian1", "1");--石破天出现
    Talk(38, "大哥，這馬下的雲氣，好像一團團雲霧在不斷的向前推湧．．．", "talkname38", 1);
    --Add3EventNum(-2, 4, 0, 0, 26)
   -- Add3EventNum(-2, 5, 0, 0, 26)
    --Add3EventNum(-2, 6, 0, 0, 3)
    ModifyEvent(-2, -2, -2, -2, 386, -1, -1, -2, -2, -2, -2, -2, -2);
    jyx2_ReplaceSceneObject("", "NPC/shipotian1", "");--石破天出现
do return end;
