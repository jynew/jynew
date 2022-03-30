if InTeam(9) == true then goto label0 end;
    Talk(0, "前辈，别来无恙？", "talkname0", 1);
    Talk(13, "哼！你又来做什么。", "talkname13", 0);
    do return end;
::label0::
-- jyx2_ReplaceSceneObject("", "NPC/zhangwuji", "1"); 
    Talk(9, "义父，你跟我们一起回中土吧。", "talkname9", 1);
    Talk(13, "你过的很好，义父就很高兴了。义父还要待在这思考对付成昆的办法，你走吧。记着，闯荡江湖千万要提防小人，就算是自己的师父，义兄都一样。", "talkname13", 0);
-- jyx2_ReplaceSceneObject("", "NPC/zhangwuji", ""); 
do return end;
