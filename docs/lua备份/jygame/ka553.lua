Talk(0, "請問是薛慕華薛前輩嗎？", "talkname0", 1);
Talk(45, "正是，不知閣下．．", "talkname45", 0);
Talk(0, "久聞薛先生醫道天下無雙，江湖上人稱”閻王敵”．", "talkname0", 1);
Talk(45, "那裡，這是江湖朋友抬愛在下了．", "talkname45", 0);
Talk(0, "如果連閻羅王都怕了薛先生你，想必醫術定是非常高明了．", "talkname0", 1);
Talk(45, "這位小兄弟今天來不知是．．．．", "talkname45", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "沒什麼事，在下途經這柳宗鎮，聽說神醫住在附近，特來拜見．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "是這樣的，我聽說薛先生醫術高明，今日來是想請先生加入我的隊伍，日後能幫我同伴們治病療傷．", "talkname0", 1);
    Talk(45, "對不起，在下技藝有限，閣下還是另請高明吧．", "talkname45", 0);
    ModifyEvent(-2, -2, -2, -2, 554, 555, -1, -2, -2, -2, -2, -2, -2);
do return end;
