Talk(21, "大膽惡賊，竟擅闖無色庵．", "talkname21", 0);
Talk(0, "無色？妳是色盲啊？這兒五顏六色這麼多，還說什麼無色．", "talkname0", 1);
Talk(21, "大膽！膽敢在此清淨之地，口出狂言．", "talkname21", 0);
if TryBattle(24) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(21, "莫非是左冷禪派你來的！想不到左盟主為了五嶽併派之事，也不顧同盟之誼了．回去告訴左冷禪，定閒還不至忘了師祖的遺訓，併派一事我是絕對不會答應的．", "talkname21", 0);
    Talk(0, "師太在說些什麼，什麼併不併派的？我只不過上這北嶽恆山來逛逛罷了．", "talkname0", 1);
    ModifyEvent(-2, -2, -2, -2, 171, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本171 场景31-0
    Add3EventNum(27, 0, 0, 0, 56)
    AddRepute(3);
do return end;
