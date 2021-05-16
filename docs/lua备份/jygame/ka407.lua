if UseItem(124) == true then goto label0 end;
    do return end;
::label0::
    AddItem(124, -1);
    Talk(0, "老前輩，我看這蜜蜂很難馴養哦！", "talkname0", 1);
    Talk(64, "沒什麼的，再過一陣子我就會讓這百花谷中到處都是蜜蜂飛舞．", "talkname64", 0);
    Talk(0, "我這有罐玉蜂漿，你拿去試看看，會不會比較好用．", "talkname0", 1);
    ModifyEvent(-2, -2, -2, -2, -2, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 2, -2, -2, -1, -1, 408, -2, -2, -2, -2, -2, -2);--by fanyu|启动408脚本。场景20-编号2
    ModifyEvent(-2, 3, -2, -2, -1, -1, 408, -2, -2, -2, -2, -2, -2);--by fanyu|启动408脚本。场景20-编号3
do return end;
