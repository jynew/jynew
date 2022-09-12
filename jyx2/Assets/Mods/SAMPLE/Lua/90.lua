Talk(90, "六一哥哥，你终于来啦！");
Talk(0, "你，你是？");
Talk(90, "我敲你个大头头，你得到了我的人就假装不认识我了吗？");
Talk(100, "我天，这姑娘才多大年纪，陶六一这臭小子也不大吧，怎么就……");
Talk(0, "哦哦，你好你好。");
Talk(90, "好你个大头头，我是你的小宝贝。");
Talk(0, "小……小宝贝。");
Talk(100, "我天，好有罪恶感。");
Talk(90, "这就对了，你不是说要带我去天涯海角吗，六一哥哥现在就带我去玩儿吧。");
if AskJoin() == true then goto label0 end;
    Talk(0, "我现在忙得狠，下次吧。");
    Talk(90, "你这个负心汉！~");
    ModifyEvent(-2, -2, -2, -2, 91, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    if TeamIsFull() == false then goto label1 end;
        Talk(90, "你的队伍已满，我无法加入。");
        do return end;
::label1::
    Talk(0, "真是怕了你。");
    Talk(90, "走走走，带我闯荡江湖去。");
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/牛妞妞", "");
    LightScence();
    Join(90);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
