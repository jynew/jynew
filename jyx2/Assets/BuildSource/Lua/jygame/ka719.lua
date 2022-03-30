if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "行走江湖，有时难免中毒，所以伙伴中能有个懂得治毒之人最好。武林中有三大解毒者，分别是毒仙王难姑，毒手药王，以及五毒教主蓝凤凰。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
