Talk(100, "这房间里飘出一股香水味，这个味道好熟悉啊，进去看看。");
DarkScence();
jyx2_FixMapObject("万烛山庄开门", 1);
jyx2_ReplaceSceneObject("", "NPC/金露", "1");
ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
LightScence();
PlayMusic(3);
do return end;
