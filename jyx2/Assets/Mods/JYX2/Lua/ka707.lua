if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "通常医者对于受伤患者都可进行医疗，但如果患者受伤情形太严重，超过你的医疗能力时，由医者所进行的医疗行为将会失效。因为医者已无能力治疗他了。这时就只能先靠药物治疗，待降低其受伤程度后，再行医疗行为。所以你平时最好多准备一些药丸，不管是别人送的，或是自己自行制造皆可。不过，最好的还是即早治疗才是预防的好方法，不要等到病入膏肓时就来不及了。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
