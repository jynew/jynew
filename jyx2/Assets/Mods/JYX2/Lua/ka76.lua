Talk(11, "不知小兄弟是哪派的高徒，其他五派的人呢？", "talkname11", 0);
Talk(0, "在下并非六大派的人，此次前来明教是想打听一本书的下落。", "talkname0", 1);
Talk(11, "是《倚天屠龙记》吧？", "talkname11", 0);
Talk(0, "江湖上传言果然非假，《倚天屠龙记》一书果然在明教中。不知前辈可否借小弟一用。", "talkname0", 1);
Talk(11, "借你一用。哼！你没将明教光明左使杨逍瞧在眼里，是不是仗着我明教大难之际来捡现成便宜？", "talkname11", 0);
Talk(0, "事情并非如此，实在是这本书对我十分重要，我非得到它不可。", "talkname0", 1);
Talk(11, "那我就先把你解决再说，免得六大派攻来后被你混水摸鱼了。", "talkname11", 0);
Talk(0, "我并不是来与明教为难的，杨左使还是留点气力对付六大派好了。", "talkname0", 1);
Talk(11, "对付你倒也不用花费什么力气。出招吧！", "talkname11", 0);
if TryBattle(9) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    DarkScence();
    ModifyEvent(-2, 4, -2, -2, -1, -1, -1, 5308, 5308, 5308, -2, -2, -2);--by fanyu|战斗后人物贴图替换。场景12-编号4
    ModifyEvent(-2, 5, -2, -2, -1, -1, -1, 5310, 5310, 5310, -2, -2, -2);--by fanyu|战斗后人物贴图替换。场景12-编号5
    ModifyEvent(-2, 6, -2, -2, -1, -1, -1, 5312, 5312, 5312, -2, -2, -2);--by fanyu|战斗后人物贴图替换。场景12-编号6
    ModifyEvent(-2, 7, -2, -2, -1, -1, -1, 5314, 5314, 5314, -2, -2, -2);--by fanyu|战斗后人物贴图替换。场景12-编号7
    ModifyEvent(-2, 8, -2, -2, -1, -1, -1, 5312, 5308, 5312, -2, -2, -2);--by fanyu|战斗后人物贴图替换。场景12-编号8
    ModifyEvent(-2, 9, -2, -2, -1, -1, -1, 5310, 5310, 5310, -2, -2, -2);--by fanyu|战斗后人物贴图替换。场景12-编号9
    ModifyEvent(-2, 10, 1, 1, -1, -1, -1, 5298, 5298, 5298, -2, -2, -2);--by fanyu|战斗后成昆出现。场景12-编号10
    jyx2_ReplaceSceneObject("", "NPC/chengkun", "1");--成昆出现
    LightScence();
    Talk(0, "杨左使，我就说了嘛，跟我打很耗力气的。", "talkname0", 1);
    Talk(11, "你……", "talkname11", 0);
    Talk(18, "哈！哈！哈！打的好，打的好。", "talkname18", 0);
    Talk(0, "哪里来的贼秃在那鬼吼鬼叫的。", "talkname0", 1);
    Talk(11, "你是谁？是怎么混进我明教的？", "talkname11", 0);
    Talk(18, "你问我成昆怎么进来的？当然是大摇大摆走进来的。还多亏了这小朋友，替我省下一番功夫。", "talkname18", 0);
    Talk(11, "你是混元霹雳手成昆，谢法王的师父？", "talkname11", 0);
    Talk(18, "没错，我就是那头笨狮的师父。拜他的鲁莽所赐，你们和六大派结下了这么多梁子。今天起明教就将化为灰烬，也了结了我多年来的心愿。", "talkname18", 0);
    Talk(0, "要是我猜的没错，这中间的一切都是你苦心安排的。", "talkname0", 1);
    Talk(18, "你这小子倒还蛮聪明的嘛。没错，激起谢逊的愤怒，让他到处乱杀人，留下我的名字好激我现身，到最后再故意让江湖人知道其实是明教谢逊所为，引起江湖各大派与明教间仇杀，这一切都是我精心计划的。", "talkname18", 0);
    Talk(11, "你为何要如此做？", "talkname11", 0);
    Talk(18, "要问就去问你们前任教主阳顶天。我原本一桩美好的姻缘却被他活生生拆散了，明明是我爱妻，只因他当上了魔教的大头子，就将我爱妻霸占了去。因此我心中便立下重誓：只要我成昆还有一口气在，定要让魔教永无宁日。我立下此誓已有二十余年，今日方见大功告成。哈哈！现下六大派应该已攻下总坛光明顶了吧。", "talkname18", 0);
    Talk(11, "什么！那我得快点赶回光明顶。", "talkname11", 0);
    Talk(18, "不用赶回去死了，你死在这就可以了。", "talkname18", 0);
    Talk(0, "喂！老秃驴，你们这些恶人怎么每次都喜欢将自己的坏勾当讲出来。是不是以为我们这些好人都会死掉，无法揭发你的阴谋？杨左使，你先赶回光明顶，这个秃驴交给我好了，让他尝尝多嘴的后果。", "talkname0", 1);
    Talk(18, "哼！就凭你。", "talkname18", 0);
    Talk(11, "你……你……真的行吗？", "talkname11", 0);
    Talk(0, "别忘了，如果连我也不行的话，那你可能更惨哦！快去吧，我随后就到。", "talkname0", 1);
    AddRepute(4);
    if TryBattle(10) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        DarkScence();
        jyx2_ReplaceSceneObject("", "NPC/yangxiao", "");--杨逍离开去光明
        ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|人物消失。场景12-编号4
        ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|人物消失。场景12-编号5
        ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|人物消失。场景12-编号6
        ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|人物消失。场景12-编号7
        ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|人物消失。场景12-编号8
        ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|人物消失。场景12-编号9
        ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|人物消失。场景12-编号0
        ModifyEvent(-2, 10, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|人物消失。场景12-编号10
         jyx2_ReplaceSceneObject("", "NPC/NPC1", "");--明教教徒离开去光明顶
         jyx2_ReplaceSceneObject("", "NPC/NPC5", "");--明教教徒离开去光明顶
         jyx2_ReplaceSceneObject("", "NPC/NPC6", "");--明教教徒离开去光明顶
         jyx2_ReplaceSceneObject("", "NPC/NPC7", "");--明教教徒离开去光明顶
         jyx2_ReplaceSceneObject("", "NPC/NPC8", "");--明教教徒离开去光明顶
         jyx2_ReplaceSceneObject("", "NPC/NPC9", "");--明教教徒离开去光明顶
        jyx2_ReplaceSceneObject("", "NPC/chengkun", "");--成昆从地道逃跑      
        ModifyEvent(-2, 11, 1, 1, 80, -1, -1, 5310, 5310, 5310, -2, -2, -2);--by fanyu|生成人物，启动80号脚本。场景12-编号11
        jyx2_ReplaceSceneObject("", "NPC/chuzi", "1");--厨子出来
        SetScenceMap(-2, 1, 28, 24, 0);--by fanyu|明教地道的门打开。场景12-坐标28,24
		jyx2_FixMapObject("明教分舵开门",1);    
        LightScence();
        
        Talk(0, "这老贼溜的倒快，没时间追他了，我得赶快赶去光明顶才是。", "talkname0", 1);
        
        AddRepute(3);
        ChangeMMapMusic(3);
       
do return end;
