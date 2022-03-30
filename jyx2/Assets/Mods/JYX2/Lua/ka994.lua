Talk(0, "欧阳公子，请先回白驼山，若有需要你帮忙时，我再去找你。", "talkname0", 1);
Leave(61);
ModifyEvent(69, 0, 1, 1, 995, -1, -1, 6080, 6080, 6080, 0, -2, -2);
ModifyEvent(69, 1, 1, 1, 995, -1, -1, 6082, 6082, 6082, 0, -2, -2);
jyx2_ReplaceSceneObject("69", "NPC/欧阳克", "1");
jyx2_ReplaceSceneObject("69", "NPC/欧阳克婢女", "1");
do return end;
