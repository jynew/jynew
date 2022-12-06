Talk(0, "铁面，你先回破庙，若有需要你时，我再去找你。", "talkname0", 1);
Leave(48);
ModifyEvent(62, 4, 1, 1, 981, -1, -1, 6378, 6378, 6378, 0, -2, -2);
jyx2_ReplaceSceneObject("62","NPC/youtanzhi","1");
jyx2_SwitchRoleAnimation("NPC/youtanzhi", "Assets/BuildSource/AnimationControllers/StandController.controller", "62");
do return end;
