if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "道德有幾斤重？別聽南賢那老頭胡扯，滿嘴仁義道德的．你想想看，如果你為了道德不被減低，卻不肯拿箱子裡的寶物，這樣你還過得了關嗎？世上那有真的聖人，重要的是你必須能在這江湖混的下去，”玩”成你的目標．所以放手去開箱子吧，頂多以後做些善事補回來不就得了．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
