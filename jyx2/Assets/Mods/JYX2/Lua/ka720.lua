if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "通常医者对于中毒患者都可进行解毒，但如果患者中毒情形太严重，超过你的解毒能力时，由医者所进行的解毒行为将会失效。因为医者已无能力为他解毒了。这时就只能先靠药物治疗，待降低其中毒程度后，再行解毒行为。所以你平时最好多准备一些药丸，不管是别人送的，或是自己自行制造皆可。不过，最好的还是即早治疗才是预防的好方法，不要等到病入膏肓时就来不及了。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
