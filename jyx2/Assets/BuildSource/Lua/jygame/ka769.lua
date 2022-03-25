if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "凡人练功皆有阴阳之分，故练了阳派功夫后便不能练阴派武功。不过有两种功夫练了之后，就可调息体内阴阳之气，使你阴，阳两种武功都可修习之。这两种武功分别是少林寺的罗汉伏魔神功及逍遥派的小无相功。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
