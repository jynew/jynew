Talk(19, "下月十五的嵩山大会上，岳某将尽力而为。", "talkname19", 0);
Talk(0, "到时我一定去帮你。", "talkname0", 1);
if InTeam(35) == false then goto label0 end;
    --jyx2_ReplaceSceneObject("", "NPC/令狐冲", "1");--原作不需要显示令狐冲，是否有必要？
    Talk(35, "是啊，师父，到时我们一定会去帮你。", "talkname35", 1); 
    --jyx2_ReplaceSceneObject("", "NPC/令狐冲", "");-- 
::label0::
    do return end;
do return end;
