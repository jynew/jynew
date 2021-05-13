if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "凡人練功皆有陰陽之分，故練了陽派功夫後便不能練陰派武功．不過有兩種功夫練了之後，就可調息體內陰陽之氣，使你陰，陽兩種武功都可修習之．這兩種武功分別是少林寺的羅漢伏魔神功及逍遙派的小無相功．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
