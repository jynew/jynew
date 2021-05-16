if UseItem(176) == true then goto label0 end;
    do return end;
::label0::
    AddItem(176, -1);
    Talk(0, "老伯，你嚐嚐看這是不是你說的那道菜．", "talkname0", 1);
    Talk(69, "我看看．．．．嗯．．．．．．一條是羊羔坐臀，一條是小豬耳朵，一條是小牛腰子，還有一條是獐腿肉加兔肉．肉只五種，但豬羊混咬是一般滋味，獐牛同嚼又是一般滋味，總共二十五種變化．嗯，沒錯，就是這種美味．", "talkname69", 0);
    Talk(0, "老伯果然了不起．", "talkname0", 1);
    Talk(69, "我就是這個饞嘴的臭脾氣，一想到吃就什麼也都忘了．古人說：”食指大動”，真是一點也不錯．我只要見到或是聞到奇珍異味，右手的食指就會跳個不住．有一次為了貪吃誤了一件大事，我一發狠，一刀砍了指頭！", "talkname69", 0);
    Talk(0, "啊！", "talkname0", 1);
    Talk(69, "指頭是砍了，饞嘴的性兒卻砍不了．當初我就是貪吃，才讓蓉兒抓住我的個性，讓我傳了那郭靖降龍十八掌．今日又忍不住，吃了你那”玉笛誰家聽落梅”，說不得只好也傳你這天下至剛的”降龍十八掌”了．", "talkname69", 0);
    Talk(0, "謝謝前輩．", "talkname0", 1);
    DarkScence();
    SetScencePosition2(30, 33);
    SetRoleFace(2);
    LightScence();
    Talk(69, "看好了，我只使一遍．", "talkname69", 0);
    PlayAnimation(0, 6228, 6254);
    DarkScence();
    SetScencePosition2(26, 33);
    ModifyEvent(-2, -2, -2, -2, 463, -1, -1, 6122, 6122, 6122, -2, -2, -2);--by fanyu 改变贴图，启动脚本463 场景23-编号0
    ModifyEvent(-2, 1, -2, -2, -1, -1, 464, -1, -1, -1, -2, -2, -2);--by fanyu 启动脚本464 场景23-编号1
    LightScence();
    Talk(69, "小子，學了這掌法，望你用於正途．否則，老叫化我第一個將你除去．", "talkname69", 0);
    Talk(0, "謹遵師父教誨．", "talkname0", 1);
    Talk(69, "什麼”師父”，我不是你師父，你燒好菜給我吃，我教你一套掌法，各不相欠．知道嗎．沒事就走吧，老叫化我不會再教你了．", "talkname69", 0);
    GetItem(62, 1);
do return end;
