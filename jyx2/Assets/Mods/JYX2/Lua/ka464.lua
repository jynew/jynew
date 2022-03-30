ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
if JudgeEventNum(2, -1) == true then goto label0 end;
    ModifyEvent(-2, 2, -2, -2, 862, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本862 场景23-编号2
::label0::
    if JudgeEventNum(3, -1) == true then goto label1 end;
        ModifyEvent(-2, 3, -2, -2, 863, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本863 场景23-编号3
::label1::
        if JudgeEventNum(4, -1) == true then goto label2 end;
            ModifyEvent(-2, 4, -2, -2, 864, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本864 场景23-编号4
::label2::
            if JudgeEventNum(5, -1) == true then goto label3 end;
                ModifyEvent(-2, 5, -2, -2, 865, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本865 场景23-编号5
::label3::
do return end;
