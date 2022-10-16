local items = {
  [0] = 74,
  [1] = 123,
  [2] = 110,
  [3] = 81,
  [4] = 180,
  [5] = 161,
  [6] = 121,
  [7] = 120,
  [8] = 72,
  [9] = 40,
  [10] = 38,
  [11] = 37,
  [12] = 30,
  [13] = 63,
  [14] = 71,
  [15] = 194,
  [16] = 193,
  [17] = 192,
  [18] = 191,
  [19] = 190
}

Talk(1193, "公子来抽奖吗？100两银子一次，有机会获得我们无际坊收藏的各种稀世珍宝哟。");
if ShowSelectPanel(0, "要抽一抽吗？", {"是", "否"}) == 0 then goto label0 end;
    Talk(0, "我最近手头不宽裕，不好意思，下次再来吧。");
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(1193, "走，走，走，没有100两银子就不要在这挡我财路！");
        do return end;
::label1::
        AddItem(174, -100);
        Talk(1193, "恭喜你，抽中了一件珍宝！");
        AddItem(items[GetImbalancedRandomInt(0, #items)], 1);
do return end;


