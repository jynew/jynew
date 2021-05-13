ScenceFromTo(20, 47, 20, 39);
Talk(94, "哈！言兄，你崑崙派自許為名門正派，想不到也覬覦這”連城訣”啊．", "talkname94", 0);
Talk(78, "哼！天下人之物，天下人得之．在場的崆峒，青城，泰山，還有這些其他各大門派的高手，不也是這麼想嗎？", "talkname78", 0);
ScenceFromTo(20, 39, 20, 47);
Talk(0, "嘩！這裡可真熱鬧，想必各位都是為那”連城訣”而來的吧．但是很對不起，這東西我先定下，沒你們的份了．", "talkname0", 1);
Talk(97, "小子，找死！", "talkname97", 0);
if TryBattle(93) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    jyx2_ReplaceSceneObject("", "NPC/1", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/2", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/3", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/4", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/5", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/6", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/7", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/8", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/9", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/10", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/11", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/12", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/13", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/14", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/15", "");--天宁寺出现
    jyx2_ReplaceSceneObject("", "NPC/16", "");--天宁寺出现
    ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 10, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 11, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 12, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 13, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 14, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 15, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 16, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 17, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    LightScence();
    AddRepute(8);
do return end;
