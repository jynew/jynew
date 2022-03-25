if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "江湖上的人做事极为隐密，有时将一些秘密写在书上，为了怕被别人发现，使用了特殊的药水，让你看不见。这时必须要用火烤，才显现的出。我这里有个烛台，你若有拿到奇怪的书时，可拿到我这来用火烤烤看。记得一件事，通常最普通的书籍反而是拿来做为其掩饰秘密的最佳工具。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
