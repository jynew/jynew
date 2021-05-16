if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "在”十四天書”中有兩本書的書名與”鵰”有關，分別是”射鵰英雄傳”及”神鵰俠侶”兩本．江湖上人傳言常在塞北看見一頭大鵰在那附近出沒．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
