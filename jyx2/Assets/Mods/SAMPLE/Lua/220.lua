if InTeam(11) == true then goto label0 end;
    do return end;
::label0::
    if GetFlagInt("云天比武招亲") == 1 then goto label1 end;
        Talk(22, "哟！朱师哥，您又来万烛书苑啦！");
        Talk(11, "我。。我就来看看。");
        Talk(22, "您是来看看我的武功，还是来看看我的美媳妇儿呢，这两样好像都不太欢迎您看呢。");
        Talk(11, "你这家伙说话真气人……");
        do return end;
::label1::
        Talk(22, "棠依这种绝世美人，嫁给一个大胖子还真是可惜了。");
        Talk(11, "你这嘴里就吐不出象牙来。");
do return end;
