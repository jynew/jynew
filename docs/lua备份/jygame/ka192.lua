Talk(0, "請問這裡是華山嗎？”華山論劍”是在這裡嗎？", "talkname0", 1);
Talk(81, "不錯，承蒙江湖同道看的起我華山派，四年舉辦一次的”華山論劍”正是由我華山派主辦，只不過不是在這，而是在另一座山頭．這裡是我華山派練功修行的地方．", "talkname81", 0);
if InTeam(35) == false then goto label0 end;
    jyx2_ReplaceSceneObject("", "NPC/linghuchong", "1");-- 
    Talk(35, "各位近來好吧？", "talkname35", 1);
    Talk(81, "大師哥．", "talkname81", 0);
    Talk(35, "別叫我大師哥了，我現在不是華山派的人了，我隨這位小兄弟浪跡天涯．", "talkname35", 1);
     jyx2_ReplaceSceneObject("", "NPC/linghuchong", "");-- 
::label0::
    Talk(0, "久聞貴派掌門”君子劍”岳先生為人正直，道德崇高，不知在下能否有幸拜見？", "talkname0", 1);
    Talk(81, "我師父就在裡面，少俠請．", "talkname81", 0);
    ModifyEvent(-2, 2, -2, -2, 193, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 3, -2, -2, 194, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 4, -2, -2, 211, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 5, -2, -2, 193, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 6, -2, -2, 194, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
