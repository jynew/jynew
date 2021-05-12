ModifyEvent(-2, 35, 1, 1, -1, -1, -1, 8246, 8246, 8246, -2, -2, -2);
ModifyEvent(-2, 36, 1, 1, -1, -1, -1, 8248, 8248, 8248, -2, -2, -2);
ModifyEvent(-2, 37, 1, 1, -1, -1, -1, 8250, 8250, 8250, -2, -2, -2);
ModifyEvent(-2, 38, 1, 1, -1, -1, -1, 8252, 8252, 8252, -2, -2, -2);
jyx2_ReplaceSceneObject("", "NPC/nanxian", "1");--南贤出现
jyx2_ReplaceSceneObject("", "NPC/beichou", "1");--北丑出现
jyx2_ReplaceSceneObject("", "NPC/chushi", "1");--厨师出现
jyx2_ReplaceSceneObject("", "NPC/kongbala", "1");--孔巴拉出现
jyx2_ReplaceSceneObject("", "Bake/Static/Langan_05_2", "");--开门
ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
Talk(0, "门总算开了，终于可以回去了．可是，心里面好像怪怪的，总觉得好像少了件什么事．．．．．．", "talkname0", 1);
if JudgeEthics(0, 0, 50) == true then goto label0 end;
    Talk(60, "小子，别想走．", "talkname60", 0);
    Talk(0, "真的发生事情了．", "talkname0", 1);
    DarkScence();
    ModifyEvent(-2, 25, 1, 1, -1, -1, -1, 8206, 8206, 8206, -2, -2, -2);
    ModifyEvent(-2, 26, 1, 1, -1, -1, -1, 8208, 8208, 8208, -2, -2, -2);
    ModifyEvent(-2, 27, 1, 1, -1, -1, -1, 8210, 8210, 8210, -2, -2, -2);
    ModifyEvent(-2, 28, 1, 1, -1, -1, -1, 8212, 8212, 8212, -2, -2, -2);
    ModifyEvent(-2, 29, 1, 1, -1, -1, -1, 8214, 8214, 8214, -2, -2, -2);
    ModifyEvent(-2, 30, 1, 1, -1, -1, -1, 8216, 8216, 8216, -2, -2, -2);
    ModifyEvent(-2, 31, 1, 1, -1, -1, -1, 8218, 8218, 8218, -2, -2, -2);
    ModifyEvent(-2, 32, 1, 1, -1, -1, -1, 8220, 8220, 8220, -2, -2, -2);
    ModifyEvent(-2, 33, 1, 1, -1, -1, -1, 8222, 8222, 8222, -2, -2, -2);
    ModifyEvent(-2, 34, 1, 1, -1, -1, -1, 8224, 8224, 8224, -2, -2, -2);
    jyx2_ReplaceSceneObject("", "NPC/xuanci", "1");--玄慈出现
    jyx2_ReplaceSceneObject("", "NPC/zhangsanfeng", "1");--张三丰出现
    jyx2_ReplaceSceneObject("", "NPC/miaorenfeng", "1");--苗人凤出现
    jyx2_ReplaceSceneObject("", "NPC/guojing", "1");--郭靖出现
    jyx2_ReplaceSceneObject("", "NPC/zhoubotong", "1");--周伯通出现
    jyx2_ReplaceSceneObject("", "NPC/huangrong", "1");--黄蓉出现
    jyx2_ReplaceSceneObject("", "NPC/huangyaoshi", "1");--黄药师出现
    jyx2_ReplaceSceneObject("", "NPC/hongqigong", "1");--洪七公出现
    jyx2_ReplaceSceneObject("", "NPC/qiaofeng", "1");--乔峰出现
    jyx2_ReplaceSceneObject("", "NPC/qiuchuji", "1");--丘处机出现   
    SetRoleFace(1);
    LightScence();
    Talk(0, "我就知道，每次在结局前，总是少不了一场大混战．", "talkname0", 1);
    Talk(67, "小子，你武功那么强，一定是从”十四天书”中领悟到什么奥妙．", "talkname67", 0);
    Talk(71, "快将它说出来，否则．．．", "talkname71", 0);
    Talk(0, "否则怎样？你们个个都是小侠我的手下败将，还想怎样？", "talkname0", 1);
    Talk(22, "一个人打不过你，那十个人呢？", "talkname22", 0);
    Talk(0, "不会吧！你们个个都是武林中有名的夙宿，不会联合起来打我一个人吧？传了出去，你们以后怎么做人？", "talkname0", 1);
    Talk(19, "事到如此，还管得了这么许多吗？", "talkname19", 0);
    Talk(51, "何况，把你杀了后，也没人会传出去了．", "talkname51", 0);
    Talk(62, "别说废话了．动手吧！", "talkname62", 0);
    if TryBattle(133) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(60, "哼！算你厉害．我们走．", "talkname60", 0);
        DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/xuanci", "");--玄慈离开
    jyx2_ReplaceSceneObject("", "NPC/zhangsanfeng", "");--张三丰离开
    jyx2_ReplaceSceneObject("", "NPC/miaorenfeng", "");--苗人凤离开
    jyx2_ReplaceSceneObject("", "NPC/guojing", "");--郭靖离开
    jyx2_ReplaceSceneObject("", "NPC/zhoubotong", "");--周伯通离开
    jyx2_ReplaceSceneObject("", "NPC/huangrong", "");--黄蓉离开
    jyx2_ReplaceSceneObject("", "NPC/huangyaoshi", "");--黄药师离开
    jyx2_ReplaceSceneObject("", "NPC/hongqigong", "");--洪七公离开
    jyx2_ReplaceSceneObject("", "NPC/qiaofeng", "");--乔峰离开
    jyx2_ReplaceSceneObject("", "NPC/qiuchuji", "");--丘处机离开 
        ModifyEvent(-2, 25, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 26, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 27, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 28, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 29, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 30, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 31, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 32, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 33, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 34, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        LightScence();
        PlayWave(23);
        Talk(0, "总算料理完毕了．咦！好像又有声音．", "talkname0", 1);
        DarkScence();
        SetScenceMap(-2, 1, 18, 25, 4062);
        SetScenceMap(-2, 1, 18, 26, 4062);
        jyx2_ReplaceSceneObject("", "Bake/Static/Light/Langan_05_2", "1");--关门
        LightScence();
        Talk(0, "怎么门又关起来了．这次大概真的结束了．走吧，我得赶快了．", "talkname0", 1);
        do return end;
::label0::
        Talk(57, "小子，你想就这样子走了吗？", "talkname57", 0);
        Talk(0, "真的发生事情了．", "talkname0", 1);
        DarkScence();
        ModifyEvent(-2, 25, 1, 1, -1, -1, -1, 8226, 8226, 8226, -2, -2, -2);
        ModifyEvent(-2, 26, 1, 1, -1, -1, -1, 8228, 8228, 8228, -2, -2, -2);
        ModifyEvent(-2, 27, 1, 1, -1, -1, -1, 8230, 8230, 8230, -2, -2, -2);
        ModifyEvent(-2, 28, 1, 1, -1, -1, -1, 8232, 8232, 8232, -2, -2, -2);
        ModifyEvent(-2, 29, 1, 1, -1, -1, -1, 8234, 8234, 8234, -2, -2, -2);
        ModifyEvent(-2, 30, 1, 1, -1, -1, -1, 8236, 8236, 8236, -2, -2, -2);
        ModifyEvent(-2, 31, 1, 1, -1, -1, -1, 8238, 8238, 8238, -2, -2, -2);
        ModifyEvent(-2, 32, 1, 1, -1, -1, -1, 8240, 8240, 8240, -2, -2, -2);
        ModifyEvent(-2, 33, 1, 1, -1, -1, -1, 8242, 8242, 8242, -2, -2, -2);
        ModifyEvent(-2, 34, 1, 1, -1, -1, -1, 8244, 8244, 8244, -2, -2, -2);
            jyx2_ReplaceSceneObject("", "NPC/xuanci", "1");--玄慈出现
    jyx2_ReplaceSceneObject("", "NPC/zhangsanfeng", "1");--张三丰出现
    jyx2_ReplaceSceneObject("", "NPC/miaorenfeng", "1");--苗人凤出现
    jyx2_ReplaceSceneObject("", "NPC/guojing", "1");--郭靖出现
    jyx2_ReplaceSceneObject("", "NPC/zhoubotong", "1");--周伯通出现
    jyx2_ReplaceSceneObject("", "NPC/huangrong", "1");--黄蓉出现
    jyx2_ReplaceSceneObject("", "NPC/huangyaoshi", "1");--黄药师出现
    jyx2_ReplaceSceneObject("", "NPC/hongqigong", "1");--洪七公出现
    jyx2_ReplaceSceneObject("", "NPC/qiaofeng", "1");--乔峰出现
    jyx2_ReplaceSceneObject("", "NPC/qiuchuji", "1");--丘处机出现 
        SetRoleFace(1);
        LightScence();
        Talk(0, "我就知道，每次在结局前，总是少不了一场大混战．", "talkname0", 1);
        Talk(69, "在这武林中，你干下了多少坏事．", "talkname69", 0);
        Talk(3, "看看你的道德指数吧，你都干了些什么？", "talkname3", 0);
        Talk(55, "练武是为了什么？行侠仗义，济弱扶倾的目的你都忘了吗？", "talkname55", 0);
        Talk(64, "你在武林中混了这么久，尽学一些坏东西．", "talkname64", 0);
        Talk(56, "是我们错了，没能把你教好．", "talkname56", 0);
        Talk(70, "但我们也不能让你就这样一走了之．", "talkname70", 0);
        Talk(50, "别怪我们出手太重，你留在这世间也只是个祸害．", "talkname50", 0);
        Talk(69, "动手吧．", "talkname69", 0);
        Talk(70, "阿弥陀佛！", "talkname70", 0);
        if TryBattle(134) == true then goto label2 end;
            Dead();
            do return end;
::label2::
            LightScence();
            Talk(55, "我们已经尽力了，还是无法消灭你这祸害．", "talkname55", 0);
            Talk(70, "阿弥陀佛！这是众生的浩劫呀！", "talkname70", 0);
            Talk(69, "杀不了你这个大魔头．唉！或许是天意吧！我们走吧．", "talkname69", 0);
            DarkScence();
            ModifyEvent(-2, 25, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 26, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 27, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 28, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 29, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 30, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 31, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 32, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 33, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 34, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    jyx2_ReplaceSceneObject("", "NPC/xuanci", "");--玄慈离开
    jyx2_ReplaceSceneObject("", "NPC/zhangsanfeng", "");--张三丰离开
    jyx2_ReplaceSceneObject("", "NPC/miaorenfeng", "");--苗人凤离开
    jyx2_ReplaceSceneObject("", "NPC/guojing", "");--郭靖离开
    jyx2_ReplaceSceneObject("", "NPC/zhoubotong", "");--周伯通离开
    jyx2_ReplaceSceneObject("", "NPC/huangrong", "");--黄蓉离开
    jyx2_ReplaceSceneObject("", "NPC/huangyaoshi", "");--黄药师离开
    jyx2_ReplaceSceneObject("", "NPC/hongqigong", "");--洪七公离开
    jyx2_ReplaceSceneObject("", "NPC/qiaofeng", "");--乔峰离开
    jyx2_ReplaceSceneObject("", "NPC/qiuchuji", "");--丘处机离开 
            LightScence();
            PlayWave(23);
            Talk(0, "总算料理完毕了．咦！好像又有声音．", "talkname0", 1);
            DarkScence();
            SetScenceMap(-2, 1, 18, 25, 4062);
            SetScenceMap(-2, 1, 18, 26, 4062);
            jyx2_ReplaceSceneObject("", "Bake/Static/Light/Langan_05_2", "1");--关门
            LightScence();
            Talk(0, "怎么门又关起来了．这次大概真的结束了．走吧，我得赶快了．", "talkname0", 1);
do return end;
