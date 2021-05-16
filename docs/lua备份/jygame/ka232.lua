Talk(0, "好啊，你們居然在這裡計劃要殺人，虧你們還是同盟的幫派．", "talkname0", 1);
Talk(84, "小子，偷聽了我們的計劃，只好殺了你滅口．怪不得我們了．上！", "talkname84", 0);
if TryBattle(42) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 14, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 15, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 16, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
jyx2_ReplaceSceneObject("", "NPC/songshandizi3 (1)", "");--嵩山弟子打败离开
jyx2_ReplaceSceneObject("", "NPC/songshandizi2 (1)", "");--嵩山弟子打败离开
jyx2_ReplaceSceneObject("", "NPC/songshandizi1 (1)", "");--嵩山弟子打败离开
jyx2_ReplaceSceneObject("", "NPC/songshandizi3 (2)", "");--嵩山弟子打败离开
jyx2_ReplaceSceneObject("", "NPC/songshandizi3 (2)", "");--嵩山弟子打败离开
jyx2_ReplaceSceneObject("", "NPC/songshandizi3 (2)", "");--嵩山弟子打败离开
    LightScence();
    Add3EventNum(27, 0, 0, 0, 37)
    AddEthics(3);
    AddRepute(1);
do return end;
