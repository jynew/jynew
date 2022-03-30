if InTeam(9) == true then goto label0 end;
    do return end;
::label0::
    jyx2_ReplaceSceneObject("", "NPC/zhangwuji", "1");--张无忌
    Talk(9, "太师父，太师父，无忌回来看你了。", "talkname9", 1);
    Talk(5, "无忌，真的是你。好孩子，你没有死，翠山可有后了。是蝶谷医仙将你医好的吗？", "talkname5", 0);
    Talk(9, "不是的。我是有了一番奇遇……如此如此……这般这般……后来修习了九阳神功，才将我身上的寒毒化去。", "talkname9", 1);
    Talk(5, "很好，很好，真难为你了。", "talkname5", 0);
    Talk(9, "我现在跟着这位大哥到处云游，也顺便历练历练自己。", "talkname9", 1);
    Talk(5, "历练自己是好的，但要记得常存侠义之心，才是我辈中人。", "talkname5", 0);
    Talk(9, "太师父教诲，无忌谨记在心……", "talkname9", 1);
    jyx2_ReplaceSceneObject("", "NPC/zhangwuji", "");--张无忌
    ModifyEvent(-2, 5, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 6, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 7, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 8, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 9, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 10, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    AddEthics(2);
do return end;
