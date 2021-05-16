if InTeam(1) == true then goto label0 end;
    Talk(4, "少俠請隨便看看，有什麼喜歡的就拿去吧．", "talkname4", 0);
    do return end;
::label0::
    Talk(0, "閻基，為何你家中有胡家刀法的缺頁．", "talkname0", 1);
    Talk(4, "胡家刀法，哦！那是我年輕時在滄州無意間所獲得的．", "talkname4", 0);
    Talk(1, "你為什麼有我胡家刀法的缺頁．", "talkname1", 1);
    Talk(4, "原來胡一刀的後人長這麼大了．", "talkname4", 0);
    Talk(1, "小時後平四叔曾告訴我，當年害死我父親的，一個跌打醫生也有份，不但如此，那跌打醫生後來還撕去幾頁胡家刀法．莫非．．．那個人就是你！", "talkname1", 1);
    Talk(0, "沒錯，這傢伙會醫術，上次還說要幫我看病．", "talkname0", 1);
    Talk(4, "既然你們認出來，我也就沒什麼好隱瞞的了．沒錯，那跌打醫生就是我．", "talkname4", 0);
    Talk(1, "你為什麼要幫苗人鳳要害死我父親．", "talkname1", 1);
    Talk(4, "誰叫”雪山飛狐”一書在他那呢？當初若不是讓他們倆傻呼呼的拼鬥，又在苗人鳳劍上餵點毒藥，我怎麼能坐收漁翁之利呢？", "talkname4", 0);
    Talk(1, "可惡的傢伙，替我父親償命來．", "talkname1", 1);
    Talk(4, "若不是有準備，我怎敢說出這些．小娃兒，試試我的新玩意，天下至毒的”七心海棠”．", "talkname4", 0);
    SetOneUsePoi(4, 99);
    if TryBattle(2) == false then goto label1 end;
        ModifyEvent(-2, 1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -2, -2);
        LightScence();
        GetItem(158, 1);
        AddRepute(2);
        AddEthics(2);
        jyx2_ReplaceSceneObject("", "NPC/yanji", "");-- 移除人物
        do return end;
::label1::
        Dead();
do return end;
