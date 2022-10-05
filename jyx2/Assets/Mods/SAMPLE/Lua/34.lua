if InTeam(30) == true then goto label0 end;
    Talk(132, "愿佛祖保佑您。");
    do return end;
::label0::
    jyx2_ReplaceSceneObject("", "NPC/虚寂 (2)", "1");
    Talk(132, "前阵子我弄丢一本经书，看完后就忘记摆哪去了。虚师弟，你能不能帮我找找？");
    Talk(30, "没问题，师兄稍等，我去给你找找。");
    jyx2_ReplaceSceneObject("", "NPC/虚寂 (2)", "");
    ModifyEvent(-2, 8, -2, -2, 316, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, -2, -2, -2, 317, 318, -1, -2, -2, -2, -2, -2, -2);
do return end;
