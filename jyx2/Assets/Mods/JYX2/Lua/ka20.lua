Talk(0, "奇怪，头怎么这么重……莫非……", "talkname0", 0);
PlayAnimation(-1, 5994, 6012);
jyx2_PlayTimeline("[Timeline]ka20_阎基居_中悲酥清风", 1, true, "");
DarkScence();
jyx2_StopTimeline("[Timeline]ka20_阎基居_中悲酥清风");
ZeroAllMP();
ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
ModifyEvent(-2, 1, 1, -2, 21, -1, -1, 5168, 5168, 5168, 0, -2, -2);
jyx2_ReplaceSceneObject("","NPC/yanji","1"); --刷出阎基

jyx2_PlayTimeline("[Timeline]ka20_阎基居_苏醒", 0, true, "");
LightScence();
jyx2_Wait(1.5);
Talk(4, "原本预备对付苗人凤的悲酥清风，居然先让你受用了！", "talkname4", 0);
PlayAnimation(-1, 6026, 6036);
Talk(0, "你……你……", "talkname0", 1);
Talk(4, "江湖上最近盛传有个年轻小毛头到处找“十四天书”，想必就是你吧？找到几本书啦？快交出来。", "talkname4", 0);
Talk(0, "你真卑鄙。", "talkname0", 1);
Talk(4, "我阎基做事向来只求结果，不问方法。怎么？还不拿出来，要大爷我亲自动手吗？要知道书对死人是没有意义的。", "talkname4", 0);
Talk(0, "倒要看看死的是你还是我。", "talkname0", 1);
jyx2_StopTimeline("[Timeline]ka20_阎基居_苏醒");
if TryBattle(1) == false then goto label0 end;
    LightScence();
    Talk(4, "想不到少侠武功如此盖世，连西夏的悲酥清风都对你没有作用，“十四天书”的确该是少侠所有。", "talkname4", 0);
    Talk(0, "转的倒是挺快的嘛，毛头小子马上就变成少侠，难怪这一点点武艺可以混到现在。", "talkname0", 1);
    Talk(4, "小的这两下哪能和少侠相比呢，混口饭吃罢了。对了，小的略通点医术，少侠是否有什么病痛，小的帮你看看。", "talkname4", 0);
    Talk(0, "我看是算了吧，给你看病，那岂不是让黄鼠狼给鸡把脉一样的道理。", "talkname0", 1);
    Talk(4, "哪儿的话，那少侠就随便看看，若不嫌弃的话，有什么喜欢的就拿去吧。", "talkname4", 0);
    ModifyEvent(-2, 7, -2, -2, -2, -2, 25, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 8, -2, -2, -2, -2, -2, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("","Dynamic/香炉","");
    AddRepute(1);
    do return end;
::label0::
    Dead();
do return end;
