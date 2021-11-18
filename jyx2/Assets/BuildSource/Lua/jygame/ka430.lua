ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除编号。场景47-编号00
ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除编号。场景47-编号01
jyx2_ReplaceSceneObject("", "NPC/一灯", ""); --一灯去百花谷
jyx2_ReplaceSceneObject("20", "NPC/yideng", "1"); 
ModifyEvent(20, 14, 1, 1, 414, -1, -1, 6152, 6152, 6152, -2, -2, -2);--by fanyu|启动脚本414，更改贴图。场景20-编号14
AddEthics(2);
do return end;
