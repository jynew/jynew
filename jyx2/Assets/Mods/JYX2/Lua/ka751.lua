if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "道德有几斤重？别听南贤那老头胡扯，满嘴仁义道德的。你想想看，如果你为了道德不被减低，却不肯拿箱子里的宝物，这样你还过得了关吗？世上哪有真的圣人，重要的是你必须能在这江湖混得下去，“玩”成你的目标。所以放手去开箱子吧，顶多以后做些善事补回来不就得了。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
