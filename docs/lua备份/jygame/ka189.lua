ScenceFromTo(30, 28, 30, 14);
Talk(20, "還請費兄回去告訴左掌門，五嶽併派之事，我衡山派是不會答應的．", "talkname20", 0);
Talk(84, "莫掌門還請三思，五嶽合併之後，對我五嶽劍派是大有好處的．到時我五嶽派就可和少林，武當各大派相庭抗禮了．", "talkname84", 4);
Talk(20, "衡山派百年基業，還不想斷送在我莫大手裡．費兄還請回吧！", "talkname20", 0);
Talk(84, "本來我是不想提的，劉師兄此次金盆洗手，是跟魔教有關吧？左盟主已知道其中曲折了，但顧全五嶽同盟之情．．．所以還是請莫掌門三思，屆時還望光臨嵩山，參加五獄同盟大會．", "talkname84", 4);
Talk(20, "送客！", "talkname20", 0);
DarkScence();
jyx2_ReplaceSceneObject("", "NPC/songshandizi1", "");--嵩山弟子出门
jyx2_ReplaceSceneObject("", "NPC/songshandizi2", "");--嵩山弟子出门
jyx2_ReplaceSceneObject("", "NPC/songshandizi3", "");--嵩山弟子出门
jyx2_ReplaceSceneObject("", "NPC/songshandizi3 (1)", "1");--嵩山弟子出门
jyx2_ReplaceSceneObject("", "NPC/songshandizi2 (1)", "1");--嵩山弟子出门
jyx2_ReplaceSceneObject("", "NPC/songshandizi1 (1)", "1");--嵩山弟子出门
jyx2_ReplaceSceneObject("", "NPC/songshandizi3 (2)", "1");--嵩山弟子出门
jyx2_ReplaceSceneObject("", "NPC/songshandizi3 (2)", "1");--嵩山弟子出门
jyx2_ReplaceSceneObject("", "NPC/songshandizi3 (2)", "1");--嵩山弟子出门
ModifyEvent(-2, 6, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, 7, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, 8, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, 14, 1, 1, 232, -1, -1, 5208, 5208, 5208, -2, 49, 53);
ModifyEvent(-2, 15, 1, 1, 232, -1, -1, 5202, 5202, 5202, -2, 48, 53);
ModifyEvent(-2, 16, 1, 1, 232, -1, -1, 5202, 5202, 5202, -2, 48, 54);
ModifyEvent(-2, 22, -2, -2, -1, -1, 225, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, 23, -2, -2, -1, -1, 226, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, 24, -2, -2, -1, -1, 227, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, 25, -2, -2, -1, -1, 228, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, 26, -2, -2, -1, -1, 229, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, 27, -2, -2, -1, -1, 230, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, 28, -2, -2, -1, -1, 231, -2, -2, -2, -2, -2, -2);
LightScence();
ScenceFromTo(30, 14, 30, 28);
do return end;
