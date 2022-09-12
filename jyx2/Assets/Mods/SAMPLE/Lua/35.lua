if InTeam(11) == true then goto label0 end;
    Talk(30, "真没意思，每天练功，又不能出去打架。");
    Talk(0, "哈哈，你这个小师傅还真有意思。");
    do return end;
::label0::
    Talk(30, "真没意思，每天练功，又不能出去打架。");
    Talk(0, "哈哈，你这个小师傅还真有意思。");
    Talk(30, "练功不就是为了锄奸惩恶嘛，你看我这肱二头肌练得多壮了，完全用不上啊。");
    Talk(0, "你这躁动的性格，还真不适合当和尚。");
    Talk(30, "哎呀，我们寿眉大师自从五年前那次血战之后，再也不带我们出去了，早知道我应该加入莫桥山庄或者钟鸣阁才对。");
    Talk(11, "我们莫桥山庄现在正忙着调查莫掌门的死因，正需要一些帮手。");
    Talk(0, "需要打架吗？");
    Talk(11, "这个……");
    Talk(0, "调查过程中肯定会遇到些磕磕碰碰，打架是难免的。");
    Talk(30, "那我来帮助你们吧，我去跟师父说一声就是！");
    if AskJoin() == true then goto label1 end;
        Talk(0, "那个，我们现在还没到要打架的时候，到时候我再来叫你啊。");
        ModifyEvent(-2, -2, -2, -2, 36, -1, -1, -2, -2, -2, -2, -2, -2);
        do return end;
::label1::
      if TeamIsFull() == false then goto label2 end;
            Talk(30, "你的队伍已满，我无法加入。");
            do return end;
::label2::
            Talk(0, "这样也好，多了一个帮手。");
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/虚寂", "");
            LightScence();
            Join(30);
            ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
