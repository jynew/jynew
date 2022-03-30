Talk(21, "大胆恶贼，竟擅闯无色庵。", "talkname21", 0);
Talk(0, "无色？你是色盲啊？这儿五颜六色这么多，还说什么无色。", "talkname0", 1);
Talk(21, "大胆！胆敢在此清净之地，口出狂言。", "talkname21", 0);
if TryBattle(24) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(21, "莫非是左冷禅派你来的！想不到左盟主为了五岳并派之事，也不顾同盟之谊了。回去告诉左冷禅，定闲还不至忘了师祖的遗训，并派一事我是绝对不会答应的。", "talkname21", 0);
    Talk(0, "师太在说些什么，什么并不并派的？我只不过上这北岳恒山来逛逛罢了。", "talkname0", 1);
    ModifyEvent(-2, -2, -2, -2, 171, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本171 场景31-0
	Add3EventNum(27, 0, 0, 0, 1);--需要拜访嵩山以外4派才会触发五岳并派事件。嵩山入口0号trigger对应起始事件为198，每拜访1派事件序号+1，202号事件为并派事件。
    AddRepute(3);
do return end;
