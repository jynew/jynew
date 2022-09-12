Talk(0, "大叔，你煮这么多牛肉，都自己吃吗？");
Talk(91, "我和我的女儿吃，不用你管！");
Talk(0, "不考虑卖给怡麟楼？");
Talk(91, "哼！他们赚大钱，我赚小钱，我再也不卖给他们了。");
Talk(0, "市场不就是如此吗，你可以涨价，何必断货呢？");
Talk(91, "我不懂这些，反正我不再卖给怡麟楼了。");
Talk(0, "那你卖给我吧。");
Talk(91, "少废话，你要是打得过我，我可以送给你，打不过就乖乖走开。");
if AskBattle() == true then goto label0 end;
    Talk(0, "真是脑子不开窍……");
    do return end;
::label0::
    if TryBattle(90) == false then goto label1 end;
        LightScence();
        Talk(91, "我牛不三说到做到，这些牛肉都给你。");
        ModifyEvent(-2, -2, -2, -2, 95, -1, -1, -2, -2, -2, -2, -2, -2);
        AddItem(90, 1);
        do return end;
::label1::
        Talk(91, "你可以滚蛋了！");
do return end;
