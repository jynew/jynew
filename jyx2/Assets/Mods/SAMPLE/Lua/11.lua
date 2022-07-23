Talk(1, "来来来，喝酒…");
Talk(0, "这酒不够香，我不喝。");
Talk(1, "这酒不香？哈哈…那你说什么酒香？");
Talk(0, "天下唯有玉石榴酒香。");
Talk(1, "哈哈哈哈…你小子还懂得真多，玉石榴这种塞外酒很难弄到，不过实不相瞒，我这儿还真有。");
Talk(113, "这家伙答应我戒酒，居然还背着我藏了这等好酒…");
Talk(0, "那可以借我喝点吗？");
Talk(1, "小弟弟，这个酒可不是谁想喝就能喝的哟？");
Talk(0, "要怎么才能借我喝点？");
Talk(1, "石榴只上高枝头，说简单点，你要是打得过我，我就送你你一瓶，哈哈哈！");
if AskBattle() == true then goto label0 end;
    Talk(0, "我还是不喝了吧……");
    do return end;
::label0::
    Talk(0, "那就来比试比试吧！");
    if TryBattle(0) == false then goto label1 end;
        LightScence();
        Talk(1, "小子，你还有两下子，我何某说到做到，这瓶子酒你拿去，地地道道的塞外玉石榴。");
        AddItem(69, 1);
        do return end;
::label1::
        LightScence();
        Talk(1, "看来你还没有口福，哈哈哈哈哈。");
do return end;
