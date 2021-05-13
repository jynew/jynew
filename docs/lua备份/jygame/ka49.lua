Talk(0, "哇塞！這山洞有怪物住著，難道就是傳說中的雪怪．糟了，被發現了．", "talkname0", 1);
if TryBattle(5) == false then goto label0 end;
    ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);--by fanyu|杀死雪怪。场景05-编号00
    ModifyEvent(-2, 1, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|杀死雪怪。场景05-编号01
    ModifyEvent(-2, 2, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|杀死雪怪。场景05-编号02
    LightScence();
    AddRepute(2);
    do return end;
::label0::
    Dead();
do return end;
