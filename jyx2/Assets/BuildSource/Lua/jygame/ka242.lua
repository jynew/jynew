if UseItem(127) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(127, -1);
    Talk(35, "好一只翡翠杯！得此美酒佳器，人生更有何憾。我令狐冲先干为敬，谢谢兄弟赠酒之情。", "talkname35", 0);
    PlayAnimation(3, 5722, 5748);
    jyx2_PlayTimeline("[Timeline]ka238_悦来客栈_令狐冲喝酒", 0, false, "NPC/令狐冲");
    jyx2_Wait(0.9);
    jyx2_StopTimeline("[Timeline]ka238_悦来客栈_令狐冲喝酒");
    ModifyEvent(-2, -2, -2, -2, 243, -1, -1, 5722, 5748, 5722, -2, -2, -2);
    Talk(0, "＜令狐冲！他就是令狐冲！＞我听江湖上议论纷纷说令狐兄已遭华山派逐出师门，不知可有此事？", "talkname0", 1);
    Talk(35, "唉！令狐冲一生仗义直行，从不做违背良心之事，到头来却落至这个结果。这件事的始末也非三言两语可道尽。唉，不谈这个，咱们喝酒吧。", "talkname35", 0);
    Talk(0, "不知令狐兄今后有何打算？", "talkname0", 1);
    Talk(35, "…………", "talkname35", 0);
    if AskJoin () == true then goto label1 end;
        Talk(0, "＜这个玩物丧志的家伙，整天就只知道喝酒，跟他在一起真是浪费我找书的时间。＞啊！令狐兄，我突然想起还有重要的事要办，我先失陪了。", "talkname0", 1);
        do return end;
::label1::
        Talk(0, "我看不如这样吧。令狐兄就和我一起同游江湖共寻美酒，才不枉此生。", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(35, "你的队伍已满，我无法加入。", "talkname35", 0);
            do return end;
::label2::
            Talk(35, "这个提议甚好，咱们就一起喝尽人世间的佳酿美酒，走！对了，兄弟，告诉你一个好玩的地方，是我在华山时发现的。那地方甚为隐密，入口在华山的背面，有空我们可以去看看。", "talkname35", 0);
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -1, -2, -2);
			jyx2_ReplaceSceneObject("","NPC/令狐冲","");
            LightScence();
            Join(35);
            AddEthics(3);
do return end;
