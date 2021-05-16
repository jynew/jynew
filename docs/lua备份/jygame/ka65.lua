if UseItem(191) == true then goto label0 end;
    do return end;
::label0::
    AddItem(191, -1);
    Talk(0, "謝前輩，這是成崑的項上人頭．成崑作惡多端已遭天譴．", "talkname0", 1);
    Talk(13, "是嗎？哈！哈！成崑啊，成崑！你作惡多端終遭天譴，但，可惜啊，可惜！我不能親手殺了你．　　　", "talkname13", 0);
    Talk(0, "成崑一事已了，謝大俠還是儘快回到中土，以免明教四分五裂．", "talkname0", 1);
    Talk(13, "少俠為我明教付出許多，謝某感激不盡．待我將這料裡完畢後，定當趕回明教．到時還望少俠前來我明教作客．唉，成崑已死，這把屠龍刀我留著還有什麼用呢？屠龍刀就送給你好了．", "talkname13", 0);
    GetItem(117, 1);
    ModifyEvent(-2, -2, -2, -2, 66, -1, -2, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 3, -2, -2, -1, -1, 67, -2, -2, -2, -2, -2, -2);
    jyx2_ReplaceSceneObject("", "NPC/xiexun", ""); 
do return end;
