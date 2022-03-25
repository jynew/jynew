if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "在江湖中行走，最要紧的使自己保持在正道之上。江湖是个是非之地，一旦稍微把持不住，就很容易误入歧途。一旦误入歧途，则正道人士将不屑于你，可能就不会加入你的行列。所以勿因小而失大，到处乱拿东西才是。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
