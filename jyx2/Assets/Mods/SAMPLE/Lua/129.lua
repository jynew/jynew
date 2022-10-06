if InTeam(123) == true then goto label0 end;
    do return end;
::label0::
    Talk(1120, "夫君，出门在外，难免有跌打损伤。这个《渡城药鉴》你带上，说不定有用。");
    Talk(123, "夫人说的是。");
    AddItem(124, 1);
    ModifyEvent(-2, -2, -2, -2, 124, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
