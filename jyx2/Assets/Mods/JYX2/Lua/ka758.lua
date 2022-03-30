if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "想要在这武林中存活下去，必须拥有过人的功夫才行。而想要在短期间就拥有高深的功夫，则必须要靠辛勤的“练功”才行。可以练功的地方虽多，但稍有不慎，你就要去见阎罗王了。所以要慎选练功之地。我知道有两个地方可以好好练功。分别是两个老人家住的地方。初期的话可以到灵蛇岛找那老太婆打架，到了中期则可以到百花谷找老顽童讨教。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
