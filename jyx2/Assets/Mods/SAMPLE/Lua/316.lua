jyx2_ReplaceSceneObject("", "NPC/虚寂 (3)", "1");
jyx2_FixMapObject("移动蒲团",1);
Talk(30, "咦！原来这本书在蒲团下面啊。");
jyx2_ReplaceSceneObject("", "Dynamic/伏虎拳", "");
AddItem(36, 1);
jyx2_ReplaceSceneObject("", "NPC/虚寂 (3)", "");
ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
