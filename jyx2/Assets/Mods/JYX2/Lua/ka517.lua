SetScenceMap(-2, 1, 25, 14, 2916);--by fanyu 改变贴图 场景28-15
SetScenceMap(-2, 1, 25, 15, 4866);--by fanyu 改变贴图 场景28-15
jyx2_FixMapObject("移动蒲团",1);
Talk(0, "咦！这蒲团下居然藏有一本书，运气真好。", "talkname0", 1);
SetScenceMap(-2, 1, 25, 15, 0);--by fanyu 改变贴图 场景28-15
jyx2_ReplaceSceneObject("","Dynamic/洗髓经","");
AddItem(44, 1);
ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移除此事件 场景28-15
do return end;
