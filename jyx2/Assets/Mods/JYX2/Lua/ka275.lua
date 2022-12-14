jyx2_ReplaceSceneObject("", "Dynamic/Leave2", "");--by citydream 屏蔽地道入口（强制推进剧情）
jyx2_SwitchRoleAnimation("NPC/tubiweng", "Assets/BuildSource/AnimationControllers/StandController.controller");
jyx2_SwitchRoleAnimation("NPC/huangzhonggong", "Assets/BuildSource/AnimationControllers/StandController.controller");
Talk(0, "四位那么好的闲情逸致聚在这儿，是不是准备打麻将，插花收不收啊？", "talkname0", 1);
Talk(33, "你这臭小子，看你做了什么好事？还敢在这说风凉话。", "talkname33", 0);
Talk(34, "我早该知道天底下哪有这么好的事情，原来这一切都是你设计出来的诡计。", "talkname34", 0);
Talk(32, "你这小子，竟然利用我们所好，实在太可恶了。", "talkname32", 0);
Talk(0, "你们在那你一言我一句的，唱双簧啊！", "talkname0", 1);
Talk(32, "小子，你真是太大胆了，竟敢把任老怪给放了，看我饶不饶你！", "talkname32", 0);
Talk(0, "喂！喂！你们在说什么啊？我还没说你们偷走我那四样宝物，你们就恶人先告状。真是岂有此理，什么态度嘛……况且我哪有放走任前辈……", "talkname0", 1);
Talk(33, "大哥，别跟这小子啰嗦了，此时再也顾不得什么以老欺小的狗屁道义了。先将这小子捉上黑木崖，向东方教主请罪，否则咱们可就吃不完兜着走了。", "talkname33", 0);
Talk(34, "好，咱们一起上。", "talkname34", 0);
Talk(0, "谁怕谁！乌龟怕铁锤！四个一起上最好，省得我麻烦。", "talkname0", 1);
Talk(31, "小子你找死！", "talkname31", 0);
if TryBattle(47) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(32, "小子，你可知你放走的是谁？", "talkname32", 0);
    Talk(0, "他不是你们请来的打手吗？", "talkname0", 1);
    Talk(31, "你是真不知还是假不知，你放走了任老怪，就是与东方教主和我日月神教为敌。", "talkname31", 0);
    Talk(0, "你们是日月神教的？", "talkname0", 1);
    Talk(33, "不错，我们是奉教主之命，在此看守任老怪的，你居然利用我们的弱点救他出去。", "talkname33", 0);
    Talk(34, "别说了，得赶紧回黑木崖向东方教主报告这件事。走！", "talkname34", 0);
    DarkScence();
	jyx2_ReplaceSceneObject("", "Dynamic/Leave2", "1");--by citydream 显示地道入口
    ModifyEvent(-2, 20, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 21, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 22, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 23, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    jyx2_ReplaceSceneObject("", "NPC/danqingsheng", "");--丹青生
    jyx2_ReplaceSceneObject("", "NPC/tubiweng", "");--秃笔翁
    jyx2_ReplaceSceneObject("", "NPC/heibaizi", "");--黑白子
    jyx2_ReplaceSceneObject("", "NPC/huangzhonggong", "");--黄钟公
    ModifyEvent(-2, 24, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 12, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 13, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 14, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 15, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    LightScence();
    AddRepute(8);
do return end;
