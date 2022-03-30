Talk(0, "莫非这二人竟是阳顶天夫妇？这里有封信，看看写些什么：", "talkname0", 1);
Talk(107, "今余神功第四层初成，即悉成昆之事，血气翻涌不克自制，真力将散，行当大归。天也命也，复何如耶？今余命在旦夕，有负衣教主重托，实为本教罪人。盼夫人持余亲笔遗书，召聚众人，令谢逊暂摄副教主之位，处分本教重务……", "talkname107", 0);
Talk(0, "原来阳教主是在练功时，因知悉成昆的事后，不慎走火入魔了。而阳教主夫人……这里有把匕首，看来是因愧疚而自杀了。这个羊皮卷，大概就是他当时在练的“神功”吧。", "talkname0", 1);
AddItem(92, 1);
ModifyEvent(-2, 0, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
ModifyEvent(-2, 1, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
SetScenceMap(11, 1, 29, 47, 0);
SetScenceMap(11, 1, 28, 46, 2050);--by fanyu|门打开。场景11-编号1
SetScenceMap(11, 1, 30, 46, 2054);--by fanyu|门打开。场景11-编号1
SetScenceMap(11, 1, 30, 47, 2052);--by fanyu|门打开。场景11-编号1
SetScenceMap(11, 1, 28, 47, 2048);--by fanyu|门打开。场景11-编号1
jyx2_FixMapObject("光明顶开门",1);
ModifyEvent(11, 0, 0, 0, -1, -1, 81, -1, -1, -1, -2, -2, -2);--by fanyu|启动81号脚本。场景11-编号0

ModifyEvent(11, 1, 1, 1, -1, -1, -1, 5316, 5316, 5316, -2, -2, -2);--by fanyu|改变贴图，出现人物。场景11-以下都是
ModifyEvent(11, 2, 1, 1, -1, -1, -1, 5330, 5330, 5330, -2, -2, -2);
jyx2_ReplaceSceneObject("11", "NPC/weiyixiao_1", "1");--韦一笑出现    
jyx2_ReplaceSceneObject("11", "NPC/yintianzheng_1", "1"); --殷天正出现
ModifyEvent(11, 3, 1, 1, -2, -2, -2, 5452, 5452, 5452, -2, -2, -2);
ModifyEvent(11, 6, 1, 1, -2, -2, -2, 5334, 5334, 5334, -2, -2, -2);
jyx2_ReplaceSceneObject("11", "NPC/yangxiao", "1");
jyx2_ReplaceSceneObject("11", "NPC/fanyao", "1"); 

ModifyEvent(11, 65, 1, 1, -1, -1, -1, 5430, 5430, 5430, -2, -2, -2);--崆峒死人
jyx2_ReplaceSceneObject("11", "NPC/崆峒弟子65-死", "1");
ModifyEvent(11, 66, 1, 1, -1, -1, -1, 5436, 5436, 5436, -2, -2, -2);--峨眉死人
jyx2_ReplaceSceneObject("11", "NPC/峨嵋弟子66-死", "1");
ModifyEvent(11, 67, 1, 1, -1, -1, -1, 5432, 5432, 5432, -2, -2, -2);--昆仑死人
ModifyEvent(11, 68, 1, 1, -1, -1, -1, 5440, 5440, 5440, -2, -2, -2);--武当死人
jyx2_ReplaceSceneObject("11", "NPC/武当弟子68-死", "1");
ModifyEvent(11, 69, 1, 1, -1, -1, -1, 5444, 5444, 5444, -2, -2, -2);--少林死人
jyx2_ReplaceSceneObject("11", "NPC/少林弟子69-死", "1");
ModifyEvent(11, 70, 1, 1, -1, -1, -1, 5432, 5432, 5432, -2, -2, -2);--昆仑死人

ModifyEvent(11, 7, 1, 1, -2, -2, -2, 5348, 5348, 5348, -2, -2, -2);
jyx2_ReplaceSceneObject("11", "NPC/hetaichong", "1");--何太冲
ModifyEvent(11, 11, 1, 1, -2, -2, -2, 5412, 5412, 5412, -2, -2, -2);--昆仑派
ModifyEvent(11, 12, 1, 1, -2, -2, -2, 5412, 5412, 5412, -2, -2, -2);
ModifyEvent(11, 13, 1, 1, -2, -2, -2, 5412, 5412, 5412, -2, -2, -2);
ModifyEvent(11, 14, 1, 1, -2, -2, -2, 5412, 5412, 5412, -2, -2, -2);
ModifyEvent(11, 15, 1, 1, -2, -2, -2, 5412, 5412, 5412, -2, -2, -2);
ModifyEvent(11, 16, 1, 1, -2, -2, -2, 5412, 5412, 5412, -2, -2, -2);
ModifyEvent(11, 17, 1, 1, -2, -2, -2, 5412, 5412, 5412, -2, -2, -2);
ModifyEvent(11, 18, 1, 1, -2, -2, -2, 5412, 5412, 5412, -2, -2, -2);
ModifyEvent(11, 19, 1, 1, -2, -2, -2, 5434, 5434, 5434, -2, -2, -2);--昆仑死人
jyx2_ReplaceSceneObject("11", "NPC/昆仑弟子11", "1");
jyx2_ReplaceSceneObject("11", "NPC/昆仑弟子12", "1");
jyx2_ReplaceSceneObject("11", "NPC/昆仑弟子13", "1");
jyx2_ReplaceSceneObject("11", "NPC/昆仑弟子14", "1");
jyx2_ReplaceSceneObject("11", "NPC/昆仑弟子15", "1");
jyx2_ReplaceSceneObject("11", "NPC/昆仑弟子16", "1");
jyx2_ReplaceSceneObject("11", "NPC/昆仑弟子17", "1");
jyx2_ReplaceSceneObject("11", "NPC/昆仑弟子18", "1");
jyx2_ReplaceSceneObject("11", "NPC/昆仑弟子19-死", "1");

ModifyEvent(11, 8, 1, 1, -2, -2, -2, 5364, 5364, 5364, -2, -2, -2);--崆峒派
jyx2_ReplaceSceneObject("11", "NPC/tangwenliang", "1");--唐文亮
ModifyEvent(11, 20, 1, 1, -2, -2, -2, 5404, 5404, 5404, -2, -2, -2);
ModifyEvent(11, 21, 1, 1, -2, -2, -2, 5404, 5404, 5404, -2, -2, -2);
ModifyEvent(11, 22, 1, 1, -2, -2, -2, 5404, 5404, 5404, -2, -2, -2);
ModifyEvent(11, 23, 1, 1, -2, -2, -2, 5404, 5404, 5404, -2, -2, -2);
ModifyEvent(11, 24, 1, 1, -2, -2, -2, 5404, 5404, 5404, -2, -2, -2);
ModifyEvent(11, 25, 1, 1, -2, -2, -2, 5404, 5404, 5404, -2, -2, -2);
ModifyEvent(11, 26, 1, 1, -2, -2, -2, 5404, 5404, 5404, -2, -2, -2);
ModifyEvent(11, 27, 1, 1, -2, -2, -2, 5404, 5404, 5404, -2, -2, -2);
ModifyEvent(11, 28, 1, 1, -2, -2, -2, 5428, 5428, 5428, -2, -2, -2);--崆峒死人
jyx2_ReplaceSceneObject("11", "NPC/崆峒弟子20", "1");
jyx2_ReplaceSceneObject("11", "NPC/崆峒弟子21", "1");
jyx2_ReplaceSceneObject("11", "NPC/崆峒弟子22", "1");
jyx2_ReplaceSceneObject("11", "NPC/崆峒弟子23", "1");
jyx2_ReplaceSceneObject("11", "NPC/崆峒弟子24", "1");
jyx2_ReplaceSceneObject("11", "NPC/崆峒弟子25", "1");
jyx2_ReplaceSceneObject("11", "NPC/崆峒弟子26", "1");
jyx2_ReplaceSceneObject("11", "NPC/崆峒弟子27", "1");
jyx2_ReplaceSceneObject("11", "NPC/崆峒弟子28-死", "1");

ModifyEvent(11, 29, 1, 1, -2, -2, -2, 5182, 5182, 5182, -2, -2, -2);--华山派
ModifyEvent(11, 30, 1, 1, -2, -2, -2, 5182, 5182, 5182, -2, -2, -2);
ModifyEvent(11, 31, 1, 1, -2, -2, -2, 5182, 5182, 5182, -2, -2, -2);
ModifyEvent(11, 32, 1, 1, -2, -2, -2, 5182, 5182, 5182, -2, -2, -2);
ModifyEvent(11, 33, 1, 1, -2, -2, -2, 5182, 5182, 5182, -2, -2, -2);
ModifyEvent(11, 34, 1, 1, -2, -2, -2, 5182, 5182, 5182, -2, -2, -2);
ModifyEvent(11, 35, 1, 1, -2, -2, -2, 5182, 5182, 5182, -2, -2, -2);
jyx2_ReplaceSceneObject("11", "NPC/华山弟子29", "1");
jyx2_ReplaceSceneObject("11", "NPC/华山弟子30", "1");
jyx2_ReplaceSceneObject("11", "NPC/华山弟子31", "1");
jyx2_ReplaceSceneObject("11", "NPC/华山弟子32", "1");
jyx2_ReplaceSceneObject("11", "NPC/华山弟子33", "1");
jyx2_ReplaceSceneObject("11", "NPC/华山弟子34", "1");
jyx2_ReplaceSceneObject("11", "NPC/华山弟子35", "1");

ModifyEvent(11, 9, 1, 1, -2, -2, -2, 5378, 5378, 5378, -2, -2, -2);--少林派
jyx2_ReplaceSceneObject("11", "NPC/xuanci", "1");--玄慈
ModifyEvent(11, 36, 1, 1, -2, -2, -2, 5426, 5426, 5426, -2, -2, -2);
ModifyEvent(11, 37, 1, 1, -2, -2, -2, 5426, 5426, 5426, -2, -2, -2);
ModifyEvent(11, 38, 1, 1, -2, -2, -2, 5426, 5426, 5426, -2, -2, -2);
ModifyEvent(11, 39, 1, 1, -2, -2, -2, 5426, 5426, 5426, -2, -2, -2);
ModifyEvent(11, 40, 1, 1, -2, -2, -2, 5426, 5426, 5426, -2, -2, -2);
ModifyEvent(11, 41, 1, 1, -2, -2, -2, 5426, 5426, 5426, -2, -2, -2);
ModifyEvent(11, 42, 1, 1, -2, -2, -2, 5426, 5426, 5426, -2, -2, -2);
ModifyEvent(11, 43, 1, 1, -2, -2, -2, 5426, 5426, 5426, -2, -2, -2);
ModifyEvent(11, 44, 1, 1, -2, -2, -2, 5426, 5426, 5426, -2, -2, -2);
ModifyEvent(11, 45, 1, 1, -2, -2, -2, 5426, 5426, 5426, -2, -2, -2);
ModifyEvent(11, 46, 1, 1, -2, -2, -2, 5426, 5426, 5426, -2, -2, -2);
ModifyEvent(11, 47, 1, 1, -2, -2, -2, 5444, 5444, 5444, -2, -2, -2);
jyx2_ReplaceSceneObject("11", "NPC/少林弟子36", "1");
jyx2_ReplaceSceneObject("11", "NPC/少林弟子37", "1");
jyx2_ReplaceSceneObject("11", "NPC/少林弟子38", "1");
jyx2_ReplaceSceneObject("11", "NPC/少林弟子39", "1");
jyx2_ReplaceSceneObject("11", "NPC/少林弟子40", "1");
jyx2_ReplaceSceneObject("11", "NPC/少林弟子41", "1");
jyx2_ReplaceSceneObject("11", "NPC/少林弟子42", "1");
jyx2_ReplaceSceneObject("11", "NPC/少林弟子43", "1");
jyx2_ReplaceSceneObject("11", "NPC/少林弟子44", "1");
jyx2_ReplaceSceneObject("11", "NPC/少林弟子45", "1");
jyx2_ReplaceSceneObject("11", "NPC/少林弟子46", "1");
jyx2_ReplaceSceneObject("11", "NPC/少林弟子47-死", "1");

ModifyEvent(11, 10, 1, 1, -2, -2, -2, 5362, 5362, 5362, -2, -2, -2);--峨眉派
jyx2_ReplaceSceneObject("11", "NPC/miejue", "1");--峨嵋派
ModifyEvent(11, 48, 1, 1, -2, -2, -2, 5402, 5402, 5402, -2, -2, -2);
ModifyEvent(11, 49, 1, 1, -2, -2, -2, 5402, 5402, 5402, -2, -2, -2);
ModifyEvent(11, 50, 1, 1, -2, -2, -2, 5402, 5402, 5402, -2, -2, -2);
ModifyEvent(11, 51, 1, 1, -2, -2, -2, 5402, 5402, 5402, -2, -2, -2);
ModifyEvent(11, 52, 1, 1, -2, -2, -2, 5402, 5402, 5402, -2, -2, -2);
ModifyEvent(11, 53, 1, 1, -2, -2, -2, 5402, 5402, 5402, -2, -2, -2);
ModifyEvent(11, 54, 1, 1, -2, -2, -2, 5402, 5402, 5402, -2, -2, -2);
ModifyEvent(11, 55, 1, 1, -2, -2, -2, 5402, 5402, 5402, -2, -2, -2);
ModifyEvent(11, 56, 1, 1, -2, -2, -2, 5402, 5402, 5402, -2, -2, -2);
ModifyEvent(11, 57, 1, 1, -2, -2, -2, 5436, 5436, 5436, -2, -2, -2);
jyx2_ReplaceSceneObject("11", "NPC/峨嵋弟子48", "1");
jyx2_ReplaceSceneObject("11", "NPC/峨嵋弟子49", "1");
jyx2_ReplaceSceneObject("11", "NPC/峨嵋弟子50", "1");
jyx2_ReplaceSceneObject("11", "NPC/峨嵋弟子51", "1");
jyx2_ReplaceSceneObject("11", "NPC/峨嵋弟子52", "1");
jyx2_ReplaceSceneObject("11", "NPC/峨嵋弟子53", "1");
jyx2_ReplaceSceneObject("11", "NPC/峨嵋弟子54", "1");
jyx2_ReplaceSceneObject("11", "NPC/峨嵋弟子55", "1");
jyx2_ReplaceSceneObject("11", "NPC/峨嵋弟子56", "1");
jyx2_ReplaceSceneObject("11", "NPC/峨嵋弟子57-死", "1");

ModifyEvent(11, 58, 1, 1, -2, -2, -2, 5392, 5392, 5392, -2, -2, -2);--武当派
ModifyEvent(11, 59, 1, 1, -2, -2, -2, 5392, 5392, 5392, -2, -2, -2);
ModifyEvent(11, 60, 1, 1, -2, -2, -2, 5392, 5392, 5392, -2, -2, -2);
ModifyEvent(11, 61, 1, 1, -2, -2, -2, 5392, 5392, 5392, -2, -2, -2);
ModifyEvent(11, 62, 1, 1, -2, -2, -2, 5392, 5392, 5392, -2, -2, -2);
ModifyEvent(11, 63, 1, 1, -2, -2, -2, 5392, 5392, 5392, -2, -2, -2);
ModifyEvent(11, 64, 1, 1, -2, -2, -2, 5392, 5392, 5392, -2, -2, -2);
jyx2_ReplaceSceneObject("11", "NPC/武当弟子58", "1");
jyx2_ReplaceSceneObject("11", "NPC/武当弟子59", "1");
jyx2_ReplaceSceneObject("11", "NPC/武当弟子60", "1");
jyx2_ReplaceSceneObject("11", "NPC/武当弟子61", "1");
jyx2_ReplaceSceneObject("11", "NPC/武当弟子62", "1");
jyx2_ReplaceSceneObject("11", "NPC/武当弟子63", "1");
jyx2_ReplaceSceneObject("11", "NPC/武当弟子64", "1");

ModifyEvent(11, 77, 1, 1, -2, -2, -2, 5302, 5302, 5302, -2, -2, -2);--明教
ModifyEvent(11, 78, 1, 1, -2, -2, -2, 5302, 5302, 5302, -2, -2, -2);
ModifyEvent(11, 79, 1, 1, -2, -2, -2, 5308, 5308, 5308, -2, -2, -2);
ModifyEvent(11, 80, 1, 1, -2, -2, -2, 5310, 5310, 5310, -2, -2, -2);
ModifyEvent(11, 81, 1, 1, -2, -2, -2, 5312, 5312, 5312, -2, -2, -2);
ModifyEvent(11, 82, 1, 1, -2, -2, -2, 5312, 5312, 5312, -2, -2, -2);
ModifyEvent(11, 83, 1, 1, -2, -2, -2, 5310, 5310, 5310, -2, -2, -2);
ModifyEvent(11, 84, 1, 1, -2, -2, -2, 5314, 5314, 5314, -2, -2, -2);
jyx2_ReplaceSceneObject("11", "NPC/明教弟子77", "1");
jyx2_ReplaceSceneObject("11", "NPC/明教弟子78", "1");
jyx2_ReplaceSceneObject("11", "NPC/明教弟子79", "1");
jyx2_ReplaceSceneObject("11", "NPC/明教弟子80", "1");
jyx2_ReplaceSceneObject("11", "NPC/明教弟子81", "1");
jyx2_ReplaceSceneObject("11", "NPC/明教弟子82", "1");
jyx2_ReplaceSceneObject("11", "NPC/明教弟子83", "1");
jyx2_ReplaceSceneObject("11", "NPC/明教弟子84", "1");
do return end;
