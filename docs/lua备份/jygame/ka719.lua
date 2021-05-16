if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "行走江湖，有時難免中毒，所以夥伴中能有個懂得治毒之人最好．武林中有三大解毒者，分別是毒仙王難姑，毒手藥王，以及五毒教主藍鳳凰．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
