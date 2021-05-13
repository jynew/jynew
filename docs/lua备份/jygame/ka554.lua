Talk(0, "請問是薛慕華薛前輩嗎？", "talkname0", 1);
Talk(45, "這位小兄弟今天來不知是．．．．", "talkname45", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "沒什麼事，在下途經這柳宗鎮，聽說神醫住在附近，特來拜見．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "是這樣的，我聽說薛先生醫術高明，今日來是想請先生加入我的隊伍，日後能幫我同伴們治病療傷．", "talkname0", 1);
    Talk(45, "對不起，在下技藝有限，閣下還是另請高明吧．", "talkname45", 0);
do return end;
