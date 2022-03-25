ScenceFromTo(30, 28, 30, 14);
jyx2_CameraFollow("Level/NPC/modaxiansheng");
Talk(20, "还请费兄回去告诉左掌门，五岳并派之事，我衡山派是不会答应的。", "talkname20", 0);
Talk(84, "莫掌门还请三思，五岳合并之后，对我五岳剑派是大有好处的。到时我五岳派就可和少林、武当各大派相庭抗礼了。", "talkname84", 4);
Talk(20, "衡山派百年基业，还不想断送在我莫大手里。费兄还请回吧！", "talkname20", 0);
Talk(84, "本来我是不想提的，刘师兄此次金盆洗手，是跟魔教有关吧？左盟主已知道其中曲折了，但顾全五岳同盟之情……所以还是请莫掌门三思，届时还望光临嵩山，参加五岳同盟大会。", "talkname84", 4);
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
jyx2_CameraFollowPlayer();
do return end;
