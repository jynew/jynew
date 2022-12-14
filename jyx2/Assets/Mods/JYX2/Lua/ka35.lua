ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 人物保留，但是不在当前位置显示 场景24-编号1
jyx2_FixMapObject("苗人凤受伤后",1);
ModifyEvent(-2, 8, 1, -2, 31, 32, -2, 5214, 5214, 5214, -2, -2, -2);--by fanyu 启动脚本31,32，改变贴图 场景24-编号8
jyx2_SwitchRoleAnimation("NPC/miaorenfeng","Assets/BuildSource/AnimationControllers/备份/MiaorenfengCovereyesController.controller");
do return end;
