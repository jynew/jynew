if InTeam(1) == true then goto label0 end;
    Talk(4, "少侠请随便看看，有什么喜欢的就拿去吧。", "talkname4", 0);
    do return end;
::label0::
    Talk(0, "阎基，为何你家中有胡家刀法的缺页？", "talkname0", 1);
    Talk(4, "胡家刀法，哦！那是我年轻时在沧州无意间所获得的。", "talkname4", 0);
    Talk(1, "你为什么有我胡家刀法的缺页。", "talkname1", 1);
    Talk(4, "原来胡一刀的后人长这么大了。", "talkname4", 0);
    Talk(1, "小时候平四叔曾告诉我，当年害死我父亲的，一个跌打医生也有份，不但如此，那跌打医生后来还撕去几页胡家刀法。莫非……那个人就是你！", "talkname1", 1);
    Talk(0, "没错，这家伙会医术，上次还说要帮我看病。", "talkname0", 1);
    Talk(4, "既然你们认出来，我也就没什么好隐瞒的了。没错，那跌打医生就是我。", "talkname4", 0);
    Talk(1, "你为什么要帮苗人凤害死我父亲？", "talkname1", 1);
    Talk(4, "谁叫《雪山飞狐》一书在他那呢？当初若不是让他们俩傻呼呼的拼斗，又在苗人凤剑上喂点毒药，我怎么能坐收渔翁之利呢？", "talkname4", 0);
    Talk(1, "可恶的家伙，替我父亲偿命来。", "talkname1", 1);
    Talk(4, "若不是有准备，我怎敢说出这些。小娃儿，试试我的新玩意，天下至毒的“七心海棠”。", "talkname4", 0);
    SetOneUsePoi(4, 99);
    if TryBattle(2) == false then goto label1 end;
        ModifyEvent(-2, 1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -2, -2);
        jyx2_ReplaceSceneObject("", "NPC/yanji", "");-- 移除人物
        LightScence();
        AddItem(158, 1);
        AddRepute(2);
        AddEthics(2);
        do return end;
::label1::
        Dead();
do return end;
