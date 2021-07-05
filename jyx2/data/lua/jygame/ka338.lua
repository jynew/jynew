ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
jyx2_ReplaceSceneObject("", "NPC/shipotian1", "");--石破天加入队伍
ModifyEvent(40, 7, 1, 1, 14, -1, -1, 6410, 6410, 6410, 0, -2, -2);
jyx2_ReplaceSceneObject("40", "NPC/shipotian1", "1");--石破天去悦来客栈
ModifyEvent(40, 8, 1, 1, 14, -1, -1, 6412, 6412, 6412, 0, -2, -2);
print('石破天去悦来客栈了')
do return end;
