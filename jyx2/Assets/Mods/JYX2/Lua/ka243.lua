Talk(35, "来，来！咱们再喝一杯。", "talkname35", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "＜这个玩物丧志的家伙，整天就只知道喝酒，跟他在一起真是浪费我找书的时间。＞啊！令狐兄，我突然想起还有重要的事要办，我先失陪了。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "我看不如这样吧。令狐兄就和我一起同游江湖共寻美酒，才不枉此生。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(35, "你的队伍已满，我无法加入。", "talkname35", 0);
        do return end;
::label1::
        Talk(35, "这个提议甚好，咱们就一起喝尽人世间的佳酿美酒，走！对了，兄弟，告诉你一个好玩的地方，是我在华山时发现的。那地方甚为隐密，入口在华山的背面，有空我们可以去看看。", "talkname35", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/令狐冲","");
        LightScence();
        Join(35);
        AddEthics(3);
do return end;
