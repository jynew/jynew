if InTeam(9) == true then goto label0 end;
    Talk(0, "前輩，別來無恙？", "talkname0", 1);
    Talk(13, "哼！你又來做什麼．", "talkname13", 0);
    do return end;
::label0::
jyx2_ReplaceSceneObject("", "NPC/zhangwuji", "1"); 
    Talk(9, "義父，你跟我們一起回中土吧．", "talkname9", 1);
    Talk(13, "你過的很好，義父就很高興了．義父還要待在這思考對付成崑的辦法，你走吧．記著，闖盪江湖千萬要提防小人，就算是自己的師父，義兄都一樣．", "talkname13", 0);
jyx2_ReplaceSceneObject("", "NPC/zhangwuji", ""); 
do return end;
