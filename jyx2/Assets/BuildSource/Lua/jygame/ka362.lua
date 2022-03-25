Talk(23, "银光灿烂，鞍自平稳。天马行空，瞬息万里……原来天马是手。并非真的是马。", "talkname23", 0);
if InTeam(38) == true then goto label0 end;
    Talk(23, "这壁上的注解说道：白居易诗云“勿轻直折剑，犹胜曲全勾”。可见我这直折之剑，方合石壁注文原意。", "talkname23", 0);
    Talk(20, "不对，“吴钩霜雪明”是主，“犹胜曲全钩”是宾。喧宾夺主，必非正道。", "talkname20", 1);
    do return end;
::label0::
    -- DarkScence();
    -- jyx2_ReplaceSceneObject("", "NPC/shipotian2", "1");--石破天出现
    -- LightScence();
    Talk(38, "＜这些口诀甚是深奥，我是弄不明白的。唉！没读过书就是不行。＞", "talkname38", 1);
    Talk(23, "这壁上的注解说道：白居易诗云“勿轻直折剑，犹胜曲全勾”。可见我这直折之剑，方合石壁注文原意。", "talkname23", 0);
    Talk(20, "不对，“吴钩霜雪明”是主，“犹胜曲全钩”是宾。喧宾夺主，必非正道。", "talkname20", 1);
    Talk(38, "＜咦！这些字的笔划看起来好像是一把把的长剑，有的剑尖朝上，有的向下，有的斜起欲飞，有的横掠欲堕。“五里穴”好热……现在跑到“曲池穴”了……自从练了木偶身上的经脉图之后，内力大盛，但从没像今日这般劲急……＞", "talkname38", 1);
    Add3EventNum(-2, 4, 0, 0, 1);
    Add3EventNum(-2, 5, 0, 0, 1);
    Add3EventNum(-2, 6, 0, 0, 1);
    ModifyEvent(-2, 10, -2, -2, 387, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 11, -2, -2, 387, -1, -1, -2, -2, -2, -2, -2, -2);
    -- DarkScence();
    -- jyx2_ReplaceSceneObject("", "NPC/shipotian2", "");--石破天出现
    -- LightScence();
do return end;
