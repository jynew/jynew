if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "相传唐太宗贞观年间曾派兵攻下了西域大国高昌，并强迫他们接受汉化。谁知高昌国人很有骨气，并不接受汉化，且视唐太宗送来的中国文物为蔽屣。传说几年前一批高昌国后代的勇士集结来到中原，抢走了“十四天书”中的《白马啸西风》一书做为对汉人的报复。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
