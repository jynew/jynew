Talk(20, "小子，你擅闖我衡山，是何用意？莫非是左冷禪派來的奸細．", "talkname20", 0);
if TryBattle(28) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, -2, -2, -2, 191, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 19, -2, -2, -1, -1, 222, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 21, -2, -2, -1, -1, 222, -2, -2, -2, -2, -2, -2);
    LightScence();
    Talk(20, "回去告訴左冷禪，下月十五在嵩山召開的大會，我莫大一定到場．我倒要看看其它三派的掌門怎麼說．", "talkname20", 0);
    GetItem(69, 1);
    AddRepute(3);
do return end;
