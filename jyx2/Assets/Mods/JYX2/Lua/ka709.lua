if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "我记得金先生曾跟我提过，《天龙八部》一书是描写有关皇室，平民，荣华，富贵一类的故事。而目前武林中具有皇室背景的英雄少年有两位。一位是大理段家的公子，另一位则是姑苏慕容家的慕容复公子。尤其慕容复现在住的燕子坞内藏书丰富，或许那里有些“十四天书”的下落。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
