Talk(59, "少侠近来如何？", "talkname59", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切还好，你们还好吧？", "talkname0", 1);
    Talk(59, "嗯。", "talkname59", 0);
    Talk(0, "你们俩真是令人羡慕的神仙侠侣。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "近日旅途有些不顺，此次前来是想请龙姑娘加入，助我一臂之力。", "talkname0", 1);
    Talk(59, "好啊，我夫妇俩受你这么大的恩惠，理应帮忙你一些。", "talkname59", 0);
    Talk(0, "真是不好意思，打扰了你跟杨兄的清静。", "talkname0", 1);
    Talk(59, "没有关系，等解决了你的问题后，再回来古墓就好了。", "talkname59", 0);
    Talk(0, "谢谢你的帮忙。", "talkname0", 1);
    DarkScence();
    if TeamIsFull() == false then goto label1 end;
        Talk(59, "你的队伍已满，我无法加入。", "talkname59", 0);
        do return end;
::label1::
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/小龙女", "");--小龙女加入队伍
    ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);        
    LightScence();
    Join(59);
do return end;
