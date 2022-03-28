Talk(0, "龙姑娘，你们古墓派还真是奇特，长年住在这古墓中，怪恐怖的。这古墓是谁造的啊？", "talkname0", 1);
Talk(59, "这古墓是全真教的祖师爷王重阳建的，又称活死人墓。", "talkname59", 0);
Talk(0, "全真教的，怎么不是古墓派的人？", "talkname0", 1);
Talk(59, "因为王重阳打不过我师祖林朝英，所以就将这古墓让了出来。从此以后，我古墓派就在此住了下来。", "talkname59", 0);
Talk(0, "原来还有这些典故。", "talkname0", 1);
Talk(59, "少侠近来如何？", "talkname59", 0);
ModifyEvent(-2, -2, -2, -2, 441, -1, -1, -2, -2, -2, -2, -2, -2);
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
