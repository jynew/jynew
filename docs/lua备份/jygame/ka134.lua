Talk(0, "在下雲遊各方，途經崑崙，見此三聖坳綠草如茵，忍不住前來欣賞一番，並順道拜會人稱”鐵琴先生”的何掌門．", "talkname0", 1);
Talk(7, "你是打從中原來的吧！欣賞完了就趕緊離去吧．最近西域已成多事之地，小心惹上禍端．", "talkname7", 0);
Talk(0, "是嗎？那我可得小心點．在下告退了．", "talkname0", 1);
SetScenceMap(-2, 1, 18, 24, 0);
SetScenceMap(-2, 1, 17, 24, 1832);
SetScenceMap(-2, 1, 19, 24, 1836);
jyx2_ReplaceSceneObject("", "Bake/Static/Door/Door_0242", "");--开门
jyx2_ReplaceSceneObject("", "Bake/Static/Door/Door_0243", "");--开门
ModifyEvent(-2, -2, -2, -2, 164, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
