if InTeam(11) == true then goto label0 end;
    Talk(30, "真没意思，每天练功，又不能出去打架。");
    Talk(0, "哈哈，你这个小师傅还真有意思。");
    do return end;
::label0::
    Talk(30, "你来了，快来和我练一练！");
    Talk(0, "喂，我都没说同意啊……");
    if TryBattle(31) == true then goto label1 end;
        Talk(30, "小子功夫不怎么样嘛，不过好久和真人没练了，打起来还真是爽。");
        Talk(0, "好吧好吧，你功夫不错，等我有空再来找你切磋。");
        do return end;
::label1::
        Talk(30, "没想到你功夫还不错。");
        Talk(0, "你武功也不错，但是只是光有直来直去的力道，还缺少了点对武学奥义的思考。");
        Talk(30, "哼！都怪我实战经验太少了，哪来的机会去思考。我们寿眉大师自从五年前那次血战之后，再也不带我们出去了，早知道我应该加入莫桥山庄或者钟鸣阁才对。");
        jyx2_ReplaceSceneObject("", "NPC/朱云天", "1");
        Talk(11, "你说莫桥山庄吗？我就是莫桥山庄的，我们山庄现在正忙着调查莫掌门的死因，正需要一些帮手。");
        Talk(0, "调查莫掌门的死因？听上去也是善义之事，需要打架吗？");
        Talk(11, "这个……");
        Talk(0, "调查过程中肯定会遇到些磕磕碰碰，打架是难免的。");
        Talk(30, "那太好了，我来帮助你们吧，我去跟师祖说一声就是！");
        if AskJoin() == true then goto label2 end;
            Talk(0, "那个，我们现在还没到要打架的时候，到时候我再来叫你啊。");
            jyx2_ReplaceSceneObject("", "NPC/朱云天", "");
            ModifyEvent(-2, -2, -2, -2, 36, -1, -1, -2, -2, -2, -2, -2, -2);
        do return end;
::label2::
        if TeamIsFull() == false then goto label3 end;
            Talk(30, "你的队伍已满，我无法加入。");
            jyx2_ReplaceSceneObject("", "NPC/朱云天", "");
            do return end;
::label3::
            Talk(0, "这样也好，多了一个帮手。");
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/虚寂", "");
            jyx2_ReplaceSceneObject("", "NPC/朱云天", "");
            LightScence();
            Join(30);
            ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;