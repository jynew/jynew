Talk(80, "大侠，大侠，请高抬贵手，不要杀我，我可以跟你讲个秘密：阳教主常常在房间中无缘无故消失不见……本教教主都会仙术，你要小心。", "talkname80", 0);
Talk(0, "我不会杀你的，快去包扎包扎。", "talkname0", 1);
--eaphone at 2021/6/2: 地道门打开应该在事件76做。厨子对话只是提供线索, 注释以下3行代码，移动到事件76里
--jyx2_ReplaceSceneObject("", "Bake/Static/Door/Door_06", "");--地道打开
--jyx2_ReplaceSceneObject("", "Bake/Static/Door/Door_07", "");--地道打开
--jyx2_ReplaceSceneObject("", "Bake/Static/Door/Door_08", "");--地道打开
-- jyx2_ReplaceSceneObject("", "NPC/chuzi", "");--厨子去包扎

do return end;
