if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "资质好坏并没有关系，只要你肯用心练武，一样可以成为大侠。桃花岛的郭靖就是一例，虽然资质不好，一样成为一代大侠。而且有时候很好用的功夫反而要资质不好的人才练的成呢。例如老顽童周伯通有套武功“左右互搏之术”，修练后可连续攻击两次。而修练这门功夫的条件就是资质不能太好。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
