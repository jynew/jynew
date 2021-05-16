Talk(0, "前輩，還在研究棋奕啊？", "talkname0", 1);
Talk(52, "唉！老朽就是容易沉迷於棋奕，武功才不如丁春秋，而被他打得武功全失．", "talkname52", 0);
if InTeam(45) == true then goto label0 end;
    Talk(0, "前輩別擔心，我們一定會替你報仇的．", "talkname0", 1);
    do return end;
::label0::
    jyx2_ReplaceSceneObject("", "NPC/xumuhua", "1");--薛慕华
    Talk(45, "師父，你老人家安好．", "talkname45", 1);
    Talk(52, "慕華，你要盡力幫助他們．知道嗎．", "talkname52", 0);
    Talk(45, "師父，我知道．", "talkname45", 1);
    Talk(0, "前輩別擔心，我們一定會替你報仇的．", "talkname0", 1);
do return end;
