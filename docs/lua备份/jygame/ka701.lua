if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "據說”飛雪連天射白鹿，笑書神俠倚碧鴛”這十四字各是那”十四天書”書名的第一個字．你可以此為線索去找．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 701, -1)
do return end;
