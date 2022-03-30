if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "自来修习内功，不论是为了强身治病，还是为了做为上乘武功的根基，必当水火互济，阴阳相配。比如说练了“足少阴肾经”便当练“足少阳胆经”，少阴少阳融会调合，体力便逐步增强。若阴阳不调而相冲相克，终将走火入魔，死于非命。相传六合丁氏祖传秘方所配制的“玄冰碧火酒”具阴阳调合之功，可治疗因练功不慎而导致阴阳不调之习武之人。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
