Talk(0, "龍姑娘，你們古墓派還真是奇特，長年住在這古墓中，怪恐怖的．這古墓是誰造的啊？", "talkname0", 1);
Talk(59, "這古墓是全真教的祖師爺王重陽建的，又稱活死人墓．", "talkname59", 0);
Talk(0, "全真教的，怎麼不是古墓派的人？", "talkname0", 1);
Talk(59, "因為王重陽打不過我師祖林朝英，所以就將這古墓讓了出來．從此以後，我古墓派就在此住了下來．", "talkname59", 0);
Talk(0, "原來還有這些典故．", "talkname0", 1);
Talk(59, "少俠近來如何？", "talkname59", 0);
ModifyEvent(-2, -2, -2, -2, 441, -1, -1, -2, -2, -2, -2, -2, -2);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切還好，你們還好吧？", "talkname0", 1);
    Talk(59, "嗯．", "talkname59", 0);
    Talk(0, "你們倆真是令人羨慕的神仙俠侶．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "近日旅途有些不順，此次前來是想請龍姑娘加入，助我一臂之力．", "talkname0", 1);
    Talk(59, "好啊，我夫婦倆受你這麼大的恩惠，理應幫忙你一些．", "talkname59", 0);
    Talk(0, "真是不好意思，打擾了妳跟楊兄的清靜．", "talkname0", 1);
    Talk(59, "沒有關係，等解決了你的問題後，再回來古墓就好了．", "talkname59", 0);
    Talk(0, "謝謝你的幫忙．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(59, "你的隊伍已滿，我無法加入．", "talkname59", 0);
        do return end;
::label1::
        jyx2_ReplaceSceneObject("", "NPC/xiaolongnv", "");--小龙女加入队伍
        DarkScence();

        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        LightScence();
        Join(59);
do return end;
