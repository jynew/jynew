if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "第三个宝藏是大燕宝藏。相传是大燕国为了以后复国用所埋藏的。传说当年负责埋藏的军队是顺着黄河而下，所以一般的猜测是埋藏在“黄河沿岸”。照我的研究是在……（２０３，２４３）附近。", "talkname74", 0);--数字反着说
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
