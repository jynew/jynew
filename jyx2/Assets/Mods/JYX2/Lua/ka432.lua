ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除编号。场景21-编号01
ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除编号。场景21-编号02
jyx2_ReplaceSceneObject("", "NPC/zhoubotong", "");--周伯通回到百花谷
jyx2_ReplaceSceneObject("", "NPC/yinggu", "");--瑛姑回到百花谷
ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除编号。场景21-编号04
ModifyEvent(47, 0, -2, -2, 429, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动429脚本。场景47-编号00
ModifyEvent(20, 4, 1, 1, 412, -1, -1, 6154, 6154, 6154, -2, -2, -2);--by fanyu|启动412脚本，改变贴图。场景20-编号04
ModifyEvent(20, 13, 1, 1, 415, -1, -1, 6158, 6158, 6158, -2, -2, -2);--by fanyu|启动415脚本，改变贴图。场景20-编号13
jyx2_ReplaceSceneObject("20", "NPC/zhoubotong", "1");--周伯通回到百花谷
jyx2_ReplaceSceneObject("20", "NPC/yinggu", "1");--瑛姑回到百花谷
ChangeScencePic(-2, 0, 990, 994);
jyx2_ReplaceSceneObject("", "Bake/Static/stone", ""); 
do return end;
