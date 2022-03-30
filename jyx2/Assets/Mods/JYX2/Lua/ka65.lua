if UseItem(191) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(191, -1);
    Talk(0, "谢前辈，这是成昆的项上人头。成昆作恶多端已遭天谴。", "talkname0", 1);
    Talk(13, "是吗？哈！哈！成昆啊，成昆！你作恶多端终遭天谴，但，可惜啊，可惜！我不能亲手杀了你。", "talkname13", 0);
    Talk(0, "成昆一事已了，谢大侠还是尽快回到中土，以免明教四分五裂。", "talkname0", 1);
    Talk(13, "少侠为我明教付出许多，谢某感激不尽。待我将这料里完毕后，定当赶回明教。到时还望少侠前来我明教作客。唉，成昆已死，这把屠龙刀我留着还有什么用呢？屠龙刀就送给你好了。", "talkname13", 0);
    AddItem(117, 1);
    ModifyEvent(-2, -2, -2, -2, 66, -1, -2, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 3, -2, -2, -1, -1, 67, -2, -2, -2, -2, -2, -2);

do return end;
