Talk(1120, "谢谢少侠救了我的夫君，这些药品送给你。");
AddItem(120, 20);
AddItem(121, 10);
AddItem(174, 100);
ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, -2, -2, -2, 124, -1, -1, -2, -2, -2, -2, -2, -2);
if InTeam(123) == true then goto label0 end;
    do return end;
::label0::
    jyx2_ReplaceSceneObject("", "NPC/朱云天", "1");
    Talk(1120, "夫君，出门在外，难免有跌打损伤。这个《渡城药鉴》你带上，说不定有用。");
    AddItem(124, 1);
do return end;
