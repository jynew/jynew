Talk(30, "怎么样，兄弟，还是少不了我这个扛打高手吧！");
if AskJoin() == true then goto label0 end;
    Talk(0, "那个，我们现在还没到要打架的时候，到时候我再来叫你啊。");
    do return end;
::label0::
  if TeamIsFull() == false then goto label1 end;
        Talk(30, "你的队伍已满，我无法加入。");
        do return end;
::label1::
        Talk(0, "是的，还是来帮帮我吧。");
        DarkScence();
        jyx2_ReplaceSceneObject("", "NPC/虚寂", "");
        LightScence();
        Join(30);
        ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
