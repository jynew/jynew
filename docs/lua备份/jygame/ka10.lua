if UseItem(133) == true then goto label0 end;
    do return end;
::label0::
    ModifyEvent(-2, -2, -2, -2, 11, -2, -2, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 1, -2, -2, -2, -2, 7, -2, -2, -2, -2, -2, -2);
    AddItem(133, -1);
    Talk(1, "咦？這．．．這是．．．", "talkname1", 0);
    Talk(0, "此兩頁刀法乃是從江南惡霸閰基家中所得．難道這就是你所失之物？", "talkname0", 1);
    Talk(1, "正是家傳胡氏刀譜的總訣．多年來尋覓不得，不料今日被你找到．小時候我曾聽平四叔說，偷我胡家刀譜的人，也是我的殺父仇人之一．今後我定要勤練胡家刀法，再找那苗人鳳及惡霸閻基報殺父之仇．小兄弟，你的盛情，我胡斐不知如何言謝！有什麼需要我效勞的，儘管說吧！", "talkname1", 0);
    Talk(0, "找到了你胡家刀法的缺頁，我卻還沒找到”雪山飛狐”一書，唉！", "talkname0", 1);
    Talk(1, "我胡斐雖不知該書的下落，但我曾經想過，為什麼我的名字與這本書名這麼相似？莫非家父當初跟此書有什麼關聯？這樣吧！若你有需要的話，儘管講一聲，我就陪你找上一找．我正好也想到江湖上去歷練一下，找害死我父親的苗人鳳報仇．當然，還得先找那偷我胡家刀法的閻基報仇，害我一直無法練功．走，找他去．而我屋內的東西，你若有需要，也可自行拿取．", "talkname1", 0);
    if JudgeEventNum(2, -1) == true then goto label1 end;
        ModifyEvent(-2, 2, -2, -2, 833, -1, -1, -2, -2, -2, -2, -2, -2);
::label1::
        if JudgeEventNum(3, -1) == true then goto label2 end;
            ModifyEvent(-2, 3, -2, -2, 834, -1, -1, -2, -2, -2, -2, -2, -2);
::label2::
            AddEthics(2);
            if AskJoin () == true then goto label3 end;
                do return end;
::label3::
                Talk(0, "胡大哥肯隨我闖盪江湖否？", "talkname0", 1);
                if TeamIsFull() == false then goto label4 end;
                    Talk(1, "你的隊伍已滿，我無法加入．", "talkname1", 0);
                    do return end;
::label4::
                    Talk(1, "好．我就隨你一走．", "talkname1", 0);
                    Talk(0, "胡大哥肯隨我闖盪江湖幫這個忙，那再好也不過了．", "talkname0", 1);
                    DarkScence();
                    ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
                    jyx2_ReplaceSceneObject("", "NPC/胡斐", ""); 
                    LightScence();
                    ModifyEvent(-2, 1, -2, -2, -2, -2, 7, -2, -2, -2, -2, -2, -2);
                    jyx2_ReplaceSceneObject("","NPC/胡斐","");

                    Join(1);
                    AddEthics(1);
do return end;
