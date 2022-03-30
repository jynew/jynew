if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "行走江湖，难免有些病痛，所以伙伴中能有个懂得医术的人最好。武林中有三大名医，分别是蝶谷医仙胡青牛，杀人名医平一指，阎王敌薛神医。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
