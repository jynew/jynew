Talk(0, "在下云游各方，途经昆仑，见此三圣坳绿草如茵，忍不住前来欣赏一番，并顺道拜会人称“铁琴先生”的何掌门。", "talkname0", 1);
Talk(7, "你是打从中原来的吧！欣赏完了就赶紧离去吧。最近西域已成多事之地，小心惹上祸端。", "talkname7", 0);
Talk(0, "是吗？那我可得小心点。在下告退了。", "talkname0", 1);
SetScenceMap(-2, 1, 18, 24, 0);
SetScenceMap(-2, 1, 17, 24, 1832);
SetScenceMap(-2, 1, 19, 24, 1836);
jyx2_FixMapObject("昆仑派开门",1);
ModifyEvent(-2, -2, -2, -2, 164, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
