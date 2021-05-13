Talk(25, "公子武功，品性，小女子都很欣賞的緊，真想跟公子一起闖盪江湖．", "talkname25", 0);
Talk(0, "＜苗族女子真大膽，說話真直接．＞", "talkname0", 1);
if AskJoin () == true then goto label0 end;
    Talk(0, "姑娘好意心領了，在下一介莽夫，實不敢耽擱姑娘的青春．", "talkname0", 1);
    do return end;
::label0::
    if TeamIsFull() == false then goto label1 end;
        Talk(25, "你的隊伍已滿，我無法加入．", "talkname25", 0);
        do return end;
::label1::
        Talk(0, "能有美人相伴天涯，實乃我之榮幸．", "talkname0", 1);
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 0, -2, -2, 619, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 1, -2, -2, 619, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 2, -2, -2, 619, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 3, -2, -2, 619, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 4, -2, -2, 619, -1, -1, -2, -2, -2, -2, -2, -2);
        LightScence();
        Join(25);
do return end;
