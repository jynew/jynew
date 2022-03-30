if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "《连城诀》一书原本是在铁骨墨萼梅念笙手上。此人有三个弟子，大弟子名叫万震山，二弟子叫言达平，三弟子叫戚长发。但没想到后来他的三个徒弟因觊觎此书，联手起来将他杀了。而万震山，言达平，戚长发三人也因互相猜忌，勾心斗角了许久。到后来这三人都陆续消失，应是与争夺此书有关，而此书最后的下落也不得而知。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
