Talk(86, "這裡是四川青城派，閒雜人等不得進入．", "talkname86", 0);
Talk(0, "你們這些江湖的敗類，不但佔地為王，而且還血洗了福威鑣局，將人家總鑣頭給捉了來．你們眼中還有王法嗎．", "talkname0", 1);
Talk(86, "讓我們”青城四秀”來告訴你什麼叫王法．", "talkname86", 0);
if TryBattle(49) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 0, -2, -2, 294, -1, -1, 6044, 6044, 6044, -2, 30, 26);
    ModifyEvent(-2, 1, -2, -2, 294, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 2, -2, -2, 294, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 3, -2, -2, 294, -1, -1, -2, -2, -2, -2, -2, -2);
    LightScence();
    Talk(0, "江湖上人說什麼”英雄豪傑，青城四秀”，在我看來，叫”狗熊野豬，青城四獸”還差不多．", "talkname0", 1);
    Talk(86, "看我師父如何收拾你．", "talkname86", 0);
    AddEthics(2);
    AddRepute(1);
do return end;
