SetScenceMap(-2, 1, 29, 22, 0);--by fanyu|明教地道出现，贴图变化。场景12-编号12
jyx2_MovePlayer("cushion-moved","Level/Dynamic","Level/Dynamic/cushion-ori")
SetScenceMap(-2, 1, 29, 21, 2898);--by fanyu|明教地道出现，贴图变化。场景12-编号12
ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
jyx2_ReplaceSceneObject("", "FX/didaoEntrance", "1"); 
jyx2_ReplaceSceneObject("", "Triggers/Leave2", "1"); 
do return end;
