if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "据说“飞雪连天射白鹿，笑书神侠倚碧鸳”这十四字各是那“十四天书”书名的第一个字。你可以此为线索去找。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
