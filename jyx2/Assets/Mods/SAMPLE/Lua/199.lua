if GetFlagInt("云天比武招亲") == 1 then goto label0 end;
    Talk(1194, "随便看看，想玩什么都行。");
    do return end;
::label0::
    if InTeam(11) == true then goto label1 end;
        Talk(1194, "随便看看，想玩什么都行。");
        do return end;
::label1::
        jyx2_ReplaceSceneObject("", "NPC/朱云天 (1)", "1");
        Talk(11, "岳父大人。");
        Talk(1194, "郎婿，棠依在莫桥山庄过得可好？");
        Talk(11, "棠依适应能力很强，和师兄弟们关系处得非常融洽，每天都笑容满面的。");
        Talk(1194, "那应该是你这孩子性子好，这些银子你拿着，棠依娇生惯养惯了，你还是要多宠着她。");
        Talk(11, "放心吧，谢谢岳父大人。");
        jyx2_ReplaceSceneObject("", "NPC/朱云天 (1)", "");
        AddItem(174, 3000);
        ModifyEvent(-2, -2, -2, -2, 1910, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
