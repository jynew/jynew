Talk(19, "下月十五的嵩山大會上，岳某將盡力而為．", "talkname19", 0);
Talk(0, "到時我一定去幫你．", "talkname0", 1);
if InTeam(35) == false then goto label0 end;
    Talk(35, "是啊，師父，到時我們一定會去幫你．", "talkname35", 1);
    jyx2_ReplaceSceneObject("", "NPC/linghuchong", "1");-- 
    jyx2_ReplaceSceneObject("", "NPC/linghuchong", "");-- 
::label0::
    do return end;
do return end;
