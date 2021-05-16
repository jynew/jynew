if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "江湖上有幾個恩怨的背後是常人所料想不到的．例如苗，胡二家的恩怨．苗人鳳與胡一刀生前本是惺惺相惜的好漢，無奈胡一刀死後，他的兒子卻視苗人鳳為第一殺父仇人．有空時你可去為他們排解．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
