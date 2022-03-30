Talk(0, "哇！这里这么多人，干什么这么热闹，可少不了小侠我。我说名门正派又怎么样，还不是一样赶尽杀绝，跟魔教又有什么两样，只不过藉口好听一点罢了。", "talkname0", 1);
Talk(14, "都是你这个兔崽子，打伤了鹰王和我，", "talkname14", 0);
Talk(11, "还有我。", "talkname11", 0);
Talk(14, "害我们只剩下范右使应战，寡不敌众……", "talkname14", 0);
Talk(0, "既然如此，我就帮你们打发这些人，当是陪罪好了。", "talkname0", 1);
Talk(70, "少侠非魔教人士，还请速速离去，以免受池鱼之殃。", "talkname70", 0);
Talk(0, "那好，大家都别打了，因为这中间着实有许多误会，让我好好说给你们听。", "talkname0", 1);
Talk(6, "现在年轻人都这么狂妄吗？你自以为是武林盟主吗！要我们听你说。", "talkname6", 0);
Talk(8, "你这小贼，跟魔教勾结，想要拖延时间，好施什么诡计么？先杀了你再说。", "talkname8", 0);
Talk(0, "我就知道要你们安安静静听我说是不太可能，那只有想办法让你们服气了。一起来吧，这样比较省事。", "talkname0", 1);
Talk(6, "好狂妄的口气。", "talkname6", 0);
Talk(70, "阿弥陀佛。", "talkname70", 0);
if TryBattle(12) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    --昆仑派
	ModifyEvent(-2, 12, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-12
    ModifyEvent(-2, 14, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-14
    ModifyEvent(-2, 15, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-15
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子12", "");
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子14", "");
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子15", "");
    ModifyEvent(-2, 16, 0, 0, -1, -1, -1, 5434, 5434, 5434, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-16
    ModifyEvent(-2, 17, 0, 0, -1, -1, -1, 5432, 5432, 5432, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-17
    ModifyEvent(-2, 18, 0, 0, -1, -1, -1, 5434, 5434, 5434, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-18
	jyx2_SwitchRoleAnimation("NPC/昆仑弟子16","Assets/BuildSource/AnimationControllers/Dead.controller");
	jyx2_SwitchRoleAnimation("NPC/昆仑弟子17","Assets/BuildSource/AnimationControllers/Dead.controller");
	jyx2_SwitchRoleAnimation("NPC/昆仑弟子18","Assets/BuildSource/AnimationControllers/Dead.controller");
	--崆峒派
    ModifyEvent(-2, 20, 0, 0, -1, -1, -1, 5428, 5428, 5428, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-20
	jyx2_SwitchRoleAnimation("NPC/崆峒弟子20","Assets/BuildSource/AnimationControllers/Dead.controller");
    ModifyEvent(-2, 21, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-21
    ModifyEvent(-2, 22, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-22
    ModifyEvent(-2, 23, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-23
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子21", "");
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子22", "");
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子23", "");
    ModifyEvent(-2, 24, 0, 0, -1, -1, -1, 5428, 5428, 5428, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-24
	jyx2_SwitchRoleAnimation("NPC/崆峒弟子24","Assets/BuildSource/AnimationControllers/Dead.controller");
    ModifyEvent(-2, 25, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-25
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子25", "");
    ModifyEvent(-2, 26, 0, 0, -1, -1, -1, 5430, 5430, 5430, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-26
	jyx2_SwitchRoleAnimation("NPC/崆峒弟子26","Assets/BuildSource/AnimationControllers/Dead.controller");
    ModifyEvent(-2, 27, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-27
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子27", "");
	--华山派
    ModifyEvent(-2, 29, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-29
    ModifyEvent(-2, 32, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-32
    ModifyEvent(-2, 33, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-33
    ModifyEvent(-2, 34, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-34
    ModifyEvent(-2, 35, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-35
	jyx2_ReplaceSceneObject("", "NPC/华山弟子29", "");
	jyx2_ReplaceSceneObject("", "NPC/华山弟子32", "");
	jyx2_ReplaceSceneObject("", "NPC/华山弟子33", "");
	jyx2_ReplaceSceneObject("", "NPC/华山弟子34", "");
	jyx2_ReplaceSceneObject("", "NPC/华山弟子35", "");
	--少林派
    ModifyEvent(-2, 38, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-38
	jyx2_ReplaceSceneObject("", "NPC/少林弟子38", "");
    ModifyEvent(-2, 39, 0, 0, -1, -1, -1, 5446, 5446, 5446, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-39
	jyx2_SwitchRoleAnimation("NPC/少林弟子39","Assets/BuildSource/AnimationControllers/Dead.controller");
    ModifyEvent(-2, 40, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-40
	jyx2_ReplaceSceneObject("", "NPC/少林弟子40", "");
    ModifyEvent(-2, 41, 0, 0, -1, -1, -1, 5444, 5444, 5444, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-41
	jyx2_SwitchRoleAnimation("NPC/少林弟子41","Assets/BuildSource/AnimationControllers/Dead.controller");
    ModifyEvent(-2, 42, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-42
    ModifyEvent(-2, 43, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-43
	jyx2_ReplaceSceneObject("", "NPC/少林弟子42", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子43", "");
    ModifyEvent(-2, 44, 0, 0, -1, -1, -1, 5444, 5444, 5444, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-44
	jyx2_SwitchRoleAnimation("NPC/少林弟子44","Assets/BuildSource/AnimationControllers/Dead.controller");
    ModifyEvent(-2, 45, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-45
	jyx2_ReplaceSceneObject("", "NPC/少林弟子45", "");
    ModifyEvent(-2, 46, 0, 0, -1, -1, -1, 5446, 5446, 5446, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-46
	jyx2_SwitchRoleAnimation("NPC/少林弟子46","Assets/BuildSource/AnimationControllers/Dead.controller");
	--峨嵋
    ModifyEvent(-2, 48, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-48
    ModifyEvent(-2, 51, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-51
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子48", "");
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子51", "");
    ModifyEvent(-2, 52, 0, 0, -1, -1, -1, 5436, 5436, 5436, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-52
    ModifyEvent(-2, 53, 0, 0, -1, -1, -1, 5438, 5438, 5438, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-53
	jyx2_SwitchRoleAnimation("NPC/峨嵋弟子52","Assets/BuildSource/AnimationControllers/Dead-FaceUp.controller");
	jyx2_SwitchRoleAnimation("NPC/峨嵋弟子53","Assets/BuildSource/AnimationControllers/Dead-FaceUp.controller");
    ModifyEvent(-2, 54, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-54
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子54", "");
    ModifyEvent(-2, 55, 0, 0, -1, -1, -1, 5436, 5436, 5436, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-55
	jyx2_SwitchRoleAnimation("NPC/峨嵋弟子55","Assets/BuildSource/AnimationControllers/Dead-FaceUp.controller");
    ModifyEvent(-2, 56, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-56
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子56", "");
	--武当
    ModifyEvent(-2, 58, 0, 0, -1, -1, -1, 5442, 5442, 5442, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-58
	jyx2_SwitchRoleAnimation("NPC/武当弟子58","Assets/BuildSource/AnimationControllers/Dead-FaceUp.controller");
    ModifyEvent(-2, 59, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-59
	jyx2_ReplaceSceneObject("", "NPC/武当弟子59", "");
    ModifyEvent(-2, 60, 0, 0, -1, -1, -1, 5440, 5440, 5440, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-60
	jyx2_SwitchRoleAnimation("NPC/武当弟子60","Assets/BuildSource/AnimationControllers/Dead-FaceUp.controller");
    ModifyEvent(-2, 61, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-61
	jyx2_ReplaceSceneObject("", "NPC/武当弟子61", "");
    ModifyEvent(-2, 62, 0, 0, -1, -1, -1, 5442, 5442, 5442, -2, -2, -2);--by fanyu|改变贴图，战斗胜利。场景11-62
	jyx2_SwitchRoleAnimation("NPC/武当弟子62","Assets/BuildSource/AnimationControllers/Dead-FaceUp.controller");
    ModifyEvent(-2, 63, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗胜利，贴图消失。场景11-63
	jyx2_ReplaceSceneObject("", "NPC/武当弟子63", "");
    LightScence();
    Talk(0, "如何？现在肯安静地听我说了吧。事情的经过是这样的……如此如此…………这般这般…………总之，一切的阴谋，都是成昆那奸贼所计划的。所以我说呢，你们两方还是握手言和吧，反正明教杀过六大派的人，六大派也杀过明教的人，大家半斤八两，差不了多少，就都罢手吧。", "talkname0", 1);
    Talk(8, "话都是你在说的，是不是真的，我们怎么知道。", "talkname8", 0);
    Talk(6, "哼！技不如人，说这么多做什么，走吧！", "talkname6", 0);
    Talk(70, "阿弥陀佛，盼少侠以后多所规劝引导，总要使明教改邪归正，少做坏事。我们走吧。", "talkname70", 0);

    DarkScence();
    ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|贴图消失。场景11-以下都是
	jyx2_ReplaceSceneObject("", "NPC/hetaichong", "");
    ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/tangwenliang", "");
    ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/xuanci", "");
    ModifyEvent(-2, 10, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/miejue", "");
	--昆仑
    ModifyEvent(-2, 11, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 12, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 13, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 14, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 15, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 16, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 17, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 18, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 19, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子11", "");
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子12", "");
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子13", "");
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子14", "");
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子15", "");
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子16", "");
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子17", "");
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子18", "");
	jyx2_ReplaceSceneObject("", "NPC/昆仑弟子19-死", "");
	--崆峒
    ModifyEvent(-2, 20, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 21, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 22, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 23, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 24, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 25, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 26, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 27, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 28, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子20", "");
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子21", "");
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子22", "");
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子23", "");
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子24", "");
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子25", "");
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子26", "");
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子27", "");
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子28-死", "");
	--华山
    ModifyEvent(-2, 29, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 30, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 31, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 32, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 33, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 34, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 35, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/华山弟子29", "");
	jyx2_ReplaceSceneObject("", "NPC/华山弟子30", "");
	jyx2_ReplaceSceneObject("", "NPC/华山弟子31", "");
	jyx2_ReplaceSceneObject("", "NPC/华山弟子32", "");
	jyx2_ReplaceSceneObject("", "NPC/华山弟子33", "");
	jyx2_ReplaceSceneObject("", "NPC/华山弟子34", "");
	jyx2_ReplaceSceneObject("", "NPC/华山弟子35", "");
	--少林
    ModifyEvent(-2, 36, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 37, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 38, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 39, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 40, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 41, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 42, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 43, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 44, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 45, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 46, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 47, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/少林弟子36", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子37", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子38", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子39", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子40", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子41", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子42", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子43", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子44", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子45", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子46", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子47-死", "");
	--峨嵋
    ModifyEvent(-2, 48, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 49, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 50, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 51, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 52, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 53, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 54, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 55, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 56, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 57, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子48", "");
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子49", "");
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子50", "");
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子51", "");
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子52", "");
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子53", "");
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子54", "");
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子55", "");
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子56", "");
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子57-死", "");
	--武当
    ModifyEvent(-2, 58, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 59, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 60, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 61, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 62, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 63, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 64, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/武当弟子58", "");
	jyx2_ReplaceSceneObject("", "NPC/武当弟子59", "");
	jyx2_ReplaceSceneObject("", "NPC/武当弟子60", "");
	jyx2_ReplaceSceneObject("", "NPC/武当弟子61", "");
	jyx2_ReplaceSceneObject("", "NPC/武当弟子62", "");
	jyx2_ReplaceSceneObject("", "NPC/武当弟子63", "");
	jyx2_ReplaceSceneObject("", "NPC/武当弟子64", "");
	
    ModifyEvent(-2, 65, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 66, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 67, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 68, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 69, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 70, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/崆峒弟子65-死", "");
	jyx2_ReplaceSceneObject("", "NPC/峨嵋弟子66-死", "");
	jyx2_ReplaceSceneObject("", "NPC/武当弟子68-死", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子69-死", "");
	
	--明教
    ModifyEvent(-2, 79, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 80, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 81, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 82, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 83, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 84, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/明教弟子79", "");
	jyx2_ReplaceSceneObject("", "NPC/明教弟子80", "");
	jyx2_ReplaceSceneObject("", "NPC/明教弟子81", "");
	jyx2_ReplaceSceneObject("", "NPC/明教弟子82", "");
	jyx2_ReplaceSceneObject("", "NPC/明教弟子83", "");
	jyx2_ReplaceSceneObject("", "NPC/明教弟子84", "");
	
    ModifyEvent(-2, 96, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 97, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 98, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	
    ModifyEvent(-2, 99, 0, 0, -1, -1, 89, -1, -1, -1, -2, -2, -2);--by fanyu|启动脚本-89。场景11-99
    ModifyEvent(-2, 3, 1, 1, 83, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-83改变对话。场景11-3
    ModifyEvent(-2, 4, 1, 1, 84, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-84改变对话。场景11-4
    ModifyEvent(-2, 5, 1, 1, 85, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-85改变对话。场景11-5
    ModifyEvent(-2, 6, 1, 1, 88, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-88改变对话。场景11-6
    ModifyEvent(-2, 77, 1, 1, 87, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-87改变对话。场景11-77
    ModifyEvent(-2, 78, 1, 1, 87, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-87改变对话。场景11-78
    SetScencePosition2(29, 34);
	jyx2_MovePlayer("placeholder","Level/Dynamic");
    LightScence();

    Talk(14, "小子，你还不赖嘛！", "talkname14", 0);
    Talk(10, "蝠王，不可无礼。明教全教下上，叩谢少侠护教救命的大恩！", "talkname10", 0);
    Talk(0, "快别这么说，仗义行侠，本就是我辈中人应当做的。况且，这次也因为我的鲁莽，害你们陷入苦战。", "talkname0", 1);
    Talk(12, "哪里，是我们没问清楚。", "talkname12", 0);
    Talk(14, "你们也别这么客套了。这样吧，大家听我一言，我说呢，这位少侠武功盖世义薄云天，于本教有存亡续绝的大恩。咱们拥立他为本教第三十四任教主。由他来做教主，总比你来做教主好吧，杨左使，你说对不对啊。", "talkname14", 0);
    Talk(11, "是啊！这位少侠来做我们教主，也比你韦一笑来做好的多。", "talkname11", 0);
    Talk(14, "你……", "talkname14", 0);
    Talk(10, "你们两个别在那里吵了，丢人现眼的。", "talkname10", 0);
    Talk(0, "不，不，在下我虽然一直梦想能当个什么掌门教主的。但是我仍有要事在身，且跟你们是不同世界的人，所以你们还是另觅他人吧。", "talkname0", 1);
    Talk(10, "少侠如果一直推却不肯，我们明教恐怕又要为此争闹不休，四分五裂，到时又会被其他门派围攻，导致灭亡了。", "talkname10", 0);
    Talk(0, "我这里有封阳教主的遗书，上面提到要谢逊谢法王暂代教主之职。我想你们还是先找到他再说吧。", "talkname0", 1);
    Talk(10, "哦！真是如此，来人啊！即刻通令下去，全力寻找谢法王。", "talkname10", 0);
    Talk(0, "另外还有一件事拜托你们，你们明教中是否有一本叫《倚天屠龙记》的书？是否能借我一下。", "talkname0", 1);
    Talk(10, "有的。阳教主曾对我提过，我们教中有一本奇书，是我明教镇教之宝，皆由历代教主相传而下。", "talkname10", 0);
    Talk(0, "是吗？真的有。皇天不负苦心人，打了这么多场仗，终于让我找到了。", "talkname0", 1);
    Talk(10, "可是，这本书因为阳教主的突然消失，其下落也无人知晓了。", "talkname10", 0);
    Talk(12, "少侠别担心，等我们找到谢法王，整顿好教务后，必当全力搜寻此书的下落。若有消息，定当告之少侠。", "talkname12", 0);
    Talk(0, "好吧，也只有这样了。我有空也会帮你们找找谢前辈，在下这就告辞了。", "talkname0", 1);
    Talk(10, "这里有块铁焰令，是我明教的信物。少侠在江湖上行走，若有什么困难需要我明教帮忙的时候，只须出示这块铁焰令，我明教全体教众一定全力相助。", "talkname10", 0);
    AddItem(190, 1);
    AddEthics(10);
    AddRepute(15);
do return end;
