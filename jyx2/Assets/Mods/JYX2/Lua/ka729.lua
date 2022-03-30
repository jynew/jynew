if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "江湖上有几个恩怨的背后是常人所料想不到的。例如苗、胡二家的恩怨。苗人凤与胡一刀生前本是惺惺相惜的好汉，无奈胡一刀死后，他的儿子却视苗人凤为第一杀父仇人。有空时你可去为他们排解。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
