if InTeam(9) == true then goto label0 end;
    do return end;
::label0::
    Talk(9, "太師父，太師父，無忌回來看你了．", "talkname9", 1);
    jyx2_ReplaceSceneObject("", "NPC/zhangwuji", "1");--张无忌
    Talk(5, "無忌，真的是你．好孩子，你沒有死，翠山可有後了．是蝶谷醫仙將你醫好的嗎？", "talkname5", 0);
    Talk(9, "不是的．我是有了一番奇遇．．．．如此如此．．．．這般這般．．．後來修習了九陽神功，才將我身上的寒毒化去．", "talkname9", 1);
    Talk(5, "很好，很好，真難為你了．", "talkname5", 0);
    Talk(9, "我現在跟著這位大哥到處雲遊，也順便歷練歷練自己．", "talkname9", 1);
    Talk(5, "歷練自己是好的，但要記得常存俠義之心，才是我輩中人．", "talkname5", 0);
    Talk(9, "太師父教誨，無忌謹記在心．．．．", "talkname9", 1);
    jyx2_ReplaceSceneObject("", "NPC/zhangwuji", "");--张无忌
    ModifyEvent(-2, 5, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 6, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 7, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 8, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 9, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 10, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    AddEthics(2);
do return end;
