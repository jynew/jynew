if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "江湖上的人做事極為隱密，有時將一些秘密寫在書上，為了怕被別人發現，使用了特殊的藥水，讓你看不見．這時必須要用火烤，才顯現的出．我這裡有個燭台，你若有拿到奇怪的書時，可拿到我這來用火烤烤看．記得一件事，通常最普通的書籍反而是拿來做為其掩飾秘密的最佳工具．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
