if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "在江湖中行走，最要緊的使自己保持在正道之上．江湖是個是非之地，一旦稍微把持不住，就很容易誤入歧途．一旦誤入歧途，則正道人士將不屑於你，可能就不會加入你的行列．所以勿因小而失大，到處亂拿東西才是．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
