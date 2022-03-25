if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "世上有很多奇妙的事是你所想不到的。例如书读多了固然可通晓许多道理，但有时太钻牛角尖反而会陷入死胡同。我知道就有位古人，将他一套神奇的武功用奇怪的文字写成，书读多的人以为是很奥妙的文章。但若是叫一个没读过书的人来看，则一目了然。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
