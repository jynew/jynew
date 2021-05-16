ScenceFromTo(41, 31, 34, 31);
Talk(3, "你們是田歸農請來的吧！神龍教何時跟田歸農搭在一起了．", "talkname3", 0);
Talk(97, "還多虧了田兄，我們才知道”飛狐外傳”一書在你這．識相的話就快將書交出來．", "talkname97", 0);
Talk(3, "田歸農呢？他怎麼不敢出來見我．", "talkname3", 0);
Talk(97, "我看你是見不到他了．田兄從毒手藥王那弄來的斷腸草粉末，藥效也真夠狠，現下你雙眼已瞎，我看”打遍天下無敵手”的金面佛苗人鳳，今日要上西天了兄弟們，上！", "talkname97", 0);
ScenceFromTo(34, 31, 41, 31);
Talk(0, "苗大俠，我幫你拿賊．", "talkname0", 1);
if TryBattle(3) == true then goto label0 end;
    Dead();
    do return end;
::label0::
jyx2_ReplaceSceneObject("", "NPC/shenlongjiaotu", "");--战斗结束，移除人物
jyx2_ReplaceSceneObject("", "NPC/shenlongjiaotu1", "");--战斗结束，移除人物
jyx2_ReplaceSceneObject("", "NPC/shenlongjiaotu2", "");--战斗结束，移除人物
jyx2_ReplaceSceneObject("", "NPC/shenlongjiaotu3", "");--战斗结束，移除人物
    ModifyEvent(-2, 2, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号2
    ModifyEvent(-2, 3, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号3
    ModifyEvent(-2, 4, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号4
    ModifyEvent(-2, 5, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号5
    ModifyEvent(-2, 6, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号6
    ModifyEvent(-2, 7, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号7
    SetScencePosition2(35, 31);
    LightScence();
    Talk(3, "小兄弟，你尊姓大名，與我可有淵源？", "talkname3", 0);
    Talk(0, "丈夫結交，但重義氣，只需肝膽相照，何必提名道姓．", "talkname0", 1);
    Talk(3, "好，苗人鳳獨來獨往，生平只有兩個知交，一個是遼東大俠胡一刀，另一個便是你這位不知姓名沒見過面的小兄弟．", "talkname3", 0);
    if InTeam(1) == false then goto label1 end;
        Talk(1, "你說什麼．那你為何要殺死胡大俠．", "talkname1", 1);
        Talk(3, "這說來話長．．．", "talkname3", 0);
        Talk(0, "大哥，我們先想辦法救苗大俠，這事以後再說．", "talkname0", 1);
::label1::
        Talk(0, "苗大俠，既然這藥是毒手藥王所配製，那我們去求毒手藥王救治，或能解得．", "talkname0", 1);
        Talk(3, "要求毒手藥王嗎？那是徒勞往返，不用去了．", "talkname3", 0);
        Talk(0, "不，天下無難事！這位毒手藥王住在那裡？", "talkname0", 1);
        Talk(3, "聽說此人在洞庭湖畔隱居．", "talkname3", 0);
        Talk(0, "我這就去了！", "talkname0", 1);
        SetScenceMap(49, 1, 28, 37, 0);--by fanyu  场景49-编号1，坐标的贴图改变，门移除
        SetScenceMap(49, 1, 27, 37, 3692);--by fanyu  场景49-编号1，坐标的贴图改变，门移除
        SetScenceMap(49, 1, 27, 36, 3694);--by fanyu  场景49-编号1，坐标的贴图改变，门移除
        ModifyEvent(-2, 9, -2, -2, -2, -2, 35, -2, -2, -2, -2, -2, -2);--by fanyu 启动35脚本 场景24-编号9
        ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
        AddEthics(2);
        AddRepute(1);
        ChangeMMapMusic(3);
do return end;
