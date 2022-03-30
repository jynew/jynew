if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "你知道吗？武林中一个最大的秘密是……在江南梅庄的地底下，关着一位高人，如果你将他救出来，则有很大的报酬。不过梅庄的四位庄主近来退隐江湖，不问世事，你很难渗入的。不过这四人都有个极大的缺点，那就是四人都各沉迷于一样事物，如果你能抓住他们的弱点，那就容易多了。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
