SetScenceMap(-2, 1, 29, 22, 0);--by fanyu|明教地道出现，贴图变化。场景12-编号12
SetScenceMap(-2, 1, 29, 21, 2898);--by fanyu|明教地道出现，贴图变化。场景12-编号12
jyx2_FixMapObject("明教地道入口出现",1);
jyx2_ReplaceSceneObject("", "Dynamic/didaoEntrance", "1"); 
jyx2_ReplaceSceneObject("", "Dynamic/Leave2", "1"); 
ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
do return end;
