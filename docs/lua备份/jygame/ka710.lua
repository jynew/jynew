if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "通常在客棧或是你家中休息時都可恢復體力，生命，及內力．除非你是受傷不輕或是有中毒狀況．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
