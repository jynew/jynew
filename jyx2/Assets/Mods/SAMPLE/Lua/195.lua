Talk(1192, "买大买小，买定离手，猜中双倍返还，要试一试吗？");
if ShowSelectPanel(0, "要试一试吗？", {"是", "否"}) == 0 then goto label0 end;
    Talk(0, "我最近手头不宽裕，不好意思，下次再来吧。");
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(1192, "走，走，走，没有100两银子就不要在这挡我财路！");
        do return end;
::label1::
        if ShowSelectPanel(0, "投入多少银两？", {"一半", "全部"}) == 1 then goto label2 end;
            if ShowSelectPanel(0, "买大买小？", {"大", "小"}) == math.random(0, 1) then goto label3 end;
                Talk(1192, "哈哈哈！猜错了！");
                AddItem(174, -GetMoneyCount() // 2);
                do return end;
::label3::
                Talk(1192, "猜中了！这是你的奖励！");
                AddItem(174, GetMoneyCount() // 2);
                do return end;
::label2::
            if ShowSelectPanel(0, "买大买小？", {"大", "小"}) == math.random(0, 1) then goto label4 end;
                Talk(1192, "哈哈哈！猜错了！");
                AddItem(174, -GetMoneyCount());
                do return end;
::label4::
                Talk(1192, "猜中了！这是你的奖励！");
                AddItem(174, GetMoneyCount());
                do return end;
