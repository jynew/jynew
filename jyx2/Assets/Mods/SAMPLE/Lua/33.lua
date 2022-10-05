if InTeam(11) == true then goto label0 end;
    Talk(132, "寿眉大师现在不出战了，主要通过修炼医术和研究药品的方式来帮助王远将军和其他正派人士。");
    do return end;
::label0::
    Talk(132, "寿眉大师现在不出战了，主要通过修炼医术和研究药品的方式来帮助王远将军和其他正派人士。");
    jyx2_ReplaceSceneObject("", "NPC/朱云天 (1)", "1");
    Talk(11, "是的，我们莫桥山庄的兄弟们多次来寿眉大师这里求医问药。");
    Talk(132, "原来是莫桥山庄的朱少侠，这里有一些药品是住持大师送给莫桥山庄的，还请收下。");
    Talk(11, "那就谢谢了！");
    jyx2_ReplaceSceneObject("", "NPC/朱云天 (1)", "");
    AddItem(30, 10);
    ModifyEvent(-2, -2, -2, -2, 34, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
