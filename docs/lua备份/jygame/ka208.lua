Talk(22, "小兄弟，我看你武功不錯，你我二人一起稱霸這江湖，如何？", "talkname22", 0);
Talk(0, "你武功那麼差，我看你還是安份一點．", "talkname0", 1);
Talk(22, "上回是老朽是太輕敵了，你還想試試看嗎？", "talkname22", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    if TryBattle(38) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
do return end;
