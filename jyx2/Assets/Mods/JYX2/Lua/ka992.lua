Talk(0, "龙姑娘，请你先回古墓，若有需要你帮忙时，我再去找你。", "talkname0", 1);
Leave(59);
ModifyEvent(18, 0, 1, 1, 993, -1, -1, 6068, 6068, 6068, 0, -2, -2);
jyx2_ReplaceSceneObject("18","NPC/小龙女","1");
jyx2_FixMapObject("古墓开门",1);
do return end;
