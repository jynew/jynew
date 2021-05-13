Talk(65, "真希望瑛姑趕快來殺我，解除我的罪孽．", "talkname65", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "．．．．", "talkname0", 1);
    do return end;
::label0::
    AddEthics(-1);
    if TryBattle(68) == false then goto label1 end;
        ModifyEvent(21, 1, -2, -2, 420, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动420脚本。场景21-编号01
        ModifyEvent(-2, -2, -2, -2, -1, -1, -1, 6226, 6226, 6226, -2, -2, -2);--by fanyu|改变贴图。场景47-编号00
        LightScence();
        AddEthics(-10);
        AddRepute(10);
        do return end;
::label1::
        LightScence();
        Talk(65, "閣下還是回去請瑛姑親自來動手吧，我會在這等她的．", "talkname65", 0);
do return end;
